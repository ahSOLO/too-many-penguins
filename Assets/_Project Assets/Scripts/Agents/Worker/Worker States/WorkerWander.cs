using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorkerWander : IState
{
    WorkerController controller;
    public WorkerWander(WorkerController controller)
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
        for (int i = 0; i < 5; i++)
        {
            var randomPt = Random.insideUnitCircle;
            var dest = controller.transform.position + new Vector3(randomPt.x * controller.maxWanderRange, 0f, randomPt.y * controller.maxWanderRange);
            Vector3 raycastOrigin = new Vector3(dest.x, 10f, dest.z);
            if (Physics.Raycast(raycastOrigin, dest - raycastOrigin, 20f, LayerMask.GetMask("Platform")))
            {
                controller.SetNavDestination(dest);
                controller.anim.SetTriggerInstant("Walk");
                return;
            }
        }
        controller.WanderEnd();
    }

    public void OnExit()
    {
        
    }

    public void Tick()
    {
        if (controller.ReachedPathEnd())
        {
            controller.WanderEnd();
        }

        controller.IslandShiftingCheck();
    }
}
