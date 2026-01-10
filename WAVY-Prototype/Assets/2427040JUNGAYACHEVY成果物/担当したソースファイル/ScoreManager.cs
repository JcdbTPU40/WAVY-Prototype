using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class IntEvent : UnityEvent<int> { }

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance { get; private set; }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void EnsureInstanceExists()
    {
        // 初回プレイ時に「開始シーンに ScoreManager が置かれていない」場合でも
        // 敵の死亡処理などで AddScore がスキップされないように、最初に1つ用意する。
        if (Instance != null)
        {
            return;
        }

#if UNITY_2023_1_OR_NEWER || UNITY_6000_0_OR_NEWER
        var existing = Object.FindFirstObjectByType<ScoreManager>(FindObjectsInactive.Include);
#else
        var existing = Object.FindObjectOfType<ScoreManager>(true);
#endif

        if (existing != null)
        {
            return;
        }

        var go = new GameObject(nameof(ScoreManager));
        var manager = go.AddComponent<ScoreManager>();
        manager.persistAcrossScenes = true;
        manager.startingScore = currentScore;
    }

    [Header("初期設定")]
    [SerializeField] private bool persistAcrossScenes = true;
    [SerializeField] private int startingScore = 0;

    [Header("イベント (Inspector から UI 連携可能)")]
    [SerializeField] private IntEvent scoreChanged = new IntEvent();

    public int CurrentScore => currentScore;

    static int currentScore;

    /// <summary>
    /// Instance の有無に関わらずスコアをリセットします。
    /// 既に ScoreManager が存在する場合はイベントも発火します。
    /// </summary>
    public static void ResetScoreGlobal(int newScore = 0)
    {
        currentScore = Mathf.Max(0, newScore);

        if (Instance != null)
        {
            Instance.scoreChanged.Invoke(currentScore);
        }
    }

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
