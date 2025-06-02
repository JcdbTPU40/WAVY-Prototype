using UnityEngine;

public class QuitScript : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        EndGame();
    }

    public void EndGame() //Escボタン押したらゲーム終了
    {
        if(Input.GetKey(KeyCode.Escape))
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
    Application.Quit(); //ゲームプレイ終了
#endif
        }
    }

    public void ExitBtn() //ボタンを押したらゲーム終了
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
    Application.Quit(); //ゲームプレイ終了
#endif
    }
}
