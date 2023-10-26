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
            SFXController.Instance.PlayOneShot(SFXController.Instance.blockHitsChar, transform.position);
            soundPlayed = true;
        }
        else if (collision.collider.CompareTag("Player") || collision.collider.CompareTag("Worker") || collision.collider.CompareTag("Sea Lion"))
        {
            SFXController.Instance.PlayOneShot(SFXController.Instance.charHitsChar, transform.position);
            soundPlayed = true;
        }
    }
}
