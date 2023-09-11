using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerChargingDash : IState
{
    PlayerController pC;
    public PlayerChargingDash(PlayerController pC)
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
        pC.SetMovementProperties(pC.walkingRotSpeed, 0f, 0f);
        pC.ResetDashChargeTime();
    }

    public void OnExit()
    {

    }

    public void Tick()
    {
        pC.MoveWithInput();
        pC.DashCharge();
    }
}
