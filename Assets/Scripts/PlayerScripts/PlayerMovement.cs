using System;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("References")]
    public Rigidbody rb;
    public Transform head;
    public Camera camera;
    public Animator animator;

    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float lookSensitivity = 2f;
    public float maxLookAngle = 80f;
    public float jumpHeight = 2f; // Jump height

    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;

    private float xRotation = 0f;
    private bool isGrounded;
    private Vector3 moveDirection;

    private float mouseX, mouseY;

    // Gravity and velocity
    private float gravity = -9.81f;
    private float verticalVelocity = 0f;

    // Animation triggers for your animator
    private int animTriggerForward;
    private int animTriggerBackward;
    private int animTriggerLeft;
    private int animTriggerRight;
    private int animTriggerForwardLeft;
    private int animTriggerForwardRight;
    private int animTriggerBackwardLeft;
    private int animTriggerBackwardRight;

    void Start()
    {
        // Lock and hide cursor for first-person control
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // Make sure references are set
        if (rb == null) rb = GetComponent<Rigidbody>();
        if (head == null) head = transform.Find("Head");
        if (camera == null) camera = GetComponentInChildren<Camera>();
        if (animator == null) animator = GetComponent<Animator>();

        // Cache animator triggers for better performance
        if (animator != null)
        {
            animTriggerForward = Animator.StringToHash("Forward");
            animTriggerBackward = Animator.StringToHash("Backward");
            animTriggerLeft = Animator.StringToHash("Left");
            animTriggerRight = Animator.StringToHash("Right");
            animTriggerForwardLeft = Animator.StringToHash("ForwardLeft");
            animTriggerForwardRight = Animator.StringToHash("ForwardRight");
            animTriggerBackwardLeft = Animator.StringToHash("BackwardLeft");
            animTriggerBackwardRight = Animator.StringToHash("BackwardRight");

            Debug.Log("Animation triggers initialized");
        }
    }

    void Update()
    {
        // Ground check
        isGrounded = Physics.CheckSphere(groundCheck ? groundCheck.position : transform.position - new Vector3(0, 1f, 0),
                                        groundDistance, groundMask);

        // Handle mouse look
        HandleLook();

        // Process movement input
        HandleMovementInput();

        // Handle jump input
        HandleJumpInput();
    }

    void HandleMovementInput()
    {
        // Get standard input axes for smoother movement
        float horizontal = Input.GetAxisRaw("Horizontal"); // A/D or Left/Right arrows
        float vertical = Input.GetAxisRaw("Vertical");     // W/S or Up/Down arrows

        // Calculate move direction relative to where player is looking
        moveDirection = (transform.right * horizontal + transform.forward * vertical).normalized;

        // Update animation triggers based on movement direction
        UpdateAnimationState(horizontal, vertical);
    }

    void HandleJumpInput()
    {
        // If player presses the space key and is grounded, apply jump force
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity); // Jump formula
        }

        // Apply gravity
        if (isGrounded && verticalVelocity < 0)
        {
            verticalVelocity = -2f; // Prevents the player from floating above ground
        }
        else
        {
            verticalVelocity += gravity * Time.deltaTime; // Apply gravity if not grounded
        }
    }

    void UpdateAnimationState(float horizontal, float vertical)
    {
        if (animator == null) return;

        // Reset all triggers first
        animator.ResetTrigger(animTriggerForward);
        animator.ResetTrigger(animTriggerBackward);
        animator.ResetTrigger(animTriggerLeft);
        animator.ResetTrigger(animTriggerRight);
        animator.ResetTrigger(animTriggerForwardLeft);
        animator.ResetTrigger(animTriggerForwardRight);
        animator.ResetTrigger(animTriggerBackwardLeft);
        animator.ResetTrigger(animTriggerBackwardRight);

        // Set the appropriate trigger based on input
        if (vertical > 0.1f) // Moving forward
        {
            if (horizontal > 0.1f) // Forward + Right
            {
                animator.SetTrigger(animTriggerForwardRight);
            }
            else if (horizontal < -0.1f) // Forward + Left
            {
                animator.SetTrigger(animTriggerForwardLeft);
            }
            else // Forward only
            {
                animator.SetTrigger(animTriggerForward);
            }
        }
        else if (vertical < -0.1f) // Moving backward
        {
            if (horizontal > 0.1f) // Backward + Right
            {
                animator.SetTrigger(animTriggerBackwardRight);
            }
            else if (horizontal < -0.1f) // Backward + Left
            {
                animator.SetTrigger(animTriggerBackwardLeft);
            }
            else // Backward only
            {
                animator.SetTrigger(animTriggerBackward);
            }
        }
        else // No vertical movement
        {
            if (horizontal > 0.1f) // Right only
            {
                animator.SetTrigger(animTriggerRight);
            }
            else if (horizontal < -0.1f) // Left only
            {
                animator.SetTrigger(animTriggerLeft);
            }
            else // No movement (Idle)
            {
                // No trigger set, transition back to Idle
            }
        }
    }

    void FixedUpdate()
    {
        // Apply movement in FixedUpdate for consistent physics
        MovePlayer();
    }

    void MovePlayer()
    {
        // Move player using physics
        if (moveDirection.magnitude > 0.1f)
        {
            Vector3 targetVelocity = moveDirection * moveSpeed;
            targetVelocity.y = verticalVelocity; // Apply vertical velocity (jump + gravity)

            // Apply movement
            rb.linearVelocity = targetVelocity;
        }
        else
        {
            // Keep vertical velocity but stop horizontal movement
            rb.linearVelocity = new Vector3(0, verticalVelocity, 0);
        }
    }

    void HandleLook()
    {
        // Get mouse input for camera rotation
        mouseX = Input.GetAxis("Mouse X") * lookSensitivity;
        mouseY = Input.GetAxis("Mouse Y") * lookSensitivity;

        // Calculate the camera rotation for looking up and down (pitch)
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -maxLookAngle, maxLookAngle);

        // Apply rotation to the head (pitch for up and down)
        head.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        // Rotate the player body (yaw for left and right)
        transform.Rotate(Vector3.up * mouseX);
    }
}
