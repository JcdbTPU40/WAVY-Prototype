using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class QuitScript : MonoBehaviour
{
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            QuitApplication();
        }
    }

    /// <summary>
    /// UI ボタンの OnClick から呼び出してゲームを終了させます。
    /// </summary>
    public void ExitButton()
    {
        QuitApplication();
    }

    private static void QuitApplication()
    {
#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
