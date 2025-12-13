using UnityEngine;
using System.Collections.Generic;

public class TailAttackHitBox : MonoBehaviour
{
    [SerializeField] TailScalebyHealth scale;
    [SerializeField] private int damage = 30;
    [SerializeField] private LayerMask enemyLayer;       // Enemy レイヤーを指定
    [SerializeField] private float hitRadius = 0.5f;     // 尻尾の太さとして判定に使う
    [SerializeField] private Transform tipPoint;         // 尻尾の先端（攻撃位置）
    [SerializeField] private float maxCheckDistance = 2f; // 1フレームで最大どれくらいの距離をチェックするか
    
    private Vector3 prevPos;
    public bool active = false;
    
    private HashSet<GameObject> hitEnemies = new HashSet<GameObject>();
    private HashSet<GameObject> hitBosses = new HashSet<GameObject>();

    private void Start()
    {
        prevPos = tipPoint != null ? tipPoint.position : transform.position; //尻尾の初期位置の確認
    }

    private void Update()
    {
        hitRadius=scale.size/8f; //尻尾の太さを更新

        if (!active || tipPoint == null) return; //攻撃状態じゃないと判定しない

        Vector3 currentPos = tipPoint.position; //現在の尻尾の位置
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

        RaycastHit[] hits = Physics.SphereCastAll(
            startPos, //開始位置
            hitRadius, //太さ
            dir.normalized, //方向
            distance, //距離
            enemyLayer, //レイヤー
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
            ProcessHit(col, point);
        }
    }

    private void ProcessHits(RaycastHit[] hits)
    {
        foreach (var hit in hits)
        {
            ProcessHit(hit.collider, hit.point, hit.normal);
        }
    }

    private void ProcessHit(Collider hitCollider, Vector3? hitPoint = null, Vector3? hitNormal = null)
    {
        if (hitCollider == null) return;

        // 敵のルートオブジェクトを取得
        var enemy = hitCollider.GetComponentInParent<EnemyScript>();
        if (enemy != null)
        {
            GameObject enemyRoot = enemy.gameObject;

            //同じ攻撃が重複しないようにする
            if (hitEnemies.Contains(enemyRoot))
            {
                return;
            }

            Vector3 actualHitPoint = hitPoint ?? hitCollider.ClosestPoint(tipPoint.position);
            Vector3 actualHitNormal = hitNormal ?? (enemy.transform.position - actualHitPoint).normalized;

            enemy.ApplyDamage(damage, actualHitPoint, actualHitNormal);

            hitEnemies.Add(enemyRoot);
            return;
        }

        // ボスのルートオブジェクトを取得
        var boss = hitCollider.GetComponentInParent<BossScript>();
        if (boss != null)
        {
            GameObject bossRoot = boss.gameObject;

            //同じ攻撃が重複しないようにする
            if (hitBosses.Contains(bossRoot))
            {
                return;
            }

            boss.take_Damage(damage);
            hitBosses.Add(bossRoot);
            return;
        }
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

    public void ResetPreviousPosition()
    {
        if (tipPoint != null)
            prevPos = tipPoint.position;
    }

    public void ClearHitEnemies()
    {
        hitEnemies.Clear();
        hitBosses.Clear();
    }
}