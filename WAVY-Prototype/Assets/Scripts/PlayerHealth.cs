using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    [Header("プレイヤーHP設定")]
    public int maxHealth = 100;
    public int currentHealth = 100;

    [Header("回復量設定")]
    public int expHealAmount = 10;

    [Header("死亡時設定")]
    [Tooltip("死亡してからリザルトへ移行するまでの秒数")]
    public float deathDelay = 5f;

    [SerializeField] private Animator animator;
    [SerializeField] private string deathTriggerName = "Die";

    private bool isDead;

    [Header("UI")]
    [SerializeField] private Slider healthSlider;

    void Awake()
    {
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        if (animator == null)
        {
            animator = GetComponentInChildren<Animator>();
        }

        // スライダー初期化
        if (healthSlider != null)
        {
            healthSlider.maxValue = maxHealth;
            healthSlider.value = currentHealth;
        }
    }

    void UpdateHealthUI()
    {
        if (healthSlider != null)
        {
            healthSlider.value = currentHealth;
        }
    }

    public void TakeDamage(int amount)
    {
        if (isDead || amount <= 0) return;

        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        UpdateHealthUI();

        if (currentHealth <= 0)
        {
            HandleDeath();
        }
    }

    public void HealFromExp()
    {
        if (isDead || expHealAmount <= 0) return;

        currentHealth += expHealAmount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        UpdateHealthUI();
    }

    void HandleDeath()
    {
        if (isDead) return;
        isDead = true;

        if (animator != null && !string.IsNullOrEmpty(deathTriggerName))
        {
            animator.SetTrigger(deathTriggerName);
        }

        // 入力や移動を止めたい場合はここでコンポーネント無効化などを行う

        StartCoroutine(DeathRoutine());
    }

    IEnumerator DeathRoutine()
    {
        yield return new WaitForSeconds(deathDelay);

        // ToEnd と同様に End シーンへ遷移
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        SceneManager.LoadScene("End");
    }
}
