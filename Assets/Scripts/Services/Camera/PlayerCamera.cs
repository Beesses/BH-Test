using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    public Transform target;
    public Camera sceneCamera;

    [Range(1f, 15f)]
    public float mouseRotateSpeed = 5f;

    public float slerpSmoothValue = 0.3f;
    public float scrollSmoothTime = 0.12f;
    public float editorFOVSensitivity = 5f;
    public float touchFOVSensitivity = 5f;

    private bool canRotate = true;

    private Quaternion currentRot;
    private Quaternion targetRot;

    private float rotX;
    private float rotY;
    private float cameraFieldOfView;
    private float cameraFOVDamp;
    private float fovChangeVelocity = 0;

    private float distanceBetweenCameraAndTarget;
    private float minXRotAngle = -85;
    private float maxXRotAngle = 2;

    private float minCameraFieldOfView = 6;
    private float maxCameraFieldOfView = 30;

    Vector3 dir;
    private void Awake()
    {
        GetCamera();

    }

    void Start()
    {
        
    }

    void Update()
    {
        if(transform.parent != null)
        {
            target = transform.parent;
            distanceBetweenCameraAndTarget = 4;
            dir = new Vector3(0, 0, distanceBetweenCameraAndTarget);
            sceneCamera.transform.position = target.position + dir;

            cameraFOVDamp = sceneCamera.fieldOfView;
            cameraFieldOfView = sceneCamera.fieldOfView;

            if (!canRotate)
            {
                return;
            }
            EditorCameraInput();
        }
    }

    private void LateUpdate()
    {
        if(transform.parent != null)
        {
            RotateCamera();
            SetCameraFOV();
        }
    }

    public void GetCamera()
    {
        if (sceneCamera == null)
        {
            sceneCamera = Camera.main;
        }

    }

    private void EditorCameraInput()
    {
        rotX += Input.GetAxis("Mouse Y") * mouseRotateSpeed;
        rotY += Input.GetAxis("Mouse X") * mouseRotateSpeed;

        if (rotX < minXRotAngle)
        {
            rotX = minXRotAngle;
        }
        else if (rotX > maxXRotAngle)
        {
            rotX = maxXRotAngle;
        }

        if (Input.mouseScrollDelta.magnitude > 0)
        {
            cameraFieldOfView += Input.mouseScrollDelta.y * editorFOVSensitivity * -1;
        }
    }

    
    private void RotateCamera()
    {

        Vector3 tempV = new Vector3(rotX, rotY, 0);
        targetRot = Quaternion.Euler(tempV);
        currentRot = Quaternion.Slerp(currentRot, targetRot, Time.smoothDeltaTime * slerpSmoothValue * 50);
        sceneCamera.transform.position = target.position + currentRot * dir;
        sceneCamera.transform.LookAt(target.position);

    }


    void SetCameraFOV()
    {
        if (cameraFieldOfView <= minCameraFieldOfView)
        {
            cameraFieldOfView = minCameraFieldOfView;
        }
        else if (cameraFieldOfView >= maxCameraFieldOfView)
        {
            cameraFieldOfView = maxCameraFieldOfView;
        }

        cameraFOVDamp = Mathf.SmoothDamp(cameraFOVDamp, cameraFieldOfView, ref fovChangeVelocity, scrollSmoothTime);
        sceneCamera.fieldOfView = cameraFOVDamp;

    }

}
