using UnityEngine;

public class WeaponHitbox : MonoBehaviour
{
    [SerializeField] private int damage = 3;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.TryGetComponent<IDamageable>(out var damageable))
        {
            return;
        }

        Vector3 point = other.ClosestPoint(transform.position);
        Vector3 normal = (other.transform.position - transform.position).normalized;

        if (normal.sqrMagnitude < Mathf.Epsilon)
        {
            normal = Vector3.up;
        }

        damageable.TakeDamage(damage, point, normal);
    }
}
