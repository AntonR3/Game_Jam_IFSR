using UnityEngine;
using UnityEngine.InputSystem;

public class MovementController : MonoBehaviour
{
    [SerializeField] Transform cameraTransform;
    [SerializeField] float moveSpeed = 5f;
    PlayerInput playerInput;
    InputAction moveAction;

    void Start()
    {
        playerInput = GetComponent<PlayerInput>();
        moveAction = playerInput.actions["Move"];

    }

    void Update()
    {
        MovePlayer();
    }

    void MovePlayer()
    {
        Vector2 input = moveAction.ReadValue<Vector2>();
        // 1. Get camera directions
        Vector3 forward = cameraTransform.forward;
        Vector3 right = cameraTransform.right;

        // 2. Flatten them on the Y axis
        forward.y = 0;
        right.y = 0;

        // 3. Normalize to keep speed consistent
        forward.Normalize();
        right.Normalize();
        Vector3 direction = Vector3.zero;
        switch (input)
        {
            case Vector2 zero when zero == Vector2.zero:
                Debug.Log("No input detected, player will not move.");
                return;
            case { x: 1, y: 0 }:
                direction = new Vector3(right.x, 0, right.z);
                break;
            case { x: -1, y: 0 }:
                direction = new Vector3(-right.x, 0, -right.z);
                break;
            case { x: 0, y: 1 }:
                direction = new Vector3(forward.x, 0, forward.z);
                break;
            case { x: 0, y: -1 }:
                direction = new Vector3(-forward.x, 0, -forward.z);
                break;
        }
        transform.position += direction.normalized * moveSpeed * Time.deltaTime;
        Debug.Log("Moving player in direction: " + direction);
        
    }
}