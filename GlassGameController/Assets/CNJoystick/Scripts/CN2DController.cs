using UnityEngine;
using System.Collections;

public class CN2DController : MonoBehaviour
{
    public CNJoystick movementJoystick;

    private Transform transformCache;
    // Use this for initialization
    void Awake()
    {
        if (movementJoystick == null)
        {
            throw new UnassignedReferenceException("Please specify movement Joystick object");
        }
        movementJoystick.FingerTouchedEvent += StartMoving;
        movementJoystick.FingerLiftedEvent += StopMoving;
        movementJoystick.JoystickMovedEvent += Move;

        transformCache = transform;
    }

    // You can extend this class and override any of these virtual methods
    protected virtual void Move(Vector3 relativeVector)
    {
        // It's actually 2D vector
        transformCache.position = transformCache.position + relativeVector;
        FaceMovementDirection(relativeVector);
    }

    private void FaceMovementDirection(Vector3 direction)
    {
        if (direction.sqrMagnitude > 0.1)
        {
            transform.up = direction;
        }
    }

    protected virtual void StopMoving()
    {
        
    }

    protected virtual void StartMoving()
    {

    }

}
