using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class MovementController : MonoBehaviour
{
    [SerializeField] Transform cameraTransform;
    [SerializeField] float moveSpeed = 5f;
    [SerializeField] float sprintMultiplier = 1.75f;
    [SerializeField] float jumpForce = 5f;
    [SerializeField] float mass = 1f;
    [SerializeField] float turnSpeed = 800f;
    [SerializeField] float groundCheckDistance = 0.5f;
    [SerializeField] PhysicsMaterial playerPhysicsMaterial;
    [SerializeField] LayerMask groundLayer;  // Set this in inspector to specific ground/asset layers, NOT "Everything"
    PlayerInput playerInput;
    InputAction moveAction;
    InputAction sprintAction;
    InputAction jumpAction;

    Rigidbody rb;

    // Cached movement direction (calculated from input each Update,
    // applied in FixedUpdate so physics/collisions are respected)
    Vector3 movementDirection = Vector3.zero;
    bool wasJumpPressedLastFrame = false;

    void Start()
    {
        playerInput = GetComponent<PlayerInput>();
        moveAction = playerInput.actions["Move"];
        sprintAction = playerInput.actions["Sprint"];
        jumpAction = playerInput.actions["Jump"];

        rb = GetComponent<Rigidbody>();
        rb.mass = mass;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        rb.constraints = RigidbodyConstraints.FreezeRotation;

        SetupLowFrictionColliders();
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

        // Apply movement through velocity so physics can resolve collisions.
        Vector3 velocity = rb.linearVelocity;
        Vector3 desiredHorizontalVelocity = movementDirection * speed;
        velocity.x = desiredHorizontalVelocity.x;
        velocity.z = desiredHorizontalVelocity.z;
        rb.linearVelocity = velocity;

        // Face the direction of movement on the Y axis only.
        if (movementDirection != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(movementDirection, Vector3.up);
            Quaternion smoothedRotation = Quaternion.RotateTowards(rb.rotation, targetRotation, turnSpeed * Time.fixedDeltaTime);
            rb.MoveRotation(Quaternion.Euler(0f, smoothedRotation.eulerAngles.y, 0f));
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
        // Handle jump input first, before early return (so jump works even without movement)
        bool jumpPressed = jumpAction != null && jumpAction.IsPressed();

        // Detect transition from not pressed to pressed
        if (jumpPressed && !wasJumpPressedLastFrame)
        {
            if (IsGrounded())
            {
                jumpRequested = true;
            }
        }

        wasJumpPressedLastFrame = jumpPressed;

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
    }

    bool IsGrounded()
    {
        // Use SphereCast to detect ground, accounting for player collider size
        Vector3 spherePos = transform.position + Vector3.down * 0.5f;
        float sphereRadius = 0.45f;

        // Try with the specified groundLayer
        bool hit = Physics.SphereCast(spherePos, sphereRadius, Vector3.down, out RaycastHit hitInfo, groundCheckDistance, groundLayer);

        Debug.DrawRay(spherePos, Vector3.down * groundCheckDistance, hit ? Color.green : Color.red, 0.01f);
        return hit;
    }

    void SetupLowFrictionColliders()
    {
        PhysicsMaterial lowFrictionMaterial = playerPhysicsMaterial;

        if (lowFrictionMaterial == null)
        {
            lowFrictionMaterial = new PhysicsMaterial("PlayerLowFriction");
            lowFrictionMaterial.dynamicFriction = 0f;
            lowFrictionMaterial.staticFriction = 0f;
            lowFrictionMaterial.bounciness = 0f;
            lowFrictionMaterial.frictionCombine = PhysicsMaterialCombine.Minimum;
            lowFrictionMaterial.bounceCombine = PhysicsMaterialCombine.Minimum;
        }

        Collider[] colliders = GetComponents<Collider>();
        for (int i = 0; i < colliders.Length; i++)
        {
            colliders[i].material = lowFrictionMaterial;
        }
    }
}