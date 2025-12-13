using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShowScore : MonoBehaviour
{
    public TextMeshProUGUI scoreText; 

    void OnEnable()
    {
        // 初期表示
        int current = ScoreManager.Instance != null ? ScoreManager.Instance.CurrentScore : 0;
        if (scoreText != null) scoreText.text =  current.ToString();

        // 以降の更新を購読（別シーンで表示する場合にも対応）
        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.ScoreChanged.AddListener(OnScoreChanged);
        }
    }

    void OnDisable()
    {
        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.ScoreChanged.RemoveListener(OnScoreChanged);
        }
    }

    void OnScoreChanged(int newScore)
    {
        if (scoreText != null) scoreText.text = "Score: " + newScore.ToString();
    }
}