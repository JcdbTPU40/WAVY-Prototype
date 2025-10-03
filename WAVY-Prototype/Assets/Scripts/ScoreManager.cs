using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class IntEvent : UnityEvent<int> { }

/// <summary>
/// シンプルなスコア管理用シングルトン。<br/>
/// ・シーンをまたいで保持（任意）<br/>
/// ・スコア加算時に UnityEvent で通知<br/>
/// </summary>
public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance { get; private set; }

    [Header("初期設定")]
    [SerializeField] private bool persistAcrossScenes = true;
    [SerializeField] private int startingScore = 0;

    [Header("イベント (Inspector から UI 連携可能)")]
    [SerializeField] private IntEvent scoreChanged = new IntEvent();

    public int CurrentScore => currentScore;

    int currentScore;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        if (persistAcrossScenes)
        {
            DontDestroyOnLoad(gameObject);
        }

        currentScore = Mathf.Max(0, startingScore);
        scoreChanged.Invoke(currentScore);
    }

    void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    /// <summary>
    /// スコアを加算します。0 以下を渡した場合は無視。
    /// </summary>
    public void AddScore(int amount)
    {
        if (amount <= 0)
        {
            return;
        }

        currentScore += amount;
        scoreChanged.Invoke(currentScore);
    }

    /// <summary>
    /// スコアを任意の値にリセットします。
    /// </summary>
    public void ResetScore(int newScore = 0)
    {
        currentScore = Mathf.Max(0, newScore);
        scoreChanged.Invoke(currentScore);
    }

    /// <summary>
    /// Inspector から UnityEvent を購読したい場合に公開。
    /// </summary>
    public IntEvent ScoreChanged => scoreChanged;
}
