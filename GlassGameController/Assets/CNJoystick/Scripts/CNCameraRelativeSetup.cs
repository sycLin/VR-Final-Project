using UnityEngine;
using System.Collections;

[RequireComponent(typeof(CharacterController))]
public class CNCameraRelativeSetup : MonoBehaviour
{
    public CNJoystick joystick;
    public float runSpeed = 6f;

    private CharacterController characterController;
    private Camera mainCamera;
    private float gravity;
    private Vector3 totalMove;

    // This variable is only valuable if you're using Mouse as input
    // if you use only Touch input, feel free to remove this variable
    // and it's usage from this code
    private bool tweakedLastFrame;

    void Awake()
    {
        joystick.JoystickMovedEvent += JoystickMovedEventHandler;
        joystick.FingerLiftedEvent += StopMoving;
        characterController = GetComponent<CharacterController>();
        mainCamera = Camera.main;
        gravity = -Physics.gravity.y;
        totalMove = Vector3.zero;
        tweakedLastFrame = false;
    }

    /** 
     * This function is called when player lifts his finger
     */
    private void StopMoving()
    {
        totalMove = Vector3.zero;
    }

    void Update()
    {
        if(!tweakedLastFrame)
        {
            totalMove = Vector3.zero;
        }
        if (!characterController.isGrounded)
        {
            totalMove.y = (Vector3.down * gravity).y;
        }
        characterController.Move(totalMove * Time.deltaTime);
        tweakedLastFrame = false;
    }

    private void JoystickMovedEventHandler(Vector3 dragVector)
    {
        dragVector.z = dragVector.y;
        dragVector.y = 0f;
        Vector3 movement = mainCamera.transform.TransformDirection(dragVector);
        movement.y = 0f;
        // Uncomment this line if you want to normalize speed,
        // to keep the speed at a constant value
        // -- UNCOMMENT THIS ---
        // movement.Normalize();
        // ---------------------
        totalMove.x = movement.x * runSpeed;
        totalMove.z = movement.z * runSpeed;
        FaceMovementDirection(movement);
        tweakedLastFrame = true;
    }

    private void FaceMovementDirection(Vector3 direction)
    {
        if (direction.sqrMagnitude > 0.1)
        {
            transform.forward = direction;
        }
    }
}
