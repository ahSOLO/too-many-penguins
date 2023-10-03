using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceBlockController : MonoBehaviour
{
    [SerializeField] private Rigidbody rb;
    [SerializeField] private Collider col;
    [SerializeField] private float timeBeforeEnablingRotations;

    private bool canRotate = false;
    private bool raycastFailed;
    private float enableRotationsTimer;
    private int platformLayer;

    private void Awake()
    {
        platformLayer = LayerMask.NameToLayer("Platform");
    }

    private void FixedUpdate()
    {
        if (!canRotate)
        {
            return;
        }
        
        if (Physics.Raycast(transform.position, Vector3.down, col.bounds.extents.y + 1f, LayerMask.GetMask("Platform")))
        {
            raycastFailed = false;
        }
        else
        {
            raycastFailed = true;
        }
    }

    private void Update()
    {
        if (!canRotate)
        {
            return;
        }
        
        if (raycastFailed == true)
        {
            enableRotationsTimer -= Time.deltaTime;
        }
        else
        {
            enableRotationsTimer = timeBeforeEnablingRotations;
        }

        if (enableRotationsTimer <= 0)
        {
            rb.constraints = RigidbodyConstraints.FreezeRotationY;
        }
        else
        {
            rb.constraints = RigidbodyConstraints.FreezeRotation;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.gameObject.layer == platformLayer)
        {
            canRotate = true;
        }
    }
}
