using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorkerFollow : IState
{
    WorkerController controller;

    private float navUpdateTimer;
    private float timeEntered;

    public WorkerFollow(WorkerController controller)
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
        SFXController.Instance.PlayOneShot(SFXController.Instance.workerFollow, controller.transform.position);
        controller.anim.SetTriggerInstant("Hop");
        controller.StartCoroutine(Utility.DelayedAction(() => controller.anim.SetTrigger("Walk"), 0.35f));
    }

    public void OnExit()
    {
        controller.anim.speed = 1f;
    }

    public void Tick()
    {
        navUpdateTimer -= Time.deltaTime;
        if (navUpdateTimer < 0)
        {
            controller.SetNavDestination(PlayerController.Instance.transform.position);
            navUpdateTimer = controller.navUpdateFrequency;

            var currentAnim = controller.anim.GetCurrentAnimatorClipInfo(0);
            if (currentAnim.Length > 0 && currentAnim[0].clip.name == "WorkerWalk_dup")
            {
                controller.anim.speed = controller.GetNavSpeedPercentage();
            }
        }
    }
}
