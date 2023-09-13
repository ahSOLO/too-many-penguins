using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorkerIdle : IState
{
    WorkerController controller;

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
        controller.TogglePhysics(true);
    }

    public void OnExit()
    {
        
    }

    public void Tick()
    {
        
    }
}
