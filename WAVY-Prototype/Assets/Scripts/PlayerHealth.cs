using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField, Min(1)] int maxHealth = 100;
    [SerializeField] Slider healthSlider;
    [SerializeField] string endSceneName = "End";

    int currentHealth;
    bool isDead;

    public int CurrentHealth => currentHealth;
    public int MaxHealth => maxHealth;

    void Awake()
    {
        maxHealth = Mathf.Max(1, maxHealth);
        currentHealth = maxHealth;
        SyncSlider();
    }

    void OnEnable()
    {
        SyncSlider();
    }

    public void TakeDamage(int amount)
    {
        if (isDead || amount <= 0)
        {
            return;
        }

        currentHealth = Mathf.Max(0, currentHealth - amount);
        SyncSlider();

        if (currentHealth <= 0)
        {
            HandleDeath();
        }
    }

    public void Heal(int amount)
    {
        if (isDead || amount <= 0)
        {
            return;
        }

        currentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth);
        SyncSlider();
    }

    void SyncSlider()
    {
        if (healthSlider == null)
        {
            return;
        }

        if (!Mathf.Approximately(healthSlider.maxValue, maxHealth))
        {
            healthSlider.maxValue = maxHealth;
        }

        if (!Mathf.Approximately(healthSlider.minValue, 0f))
        {
            healthSlider.minValue = 0f;
        }

        healthSlider.value = currentHealth;
    }

    void HandleDeath()
    {
        if (isDead)
        {
            return;
        }

        isDead = true;
        Time.timeScale = 1f;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        if (!string.IsNullOrWhiteSpace(endSceneName))
        {
            SceneManager.LoadScene(endSceneName);
        }
        else
        {
            Debug.LogError("End scene name is not set on PlayerHealth.");
        }
    }
}
