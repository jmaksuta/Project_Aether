using UnityEngine;

public class PlayerMovementAnimationController : MonoBehaviour
{
    private Animator animator;
    public float walkSpeed = 2f;
    public float runSpeed = 5f;
    public float rotationSpeed = 100f;

    private void Awake()
    {
        animator = GetComponentInChildren<Animator>();
        if (animator == null)
        {
            Debug.LogError("Animator component not found in children of " + gameObject.name);
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (animator == null)
        {
            return; // Exit if animator is not set
        }

        float horisontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        Vector3 moveDirection = new Vector3(horisontalInput, 0f, verticalInput).normalized;

        bool isMoving = moveDirection.magnitude > 0.1f;
        bool isRunning = isMoving && Input.GetKey(KeyCode.LeftShift);
        bool isWalking = isMoving && !isRunning;

        animator.SetBool("isWalking", isWalking);
        animator.SetBool("isRunning", isRunning);
        animator.SetBool("isIdle", !isMoving);

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
        if (isWalking)
        {
            transform.Translate(moveDirection * walkSpeed * Time.deltaTime, Space.World);
        }
        if (isRunning)
        {
            transform.Translate(moveDirection * runSpeed * Time.deltaTime, Space.World);
        }

        // character rotation
        if (moveDirection.magnitude > 0)
        {
            Quaternion targetRotation = Quaternion.LookRotation(new Vector3(moveDirection.x, 0, moveDirection.z));
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }

        // Action Trigger (Pick Up)
        if (Input.GetKeyDown(KeyCode.E))
        {
            animator.SetTrigger("TriggerPickUp");
            // TODO: add actual pickup game logic here.

        }
    }
}
