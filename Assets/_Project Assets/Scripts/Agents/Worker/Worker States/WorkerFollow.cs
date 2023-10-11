using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorkerFollow : IState
{
    WorkerController controller;

    private float navUpdateTimer;
    private float timeEntered;

    public WorkerFollow(WorkerController controller)
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
        
    }

    public void OnExit()
    {
        
    }

    public void Tick()
    {
        navUpdateTimer -= Time.deltaTime;
        if (navUpdateTimer < 0)
        {
            controller.SetNavDestination(PlayerController.Instance.transform.position);
            navUpdateTimer = controller.navUpdateFrequency;
        }
    }
}
