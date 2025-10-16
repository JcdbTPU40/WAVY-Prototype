using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class SuperMarioGalaxyMainMenuManager : MonoBehaviour
{
    [Header("Input Actions")]
    [SerializeField] private InputActionAsset inputActions;
    [SerializeField] private string actionMapName = "MainMenu";
    [SerializeField] private string startActionName = "Start";

    [Header("UI")]
    [SerializeField] private CanvasGroup panelCanvasGroup;
    [SerializeField] private float panelFadeDuration = 0.4f;
    [SerializeField] private CanvasGroup nextPanelCanvasGroup;
    [SerializeField] private float nextPanelFadeDuration = 0.4f;

    [Header("HDR Multiplier Effect")]
    [SerializeField] private HDRMultiplierLerp hdrMultiplierLerp;
    [SerializeField] private bool waitForHdrEffectCompletion = true;

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip startSfx;

    private InputAction startAction;
    private bool isTransitionRunning;
    private bool hasTransitionCompleted;

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
        if (isTransitionRunning || hasTransitionCompleted)
        {
            return;
        }

        PlayStartSound();
        StartCoroutine(PlayStartSequence());
    }

    private IEnumerator PlayStartSequence()
    {
        isTransitionRunning = true;

        yield return StartCoroutine(PlayHdrEffectIfAvailable());

        if (panelCanvasGroup != null)
        {
            yield return StartCoroutine(FadeCanvasGroup(panelCanvasGroup, 0f, panelFadeDuration));
            panelCanvasGroup.gameObject.SetActive(false);
        }

        if (nextPanelCanvasGroup != null)
        {
            if (!nextPanelCanvasGroup.gameObject.activeSelf)
            {
                nextPanelCanvasGroup.gameObject.SetActive(true);
            }

            nextPanelCanvasGroup.alpha = 0f;
            yield return StartCoroutine(FadeCanvasGroup(nextPanelCanvasGroup, 1f, nextPanelFadeDuration));
        }

        isTransitionRunning = false;
        hasTransitionCompleted = true;
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

    private IEnumerator PlayHdrEffectIfAvailable()
    {
        if (hdrMultiplierLerp == null)
        {
            yield break;
        }

        if (waitForHdrEffectCompletion)
        {
            yield return hdrMultiplierLerp.PlayCoroutine();
        }
        else
        {
            hdrMultiplierLerp.Play();
        }
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
