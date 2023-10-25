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
        pC.ToggleIndicatorRect(true);
        pC.ResetIndicatorRect();
        ToggleVFX(true);

        SFXController.Instance.PlayInstance(SFXController.Instance.leaderDash, ref SFXController.Instance.leaderChargeInstance, pC.transform);
    }

    public void OnExit()
    {
        pC.ToggleIndicatorRect(false);
        ToggleVFX(false);

        SFXController.Instance.StopInstance(SFXController.Instance.leaderChargeInstance);
    }

    public void Tick()
    {
        if (pC.PollLeftMouseButton() && Physics.Raycast(Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue()), out RaycastHit hit))
        {
            var dir = hit.point - pC.transform.position;
            pC.MoveWithInput(new Vector2(dir.x, dir.z));
        }
        else
        {
            pC.MoveWithInput();
        }

        pC.DashCharge();
        pC.ChargeIndicatorRect();
    }

    private void ToggleVFX(bool isActive)
    {
        if (isActive)
        {
            pC.chargingVFX.SetActive(true);
            pC.chargingVFXParticleSystemRipples.Simulate(0.5f);
            pC.chargingVFXParticleSystemRipples.Play();
        }
        else
        {
            pC.chargingVFX.SetActive(false);
        }
    }
}
