using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityAtoms.BaseAtoms;

public class WaterTrigger : MonoBehaviour
{
    [SerializeField] private ColliderEvent iceBlockHitsWater;
    [SerializeField] private ColliderEvent playerHitsWater;
    [SerializeField] private ColliderEvent workerHitsWater;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ice Block"))
        {
            iceBlockHitsWater.Raise(other);
        }

        else if (other.CompareTag("Player"))
        {
            playerHitsWater.Raise(other);
        }

        else if (other.CompareTag("Worker"))
        {
            workerHitsWater.Raise(other);
        }
    }
}
