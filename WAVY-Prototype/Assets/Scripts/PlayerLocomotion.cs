using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerLocomotion : MonoBehaviour
{
    InputManager inputManager; // Interface for input management

    Vector3 moveDirection;          // Direction of movement based on input
    Transform cameraObject;         // Reference to the camera for directional movement
    Rigidbody playerRigidbody;      // Rigidbody component for physics-based movement

    [Header("Movement Speeds")]
    public float walkingSpeed = 2;
    public float runningSpeed = 7;
    public float rotationSpeed = 15;

    private void Start()
    {
        inputManager = GetComponent<InputManager>(); // Get the InputManager component
        cameraObject = Camera.main.transform; // Reference to the main camera's transform
        playerRigidbody = GetComponent<Rigidbody>(); // Get the Rigidbody component for physics interactions
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

        if (inputManager.moveAmount > 0.5f)
        {
            moveDirection *= runningSpeed;
        }
        else
        {
            moveDirection *= walkingSpeed;
        }

        Vector3 movementVelocity = moveDirection;
        playerRigidbody.linearVelocity = movementVelocity; // Adjust speed as needed
    }

    private void HandleRotation()
    {
        Vector3 targetDirection = Vector3.zero; // Initialize target direction

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
