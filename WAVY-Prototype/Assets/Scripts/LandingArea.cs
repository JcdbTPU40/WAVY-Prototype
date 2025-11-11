using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LandingArea : MonoBehaviour
{
    float radius = 3f;
    float duration = 5f;
    int damagePerTick = 5;
    float tickInterval = 1f;

    public void Initialize(float radius, float duration, int damagePerTick, float tickInterval)
    {
        this.radius = Mathf.Max(0f, radius);
        this.duration = Mathf.Max(0f, duration);
        this.damagePerTick = Mathf.Max(0, damagePerTick);
        this.tickInterval = Mathf.Max(0.01f, tickInterval);

        StartCoroutine(RunArea());
    }

    IEnumerator RunArea()
    {
        float endTime = Time.time + duration;
        while (Time.time < endTime)
        {
            ApplyTick();
            yield return new WaitForSeconds(tickInterval);
        }
        Destroy(gameObject);
    }

    void ApplyTick()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, radius);
        if (hits == null || hits.Length == 0) return;

        HashSet<Transform> damaged = new HashSet<Transform>();
        foreach (var col in hits)
        {
            if (col == null) continue;

            // EnemyScript 優先
            var enemy = col.GetComponentInParent<EnemyScript>();
            if (enemy != null && !damaged.Contains(enemy.transform))
            {
                enemy.ApplyDamage(damagePerTick);
                damaged.Add(enemy.transform);
                continue;
            }

            // BossScript
            var boss = col.GetComponentInParent<BossScript>();
            if (boss != null && !damaged.Contains(boss.transform))
            {
                boss.take_Damage(damagePerTick);
                damaged.Add(boss.transform);
                continue;
            }

            // Tower
            var tower = col.GetComponentInParent<EnemyTowerHealth>();
            if (tower != null && !damaged.Contains(tower.transform))
            {
                // Tower にはダメージを投げる
                tower.TakeDamage(damagePerTick);
                damaged.Add(tower.transform);
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1f, 0.4f, 0.1f, 0.5f);
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}