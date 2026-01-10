using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro;

public class InputCountdownUI : MonoBehaviour
{
    [Header("Input")]
    public InputActionReference actionRef;

    [Header("UI References")]
    public Image fillImage;
    public Image alphaImage;
    public TextMeshProUGUI countdownText;  // 推奨
    public Text legacyText;                // 旧Text使うならこちら

    [Header("Timing")]
    public float duration = 3.0f;

    [Header("Alpha")]
    [Range(0f, 1f)] public float holdAlpha = 0.4f;
    private float _alphaOriginal = 1f;

    private Coroutine _fillCo;
    private Coroutine _alphaCo;
    private Coroutine _countCo;

    private void OnEnable()
    {
        if (actionRef != null && actionRef.action != null)
        {
            actionRef.action.performed += OnPerformed;
            actionRef.action.Enable();
        }
    }

    private void OnDisable()
    {
        if (actionRef != null && actionRef.action != null)
        {
            actionRef.action.performed -= OnPerformed;
            actionRef.action.Disable();
        }
    }

    private void OnPerformed(InputAction.CallbackContext ctx)
    {
        // 1) Fill: 実行中なら無視、止まっている時だけ開始
        if (fillImage != null && _fillCo == null)
        {
            _fillCo = StartCoroutine(FillDownThenReset(fillImage, duration));
        }

        // 2) Countdown Text: 実行中なら無視。止まっている時だけ再アクティブ＆開始
        if ((countdownText != null || legacyText != null) && _countCo == null)
        {
            EnsureTextActive(true);
            _countCo = StartCoroutine(CountDown(duration));
        }

        // 3) Alpha: 実行中なら無視。止まっている時だけ開始
        if (alphaImage != null && _alphaCo == null)
        {
            _alphaCo = StartCoroutine(AlphaHoldThenRestore(alphaImage, duration, holdAlpha));
        }
    }

    private System.Collections.IEnumerator FillDownThenReset(Image img, float time)
    {
        img.fillAmount = 1f;
        float t = 0f;
        while (t < time)
        {
            t += Time.deltaTime;
            float p = Mathf.Clamp01(t / time);
            img.fillAmount = 1f - p;
            yield return null;
        }
        img.fillAmount = 1f; // 即リセット
        _fillCo = null;
    }

    private System.Collections.IEnumerator CountDown(float time)
    {
        float remain = time;
        while (remain > 0f)
        {
            remain -= Time.deltaTime;
            SetTextSeconds(Mathf.Max(0f, remain));
            yield return null;
        }
        SetTextSeconds(0f);
        EnsureTextActive(false); // 0秒で非アクティブ
        _countCo = null;
    }

    private System.Collections.IEnumerator AlphaHoldThenRestore(Image img, float time, float targetAlpha)
    {
        var c = img.color;
        _alphaOriginal = c.a;

        // すぐ指定αへ
        c.a = Mathf.Clamp01(targetAlpha);
        img.color = c;

        float t = 0f;
        while (t < time)
        {
            t += Time.deltaTime;
            yield return null;
        }

        // 終わったら即戻す
        c.a = _alphaOriginal;
        img.color = c;

        _alphaCo = null;
    }

    private void SetTextSeconds(float seconds)
    {
        string s = seconds.ToString("0.0");
        if (countdownText != null) countdownText.text = s;
        if (legacyText != null) legacyText.text = s;
    }

    private void EnsureTextActive(bool active)
    {
        if (countdownText != null) countdownText.gameObject.SetActive(active);
        if (legacyText != null) legacyText.gameObject.SetActive(active);
    }
}
