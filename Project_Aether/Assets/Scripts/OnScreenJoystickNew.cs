using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.OnScreen;
using System.ComponentModel;

/// <summary>  
/// A simple on-screen virtual joystick that injects Vector2 input into Unity's new Input System.  
/// Attach this script to a UI Panel (e.g., a child of your Canvas).  
/// Requires RectTransform references for the joystick background and handle.  
/// <br />
/// Copyright 2025 John Maksuta
/// </summary>  
[DisplayName("On-Screen Joystick (New Input System)")]
public class OnScreenJoystickNew : OnScreenControl, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    [Header("UI References")]
    [SerializeField]
    private RectTransform joystickHandle; // The handle that moves
    [SerializeField]
    private GameObject joystickBackground; // The background for the joystick

    [Header("Stick Settings")]
    [SerializeField]
    private float movementRange = 100f; // Max distance handle can move from center
    [SerializeField]
    private float deadZone = 0.1f; // Percentage of movement range for dead zone


    // This field stores the actual path string and is serialized in the Inspector
    [SerializeField]
    private string m_ControlPath = "<Gamepad>/leftStick"; // Default to left stick of a gamepad

    private Vector2 initialHandlePosition;
    private Vector2 currentPointerPosition;
    private RectTransform backgroundRect;

    /// <summary>
    /// This is the property that OnScreenControl requires you to override.
    /// It specifies the path to the control this on-screen element simulates.
    /// We want it to be a Vector2 control (like a gamepad stick). 
    /// </summary>
    protected override string controlPathInternal
    {
        get => m_ControlPath;
        set => m_ControlPath = value;
    }


    protected override void OnEnable()
    {
        base.OnEnable(); // Call the base class OnEnable

        if (joystickHandle == null)
        {
            Debug.LogError("Joystick Handle not assigned!");
            return;
        }
        if (joystickBackground == null)
        {
            Debug.LogError("Joystick Background not assigned!");
            return;
        }

        backgroundRect = GetComponent<RectTransform>(); // The RectTransform of this GameObject (which is the background)
        initialHandlePosition = joystickHandle.anchoredPosition;

        // Reset the handle to the center when enabled
        joystickHandle.anchoredPosition = initialHandlePosition;
        SendValueToControl(Vector2.zero); // Send zero input initially
    }

    protected override void OnDisable()
    {
        base.OnDisable();
    }

    void Awake()
    {
        
    }

    void Update()
    {
        
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        // Move the entire joystick (background and handle) to the touch point if desired
        // For a fixed joystick, you might skip this part.
        // For a "floating" joystick, this is where you'd reposition the background.
        // In this example, we assume the background is fixed.

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            backgroundRect,
            eventData.position,
            eventData.pressEventCamera,
            out currentPointerPosition
        );

        // Immediately update the handle position and send input
        UpdateStick(currentPointerPosition);
    }

    public void OnDrag(PointerEventData eventData)
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
           backgroundRect,
           eventData.position,
           eventData.pressEventCamera,
           out currentPointerPosition
       );

        UpdateStick(currentPointerPosition);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        // Reset handle to center
        joystickHandle.anchoredPosition = initialHandlePosition;
        // Send zero input when released
        SendValueToControl(Vector2.zero);
    }

    private void UpdateStick(Vector2 pointerLocalPosition)
    {
        // Calculate the raw offset from the center of the background
        Vector2 offset = pointerLocalPosition - initialHandlePosition;

        // Clamp the offset magnitude to the movement range
        Vector2 clampedOffset = Vector2.ClampMagnitude(offset, movementRange);

        // Move the handle
        joystickHandle.anchoredPosition = initialHandlePosition + clampedOffset;

        // Calculate the normalized direction (from -1 to 1)
        Vector2 normalizedDirection = clampedOffset / movementRange;

        // Apply dead zone
        if (normalizedDirection.magnitude < deadZone)
        {
            normalizedDirection = Vector2.zero;
        }
        else
        {
            // Re-normalize and scale by dead zone if outside
            normalizedDirection = normalizedDirection.normalized * ((normalizedDirection.magnitude - deadZone) / (1f - deadZone));
        }

        // Send the Vector2 value to the Input System
        // This is the core of how you use controlPathInternal with a Vector2
        // The SendValueToControl method automatically uses the 'controlPathInternal'
        // to direct the input to the correct virtual control.
        SendValueToControl(normalizedDirection);
    }

}
