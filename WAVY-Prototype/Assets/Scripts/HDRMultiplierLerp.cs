using UnityEngine;
using UnityEngine.UI;

public class HDRMultiplierController : MonoBehaviour
{
    public RawImage targetImage; // 対象RawImage
    public float startValue = 900f;
    public float endValue = 1f;
    public float duration = 1f;

    private Material mat;
    private float timer = 0f;
    private bool isRunning = false;

    void Start()
    {
        if (targetImage == null)
        {
            Debug.LogError("RawImageが指定されていません。");
            return;
        }

        // マテリアルのインスタンスを作成（共有マテリアルを汚さないように）
        mat = Instantiate(targetImage.material);
        targetImage.material = mat;

        // 初期値をセット
        mat.SetFloat("_HdrMultiply", startValue);

        // ✅ 追加：起動時に自動で実行
        Play();
    }

    void Update()
    {
        if (!isRunning) return;

        timer += Time.deltaTime;
        float t = Mathf.Clamp01(timer / duration);
        float value = Mathf.Lerp(startValue, endValue, t);
        mat.SetFloat("_HdrMultiply", value);

        if (t >= 1f) isRunning = false;
    }

    // HDR Multiplierを変化させる処理を開始
    public void Play()
    {
        timer = 0f;
        isRunning = true;
    }
}
