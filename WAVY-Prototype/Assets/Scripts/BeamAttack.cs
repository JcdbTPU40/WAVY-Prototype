using UnityEngine;

public class BeamAttack : MonoBehaviour
{
	[Header("Beam Settings")]
	[SerializeField] float damage = 50f;

	void Start()
	{
		DetectEnemiesInBeam();
	}

	void DetectEnemiesInBeam()
	{
		Collider beamCollider = GetComponent<Collider>();
		if (beamCollider == null)
		{
			Debug.LogWarning("ビームにColliderが設定されていません", this);
			return;
		}

		Collider[] hitColliders = Physics.OverlapBox(
			transform.position,
			beamCollider.bounds.size / 2,
			transform.rotation
		);

		int enemyCount = 0;
		foreach (Collider collider in hitColliders)
		{
			if (collider.CompareTag("Enemy"))
			{
				enemyCount++;
				Destroy(collider.gameObject);
			}
		}

		if (enemyCount > 0)
		{
			Debug.Log($"ビーム攻撃: {enemyCount}体の敵を削除", this);
		}
	}

	void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag("Enemy"))
		{
			Destroy(other.gameObject);
		}
	}

	void OnDrawGizmos()
	{
		Gizmos.color = Color.cyan;
		Gizmos.matrix = transform.localToWorldMatrix;
		Gizmos.DrawWireCube(Vector3.zero, Vector3.one);
	}
}