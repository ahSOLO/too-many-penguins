using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDashing : IState
{
    PlayerController pC;
    public PlayerDashing(PlayerController pC)
    {
        this.pC = pC;
    }

    public void FixedTick()
    {

    }

    public void LateTick()
    {

    }

    public void OnEnter()
    {
        pC.DashStart();
        SFXController.Instance.PlayOneShot(SFXController.Instance.leaderDash, pC.transform.position);
    }

    public void OnExit()
    {

    }

    public void Tick()
    {
        pC.DashTick();
    }
}
