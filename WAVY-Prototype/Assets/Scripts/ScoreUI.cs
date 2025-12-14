using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.SocialPlatforms.Impl;

public class ScoreUI : MonoBehaviour
{
    [SerializeField] private TMP_Text scoreText;

    IEnumerator Start()
    {
        yield return null;

        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.ScoreChanged.AddListener(UpdateScoreUI);
            UpdateScoreUI(ScoreManager.Instance.CurrentScore); // 初期スコアを反映
        }
    }

    public void UpdateScoreUI(int newScore)
    {
        if (scoreText != null )
        {
            scoreText.text = $"{newScore}";
        }
    }
}