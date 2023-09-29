using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorkerIdle : IState
{
    WorkerController controller;
    private float startWanderTimer;

    public WorkerIdle(WorkerController controller)
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
        startWanderTimer = UnityEngine.Random.Range(controller.minWanderStartTime, controller.maxWanderStartTime);
    }

    public void OnExit()
    {
        
    }

    public void Tick()
    {
        startWanderTimer -= Time.deltaTime;
        if (startWanderTimer <= 0)
        {
            controller.Wander();
        }
    }
}
