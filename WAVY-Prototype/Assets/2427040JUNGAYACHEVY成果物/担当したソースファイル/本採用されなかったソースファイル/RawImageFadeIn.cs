using System.Collections;
using TMPro;
using UnityEngine;

/// <summary>
/// 指定した TextMeshPro (UGUI) をゆっくりフェードインさせるコンポーネント。
/// </summary>
public class RawImageFadeIn : MonoBehaviour
{
    [Header("対象 TextMeshPro (UGUI)")]
    [SerializeField] private TMP_Text targetText;

    [Header("フェード設定")]
    [SerializeField] private float delay = 4f;
    [SerializeField] private float fadeDuration = 1f;

    [Header("再生制御")]
    [SerializeField] private bool playOnStart = true;

    private CanvasGroup canvasGroup;
    private Coroutine fadeRoutine;

    private void Awake()
    {
        if (targetText == null)
        {
            targetText = GetComponentInChildren<TMP_Text>();
        }

        if (targetText == null)
        {
            Debug.LogWarning($"{nameof(RawImageFadeIn)}: TMP_Text が見つかりません。Inspector で設定してください。", this);
            enabled = false;
            return;
        }

        canvasGroup = targetText.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = targetText.gameObject.AddComponent<CanvasGroup>();
        }
    }

    private void OnEnable()
    {
        if (playOnStart)
        {
            Play();
        }
    }

    /// <summary>
    /// フェードイン処理を初期化して再生する。
    /// </summary>
    public void Play()
    {
        if (canvasGroup == null)
        {
            return;
        }

        if (fadeRoutine != null)
        {
            StopCoroutine(fadeRoutine);
        }

        canvasGroup.alpha = 0f;
        fadeRoutine = StartCoroutine(FadeInRoutine());
    }

    private IEnumerator FadeInRoutine()
    {
        if (delay > 0f)
        {
            yield return new WaitForSeconds(delay);
        }

        float elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float normalized = fadeDuration > 0f ? Mathf.Clamp01(elapsed / fadeDuration) : 1f;
            canvasGroup.alpha = Mathf.Lerp(0f, 1f, normalized);
            yield return null;
        }

        canvasGroup.alpha = 1f;
        fadeRoutine = null;
    }
}
