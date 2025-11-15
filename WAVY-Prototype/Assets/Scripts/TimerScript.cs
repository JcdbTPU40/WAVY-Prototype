using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TimerScript : MonoBehaviour
{
    private float totalTime; //総計時間
    [SerializeField] private int minutes; //残り時間の分
    [SerializeField] private float seconds; //残り時間の秒
    private float oldSeconds; //前回のフレームの秒数を保持するための変数
    private TextMeshProUGUI timerText;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        totalTime = minutes * 60 + seconds; //例えば3分 *  60 = 180 180 + 0秒 = 総計180秒)
        oldSeconds = 0f;
        timerText = GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        totalTime = minutes * 60 + seconds; //現在の残り時間を計算
        totalTime -= Time.deltaTime;

        minutes = (int)totalTime / 60;  //分の計算
        seconds = totalTime - minutes * 60;

        if ((int)seconds != (int)oldSeconds)
        {
            timerText.text = minutes.ToString("00") + ":" + ((int)seconds).ToString("00"); //UIのテキスト表示を更新
        }
        oldSeconds = seconds;

        if (totalTime <= 0f)
        {
            return; //0秒を過ぎても処理が続くのを防ぐためにリターンを入れる
        }
    }

    
}
