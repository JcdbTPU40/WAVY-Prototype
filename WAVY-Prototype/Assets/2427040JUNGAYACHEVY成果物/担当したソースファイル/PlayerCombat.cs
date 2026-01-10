using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Unity.VisualScripting;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class PlayerCombat : MonoBehaviour
{
	#region Constants
	const int TowerDamagePerHit = 1;
	const int GizmoCircleSegments = 32;
	const float MinimumRadius = 0.01f;
	const float MinimumRange = 0.001f;
	#endregion

	#region Components
	InputManager inputManager;
	AnimatorManager animatorManager;
	Animator animator;
	CharacterController characterController;
	#endregion

	#region Attack Settings
	// [Header("Basic Attack Settings")]
	// [SerializeField] float attackDamage = 20f;
	// [SerializeField] float attackCooldown = 1f;
	// [SerializeField] float attackAnimationDuration = 0.5f;

	[Header("SE")]
	[SerializeField] AudioSource seSource;
	[SerializeField] AudioClip chargeHitSE;
	[SerializeField] AudioClip tailStartSE;
	[SerializeField] AudioClip chargeStartSE;

	[Header("Tail Attack Settings")]
	[SerializeField] float tailAttackDamage = 35f;
	[SerializeField] float tailAttackRadius = 2f;
	[SerializeField, Range(10f, 180f)] float tailAttackAngle = 90f;
	[SerializeField] float tailAttackHitDelay = 0.2f;
	[SerializeField] float tailHeightOffset = 0.5f;
	[SerializeField] float tailKnockbackDistance = 3f;
	[SerializeField, Min(0f)] float tailAttackCooldown = 1f;
	[SerializeField, Min(0f)] float tailAttackAnimationDuration = 0.5f;

	// [Header("Beam Attack Settings")]
	// [SerializeField] public GameObject beamPrefab;
	// public float beamDuration = 2f;
	// public float beamCooldown = 3f;
	// public Vector3 beamOffset = new Vector3(0f, 0f, 2f);
	// public float beamDamage = 45f;
	// public float beamRange = 12f;
	// public float beamRadius = 1.2f;
	// public float beamHitDelay = 0.1f;
	// public float beamKnockbackDistance = 2.5f;

	// 投擲（Throw）関連（既存）
	// [Header("Throw (投擲) Attack Settings")]
	// [SerializeField] GameObject throwProjectilePrefab = null; // Rigidbody を持つ弾のプレハブ（ProjectileThrow コンポーネント推奨）
	// [SerializeField, Min(0f)] float throwRange = 8f;          // 水平射程（前方方向の距離）
	// [SerializeField, Range(5f, 85f)] float throwAngleDeg = 45f; // 発射角度（度）
	// [SerializeField] Vector3 throwSpawnOffset = new Vector3(0f, 3.0f, 3.0f); // ボス基準のスポーンオフセット
	// [SerializeField, Min(0f)] float throwCooldown = 2f;
	// [SerializeField, Min(0f)] float throwProjectileLifetime = 10f;
	// [SerializeField] GameObject landingAreaPrefab;
	// [SerializeField, Min(0f)] float landingAreaRadius = 3f;
	// [SerializeField, Min(0f)] float landingAreaDuration = 5f;
	// [SerializeField, Min(0f)] int landingAreaDamagePerTick = 5;
	// [SerializeField, Min(0f)] float landingAreaTickInterval = 1f;
	//
	// public enum BeamOrThrowMode { Beam = 0, Throw = 1 }
	// [Header("Beam / Throw Mode")]
	// [SerializeField] BeamOrThrowMode beamOrThrowMode = BeamOrThrowMode.Beam;

	[Header("Charge Attack Settings")]
	[SerializeField] float chargeDamage = 20f;
	[SerializeField] float chargeDistance = 5f;
	[SerializeField] float chargeDuration = 0.4f;
	[SerializeField] float chargeCooldown = 3f;
	[SerializeField] float chargeHitRadius = 1.5f;
	[SerializeField] float chargeKnockbackDistance = 2f;
	[SerializeField, Min(0f)] float chargeTowerHitInterval = 1f;

	[Header("Animator Parameters")]
	[SerializeField] string attackBoolName = "Attack";
	[SerializeField] string tailTriggerName = "Tail";
	// [SerializeField] string beamTriggerName = "Beam";
	[SerializeField] string chargeTriggerName = "Charge";

	[Header("Target Filtering")]
	[SerializeField] LayerMask enemyLayers = ~0;
	#endregion

	#region State Management
	bool canAttack = true;
	bool canCharge = true;
	bool isAttacking;
	bool isCharging;

	bool hasAttackBool;
	bool hasTailTrigger;
	bool hasChargeTrigger;

	readonly Dictionary<EnemyTowerHealth, float> towerHitTimestamps = new Dictionary<EnemyTowerHealth, float>();
	readonly HashSet<EnemyScript> chargeHitEnemies = new HashSet<EnemyScript>();
	readonly HashSet<BossEnemy> chargeHitBossEnemies = new HashSet<BossEnemy>();
	readonly HashSet<BossScript> chargeHitBosses = new HashSet<BossScript>();
	#endregion

	#region Unity Lifecycle
	void Awake()
	{
		InitializeComponents();
		InitializeAnimatorParameters();
	}
	#endregion

	#region Initialization
	void InitializeComponents()
	{
		inputManager = GetComponent<InputManager>();
		animatorManager = GetComponent<AnimatorManager>();
		animator = GetComponent<Animator>();
		characterController = GetComponent<CharacterController>();
	}

	void InitializeAnimatorParameters()
	{
		if (animator != null)
		{
			hasAttackBool = HasParameter(attackBoolName, AnimatorControllerParameterType.Bool);
			hasTailTrigger = HasParameter(tailTriggerName, AnimatorControllerParameterType.Trigger);
			hasChargeTrigger = HasParameter(chargeTriggerName, AnimatorControllerParameterType.Trigger);
		}
	}

	bool HasParameter(string paramName, AnimatorControllerParameterType type)
	{
		if (animator == null || string.IsNullOrEmpty(paramName))
		{
			return false;
		}

		foreach (var param in animator.parameters)
		{
			if (param.type == type && param.name == paramName)
			{
				return true;
			}
		}

		return false;
	}
	#endregion

	#region Input Handling
	public void HandleAllCombatInput()
	{
		if (inputManager == null) return;

		HandleAttackInput();
		HandleTailInput();
		// HandleBeamInput(); // 仕様からBeam/Throwが削除されたため
		HandleChargeInput();
	}

	void HandleAttackInput()
	{
		inputManager.attackInput = false;
	}

	void HandleTailInput()
	{
		if (inputManager.tailInput && canAttack && !isAttacking && !isCharging)
		{
			PerformTailAttackHitBox();
		}
		inputManager.tailInput = false;
	}

	// void HandleBeamInput()
	// {
	// 	// 仕様からBeam/Throwが削除されたため無効化
	// 	inputManager.beamInput = false;
	// }

	void HandleChargeInput()
	{
		if (inputManager.chargeInput && canCharge && !isAttacking && !isCharging)
		{
			PerformChargeAttack();
		}
		inputManager.chargeInput = false;
	}
	#endregion

	#region Attack Execution
	
	public TailAttackHitBox tailAttackHitBox;

	public void PerformTailAttackHitBox()
	{
		if (tailAttackHitBox == null)
		{
			Debug.LogWarning("TailAttackHitBox が設定されていません", this);
			inputManager.tailInput = false;
			return;
		}

		PlaySE(tailStartSE);

		tailAttackHitBox.SetDamage(Mathf.RoundToInt(tailAttackDamage));

		isAttacking = true;
		canAttack = false;

    	TriggerAttackAnimation(true, true, "Attack");

    	StartCoroutine(TailAttackProcess());

		StartCoroutine(AttackCooldownRoutine());
	}

	IEnumerator TailAttackProcess()
	{
		if (tailAttackHitBox == null) yield break;

		tailAttackHitBox.ResetPreviousPosition();
		tailAttackHitBox.ClearHitEnemies();

    	yield return new WaitForSeconds(tailAttackHitDelay);
    	tailAttackHitBox.active = true;
    	yield return new WaitForSeconds(0.1f);
    	tailAttackHitBox.active = false;
	}
	void TriggerAttackAnimation(bool useTailTrigger, bool useAttackBool, string fallbackAnimationName)
	{
		bool triggered = false;

		if (animator != null)
		{
			if (useTailTrigger && hasTailTrigger)
			{
				animator.SetTrigger(tailTriggerName);
				triggered = true;
			}
			if (useAttackBool && hasAttackBool)
			{
				animator.SetBool(attackBoolName, true);
				triggered = true;
			}
		}

		if (!triggered && animatorManager != null && !string.IsNullOrEmpty(fallbackAnimationName))
		{
			animatorManager.PlayTargetAnimation(fallbackAnimationName, true);
		}
	}

	// void PerformBeamAttack() { }
	// void TriggerBeamAnimation() { }
	// void SpawnBeamVisual() { }
	void PerformChargeAttack()
	{
		chargeHitEnemies.Clear();
		chargeHitBossEnemies.Clear();
		chargeHitBosses.Clear();
		isCharging = true;
		canCharge = false;

		PlaySE(chargeStartSE);

		TriggerChargeAnimation();
		StartCoroutine(ChargeMoveRoutine());
		StartCoroutine(ChargeCooldownRoutine());
	}

	void TriggerChargeAnimation()
	{
		if (animator != null && hasChargeTrigger)
		{
			animator.SetTrigger(chargeTriggerName);
		}
		else if (animatorManager != null)
		{
			animatorManager.PlayTargetAnimation("Charge", true);
		}
	}
	// void PerformThrowAttack() { }
	// Vector3 CalculateThrowVelocity() { return Vector3.zero; }
	// Vector3 GetHorizontalDirection(Vector3 forward) { return Vector3.forward; }
	// float CalculateProjectileSpeed(float angleRad, float distance, float gravity) { return 0f; }
	// void SpawnThrowProjectile(Vector3 spawnPos, Vector3 initialVelocity) { }
	// void ConfigureProjectileThrow(GameObject projectile) { }
	#endregion

	#region Coroutines
	IEnumerator AttackCooldownRoutine()
	{
		yield return new WaitForSeconds(tailAttackAnimationDuration);
		isAttacking = false;

		if (animator != null && hasAttackBool)
		{
			animator.SetBool(attackBoolName, false);
		}

		yield return new WaitForSeconds(Mathf.Max(0f, tailAttackCooldown - tailAttackAnimationDuration));
		canAttack = true;
	}

	IEnumerator ChargeCooldownRoutine()
	{
		yield return new WaitForSeconds(chargeCooldown);
		canCharge = true;
	}

	IEnumerator TailAttackRoutine()
	{
		// Tailの当たり判定は TailAttackHitBox へ移行
		yield break;
	}

	// IEnumerator BeamDamageRoutine() { yield break; }

	IEnumerator ChargeMoveRoutine()
	{
		Vector3 direction = transform.forward;
		float elapsed = 0f;
		float speed = chargeDistance / Mathf.Max(0.01f, chargeDuration);

		while (elapsed < chargeDuration)
		{
			float delta = Time.deltaTime;
			Vector3 movement = direction * speed * delta;
			MoveCharacter(movement);
			DetectAndRemoveEnemies();

			elapsed += delta;
			yield return null;
		}

		DetectAndRemoveEnemies();
		isCharging = false;
		chargeHitEnemies.Clear();
		chargeHitBossEnemies.Clear();
		chargeHitBosses.Clear();
	}

	IEnumerator DestroyAfterDelay(GameObject instance, float delay)
	{
		yield return new WaitForSeconds(delay);
		if (instance != null)
		{
			Destroy(instance);
		}
	}
	#endregion

	#region Movement & Damage Application
	void MoveCharacter(Vector3 displacement)
	{
		if (characterController != null)
		{
			Vector3 verticalVelocity = Vector3.zero;
			if (!characterController.isGrounded)
			{
				verticalVelocity.y = Physics.gravity.y * Time.deltaTime;
			}

			characterController.Move(displacement + verticalVelocity);
		}
		else
		{
			transform.position += displacement;
		}
	}

	void DetectAndRemoveEnemies()
	{
		CleanupTowerHitCache();
		int mask = GetValidLayerMask();
		Collider[] colliders = GetChargeHitColliders(mask);
		
		foreach (Collider collider in colliders)
		{
			if (collider == null) continue;
			if (IsSelfCollider(collider)) continue;

			if (TryHitEnemy(collider)) continue;
			if (TryHitBossEnemy(collider)) continue;
			if (TryHitBoss(collider)) continue;
			TryHitTower(collider);
		}
	}

	Collider[] GetChargeHitColliders(int mask)
	{
		float clampedRadius = Mathf.Max(0.01f, chargeHitRadius);
		if (characterController == null)
		{
			return Physics.OverlapSphere(transform.position, clampedRadius, mask, QueryTriggerInteraction.Collide);
		}

		Vector3 worldCenter = transform.TransformPoint(characterController.center);
		float radius = Mathf.Max(clampedRadius, characterController.radius);
		float height = Mathf.Max(characterController.height, radius * 2f);
		float half = Mathf.Max(0f, (height * 0.5f) - radius);
		Vector3 p1 = worldCenter + Vector3.up * half;
		Vector3 p2 = worldCenter - Vector3.up * half;

		return Physics.OverlapCapsule(p1, p2, radius, mask, QueryTriggerInteraction.Collide);
	}

	void PlaySE(AudioClip clip)
	{
    if (clip == null || seSource == null) return;
    seSource.PlayOneShot(clip);
	}

	bool IsSelfCollider(Collider collider)
	{
		if (collider == null) return true;
		return collider.transform == transform || collider.transform.IsChildOf(transform);
	}

	bool TryHitEnemy(Collider collider)
	{
		EnemyScript enemy = collider.GetComponentInParent<EnemyScript>();
		if (enemy == null || !chargeHitEnemies.Add(enemy)) return false;

		int damage = Mathf.RoundToInt(chargeDamage);
		Vector3 hitPoint = collider.ClosestPoint(transform.position);
		Vector3 hitNormal = (enemy.transform.position - hitPoint).sqrMagnitude > Mathf.Epsilon
			? (enemy.transform.position - hitPoint).normalized
			: transform.forward * -1f;
		enemy.ApplyDamage(damage, hitPoint, hitNormal);

		PlaySE(chargeHitSE);

		Vector3 fromPlayer = enemy.transform.position - transform.position;
		ApplyKnockback(enemy, fromPlayer, chargeKnockbackDistance);
		return true;
	}

	bool TryHitBossEnemy(Collider collider)
	{
		BossEnemy bossEnemy = collider.GetComponentInParent<BossEnemy>();
		if (bossEnemy == null || !chargeHitBossEnemies.Add(bossEnemy)) return false;

		int damage = Mathf.RoundToInt(chargeDamage);
		Vector3 hitPoint = collider.ClosestPoint(transform.position);
		Vector3 hitNormal = (bossEnemy.transform.position - hitPoint).sqrMagnitude > Mathf.Epsilon
			? (bossEnemy.transform.position - hitPoint).normalized
			: transform.forward * -1f;
		bossEnemy.ApplyDamage(damage, hitPoint, hitNormal);

		PlaySE(chargeHitSE);

		return true;
	}

	bool TryHitBoss(Collider collider)
	{
		BossScript boss = collider.GetComponentInParent<BossScript>();
		if (boss == null || !chargeHitBosses.Add(boss)) return false;

		boss.take_Damage(Mathf.RoundToInt(chargeDamage));

		PlaySE(chargeHitSE);

		return true;
	}

	bool TryHitTower(Collider collider)
	{
		EnemyTowerHealth tower = collider.GetComponentInParent<EnemyTowerHealth>();
		if (tower == null) return false;

		float interval = Mathf.Max(0f, chargeTowerHitInterval);
		if (interval > 0f && towerHitTimestamps.TryGetValue(tower, out float lastHit) && Time.time - lastHit < interval)
		{
			return false;
		}

		towerHitTimestamps[tower] = Time.time;
		tower.TakeDamage(TowerDamagePerHit);
		return true;
	}

	void CleanupTowerHitCache()
	{
		if (towerHitTimestamps.Count == 0) return;

		var staleEntries = new List<EnemyTowerHealth>();
		foreach (var entry in towerHitTimestamps)
		{
			if (entry.Key == null) staleEntries.Add(entry.Key);
		}

		foreach (var stale in staleEntries)
		{
			towerHitTimestamps.Remove(stale);
		}
	}

	void ApplyTailDamage()
	{
		// Tailの当たり判定は TailAttackHitBox へ移行
	}

	// void ApplyBeamDamage() { }

	int GetValidLayerMask()
	{
		return enemyLayers.value == 0 ? Physics.DefaultRaycastLayers : enemyLayers.value;
	}

	(EnemyScript enemy, BossScript boss, EnemyTowerHealth tower) GetTargetComponents(Collider hit)
	{
		EnemyScript enemy = hit.GetComponentInParent<EnemyScript>();
		BossScript boss = enemy != null ? null : hit.GetComponentInParent<BossScript>();
		EnemyTowerHealth tower = (enemy != null || boss != null) ? null : hit.GetComponentInParent<EnemyTowerHealth>();
		return (enemy, boss, tower);
	}

	Transform GetTargetTransform(EnemyScript enemy, BossScript boss, EnemyTowerHealth tower)
	{
		if (enemy != null) return enemy.transform;
		if (boss != null) return boss.transform;
		if (tower != null) return tower.transform;
		return null;
	}

	Vector3 GetDirectionToTarget(Vector3 origin, Vector3 targetPosition)
	{
		Vector3 toTarget = targetPosition - origin;
		toTarget.y = 0f;
		return toTarget.sqrMagnitude < Mathf.Epsilon ? transform.forward : toTarget;
	}

	void ApplyDamageToTarget(EnemyScript enemy, BossScript boss, EnemyTowerHealth tower, float damage, Vector3 direction, float knockbackDistance)
	{
		if (enemy != null)
		{
			enemy.ApplyDamage(Mathf.RoundToInt(damage));
			ApplyKnockback(enemy, direction, knockbackDistance);
		}
		else if (boss != null)
		{
			boss.take_Damage(Mathf.RoundToInt(damage));
		}
		else if (tower != null)
		{
			tower.TakeDamage(TowerDamagePerHit);
		}
	}

	void ApplyKnockback(EnemyScript enemy, Vector3 direction, float distance)
	{
		if (enemy == null || distance <= 0f) return;

		Vector3 knockDir = GetNormalizedKnockbackDirection(direction);
		float clampedDistance = Mathf.Max(0f, distance);

		NavMeshAgent enemyAgent = enemy.GetComponent<NavMeshAgent>();
		if (enemyAgent != null && enemyAgent.enabled)
		{
			enemyAgent.Move(knockDir * clampedDistance);
			return;
		}

		Rigidbody enemyRb = enemy.GetComponent<Rigidbody>();
		if (enemyRb != null && !enemyRb.isKinematic)
		{
			float force = clampedDistance / Mathf.Max(Time.fixedDeltaTime, 0.02f);
			enemyRb.AddForce(knockDir * force, ForceMode.VelocityChange);
			return;
		}

		enemy.transform.position += knockDir * clampedDistance;
	}

	Vector3 GetNormalizedKnockbackDirection(Vector3 direction)
	{
		Vector3 knockDir = new Vector3(direction.x, 0f, direction.z);
		
		if (knockDir.sqrMagnitude < Mathf.Epsilon)
		{
			knockDir = transform.forward;
		}
		
		return knockDir.normalized;
	}
	#endregion

	#region Gizmos
	void OnDrawGizmosSelected()
	{
		Vector3 origin = transform.position;
		Vector3 forward = transform.forward;

		// Tail attack fan
		Vector3 tailOrigin = origin + Vector3.up * tailHeightOffset;
		float clampedTailRadius = Mathf.Max(0f, tailAttackRadius);
		if (clampedTailRadius > 0f)
		{
			Gizmos.color = new Color(1f, 0.85f, 0f, 0.85f);
			Gizmos.DrawWireSphere(tailOrigin, clampedTailRadius);

			float clampedKnockbackDistance = Mathf.Max(0f, tailKnockbackDistance);
			if (clampedKnockbackDistance > 0f)
			{
				Gizmos.DrawLine(tailOrigin, tailOrigin + forward.normalized * clampedKnockbackDistance);
			}

#if UNITY_EDITOR
			if (tailAttackAngle > 0f)
			{
				float halfAngle = Mathf.Clamp(tailAttackAngle * 0.5f, 0f, 180f);
				Vector3 leftDir = Quaternion.AngleAxis(-halfAngle, Vector3.up) * forward;
				Vector3 rightDir = Quaternion.AngleAxis(halfAngle, Vector3.up) * forward;

				Handles.color = new Color(1f, 0.85f, 0f, 0.2f);
				Handles.DrawSolidArc(tailOrigin, Vector3.up, leftDir, tailAttackAngle, clampedTailRadius);

				Handles.color = new Color(1f, 0.75f, 0f, 1f);
				Handles.DrawAAPolyLine(3f, new Vector3[]
				{
					tailOrigin,
					tailOrigin + leftDir.normalized * clampedTailRadius
				});
				Handles.DrawAAPolyLine(3f, new Vector3[]
				{
					tailOrigin,
					tailOrigin + rightDir.normalized * clampedTailRadius
				});
			}
#endif
		}

		// Beam capsule
		// 仕様からBeam/Throwが削除されたため表示無効

		// Charge hit radius
		float clampedChargeRadius = Mathf.Max(0f, chargeHitRadius);
		if (clampedChargeRadius > 0f)
		{
			Gizmos.color = Color.cyan;
			Gizmos.DrawWireSphere(origin, clampedChargeRadius);
		}
	}

	void DrawWireCapsule(Vector3 start, Vector3 end, float radius)
	{
		if (radius <= 0f) return;

		Vector3 axis = end - start;
		if (axis.sqrMagnitude < Mathf.Epsilon)
		{
			Gizmos.DrawWireSphere(start, radius);
			return;
		}

		Vector3 direction = axis.normalized;
		Vector3 tangent = GetPerpendicularVector(direction) * radius;
		Vector3 bitangent = Vector3.Cross(direction, tangent.normalized) * radius;

		DrawWireCircle(start, direction, radius);
		DrawWireCircle(end, direction, radius);

		Gizmos.DrawLine(start + tangent, end + tangent);
		Gizmos.DrawLine(start - tangent, end - tangent);
		Gizmos.DrawLine(start + bitangent, end + bitangent);
		Gizmos.DrawLine(start - bitangent, end - bitangent);
	}

	Vector3 GetPerpendicularVector(Vector3 direction)
	{
		if (direction.sqrMagnitude < Mathf.Epsilon) return Vector3.right;

		Vector3 perpendicular = Vector3.Cross(direction, Vector3.up);
		if (perpendicular.sqrMagnitude < 0.0001f)
		{
			perpendicular = Vector3.Cross(direction, Vector3.right);
		}

		return perpendicular.normalized;
	}

	void DrawWireCircle(Vector3 center, Vector3 normal, float radius)
	{
		if (radius <= 0f) return;

		Vector3 tangent = GetPerpendicularVector(normal);
		Vector3 bitangent = Vector3.Cross(normal.normalized, tangent);

		float angleStep = 360f / GizmoCircleSegments;
		Vector3 previousPoint = center + tangent * radius;

		for (int i = 1; i <= GizmoCircleSegments; i++)
		{
			float radians = Mathf.Deg2Rad * angleStep * i;
			Vector3 localPoint = (Mathf.Cos(radians) * tangent + Mathf.Sin(radians) * bitangent) * radius;
			Vector3 nextPoint = center + localPoint;
			Gizmos.DrawLine(previousPoint, nextPoint);
			previousPoint = nextPoint;
		}
	}
	#endregion
}
