using UnityEngine;
using System.Collections;

public class CNCameraFollow : MonoBehaviour
{
    public Transform targetObject;
    
    public CNJoystick joystick;

    [Range(1f, 15f)]
    public float cameraDistance = 3f;
    [Range(1f, 100f)]
    public float rotateSpeed = 100f;
    [Range(1f, 5f)]
    public float distanceSpeed = 1f;
    [Range(0f, 360f)]
    public float cameraYAngle = 270f;

    private const float minDistance = 2f;
    private const float maxDistance = 10f;

    void Start()
    {
        if (targetObject == null)
        {
            throw new UnassignedReferenceException("Please, specify player target to follow");
        }
        if (joystick != null)
        {
            joystick.JoystickMovedEvent += ChangeAngle;
        } 
    }

    void LateUpdate()
    {
        SimpleCamera();
        transform.LookAt(targetObject);
    }

    void ChangeAngle(Vector3 relativePosition)
    {
        cameraYAngle -= relativePosition.x * rotateSpeed * Time.deltaTime;
        if ((cameraDistance < minDistance && relativePosition.y < 0f) || 
            (cameraDistance > maxDistance && relativePosition.y > 0f) ||
            cameraDistance >= minDistance && cameraDistance <= maxDistance)
        {
            cameraDistance -= relativePosition.y * distanceSpeed * Time.deltaTime;
        }
    }

    void SimpleCamera()
    {
        Vector3 newPosition = targetObject.position;
        newPosition.x = targetObject.position.x + cameraDistance * Mathf.Sin(cameraYAngle * Mathf.Deg2Rad);
        newPosition.z = targetObject.position.z + cameraDistance * Mathf.Cos(cameraYAngle * Mathf.Deg2Rad);
        newPosition.y = targetObject.position.y + cameraDistance;
        transform.position = newPosition;
    }

}
