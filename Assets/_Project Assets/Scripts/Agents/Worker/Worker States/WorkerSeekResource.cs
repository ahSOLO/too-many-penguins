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
        // Head to closest free resource within radius
        var cols = Physics.OverlapSphere(controller.transform.position, controller.maxSeekDistance, LayerMask.GetMask("Resource"));
        List<Resource> freeResourcesInRange = new List<Resource>();

        foreach (var col in cols)
        {
            var resource = col.GetComponentInParent<Resource>();
            if (!resource.IsFull())
            {
                freeResourcesInRange.Add(resource);
            }
        }
        
        if (freeResourcesInRange.Count > 0)
        {
            var closest = freeResourcesInRange[0];
            var closestDist = (freeResourcesInRange[0].transform.position - controller.transform.position).sqrMagnitude;
            for (int i = 1; i < freeResourcesInRange.Count; i++)
            {
                var curDist = (freeResourcesInRange[i].transform.position - controller.transform.position).sqrMagnitude;
                if (curDist < closestDist)
                {
                    closest = freeResourcesInRange[i];
                    closestDist = curDist;
                }
            }

            controller.SetNavDestination(closest.transform.position);
            controller.TargetResource = closest;
            closest.AttachGatherer(controller);
        }
        else
        {
            controller.ResourceNotFound();
        }
    }

    public void OnExit()
    {
        controller.TargetResource?.DetachGatherer(controller);
    }

    public void Tick()
    {
        if (controller.ReachedPathEnd())
        {
            controller.LocatedResource();
        }
    }
}
