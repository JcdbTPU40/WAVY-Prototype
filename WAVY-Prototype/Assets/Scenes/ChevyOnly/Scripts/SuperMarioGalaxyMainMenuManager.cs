using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class SuperMarioGalaxyMainMenuManager : MonoBehaviour
{
    [Header("Input Actions")]
    [SerializeField] private InputActionAsset inputActions;
    [SerializeField] private string actionMapName = "MainMenu";
    [SerializeField] private string startActionName = "Start";

    [Header("Bloom Settings")]
    [SerializeField] private Volume globalVolume;
    [SerializeField] private float bloomIntensityPeak = 30f;
    [SerializeField] private float bloomFadeInDuration = 0.5f;
    [SerializeField] private float bloomHoldDuration = 0.2f;
    [SerializeField] private float bloomFadeOutDuration = 1f;

    [Header("UI")]
    [SerializeField] private CanvasGroup panelCanvasGroup;
    [SerializeField] private float panelFadeDuration = 0.4f;

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip startSfx;

    private InputAction startAction;
    private Bloom bloom;
    private bool isTransitionRunning;
    private float defaultBloomIntensity;

    private void Awake()
    {
    if (globalVolume == null)
    {
#if UNITY_2023_1_OR_NEWER
        globalVolume = FindFirstObjectByType<Volume>();
#else
        globalVolume = FindObjectOfType<Volume>();
#endif
    }

        if (globalVolume != null)
        {
            globalVolume.profile.TryGet(out bloom);
            if (bloom != null)
            {
                defaultBloomIntensity = bloom.intensity.value;
            }
        }
    }

    private void OnEnable()
    {
        SetupInput();
    }

    private void OnDisable()
    {
        if (startAction != null)
        {
            startAction.started -= OnStartAction;
            startAction.Disable();
        }
    }

    private void SetupInput()
    {
        if (inputActions == null)
        {
            Debug.LogWarning("InputActionAsset が設定されていません", this);
            return;
        }

        InputActionMap map = inputActions.FindActionMap(actionMapName, true);
        if (map == null)
        {
            Debug.LogWarning($"ActionMap '{actionMapName}' が見つかりません", this);
            return;
        }

        startAction = map.FindAction(startActionName, true);
        if (startAction == null)
        {
            Debug.LogWarning($"Action '{startActionName}' が見つかりません", this);
            return;
        }

        startAction.started += OnStartAction;
        startAction.Enable();
        map.Enable();
    }

    private void OnStartAction(InputAction.CallbackContext context)
    {
        if (isTransitionRunning)
        {
            return;
        }

        PlayStartSound();
        StartCoroutine(PlayStartSequence());
    }

    private IEnumerator PlayStartSequence()
    {
        isTransitionRunning = true;

        if (bloom != null)
        {
            yield return StartCoroutine(AnimateBloom());
        }

        if (panelCanvasGroup != null)
        {
            yield return StartCoroutine(FadeCanvasGroup(panelCanvasGroup, 0f, panelFadeDuration));
            panelCanvasGroup.gameObject.SetActive(false);
        }

        isTransitionRunning = false;
    }

    private IEnumerator AnimateBloom()
    {
        if (bloom == null)
        {
            yield break;
        }

        float elapsed = 0f;
        float startIntensity = bloom.intensity.value;
        float targetIntensity = bloomIntensityPeak;

        while (elapsed < bloomFadeInDuration)
        {
            elapsed += Time.deltaTime;
            float t = bloomFadeInDuration > 0f ? Mathf.Clamp01(elapsed / bloomFadeInDuration) : 1f;
            bloom.intensity.value = Mathf.Lerp(startIntensity, targetIntensity, t);
            yield return null;
        }

        bloom.intensity.value = targetIntensity;

        if (bloomHoldDuration > 0f)
        {
            yield return new WaitForSeconds(bloomHoldDuration);
        }

        elapsed = 0f;
        while (elapsed < bloomFadeOutDuration)
        {
            elapsed += Time.deltaTime;
            float t = bloomFadeOutDuration > 0f ? Mathf.Clamp01(elapsed / bloomFadeOutDuration) : 1f;
            bloom.intensity.value = Mathf.Lerp(targetIntensity, defaultBloomIntensity, t);
            yield return null;
        }

        bloom.intensity.value = defaultBloomIntensity;
    }

    private IEnumerator FadeCanvasGroup(CanvasGroup group, float targetAlpha, float duration)
    {
        if (group == null)
        {
            yield break;
        }

        float startAlpha = group.alpha;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = duration > 0f ? Mathf.Clamp01(elapsed / duration) : 1f;
            group.alpha = Mathf.Lerp(startAlpha, targetAlpha, t);
            yield return null;
        }

        group.alpha = targetAlpha;
    }

    private void PlayStartSound()
    {
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }

        if (audioSource == null)
        {
            return;
        }

        if (startSfx != null)
        {
            audioSource.PlayOneShot(startSfx);
        }
        else if (audioSource.clip != null)
        {
            audioSource.Play();
        }
    }
}
