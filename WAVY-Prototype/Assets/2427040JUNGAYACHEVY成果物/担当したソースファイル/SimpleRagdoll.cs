using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SimpleRagdoll : MonoBehaviour
{
    [Header("Refs")]
    public Animator anim;
    public NavMeshAgent agent;          // 任意（あればセット）
    public Rigidbody rootRb;            // ルートに付けたRigidbody（無ければnullでOK）
    public Collider rootCol;            // ルートのCapsule等（無ければnullでOK）

    [Header("Physics Tweaks")]
    [SerializeField] LayerMask groundMask = ~0;
    [SerializeField] float liftBeforePhysics = 0.08f; // ラグドール化直前に持ち上げる高さ
    [SerializeField] float groundRayLen = 2.0f;

    [Header("Cleanup")]
    [SerializeField] float autoDestroySec = 10f;

    private Rigidbody rootRigidbody; // Root（本体）のRB参照

    List<Rigidbody> ragRBs = new();
    List<Collider> ragCols = new();

    void Reset() // エディタでAddしたとき用
    {
        anim = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        rootRb = GetComponent<Rigidbody>();
        rootCol = GetComponent<Collider>();
    }

    void Awake()
    {
        if (rootRigidbody == null)
            rootRigidbody = GetComponent<Rigidbody>();

        // 子RB/Colを収集（自分自身は除外）
        foreach (var rb in GetComponentsInChildren<Rigidbody>())
        {
            if (rb.gameObject == gameObject) continue;
            ragRBs.Add(rb);
        }
        foreach (var col in GetComponentsInChildren<Collider>())
        {
            if (col.gameObject == gameObject) continue;
            ragCols.Add(col);
        }

        SetRagdoll(false);
    }

    // on=true でラグドール化、falseで通常（アニメ側）
    public void SetRagdoll(bool on)
    {
        if (anim) anim.enabled = !on;
        if (agent) agent.enabled = !on;

        // Root側はラグドール中は物理から外す（干渉源を切る）
        if (rootRb)
        {
            if (!on)
            {
                // 通常状態：Rootを物理有効（ノックバック等で使用）
                rootRb.isKinematic = false;
                rootRb.detectCollisions = true;
                // Kinematicでない時のみ速度リセットを行う
                rootRb.linearVelocity = Vector3.zero;
                rootRb.angularVelocity = Vector3.zero;
            }
            else
            {
                // ラグドール中：Rootを物理から外す（干渉源を切る）。
                // すでにKinematicの場合、linearVelocityの設定はサポートされないため行わない。
                if (!rootRb.isKinematic)
                {
                    rootRb.linearVelocity = Vector3.zero;
                    rootRb.angularVelocity = Vector3.zero;
                }
                rootRb.isKinematic = true;           // ラグドール中はキネマティック
                rootRb.detectCollisions = false;
            }
        }
        if (rootCol) rootCol.enabled = !on;

        // 子ボーンは反転：通常は寝かせる、ラグドール時に起こす
        foreach (var rb in ragRBs)
        {
            rb.isKinematic = !on;
            rb.detectCollisions = on;

            // ラグドール中はトンネル防止＆見栄え改善
            rb.collisionDetectionMode = on ? CollisionDetectionMode.ContinuousDynamic
                                           : CollisionDetectionMode.Discrete;
            rb.interpolation = on ? RigidbodyInterpolation.Interpolate
                                  : RigidbodyInterpolation.None;

            if (on)
            {
                // 初期化（前フレームの速度を引きずらない）
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }
        }

        foreach (var col in ragCols) col.enabled = on;
    }

    // 足元の地面にスナップして ちょい上げ（地面めり込み防止）
    void LiftAboveGround()
    {
        Vector3 rayStart = transform.position + Vector3.up * 0.2f;
        if (Physics.Raycast(rayStart, Vector3.down, out var hit, groundRayLen, groundMask, QueryTriggerInteraction.Ignore))
        {
            float targetY = hit.point.y + liftBeforePhysics;
            if (transform.position.y < targetY)
            {
                var p = transform.position; p.y = targetY; transform.position = p;
            }
        }
    }

    // 死亡時に呼ぶ：ラグドール化＋インパルス付与
    public void Die(Vector3 hitPos, Vector3 impulse)
    {
        SetRagdoll(true);

        // インパルスは子RB（胸・骨盤など）にだけ適用。Root には適用しない
        var childRBs = GetComponentsInChildren<Rigidbody>(true);
        Rigidbody drive = null;
        foreach (var r in childRBs)
        {
            if (r != null && r != rootRigidbody && (drive == null))
            {
                // 任意で優先ボーンを選ぶ実装でもOK
                drive = r;
            }
        }
        if (drive != null && !drive.isKinematic)
        {
            drive.AddForce(impulse, ForceMode.VelocityChange);
        }

        if (autoDestroySec > 0f) Destroy(gameObject, autoDestroySec);
    }

    Rigidbody FindNearestRigidbody(Vector3 pos)
    {
        Rigidbody nearest = null;
        float best = float.MaxValue;
        foreach (var rb in ragRBs)
        {
            float d = (rb.worldCenterOfMass - pos).sqrMagnitude;
            if (d < best) { best = d; nearest = rb; }
        }
        return nearest;
    }

    Rigidbody FindPreferBone(string[] names)
    {
        foreach (var n in names)
        {
            foreach (var rb in ragRBs)
            {
                if (rb && rb.name.IndexOf(n, System.StringComparison.OrdinalIgnoreCase) >= 0)
                    return rb;
            }
        }
        return null;
    }
}
