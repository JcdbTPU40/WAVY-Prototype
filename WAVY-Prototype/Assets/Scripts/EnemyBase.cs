using System.Collections;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// 敵キャラクターの共通挙動：
/// - プレイヤーを追跡し、一定距離で停止して攻撃
/// - 被弾でHP減少、0で死亡（ラグドール/スコア/経験値生成）
/// </summary>
public abstract class EnemyBase : MonoBehaviour
{
    [Header("経験値プレハブと生成位置")]
    [SerializeField] GameObject exp_Prefab;
    [SerializeField] Transform[] exp_Spawnpoint;

    [Header("移動速度")]
    public float EnemySpeed;

    [Header("攻撃範囲")]
    public float AtDistance;

    [Header("追跡開始距離")]
    [SerializeField, Min(0f)] public float ChaseStartDistance = 15f;

    [Header("攻撃間隔")]
    public float AttackInterval;
    [Header("攻撃時間")]
    public float AttackTimer = 0.5f;

    [Header("最大HP")]
    public int MaxHP;
    private int currentHP;
    protected bool hasDied;

    [Header("死亡時の死体を無くすまでの時間")]
    public float DeathTime = 2.0f;
    [Header("敵を倒した時のスコア")]
    public int ScoreOnDeath = 100;

    [Header("攻撃設定")]
    PlayerHealth damage;
    [SerializeField] int attackDamage = 10;
    private float lastAttackTime;
    bool isAttacking = false;

    [Header("ヒット演出")]
    [SerializeField] private GameObject hitFxPrefab;
    [SerializeField] private Vector3 hitFxOffset = new Vector3(0f, 0.5f, 0f);
    [SerializeField] private float hitFxDelay = 0f;

    [Header("ラグドール設定")]
    [SerializeField] private SimpleRagdoll ragdollController;
    [SerializeField] private float deathKnockbackForce = 12f;
    [SerializeField] private float deathKnockbackUpForce = 2f;
    [SerializeField, Range(0f, 89f)] private float deathKnockbackAngle = 30f;

    [Header("死亡後ノックバック制御")]
    [SerializeField] private bool disableKnockbackAfterDeath = true;

    [Header("被弾ノックバック")]
    [SerializeField] private float hitKnockbackForce = 4f;
    [SerializeField] private float hitKnockbackUpForce = 1.5f;
    [SerializeField, Range(0f, 89f)] private float hitKnockbackAngle = 20f;
    [SerializeField] private float hitKnockbackRecoveryDelay = 0.25f;

    [Header("尻尾攻撃ノックバック")]
    [SerializeField] private bool enableTailKnockback = true;
    [SerializeField, Min(0f)] private float tailKnockbackForceMultiplier = 2.0f;
    [SerializeField, Min(0f)] private float tailKnockbackUpForceMultiplier = 1.0f;

    [Header("地面判定設定")]
    [SerializeField] LayerMask groundMask = ~0;
    [SerializeField] float liftBeforePhysics = 0.08f;
    [SerializeField] float groundRayLength = 2.0f;

    protected GameObject Target;
    protected NavMeshAgent agent;
    protected Animator animator;
    protected Rigidbody rb;

    private Vector3 lastHitPosition = Vector3.zero;
    private Vector3 lastHitDirection = Vector3.zero;
    private bool hasLastHitData;
    private Coroutine knockbackRoutine;
    private bool wasRbKinematicBeforeKnockback;
    private bool agentWasEnabledBeforeKnockback;
    protected bool isKnockbacking;

    protected virtual void Start()
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
            agent.speed = EnemySpeed;
        }

        currentHP = MaxHP;
        Target = GameObject.FindGameObjectWithTag("Player");
    }

    protected virtual void Update()
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
                HandleOutOfChaseRange();
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

    /// <summary>
    /// 追跡範囲外の挙動（デフォルトは待機）。派生クラスで上書き可能。
    /// </summary>
    protected virtual void HandleOutOfChaseRange()
    {
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

    protected virtual void OnTriggerEnter(Collider other)
    {
        if (hasDied) return;

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
    public void ApplyDamage(int damageAmount, Vector3? hitPoint = null, Vector3? hitNormal = null)
    {
        if (hasDied || damageAmount <= 0)
        {
            return;
        }

        currentHP -= damageAmount;
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

    private void TriggerRagdollDeath()
    {
        // ① 物理切替直前に少し持ち上げて“地面と重ならない”状態を作る
        LiftAboveGround(transform);

        // ② Rootの干渉源を切る
        if (agent != null) { agent.enabled = false; }

        var rootCol = GetComponent<Collider>();
        if (rootCol != null) rootCol.enabled = false;

        if (rb != null)
        {
            rb.isKinematic = true;
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

    // ragdollControllerが未設定の時用：子RBに設定＋インパルス
    private void EnableRagdoll_StableFallback(Vector3 impulse)
    {
        // Rootは既に切っている前提
        var childRBs = GetComponentsInChildren<Rigidbody>();
        foreach (var crb in childRBs)
        {
            if (crb == rb) continue;
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

    private Rigidbody FindPreferBone(Rigidbody[] rbs, params string[] names)
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

    private bool LiftAboveGround(Transform t)
    {
        if (t == null) return false;
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

    private void CacheLastHitData(Vector3? hitPoint, Vector3? hitNormal)
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

    private void TryAttackPlayer()
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

    private IEnumerator PerformAttack()
    {
        if (isAttacking) yield break;

        isAttacking = true;

        animator?.SetTrigger("Attack");
        damage = Target.GetComponent<PlayerHealth>();

        yield return new WaitForSeconds(AttackTimer);

        if (damage != null)
        {
            damage.TakeDamage(attackDamage);
        }
        Target.SendMessage("TakeDamage", attackDamage, SendMessageOptions.DontRequireReceiver);

        isAttacking = false;
    }

    private void ApplyHitKnockback()
    {
        ApplyKnockback(hitKnockbackForce, hitKnockbackUpForce, hitKnockbackAngle, hitKnockbackRecoveryDelay);
    }

    /// <summary>
    /// 尻尾攻撃専用のノックバック。プレイヤーから離れる方向など、外部で方向を指定できる。
    /// </summary>
    public void ApplyTailKnockback(Vector3 directionAway, Vector3? hitPoint = null)
    {
        if (!enableTailKnockback) return;
        if (hasDied) return;

        Vector3 dir = directionAway;
        if (dir.sqrMagnitude < Mathf.Epsilon)
        {
            dir = -transform.forward;
        }

        lastHitDirection = dir.normalized;
        lastHitPosition = hitPoint ?? (transform.position + Vector3.up * 0.5f);
        hasLastHitData = true;

        float force = hitKnockbackForce * tailKnockbackForceMultiplier;
        float upForce = hitKnockbackUpForce * tailKnockbackUpForceMultiplier;
        ApplyKnockback(force, upForce, hitKnockbackAngle, hitKnockbackRecoveryDelay);
    }

    private void ApplyKnockback(float knockbackForce, float knockbackUpForce, float knockbackAngle, float recoveryDelay)
    {
        if (hasDied) return;
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

        Vector3 angledDir = Quaternion.AngleAxis(knockbackAngle, rotationAxis) * horizontalDir;
        Vector3 force = angledDir.normalized * knockbackForce;

        if (knockbackUpForce != 0f)
        {
            force += Vector3.up * knockbackUpForce;
        }

        isKnockbacking = true;
        CancelActiveKnockback();
        isKnockbacking = true;
        knockbackRoutine = StartCoroutine(HandleHitKnockback(force, recoveryDelay));
    }

    private IEnumerator HandleHitKnockback(Vector3 force, float recoveryDelay)
    {
        agentWasEnabledBeforeKnockback = agent != null && agent.enabled;
        if (agentWasEnabledBeforeKnockback)
        {
            agent.ResetPath();
            agent.isStopped = true;

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
        else
        {
            // Rigidbodyが無い敵でも、最低限「後ろにズレる」挙動を作る（NavMeshAgent優先）
            Vector3 planar = Vector3.ProjectOnPlane(force, Vector3.up);
            Vector3 displacement = planar.normalized * Mathf.Min(1.0f, planar.magnitude * 0.12f);
            if (displacement.sqrMagnitude > Mathf.Epsilon)
            {
                if (agent != null && agentWasEnabledBeforeKnockback && agent.isOnNavMesh)
                {
                    agent.Warp(transform.position + displacement);
                }
                else
                {
                    transform.position += displacement;
                }
            }
        }

        float elapsed = 0f;
        bool interrupted = false;
        while (elapsed < recoveryDelay)
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
            agent.Warp(transform.position);
            agent.updatePosition = true;
            agent.updateRotation = true;
            agent.isStopped = false;
        }

        agentWasEnabledBeforeKnockback = false;
        isKnockbacking = false;
        knockbackRoutine = null;
    }

    private void CancelActiveKnockback(bool resumeAgent = true)
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
            agent.Warp(transform.position);
            agent.updatePosition = true;
            agent.updateRotation = true;
            agent.isStopped = false;
        }

        agentWasEnabledBeforeKnockback = false;
        isKnockbacking = false;
    }

    private void SetLayerRecursively(GameObject obj, int layer)
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
