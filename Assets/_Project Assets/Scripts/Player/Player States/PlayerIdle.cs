using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerIdle : IState
{
    PlayerController pC;
    public PlayerIdle(PlayerController pC)
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
        pC.anim.ResetTrigger("Walk");
        pC.anim.SetTrigger("Idle");
    }

    public void OnExit()
    {
        
    }

    public void Tick()
    {
        
    }
}
