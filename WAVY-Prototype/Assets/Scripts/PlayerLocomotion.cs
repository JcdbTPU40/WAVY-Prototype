using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerLocomotion : MonoBehaviour
{
    InputManager inputManager;

    Vector3 moveDirection;
    Transform cameraObject;
    Rigidbody playerRigidbody;

    public float movementSpeed = 7; // Speed of the player movement
    public float rotationSpeed = 15; // Speed of the player rotation

    private void Awake()
    {
        inputManager = GetComponent<InputManager>();
        playerRigidbody = GetComponent<Rigidbody>();
        cameraObject = Camera.main.transform; // Get the main camera's transform
    }

    public void HandleAllMovement()
    {
        HandleMovement(); // Handle player movement
        HandleRotation(); // Handle player rotation
    }

    private void HandleMovement()
    {
        moveDirection = cameraObject.forward * inputManager.verticalInput; // Movement Input
        moveDirection += cameraObject.right * inputManager.horizontalInput; // Horizontal Input
        moveDirection.Normalize(); // Normalize to ensure consistent speed
        moveDirection.y = 0; // Keep movement on the horizontal plane
        moveDirection *= movementSpeed; // Apply speed to the movement direction

        Vector3 movementVelocity = moveDirection;
        playerRigidbody.linearVelocity = movementVelocity; // Adjust speed as needed
    }

    private void HandleRotation()
    {
        Vector3 targetDirection = Vector3.zero;

        targetDirection = cameraObject.forward * inputManager.verticalInput; // Forward movement
        targetDirection += cameraObject.right * inputManager.horizontalInput; // Right movement
        targetDirection.Normalize(); // Normalize to ensure consistent direction
        targetDirection.y = 0; // Keep rotation on the horizontal plane

        if (targetDirection == Vector3.zero) // If no input, do not rotate
        {
            targetDirection = transform.forward; // Maintain current forward direction
        }

        Quaternion targetRotation = Quaternion.LookRotation(targetDirection); // Create rotation based on movement direction
        Quaternion playerRotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime); // Smoothly rotate towards the target direction

        transform.rotation = playerRotation; // Apply the rotation to the player
    }
}
