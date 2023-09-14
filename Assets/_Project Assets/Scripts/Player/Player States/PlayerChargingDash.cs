using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

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
        pC.SetMovementProperties(pC.chargingRotSpeed, 0f, 0f);
        pC.ResetDashChargeTime();
    }

    public void OnExit()
    {

    }

    public void Tick()
    {
        if (pC.PollMouseDown() && Physics.Raycast(Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue()), out RaycastHit hit))
        {
            var dir = hit.point - pC.transform.position;
            Debug.DrawRay(hit.point, dir, Color.black);
            pC.MoveWithInput(new Vector2(dir.x, dir.z));
        }
        else
        {
            pC.MoveWithInput();
        }
        pC.DashCharge();
    }
}
