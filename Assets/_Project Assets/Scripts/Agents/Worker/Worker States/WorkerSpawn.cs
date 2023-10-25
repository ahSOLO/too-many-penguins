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

        controller.StartCoroutine(Utility.DelayedAction(() => SFXController.Instance.PlayOneShot(SFXController.Instance.workerJumpsOutWater, controller.transform.position), 0.1f));
    }

    public void OnExit()
    {
        controller.spawnComplete = true;
        controller.currentWeightUpdate.Raise(Mathf.RoundToInt(LevelManager.Instance.workerParent.childCount * IslandWeightController.Instance.penguinWeight + LevelManager.Instance.seaLionParent.childCount * IslandWeightController.Instance.seaLionWeight));
        controller.TogglePhysics(false);
    }

    public void Tick()
    {
        
    }
}
