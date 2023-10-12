using System.Collections;
using System.Collections.Generic;
using UnityAtoms.BaseAtoms;
using UnityEngine;
using UnityEngine.AI;

public class SeaLionController : AgentController
{
    public IntEvent currentWeightUpdate;

    private SeaLionSpawn spawnState;
    private SeaLionIdle idleState;
    private SeaLionWander wanderState;

    public float minWanderStartTime;
    public float maxWanderStartTime;
    public float maxWanderRange;

    private bool wantsToWander;

    protected override void Awake()
    {
        base.Awake();

        spawnState = new SeaLionSpawn(this);
        idleState = new SeaLionIdle(this);
        wanderState = new SeaLionWander(this);
        
        sM.SetInitialState(spawnState);
    }

    private void Start()
    {
        sM.AddTransition(spawnState, IsGrounded, idleState);
        sM.AddTransition(idleState, () => wantsToWander, wanderState);
        sM.AddTransition(wanderState, () => !wantsToWander, idleState);
    }

    public void Wander()
    {
        wantsToWander = true;
    }

    public void WanderEnd()
    {
        wantsToWander = false;
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
