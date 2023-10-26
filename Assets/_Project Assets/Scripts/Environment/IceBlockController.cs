using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceBlockController : MonoBehaviour
{
    [SerializeField] private Rigidbody rb;
    [SerializeField] private Collider col;
    [SerializeField] private float timeBeforeEnablingRotations;
    [SerializeField] private float slideSFXSqredVelocityThreshold;

    private bool canRotate = false;
    private bool raycastFailed;
    private float enableRotationsTimer;
    private int platformLayer;
    private bool playingSlideSFX;
    private FMOD.Studio.EventInstance slideSFXInstance;

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

        if (rb.velocity.sqrMagnitude < slideSFXSqredVelocityThreshold)
        {
            SFXController.Instance.StopInstance(slideSFXInstance);
            playingSlideSFX = false;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.gameObject.layer == platformLayer)
        {
            canRotate = true;
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.collider.gameObject.layer == platformLayer && rb.velocity.sqrMagnitude > slideSFXSqredVelocityThreshold && playingSlideSFX == false)
        {
            slideSFXInstance = SFXController.Instance.PlayNewInstance(SFXController.Instance.blockSlides, transform);
            playingSlideSFX = true;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.collider.gameObject.layer == platformLayer)
        {
            SFXController.Instance.StopInstance(slideSFXInstance);
            playingSlideSFX = false;
        }
    }
}
