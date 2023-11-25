using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class AgentSFXTrigger : BaseCollisionSFXTrigger
{
    protected override void Awake()
    {
        base.Awake();
    }

    protected override void OnCollisionEnter(Collision collision)
    {
        base.Awake();

        if (collision.collider.CompareTag("Ice Block"))
        {
            var instance = SFXController.Instance.PlayNewInstance(SFXController.Instance.blockHitsChar, transform);
            var forceParamId = Utility.GetFMODParameterID(instance, "force");
            var forceValue = Mathf.Clamp(collision.relativeVelocity.magnitude * 1.5f, 0, 100);
            instance.setParameterByID(forceParamId, forceValue);
            instance.release();
            soundPlayed = true;
        }
        else if (collision.collider.CompareTag("Player") || collision.collider.CompareTag("Worker") || collision.collider.CompareTag("Sea Lion"))
        {
            var instance = SFXController.Instance.PlayNewInstance(SFXController.Instance.charHitsChar, transform);
            var forceParamId = Utility.GetFMODParameterID(instance, "force");
            var forceValue = Mathf.Clamp(collision.relativeVelocity.magnitude * 1.5f, 0, 100);
            instance.setParameterByID(forceParamId, forceValue);
            instance.release();
            soundPlayed = true;
        }
    }
}
