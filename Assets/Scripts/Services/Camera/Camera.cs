using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera : MonoBehaviour
{
    public float RotationSensitivity = 75.0f;
    public float minAngle = -25.0f;
    public float maxAngle = 25.0f;
    float yRotate;
    float xRotate;

    void Update()
    {
        yRotate += Input.GetAxis("Mouse Y") * RotationSensitivity * Time.deltaTime;
        yRotate = Mathf.Clamp(yRotate, minAngle, maxAngle);
        xRotate += Input.GetAxis("Mouse X") * RotationSensitivity * Time.deltaTime;
        xRotate = Mathf.Clamp(xRotate, minAngle, maxAngle);
        transform.localEulerAngles = new Vector3(-yRotate, xRotate, 0.0f);
    }
}
