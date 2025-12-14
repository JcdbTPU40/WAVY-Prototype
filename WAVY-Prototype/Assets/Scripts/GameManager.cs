using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem; // New Input System integration

public class GameManager : MonoBehaviour
{
    [Header("Pause Menu")]
    [SerializeField] private GameObject pauseMenuUI;
    [SerializeField] private bool manageCursorVisibility = true;

    [Header("Pause Button Image")]
    [SerializeField] private PopUpController pauseButtonController;

    [Header("Cursor")]
    [Tooltip("These scenes will force cursor visible/unlocked even when not paused.")]
    [SerializeField] private string[] forceCursorVisibleSceneNames = new[] { "Start", "GameOver", "GameClear", "End" };

    [Header("Scenes")]
    [SerializeField] private string mainMenuSceneName = "Start";

    public static GameManager Instance { get; private set; }

    public bool IsPaused { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    private PlayerInputActions inputActions;

    private void OnEnable()
    {
        if (inputActions == null)
        {
            inputActions = new PlayerInputActions();
        }

        inputActions.Player.Pause.performed -= OnPausePerformed;
        inputActions.Player.Pause.performed += OnPausePerformed;
        inputActions.Enable();

        SceneManager.activeSceneChanged -= OnActiveSceneChanged;
        SceneManager.activeSceneChanged += OnActiveSceneChanged;
    }

    private void Start()
    {
        ResumeGame();
        ApplyCursorStateForScene(SceneManager.GetActiveScene().name);
    }

    private void OnPausePerformed(InputAction.CallbackContext ctx)
    {
        TogglePause();
    }

    public void TogglePause()
    {
        if (IsPaused)
        {
            ResumeGame();
        }
        else
        {
            PauseGame();
        }
    }

    public void PauseGame()
    {
        if (IsPaused)
        {
            return;
        }

        IsPaused = true;
        Time.timeScale = 0f;

        if (pauseMenuUI != null)
        {
            pauseMenuUI.SetActive(true);
            pauseButtonController.UpdateButtonSprite(true);
        }
        UpdateCursorState();
    }

    public void ResumeGame()
    {
        if (!IsPaused && Time.timeScale.Equals(1f))
        {
            if (pauseMenuUI != null && pauseMenuUI.activeSelf)
            {
                pauseButtonController.UpdateButtonSprite(false);
            }

            UpdateCursorState();
            return;
        }

        IsPaused = false;
        Time.timeScale = 1f;

        if (pauseMenuUI != null)
        {
            pauseMenuUI.SetActive(false);
            pauseButtonController.UpdateButtonSprite(false);
        }

        if(pauseButtonController != null)
        {   
            pauseMenuUI.SetActive(false);
            pauseButtonController.UpdateButtonSprite(false);
        }

        UpdateCursorState();
    }

    public void ReturnToMainMenu()
    {
        Time.timeScale = 1f;
        IsPaused = false;

        if (pauseMenuUI != null)
        {
            pauseMenuUI.SetActive(false);
        }

        UpdateCursorState(forceVisible: true);

        if (!string.IsNullOrWhiteSpace(mainMenuSceneName))
        {
            SceneManager.LoadScene(mainMenuSceneName);
        }
        else
        {
            Debug.LogError("Start scene name is not set on the GameManager.");
        }
    }

    // Backwards compatibility if some Button still points to old method name in Inspector
    public void ReturnTomainMenuSceneName()
    {
        ReturnToMainMenu();
    }

    public void OnPauseButtonPressed()
    {
        TogglePause();
    }

    private void OnDisable()
    {
        if (inputActions != null)
        {
            inputActions.Player.Pause.performed -= OnPausePerformed;
            inputActions.Disable();
        }

        SceneManager.activeSceneChanged -= OnActiveSceneChanged;
    }

    private void OnActiveSceneChanged(Scene oldScene, Scene newScene)
    {
        ApplyCursorStateForScene(newScene.name);
    }

    private void ApplyCursorStateForScene(string sceneName)
    {
        UpdateCursorState(forceVisible: ShouldForceCursorVisibleForScene(sceneName));
    }

    private bool ShouldForceCursorVisibleForScene(string sceneName)
    {
        if (string.IsNullOrWhiteSpace(sceneName) || forceCursorVisibleSceneNames == null)
        {
            return false;
        }

        for (int i = 0; i < forceCursorVisibleSceneNames.Length; i++)
        {
            string forcedSceneName = forceCursorVisibleSceneNames[i];
            if (string.IsNullOrWhiteSpace(forcedSceneName))
            {
                continue;
            }

            if (string.Equals(sceneName, forcedSceneName, System.StringComparison.Ordinal))
            {
                return true;
            }
        }

        return false;
    }

    private void UpdateCursorState(bool forceVisible = false)
    {
        if (!manageCursorVisibility)
        {
            return;
        }

        bool showCursor = forceVisible || IsPaused;
        Cursor.visible = showCursor;
        Cursor.lockState = showCursor ? CursorLockMode.None : CursorLockMode.Locked;
    }
}
