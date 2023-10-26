using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class BaseCollisionSFXTrigger : MonoBehaviour
{
    [SerializeField] private float minSqredVelocityToPlaySFX;
    protected Rigidbody rb;
    protected bool soundPlayed = false;

    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    protected virtual void OnCollisionEnter(Collision collision)
    {
        if (collision.relativeVelocity.sqrMagnitude < minSqredVelocityToPlaySFX)
        {
            return;
        }
        
        var sFXTrigger = collision.collider.attachedRigidbody.GetComponent<BaseCollisionSFXTrigger>();
        if ( sFXTrigger == null || sFXTrigger.soundPlayed == true)
        {
            return;
        }
    }

    private void Update()
    {
        soundPlayed = false;
    }
}
