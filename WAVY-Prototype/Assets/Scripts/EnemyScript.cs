using System.Collections;
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

    [Header("攻撃設定")]
    [SerializeField] int attackDamage = 10;
    private float lastAttackTime;

    [Header("ヒット演出")]
    [SerializeField] private GameObject hitFxPrefab;
    [SerializeField] private Vector3 hitFxOffset = new Vector3(0f, 0.5f, 0f);
    [SerializeField] private float hitFxDelay = 0f;      // ヒットから再生までの遅延秒

    [Header("ラグドール設定")]
    [SerializeField] private SimpleRagdoll ragdollController;
    [SerializeField] private float deathKnockbackForce = 12f;
    [SerializeField] private float deathKnockbackUpForce = 2f;
    [SerializeField, Range(0f, 89f)] private float deathKnockbackAngle = 30f;

    [Header("被弾ノックバック")]
    [SerializeField] private float hitKnockbackForce = 4f;
    [SerializeField] private float hitKnockbackUpForce = 1.5f;
    [SerializeField, Range(0f, 89f)] private float hitKnockbackAngle = 20f;
    [SerializeField] private float hitKnockbackRecoveryDelay = 0.25f;

    private GameObject Target;                       // 追跡対象（Player）
    private NavMeshAgent agent;                      // NavMeshAgent参照
    private Animator animator;                       // アニメーター（現在未使用）
    private Rigidbody rb;                            // Rigidbody（物理挙動が必要なら利用）

    private Vector3 lastHitPosition = Vector3.zero;
    private Vector3 lastHitDirection = Vector3.zero;
    private bool hasLastHitData;
    private Coroutine knockbackRoutine;
    private bool wasRbKinematicBeforeKnockback;
    private bool agentWasEnabledBeforeKnockback;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        if (ragdollController == null)
        {
            ragdollController = GetComponent<SimpleRagdoll>();
        }

        if (agent != null)
        {
            agent.speed = EnemySpeed;                // エージェント速度を設定
        }
        currentHP = MaxHP;                           // 現在HP初期化
        Target = GameObject.FindGameObjectWithTag("Player"); // Playerタグから追跡対象取得
    }

    void Update()
    {
        if (Target != null)
        {
            float dis = Vector3.Distance(Target.transform.position, transform.position);
            transform.LookAt(Target.transform);

            if (agent != null && agent.enabled && agent.isOnNavMesh)
            {
                if (dis > AtDistance)
                {
                    if (agent.isStopped)
                    {
                        agent.isStopped = false;
                    }
                    agent.SetDestination(Target.transform.position);

                    if (animator != null)
                    {
                        animator.SetBool("IsWalking", true);
                    }
                }
                else
                {
                    if (!agent.isStopped)
                    {
                        agent.ResetPath();
                        agent.isStopped = true;
                    }

                    if (animator != null)
                    {
                        animator.SetBool("IsWalking", false);
                    }

                    TryAttackPlayer();
                }
            }
        }
        else
        {
            if (animator != null)
            {
                animator.SetBool("IsWalking", false);
            }
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
            Vector3 hitPoint = other.ClosestPoint(transform.position);
            Vector3 hitNormal = (transform.position - hitPoint).sqrMagnitude > Mathf.Epsilon
                ? (transform.position - hitPoint).normalized
                : transform.forward * -1f;

            ApplyDamage(damager.damage, hitPoint, hitNormal);
        }
    }

    /// <summary>
    /// 外部から敵にダメージを与える際に利用する。
    /// </summary>
    /// <param name="damage">減算するHP量。0以下の場合は無視。</param>
    public void ApplyDamage(int damage, Vector3? hitPoint = null, Vector3? hitNormal = null)
    {
        if (hasDied || damage <= 0)
        {
            return;
        }

        currentHP -= damage;
        CacheLastHitData(hitPoint, hitNormal);

        if (hitFxDelay <= 0f)
        {
            SpawnHitEffect(hitPoint, hitNormal);
        }
        else
        {
            StartCoroutine(SpawnHitEffectDelayed(hitPoint, hitNormal));
        }

        if (currentHP <= 0)
        {
            HandleDeath();
        }
        else
        {
            ApplyHitKnockback();
            //animator?.SetTrigger("GetHit");
        }
    }

    private void SpawnHitEffect(Vector3? hitPoint, Vector3? hitNormal)
    {
        if (hitFxPrefab == null)
        {
            return;
        }

        Vector3 spawnPosition = hitPoint ?? transform.position + hitFxOffset;
        Vector3 normal = hitNormal ?? (transform.forward * -1f);

        if (normal.sqrMagnitude < Mathf.Epsilon)
        {
            normal = Vector3.up;
        }

        Quaternion rotation = Quaternion.LookRotation(normal);
        Instantiate(hitFxPrefab, spawnPosition, rotation);
    }

    private IEnumerator SpawnHitEffectDelayed(Vector3? hitPoint, Vector3? hitNormal)
    {
        yield return new WaitForSeconds(hitFxDelay);
        SpawnHitEffect(hitPoint, hitNormal);
    }

    private void HandleDeath()
    {
        if (hasDied)
        {
            return;
        }

        CancelActiveKnockback(false);

        hasDied = true;
        currentHP = 0;

        if (agent != null)
        {
            if (agent.enabled)
            {
                agent.ResetPath();
                agent.isStopped = true;
            }
            agent.enabled = false;
        }

        TriggerRagdollDeath();

        if (ScoreOnDeath > 0 && ScoreManager.Instance != null)
        {
            ScoreManager.Instance.AddScore(ScoreOnDeath);
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

    void TriggerRagdollDeath()
    {
        if (ragdollController == null)
        {
            EnableRagdoll();
            return;
        }

        Vector3 hitPos = hasLastHitData ? lastHitPosition : transform.position + Vector3.up * 0.5f;
        Vector3 direction = hasLastHitData && lastHitDirection.sqrMagnitude > Mathf.Epsilon
            ? lastHitDirection.normalized
            : (-transform.forward);

        Vector3 horizontalDir = Vector3.ProjectOnPlane(direction, Vector3.up);
        if (horizontalDir.sqrMagnitude < Mathf.Epsilon)
        {
            horizontalDir = Vector3.ProjectOnPlane(-transform.forward, Vector3.up);
        }
        if (horizontalDir.sqrMagnitude < Mathf.Epsilon)
        {
            horizontalDir = Vector3.forward;
        }
        horizontalDir.Normalize();

        Vector3 rotationAxis = Vector3.Cross(horizontalDir, Vector3.up);
        if (rotationAxis.sqrMagnitude < Mathf.Epsilon)
        {
            rotationAxis = transform.right;
        }
        rotationAxis.Normalize();

        Vector3 angledDir = Quaternion.AngleAxis(deathKnockbackAngle, rotationAxis) * horizontalDir;
        Vector3 finalForce = angledDir.normalized * deathKnockbackForce;
        if (deathKnockbackUpForce != 0f)
        {
            finalForce += Vector3.up * deathKnockbackUpForce;
        }

        ragdollController.Die(hitPos, finalForce);
        enabled = false; // 以降のUpdate処理を停止
    }

    private void EnableRagdoll()
    {
        Rigidbody[] rigidbodies = GetComponentsInChildren<Rigidbody>();
        foreach (var childRb in rigidbodies)
        {
            childRb.isKinematic = false;
        }

        Collider[] colliders = GetComponentsInChildren<Collider>();
        foreach (var col in colliders)
        {
            col.enabled = true;
        }

        if (animator != null)
        {
            animator.enabled = false;
        }
    }

    void CacheLastHitData(Vector3? hitPoint, Vector3? hitNormal)
    {
        if (hitPoint.HasValue)
        {
            lastHitPosition = hitPoint.Value;
        }
        else
        {
            lastHitPosition = transform.position + hitFxOffset;
        }

        Vector3 chosenDirection = Vector3.zero;

        if (hitNormal.HasValue && hitNormal.Value.sqrMagnitude > Mathf.Epsilon)
        {
            chosenDirection = hitNormal.Value.normalized;
        }

        if (chosenDirection.sqrMagnitude < 0.25f 
            && hitPoint.HasValue)
        {
            Vector3 fromHitPoint = transform.position - hitPoint.Value;
            if (fromHitPoint.sqrMagnitude > Mathf.Epsilon)
            {
                chosenDirection = fromHitPoint.normalized;
            }
        }

        if (chosenDirection.sqrMagnitude < Mathf.Epsilon && Target != null)
        {
            Vector3 awayFromTarget = transform.position - Target.transform.position;
            if (awayFromTarget.sqrMagnitude > Mathf.Epsilon)
            {
                chosenDirection = awayFromTarget.normalized;
            }
        }

        if (chosenDirection.sqrMagnitude < Mathf.Epsilon)
        {
            chosenDirection = -transform.forward;
        }

        lastHitDirection = chosenDirection;
        hasLastHitData = true;
    }

    void TryAttackPlayer()
    {
        if (Target == null || !Target.CompareTag("Player"))
        {
            return;
        }

        if (Time.time - lastAttackTime < AttackInterval)
        {
            return;
        }

        lastAttackTime = Time.time;
        PerformAttack();
    }

    void PerformAttack()
    {
        animator?.SetTrigger("Attack");
        Target.SendMessage("TakeDamage", attackDamage, SendMessageOptions.DontRequireReceiver);
    }

    void ApplyHitKnockback()
    {
        if (!hasLastHitData || lastHitDirection.sqrMagnitude < Mathf.Epsilon)
        {
            return;
        }

        Vector3 horizontalDir = Vector3.ProjectOnPlane(lastHitDirection, Vector3.up);
        if (horizontalDir.sqrMagnitude < Mathf.Epsilon)
        {
            horizontalDir = -transform.forward;
        }
        horizontalDir.Normalize();

        Vector3 rotationAxis = Vector3.Cross(horizontalDir, Vector3.up);
        if (rotationAxis.sqrMagnitude < Mathf.Epsilon)
        {
            rotationAxis = transform.right;
        }
        rotationAxis.Normalize();

        Vector3 angledDir = Quaternion.AngleAxis(hitKnockbackAngle, rotationAxis) * horizontalDir;
        Vector3 force = angledDir.normalized * hitKnockbackForce;

        if (hitKnockbackUpForce != 0f)
        {
            force += Vector3.up * hitKnockbackUpForce;
        }

        CancelActiveKnockback();
        knockbackRoutine = StartCoroutine(HandleHitKnockback(force));
    }

    IEnumerator HandleHitKnockback(Vector3 force)
    {
        agentWasEnabledBeforeKnockback = agent != null && agent.enabled;
        if (agentWasEnabledBeforeKnockback)
        {
            agent.ResetPath();
            agent.isStopped = true;
            agent.enabled = false;
        }

        if (rb != null)
        {
            wasRbKinematicBeforeKnockback = rb.isKinematic;
            rb.isKinematic = false;
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.AddForce(force, ForceMode.VelocityChange);
        }
        else
        {
            transform.position += force * Time.deltaTime;
        }

        float elapsed = 0f;
        bool interrupted = false;
        while (elapsed < hitKnockbackRecoveryDelay)
        {
            if (hasDied)
            {
                interrupted = true;
                break;
            }
            elapsed += Time.deltaTime;
            yield return null;
        }

        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.isKinematic = wasRbKinematicBeforeKnockback;
        }

        if (!interrupted && agentWasEnabledBeforeKnockback && agent != null)
        {
            agent.enabled = true;
            agent.isStopped = false;
        }

        agentWasEnabledBeforeKnockback = false;
        knockbackRoutine = null;
    }

    void CancelActiveKnockback(bool resumeAgent = true)
    {
        if (knockbackRoutine == null)
        {
            return;
        }

        StopCoroutine(knockbackRoutine);
        knockbackRoutine = null;

        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.isKinematic = wasRbKinematicBeforeKnockback;
        }

        if (resumeAgent && agentWasEnabledBeforeKnockback && agent != null)
        {
            agent.enabled = true;
            agent.isStopped = false;
        }

        agentWasEnabledBeforeKnockback = false;
    }
}
