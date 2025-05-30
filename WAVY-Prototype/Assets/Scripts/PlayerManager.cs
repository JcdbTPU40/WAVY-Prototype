using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    InputManager inputManager;
    CameraManager cameraManager; // Reference to the CameraManager
    PlayerLocomotion playerLocomotion;

    private void Awake()
    {
        inputManager = GetComponent<InputManager>();
        cameraManager = Object.FindAnyObjectByType<CameraManager>(); // Find the CameraManager in the scene 
        playerLocomotion = GetComponent<PlayerLocomotion>();
    }

    private void Update()
    {
        inputManager.HandleAllInputs(); // Handle all player inputs
    }

    private void FixedUpdate()
    {
        playerLocomotion.HandleAllMovement(); // Handle all player movement
    }

    private void LateUpdate()
    {
        cameraManager.HandleAllCameraMovement(); // Update camera position to follow the player
    }
}
