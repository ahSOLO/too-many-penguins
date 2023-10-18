using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeaLionIdle : IState
{
    SeaLionController controller;
    private float startWanderTimer;
    private float attackCheckTimer;

    public SeaLionIdle(SeaLionController controller)
    {
        this.controller = controller;
    }

    public void FixedTick()
    {
        
    }

    public void LateTick()
    {
        
    }

    public void OnEnter()
    {
        startWanderTimer = Random.Range(controller.minWanderStartTime, controller.maxWanderStartTime);
    }

    public void OnExit()
    {
        
    }

    public void Tick()
    {
        controller.IslandShiftingCheck();

        startWanderTimer -= Time.deltaTime;
        if (startWanderTimer <= 0)
        {
            controller.Wander();
        }

        attackCheckTimer -= Time.deltaTime;
        if (attackCheckTimer <= 0)
        {
            controller.targetWorker = null;
            controller.AttackCheck();
            if (controller.targetWorker != null)
            {
                controller.Attack();
            }
            attackCheckTimer = controller.attackCheckFrequency;
        }
    }
}
