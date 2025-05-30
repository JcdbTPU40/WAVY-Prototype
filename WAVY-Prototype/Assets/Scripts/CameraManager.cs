using UnityEngine;

public class CameraManager : MonoBehaviour
{
    InputManager inputManager; // Reference to the InputManager

    public Transform targetTransform; // The target to follow
    public Transform cameraPivot; // The pivot point for camera rotation
    public Transform cameraTransform; // The camera transform
    public LayerMask collisionLayers; // Layers to check for collisions
    private float defaultPosition; // Default position of the camera
    private Vector3 cameraFollowVelocity = Vector3.zero;
    private Vector3 cameraVectorPosition; // Position of the camera relative to the pivot

    public float cameraCollisionOffSet = 0.2f; // Offset for camera collision detection
    public float minimumCollisionOffSet = 0.2f; // Minimum offset to prevent camera from getting too close
    public float cameraCollisonradius = 2f; // Radius for camera collision detection
    public float cameraFollowSpeed = 0.2f;
    public float cameraLookSpeed = 2; // Speed of camera rotation
    public float cameraPivotSpeed = 2; // Speed of camera pivoting

    public float lookAngle; // Camera looking up and down
    public float pivotAngle; // Camera looking left and right
    public float minimumPivotAngle = -35; // Minimum angle for camera pivot
    public float maximumPivotAngle = 35; // Maximum angle for camera pivot

    private void Awake()
    {
        inputManager = Object.FindAnyObjectByType<InputManager>();
        targetTransform = Object.FindAnyObjectByType<PlayerManager>().transform;
        cameraTransform = Camera.main.transform;
        defaultPosition = cameraTransform.localPosition.z; // Store the default position of the camera
    }

    public void HandleAllCameraMovement()
    {
        FollowTarget();
        RotateCamera();
        HandleCameraCollisions();
    }
    private void FollowTarget()
    {
        Vector3 targetPosition = Vector3.SmoothDamp(transform.position, targetTransform.position, ref cameraFollowVelocity, cameraFollowSpeed);

        transform.position = targetPosition;
    }

    private void RotateCamera()
    {
        Vector3 rotation;
        Quaternion targetRotation;

        lookAngle += (inputManager.cameraInputX * cameraLookSpeed);
        pivotAngle -= (inputManager.cameraInputY * cameraLookSpeed);
        pivotAngle = Mathf.Clamp(pivotAngle, minimumPivotAngle, maximumPivotAngle);

        rotation = Vector3.zero;
        rotation.y = lookAngle;
        targetRotation = Quaternion.Euler(rotation);
        transform.rotation = targetRotation;

        rotation = Vector3.zero;
        rotation.x = pivotAngle;
        targetRotation = Quaternion.Euler(rotation);
        cameraPivot.localRotation = targetRotation;
    }

    private void HandleCameraCollisions()
    {
        float targetPosition = defaultPosition;
        RaycastHit hit;
        Vector3 direction = cameraTransform.position - cameraPivot.position;
        direction.Normalize();

        if (Physics.SphereCast(cameraPivot.transform.position, cameraCollisonradius, direction, out hit, Mathf.Abs(targetPosition), collisionLayers))
        {
            float distance = Vector3.Distance(cameraPivot.position, hit.point);
            targetPosition =- (distance - cameraCollisionOffSet);
        }

        if(Mathf.Abs(targetPosition) < minimumCollisionOffSet)
        {
            targetPosition -= minimumCollisionOffSet;
        }

        cameraVectorPosition.z = Mathf.Lerp(cameraTransform.localPosition.z, targetPosition, 0.2f);
        cameraTransform.localPosition = cameraVectorPosition;
    }
}
