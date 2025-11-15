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
        SceneManager.LoadScene("Start");
    }
}
