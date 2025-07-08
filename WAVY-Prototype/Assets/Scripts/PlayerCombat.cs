using UnityEngine;
using System.Collections;

public class PlayerCombat : MonoBehaviour
{
    InputManager inputManager;
    AnimatorManager animatorManager;

    [Header("Attack Settings")]
    public float attackDamage = 20f;
    public float attackRange = 2f;
    public float attackCooldown = 1f;
    // public LayerMask enemyLayers = 1; // 敵のレイヤーマスク

    [Header("Attack Animation")]
    public float attackAnimationDuration = 0.5f;

    private bool canAttack = true;
    private bool isAttacking = false;

    private void Awake()
    {
        inputManager = GetComponent<InputManager>();
        animatorManager = GetComponent<AnimatorManager>();
    }

    public void HandleAllCombatInput()
    {
        HandleAttackInput();
    }

    private void HandleAttackInput()
    {
        Debug.Log($"攻撃入力チェック: attackInput={inputManager.attackInput}, canAttack={canAttack}, isAttacking={isAttacking}");
        
        if (inputManager.attackInput && canAttack && !isAttacking)
        {
            Debug.Log("攻撃入力を受け取りました。攻撃を実行します。");
            PerformAttack();
            
            // 攻撃入力をリセット
            inputManager.attackInput = false;
        }
        else if (inputManager.attackInput)
        {
            // 攻撃できない理由をログ出力
            if (!canAttack)
                Debug.Log("攻撃がクールダウン中です");
            if (isAttacking)
                Debug.Log("既に攻撃中です");
            
            // 攻撃入力をリセット
            inputManager.attackInput = false;
        }
    }

    private void PerformAttack()
    {
        isAttacking = true;
        canAttack = false;

        Debug.Log("攻撃を実行中...");

        // アニメーション再生（AnimatorManagerがある場合）
        if (animatorManager != null)
        {
            Debug.Log("攻撃アニメーションを再生");
            animatorManager.PlayTargetAnimation("Attack", true);
        }
        else
        {
            Debug.Log("AnimatorManagerが見つかりません");
        }

        // 敵の検知は後で実装
        // DetectEnemies();

        // クールダウン開始
        StartCoroutine(AttackCooldown());
    }

    private void DetectEnemies()
    {
        // プレイヤーの前方に攻撃判定を作成
        Vector3 attackPosition = transform.position + transform.forward * (attackRange / 2);
        
        // 球状の攻撃範囲で敵を検出
        Collider[] hitEnemies = Physics.OverlapSphere(attackPosition, attackRange);

        Debug.Log($"攻撃判定: {hitEnemies.Length}体の敵を検出");

        // 敵へのダメージ処理は後で実装
        /*
        foreach (Collider enemy in hitEnemies)
        {
            // 敵のHealthコンポーネントを取得してダメージを与える
            EnemyHealth enemyHealth = enemy.GetComponent<EnemyHealth>();
            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(attackDamage);
                Debug.Log($"敵 {enemy.name} に {attackDamage} ダメージを与えました！");
            }
        }
        */
    }

    private IEnumerator AttackCooldown()
    {
        yield return new WaitForSeconds(attackAnimationDuration);
        isAttacking = false;
        
        // Attackパラメータをfalseにリセット
        if (animatorManager != null)
        {
            animatorManager.PlayTargetAnimation("Idle", false); // または別の方法でfalseに設定
        }
        
        Debug.Log("攻撃アニメーション終了");
        
        yield return new WaitForSeconds(attackCooldown - attackAnimationDuration);
        canAttack = true;
        Debug.Log("攻撃クールダウン終了");
    }

    // 攻撃範囲を可視化（デバッグ用）
    private void OnDrawGizmosSelected()
    {
        if (transform != null)
        {
            Gizmos.color = Color.red;
            Vector3 attackPosition = transform.position + transform.forward * (attackRange / 2);
            Gizmos.DrawWireSphere(attackPosition, attackRange);
        }
    }

    public void SetAttackState(bool state)
    {
        Animator animator = GetComponent<Animator>();
        if (animator != null)
        {
            animator.SetBool("Attack", state);
        }
    }
}