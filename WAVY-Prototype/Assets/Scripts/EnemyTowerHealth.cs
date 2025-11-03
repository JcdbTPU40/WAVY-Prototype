using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyTowerHealth : MonoBehaviour
{
    const int MaxHearts = 3;
    const int HealthPerHeart = 2;

    [SerializeField, Min(0)] public int maxHealth = MaxHearts * HealthPerHeart; // 初期最大体力
    private int currentHealth;

    [Header("プレイヤー攻撃検出")]
    [SerializeField] float detectionRadius = 2f;
    [SerializeField] int detectionDamage = 1;
    [SerializeField] float damageInterval = 1f;
    [SerializeField] LayerMask playerAttackLayers = ~0;
    [SerializeField] string playerAttackTag = "Player";

    [Header("ハートUI設定")]
    [SerializeField] Transform heartContainer;
    [SerializeField] List<Image> heartImages = new List<Image>();
    [SerializeField] Transform heartLookTarget;

    [Header("破壊時スポーン設定")]
    [SerializeField] GameObject deathSpawnPrefab;
    [SerializeField, Min(0)] int deathSpawnCount = 0;
    [SerializeField, Min(0f)] float deathSpawnRadius = 3f;
    [SerializeField] Vector3 deathSpawnOffset = Vector3.zero;
    [SerializeField] bool inheritSpawnRotation = true;
    [SerializeField] Transform deathSpawnParent;

    [Header("破壊時にボスが来て滞在する時間（秒）")]
    [SerializeField, Min(0f)] float bossInvestigateDuration = 20f;

    float lastDamageTime;
    readonly List<Image> runtimeHeartImages = new List<Image>();
    bool loggedMissingHearts;

    void Start()
    {
        // 初期体力とハートUIを設定
        maxHealth = Mathf.Clamp(maxHealth, 0, MaxHearts * HealthPerHeart);
        currentHealth = maxHealth;
        PrepareHeartImages();
        UpdateHeartDisplay();
    }

    void Update()
    {
        TryDetectPlayerAttack();
        UpdateHeartFacing();
    }

    void LateUpdate()
    {
        UpdateHeartFacing();
    }

    // 攻撃を受けたときに呼び出されるメソッド
    public void TakeDamage(int damage)
    {
        if (damage <= 0 || currentHealth <= 0)
        {
            return;
        }

        int previousHealth = currentHealth;
        currentHealth = Mathf.Clamp(currentHealth - Mathf.Abs(damage), 0, maxHealth);

        if (currentHealth == previousHealth)
        {
            return;
        }

        UpdateHeartDisplay();
        Debug.Log($"Enemy_towerの体力: {currentHealth}");

        // 体力が0以下になったら壊れる
        if (currentHealth <= 0)
        {
            DestroyTower();
        }
    }

    void TryDetectPlayerAttack()
    {
        if (currentHealth <= 0 || Time.time - lastDamageTime < damageInterval)
        {
            return;
        }

        Collider[] hits = Physics.OverlapSphere(transform.position, detectionRadius, playerAttackLayers, QueryTriggerInteraction.Collide);
        if (hits == null || hits.Length == 0)
        {
            return;
        }

        foreach (Collider hit in hits)
        {
            if (hit == null)
            {
                continue;
            }

            if (hit.transform != null && hit.transform.IsChildOf(transform))
            {
                continue;
            }

            if (!string.IsNullOrEmpty(playerAttackTag) && !hit.CompareTag(playerAttackTag))
            {
                continue;
            }

            lastDamageTime = Time.time;
            TakeDamage(detectionDamage);
            break;
        }
    }

    private void DestroyTower()
    {
        Vector3 towerPos = transform.position;

        SpawnEnemiesOnDeath();

        // ボスへ通知（位置と滞在時間）
        NotifyBossesOfDestruction(towerPos, bossInvestigateDuration);

        Debug.Log("Enemy_towerが破壊されました！");
        Destroy(gameObject); // オブジェクトを破壊
    }

    void SpawnEnemiesOnDeath()
    {
        if (deathSpawnPrefab == null || deathSpawnCount <= 0)
        {
            return;
        }

        Quaternion spawnRotation = inheritSpawnRotation ? transform.rotation : Quaternion.identity;
        Transform parent = deathSpawnParent != null ? deathSpawnParent : null;

        for (int i = 0; i < deathSpawnCount; i++)
        {
            Vector3 spawnPosition = transform.position + deathSpawnOffset;
            if (deathSpawnRadius > 0f)
            {
                // 明示的に UnityEngine.Random を使う（System.Random と衝突する環境を回避）
                Vector2 randomCircle = UnityEngine.Random.insideUnitCircle * deathSpawnRadius;
                spawnPosition += new Vector3(randomCircle.x, 0f, randomCircle.y);
            }

            if (parent != null)
            {
                Instantiate(deathSpawnPrefab, spawnPosition, spawnRotation, parent);
            }
            else
            {
                Instantiate(deathSpawnPrefab, spawnPosition, spawnRotation);
            }
        }
    }

    void NotifyBossesOfDestruction(Vector3 towerWorldPos, float stayDuration)
    {
        // シーン内のすべての BossScript に通知（複数ボス対応）
        BossScript[] bosses = FindObjectsOfType<BossScript>();
        if (bosses == null || bosses.Length == 0) return;

        foreach (var boss in bosses)
        {
            if (boss == null) continue;
            boss.OnTowerDestroyed(towerWorldPos, stayDuration);
        }
    }

    void PrepareHeartImages()
    {
        runtimeHeartImages.Clear();

        if (heartImages != null)
        {
            for (int i = 0; i < heartImages.Count; i++)
            {
                Image image = heartImages[i];
                if (image != null && !runtimeHeartImages.Contains(image))
                {
                    runtimeHeartImages.Add(image);
                }
            }
        }

        if (runtimeHeartImages.Count < MaxHearts && heartContainer != null)
        {
            for (int i = 0; i < heartContainer.childCount; i++)
            {
                Image childImage = heartContainer.GetChild(i).GetComponent<Image>();
                if (childImage != null && !runtimeHeartImages.Contains(childImage))
                {
                    runtimeHeartImages.Add(childImage);
                    if (runtimeHeartImages.Count >= MaxHearts)
                    {
                        break;
                    }
                }
            }
        }

        runtimeHeartImages.Sort((a, b) =>
        {
            if (a == null || b == null)
            {
                return 0;
            }
            return a.transform.GetSiblingIndex().CompareTo(b.transform.GetSiblingIndex());
        });

        if (runtimeHeartImages.Count > MaxHearts)
        {
            runtimeHeartImages.RemoveRange(MaxHearts, runtimeHeartImages.Count - MaxHearts);
        }

        ResolveHeartLookTarget();
    }

    void UpdateHeartDisplay()
    {
        if (runtimeHeartImages.Count == 0)
        {
            if (!loggedMissingHearts)
            {
                Debug.LogWarning("ハートUIが設定されていません", this);
                loggedMissingHearts = true;
            }
            return;
        }

        int remainingHealth = Mathf.Clamp(currentHealth, 0, MaxHearts * HealthPerHeart);

        for (int i = 0; i < runtimeHeartImages.Count; i++)
        {
            Image heart = runtimeHeartImages[i];
            if (heart == null)
            {
                continue;
            }

            float fill = 0f;
            if (remainingHealth >= HealthPerHeart)
            {
                fill = 1f;
                remainingHealth -= HealthPerHeart;
            }
            else if (remainingHealth == 1)
            {
                fill = 0.5f;
                remainingHealth = 0;
            }

            heart.fillAmount = fill;
            heart.gameObject.SetActive(fill > 0f);
        }
    }

    void UpdateHeartFacing()
    {
        if (heartContainer == null)
        {
            return;
        }

        if (heartLookTarget == null)
        {
            ResolveHeartLookTarget();
            if (heartLookTarget == null)
            {
                return;
            }
        }

        Vector3 targetPosition = heartLookTarget.position;
        Vector3 direction = targetPosition - heartContainer.position;
        direction.y = 0f;

        if (direction.sqrMagnitude < 0.0001f)
        {
            return;
        }

        heartContainer.rotation = Quaternion.LookRotation(direction);
    }

    void ResolveHeartLookTarget()
    {
        if (heartLookTarget != null)
        {
            return;
        }

        if (!Application.isPlaying)
        {
            return;
        }

        if (!string.IsNullOrEmpty(playerAttackTag))
        {
            GameObject player = GameObject.FindGameObjectWithTag(playerAttackTag);
            if (player != null)
            {
                heartLookTarget = player.transform;
            }
        }

        if (heartLookTarget == null && Camera.main != null)
        {
            heartLookTarget = Camera.main.transform;
        }
    }

#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
#endif

#if UNITY_EDITOR
    void OnValidate()
    {
        maxHealth = Mathf.Clamp(maxHealth, 0, MaxHearts * HealthPerHeart);
        if (!Application.isPlaying)
        {
            currentHealth = maxHealth;
        }

        PrepareHeartImages();
        UpdateHeartDisplay();
    }
#endif
}
