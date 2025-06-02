using TMPro;
using UnityEngine;

public class TimerScript : MonoBehaviour
{
    private float totalTime; //合計時間
    [SerializeField] private int minutes; //制限時間の分
    [SerializeField] private float seconds; //制限時間の秒
    private float oldSeconds; //タイマーの安定性、滑らかさの表示
    private TextMeshProUGUI timerText;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        totalTime = minutes * 60 + seconds; //例（3分 * 60 = 180 180 + 0秒 = 合計180秒)
        oldSeconds = 0f;
        timerText = GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        totalTime = minutes * 60 + seconds; //合計時間の計算
        totalTime -= Time.deltaTime;

        minutes = (int)totalTime / 60;　  //　再設定
        seconds = totalTime - minutes * 60;

        if ((int)seconds != (int)oldSeconds)
        {
            timerText.text = minutes.ToString("00") + ":" + ((int)seconds).ToString("00"); //UIのテキスト表示（時間）
        }
        oldSeconds = seconds;

        if (totalTime <= 0f)
        {
            return; //0秒以下になったら処理を止めるよ！　//ビームシーンの切り替えになる予定。
        }
    }
}
