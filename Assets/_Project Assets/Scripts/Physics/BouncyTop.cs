using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BouncyTop : MonoBehaviour
{
    [SerializeField] private Collider col;
    [SerializeField] private float additionalFuzzyForce;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.GetContact(0).point.y > transform.position.y + col.bounds.center.y + col.bounds.extents.y - 0.1f)
        {
            collision.collider.attachedRigidbody.AddForce(
                collision.GetContact(0).normal.normalized + 
                new Vector3(UnityEngine.Random.Range(0f, 1f), UnityEngine.Random.Range(0f, 1f), UnityEngine.Random.Range(0f, 1f)).normalized * additionalFuzzyForce, 
                ForceMode.VelocityChange);
        }
    }
}
