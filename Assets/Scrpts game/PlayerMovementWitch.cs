using UnityEngine;

public class PlayerMovementWitch : MonoBehaviour
{
    public float moveSpeed = 4.0f;
    public float turnSpeed = 10.0f;
    public Transform cameraTransform;

    private CharacterController controller;
    private Animator animator;
    private float gravity = -9.81f;
    private Vector3 velocity;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();

        if (cameraTransform == null && Camera.main != null)
        {
            cameraTransform = Camera.main.transform;
        }
    }

    void Update()
    {
        // 1. Reset downward force when touching ground
        if (controller.isGrounded && velocity.y < 0)
        {
            velocity.y = -2f; // Small constant force to keep her snapped to slopes/floor
        }

        // 2. Input and movement relative to camera
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;

        bool isMoving = direction.magnitude >= 0.1f;

        if (animator != null)
        {
            animator.SetBool("isWalking", isMoving);
        }

        if (isMoving)
        {
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cameraTransform.eulerAngles.y;
            float angle = Mathf.LerpAngle(transform.eulerAngles.y, targetAngle, Time.deltaTime * turnSpeed);

            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            Vector3 moveDirection = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            controller.Move(moveDirection.normalized * moveSpeed * Time.deltaTime);
        }

        // 3. Apply standard gravity
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }
}