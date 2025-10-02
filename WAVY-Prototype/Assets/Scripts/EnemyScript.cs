using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// 敵キャラクターの基本挙動：
//// プレイヤーを追跡し、一定距離で停止。被弾するとHPが減少し、0で経験値プレハブを生成して消滅。
/// </summary>
public class EnemyScript : MonoBehaviour
{
    [Header("経験値プレハブと生成位置")]
    [SerializeField] GameObject exp_Prefab;          // 倒された際に生成する経験値オブジェクト
    [SerializeField] Transform[] exp_Spawnpoint;     // 経験値の生成位置（複数対応）
    
    [Header("移動速度")]
    public float EnemySpeed;                         // NavMeshAgent に設定する移動速度

    [Header("攻撃範囲")]
    public float AtDistance;                         // この距離以内に入ると追跡を止める（将来：攻撃に遷移予定）

    [Header("攻撃間隔")]
    public float AttackInterval;                     // 攻撃間隔（未使用：将来実装用）
    [Header("攻撃時間")]
    public float AttackTimer = 1.0f;                 // 攻撃アニメーション時間などの想定（未使用）

    [Header("最大HP")]
    public int MaxHP;                                // 最大HP
    private int currentHP;                           // 現在HP
    private bool hasDied;                            // 死亡済みフラグ（多重処理防止）

    [Header("死亡時の死体を無くすまでの時間")]
    public float DeathTime = 2.0f;                   // Destroyまでの遅延
    [Header("敵を倒した時のスコア")]
    public int ScoreOnDeath = 100;                   // スコア加算量（ScoreScript未使用）

    //private ScoreScript scoreScript;               // スコア管理（コメントアウト中）
    private GameObject Target;                       // 追跡対象（Player）
    private NavMeshAgent agent;                      // NavMeshAgent参照
    private Animator animator;                       // アニメーター（現在未使用）
    private Rigidbody rb;                            // Rigidbody（物理挙動が必要なら利用）

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();

        if (agent != null)
        {
            agent.speed = EnemySpeed;                // エージェント速度を設定
        }
        currentHP = MaxHP;                           // 現在HP初期化
        Target = GameObject.FindGameObjectWithTag("Player"); // Playerタグから追跡対象取得
        //scoreScript = FindFirstObjectByType<ScoreScript>(); // スコア管理取得（未使用）
    }

    void Update()
    {
        if (Target != null)
        {
            float dis = Vector3.Distance(Target.transform.position, transform.position); // プレイヤーとの距離
            transform.LookAt(Target.transform); // 常にプレイヤー方向を向く（首だけ回したい場合は別オブジェクト化推奨）

            if (agent != null && agent.enabled && agent.isOnNavMesh)
            {
                if (dis > AtDistance)
                {
                    // 攻撃距離外：追跡継続
                    if (agent.isStopped)
                    {
                        agent.isStopped = false; // 再開
                    }
                    agent.SetDestination(Target.transform.position); // 目標地点更新
                    //animator.SetBool("IsWalking", true);
                }
                else
                {
                    // 攻撃距離内：停止（攻撃未実装）
                    if (!agent.isStopped)
                    {
                        agent.ResetPath(); // 進行経路クリア
                        agent.isStopped = true;
                    }
                    //animator.SetBool("IsWalking", false);
                }
            }
        }
        else
        {
            // ターゲットが存在しない（例：プレイヤーが破壊された）
            //animator.SetBool("IsWalking", false);
        }
    }

    // 将来：攻撃アニメーションやダメージ判定をここで呼ぶ想定
    //void Attack()
    //{
    //    animator.SetTrigger("Attack");
    //}

    private void OnTriggerEnter(Collider other)
    {
        // 衝突相手にDamegeScript（綴り注意：Damageではない）が付いているか確認
        var damager = other.GetComponent<DamegeScript>();
        if (damager != null)
        {
            ApplyDamage(damager.damage);
        }
    }

    /* 敵破壊時にスコア加算したい場合は復活させる
    void OnDestroy()
    {
        if (scoreScript != null)
        {
            scoreScript.AddScore(ScoreOnDeath);
        }
    }
    */

    /// <summary>
    /// 外部から敵にダメージを与える際に利用する。
    /// </summary>
    /// <param name="damage">減算するHP量。0以下の場合は無視。</param>
    public void ApplyDamage(int damage)
    {
        if (hasDied || damage <= 0)
        {
            return;
        }

        currentHP -= damage;

        if (currentHP <= 0)
        {
            HandleDeath();
        }
        else
        {
            //animator?.SetTrigger("GetHit"); // 被弾アニメを再活性化したい場合に利用
        }
    }

    private void HandleDeath()
    {
        if (hasDied)
        {
            return;
        }

        hasDied = true;
        currentHP = 0;

        //animator?.SetTrigger("Die");

        if (agent != null && agent.enabled)
        {
            agent.ResetPath();
            agent.isStopped = true;
        }

        if (exp_Prefab != null && exp_Spawnpoint != null)
        {
            for (int i = 0; i < exp_Spawnpoint.Length; i++)
            {
                Transform point = exp_Spawnpoint[i];
                if (point == null)
                {
                    continue;
                }
                Instantiate(exp_Prefab, point.position, Quaternion.identity);
            }
        }

        Destroy(gameObject, DeathTime);
    }
}
