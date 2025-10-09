using UnityEngine;

public class EnemyHealth : MonoBehaviour, IDamageable
{
    [SerializeField] private int hp = 10;
    [SerializeField] private GameObject hitFxPrefab;

    public void TakeDamage(int amount, Vector3 hitPoint, Vector3 hitNormal)
    {
        hp -= amount;

        if (hitFxPrefab != null)
        {
            Instantiate(hitFxPrefab, hitPoint, Quaternion.LookRotation(hitNormal));
        }

        if (hp <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Destroy(gameObject);
    }
}
