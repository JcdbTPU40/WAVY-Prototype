using UnityEngine;
using UnityEngine.SceneManagement;
public class ToEnd : MonoBehaviour
{
    public void TimeUp() //時間切れ時の処理
    {
        SceneManager.LoadScene("End");
    }
}
