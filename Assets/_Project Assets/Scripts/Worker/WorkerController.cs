using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent), typeof(Rigidbody), typeof(StateMachine))]
public class WorkerController : MonoBehaviour
{
    private NavMeshAgent nav;
    private Rigidbody rb;
    private StateMachine sM;
    [SerializeField] private CapsuleCollider col;

    private WorkerIdle idleState;
    private WorkerFollow followState;
    private WorkerSeekResource seekResourceState;

    public float disablePhysicsSqredVelocityThreshold;
    public float navUpdateFrequency;
    public float maxSeekDistance;

    private bool wantsToFollow;
    private bool wantsToSeekResource;
    private bool canResumeNavigation;
    private Coroutine activeCoroutine;

    private void Awake()
    {
        nav = GetComponent<NavMeshAgent>();
        rb = GetComponent<Rigidbody>();
        sM = GetComponent<StateMachine>();

        idleState = new WorkerIdle(this);
        followState = new WorkerFollow(this);
        seekResourceState = new WorkerSeekResource(this);
        
        sM.SetInitialState(idleState);
    }

    private void Start()
    {
        sM.AddTransition(idleState, () => wantsToFollow, followState);
        sM.AddTransition(idleState, () => wantsToSeekResource, seekResourceState);
        sM.AddTransition(followState, () => wantsToSeekResource, seekResourceState);
        sM.AddTransition(seekResourceState, () => !wantsToSeekResource, idleState);
        sM.AddTransition(seekResourceState, () => wantsToFollow, followState);
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

    public void PotentialCollisionCheck(Rigidbody rb)
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

    private void Update()
    {
        if (canResumeNavigation)
        {
            ReenableNavMeshCheck();
        }
    }

    public void FollowPlayer()
    {
        wantsToSeekResource = false;
        wantsToFollow = true;
    }

    public void SeekResource()
    {
        wantsToFollow = false;
        wantsToSeekResource = true;
    }

    public void TogglePhysics(bool isEnabled)
    {
        nav.enabled = !isEnabled;
        rb.isKinematic = !isEnabled;
        rb.interpolation = isEnabled ? RigidbodyInterpolation.Interpolate : RigidbodyInterpolation.None;
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
}
