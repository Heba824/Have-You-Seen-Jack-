using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovementWitch : MonoBehaviour
{
    [Header("Movement Speeds")]
    [SerializeField] private float walkSpeed = 3.5f;
    [SerializeField] private float sprintSpeed = 7.0f;
    [SerializeField] private float turnSmoothTime = 0.1f;

    [Header("Jumping & Gravity")]
    [SerializeField] private float jumpHeight = 1.8f;
    [SerializeField] private float gravity = -19.62f;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundDistance = 0.3f;
    [SerializeField] private LayerMask groundMask;

    [Header("Camera Reference")]
    [SerializeField] private Transform cameraTransform;

    [Header("Camera Perspective Mode")]
    [Tooltip("Enable this when First-Person POV is active so the character body rotates strictly with camera direction.")]
    public bool isFirstPersonMode = false;

    private CharacterController controller;
    private Animator animator;
    private Vector3 velocity;
    private bool isGrounded;
    private float turnSmoothVelocity;

    private void Start()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();

        if (cameraTransform == null && Camera.main != null)
        {
            cameraTransform = Camera.main.transform;
        }
    }

    private void Update()
    {
        HandleGroundCheck();
        HandleMovement();
        HandleJump();
    }

    private void HandleGroundCheck()
    {
        // Spherical ground check at character's feet
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        // Constant small downward force while grounded to keep player attached to slopes/stairs
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        if (animator != null)
        {
            animator.SetBool("isGrounded", isGrounded);
        }
    }

   private void HandleMovement()
{
    float horizontal = Input.GetAxisRaw("Horizontal"); // A/D
    float vertical = Input.GetAxisRaw("Vertical");     // W/S
    Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;

    bool isMoving = direction.magnitude >= 0.1f;
    bool isSprinting = Input.GetKey(KeyCode.LeftShift) && isMoving;
    float currentSpeed = isSprinting ? sprintSpeed : walkSpeed;

    if (isFirstPersonMode)
    {
        // 1. Lock character body rotation to the camera's Y-yaw continuously
        // This ensures your body never turns left/right on its own during A/D strafing
        Vector3 cameraForward = cameraTransform.forward;
        cameraForward.y = 0f; // Keep rotation strictly on the horizontal plane
        
        if (cameraForward.sqrMagnitude > 0.001f)
        {
            transform.rotation = Quaternion.LookRotation(cameraForward);
        }

        // 2. Strafe relative to camera view
        if (isMoving)
        {
            Vector3 cameraRight = cameraTransform.right;
            cameraRight.y = 0f;

            // Strafe vector combines forward/back and left/right relative to view direction
            Vector3 moveDir = (cameraForward * vertical + cameraRight * horizontal).normalized;
            controller.Move(moveDir * (currentSpeed * Time.deltaTime));
        }
    }
    else
    {
        // THIRD-PERSON ROTATION: Smoothly rotate character toward movement vector
        if (isMoving)
        {
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cameraTransform.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            controller.Move(moveDir.normalized * (currentSpeed * Time.deltaTime));
        }
    }

    // Update Animator parameters
    // Update Animator parameters
    // Update Animator parameters
    if (animator != null)
    {
        animator.SetBool("isWalking", isMoving && !isSprinting);
        animator.SetBool("isRunning", isSprinting);
    }
}

    private void HandleJump()
    {
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            // Physics jump velocity: v = sqrt(2 * g * h)
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);

            if (animator != null)
            {
                animator.SetTrigger("JumpTrigger");
            }
        }

        // Apply downward acceleration due to gravity
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    private void OnDrawGizmosSelected()
    {
        // Visualizes the ground check sphere in Scene view for easy debugging
        if (groundCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundDistance);
        }
    }

    // Helper method to trigger mode changes programmatically from CameraController
    public void SetFirstPersonMode(bool active)
    {
        isFirstPersonMode = active;
    }
}