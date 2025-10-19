using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class FadeOutImage : MonoBehaviour
{
    [Header("対象のImage")]
    public Image targetImage;

    [Header("フェード時間(秒)")]
    public float duration = 1.0f;

    [Header("フェードカーブ (0→1)")]
    public AnimationCurve fadeCurve = AnimationCurve.Linear(0, 0, 1, 1);
    // ↑ デフォルトは直線（等速）
    // 例: EaseOutにしたいなら → Keyframe(0,0), Keyframe(1,1)を少しカーブさせる

    void Start()
    {
        StartCoroutine(FadeOut());
    }

    IEnumerator FadeOut()
    {
        Color color = targetImage.color;
        float startAlpha = 1f;
        float endAlpha = 0f;
        float time = 0f;

        while (time < duration)
        {
            time += Time.deltaTime;
            float t = time / duration;

            // カーブを適用（0〜1の補間値をカーブ経由で変化）
            float curveValue = fadeCurve.Evaluate(t);
            color.a = Mathf.Lerp(startAlpha, endAlpha, curveValue);

            targetImage.color = color;
            yield return null;
        }

        // 最後に透明度を完全に0に
        color.a = endAlpha;
        targetImage.color = color;
    }
}