using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeaLionRest : IState
{
    SeaLionController controller;
    private float resumeIdleTimer;

    public SeaLionRest(SeaLionController controller)
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
        resumeIdleTimer = controller.restTime;
    }

    public void OnExit()
    {
        
    }

    public void Tick()
    {
        controller.IslandShiftingCheck();

        resumeIdleTimer -= Time.deltaTime;
        if (resumeIdleTimer <= 0)
        {
            controller.RestEnd();
        }
    }
}
