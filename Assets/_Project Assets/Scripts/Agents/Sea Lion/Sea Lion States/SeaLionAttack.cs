using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeaLionAttack : IState
{
    SeaLionController controller;
    public SeaLionAttack(SeaLionController controller)
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
        controller.anim.SetTriggerInstant("Attack");
    }

    public void OnExit()
    {
        
    }

    public void Tick()
    {
        controller.IslandShiftingCheck();

        if (controller.targetWorker == null)
        {
            controller.Rest();
        }
        else
        {
            controller.RotateTowards(controller.targetWorker.transform.position);

            if (Physics.Raycast(controller.transform.position, controller.transform.forward, controller.attackCheckDistance, controller.workerLayerMask))
            {
                controller.TogglePhysics(true);
                controller.ApplyForce(controller.transform.forward * controller.attackSpeed);

                SFXController.Instance.PlayOneShot(SFXController.Instance.seaLionAttacks, controller.transform.position);

                controller.Rest();
            }
        }
    }
}
