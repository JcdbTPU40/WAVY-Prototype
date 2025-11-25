using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class PlayerCombat : MonoBehaviour
{
	InputManager inputManager;
	AnimatorManager animatorManager;
	Animator animator;
	CharacterController characterController;
	const int TowerDamagePerHit = 1;

	[Header("Attack Settings")]
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
	[SerializeField] public GameObject beamPrefab;
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

	bool canAttack = true;
	bool canBeam = true;
	bool canCharge = true;
	bool isAttacking;
	bool isCharging;
	// 追加: 投擲制御
	bool canThrow = true;

	bool hasAttackBool;
	bool hasTailTrigger;
	bool hasBeamTrigger;
	bool hasChargeTrigger;
	readonly Dictionary<EnemyTowerHealth, float> towerHitTimestamps = new Dictionary<EnemyTowerHealth, float>();
	readonly HashSet<EnemyScript> chargeHitEnemies = new HashSet<EnemyScript>();
	readonly HashSet<BossScript> chargeHitBosses = new HashSet<BossScript>();

	void Awake()
	{
		inputManager = GetComponent<InputManager>();
		animatorManager = GetComponent<AnimatorManager>();
		animator = GetComponent<Animator>();
		characterController = GetComponent<CharacterController>();

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

	public void HandleAllCombatInput()
	{
		if (inputManager == null)
		{
			return;
		}

		HandleAttackInput(); // 追加: 投擲入力をここで処理
		HandleTailInput();
		HandleBeamInput();
		HandleChargeInput();
	}

	void HandleAttackInput()
	{
		// attackInput は InputManager が一フレームフラグとしてセットする想定
		// ここでは通常の「攻撃（Attack）」入力は消費するのみとし、
		// 投擲はビーム入力で切り替えて発動するように変更しています。
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
		// beamInput を受けて、Inspector 設定に従い Beam または Throw を発動する
		if (inputManager.beamInput && !isAttacking && !isCharging)
		{
			if (beamOrThrowMode == BeamOrThrowMode.Beam)
			{
				if (canBeam)
				{
					PerformBeamAttack();
				}
			}
			else // Throw モード
			{
				if (canThrow)
				{
					PerformThrowAttack();
				}
			}
		}

		// フラグは消費しておく
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

		if (beamPrefab != null)
		{
			Vector3 spawnPosition = transform.position + transform.TransformDirection(beamOffset);
			Quaternion spawnRotation = transform.rotation * Quaternion.Euler(90f, 0f, 0f);
			GameObject beamInstance = Instantiate(beamPrefab, spawnPosition, spawnRotation);
			beamInstance.transform.localScale = new Vector3(10f, 100f, 10f);
			StartCoroutine(DestroyAfterDelay(beamInstance, beamDuration));
		}
		else
		{
			Debug.LogWarning("Beam prefab が設定されていません", this);
		}

		StartCoroutine(BeamDamageRoutine());

		StartCoroutine(BeamCooldownRoutine());
	}
	void PerformChargeAttack()
	{
		chargeHitEnemies.Clear();
		isCharging = true;
		canCharge = false;

		if (animator != null)
		{
			if (hasChargeTrigger)
			{
				animator.SetTrigger(chargeTriggerName);
			}
		}
		else if (animatorManager != null)
		{
			animatorManager.PlayTargetAnimation("Charge", true);
		}

		StartCoroutine(ChargeMoveRoutine());
		StartCoroutine(ChargeCooldownRoutine());
	}
	void PerformThrowAttack()
	{
		if (!canThrow || throwProjectilePrefab == null)
		{
			// 無効またはプレハブ未設定ならクールダウンだけ行う
			StartCoroutine(ThrowCooldownRoutine());
			return;
		}

		canThrow = false;

		// アニメーション（存在すれば Attack の bool/triggers を利用）
		TriggerAttackAnimation(false, true, "Attack");

		// 実際に弾を生成して打つ（即時）
		Vector3 spawnPos = transform.position + transform.TransformDirection(throwSpawnOffset);
		float angleRad = Mathf.Deg2Rad * Mathf.Clamp(throwAngleDeg, 5f, 85f);

		// 水平方向の単位ベクトル
		Vector3 forward = transform.forward;
		Vector3 horizontalDir = new Vector3(forward.x, 0f, forward.z).normalized;
		if (horizontalDir.sqrMagnitude < Mathf.Epsilon)
		{
			horizontalDir = Vector3.forward;
		}

		// 重力の正数値
		float g = Mathf.Abs(Physics.gravity.y);
		// 目標水平距離 = throwRange
		float d = Mathf.Max(0.001f, throwRange);

		// 初速度の大きさ（単純な角度指定から計算）
		// v = sqrt(d * g / sin(2*angle))
		float denom = Mathf.Sin(2f * angleRad);
		float speed = 0f;
		if (Mathf.Abs(denom) > 0.0001f)
		{
			float tmp = d * g / denom;
			if (tmp < 0f) tmp = 0f;
			speed = Mathf.Sqrt(tmp);
		}
		else
		{
			// フォールバック
			speed = 10f;
		}

		Vector3 initialVelocity = horizontalDir * (speed * Mathf.Cos(angleRad)) + Vector3.up * (speed * Mathf.Sin(angleRad));

		GameObject proj = Instantiate(throwProjectilePrefab, spawnPos, Quaternion.LookRotation(initialVelocity.normalized));
		if (proj != null)
		{
			Rigidbody rb = proj.GetComponent<Rigidbody>();
			if (rb != null)
			{
				rb.linearVelocity = initialVelocity;
			}
			else
			{
				// Rigidbody 無ければ forward を設定して放り出すふりをする
				proj.transform.forward = initialVelocity.normalized;
			}

			// ProjectileThrow コンポーネントへ着弾エリア情報を渡す（存在すれば）
			var pt = proj.GetComponent<ProjectileThrow>();
			if (pt != null)
			{
				pt.landingAreaPrefab = landingAreaPrefab;
				pt.landingAreaRadius = landingAreaRadius;
				pt.landingAreaDuration = landingAreaDuration;
				pt.landingAreaDamagePerTick = landingAreaDamagePerTick;
				pt.landingAreaTickInterval = landingAreaTickInterval;
			}

			if (throwProjectileLifetime > 0f)
			{
				Destroy(proj, throwProjectileLifetime);
			}
		}

		// 投擲クールダウン開始
		StartCoroutine(ThrowCooldownRoutine());
	}

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

	IEnumerator ChargeCooldownRoutine()
	{
		yield return new WaitForSeconds(chargeCooldown);
		canCharge = true;
	}

	IEnumerator ThrowCooldownRoutine()
	{
		yield return new WaitForSeconds(Mathf.Max(0f, throwCooldown));
		canThrow = true;
	}

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
		if (towerHitTimestamps.Count == 0)
		{
			return;
		}

		var staleEntries = new List<EnemyTowerHealth>();
		foreach (var entry in towerHitTimestamps)
		{
			if (entry.Key == null)
			{
				staleEntries.Add(entry.Key);
			}
		}

		for (int i = 0; i < staleEntries.Count; i++)
		{
			towerHitTimestamps.Remove(staleEntries[i]);
		}
	}

	void ApplyTailDamage()
	{
		int mask = enemyLayers.value == 0 ? Physics.DefaultRaycastLayers : enemyLayers.value;
		Vector3 origin = transform.position + Vector3.up * tailHeightOffset;
		Collider[] hits = Physics.OverlapSphere(origin, tailAttackRadius, mask, QueryTriggerInteraction.Ignore);
		if (hits == null || hits.Length == 0)
		{
			return;
		}

		float halfAngle = tailAttackAngle * 0.5f;
		HashSet<Transform> damagedTargets = new HashSet<Transform>();

		foreach (Collider hit in hits)
		{
			if (hit == null)
			{
				continue;
			}

			EnemyScript enemy = hit.GetComponentInParent<EnemyScript>();
			BossScript boss = enemy != null ? null : hit.GetComponentInParent<BossScript>();
			EnemyTowerHealth tower = (enemy != null || boss != null) ? null : hit.GetComponentInParent<EnemyTowerHealth>();
			Transform targetTransform = enemy != null ? enemy.transform : boss != null ? boss.transform : tower != null ? tower.transform : null;
			if (targetTransform == null || damagedTargets.Contains(targetTransform))
			{
				continue;
			}

			Vector3 toTarget = targetTransform.position - origin;
			toTarget.y = 0f;
			if (toTarget.sqrMagnitude < Mathf.Epsilon)
			{
				toTarget = transform.forward;
			}

			float angle = Vector3.Angle(transform.forward, toTarget);
			if (angle > halfAngle)
			{
				continue;
			}

			if (enemy != null)
			{
				enemy.ApplyDamage(Mathf.RoundToInt(tailAttackDamage));
				ApplyKnockback(enemy, toTarget, tailKnockbackDistance);
			}
			else if (boss != null)
			{
				// ボスにはダメージのみ与え、ノックバックは適用しない
				boss.take_Damage(Mathf.RoundToInt(tailAttackDamage));
			}
			else if (tower != null)
			{
				tower.TakeDamage(TowerDamagePerHit);
			}

			damagedTargets.Add(targetTransform);
		}
	}

	void ApplyBeamDamage()
	{
		int mask = enemyLayers.value == 0 ? Physics.DefaultRaycastLayers : enemyLayers.value;
		Vector3 start = transform.position + transform.TransformDirection(beamOffset);
		Vector3 end = start + transform.forward * Mathf.Max(0f, beamRange);
		Collider[] hits = Physics.OverlapCapsule(start, end, Mathf.Max(0.01f, beamRadius), mask, QueryTriggerInteraction.Ignore);
		if (hits == null || hits.Length == 0)
		{
			return;
		}

		HashSet<Transform> damagedTargets = new HashSet<Transform>();
		foreach (Collider hit in hits)
		{
			if (hit == null)
			{
				continue;
			}

			EnemyScript enemy = hit.GetComponentInParent<EnemyScript>();
			BossScript boss = enemy != null ? null : hit.GetComponentInParent<BossScript>();
			EnemyTowerHealth tower = (enemy != null || boss != null) ? null : hit.GetComponentInParent<EnemyTowerHealth>();
			Transform targetTransform = enemy != null ? enemy.transform : boss != null ? boss.transform : tower != null ? tower.transform : null;
			if (targetTransform == null || damagedTargets.Contains(targetTransform))
			{
				continue;
			}

			Vector3 toTarget = targetTransform.position - start;
			toTarget.y = 0f;
			if (toTarget.sqrMagnitude < Mathf.Epsilon)
			{
				toTarget = transform.forward;
			}

			if (enemy != null)
			{
				enemy.ApplyDamage(Mathf.RoundToInt(beamDamage));
				ApplyKnockback(enemy, toTarget, beamKnockbackDistance);
			}
			else if (boss != null)
			{
				// ボスにはダメージのみ与え、ノックバックは適用しない
				boss.take_Damage(Mathf.RoundToInt(beamDamage));
			}
			else if (tower != null)
			{
				tower.TakeDamage(TowerDamagePerHit);
			}

			damagedTargets.Add(targetTransform);
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

	IEnumerator DestroyAfterDelay(GameObject instance, float delay)
	{
		yield return new WaitForSeconds(delay);
		if (instance != null)
		{
			Destroy(instance);
		}
	}

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
		if (direction.sqrMagnitude < Mathf.Epsilon)
		{
			return Vector3.right;
		}

		Vector3 perpendicular = Vector3.Cross(direction, Vector3.up);
		if (perpendicular.sqrMagnitude < 0.0001f)
		{
			perpendicular = Vector3.Cross(direction, Vector3.right);
		}

		return perpendicular;
	}

	void DrawWireCircle(Vector3 center, Vector3 normal, float radius)
	{
		if (radius <= 0f)
		{
			return;
		}

		normal = normal.normalized;
		Vector3 tangent = GetAnyPerpendicular(normal).normalized;
		Vector3 bitangent = Vector3.Cross(normal, tangent).normalized;

		const int segmentCount = 32;
		float angleStep = 360f / segmentCount;
		Vector3 previousPoint = center + tangent * radius;

		for (int i = 1; i <= segmentCount; i++)
		{
			float rad = Mathf.Deg2Rad * angleStep * i;
			Vector3 localPoint = (Mathf.Cos(rad) * tangent + Mathf.Sin(rad) * bitangent) * radius;
			Vector3 nextPoint = center + localPoint;
			Gizmos.DrawLine(previousPoint, nextPoint);
			previousPoint = nextPoint;
		}
	}
}
