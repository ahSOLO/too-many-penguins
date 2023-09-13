using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWalking : IState
{
    PlayerController pC;
    public PlayerWalking(PlayerController pC)
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
        pC.SetMovementProperties(pC.walkingRotSpeed, pC.walkingAccel, pC.walkingMaxSpd);
    }

    public void OnExit()
    {
        
    }

    public void Tick()
    {
        pC.MoveWithInput();
    }
}
