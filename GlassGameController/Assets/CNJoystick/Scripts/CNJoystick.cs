using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// Placement snap enum
public enum PlacementSnap
{
    leftTop,
    leftBottom,
    rightTop,
    rightBottom
}
/**
 * Joystic move event delegate. 
 *  Magnitude is a value from 0 to 1 inclusive.
 *  0 is Center and 1 is full radius
 */
public delegate void JoystickMoveEventHandler(Vector3 relativeVector);
public delegate void FingerLiftedEventHandler();
public delegate void FingerTouchedEventHandler();

public class CNJoystick : MonoBehaviour
{
    // Private delegate to automatically switch between Mouse input and Touch input
    private delegate void InputHandler();

    public bool remoteTesting = false;
    // Editor variables
    //public float pixelToUnits = 1000f;
    public PlacementSnap placementSnap = PlacementSnap.leftBottom;
    public Rect tapZone;

    // Script-only public variables
    public Camera CurrentCamera { get; set; }
    public event JoystickMoveEventHandler JoystickMovedEvent;
    public event FingerLiftedEventHandler FingerLiftedEvent;
    public event FingerTouchedEventHandler FingerTouchedEvent;
    /**
     * Private instance variables
     */
    // Joystick base, large circle
    private GameObject joystickBase;
    // Joystick itself, small circle
    private GameObject joystick;
    // Radius of the large circle in world units
    private float joystickBaseRadius;
    // Camera frustum height
    private float frustumHeight;
    // Camera frustum width
    private float frustumWidth;
    // Finger ID to track
    private int myFingerId = -1;
    // Where did we touch initially
    private Vector3 invokeTouchPosition;
    // Relative position of the small joystick circle
    private Vector3 joystickRelativePosition;
    // Screen point in units cacher variable
    private Vector3 screenPointInUnits;
    // Magic Vector3, needed for different snap placements
    private Vector3 relativeExtentSummand;
    // This joystick is currently being tweaked
    private bool isTweaking = false;
    // Touch or Click
    private InputHandler CurrentInputHandler;
    // Distance to camera
    private float distanceToCamera = 0.5f;
    // Half of screen sizes
    private float halfScreenHeight;
    private float halfScreenWidth;
    // Full screen sizes
    private float screenHeight;
    private float screenWidth;
    // Snap position, relative joystick position
    private Vector3 snapPosition;
    // (-halfScreenWidth, -halfScreenHeight, 0f)
    // Visually, it's the bottom left point in local units
    private Vector3 cleanPosition;
    // Some transform cache variables
    private Transform joystickBaseTransform;
    private Transform joystickTransform;
    private Transform transformCache;

    // Use this for initialization
    void Awake()
    {
        CurrentCamera = transform.parent.camera;

        transformCache = transform;
        joystickBase = transformCache.FindChild("Base").gameObject;
        joystickBaseTransform = joystickBase.transform;
        joystick = transform.FindChild("Joystick").gameObject;
        joystickTransform = joystick.transform;

        InitialCalculations();

#if UNITY_IPHONE || UNITY_ANDROID || UNITY_WP8 || UNITY_BLACKBERRY
        CurrentInputHandler = TouchInputHandler;
#endif
#if UNITY_EDITOR || UNITY_WEBPLAYER || UNITY_STANDALONE
        // gameObject.SetActive(false);
        CurrentInputHandler = MouseInputHandler;
#endif
        if (remoteTesting)
            CurrentInputHandler = TouchInputHandler;
    }

    void Update()
    {
        // Automatically call proper input handler
        CurrentInputHandler();
    }

    /** Our touch Input handler
     * Most of the work is done in this function
     */
    void TouchInputHandler()
    {
        // Current touch count
        int touchCount = Input.touchCount;
        // If we're not yet tweaking, we should check
        // whether any touch lands on our BoxCollider or not
        if (!isTweaking)
        {
            for (int i = 0; i < touchCount; i++)
            {
                // Get current touch
                Touch touch = Input.GetTouch(i);
                // We check it's phase.
                // If it's not a Begin phase, finger didn't tap the screen
                // it's probably just slided to our TapRect
                // So for the sake of optimization we won't do anything with this touch
                // But if it's a tap, we check if it lands on our TapRect 
                // See TouchOccured function
                if (touch.phase == TouchPhase.Began && TouchOccured(touch.position))
                {
                    // We should store our finger ID 
                    myFingerId = touch.fingerId;
                    // If it's a valid touch, we dispatch our FingerTouchEvent
                    if (FingerTouchedEvent != null)
                        FingerTouchedEvent();
                }
            }
        }
        // We take Touch screen position and convert it to local joystick - relative coordinates
        else
        {
            // This boolean represents if current touch has a Ended phase.
            // It's here for more code readability
            bool isGoingToEnd = false;
            for (int i = 0; i < touchCount; i++)
            {
                Touch touch = Input.GetTouch(i);
                // For every finger out there, we check if OUR finger has just lifted from the screen
                if (myFingerId == touch.fingerId && touch.phase == TouchPhase.Ended)
                {
                    // And if it does, we reset our Joystick with this function
                    ResetJoystickPosition();
                    // We store our boolean here
                    isGoingToEnd = true;
                    // And dispatch our FingerLiftedEvent
                    if (FingerLiftedEvent != null)
                        FingerLiftedEvent();
                }
            }
            // If user didn't lift his finger this frame
            if (!isGoingToEnd)
            {
                // We find our current Touch index (it's not always equal to Finger index)
                int currentTouchIndex = FindMyFingerId();
                if (currentTouchIndex != -1)
                {
                    // And call our TweakJoystick function with this finger
                    TweakJoystick(Input.GetTouch(currentTouchIndex).position);
                }
            }
        }
    }
#if UNITY_EDITOR || UNITY_WEBPLAYER || UNITY_STANDALONE
    // Mouse input handler, nothing really interesting
    // It's pretty straightforward
    void MouseInputHandler()
    {
        if (Input.GetMouseButtonDown(0))
        {
            TouchOccured(Input.mousePosition);
        }
        if (Input.GetMouseButton(0))
        {
            if (isTweaking)
            {
                TweakJoystick(Input.mousePosition);
            }
        }
        if (Input.GetMouseButtonUp(0))
        {
            ResetJoystickPosition();
        }
    }
#endif
    /**
     * Snappings calculation
     * Joystick radius calculation based on specified pixel to units value
     * Initial on-screen placement, relative to the specified camera
     */
    void InitialCalculations()
    {
        halfScreenHeight = CurrentCamera.orthographicSize;
        halfScreenWidth = halfScreenHeight * CurrentCamera.aspect;

        screenHeight = halfScreenHeight * 2f;
        screenWidth = halfScreenWidth * 2f;

        snapPosition = new Vector3();
        cleanPosition = new Vector3(-halfScreenWidth, -halfScreenHeight);

        switch (placementSnap)
        {
            // We do nothing, it's a default position
            case PlacementSnap.leftBottom:
                snapPosition.x = -halfScreenWidth + tapZone.width / 2f - tapZone.x;
                snapPosition.y = -halfScreenHeight + tapZone.height / 2f - tapZone.y;

                // Tap zone change so we can utilize Rect's .Contains() method
                tapZone.x = 0f;
                tapZone.y = 0f;
                break;
            // We swap Y component
            case PlacementSnap.leftTop:
                snapPosition.x = -halfScreenWidth + tapZone.width / 2f - tapZone.x;
                snapPosition.y = halfScreenHeight - tapZone.height / 2f - tapZone.y;

                // Tap zone change so we can utilize Rect's .Contains() method
                tapZone.x = 0f;
                tapZone.y = screenHeight - tapZone.height;
                break;
            // We swap X component
            case PlacementSnap.rightBottom:
                snapPosition.x = halfScreenWidth - tapZone.width / 2f - tapZone.x;
                snapPosition.y = -halfScreenHeight + tapZone.height / 2f - tapZone.y;

                // Tap zone change so we can utilize Rect's .Contains() method
                tapZone.x = screenWidth - tapZone.width;
                tapZone.y = 0f;
                break;
            // We swap both X and Y component
            case PlacementSnap.rightTop:
                snapPosition.x = halfScreenWidth - tapZone.width / 2f - tapZone.x;
                snapPosition.y = halfScreenHeight - tapZone.height / 2f - tapZone.y;

                // Tap zone change so we can utilize Rect's .Contains() method
                tapZone.x = screenWidth - tapZone.width;
                tapZone.y = screenHeight - tapZone.height;
                break;
        }
        transformCache.localPosition = snapPosition;

        SpriteRenderer joystickBaseSpriteRenderer = joystickBase.renderer as SpriteRenderer;
        joystickBaseRadius = joystickBaseSpriteRenderer.bounds.extents.x;

        /** OBSOLETE CODE USED FOR PERSPECTIVE CAMERAS 
         *  Some crazy stuff with pyramid frustums and things
        distanceToCamera = transform.localPosition.z;

        // We need to find clear bounds of our Collider so we place our joystick to world's zero
        transform.position = Vector3.zero;
        transform.rotation = Quaternion.identity;
        Bounds cleanBounds = collider.bounds;
        Vector3 currentExtents = collider.bounds.extents;

        // Then we reset it to it's normal position
        transform.localPosition = new Vector3(0f, 0f, distanceToCamera);
        transform.localRotation = Quaternion.identity;

        // Calculating frustum height and frustum width at given distance from the camera
        frustumHeight = 2.0f * distanceToCamera * Mathf.Tan(CurrentCamera.fieldOfView * 0.5f * Mathf.Deg2Rad);
        frustumWidth = frustumHeight * CurrentCamera.aspect;

        // We also need to find the radius of our base
        // That's why we need pixelToUnits variable
        SpriteRenderer spr = joystickBase.renderer as SpriteRenderer;
        joystickBaseRadius = (spr.sprite.rect.height / 2) / pixelToUnits;

        float x = 0f;
        float y = 0f;

        // Some dirty magic here
        // Relative position check for optimisation
        // If our snapping is different than leftBottom, we need to recalculate relative positions
        // DO NOT TOUCH
        //=======================================================================================
        switch (placementSnap)
        {
            // We do nothing, it's a default position
            case PlacementSnap.leftBottom:
                x = -frustumWidth * 0.5f - cleanBounds.min.x;
                y = -frustumHeight * 0.5f - cleanBounds.min.y;
                break;
            // We swap Y component
            case PlacementSnap.leftTop:
                x = -frustumWidth * 0.5f - cleanBounds.min.x;
                y = frustumHeight * 0.5f - cleanBounds.max.y;
                currentExtents.y = cleanBounds.extents.y + Mathf.Abs(cleanBounds.center.y * 2f);
                break;
            // We swap X component
            case PlacementSnap.rightBottom:
                x = frustumWidth * 0.5f - cleanBounds.max.x;
                y = -frustumHeight * 0.5f - cleanBounds.min.y;
                currentExtents.x = frustumWidth - cleanBounds.extents.x;
                break;
            // We swap both X and Y component
            case PlacementSnap.rightTop:
                x = frustumWidth * 0.5f - cleanBounds.max.x;
                y = frustumHeight * 0.5f - cleanBounds.max.y;
                currentExtents.y = cleanBounds.extents.y + Mathf.Abs(cleanBounds.center.y * 2f);
                currentExtents.x = frustumWidth - cleanBounds.extents.x;
                break;
        }
        // We'll use this Vector3 to calculate relative joystick position
        // It's needed because we have to convert camera cordinates to local relative joystick position
        relativeExtentSummand = currentExtents - cleanBounds.center;
        transform.localPosition = new Vector3(x, y, distanceToCamera);
        joystickRelativePosition = new Vector3();
        //=======================================================================================
         * */
    }

    /**
     * Touch or mouse click occured
     * Store initial local position
     * Vector3 touchPosition is in Screen coordinates
     * 
     * Returns true if finger found
     * Returns fals if not
     */
    bool TouchOccured(Vector3 touchPosition)
    {
        ScreenPointToRelativeFrustumPoint(touchPosition);
        if (tapZone.Contains(screenPointInUnits))
        {
            isTweaking = true;
            invokeTouchPosition = screenPointInUnits;
            transformCache.localPosition = cleanPosition;
            joystickBaseTransform.localPosition = invokeTouchPosition;
            joystickTransform.localPosition = invokeTouchPosition;
            return true;
        }

        /** OBSOLETE
        Ray screenRay = CurrentCamera.ScreenPointToRay(touchPosition);
        RaycastHit hit;
        if (Physics.Raycast(screenRay, out hit, distanceToCamera * 2f))
        {
            if (hit.collider == collider)
            {
                isTweaking = true;
                invokeTouchPosition = transform.InverseTransformPoint(hit.point);
                invokeTouchPosition.z = 0f;
                joystickBase.transform.localPosition = invokeTouchPosition;
                joystick.transform.localPosition = invokeTouchPosition;
                return true;
            }
        }* */
        return false;
    }

    /**
     * Try to drag small joystick knob to it's desired position (in Screen coordinates)
     */
    void TweakJoystick(Vector3 desiredPosition)
    {
        // We convert our screen coordinates of the touch to local frustum coordinates
        ScreenPointToRelativeFrustumPoint(desiredPosition);
        // And then we find our joystick relative position
        Vector3 dragDirection = screenPointInUnits - invokeTouchPosition;
        // If the joystick is inside it's base, we keep it's position under a finger
        // sqrMagnitude is used for optimization, as Multiplication operation is much cheaper than Square Root
        if (dragDirection.sqrMagnitude <= joystickBaseRadius * joystickBaseRadius)
        {
            joystickTransform.localPosition = screenPointInUnits;
            dragDirection /= joystickBaseRadius;
        }
        // But if we drag our finger too far, joystick will remain at the border of it's base
        else
        {
            joystickTransform.localPosition = invokeTouchPosition + dragDirection.normalized * joystickBaseRadius;
            dragDirection.Normalize();
        }

        // If we're tweaking, we should dispatch our event
        if (JoystickMovedEvent != null)
        {
            JoystickMovedEvent(dragDirection);
        }
    }

    /**
     * Resetting BOTH joystick sprites to their initial position
     */
    void ResetJoystickPosition()
    {
        isTweaking = false;
        transformCache.localPosition = snapPosition;
        joystickBaseTransform.localPosition = Vector3.zero;
        joystickTransform.localPosition = Vector3.zero;
        myFingerId = -1;
    }

    /**
     * We need to convert our touch or mouse position to our local joystick position
     */
    void ScreenPointToRelativeFrustumPoint(Vector3 point)
    {
        // Percentage
        float screenPointXPercent = point.x / Screen.width;
        float screenPointYPercent = point.y / Screen.height;

        screenPointInUnits.x = screenPointXPercent * screenWidth;
        screenPointInUnits.y = screenPointYPercent * screenHeight;
        screenPointInUnits.z = 0f;
        /** OBSOLETE
        // Dirty magic again, finding super - local coordinates of the touch position
        joystickRelativePosition.x = screenPointXPercent * frustumWidth;
        joystickRelativePosition.y = screenPointYPercent * frustumHeight;
        joystickRelativePosition -= relativeExtentSummand;
        joystickRelativePosition.z = 0f;
         * */
    }

    // Sometimes when user lifts his finger, current touch index changes.
    // To keep track of our finger, we need to know which finger has the user lifted
    int FindMyFingerId()
    {
        int touchCount = Input.touchCount;
        for (int i = 0; i < touchCount; i++)
        {
            if (Input.GetTouch(i).fingerId == myFingerId)
            {
                // We return current Touch index if it's our finger
                return i;
            }
        }
        // And we return -1 if there's no such finger
        // Usually this happend after user lifts the finger which he touched first
        return -1;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Vector3 gizmoPosition = new Vector3(transform.position.x + tapZone.x, transform.position.y + tapZone.y, transform.position.z);
        Gizmos.DrawWireCube(gizmoPosition, new Vector3(tapZone.width, tapZone.height));
    }



}
