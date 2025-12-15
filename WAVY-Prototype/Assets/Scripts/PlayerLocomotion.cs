using UnityEngine;

public class PlayerLocomotion : MonoBehaviour
{
    InputManager inputManager;
    AnimatorManager animatorManager;

    Vector3 moveDirection;
    Transform cameraObject;
    CharacterController characterController; // CharacterController を使用（プロジェクト標準）
    Animator animator;

    [Header("Ult")]
    [SerializeField] PlayerHealth Ult;
    [SerializeField] float ultSpeed;

    [Header("Movement Speeds")]
    public float walkingSpeed = 2f;
    public float runningSpeed = 7f;
    public float rotationSpeed = 15f;

    [Header("Gravity Settings")]
    [SerializeField] float gravity = 9.8f;
    [SerializeField] float groundCheckDistance = 0.2f;
    [SerializeField] LayerMask groundLayers = ~0;
    [SerializeField] string groundTag = "Ground";
    float verticalVelocity;
    RaycastHit lastGroundHit;

    [Header("Animator Parameters")]
    [SerializeField] string speedParameterName = "Speed";
    [SerializeField] string movementTimeParameterName = "time";

    bool hasSpeedFloat;
    bool hasTimeFloat;
    float movementTime = 0f;

    [Header("物理押し出し")]
    public float pushPower = 2.0f;

    private void Start()
    {
        inputManager = GetComponent<InputManager>();
        animatorManager = GetComponent<AnimatorManager>();
        animator = GetComponent<Animator>();
        cameraObject = Camera.main.transform;
        characterController = GetComponent<CharacterController>();

        if (characterController == null)
        {
            Debug.LogError("CharacterController が見つかりません。Player に CharacterController を追加してください。", this);
        }

        if (animator != null)
        {
            hasSpeedFloat = HasAnimatorParameter(speedParameterName, AnimatorControllerParameterType.Float);
            hasTimeFloat = HasAnimatorParameter(movementTimeParameterName, AnimatorControllerParameterType.Float);
        }
    }

    public void HandleAllMovement()
    {
        HandleMovement();
        HandleRotation();
        UpdateAnimator();
    }

private void HandleMovement()
{
    if (characterController == null || inputManager == null) return;
    if (!EnsureCameraReference()) return;

    // --- ① カメラ相対の移動方向 ---
    moveDirection = cameraObject.forward * inputManager.verticalInput;
    moveDirection += cameraObject.right * inputManager.horizontalInput;
    moveDirection.y = 0f;

    if (moveDirection.sqrMagnitude > 1f)
        moveDirection.Normalize();

    // --- ② 速度 ---
    float currentSpeed;
    
    if(Ult.IsUlt)
        {
            currentSpeed=ultSpeed;
        }
        else
        {
            currentSpeed = inputManager.moveAmount > 0.5f ? runningSpeed : walkingSpeed;
        }
        Vector3 horizontalVelocity = moveDirection * currentSpeed;

    // --- ③ Ground 判定（Move 前）---
    bool grounded = IsGrounded(out lastGroundHit);

    if (grounded)
    {
        if (verticalVelocity < 0f)
            verticalVelocity = 0f;
    }
    else
    {
        verticalVelocity -= gravity * Time.deltaTime;
    }

    // --- ④ Move（重力込み）---
    Vector3 velocity = horizontalVelocity;
    velocity.y = verticalVelocity;

    characterController.Move(velocity * Time.deltaTime);

    // --- ⑤ Move 後の Ground 判定 & 吸着 ---
    bool groundedAfterMove = IsGrounded(out lastGroundHit);

    if (groundedAfterMove)
    {
        SnapToGround(lastGroundHit);
        verticalVelocity = 0f;
    }
}


    private void HandleRotation()
    {
        if (inputManager == null) return;
        if (!EnsureCameraReference()) return;

        Vector3 targetDirection = Vector3.zero;
        targetDirection = cameraObject.forward * inputManager.verticalInput;
        targetDirection += cameraObject.right * inputManager.horizontalInput;
        targetDirection.Normalize();
        targetDirection.y = 0;

        if (targetDirection == Vector3.zero)
        {
            targetDirection = transform.forward;
        }

        Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
        Quaternion playerRotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

        transform.rotation = playerRotation;
    }

    private void UpdateAnimator()
    {
        if (animator == null || inputManager == null) return;

        // 移動速度をアニメーターに反映
        if (hasSpeedFloat)
        {
            animator.SetFloat(speedParameterName, inputManager.moveAmount);
        }

        // 移動時間を更新
        if (inputManager.moveAmount > 0.1f)
        {
            movementTime += Time.deltaTime;
        }
        else
        {
            movementTime = 0f;
        }

        if (hasTimeFloat)
        {
            animator.SetFloat(movementTimeParameterName, movementTime);
        }

        // AnimatorManager との連携（存在する場合）
        if (animatorManager != null)
        {
            animatorManager.UpdateAnimatorValues(0, inputManager.moveAmount);
        }
    }

    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        Rigidbody body = hit.collider.attachedRigidbody;

        if (body == null || body.isKinematic)
        {
            return;
        }

        // 下方向の押し出しは無視
        if (hit.moveDirection.y < -0.3f)
        {
            return;
        }

        Vector3 pushDir = new Vector3(hit.moveDirection.x, 0, hit.moveDirection.z);
        body.AddForce(pushDir * pushPower, ForceMode.VelocityChange);
    }

    bool HasAnimatorParameter(string parameterName, AnimatorControllerParameterType type)
    {
        if (animator == null || string.IsNullOrEmpty(parameterName))
        {
            return false;
        }

        foreach (var parameter in animator.parameters)
        {
            if (parameter.type == type && parameter.name == parameterName)
            {
                return true;
            }
        }

        return false;
    }

    bool EnsureCameraReference()
    {
        if (cameraObject != null) return true;

        Camera mainCamera = Camera.main;
        if (mainCamera == null)
        {
            Debug.LogWarning("Camera.main が見つからないため、移動方向を計算できません", this);
            return false;
        }

        cameraObject = mainCamera.transform;
        return cameraObject != null;
    }

    bool IsGrounded(out RaycastHit groundHit)
{
    groundHit = default;
    if (characterController == null) return false;

    Bounds bounds = characterController.bounds;
    Vector3 origin = bounds.center;
    float radius = Mathf.Max(0.01f, characterController.radius - 0.02f);
    float rayLength = bounds.extents.y + groundCheckDistance;

    if (Physics.SphereCast(origin, radius, Vector3.down, out groundHit, rayLength, groundLayers, QueryTriggerInteraction.Ignore))
    {
        // Ground タグが設定されている場合だけフィルタする
        if (!string.IsNullOrEmpty(groundTag))
        {
            if (!groundHit.collider.CompareTag(groundTag))
            {
                // Ground じゃないものに当たったら接地扱いしない
                groundHit = default;
                return false;
            }
        }

        return true;
    }

    return false;
}

    void SnapToGround(RaycastHit groundHit)
    {
        if (characterController == null) return;
        if (groundHit.collider == null) return;

        Bounds bounds = characterController.bounds;
        float bottom = bounds.center.y - bounds.extents.y;
        float targetBottom = groundHit.point.y + characterController.skinWidth;
        float offset = targetBottom - bottom;

        if (offset > 0f)
        {
            characterController.Move(Vector3.up * offset);
        }
    }
}
