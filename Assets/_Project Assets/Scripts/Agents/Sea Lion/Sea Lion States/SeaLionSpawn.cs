using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeaLionSpawn : IState
{
    SeaLionController controller;

    public SeaLionSpawn(SeaLionController controller)
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
        controller.currentWeightUpdate.Raise(Mathf.RoundToInt(LevelManager.Instance.workerParent.childCount * IslandWeightController.Instance.penguinWeight + LevelManager.Instance.seaLionParent.childCount * IslandWeightController.Instance.seaLionWeight));
        controller.TogglePhysics(false);
    }

    public void Tick()
    {
        
    }
}
