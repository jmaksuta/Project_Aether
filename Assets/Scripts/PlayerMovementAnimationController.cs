using System;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovementAnimationController : MonoBehaviour
{
    public Animator animator;
    private CharacterController characterController;
    public CharacterCustomizationManager characterCustomizationManager;

    public float walkSpeed = 2f;
    public float runSpeed = 5f;
    public float rotationSpeed = 100f;
    public float rotationOffset = 0f; // Offset for character rotation, if needed

    public Boolean CanMove;

    private DateTime idleStartTime = DateTime.MinValue;

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
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
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
        bool isCrouching = Input.GetKey(KeyCode.C);
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
        if (characterCustomizationManager != null)
        {
            characterCustomizationManager.SetAnimationValue(animationVariable, value);
        }
    }

    private void SetAnimationValue(String animationVariable, float value)
    {
        if (animator != null)
        {
            animator.SetFloat(animationVariable, value);
        }
        if (characterCustomizationManager != null)
        {
            characterCustomizationManager.SetAnimationValue(animationVariable, value);
        }
    }

    private void SetAnimationValue(String animationVariable)
    {
        if (animator != null)
        {
            animator.SetTrigger(animationVariable);
        }
        if (characterCustomizationManager != null)
        {
            characterCustomizationManager.SetAnimationValue(animationVariable);
        }
    }
}
