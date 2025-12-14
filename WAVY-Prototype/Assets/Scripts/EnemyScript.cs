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

    [Header("追跡開始距離")]
    [SerializeField, Min(0f)] public float ChaseStartDistance = 15f; // この距離以内に入ったときだけプレイヤーを追跡開始

    [Header("攻撃間隔")]
    public float AttackInterval;                     // 攻撃間隔（未使用：将来実装用）
    [Header("攻撃時間")]
    public float AttackTimer = 0.5f;                 // 攻撃アニメーション時間などの想定（未使用）

    [Header("最大HP")]
    public int MaxHP;                                // 最大HP
    private int currentHP;                           // 現在HP
    private bool hasDied;                            // 死亡済みフラグ（多重処理防止）

    [Header("死亡時の死体を無くすまでの時間")]
    public float DeathTime = 2.0f;                   // Destroyまでの遅延
    [Header("敵を倒した時のスコア")]
    public int ScoreOnDeath = 100;                   // スコア加算量（ScoreScript未使用）

    [Header("攻撃設定")]
    PlayerHealth damage;
    [SerializeField] int attackDamage = 10;
    private float lastAttackTime;
    bool isAttacking=false;

    [Header("ヒット演出")]
    [SerializeField] private GameObject hitFxPrefab;
    [SerializeField] private Vector3 hitFxOffset = new Vector3(0f, 0.5f, 0f);
    [SerializeField] private float hitFxDelay = 0f;      // ヒットから再生までの遅延秒

    [Header("ラグドール設定")]
    [SerializeField] private SimpleRagdoll ragdollController;
    [SerializeField] private float deathKnockbackForce = 12f;
    [SerializeField] private float deathKnockbackUpForce = 2f;
    [SerializeField, Range(0f, 89f)] private float deathKnockbackAngle = 30f;

    [Header("死亡後ノックバック制御")]
    [SerializeField] private bool disableKnockbackAfterDeath = true; // 死んだら一切ノックバックしない

    [Header("被弾ノックバック")]
    [SerializeField] private float hitKnockbackForce = 4f;
    [SerializeField] private float hitKnockbackUpForce = 1.5f;
    [SerializeField, Range(0f, 89f)] private float hitKnockbackAngle = 20f;
    [SerializeField] private float hitKnockbackRecoveryDelay = 0.25f;

    [Header("地面判定設定")]
    [SerializeField] LayerMask groundMask = ~0;  // 地面用。必要に応じて調整
    [SerializeField] float liftBeforePhysics = 0.08f; // 8cmくらい持ち上げ
    [SerializeField] float groundRayLength = 2.0f;

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
    private bool isKnockbacking;

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
        // Knockback中や死亡後は、LookAt/経路更新/攻撃などのAI更新を止める
        if (isKnockbacking || hasDied)
        {
            if (animator != null)
            {
                animator.SetBool("IsWalking", false);
            }
            if (agent != null && agent.enabled && !agent.isStopped)
            {
                agent.ResetPath();
                agent.isStopped = true;
            }
            return;
        }

        if (Target != null)
        {
            float dis = Vector3.Distance(Target.transform.position, transform.position);
            
            // 追跡開始距離の判定：この距離以内に入ったときだけ追跡する
            if (dis <= ChaseStartDistance)
            {
                transform.LookAt(Target.transform);

                if (agent != null && agent.enabled && agent.isOnNavMesh)
                {
                    if (dis > AtDistance)
                    {
                        // 攻撃範囲外：追跡続行
                        if (agent.isStopped)
                        {
                            agent.isStopped = false;
                        }
                        agent.SetDestination(Target.transform.position);

                        if (animator != null)
                        {
                            Debug.Log("歩いてるよ");
                            animator.SetBool("IsWalking", true);
                        }
                    }
                    else
                    {
                        // 攻撃範囲内：停止して攻撃
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
                // 追跡範囲外：待機状態
                if (agent != null && agent.enabled && !agent.isStopped)
                {
                    agent.ResetPath();
                    agent.isStopped = true;
                }

                if (animator != null)
                {
                    animator.SetBool("IsWalking", false);
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
        if (hasDied) return; // ★死亡後は攻撃を無視（ヒットデータも更新しない）

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

        // 任意：死亡直後にレイヤーを切り替え、以降の攻撃ヒットを物理的に避ける
        if (disableKnockbackAfterDeath)
        {
            int corpse = LayerMask.NameToLayer("Corpse");
            if (corpse != -1)
            {
                SetLayerRecursively(gameObject, corpse);
            }
        }

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
        // ① 物理切替直前に少し持ち上げて“地面と重ならない”状態を作る
        LiftAboveGround(transform);

        // ② Rootの干渉源を切る
        if (agent != null) { agent.enabled = false; }

        var rootCol = GetComponent<Collider>();
        if (rootCol != null) rootCol.enabled = false;

        if (rb != null)
        {
            rb.isKinematic = true; // Rootは物理から外す
            rb.detectCollisions = false;
        }

        if (animator != null) animator.enabled = false;

        // ③ ノックバック方向の計算
        Vector3 hitPos = hasLastHitData ? lastHitPosition : transform.position + Vector3.up * 0.5f;
        Vector3 direction = hasLastHitData && lastHitDirection.sqrMagnitude > Mathf.Epsilon
            ? lastHitDirection.normalized
            : (-transform.forward);

        Vector3 horizontalDir = Vector3.ProjectOnPlane(direction, Vector3.up);
        if (horizontalDir.sqrMagnitude < Mathf.Epsilon) horizontalDir = Vector3.forward;
        horizontalDir.Normalize();

        Vector3 rotationAxis = Vector3.Cross(horizontalDir, Vector3.up);
        if (rotationAxis.sqrMagnitude < Mathf.Epsilon) rotationAxis = transform.right;
        rotationAxis.Normalize();

        Vector3 angledDir = Quaternion.AngleAxis(deathKnockbackAngle, rotationAxis) * horizontalDir;
        Vector3 finalForce = angledDir.normalized * deathKnockbackForce + Vector3.up * Mathf.Max(0.1f, deathKnockbackUpForce);

        // ★死亡後ノックバックを禁止する場合は力をゼロにする
        if (disableKnockbackAfterDeath)
            finalForce = Vector3.zero;

        // ④ ラグドールを安全に有効化
        if (ragdollController != null)
        {
            ragdollController.Die(hitPos, finalForce);
        }
        else
        {
            EnableRagdoll_StableFallback(finalForce);
        }

        enabled = false;
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

    // ragdollControllerが未設定の時用：子RBに設定＋インパルス
    void EnableRagdoll_StableFallback(Vector3 impulse)
    {
        // Rootは既に切っている前提
        var childRBs = GetComponentsInChildren<Rigidbody>();
        foreach (var crb in childRBs)
        {
            if (crb == rb) continue; // Rootは除外
            crb.isKinematic = false;
            crb.detectCollisions = true;
            crb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
            crb.interpolation = RigidbodyInterpolation.Interpolate;
        }

        // 代表ボーン（胸 or 骨盤）にだけインパルス
        Rigidbody drive = FindPreferBone(childRBs, "Spine", "Hips", "Chest");
        if (drive == null) drive = childRBs.Length > 0 ? childRBs[0] : null;
        if (drive != null)
        {
            drive.AddForce(impulse, ForceMode.VelocityChange);
        }
    }

    Rigidbody FindPreferBone(Rigidbody[] rbs, params string[] names)
    {
        foreach (var n in names)
        {
            foreach (var r in rbs)
            {
                if (r != null && r.transform.name.IndexOf(n, System.StringComparison.OrdinalIgnoreCase) >= 0)
                    return r;
            }
        }
        return null;
    }

    bool LiftAboveGround(Transform t)
    {
        if (t == null) return false;
        // 足元基準のレイ。必要なら足元のダミーTransformを用意してそこから撃つ
        Vector3 rayStart = t.position + Vector3.up * 0.2f;
        if (Physics.Raycast(rayStart, Vector3.down, out var hit, groundRayLength, groundMask, QueryTriggerInteraction.Ignore))
        {
            float targetY = hit.point.y + liftBeforePhysics;
            if (t.position.y < targetY)
            {
                var p = t.position; p.y = targetY; t.position = p;
                return true;
            }
        }
        return false;
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
        StartCoroutine(PerformAttack());
    }

    IEnumerator PerformAttack()
    {
        if(isAttacking) yield break;

        isAttacking = true;

        animator?.SetTrigger("Attack");
        damage = Target.GetComponent<PlayerHealth>();

        yield return new WaitForSeconds(AttackTimer);

        damage.TakeDamage(attackDamage);
        Target.SendMessage("TakeDamage", attackDamage, SendMessageOptions.DontRequireReceiver);

        isAttacking=false;
    }

    void ApplyHitKnockback()
    {
        if (hasDied) return; // ★死亡後は完全無効
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

        // 連続被弾でも1フレーム追従しないよう先にフラグを立てる
        isKnockbacking = true;
        CancelActiveKnockback();
        // Cancel内でfalseになっている可能性に備え明示再設定
        isKnockbacking = true;
        knockbackRoutine = StartCoroutine(HandleHitKnockback(force));
    }

    IEnumerator HandleHitKnockback(Vector3 force)
    {
        agentWasEnabledBeforeKnockback = agent != null && agent.enabled;
        if (agentWasEnabledBeforeKnockback)
        {
            agent.ResetPath();
            agent.isStopped = true;

            // ★これが重要：内部位置を今に合わせ、追従を止める
            agent.nextPosition = transform.position;
            agent.updatePosition = false;
            agent.updateRotation = false;
        }

        isKnockbacking = true;

        if (rb != null)
        {
            wasRbKinematicBeforeKnockback = rb.isKinematic;
            LiftAboveGround(transform);
            rb.isKinematic = false;
            rb.detectCollisions = true;
            rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
            rb.interpolation = RigidbodyInterpolation.Interpolate;

            #if UNITY_6000_0_OR_NEWER
            rb.linearVelocity = Vector3.zero;
            #else
            rb.velocity = Vector3.zero;
            #endif
            rb.angularVelocity = Vector3.zero;

            force += Vector3.up * 0.15f;
            rb.AddForce(force, ForceMode.VelocityChange);
        }

        float elapsed = 0f;
        bool interrupted = false;
        while (elapsed < hitKnockbackRecoveryDelay)
        {
            if (hasDied) { interrupted = true; break; }
            elapsed += Time.deltaTime;
            yield return null;
        }

        if (rb != null)
        {
            #if UNITY_6000_0_OR_NEWER
            rb.linearVelocity = Vector3.zero;
            #else
            rb.velocity = Vector3.zero;
            #endif
            rb.angularVelocity = Vector3.zero;
            rb.isKinematic = wasRbKinematicBeforeKnockback;
        }

        if (!interrupted && agentWasEnabledBeforeKnockback && agent != null)
        {
            // ★現在位置をAgentに同期してから再開
            agent.Warp(transform.position);
            agent.updatePosition = true;
            agent.updateRotation = true;
            agent.isStopped = false;
        }

        agentWasEnabledBeforeKnockback = false;
        isKnockbacking = false;
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
            #if UNITY_6000_0_OR_NEWER
            rb.linearVelocity = Vector3.zero;
            #else
            rb.velocity = Vector3.zero;
            #endif
            rb.angularVelocity = Vector3.zero;
            rb.isKinematic = wasRbKinematicBeforeKnockback;
        }

        if (resumeAgent && agentWasEnabledBeforeKnockback && agent != null)
        {
            // Knockback中に停止・非追従にしていたAgentを安全に再開
            agent.Warp(transform.position);
            agent.updatePosition = true;
            agent.updateRotation = true;
            agent.isStopped = false;
        }

        agentWasEnabledBeforeKnockback = false;
        isKnockbacking = false;
    }

    // 任意：指定GameObject配下の全てのレイヤーを変更
    void SetLayerRecursively(GameObject obj, int layer)
    {
        if (obj == null) return;
        obj.layer = layer;
        foreach (Transform c in obj.transform)
        {
            if (c != null)
            {
                SetLayerRecursively(c.gameObject, layer);
            }
        }
    }
}
