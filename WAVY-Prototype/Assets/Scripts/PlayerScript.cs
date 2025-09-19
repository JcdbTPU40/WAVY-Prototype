using Unity.Burst.Intrinsics;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

// Player の移動や攻撃などを制御するスクリプト
// - NavMeshAgent を使って物理的に移動
// - Animator を制御してアニメーションを再生

public class PlayerScript : MonoBehaviour
{
    InputAction moveAction; // 移動入力 (WASD / スティック)
    InputAction punchingAction; // 攻撃入力 (パンチなど)

    [Header("移動速度")]
    public float speed = 2;
    [Header("旋回速度")]
    public float turnSpeed = 20f;
    [Header("入力から変換した移動ベクトル")]
    Vector3 m_Movement;
    [Header("プレイヤーの回転情報")]
    Quaternion m_Rotation = Quaternion.identity;
    Animator m_Animator;
    [Header("移動時間")]

    float time = 0f;
    //NavMeshAgentを利用して移動する]
    NavMeshAgent agent;

    //CharacterController character;  //   
    
    void Start()
    {
        // InputSystemのアクションマップからアクションを取得
        moveAction = InputSystem.actions.FindAction("Move");
        punchingAction = InputSystem.actions.FindAction("Attack");
        m_Animator = GetComponent<Animator>();

        //character = GetComponent<CharacterController>(); 
        // NavMeshAgent コンポーネントを取得
        agent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        //移動入力の取得
        Vector2 moveValue = moveAction.ReadValue<Vector2>();

        // 入力に基づいて移動ベクトルを計算
        m_Movement.Set(moveValue.x, 0f, moveValue.y);

        bool hasHorizontalInput = !Mathf.Approximately(moveValue.x, 0f);
        bool hasVerticalInput = !Mathf.Approximately(moveValue.y, 0f);
        bool isWalking = hasHorizontalInput || hasVerticalInput;
        //m_Animator.SetBool("IsWalking", isWalking);

        // 移動中ならtimeを増加そのあとアニメーターへ反映
        if (isWalking)
        {
            time += Time.deltaTime;
        }
        else
        {
            time = 0;
        }
        m_Animator.SetFloat("time", time);

        Vector3 desiredForward = Vector3.RotateTowards(transform.forward, m_Movement, turnSpeed * Time.deltaTime, 0f);
        m_Rotation = Quaternion.LookRotation(desiredForward);

        transform.rotation = m_Rotation;

        Vector3 forwardMovement = Vector3.forward * m_Movement.magnitude * speed * Time.deltaTime;
        // transform.Translate(forwardMovement);

        Vector3 motion;
        motion = transform.TransformDirection(forwardMovement);
        //character.Move(motion);
        agent.Move(motion);

        // 攻撃入力の処理
        // 攻撃中は歩けないようにする
        if (!isWalking)
        {
            if (punchingAction.WasPressedThisFrame())
            {
                m_Animator.SetTrigger("Attack");
                m_Animator.SetInteger("AttackType", 0);
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
}


