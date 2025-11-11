using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class ProjectileThrow : MonoBehaviour
{
    [Header("Landing area settings (投擲弾の着弾エリア)")]
    public GameObject landingAreaPrefab = null;
    public float landingAreaRadius = 3f;
    public float landingAreaDuration = 5f;
    public int landingAreaDamagePerTick = 5;
    public float landingAreaTickInterval = 1f;

    // 当たり判定で着弾と見なす（Trigger/非Trigger 両対応）
    void OnCollisionEnter(Collision collision)
    {
        if (collision == null || collision.contactCount == 0) return;
        ContactPoint cp = collision.GetContact(0);
        HandleImpact(cp.point, cp.normal);
    }

    /*void OnTriggerEnter(Collider other)
    {
        // Trigger の場合は自分の中心で生成
        HandleImpact(transform.position);
    }*/

    void HandleImpact(Vector3 contactPoint, Vector3 surfaceNormal)
    {
        // 着弾エフェクト（着弾エリア）の生成
        if (landingAreaPrefab != null)
        {
            // 回転を法線に合わせる：上方向を surfaceNormal にする
            Vector3 normal = surfaceNormal.normalized;
            if (normal.sqrMagnitude < Mathf.Epsilon) normal = Vector3.up;

            // forward を法線の接線面上で決定（弾の進行方向を基にするか代替を選ぶ）
            Vector3 forward = Vector3.ProjectOnPlane(transform.forward, normal);
            if (forward.sqrMagnitude < 1e-4f)
            {
                // forward が小さい場合は法線と世界上方向から接線を作る
                forward = Vector3.Cross(normal, Vector3.up);
                if (forward.sqrMagnitude < 1e-4f)
                {
                    forward = Vector3.forward;
                }
            }
            forward.Normalize();

            Quaternion rot = Quaternion.LookRotation(forward, normal);

            Vector3 spawnPos = contactPoint - normal * 0.2f;

            GameObject area = Instantiate(landingAreaPrefab, spawnPos, rot);
            // 必要パラメータを付与できるなら付与（LandingArea スクリプトがあれば渡す）
            var la = area.GetComponent<LandingArea>();
            if (la != null)
            {
                la.Initialize(landingAreaRadius, landingAreaDuration, landingAreaDamagePerTick, landingAreaTickInterval);
            }
        }

        Destroy(gameObject);
    }
}