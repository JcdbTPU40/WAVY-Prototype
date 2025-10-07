using UnityEngine;

public class Start_Screen : MonoBehaviour
{
    [Header("非表示にするテキスト")]
    [SerializeField] GameObject text_AnyButtonPress;
    [Header("表示するボタン")]
    [SerializeField] GameObject start_Button;
    [SerializeField] GameObject howto_Button;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.anyKeyDown)
        {
            Debug.Log("ボタン検知");

            text_AnyButtonPress.SetActive(false);
            start_Button.SetActive(true);
            howto_Button.SetActive(true);
        }
    }
}
