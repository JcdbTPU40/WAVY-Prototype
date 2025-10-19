using UnityEngine;
using UnityEngine.UI;

public class ImageBlink : MonoBehaviour
{
    public RawImage targetImage;
    [Tooltip("プレイ開始時に行う初期フェードインの時間（秒）")]
    public float initialFadeInDuration = 3f;
    public float fadeDuration = 0.5f; // フェードイン・アウトにかかる時間
    public float interval = 1f;       // 1秒間隔で点滅

    [Header("フェード曲線 (X: 正規化時間 0-1, Y: アルファ)")]
    public AnimationCurve initialFadeCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
    public AnimationCurve fadeCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

    private CanvasGroup canvasGroup;

    void Start()
    {
        // RawImageにCanvasGroupがない場合は追加
        canvasGroup = targetImage.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = targetImage.gameObject.AddComponent<CanvasGroup>();
        }

        StartCoroutine(BlinkSequence());
    }

    System.Collections.IEnumerator BlinkSequence()
    {
        if (canvasGroup == null)
        {
            yield break;
        }

        // 最初は透明にして初期フェードイン
        canvasGroup.alpha = 0f;

    yield return FadeCoroutine(0f, 1f, Mathf.Max(0.01f, initialFadeInDuration), initialFadeCurve);

        bool fadingIn = false; // 直後にフェードアウトから開始（点滅サイクル）

        while (true)
        {
            if (fadingIn)
            {
                yield return FadeCoroutine(0f, 1f, fadeDuration, fadeCurve);
            }
            else
            {
                yield return FadeCoroutine(1f, 0f, fadeDuration, fadeCurve);
            }

            fadingIn = !fadingIn;

            if (interval > 0f)
            {
                yield return new WaitForSeconds(interval);
            }
        }
    }

    private System.Collections.IEnumerator FadeCoroutine(float from, float to, float duration, AnimationCurve curve)
    {
        if (canvasGroup == null)
        {
            yield break;
        }

        if (duration <= 0f)
        {
            canvasGroup.alpha = to;
            yield break;
        }

        float elapsed = 0f;
        AnimationCurve effectiveCurve = curve ?? AnimationCurve.Linear(0f, 0f, 1f, 1f);

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            float curveValue = Mathf.Clamp01(effectiveCurve.Evaluate(t));
            canvasGroup.alpha = Mathf.Lerp(from, to, curveValue);
            yield return null;
        }

        canvasGroup.alpha = to;
    }
}
