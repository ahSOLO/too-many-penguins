using System.Collections;
using System.Collections.Generic;
using UnityAtoms.BaseAtoms;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent), typeof(Rigidbody), typeof(StateMachine))]
public class WorkerController : MonoBehaviour
{
    [SerializeField] private Renderer rend;
    private NavMeshAgent nav;
    private Rigidbody rb;
    private StateMachine sM;

    public IntEvent currentWeightUpdate;
    [SerializeField] private CapsuleCollider col;

    private WorkerSpawn spawnState;
    private WorkerIdle idleState;
    private WorkerFollow followState;
    private WorkerSeekResource seekResourceState;
    private WorkerHarvestResource harvestState;

    public float disablePhysicsSqredVelocityThreshold;
    public float navUpdateFrequency;
    public float maxSeekDistance;
    public float harvestDuration;
    public float maxHarvestDistance;
    public float harvestRate;
    public Resource TargetResource { get; set; }
    [SerializeField] private Vector3 spawnVelocity;
    public bool spawnComplete;

    private bool wantsToFollow;
    private bool wantsToSeekResource;
    private bool wantsToHarvestResource;
    private bool canResumeNavigation;
    private Coroutine activeCoroutine;

    private void Awake()
    {
        nav = GetComponent<NavMeshAgent>();
        rb = GetComponent<Rigidbody>();
        sM = GetComponent<StateMachine>();

        spawnState = new WorkerSpawn(this);
        idleState = new WorkerIdle(this);
        followState = new WorkerFollow(this);
        seekResourceState = new WorkerSeekResource(this);
        harvestState = new WorkerHarvestResource(this);
        
        sM.SetInitialState(spawnState);
    }

    private void Start()
    {
        sM.AddTransition(spawnState, IsGrounded, idleState);
        sM.AddTransition(idleState, () => wantsToFollow, followState);
        sM.AddTransition(idleState, () => wantsToSeekResource, seekResourceState);
        sM.AddTransition(followState, () => wantsToSeekResource, seekResourceState);
        sM.AddTransition(seekResourceState, () => !wantsToSeekResource && !wantsToHarvestResource, idleState);
        sM.AddTransition(seekResourceState, () => wantsToFollow, followState);
        sM.AddTransition(seekResourceState, () => wantsToHarvestResource, harvestState);
        sM.AddTransition(harvestState, () => !wantsToHarvestResource, idleState);
        sM.AddTransition(harvestState, () => wantsToFollow, followState);
        sM.AddTransition(harvestState, () => wantsToSeekResource, seekResourceState);
    }

    private void FixedUpdate()
    {
        var cols = Physics.OverlapCapsule(col.transform.position - new Vector3(0f, col.height / 2f, 0f), col.transform.position + new Vector3(0f, col.height / 2f, 0f), col.radius + 0.05f, LayerMask.GetMask("Player", "Ice Block", "Worker"));
        if (cols.Length > 0)
        {
            foreach (var col in cols)
            {
                PotentialCollisionCheck(col.attachedRigidbody);
            }
        }
    }

    private void Update()
    {
        if (canResumeNavigation)
        {
            ReenableNavMeshCheck();
        }
    }

    private void PotentialCollisionCheck(Rigidbody rb)
    {
        if (Vector3.Dot(rb.velocity, transform.position - rb.position) > 0)
        {
            canResumeNavigation = false;
            TogglePhysics(true);
            if (activeCoroutine != null)
            {
                StopCoroutine(activeCoroutine);
            }
            activeCoroutine = StartCoroutine(Utility.DelayedAction(() => canResumeNavigation = true, 0.6f));
        }
    }

    private bool IsGrounded()
    {
        var raycastLength = rend.bounds.extents.y + 0.1f;
        var layerMask = LayerMask.GetMask("Platform");
        return Physics.Raycast(new Vector3(transform.position.x, transform.position.y, transform.position.z), Vector3.down, raycastLength, layerMask);
    }

    public void SetSpawnVelocity()
    {
        rb.AddForce(transform.rotation * spawnVelocity, ForceMode.VelocityChange);
    }

    public void FollowPlayer()
    {
        wantsToSeekResource = false;
        wantsToFollow = true;
        wantsToHarvestResource = false;
    }

    public void SeekResource()
    {
        wantsToFollow = false;
        wantsToSeekResource = true;
        wantsToHarvestResource = false;
    }

    public void TogglePhysics(bool isEnabled)
    {
        nav.enabled = !isEnabled;
        rb.isKinematic = !isEnabled;
        rb.interpolation = isEnabled ? RigidbodyInterpolation.Interpolate : RigidbodyInterpolation.None;
        
        if (!isEnabled && TargetResource != null)
        {
            SetNavDestination(TargetResource.transform.position);
        }
    }

    public void ReenableNavMeshCheck()
    {
        if (nav.enabled == false && rb.velocity.sqrMagnitude < disablePhysicsSqredVelocityThreshold)
        {
            TogglePhysics(false);
            canResumeNavigation = false;
        }
    }

    public void SetNavDestination(Vector3 dest)
    {
        if (nav.enabled)
        {
            nav.SetDestination(dest);
        }
    }

    public void ResourceNotFound()
    {
        wantsToSeekResource = false;
    }

    public bool ReachedPathEnd()
    {
        if (nav.enabled && !nav.pathPending && nav.remainingDistance <= nav.stoppingDistance && (!nav.hasPath || nav.velocity.sqrMagnitude < 0.01f))
        {
            return true;
        }
        return false;
    }

    public void LocatedResource()
    {
        wantsToHarvestResource = true;
        wantsToSeekResource = false;
    }

    public void HarvestInterrupted()
    {
        wantsToHarvestResource = false;
        wantsToSeekResource = false;
    }

    public void RotateTowards(Vector3 target)
    {
        rb.rotation = Quaternion.RotateTowards(rb.rotation, Quaternion.LookRotation(new Vector3(target.x - transform.position.x, 0f, target.z - transform.position.z), Vector3.up), nav.angularSpeed * Time.deltaTime);
    }
}