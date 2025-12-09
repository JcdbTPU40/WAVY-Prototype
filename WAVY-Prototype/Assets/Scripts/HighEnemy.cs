// ...existing code...
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 移動機能（NavMeshAgent）を取り除いた Enemy 実装。
/// 被弾、ノックバック、ラグドール、攻撃、スコア等は元実装に準拠しますが
/// 自走移動は行いません（プレイヤー追跡や経路探索は不要な用途向け）。
/// </summary>
public class HighEnemy : MonoBehaviour
{
    [Header("経験値プレハブと生成位置")]
    [SerializeField] GameObject exp_Prefab;          // 倒された際に生成する経験値オブジェクト
    [SerializeField] Transform[] exp_Spawnpoint;     // 経験値の生成位置（複数対応）

    [Header("攻撃範囲")]
    public float AtDistance = 2.0f;                   // 近接攻撃判定距離

    [Header("攻撃間隔")]
    public float AttackInterval = 1.5f;               // 攻撃間隔
    [Header("攻撃時間")]
    public float AttackTimer = 1.0f;

    [Header("最大HP")]
    public int MaxHP = 10;                            // 最大HP
    private int currentHP;                            // 現在HP
    private bool hasDied;                             // 死亡済みフラグ（多重処理防止）

    [Header("死亡時の死体を無くすまでの時間")]
    public float DeathTime = 2.0f;                    // Destroyまでの遅延
    [Header("敵を倒した時のスコア")]
    public int ScoreOnDeath = 100;                    // スコア加算量

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
    private Animator animator;                       // アニメーター（現在未使用）
    private Rigidbody rb;                            // Rigidbody（物理挙動が必要なら利用）

    private Vector3 lastHitPosition = Vector3.zero;
    private Vector3 lastHitDirection = Vector3.zero;
    private bool hasLastHitData;
    private Coroutine knockbackRoutine;
    private bool wasRbKinematicBeforeKnockback;
    private bool isKnockbacking;

    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        if (ragdollController == null)
        {
            ragdollController = GetComponent<SimpleRagdoll>();
        }

        currentHP = MaxHP;                           // 現在HP初期化
        Target = GameObject.FindGameObjectWithTag("Player"); // Playerタグから参照取得
    }

    void Update()
    {
        // Knockback中や死亡後は、アクションを止める
        if (isKnockbacking || hasDied)
        {
            if (animator != null)
            {
                animator.SetBool("IsWalking", false);
            }
            return;
        }

        // 移動仕様を持たないので、ターゲットの方向を向く・範囲で攻撃判定のみ行う
        if (Target != null)
        {
            transform.LookAt(Target.transform);

            float dis = Vector3.Distance(Target.transform.position, transform.position);
            if (dis <= AtDistance)
            {
                TryAttackPlayer();
            }
            else
            {
                if (animator != null)
                {
                    animator.SetBool("IsWalking", false);
                }
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (hasDied) return; // 死亡後は無視

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
        }
    }

    private void SpawnHitEffect(Vector3? hitPoint, Vector3? hitNormal)
    {
        if (hitFxPrefab == null) return;

        Vector3 spawnPosition = hitPoint ?? transform.position + hitFxOffset;
        Vector3 normal = hitNormal ?? (transform.forward * -1f);
        if (normal.sqrMagnitude < Mathf.Epsilon) normal = Vector3.up;

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
        if (hasDied) return;

        CancelActiveKnockback(false);

        hasDied = true;
        currentHP = 0;

        if (disableKnockbackAfterDeath)
        {
            int corpse = LayerMask.NameToLayer("Corpse");
            if (corpse != -1) SetLayerRecursively(gameObject, corpse);
        }

        // ラグドール化（NavMeshAgent削除版では agent 操作はしない）
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
                if (point == null) continue;
                Instantiate(exp_Prefab, point.position, Quaternion.identity);
            }
        }

        Destroy(gameObject, DeathTime);
    }

    void TriggerRagdollDeath()
    {
        // 少し持ち上げて安全に物理へ移行
        LiftAboveGround(transform);

        var rootCol = GetComponent<Collider>();
        if (rootCol != null) rootCol.enabled = false;

        if (rb != null)
        {
            rb.isKinematic = true;
            rb.detectCollisions = false;
        }

        if (animator != null) animator.enabled = false;

        // ノックバック計算
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

        if (disableKnockbackAfterDeath) finalForce = Vector3.zero;

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

    void EnableRagdoll_StableFallback(Vector3 impulse)
    {
        var childRBs = GetComponentsInChildren<Rigidbody>();
        foreach (var crb in childRBs)
        {
            if (crb == rb) continue;
            crb.isKinematic = false;
            crb.detectCollisions = true;
            crb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
            crb.interpolation = RigidbodyInterpolation.Interpolate;
        }

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
        if (hitPoint.HasValue) lastHitPosition = hitPoint.Value;
        else lastHitPosition = transform.position + hitFxOffset;

        Vector3 chosenDirection = Vector3.zero;

        if (hitNormal.HasValue && hitNormal.Value.sqrMagnitude > Mathf.Epsilon)
        {
            chosenDirection = hitNormal.Value.normalized;
        }

        if (chosenDirection.sqrMagnitude < 0.25f && hitPoint.HasValue)
        {
            Vector3 fromHitPoint = transform.position - hitPoint.Value;
            if (fromHitPoint.sqrMagnitude > Mathf.Epsilon) chosenDirection = fromHitPoint.normalized;
        }

        if (chosenDirection.sqrMagnitude < Mathf.Epsilon && Target != null)
        {
            Vector3 awayFromTarget = transform.position - Target.transform.position;
            if (awayFromTarget.sqrMagnitude > Mathf.Epsilon) chosenDirection = awayFromTarget.normalized;
        }

        if (chosenDirection.sqrMagnitude < Mathf.Epsilon) chosenDirection = -transform.forward;

        lastHitDirection = chosenDirection;
        hasLastHitData = true;
    }

    void TryAttackPlayer()
    {
        if (Target == null || !Target.CompareTag("Player")) return;
        if (Time.time - lastAttackTime < AttackInterval) return;

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
        if (hasDied) return;
        if (!hasLastHitData || lastHitDirection.sqrMagnitude < Mathf.Epsilon) return;

        Vector3 horizontalDir = Vector3.ProjectOnPlane(lastHitDirection, Vector3.up);
        if (horizontalDir.sqrMagnitude < Mathf.Epsilon) horizontalDir = -transform.forward;
        horizontalDir.Normalize();

        Vector3 rotationAxis = Vector3.Cross(horizontalDir, Vector3.up);
        if (rotationAxis.sqrMagnitude < Mathf.Epsilon) rotationAxis = transform.right;
        rotationAxis.Normalize();

        Vector3 angledDir = Quaternion.AngleAxis(hitKnockbackAngle, rotationAxis) * horizontalDir;
        Vector3 force = angledDir.normalized * hitKnockbackForce;
        if (hitKnockbackUpForce != 0f) force += Vector3.up * hitKnockbackUpForce;

        isKnockbacking = true;
        CancelActiveKnockback();
        isKnockbacking = true;
        knockbackRoutine = StartCoroutine(HandleHitKnockback(force));
    }

    IEnumerator HandleHitKnockback(Vector3 force)
    {
        wasRbKinematicBeforeKnockback = rb != null ? rb.isKinematic : true;

        if (rb != null)
        {
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

        isKnockbacking = false;
        knockbackRoutine = null;
    }

    void CancelActiveKnockback(bool resumeAgent = false)
    {
        if (knockbackRoutine == null) return;

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

        isKnockbacking = false;
    }

    void SetLayerRecursively(GameObject obj, int layer)
    {
        if (obj == null) return;
        obj.layer = layer;
        foreach (Transform c in obj.transform)
        {
            if (c != null) SetLayerRecursively(c.gameObject, layer);
        }
    }
}
// ...existing code...