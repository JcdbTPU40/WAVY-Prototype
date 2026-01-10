using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class HDRMultiplierLerp : MonoBehaviour
{
    [SerializeField] private RawImage targetImage; // 対象RawImage
    [SerializeField] private float startValue = 900f;
    [SerializeField] private float endValue = 1f;
    [SerializeField] private float duration = 1f;
    [SerializeField] private bool playOnAwake = true;

    private Material mat;
    private float timer = 0f;
    private bool isRunning = false;

    public float Duration => duration;
    public bool IsRunning => isRunning;

    void Start()
    {
        if (!EnsureMaterialInstance())
        {
            Debug.LogError("RawImageが指定されていません。", this);
            return;
        }

        ApplyValue(startValue);

        if (playOnAwake)
        {
            Play();
        }
    }

    void Update()
    {
        if (!isRunning) return;

        timer += Time.deltaTime;
        float t = Mathf.Clamp01(timer / duration);
        float value = Mathf.Lerp(startValue, endValue, t);
        ApplyValue(value);

        if (t >= 1f) isRunning = false;
    }

    // HDR Multiplierを変化させる処理を開始
    public void Play()
    {
        if (!EnsureMaterialInstance())
        {
            Debug.LogError("RawImageが指定されていません。", this);
            return;
        }

        timer = 0f;
        isRunning = true;
        ApplyValue(startValue);
    }

    public IEnumerator PlayCoroutine()
    {
        Play();
        while (isRunning)
        {
            yield return null;
        }
    }

    public void StopAndReset()
    {
        isRunning = false;
        timer = 0f;
        ApplyValue(startValue);
    }

    private bool EnsureMaterialInstance()
    {
        if (targetImage == null)
        {
            return false;
        }

        if (mat == null)
        {
            if (targetImage.material == null)
            {
                Debug.LogWarning("RawImageにマテリアルが設定されていません。", this);
                return false;
            }

            mat = Instantiate(targetImage.material);
            targetImage.material = mat;
        }

        return true;
    }

    private void ApplyValue(float value)
    {
        if (mat == null)
        {
            return;
        }

        mat.SetFloat("_HdrMultiply", value);
    }
}
