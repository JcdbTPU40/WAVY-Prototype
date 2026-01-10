using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class BloomIntensityChanger : MonoBehaviour
{
    [SerializeField] private Volume volume;
    private Bloom bloom;
    private float duration = 3f; // 3秒間で変化
    private float startValue = 200f;
    private float endValue = 0.5f;
    private float timeElapsed = 0f;
    private bool isChanging = false;

    void Start()
    {
        // VolumeからBloomを取得
        if (volume.profile.TryGet(out bloom))
        {
            bloom.intensity.value = startValue;
            isChanging = true;
        }
    }

    void Update()
    {
        if (isChanging && bloom != null)
        {
            timeElapsed += Time.deltaTime;
            float t = Mathf.Clamp01(timeElapsed / duration);
            bloom.intensity.value = Mathf.Lerp(startValue, endValue, t);

            if (t >= 1f)
                isChanging = false;
        }
    }
}
