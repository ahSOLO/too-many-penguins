using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFalling : IState
{
    PlayerController pC;
    public PlayerFalling(PlayerController pC)
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
        pC.anim.SetTriggerInstant("Idle");
    }

    public void OnExit()
    {
        
    }

    public void Tick()
    {
        
    }
}
