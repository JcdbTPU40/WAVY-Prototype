using UnityEngine;
using UnityEngine.SceneManagement;

public class ToEnd : MonoBehaviour
{
    public void TimeUp() //時間切れ時の処理
    {
        // カーソルを表示＆ロック解除
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        SceneManager.LoadScene("End");
    }
}
