using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem; // New Input System integration

public class GameManager : MonoBehaviour
{
    [Header("Pause Menu")]
    [SerializeField] private GameObject pauseMenuUI;
    [SerializeField] private bool manageCursorVisibility = true;

    [Header("Scenes")]
    [SerializeField] private string mainMenuSceneName = "Main Menu";

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
    }

    private void Start()
    {
        ResumeGame();
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
        }
        else
        {
            Debug.LogWarning("Pause menu UI is not assigned in the GameManager inspector.");
        }

        UpdateCursorState();
    }

    public void ResumeGame()
    {
        if (!IsPaused && Time.timeScale.Equals(1f))
        {
            if (pauseMenuUI != null && pauseMenuUI.activeSelf)
            {
                pauseMenuUI.SetActive(false);
            }

            UpdateCursorState();
            return;
        }

        IsPaused = false;
        Time.timeScale = 1f;

        if (pauseMenuUI != null)
        {
            pauseMenuUI.SetActive(false);
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
            Debug.LogError("Main menu scene name is not set on the GameManager.");
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
