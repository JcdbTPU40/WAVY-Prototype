using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    [SerializeField] private string firstStageSceneName = "Stage1";

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PlayGame()
    {
        Time.timeScale = 1f; // 念のため（ポーズ解除）
        if (!string.IsNullOrWhiteSpace(firstStageSceneName))
        {
            SceneManager.LoadScene(firstStageSceneName);
        }
        else
        {
            Debug.LogError("Stage シーン名が設定されていません。");
        }
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
