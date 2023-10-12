using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityAtoms.BaseAtoms;

public class WaterTrigger : MonoBehaviour
{
    [SerializeField] private ColliderEvent iceBlockHitsWater;
    [SerializeField] private ColliderEvent playerHitsWater;
    [SerializeField] private ColliderEvent workerHitsWater;
    [SerializeField] private ColliderEvent seaLionHitsWater;

    private Collider col;

    private void Awake()
    {
        col = GetComponent<Collider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        var collisionPoint = new Vector3(other.bounds.center.x + other.attachedRigidbody.velocity.x * 0.1f, other.bounds.center.y - other.bounds.extents.y, other.bounds.center.z + other.attachedRigidbody.velocity.z * 0.1f);

        if (other.CompareTag("Ice Block"))
        {
            iceBlockHitsWater.Raise(other);
            VFXController.Instance.PlayVFX(collisionPoint + Vector3.up * 0.1f, VFXController.Instance.cubeSplashVFX);
        }

        else if (other.CompareTag("Player"))
        {
            playerHitsWater.Raise(other);
            VFXController.Instance.PlayVFX(collisionPoint - Vector3.up * 0.125f, VFXController.Instance.workerSplashVFX, 1.5f);
        }

        else if (other.CompareTag("Worker"))
        {
            workerHitsWater.Raise(other);
            VFXController.Instance.PlayVFX(collisionPoint - Vector3.up * 0.025f, VFXController.Instance.workerSplashVFX);
        }

        else if (other.CompareTag("Sea Lion"))
        {
            seaLionHitsWater.Raise(other);
            VFXController.Instance.PlayVFX(collisionPoint - Vector3.up * 0.125f, VFXController.Instance.workerSplashVFX, 1.5f);
        }
    }
}
