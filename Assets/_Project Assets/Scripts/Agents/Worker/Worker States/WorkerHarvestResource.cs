using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorkerHarvestResource : IState
{
    WorkerController controller;
    private float harvestTimer;

    public WorkerHarvestResource(WorkerController controller)
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
        harvestTimer = controller.harvestDuration;
        controller.TargetResource?.AttachGatherer(controller);
    }

    public void OnExit()
    {
        controller.TargetResource?.DetachGatherer(controller);
        controller.TargetResource = null;
    }

    public void Tick()
    {
        if (controller.TargetResource == null || controller.TargetResource.gameObject.activeInHierarchy == false || (controller.TargetResource.transform.position - controller.transform.position).magnitude > controller.maxHarvestDistance)
        {
            controller.HarvestInterrupted();
            return;
        }
        else
        {
            harvestTimer -= Time.deltaTime;
            if (harvestTimer <= 0)
            {
                controller.TargetResource.Harvest(controller.harvestRate);
                harvestTimer = controller.harvestDuration;
            }
        }

        controller.RotateTowards(controller.TargetResource.transform.position);
    }
}
