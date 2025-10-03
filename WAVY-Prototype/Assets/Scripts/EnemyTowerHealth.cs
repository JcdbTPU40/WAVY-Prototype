using UnityEngine;

public class EnemyTowerHealth : MonoBehaviour
{
    [SerializeField] public int maxHealth = 100; // 最大体力
    private int currentHealth;

    [Header("プレイヤー攻撃検出")]
    [SerializeField] float detectionRadius = 2f;
    [SerializeField] int detectionDamage = 10;
    [SerializeField] float damageInterval = 0.5f;
    [SerializeField] LayerMask playerAttackLayers = ~0;
    [SerializeField] string playerAttackTag = "Player";

    [Header("破壊時スポーン設定")]
    [SerializeField] GameObject deathSpawnPrefab;
    [SerializeField, Min(0)] int deathSpawnCount = 0;
    [SerializeField, Min(0f)] float deathSpawnRadius = 3f;
    [SerializeField] Vector3 deathSpawnOffset = Vector3.zero;
    [SerializeField] bool inheritSpawnRotation = true;
    [SerializeField] Transform deathSpawnParent;

    float lastDamageTime;

    void Start()
    {
        // 初期体力を設定
        currentHealth = maxHealth;
    }

    void Update()
    {
        TryDetectPlayerAttack();
    }

    // 攻撃を受けたときに呼び出されるメソッド
    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
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
        SpawnEnemiesOnDeath();
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
                Vector2 randomCircle = Random.insideUnitCircle * deathSpawnRadius;
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

#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
#endif
}
