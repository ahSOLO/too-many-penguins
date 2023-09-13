using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorkerSeekResource : IState
{
    WorkerController controller;
    public WorkerSeekResource(WorkerController controller)
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
        // Head to closest resource within radius
        var cols = Physics.OverlapSphere(controller.transform.position, controller.maxSeekDistance, LayerMask.GetMask("Resource"));
        
        if (cols.Length > 0)
        {
            var closest = cols[0];
            var closestDist = (cols[0].transform.position - controller.transform.position).sqrMagnitude;
            for (int i = 1; i < cols.Length; i++)
            {
                var curDist = (cols[i].transform.position - controller.transform.position).sqrMagnitude;
                if (curDist < closestDist)
                {
                    closest = cols[i];
                    closestDist = curDist;
                }
            }

            controller.SetNavDestination(closest.transform.position);
        }
        else
        {

        }
    }

    public void OnExit()
    {
        
    }

    public void Tick()
    {
        
    }
}
