using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Transform cameraMoveTarget;
    [SerializeField] private float cameraFollowSpeed;
    [SerializeField] private float deadZoneDistance;

    private void Update()
    {
        if ((transform.position - cameraMoveTarget.position).magnitude > deadZoneDistance)
        {
            transform.position = Vector3.Lerp(transform.position, cameraMoveTarget.position, cameraFollowSpeed * Time.deltaTime);
        }
    }
}
