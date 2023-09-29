using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorkerSpawn : IState
{
    WorkerController controller;

    public WorkerSpawn(WorkerController controller)
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
        controller.SetSpawnVelocity();
    }

    public void OnExit()
    {
        controller.spawnComplete = true;
        controller.currentWeightUpdate.Raise(Mathf.RoundToInt(IslandWeightController.Instance.penguinWeight * controller.transform.parent.childCount));
        controller.TogglePhysics(false);
    }

    public void Tick()
    {
        
    }
}
