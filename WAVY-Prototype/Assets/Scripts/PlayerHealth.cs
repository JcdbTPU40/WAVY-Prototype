using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class PlayerHealth : MonoBehaviour
{
    [Header("Ult設定")]
    [SerializeField]public float ultTailSize=30f;
    [SerializeField]float keepTime=5f;
    [SerializeField]float ultTime=5f;
    float keepCount;
    public bool IsUlt;

    [Header("体力設定")]
    [SerializeField,Min(1)] int startHealth=100;
    [SerializeField, Min(1)] int maxHealth = 150;
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
        startHealth = Mathf.Max(1, startHealth);
        currentHealth = startHealth;
        keepCount=0;
        IsUlt=false;

        SyncSlider();
        UpdateHpImage();
    }

    void OnEnable()
    {
        SyncSlider();
        UpdateHpImage();
    }

    void Update()
    {
        if(currentHealth>=150f)
        {
            keepCount+=Time.deltaTime;
        }
        else
        {
            keepCount=0;
            IsUlt=false;
        }

        if(keepCount>=keepTime)
        {
            IsUlt=true;
        }

        if(keepCount-keepTime>=ultTime)
        {
            keepCount=0;
            IsUlt=false;
            currentHealth=100;
            SyncSlider();
            UpdateHpImage();
        }
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
            float percent = (float)currentHealth / startHealth * 100f;
            percent = Mathf.Clamp(percent, 0f, (float)maxHealth / startHealth * 100f);
            Debug.Log(percent);
            HPPercentText.text = Mathf.RoundToInt(percent) + "%";
        }
    }


    void UpdateHpImage()
    {
        if(hpImage == null)
        {
            return;
        }

        float healthPercent = (float)currentHealth / startHealth;

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
