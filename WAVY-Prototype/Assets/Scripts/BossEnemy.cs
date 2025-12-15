using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// 敵キャラクターの基本挙動：
//// プレイヤーを追跡し、一定距離で停止。被弾するとHPが減少し、0で経験値プレハブを生成して消滅。
/// </summary>
public class BossEnemy : EnemyBase
{
    [Header("ボスに群がる挙動設定")]
    public float radius = 5.0f;
    public float jitter = 1f; // 到達点のランダムズレ
    public Transform targetBoss;

    [Header("targetBoss未設定時の徘徊")]
    [SerializeField, Min(0f)] private float wanderRadiusWhenNoBoss = 10f;
    private Vector3 homePosition;

    protected override void Start()
    {
        base.Start();
        homePosition = transform.position;
    }

    protected override void HandleOutOfChaseRange()
    {
        MoveToRandomAroundTarget();

        if (animator != null && agent != null && agent.enabled)
        {
            animator.SetBool("IsWalking", agent.hasPath && agent.remainingDistance > 0.8f);
        }
    }

    private void MoveToRandomAroundTarget()
    {
        if (agent == null || !agent.enabled || !agent.isOnNavMesh)
        {
            return;
        }

        // 毎フレーム新規目的地を出すと挙動が不安定になりやすいので、到達したら次を決める
        if (agent.pathPending)
        {
            return;
        }
        if (agent.hasPath && agent.remainingDistance > 0.8f)
        {
            return;
        }

        Vector3 anchorPosition = targetBoss != null ? targetBoss.position : homePosition;
        float useRadius = targetBoss != null ? Mathf.Max(0f, radius) : wanderRadiusWhenNoBoss;
        float minRadius = targetBoss != null ? Mathf.Min(4.0f, useRadius) : 0f;

        Vector3 dir = Random.insideUnitCircle.normalized;
        Vector3 dest = anchorPosition + new Vector3(dir.x, 0, dir.y) * Random.Range(minRadius, useRadius);
        // small jitter
        dest += new Vector3(Random.Range(-jitter, jitter), 0, Random.Range(-jitter, jitter));
        NavMeshHit hit;
        if (NavMesh.SamplePosition(dest, out hit, 1.5f, NavMesh.AllAreas))
        {
            agent.SetDestination(hit.position);
        }
        else
        {
            agent.SetDestination(dest);
        }
    }
}
