using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Transform cameraRootTarget;
    [SerializeField] private Transform cameraRoot;
    [SerializeField] private float cameraFollowSpeed;
    [SerializeField] private float cameraRotateSpeed;
    [SerializeField] private float cameraAllowRotateThreshold;
    [SerializeField] private float deadZoneSqredDistance;
    [SerializeField] private float cameraZoomSpeed;
    [SerializeField] private float cameraZoomInSize;
    [SerializeField] private float cameraZoomOutSize;

    private Camera cam;
    private InputAction cameraRotate;
    private InputAction cameraZoom;
    private bool readyToRotate = true;
    private float zoomTarget;

    private void Awake()
    {
        cam = GetComponent<Camera>();
        zoomTarget = cameraZoomInSize;
    }

    private void Start()
    {
        cameraRotate = InputManager.Instance.GetInputAction("Camera Rotate");
        cameraZoom = InputManager.Instance.GetInputAction("Camera Zoom");
        cameraRotate.Enable();
        cameraZoom.Enable();

        cameraZoom.performed += ctx => ToggleCameraZoom();
    }

    private void Update()
    {
        ApplyCameraRotInput(cameraRotate.ReadValue<float>());

        var rotDiff = Mathf.Abs(cameraRoot.rotation.y - cameraRootTarget.rotation.y);
        if ( rotDiff > Mathf.Epsilon)
        {
            cameraRoot.rotation = Quaternion.Slerp(cameraRoot.rotation, cameraRootTarget.rotation, cameraRotateSpeed * Time.deltaTime);
        }    
        if (rotDiff < cameraAllowRotateThreshold)
        {
            readyToRotate = true;
        }

        if ((cameraRoot.position - cameraRootTarget.position).sqrMagnitude > deadZoneSqredDistance)
        {
            cameraRoot.position = Vector3.Lerp(cameraRoot.position, cameraRootTarget.position, cameraFollowSpeed * Time.deltaTime);
        }

        if (cam.orthographicSize != zoomTarget)
        {
            cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, zoomTarget, cameraZoomSpeed * Time.deltaTime);
        }
    }

    private void ApplyCameraRotInput(float input)
    {
        if (!readyToRotate)
        {
            return;
        }    

        if (input == 1f)
        {
            cameraRootTarget.Rotate(new Vector3(0, 90f, 0));
            readyToRotate = false;
        }
        else if (input == -1f)
        {
            cameraRootTarget.Rotate(new Vector3(0, -90f, 0));
            readyToRotate = false;
        }
    }

    private void ToggleCameraZoom()
    {
        if (zoomTarget == cameraZoomInSize)
        {
            zoomTarget = cameraZoomOutSize;
        }
        else
        {
            zoomTarget = cameraZoomInSize;
        }
    }
}
