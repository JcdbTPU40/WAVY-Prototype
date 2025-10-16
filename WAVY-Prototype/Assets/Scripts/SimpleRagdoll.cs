using System.Collections.Generic;
using UnityEngine;

public class SimpleRagdoll : MonoBehaviour
{
    public Animator anim;
    List<Rigidbody> ragRBs = new();
    List<Collider> ragCols = new();

    void Awake()
    {
        foreach (var rb in GetComponentsInChildren<Rigidbody>())
        {
            if (rb.gameObject == gameObject) continue; // ルート除外
            ragRBs.Add(rb);
        }
        foreach (var col in GetComponentsInChildren<Collider>())
        {
            if (col.gameObject == gameObject) continue;
            ragCols.Add(col);
        }

        SetRagdoll(false);
    }

    void SetRagdoll(bool active)
    {
        if (anim) anim.enabled = !active;
        foreach (var rb in ragRBs) rb.isKinematic = !active;
        foreach (var col in ragCols) col.enabled = active;
    }

    public void Die(Vector3 hitPos, Vector3 force)
    {
        SetRagdoll(true);
        Rigidbody nearest = null;
        float best = float.MaxValue;
        foreach (var rb in ragRBs)
        {
            float dist = (rb.worldCenterOfMass - hitPos).sqrMagnitude;
            if (dist < best) { best = dist; nearest = rb; }
        }
        if (nearest) nearest.AddForceAtPosition(force, hitPos, ForceMode.Impulse);
        Destroy(gameObject, 10f);
    }
}
