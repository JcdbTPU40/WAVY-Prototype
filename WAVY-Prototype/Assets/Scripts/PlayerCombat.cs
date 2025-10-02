using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
	InputManager inputManager;
	AnimatorManager animatorManager;
	Animator animator;
	CharacterController characterController;

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

	[Header("Beam Attack Settings")]
	public GameObject beamPrefab;
	public float beamDuration = 2f;
	public float beamCooldown = 3f;
	public Vector3 beamOffset = new Vector3(0f, 0f, 2f);
	public float beamDamage = 45f;
	public float beamRange = 12f;
	public float beamRadius = 1.2f;
	public float beamHitDelay = 0.1f;

	[Header("Charge Attack Settings")]
	public float chargeDistance = 5f;
	public float chargeDuration = 0.4f;
	public float chargeCooldown = 3f;
	public float chargeHitRadius = 1.5f;

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

	bool hasAttackBool;
	bool hasTailTrigger;
	bool hasBeamTrigger;
	bool hasChargeTrigger;

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

		HandleAttackInput();
		HandleTailInput();
		HandleBeamInput();
		HandleChargeInput();
	}

	void HandleAttackInput()
	{
		if (inputManager.attackInput && canAttack && !isAttacking && !isCharging)
		{
			PerformAttack();
		}

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
		if (inputManager.beamInput && canBeam && !isAttacking && !isCharging)
		{
			PerformBeamAttack();
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

	void PerformAttack()
	{
		isAttacking = true;
		canAttack = false;

		TriggerAttackAnimation(true, true, "Attack");

		StartCoroutine(AttackCooldownRoutine());
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
	}

	IEnumerator ChargeCooldownRoutine()
	{
		yield return new WaitForSeconds(chargeCooldown);
		canCharge = true;
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
		Collider[] colliders = Physics.OverlapSphere(transform.position, chargeHitRadius);
		foreach (Collider collider in colliders)
		{
			if (collider.CompareTag("Enemy"))
			{
				Destroy(collider.gameObject);
			}
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
		HashSet<EnemyScript> damagedEnemies = new HashSet<EnemyScript>();

		foreach (Collider hit in hits)
		{
			if (hit == null)
			{
				continue;
			}

			EnemyScript enemy = hit.GetComponentInParent<EnemyScript>();
			if (enemy == null || damagedEnemies.Contains(enemy))
			{
				continue;
			}

			Vector3 toEnemy = enemy.transform.position - origin;
			toEnemy.y = 0f;
			if (toEnemy.sqrMagnitude < Mathf.Epsilon)
			{
				toEnemy = transform.forward;
			}

			float angle = Vector3.Angle(transform.forward, toEnemy);
			if (angle > halfAngle)
			{
				continue;
			}

			enemy.ApplyDamage(Mathf.RoundToInt(tailAttackDamage));
			damagedEnemies.Add(enemy);
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

		HashSet<EnemyScript> damagedEnemies = new HashSet<EnemyScript>();
		foreach (Collider hit in hits)
		{
			if (hit == null)
			{
				continue;
			}

			EnemyScript enemy = hit.GetComponentInParent<EnemyScript>();
			if (enemy == null || damagedEnemies.Contains(enemy))
			{
				continue;
			}

			enemy.ApplyDamage(Mathf.RoundToInt(beamDamage));
			damagedEnemies.Add(enemy);
		}
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
		Gizmos.color = Color.red;
		Gizmos.DrawWireSphere(transform.position + transform.forward * (attackRange * 0.5f), attackRange);

		Gizmos.color = Color.cyan;
		Gizmos.DrawWireSphere(transform.position, chargeHitRadius);
	}
}
