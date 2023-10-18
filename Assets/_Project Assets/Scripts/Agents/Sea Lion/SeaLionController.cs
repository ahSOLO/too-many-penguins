using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityAtoms.BaseAtoms;
using UnityEngine;
using UnityEngine.AI;

public class SeaLionController : AgentController
{
    public IntEvent currentWeightUpdate;

    private SeaLionSpawn spawnState;
    private SeaLionIdle idleState;
    private SeaLionWander wanderState;
    private SeaLionAttack attackState;
    private SeaLionRest restState;

    public float minWanderStartTime;
    public float maxWanderStartTime;
    public float maxWanderRange;

    public LayerMask workerLayerMask;
    public float attackCheckFrequency;
    public float attackCheckDistance;
    public float attackSpeed;
    public WorkerController targetWorker;
    public float restTime;

    private bool wantsToWander;
    private bool wantsToAttack;
    private bool wantsToRest;

    protected override void Awake()
    {
        base.Awake();

        spawnState = new SeaLionSpawn(this);
        idleState = new SeaLionIdle(this);
        wanderState = new SeaLionWander(this);
        attackState = new SeaLionAttack(this);
        restState = new SeaLionRest(this);
        
        sM.SetInitialState(spawnState);

        workerLayerMask = LayerMask.GetMask("Worker");
    }

    private void Start()
    {
        sM.AddTransition(spawnState, IsGrounded, idleState);
        sM.AddTransition(idleState, () => wantsToWander, wanderState);
        sM.AddTransition(wanderState, () => !wantsToWander, idleState);
        sM.AddTransition(idleState, () => wantsToAttack, attackState);
        sM.AddTransition(attackState, () => wantsToRest, restState);
        sM.AddTransition(restState, () => !wantsToRest, idleState);
    }

    public void Wander()
    {
        wantsToWander = true;
    }

    public void WanderEnd()
    {
        wantsToWander = false;
    }

    public void Attack()
    {
        wantsToAttack = true;
    }

    public void Rest()
    {
        wantsToAttack = false;
        wantsToRest = true;
    }

    public void RestEnd()
    {
        wantsToRest = false;
    }

    public void AttackCheck()
    {
        var cols = Physics.OverlapSphere(transform.position, attackCheckDistance, workerLayerMask);
        if (cols.Length > 0)
        {
            cols = cols.OrderBy(col => (col.transform.position - transform.position).sqrMagnitude).ToArray();
            foreach (var col in cols)
            {
                if (col.gameObject.activeInHierarchy && Physics.Raycast(transform.position, col.transform.position - transform.position, attackCheckDistance, workerLayerMask))
                {
                    targetWorker = col.GetComponentInParent<WorkerController>();
                    return;
                }
            }
        }
    }

    public void ApplyForce(Vector3 force)
    {
        rb.AddForce(force, ForceMode.VelocityChange);
    }

    public override void TogglePhysics(bool isEnabled, float resumeNavigationTime = 0.6f)
    {
        base.TogglePhysics(isEnabled, resumeNavigationTime);

        if (isEnabled)
        {
            WanderEnd();
        }
    }
}
