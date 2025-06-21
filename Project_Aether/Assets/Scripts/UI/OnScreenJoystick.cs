using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class OnScreenJoystick : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    [Header("Joystick References")]
    [SerializeField]
    private RectTransform joystickBackground;
    [SerializeField]
    private RectTransform joystickHandle;

    /// <summary>
    /// The max distance handle can move from center.
    /// </summary>
    [Header("Settings"), Tooltip("The max Distance handler can move from the center.")]
    [SerializeField]
    private float joystickRange = 100f;
    /// <summary>
    /// How far the handle must move before input registers.
    /// </summary>
    [SerializeField]
    private float deadZoneRadius = 0.1f;

    private Vector2 joystickOriginalPos;
    /// <summary>
    /// stoe the normalized direction (x, y).
    /// </summary>
    private Vector2 inputDirection;

    /// <summary>
    /// Public property to get the current input direction.
    /// </summary>
    public Vector2 Direction => inputDirection;

    // --- NEW: Define a static event for movement input ---
    // 'public static' means any script can access it without a direct reference to an OnScreenJoystick instance.
    // 'event Action<Vector2>' means it's an event that takes a Vector2 as an argument.
    public static event Action<Vector2> OnMoveInput;
    // --- END NEW ---

    void Start()
    {
        joystickOriginalPos = joystickBackground.anchoredPosition;
        joystickHandle.anchoredPosition = Vector2.zero;
        inputDirection = Vector2.zero;
        // Optionally, ensure the event is triggered with zero when starting
        OnMoveInput?.Invoke(inputDirection); // Null conditional operator protects against no listeners
    }



    // Update is called once per frame
    void Update()
    {

    }

    // Called while the pointer is dragging on the joystick background.
    public void OnDrag(PointerEventData eventData)
    {
        // Get the local position of the touch relative to the joystick background
        RectTransformUtility.ScreenPointToLocalPointInRectangle(joystickBackground, eventData.position, eventData.pressEventCamera, out Vector2 localPoint);

        // Calculate the clamped position of the handle.
        Vector2 clampedPosition = Vector2.ClampMagnitude(localPoint, joystickRange);

        // Move the handle
        joystickHandle.anchoredPosition = clampedPosition;

        Vector2 newDirection = clampedPosition / joystickRange;

        // Apply dead zone
        if (newDirection.magnitude < deadZoneRadius)
        {
            newDirection = Vector2.zero;// Reset direction if within dead zone
            //joystickHandle.anchoredPosition = Vector2.zero; // Reset handle position
        }

        // --- NEW: Only invoke the event if the direction has actually changed ---
        if (newDirection != inputDirection)
        {
            inputDirection = newDirection;
            OnMoveInput?.Invoke(inputDirection); // Invoke the event with the new direction
        }
        // --- END NEW ---
    }

    // called when pointer (finger) touches the joystick background.
    public void OnPointerDown(PointerEventData eventData)
    {
        // Move joystick background to the touch position (optional, good for floating joysticks)
        joystickBackground.anchoredPosition = eventData.position - joystickBackground.sizeDelta * 0.5f;
        // immediately start dragging on touch down
        OnDrag(eventData);
    }

    // Called when the pointer (finger) is lifted from the joystick background.
    public void OnPointerUp(PointerEventData eventData)
    {
        // Reset handle position to center
        joystickHandle.anchoredPosition = Vector2.zero;
        // Reset input direction    
        inputDirection = Vector2.zero;

        // Reset joystick background position to original if it was floating
        joystickBackground.anchoredPosition = joystickOriginalPos;

        // --- NEW: Always invoke the event with zero when joystick is released ---
        OnMoveInput?.Invoke(inputDirection);
        // --- END NEW ---
    }

    // For debugging in editor
    private void OnGUI()
    {
        GUI.Label(new Rect(10, 10, 200, 30), $"Input: {inputDirection.x:F2},{inputDirection.y:F2}");
    }

}
