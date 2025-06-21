using System;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovementAnimationController : MonoBehaviour
{
    public Animator animator;
    private CharacterController characterController;
    //public CharacterCustomizationManager characterCustomizationManager;
    // Reference to your PlayerInputActions asset
    // You can drag your 'PlayerInputActions' asset here in the Inspector
    [SerializeField]
    private PlayerControls playerInputActions;
    // Store the current movement input
    private Vector2 currentMoveInput;


    public float walkSpeed = 2f;
    public float runSpeed = 5f;
    public float rotationSpeed = 100f;
    public float rotationOffset = 0f; // Offset for character rotation, if needed

    public Boolean CanMove;

    private DateTime idleStartTime = DateTime.MinValue;

    private bool isPickUp = false;
    private bool isUse = false;
    private bool isCrouching = false;
    private bool isRunning = false;
    // TODO: isRunning = isMoving && Input.GetKey(KeyCode.LeftShift);
    private void Awake()
    {
        if (animator == null)
        {
            Debug.LogError("Animator component is not assigned in the inspector for " + gameObject.name);
            animator = GetComponentInChildren<Animator>();
            if (animator == null)
            {
                Debug.LogError("Animator component not found in children of " + gameObject.name);
            }
        }
        characterController = GetComponent<CharacterController>();
        if (characterController == null)
        {
            Debug.LogError("CharacterController component not found on " + gameObject.name);
        }
        // Initialize PlayerInputActions if not already set (e.g., if created via PlayerInput component)
        if (playerInputActions == null)
        {
            playerInputActions = new PlayerControls();
        }
        // Subscribe to the Move action's events
        // This will update 'currentMoveInput' whenever the Move action is performed or canceled
        playerInputActions.Player.Move.performed += OnMovePerformed;
        playerInputActions.Player.Move.canceled += OnMoveCanceled;
        playerInputActions.Player.UseItem.performed += OnUseItemPerformed;
        playerInputActions.Player.PickUp.performed += OnPickUpPerformed;
    }

    void OnEnable()
    {
        playerInputActions.Enable(); // Enable the action map when the script is enabled
    }

    void OnDisable()
    {
        playerInputActions.Disable(); // Disable the action map when the script is disabled

        // Unsubscribe to prevent memory leaks or calling on destroyed objects
        playerInputActions.Player.Move.performed -= OnMovePerformed;
        playerInputActions.Player.Move.canceled -= OnMoveCanceled;
        playerInputActions.Player.UseItem.performed -= OnUseItemPerformed;
        playerInputActions.Player.PickUp.performed -= OnPickUpPerformed;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    void Update()
    {
        // If you want to update animation parameters every frame regardless of input changes,
        // you can call UpdateAnimationParameters() here.
        // However, it's often more efficient to only update when input actually changes,
        // as done in OnMovePerformed/OnMoveCanceled.
        // For continuous values like movement, calling it in Update() might feel smoother
        // if your animation blend tree is complex and relies on small, continuous changes.
        // Experiment with what feels best for your specific animation setup.
        // For simplicity, calling it in Update() is fine if performance isn't a bottleneck.
        updatePlayer();
        UpdateAnimationParameters();
    }


    // Update is called once per frame
    void UpdateOld()
    {
        if (animator == null || characterController == null)
        {
            return; // Exit if animator is not set
        }

        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        // --- Calculate World-Relative Movement Direction ---
        // For a top-down game where W is always "up" (world Z) and A is "left" (world -X)
        // moveDirection is directly based on world axes.
        Vector3 moveDirection = new Vector3(horizontalInput, 0f, verticalInput).normalized;

        //Vector3 moveDirection = new Vector3(horizontalInput, 0f, verticalInput).normalized;

        bool isMoving = moveDirection.magnitude > 0.1f;
        bool isRunning = isMoving && Input.GetKey(KeyCode.LeftShift);
        bool isWalking = isMoving && !isRunning;

        DateTime dateNow = DateTime.Now;
        if (!isMoving && idleStartTime == DateTime.MinValue)
        {
            idleStartTime = DateTime.Now;
        }
        else if (isMoving)
        {
            idleStartTime = DateTime.MinValue;
        }
        TimeSpan difference = dateNow.Subtract(idleStartTime);

        animator.SetBool("IsWalking", isWalking);
        animator.SetBool("IsRunning", isRunning);


        float currentSpeed = 0f;
        if (isWalking)
        {
            currentSpeed = 0.5f;
        }
        if (isRunning)
        {
            currentSpeed = 1f;
        }
        animator.SetFloat("Speed", currentSpeed);

        // if animations bake root motion, the animator component handles movement.
        // otherwise, apply movement here.
        if (CanMove)
        {
            //if (isWalking)
            //{
            //    transform.Translate(moveDirection * walkSpeed * Time.deltaTime, Space.World);
            //}
            //if (isRunning)
            //{
            //    transform.Translate(moveDirection * runSpeed * Time.deltaTime, Space.World);
            //}

            float actualMoveSpeed = 0f;
            if (isWalking)
            {
                actualMoveSpeed = walkSpeed;
            }
            else if (isRunning)
            {
                actualMoveSpeed = runSpeed;
            }
            // Apply movement using the world-relative moveDirection
            characterController.Move(moveDirection * actualMoveSpeed * Time.deltaTime);
        }

        // character rotation
        //if (moveDirection.magnitude > 0)
        //{
        //    Quaternion targetRotation = Quaternion.LookRotation(new Vector3(moveDirection.x, 0, moveDirection.z));
        //    transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        //}

        // --- Character Rotation (rotate towards the world-relative movement direction) ---
        if (moveDirection.magnitude > 0)
        {
            // The character should face the direction they are moving in world space.
            Quaternion targetRotation = Quaternion.LookRotation(new Vector3(moveDirection.x, 0, moveDirection.z));
            //transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

            // Define the offset rotation. Common values:
            // Quaternion offsetRotation = Quaternion.Euler(0, 90, 0);   // If model faces X when it should face Z
            // Quaternion offsetRotation = Quaternion.Euler(0, -90, 0);  // If model faces -X when it should face Z
            // Quaternion offsetRotation = Quaternion.Euler(0, 180, 0); // If model faces -Z when it should face Z

            // Pick the correct offset based on your observation:
            Quaternion offsetRotation = Quaternion.Euler(0, rotationOffset, 0); // Example: if model is 90 degrees too far clockwise

            // Apply the offset rotation to the target rotation
            targetRotation *= offsetRotation; // Multiply to apply the offset

            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, 360);
        }

        bool isPickUp = Input.GetKeyDown(KeyCode.E);
        bool isUse = Input.GetKeyDown(KeyCode.F);
        // Action Trigger (Pick Up)
        if (isPickUp)
        {
            animator.SetTrigger("TriggerPickUp");
            // TODO: add actual pickup game logic here.

        }
        if (isUse)
        {
            animator.SetTrigger("TriggerUse");
        }
        //bool isCrouching = Input.GetKey(KeyCode.C);
        //animator.SetBool("IsCrouching", isCrouching);
        SetAnimationValue("IsCrouching", isCrouching);


        bool isAction = (isPickUp || isCrouching);

        bool isIdle = !isMoving && !isAction && (difference.TotalSeconds > 2);
        //animator.SetBool("IsIdle", isIdle);
        SetAnimationValue("IsIdle", isIdle);
    }


    private void updatePlayer()
    {
        //if (animator == null || characterController == null)
        //{
        //    return; // Exit if animator is not set
        //}

        //float horizontalInput = Input.GetAxis("Horizontal");
        //float verticalInput = Input.GetAxis("Vertical");

        // --- Calculate World-Relative Movement Direction ---
        // For a top-down game where W is always "up" (world Z) and A is "left" (world -X)
        // moveDirection is directly based on world axes.
        Vector3 moveDirection = new Vector3(currentMoveInput.x, 0f, currentMoveInput.y).normalized;

        //Vector3 moveDirection = new Vector3(horizontalInput, 0f, verticalInput).normalized;

        bool isMoving = moveDirection.magnitude > 0.1f;
        //bool isRunning = isMoving && Input.GetKey(KeyCode.LeftShift);
        bool isWalking = isMoving && !isRunning;

        DateTime dateNow = DateTime.Now;
        if (!isMoving && idleStartTime == DateTime.MinValue)
        {
            idleStartTime = DateTime.Now;
        }
        else if (isMoving)
        {
            idleStartTime = DateTime.MinValue;
        }
        TimeSpan difference = dateNow.Subtract(idleStartTime);

        animator.SetBool("IsWalking", isWalking);
        animator.SetBool("IsRunning", isRunning);


        float currentSpeed = 0f;
        if (isWalking)
        {
            currentSpeed = 0.5f;
        }
        if (isRunning)
        {
            currentSpeed = 1f;
        }
        animator.SetFloat("Speed", currentSpeed);

        // TODO: the move magnitude.
        //float speed = currentMoveInput.magnitude;
        //animator.SetFloat("Speed", speed);

        // if animations bake root motion, the animator component handles movement.
        // otherwise, apply movement here.
        if (CanMove)
        {
            //if (isWalking)
            //{
            //    transform.Translate(moveDirection * walkSpeed * Time.deltaTime, Space.World);
            //}
            //if (isRunning)
            //{
            //    transform.Translate(moveDirection * runSpeed * Time.deltaTime, Space.World);
            //}

            float actualMoveSpeed = 0f;
            if (isWalking)
            {
                actualMoveSpeed = walkSpeed;
            }
            else if (isRunning)
            {
                actualMoveSpeed = runSpeed;
            }
            // Apply movement using the world-relative moveDirection
            characterController.Move(moveDirection * actualMoveSpeed * Time.deltaTime);
        }

        // character rotation
        //if (moveDirection.magnitude > 0)
        //{
        //    Quaternion targetRotation = Quaternion.LookRotation(new Vector3(moveDirection.x, 0, moveDirection.z));
        //    transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        //}

        // --- Character Rotation (rotate towards the world-relative movement direction) ---
        if (moveDirection.magnitude > 0)
        {
            // The character should face the direction they are moving in world space.
            Quaternion targetRotation = Quaternion.LookRotation(new Vector3(moveDirection.x, 0, moveDirection.z));
            //transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

            // Define the offset rotation. Common values:
            // Quaternion offsetRotation = Quaternion.Euler(0, 90, 0);   // If model faces X when it should face Z
            // Quaternion offsetRotation = Quaternion.Euler(0, -90, 0);  // If model faces -X when it should face Z
            // Quaternion offsetRotation = Quaternion.Euler(0, 180, 0); // If model faces -Z when it should face Z

            // Pick the correct offset based on your observation:
            Quaternion offsetRotation = Quaternion.Euler(0, rotationOffset, 0); // Example: if model is 90 degrees too far clockwise

            // Apply the offset rotation to the target rotation
            targetRotation *= offsetRotation; // Multiply to apply the offset

            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, 360);
        }

        //bool isPickUp = false; // playerInputActions.Player.PickUp; // Input.GetKeyDown(KeyCode.E);
        //bool isUse = false; // playerInputActions.Player.UseItem.is; // Input.GetKeyDown(KeyCode.F);
        //// Action Trigger (Pick Up)
        if (this.isPickUp)
        {
            animator.SetTrigger("TriggerPickUp");
            // TODO: add actual pickup game logic here.

        }
        if (this.isUse)
        {
            animator.SetTrigger("TriggerUse");
        }
        //bool isCrouching = Input.GetKey(KeyCode.C);
        //animator.SetBool("IsCrouching", isCrouching);
        SetAnimationValue("IsCrouching", isCrouching);


        bool isAction = (isPickUp || isCrouching);

        bool isIdle = !isMoving && !isAction && (difference.TotalSeconds > 2);
        //animator.SetBool("IsIdle", isIdle);
        SetAnimationValue("IsIdle", isIdle);
    }


    private void SetAnimationValue(String animationVariable, bool value)
    {
        if (animator != null)
        {
            animator.SetBool(animationVariable, value);
        }
        //if (characterCustomizationManager != null)
        //{
        //    characterCustomizationManager.SetAnimationValue(animationVariable, value);
        //}
    }

    private void SetAnimationValue(String animationVariable, float value)
    {
        if (animator != null)
        {
            animator.SetFloat(animationVariable, value);
        }
        //if (characterCustomizationManager != null)
        //{
        //    characterCustomizationManager.SetAnimationValue(animationVariable, value);
        //}
    }

    private void SetAnimationValue(String animationVariable)
    {
        if (animator != null)
        {
            animator.SetTrigger(animationVariable);
        }
        //if (characterCustomizationManager != null)
        //{
        //    characterCustomizationManager.SetAnimationValue(animationVariable);
        //}
    }


    // Callback for when the Move action is performed (e.g., WASD pressed, joystick moved)
    private void OnMovePerformed(InputAction.CallbackContext context)
    {
        this.currentMoveInput = context.ReadValue<Vector2>();
        //UpdateAnimationParameters(); // Update animations immediately
        updatePlayer();
    }

    // Callback for when the Move action is canceled (e.g., WASD released, joystick centered)
    private void OnMoveCanceled(InputAction.CallbackContext context)
    {
        this.currentMoveInput = Vector2.zero;
        //UpdateAnimationParameters(); // Update animations immediately
        updatePlayer();
    }

    private void OnUseItemPerformed(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            this.isUse = true;
            updatePlayer();
            //SetAnimationValue("TriggerUse");
            isUse = false;
        }
    }

    private void OnPickUpPerformed(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            this.isPickUp = true;
            updatePlayer();
            //SetAnimationValue("TriggerPickUp");
            isPickUp = false;
        }
    }

    private void UpdateAnimationParameters()
    {
        if (animator == null) return;

        // Use the currentMoveInput to set your animator parameters
        // Assuming your Animator has 'Horizontal' and 'Vertical' float parameters
        animator.SetFloat("Horizontal", currentMoveInput.x);
        animator.SetFloat("Vertical", currentMoveInput.y);

        // You might also want to set a 'Speed' parameter based on the magnitude
        // For a blend tree from idle to walk/run
        float speed = currentMoveInput.magnitude;
        animator.SetFloat("Speed", speed);

        // Example for a 'IsMoving' boolean parameter
        animator.SetBool("IsMoving", speed > 0.1f); // Use a small threshold to avoid jitter
    }
    // --- You'll need similar methods for Jump, Crouch, etc., if they also affect animation ---

    // Example for Jump animation:
    // This assumes you have a 'JumpTrigger' in your Animator.
    // Make sure your PlayerInputActions has a 'Jump' action.
    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (animator != null)
            {
                animator.SetTrigger("JumpTrigger");
            }
        }
    }

    // Example for Crouch animation:
    // This assumes you have an 'IsCrouching' boolean in your Animator.
    // Make sure your PlayerInputActions has a 'Crouch' action (and it uses Hold interaction).
    public void OnCrouch(InputAction.CallbackContext context)
    {
        if (animator != null)
        {
            if (context.performed)
            {
                animator.SetBool("IsCrouching", true);
            }
            else if (context.canceled)
            {
                animator.SetBool("IsCrouching", false);
            }
        }
    }

    public void OnPickUp(InputAction.CallbackContext context)
    {
        if (animator != null)
        {
            if (context.performed)
            {
                animator.SetBool("IsCrouching", true);
            }
            else if (context.canceled)
            {
                animator.SetBool("IsCrouching", false);
            }
        }
    }
}
