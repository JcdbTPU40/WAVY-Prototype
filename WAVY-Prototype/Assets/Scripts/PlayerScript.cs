using Unity.Burst.Intrinsics;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

// Player の移動や攻撃などを制御するスクリプト
// - CharacterController を使って物理的に移動
// - Animator を制御してアニメーションを再生

public class PlayerScript : MonoBehaviour
{
    InputAction moveAction; // 移動入力 (WASD / スティック)
    InputAction punchingAction; // 攻撃入力 (パンチなど)

    [Header("移動速度")]
    public float speed = 2;
    [Header("旋回速度")]
    public float turnSpeed = 20f;
    [Header("カメラ参照")]
    public Transform cameraTransform;
    [Header("入力から変換した移動ベクトル")]
    Vector3 m_Movement;
    [Header("プレイヤーの回転情報")]
    Quaternion m_Rotation = Quaternion.identity;
    [Header("重力加速度")]
    public float gravity = 9.8f;
    [Header("垂直方向の速度")]
    private float verticalVelocity = 0f;
    Animator m_Animator;
    [Header("Animator パラメータ名")]
    [SerializeField] string speedParameterName = "Speed";
    [SerializeField] string movementTimeParameterName = "time";
    [SerializeField] string attackTriggerName = "Attack";
    [SerializeField] string attackTypeParameterName = "AttackType";
    bool hasSpeedFloat;
    bool hasTimeFloat;
    bool hasAttackTrigger;
    bool hasAttackTypeInt;
    [Header("移動時間")]

    float time = 0f;
    // CharacterControllerを利用して移動する
    CharacterController character;
    // 戦闘システム
    PlayerCombat playerCombat;   
    
    void Start()
    {
        // InputSystemのアクションマップからアクションを取得
        moveAction = InputSystem.actions.FindAction("Move");
        punchingAction = InputSystem.actions.FindAction("Attack");
        m_Animator = GetComponent<Animator>();

        character = GetComponent<CharacterController>();
        playerCombat = GetComponent<PlayerCombat>();

        if (m_Animator != null)
        {
            hasSpeedFloat = HasAnimatorParameter(m_Animator, speedParameterName, AnimatorControllerParameterType.Float);
            hasTimeFloat = HasAnimatorParameter(m_Animator, movementTimeParameterName, AnimatorControllerParameterType.Float);
            hasAttackTrigger = HasAnimatorParameter(m_Animator, attackTriggerName, AnimatorControllerParameterType.Trigger);
            hasAttackTypeInt = HasAnimatorParameter(m_Animator, attackTypeParameterName, AnimatorControllerParameterType.Int);

            if (!hasTimeFloat && !string.IsNullOrEmpty(movementTimeParameterName))
            {
                Debug.LogWarning($"Animatorに float パラメータ '{movementTimeParameterName}' が見つかりません。アニメーション側へ追加するか、スクリプトのパラメータ名を調整してください。", this);
            }
        }
        
        // カメラが設定されていない場合、メインカメラを自動で取得
        if (cameraTransform == null)
        {
            cameraTransform = Camera.main.transform;
        }
    }

    void Update()
    {
        //移動入力の取得
        Vector2 moveValue = moveAction.ReadValue<Vector2>();

        // カメラの向きに基づいて移動ベクトルを計算
        Vector3 cameraForward = Vector3.zero;
        Vector3 cameraRight = Vector3.zero;
        
        if (cameraTransform != null)
        {
            // カメラの前方向と右方向を取得（Y軸成分は除く）
            cameraForward = cameraTransform.forward;
            cameraForward.y = 0f;
            cameraForward.Normalize();
            
            cameraRight = cameraTransform.right;
            cameraRight.y = 0f;
            cameraRight.Normalize();
        }
        else
        {
            // カメラがない場合はワールド座標系を使用
            cameraForward = Vector3.forward;
            cameraRight = Vector3.right;
        }

        // 入力に基づいてカメラ相対の移動ベクトルを計算
        m_Movement = cameraRight * moveValue.x + cameraForward * moveValue.y;

        bool hasHorizontalInput = !Mathf.Approximately(moveValue.x, 0f);
        bool hasVerticalInput = !Mathf.Approximately(moveValue.y, 0f);
        bool isWalking = hasHorizontalInput || hasVerticalInput;

        // 移動速度をAnimatorのSpeedパラメータに設定
        float movementSpeed = m_Movement.magnitude;
        if (m_Animator != null && hasSpeedFloat)
        {
            m_Animator.SetFloat(speedParameterName, movementSpeed);
        }

        // 移動中ならtimeを増加そのあとアニメーターへ反映（既存のアニメーション用）
        if (isWalking)
        {
            time += Time.deltaTime;
        }
        else
        {
            time = 0;
        }
        if (m_Animator != null && hasTimeFloat)
        {
            m_Animator.SetFloat(movementTimeParameterName, time);
        }

        // 移動方向がある場合のみプレイヤーを回転
        if (m_Movement.magnitude > 0.1f)
        {
            Vector3 desiredForward = Vector3.RotateTowards(transform.forward, m_Movement, turnSpeed * Time.deltaTime, 0f);
            m_Rotation = Quaternion.LookRotation(desiredForward);
            transform.rotation = m_Rotation;
        }

        // CharacterControllerを使った移動処理
        Vector3 motion = m_Movement * speed * Time.deltaTime;
        
        // 重力処理
        if (character.isGrounded)
        {
            verticalVelocity = 0f; // 地面についているときは垂直速度をリセット
        }
        else
        {
            verticalVelocity -= gravity * Time.deltaTime; // 重力を適用
        }
        
        motion.y = verticalVelocity * Time.deltaTime;
        character.Move(motion);

        // 戦闘入力の処理
        if (playerCombat != null)
        {
            playerCombat.HandleAllCombatInput();
        }

        // 攻撃入力の処理
        // 攻撃中は歩けないようにする
        if (!isWalking)
        {
            if (punchingAction.WasPressedThisFrame())
            {
                if (m_Animator != null)
                {
                    if (hasAttackTrigger)
                    {
                        m_Animator.SetTrigger(attackTriggerName);
                    }
                    if (hasAttackTypeInt)
                    {
                        m_Animator.SetInteger(attackTypeParameterName, 0);
                    }
                }
            }
            //突進はここにいれてもいいかも
            //else if (crossPunchAction.WasPressedThisFrame())
            //{
            //m_Animator.SetTrigger("Attack");
            //m_Animator.SetInteger("AttackType", 1);

            //}
            //else if (RoundhouseKickAction.WasPressedThisFrame())
            //{
            //m_Animator.SetTrigger("Attack");
            //m_Animator.SetInteger("AttackType", 2);
            //}
        }
    }

    float pushPower = 2.0f;
    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        Rigidbody body = hit.collider.attachedRigidbody;

        // no rigidbody
        if (body == null || body.isKinematic)
        {
            return;
        }

        // We dont want to push objects below us
        if (hit.moveDirection.y < -0.3)
        {
            return;
        }

        Vector3 pushDir = new Vector3(hit.moveDirection.x, 0, hit.moveDirection.z);
        body.AddForce(pushDir * pushPower, ForceMode.VelocityChange);
    }

    //ダメージを受けたときの処理
    private void OnTriggerEnter(Collider other)
    {
        var damager = other.GetComponent<DamegeScript>();
        if (damager != null && damager.isAttacking)
        {
            //animator.SetTrigger("GetHit");
        }
    }

    bool HasAnimatorParameter(Animator animator, string parameterName, AnimatorControllerParameterType type)
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


