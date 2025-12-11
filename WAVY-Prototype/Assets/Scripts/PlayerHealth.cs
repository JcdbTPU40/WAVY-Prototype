using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField, Min(1)] int maxHealth = 100;
    [SerializeField] Slider healthSlider;
    [SerializeField] string endSceneName = "End";

    [Header("HP画像")]
    [SerializeField] Image hpImage;
    [SerializeField] Sprite hp25PercentSprite;
    [SerializeField] Sprite hp50PercentSprite;
    [SerializeField] Sprite hp75PercentSprite;
    [SerializeField] Sprite hp100PercentSprite;

    [SerializeField] private TMP_Text HPPercentText;
    int currentHealth;
    bool isDead;

    public int CurrentHealth => currentHealth;
    public int MaxHealth => maxHealth;

    void Awake()
    {
        maxHealth = Mathf.Max(1, maxHealth);
        currentHealth = maxHealth;
        SyncSlider();
        UpdateHpImage();
    }

    void OnEnable()
    {
        SyncSlider();
        UpdateHpImage();
    }

    public void TakeDamage(int amount)
    {
        if (isDead || amount <= 0)
        {
            return;
        }

        currentHealth = Mathf.Max(0, currentHealth - amount);
        SyncSlider();
        UpdateHpImage();

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
        UpdateHpImage();
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

        if(HPPercentText != null)
        {
            float percent = ((float)currentHealth / maxHealth) * 100f;
            HPPercentText.text = Mathf.RoundToInt(percent).ToString() + "%";
        }
    }


    void UpdateHpImage()
    {
        if(hpImage == null)
        {
            return;
        }

        float healthPercent = (float)currentHealth / maxHealth;

        if(healthPercent >= 0.75f)
        {
            hpImage.sprite = hp100PercentSprite;
        }
        else if(healthPercent >= 0.5f)
        {
            hpImage.sprite = hp75PercentSprite;
        }
        else if(healthPercent >= 0.25f)
        {
            hpImage.sprite = hp50PercentSprite;
        }
        else
        {
            hpImage.sprite = hp25PercentSprite;
        }
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
