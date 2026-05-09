using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class MovementController : MonoBehaviour
{
    [SerializeField] Transform cameraTransform;
    [SerializeField] float moveSpeed = 5f;
    [SerializeField] float sprintMultiplier = 1.75f;
    [SerializeField] float jumpForce = 5f;
    [SerializeField] float groundCheckDistance = 0.2f;
    [SerializeField] LayerMask groundLayer = ~0;
    PlayerInput playerInput;
    InputAction moveAction;
    InputAction sprintAction;
    InputAction jumpAction;

    Rigidbody rb;

    // Cached movement direction (calculated from input each Update,
    // applied in FixedUpdate so physics/collisions are respected)
    Vector3 movementDirection = Vector3.zero;

    void Start()
    {
        playerInput = GetComponent<PlayerInput>();
        moveAction = playerInput.actions["Move"];
        sprintAction = playerInput.actions["Sprint"];
        jumpAction = playerInput.actions["Jump"];

        rb = GetComponent<Rigidbody>();
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        rb.constraints = RigidbodyConstraints.FreezeRotation;
    }

    void Update()
    {
        ReadInputAndCalculateDirection();
    }

    // Used to request a jump during FixedUpdate
    bool jumpRequested = false;

    void FixedUpdate()
    {
        // Sprint input affects speed
        bool sprinting = false;
        if (sprintAction != null)
        {
            sprinting = sprintAction.ReadValue<float>() > 0.5f;
        }

        float speed = moveSpeed * (sprinting ? sprintMultiplier : 1f);

        // Apply movement with Rigidbody to enable collision response
        if (movementDirection != Vector3.zero)
        {
            Vector3 newPos = rb.position + movementDirection * speed * Time.fixedDeltaTime;
            rb.MovePosition(newPos);
        }

        // Apply jump if requested
        if (jumpRequested)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            jumpRequested = false;
        }
    }

    void ReadInputAndCalculateDirection()
    {
        Vector2 input = moveAction.ReadValue<Vector2>();

        if (input == Vector2.zero)
        {
            movementDirection = Vector3.zero;
            return;
        }

        Vector3 forward = cameraTransform.forward;
        Vector3 right = cameraTransform.right;
        forward.y = 0;
        right.y = 0;
        forward.Normalize();
        right.Normalize();

        movementDirection = (forward * input.y + right * input.x).normalized;

        // Handle jump input (request it for FixedUpdate so physics is consistent)
        if (jumpAction != null && jumpAction.triggered)
        {
            if (IsGrounded())
            {
                jumpRequested = true;
            }
        }
    }

    bool IsGrounded()
    {
        // Cast a short ray down to detect ground. Tunable via groundCheckDistance and groundLayer.
        return Physics.Raycast(transform.position, Vector3.down, groundCheckDistance + 0.01f, groundLayer);
    }
}