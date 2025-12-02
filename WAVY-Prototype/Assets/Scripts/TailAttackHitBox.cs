using UnityEngine;
using System.Collections.Generic;

public class TailAttackHitBox : MonoBehaviour
{
    [SerializeField] private int damage = 10;
    [SerializeField] private LayerMask enemyLayer;       // Enemy レイヤーを指定
    [SerializeField] private float hitRadius = 0.5f;     // 尻尾の太さとして判定に使う
    [SerializeField] private Transform tipPoint;         // 尻尾の先端（攻撃位置）
    [SerializeField] private float maxCheckDistance = 2f; // 1フレームで最大どれくらいの距離をチェックするか
    
    private Vector3 prevPos;
    public bool active = false;
    
    private HashSet<GameObject> hitEnemies = new HashSet<GameObject>();

    private void Start()
    {
        prevPos = tipPoint != null ? tipPoint.position : transform.position;
    }

    private void Update()
    {
        if (!active || tipPoint == null) return;

        Vector3 currentPos = tipPoint.position;
        Vector3 dir = currentPos - prevPos;
        float distance = dir.magnitude;

        if (distance > maxCheckDistance)
        {
            int steps = Mathf.CeilToInt(distance / maxCheckDistance);
            for (int i = 0; i < steps; i++)
            {
                float t = (float)i / steps;
                float nextT = (float)(i + 1) / steps;
                Vector3 stepStart = Vector3.Lerp(prevPos, currentPos, t);
                Vector3 stepEnd = Vector3.Lerp(prevPos, currentPos, nextT);
                CheckHitsBetweenPoints(stepStart, stepEnd);
            }
        }
        else if (distance > 0.001f)
        {
            CheckHitsBetweenPoints(prevPos, currentPos);
        }
        else
        {
            CheckHitsAtPoint(currentPos);
        }

        prevPos = currentPos;
    }

    private void CheckHitsBetweenPoints(Vector3 startPos, Vector3 endPos)
    {
        Vector3 dir = endPos - startPos;
        float distance = dir.magnitude;
        
        if (distance < 0.001f)
        {
            CheckHitsAtPoint(startPos);
            return;
        }

        // SphereCastAllで判定（より確実）
        RaycastHit[] hits = Physics.SphereCastAll(
            startPos,
            hitRadius,
            dir.normalized,
            distance,
            enemyLayer,
            QueryTriggerInteraction.Collide
        );
        ProcessHits(hits);
    }

    private void CheckHitsAtPoint(Vector3 point)
    {
        Collider[] colliders = Physics.OverlapSphere(
            point,
            hitRadius,
            enemyLayer,
            QueryTriggerInteraction.Collide
        );

        foreach (var col in colliders)
        {
            ProcessSingleHit(col, point);
        }
    }

    private void ProcessHits(RaycastHit[] hits)
    {
        foreach (var hit in hits)
        {
            ProcessSingleHit(hit.collider, hit.point, hit.normal);
        }
    }

    private void ProcessSingleHit(Collider hitCollider, Vector3? hitPoint = null, Vector3? hitNormal = null)
    {
        if (hitCollider == null) return;

        // 敵のルートオブジェクトを取得
        var enemy = hitCollider.GetComponentInParent<EnemyScript>();
        if (enemy == null) return;

        GameObject enemyRoot = enemy.gameObject;

        if (hitEnemies.Contains(enemyRoot))
        {
            return;
        }

        Vector3 actualHitPoint = hitPoint ?? hitCollider.ClosestPoint(tipPoint.position);
        Vector3 actualHitNormal = hitNormal ?? (enemy.transform.position - actualHitPoint).normalized;

        // ダメージを与える
        enemy.ApplyDamage(damage, actualHitPoint, actualHitNormal);

        hitEnemies.Add(enemyRoot);
    }

    private void OnDrawGizmos()
    {
        if (tipPoint == null) return;

        Gizmos.color = active ? Color.red : Color.yellow;
        Gizmos.DrawWireSphere(tipPoint.position, hitRadius);
        
        // 前フレームとの軌跡を表示
        if (active && Application.isPlaying)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(prevPos, tipPoint.position);
        }
    }
}
