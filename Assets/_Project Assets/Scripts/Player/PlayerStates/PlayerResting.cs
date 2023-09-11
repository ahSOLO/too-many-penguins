using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerResting : IState
{
    PlayerController pC;
    public PlayerResting(PlayerController pC)
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
        pC.RestStart();   
    }

    public void OnExit()
    {
        
    }

    public void Tick()
    {
        pC.RestTick();
    }
}
