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
    }

    public void OnExit()
    {

    }

    public void Tick()
    {
        pC.DashTick();
    }
}
