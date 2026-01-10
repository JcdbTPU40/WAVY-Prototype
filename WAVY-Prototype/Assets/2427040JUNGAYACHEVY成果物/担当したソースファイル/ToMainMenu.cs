using UnityEngine;
using UnityEngine.SceneManagement;

public class ToMainMenu : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }
    
    public void MenuBtn() //�X�^�[�g�{�^�����\�b�h�������Main���Ăяo��
    {
        ScoreManager.ResetScoreGlobal(0);
        SceneManager.LoadScene("Start");
    }

    public void RetryBtn() 
    {
        ScoreManager.ResetScoreGlobal(0);
        SceneManager.LoadScene("Mario Stage 1");
    }
}
