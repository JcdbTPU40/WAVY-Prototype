using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
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
	[Header("Basic Attack Settings")]
	public float attackDamage = 20f;
	public float attackRange = 2f;
	public float attackCooldown = 1f;

	[Header("Tail Attack Settings")]
	public float tailAttackDamage = 35f;
	public float tailAttackRadius = 2f;
	[Range(10f, 180f)] public float tailAttackAngle = 90f;
	public float tailAttackHitDelay = 0.2f;
	public float tailHeightOffset = 0.5f;
	public float tailKnockbackDistance = 3f;

	[Header("Beam Attack Settings")]
	public GameObject beamPrefab;
	public float beamDuration = 2f;
	public float beamCooldown = 3f;
	public Vector3 beamOffset = new Vector3(0f, 0f, 2f);
	public float beamDamage = 45f;
	public float beamRange = 12f;
	public float beamRadius = 1.2f;
	public float beamHitDelay = 0.1f;
	public float beamKnockbackDistance = 2.5f;

	// 投擲（Throw）関連（既存）
	[Header("Throw (投擲) Attack Settings")]
	[SerializeField] GameObject throwProjectilePrefab = null; // Rigidbody を持つ弾のプレハブ（ProjectileThrow コンポーネント推奨）
	[SerializeField, Min(0f)] float throwRange = 8f;          // 水平射程（前方方向の距離）
	[SerializeField, Range(5f, 85f)] float throwAngleDeg = 45f; // 発射角度（度）
	[SerializeField] Vector3 throwSpawnOffset = new Vector3(0f, 3.0f, 3.0f); // ボス基準のスポーンオフセット
	[SerializeField, Min(0f)] float throwCooldown = 2f;
	[SerializeField, Min(0f)] float throwProjectileLifetime = 10f;

	// ビーム入力でビームか投擲かを切り替える（Inspector で排他的に設定）
	public enum BeamOrThrowMode { Beam = 0, Throw = 1 }
	[Header("Beam / Throw 切替 (ビーム入力でどちらを発動するか)")]
	[SerializeField] BeamOrThrowMode beamOrThrowMode = BeamOrThrowMode.Beam;

	// 着弾エリア（ProjectileThrow から渡される値）
	[SerializeField] GameObject landingAreaPrefab = null;
	[SerializeField, Min(0f)] float landingAreaRadius = 3f;
	[SerializeField, Min(0f)] float landingAreaDuration = 5f;
	[SerializeField, Min(0f)] int landingAreaDamagePerTick = 5;
	[SerializeField, Min(0f)] float landingAreaTickInterval = 1f;

	[Header("Charge Attack Settings")]
	public float chargeDistance = 5f;
	public float chargeDuration = 0.4f;
	public float chargeCooldown = 3f;
	public float chargeHitRadius = 1.5f;
	public float chargeKnockbackDistance = 2f;
	[Min(0f)] public float chargeTowerHitInterval = 1f;

	[Header("Animation Timings")]
	public float attackAnimationDuration = 0.5f;

	[Header("Animator Parameters")]
	[SerializeField] string attackBoolName = "Attack";
	[SerializeField] string tailTriggerName = "Tail";
	[SerializeField] string beamTriggerName = "Beam";
	[SerializeField] string chargeTriggerName = "Charge";

	[Header("Target Filtering")]
	[SerializeField] LayerMask enemyLayers = ~0;
	#endregion

	#region State Management
	bool canAttack = true;
	bool canBeam = true;
	bool canCharge = true;
	bool canThrow = true;
	bool isAttacking;
	bool isCharging;

	bool hasAttackBool;
	bool hasTailTrigger;
	bool hasBeamTrigger;
	bool hasChargeTrigger;

	readonly Dictionary<EnemyTowerHealth, float> towerHitTimestamps = new Dictionary<EnemyTowerHealth, float>();
	readonly HashSet<EnemyScript> chargeHitEnemies = new HashSet<EnemyScript>();
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
			hasBeamTrigger = HasParameter(beamTriggerName, AnimatorControllerParameterType.Trigger);
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
		HandleBeamInput();
		HandleChargeInput();
	}

	void HandleAttackInput()
	{
		// 通常攻撃入力を消費（現在は投擲がビーム入力で切り替わるため）
		inputManager.attackInput = false;
	}

	void HandleTailInput()
	{
		if (inputManager.tailInput && canAttack && !isAttacking && !isCharging)
		{
			PerformTailAttack();
		}

		inputManager.tailInput = false;
	}

	void HandleBeamInput()
	{
		if (inputManager.beamInput && !isAttacking && !isCharging)
		{
			if (beamOrThrowMode == BeamOrThrowMode.Beam && canBeam)
			{
				PerformBeamAttack();
			}
			else if (beamOrThrowMode == BeamOrThrowMode.Throw && canThrow)
			{
				PerformThrowAttack();
			}
		}

		inputManager.beamInput = false;
	}

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
	void PerformTailAttack()
	{
		isAttacking = true;
		canAttack = false;

		TriggerAttackAnimation(true, true, "Attack");
		StartCoroutine(TailAttackRoutine());

		StartCoroutine(AttackCooldownRoutine());
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

	void PerformBeamAttack()
	{
		canBeam = false;

		TriggerBeamAnimation();
		SpawnBeamVisual();
		StartCoroutine(BeamDamageRoutine());
		StartCoroutine(BeamCooldownRoutine());
	}

	void TriggerBeamAnimation()
	{
		if (animator != null)
		{
			if (hasBeamTrigger)
			{
				animator.SetTrigger(beamTriggerName);
			}
			else if (hasChargeTrigger)
			{
				animator.SetTrigger(chargeTriggerName);
			}
		}
		else if (animatorManager != null)
		{
			animatorManager.PlayTargetAnimation("Beam", true);
		}
	}

	void SpawnBeamVisual()
	{
		if (beamPrefab == null)
		{
			Debug.LogWarning("Beam prefab が設定されていません", this);
			return;
		}

		Vector3 spawnPosition = transform.position + transform.TransformDirection(beamOffset);
		Quaternion spawnRotation = transform.rotation * Quaternion.Euler(90f, 0f, 0f);
		GameObject beamInstance = Instantiate(beamPrefab, spawnPosition, spawnRotation);
		beamInstance.transform.localScale = new Vector3(10f, 100f, 10f);
		StartCoroutine(DestroyAfterDelay(beamInstance, beamDuration));
	}
	void PerformChargeAttack()
	{
		chargeHitEnemies.Clear();
		chargeHitBosses.Clear();
		isCharging = true;
		canCharge = false;

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
	void PerformThrowAttack()
	{
		if (!canThrow || throwProjectilePrefab == null)
		{
			StartCoroutine(ThrowCooldownRoutine());
			return;
		}

		canThrow = false;
		TriggerAttackAnimation(false, true, "Attack");

		Vector3 spawnPos = transform.position + transform.TransformDirection(throwSpawnOffset);
		Vector3 initialVelocity = CalculateThrowVelocity();

		SpawnThrowProjectile(spawnPos, initialVelocity);
		StartCoroutine(ThrowCooldownRoutine());
	}

	Vector3 CalculateThrowVelocity()
	{
		float angleRad = Mathf.Deg2Rad * Mathf.Clamp(throwAngleDeg, 5f, 85f);
		Vector3 horizontalDir = GetHorizontalDirection(transform.forward);
		float g = Mathf.Abs(Physics.gravity.y);
		float d = Mathf.Max(MinimumRange, throwRange);

		float speed = CalculateProjectileSpeed(angleRad, d, g);
		return horizontalDir * (speed * Mathf.Cos(angleRad)) + Vector3.up * (speed * Mathf.Sin(angleRad));
	}

	Vector3 GetHorizontalDirection(Vector3 forward)
	{
		Vector3 horizontalDir = new Vector3(forward.x, 0f, forward.z).normalized;
		return horizontalDir.sqrMagnitude < Mathf.Epsilon ? Vector3.forward : horizontalDir;
	}

	float CalculateProjectileSpeed(float angleRad, float distance, float gravity)
	{
		float denom = Mathf.Sin(2f * angleRad);
		if (Mathf.Abs(denom) > 0.0001f)
		{
			float tmp = distance * gravity / denom;
			return Mathf.Sqrt(Mathf.Max(0f, tmp));
		}
		return 10f; // フォールバック速度
	}

	void SpawnThrowProjectile(Vector3 spawnPos, Vector3 initialVelocity)
	{
		GameObject proj = Instantiate(throwProjectilePrefab, spawnPos, Quaternion.LookRotation(initialVelocity.normalized));
		if (proj == null) return;

		ApplyProjectileVelocity(proj, initialVelocity);
		ConfigureProjectileThrow(proj);

		if (throwProjectileLifetime > 0f)
		{
			Destroy(proj, throwProjectileLifetime);
		}
	}

	void ApplyProjectileVelocity(GameObject projectile, Vector3 velocity)
	{
		Rigidbody rb = projectile.GetComponent<Rigidbody>();
		if (rb != null)
		{
			rb.linearVelocity = velocity;
		}
		else
		{
			projectile.transform.forward = velocity.normalized;
		}
	}

	void ConfigureProjectileThrow(GameObject projectile)
	{
		var pt = projectile.GetComponent<ProjectileThrow>();
		if (pt != null)
		{
			pt.landingAreaPrefab = landingAreaPrefab;
			pt.landingAreaRadius = landingAreaRadius;
			pt.landingAreaDuration = landingAreaDuration;
			pt.landingAreaDamagePerTick = landingAreaDamagePerTick;
			pt.landingAreaTickInterval = landingAreaTickInterval;
		}
	}
	#endregion

	#region Coroutines
	IEnumerator AttackCooldownRoutine()
	{
		yield return new WaitForSeconds(attackAnimationDuration);
		isAttacking = false;

		if (animator != null && hasAttackBool)
		{
			animator.SetBool(attackBoolName, false);
		}

		yield return new WaitForSeconds(Mathf.Max(0f, attackCooldown - attackAnimationDuration));
		canAttack = true;
	}

	IEnumerator BeamCooldownRoutine()
	{
		yield return new WaitForSeconds(beamCooldown);
		canBeam = true;
	}

	IEnumerator ThrowCooldownRoutine()
	{
		yield return new WaitForSeconds(Mathf.Max(0f, throwCooldown));
		canThrow = true;
	}

	IEnumerator ChargeCooldownRoutine()
	{
		yield return new WaitForSeconds(chargeCooldown);
		canCharge = true;
	}

	IEnumerator TailAttackRoutine()
	{
		float waitTime = Mathf.Max(0f, Mathf.Min(tailAttackHitDelay, attackAnimationDuration));
		if (waitTime > 0f)
		{
			yield return new WaitForSeconds(waitTime);
		}

		ApplyTailDamage();
	}

	IEnumerator BeamDamageRoutine()
	{
		float waitTime = Mathf.Max(0f, beamHitDelay);
		if (waitTime > 0f)
		{
			yield return new WaitForSeconds(waitTime);
		}

		ApplyBeamDamage();
	}

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
		Collider[] colliders = Physics.OverlapSphere(transform.position, chargeHitRadius);
		foreach (Collider collider in colliders)
		{
			if (collider == null)
			{
				continue;
			}

			EnemyScript enemy = collider.GetComponentInParent<EnemyScript>();
			if (enemy != null)
			{
				if (!chargeHitEnemies.Add(enemy))
				{
					continue;
				}
				enemy.ApplyDamage(Mathf.RoundToInt(attackDamage));
				Vector3 fromPlayer = enemy.transform.position - transform.position;
				ApplyKnockback(enemy, fromPlayer, chargeKnockbackDistance);
				continue;
			}

			BossScript boss = collider.GetComponentInParent<BossScript>();
			if (boss != null)
			{
				if (!chargeHitBosses.Add(boss))
				{
					continue;
				}
				// ボスにはダメージのみ（ノックバック無し）
				boss.take_Damage(Mathf.RoundToInt(attackDamage));
				continue;
			}

			EnemyTowerHealth tower = collider.GetComponentInParent<EnemyTowerHealth>();
			if (tower != null)
			{
				float lastHit;
				float interval = Mathf.Max(0f, chargeTowerHitInterval);
				if (interval > 0f && towerHitTimestamps.TryGetValue(tower, out lastHit) && Time.time - lastHit < interval)
				{
					continue;
				}

				towerHitTimestamps[tower] = Time.time;
				tower.TakeDamage(TowerDamagePerHit);
			}
		}
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
		int mask = enemyLayers.value == 0 ? Physics.DefaultRaycastLayers : enemyLayers.value;
		Vector3 origin = transform.position + Vector3.up * tailHeightOffset;
		Collider[] hits = Physics.OverlapSphere(origin, tailAttackRadius, mask, QueryTriggerInteraction.Ignore);
		
		if (hits == null || hits.Length == 0) return;

		float halfAngle = tailAttackAngle * 0.5f;
		HashSet<Transform> damagedTargets = new HashSet<Transform>();

		foreach (Collider hit in hits)
		{
			if (hit == null) continue;

			var (enemy, boss, tower) = GetTargetComponents(hit);
			Transform targetTransform = GetTargetTransform(enemy, boss, tower);
			
			if (targetTransform == null || damagedTargets.Contains(targetTransform)) continue;

			Vector3 toTarget = GetDirectionToTarget(origin, targetTransform.position);
			float angle = Vector3.Angle(transform.forward, toTarget);
			
			if (angle > halfAngle) continue;

			ApplyDamageToTarget(enemy, boss, tower, tailAttackDamage, toTarget, tailKnockbackDistance);
			damagedTargets.Add(targetTransform);
		}
	}

	void ApplyBeamDamage()
	{
		int mask = enemyLayers.value == 0 ? Physics.DefaultRaycastLayers : enemyLayers.value;
		Vector3 start = transform.position + transform.TransformDirection(beamOffset);
		Vector3 end = start + transform.forward * Mathf.Max(0f, beamRange);
		Collider[] hits = Physics.OverlapCapsule(start, end, Mathf.Max(MinimumRadius, beamRadius), mask, QueryTriggerInteraction.Ignore);
		
		if (hits == null || hits.Length == 0) return;

		HashSet<Transform> damagedTargets = new HashSet<Transform>();
		foreach (Collider hit in hits)
		{
			if (hit == null) continue;

			var (enemy, boss, tower) = GetTargetComponents(hit);
			Transform targetTransform = GetTargetTransform(enemy, boss, tower);
			
			if (targetTransform == null || damagedTargets.Contains(targetTransform)) continue;

			Vector3 toTarget = GetDirectionToTarget(start, targetTransform.position);
			ApplyDamageToTarget(enemy, boss, tower, beamDamage, toTarget, beamKnockbackDistance);
			damagedTargets.Add(targetTransform);
		}
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
		if (enemy == null)
		{
			return;
		}

		Vector3 knockDir = direction;
		knockDir.y = 0f;

		if (knockDir.sqrMagnitude < Mathf.Epsilon)
		{
			knockDir = transform.forward;
		}

		knockDir.Normalize();
		float clampedDistance = Mathf.Max(0f, distance);

		if (clampedDistance <= 0f)
		{
			return;
		}

		NavMeshAgent enemyAgent = enemy.GetComponent<NavMeshAgent>();
		if (enemyAgent != null && enemyAgent.enabled)
		{
			enemyAgent.Move(knockDir * clampedDistance);
			return;
		}

		Rigidbody enemyRb = enemy.GetComponent<Rigidbody>();
		if (enemyRb != null && !enemyRb.isKinematic)
		{
			enemyRb.AddForce(knockDir * clampedDistance / Mathf.Max(Time.fixedDeltaTime, 0.02f), ForceMode.VelocityChange);
			return;
		}

		enemy.transform.position += knockDir * clampedDistance;
	}
	#endregion

	#region Gizmos
	void OnDrawGizmosSelected()
	{
		Vector3 origin = transform.position;
		Vector3 forward = transform.forward;

		// Basic attack range
		float clampedAttackRange = Mathf.Max(0f, attackRange);
		if (clampedAttackRange > 0f)
		{
			Vector3 attackCenter = origin + forward * (clampedAttackRange * 0.5f);
			Gizmos.color = new Color(1f, 0.3f, 0.3f, 0.9f);
			Gizmos.DrawWireSphere(attackCenter, clampedAttackRange);
			Gizmos.DrawLine(origin, origin + forward * clampedAttackRange);
		}

		// Tail attack fan
		Vector3 tailOrigin = origin + Vector3.up * tailHeightOffset;
		float clampedTailRadius = Mathf.Max(0f, tailAttackRadius);
		if (clampedTailRadius > 0f)
		{
			Gizmos.color = new Color(1f, 0.85f, 0f, 0.85f);
			Gizmos.DrawWireSphere(tailOrigin, clampedTailRadius);

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
		Vector3 beamStart = origin + transform.TransformDirection(beamOffset);
		float clampedBeamRange = Mathf.Max(0f, beamRange);
		float clampedBeamRadius = Mathf.Max(0.01f, beamRadius);
		if (clampedBeamRange > 0f)
		{
			Vector3 beamEnd = beamStart + forward * clampedBeamRange;
			Gizmos.color = new Color(0.2f, 0.6f, 1f, 0.9f);
			DrawWireCapsule(beamStart, beamEnd, clampedBeamRadius);
		}

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
		Vector3 axis = end - start;
		if (axis.sqrMagnitude < Mathf.Epsilon)
		{
			Gizmos.DrawWireSphere(start, radius);
			return;
		}

		Vector3 direction = axis.normalized;
		Vector3 tangent = GetAnyPerpendicular(direction).normalized * radius;
		Vector3 bitangent = Vector3.Cross(direction, tangent).normalized * radius;

		DrawWireCircle(start, direction, radius);
		DrawWireCircle(end, direction, radius);

		Gizmos.DrawLine(start + tangent, end + tangent);
		Gizmos.DrawLine(start - tangent, end - tangent);
		Gizmos.DrawLine(start + bitangent, end + bitangent);
		Gizmos.DrawLine(start - bitangent, end - bitangent);
	}

	Vector3 GetAnyPerpendicular(Vector3 direction)
	{
		if (direction.sqrMagnitude < Mathf.Epsilon) return Vector3.right;

		Vector3 perpendicular = Vector3.Cross(direction, Vector3.up);
		if (perpendicular.sqrMagnitude < 0.0001f)
		{
			perpendicular = Vector3.Cross(direction, Vector3.right);
		}

		return perpendicular;
	}

	void DrawWireCircle(Vector3 center, Vector3 normal, float radius)
	{
		if (radius <= 0f) return;

		normal = normal.normalized;
		Vector3 tangent = GetAnyPerpendicular(normal).normalized;
		Vector3 bitangent = Vector3.Cross(normal, tangent).normalized;

		float angleStep = 360f / GizmoCircleSegments;
		Vector3 previousPoint = center + tangent * radius;

		for (int i = 1; i <= GizmoCircleSegments; i++)
		{
			float rad = Mathf.Deg2Rad * angleStep * i;
			Vector3 localPoint = (Mathf.Cos(rad) * tangent + Mathf.Sin(rad) * bitangent) * radius;
			Vector3 nextPoint = center + localPoint;
			Gizmos.DrawLine(previousPoint, nextPoint);
			previousPoint = nextPoint;
		}
	}
	#endregion
}
