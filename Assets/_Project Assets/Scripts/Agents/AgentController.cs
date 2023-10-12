using System.Collections;
using System.Collections.Generic;
using UnityAtoms.BaseAtoms;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent), typeof(Rigidbody), typeof(StateMachine))]
public class AgentController : MonoBehaviour
{
    [SerializeField] protected Renderer rend;
    protected NavMeshAgent nav;
    protected Rigidbody rb;
    protected StateMachine sM;

    [SerializeField] protected CapsuleCollider col;
    private LayerMask colliderMask;

    public float disablePhysicsSqredVelocityThreshold;

    [SerializeField] protected Vector3 spawnVelocity;
    public bool spawnComplete;

    protected bool canResumeNavigation;
    protected Coroutine activeCoroutine;
    public enum GroundedState { Airborne, Grounded, InGround };

    protected virtual void Awake()
    {
        nav = GetComponent<NavMeshAgent>();
        rb = GetComponent<Rigidbody>();
        sM = GetComponent<StateMachine>();

        colliderMask = LayerMask.GetMask("Player", "Ice Block", "Worker", "Sea Lion");
    }

    protected virtual void FixedUpdate()
    {
        var cols = Physics.OverlapCapsule(col.transform.position - new Vector3(col.center.x, col.height / 2f, col.center.z), col.transform.position + new Vector3(col.center.x, col.height / 2f, col.center.z), col.radius + 0.1f, colliderMask);
        if (cols.Length > 0)
        {
            foreach (var col in cols)
            {
                PotentialCollisionCheck(col.attachedRigidbody);
            }
        }
    }

    protected void Update()
    {
        if (canResumeNavigation)
        {
            ReenableNavMeshCheck();
        }
    }

    protected void PotentialCollisionCheck(Rigidbody rb)
    {
        if (Vector3.Dot(rb.velocity, transform.position - rb.position) > 0)
        {
            TogglePhysics(true, 0.6f);
        }
    }

    protected GroundedState GroundCheck()
    {
        var raycastLength = col.bounds.extents.y + 0.1f;
        var layerMask = LayerMask.GetMask("Platform");
        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hitInfo, raycastLength, layerMask))
        {
            if (transform.position.y - hitInfo.point.y >= col.bounds.extents.y)
            {
                return GroundedState.Grounded;
            }
            else
            {
                return GroundedState.InGround;
            }
        }
        else
        {
            return GroundedState.Airborne;
        }
    }

    protected bool IsGrounded()
    {
        return GroundCheck() == GroundedState.Grounded;
    }

    public void IslandShiftingCheck()
    {
        if (rb.isKinematic)
        {
            var groundCheck = GroundCheck();
            if (groundCheck != GroundedState.Grounded)
            {
                TogglePhysics(true, 0.2f);
            }
        }
    }

    public void SetSpawnVelocity()
    {
        rb.AddForce(transform.rotation * spawnVelocity, ForceMode.VelocityChange);
    }

    public virtual void TogglePhysics(bool isEnabled, float resumeNavigationTime = 0.6f)
    {
        nav.enabled = !isEnabled;
        rb.isKinematic = !isEnabled;
        rb.interpolation = isEnabled ? RigidbodyInterpolation.Interpolate : RigidbodyInterpolation.None;

        if (isEnabled)
        {
            canResumeNavigation = false;
            if (activeCoroutine != null)
            {
                StopCoroutine(activeCoroutine);
            }
            activeCoroutine = StartCoroutine(Utility.DelayedAction(() => canResumeNavigation = true, resumeNavigationTime));
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

    public bool ReachedPathEnd()
    {
        if (nav.enabled && !nav.pathPending && nav.remainingDistance <= nav.stoppingDistance && (!nav.hasPath || nav.velocity.sqrMagnitude < 0.01f))
        {
            return true;
        }
        return false;
    }

    public void RotateTowards(Vector3 target)
    {
        rb.rotation = Quaternion.RotateTowards(rb.rotation, Quaternion.LookRotation(new Vector3(target.x - transform.position.x, 0f, target.z - transform.position.z), Vector3.up), nav.angularSpeed * Time.deltaTime);
    }
}
