using UnityEngine;

public class PlayerLocomotion : MonoBehaviour
{
    InputManager inputManager;
    AnimatorManager animatorManager;

    Vector3 moveDirection;
    Transform cameraObject;
    CharacterController characterController; // CharacterController を使用（プロジェクト標準）
    Animator animator;

    [Header("Movement Speeds")]
    public float walkingSpeed = 2f;
    public float runningSpeed = 7f;
    public float rotationSpeed = 15f;

    [Header("Gravity Settings")]
    public float gravity = 9.8f;
    private float verticalVelocity = 0f;

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

        // カメラ相対の移動方向を計算
        moveDirection = cameraObject.forward * inputManager.verticalInput;
        moveDirection += cameraObject.right * inputManager.horizontalInput;
        moveDirection.Normalize();
        moveDirection.y = 0;

        // 速度を決定（走り/歩き）
        float currentSpeed = inputManager.moveAmount > 0.5f ? runningSpeed : walkingSpeed;
        Vector3 motion = moveDirection * currentSpeed * Time.deltaTime;

        // 重力処理
        if (characterController.isGrounded)
        {
            verticalVelocity = 0f;
        }
        else
        {
            verticalVelocity -= gravity * Time.deltaTime;
        }

        motion.y = verticalVelocity * Time.deltaTime;
        characterController.Move(motion);
    }

    private void HandleRotation()
    {
        if (inputManager == null) return;

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
}
