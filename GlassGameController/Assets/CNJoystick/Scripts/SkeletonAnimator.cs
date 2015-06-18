using UnityEngine;
using System.Collections;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(CNCameraRelativeSetup))]
public class SkeletonAnimator : MonoBehaviour
{
    private const string IDLE = "Idle";
    private const string WALK = "Walk";
    private const string RUN = "Run";
    private const string ATTACK = "Attack";
    private const string ATTACK_1 = "Attack1";
    private const float WALK_SPEED_MULTIPLIER = 0.6f;
    private const float RUN_SPEED_MULTIPLIER = 2f;

    private CharacterController charController;
    private CNCameraRelativeSetup cameraRelativeSetup;
    private CNJoystick joystick;

    // Use this for initialization
    void Awake()
    {
        charController = GetComponent<CharacterController>();
        cameraRelativeSetup = GetComponent<CNCameraRelativeSetup>();
        joystick = cameraRelativeSetup.joystick;

        joystick.JoystickMovedEvent += AnimateMovement;
        joystick.FingerLiftedEvent += StoppedMoving;
    }

    void Update()
    {
        Debug.Log(charController.velocity);
    }

    // Update is called once per frame
    void AnimateMovement(Vector3 relativeMovement)
    {
        //Debug.Log(charController.velocity.sqrMagnitude);
        float sqrMag = relativeMovement.sqrMagnitude;
        if (sqrMag > 0f)
        {
            if (sqrMag >= 0.3f)
            {
                animation[WALK].speed = charController.velocity.magnitude / RUN_SPEED_MULTIPLIER;
                animation.CrossFade(RUN);
            }
            else
            {
                animation[WALK].speed = charController.velocity.magnitude / WALK_SPEED_MULTIPLIER;
                animation.CrossFade(WALK);
            }

        }
    }

    void StoppedMoving()
    {
        animation.CrossFade(IDLE);
    }
}
