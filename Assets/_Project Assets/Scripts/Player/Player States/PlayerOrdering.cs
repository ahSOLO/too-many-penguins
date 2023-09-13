using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerOrdering : IState
{
    PlayerController pC;
    public PlayerOrdering(PlayerController pC)
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
        pC.orderCircle.SetActive(true);
    }

    public void OnExit()
    {
        pC.Order(pC.orderCircle.transform.localScale.x / 2, pC.currentOrder);
        pC.orderCircle.transform.localScale = new Vector3(pC.orderCircleStartRadius, pC.orderCircle.transform.localScale.y, pC.orderCircleStartRadius);
        pC.orderCircle.SetActive(false);
    }

    public void Tick()
    {
        var target = Mathf.Min(pC.orderCircle.transform.localScale.x + pC.circleExpansionRate * Time.deltaTime, pC.orderCircleEndRadius);
        pC.orderCircle.transform.localScale = new Vector3(target, pC.orderCircle.transform.localScale.y, target);
    }
}
