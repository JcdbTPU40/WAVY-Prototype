using UnityEngine;

/// <summary>
/// 指定したブロックが「非アクティブ」になったタイミングで敵を複数スポーンさせるスクリプト。
/// ・spawnBlock が OFF になった瞬間に SpawnEnemies() を発火
/// ・spawnOnlyOnce に応じて一度限り／何度でもスポーンを切り替え可能
/// ・spawnPoints それぞれの位置／回転を使って敵プレハブを配置
/// </summary>
public class spawner_destruction : MonoBehaviour
{
    [Header("スポーン地点 (複数指定可)")]
    [SerializeField] private Transform[] spawnPoints;  // 敵を出現させたい地点の配列（空や null があるとスキップ）

    [Header("生成する敵プレハブ")]
    [SerializeField] private GameObject enemyPrefab;   // 実際に Instantiate するプレハブ。未設定だと警告を表示

    [Header("ブロック判定オブジェクト (非アクティブ化でスポーン) ")]
    [SerializeField] private GameObject spawnBlock;    // このオブジェクトが非アクティブになったらスポーン処理を開始

    [Header("生成先の親 (任意)")]
    [SerializeField] private Transform spawnedParent;  // 生成した敵をまとめたい親オブジェクト。null ならルートに配置

    [Header("一度きりのスポーンにするか")]
    [SerializeField] private bool spawnOnlyOnce = true;// true: 最初の非アクティブで1回だけ / false: 再アクティブ化で再スポーン可

    private bool hasSpawned;                           // 既にスポーン済みかどうかを記録するフラグ
    private bool hasLoggedMissingSpawnBlock;           // spawnBlock 未設定警告を重複表示しないためのフラグ

    private void Awake()
    {
        // 起動直後にインスペクタの設定不足をチェックして警告を出す
        ValidateConfiguration();
    }

    private void OnEnable()
    {
        // spawnOnlyOnce が false の場合は有効化のたびにスポーン状態をリセットする
        if (!spawnOnlyOnce)
        {
            hasSpawned = false;
        }
    }

    private void Update()
    {
        // spawnBlock が未設定のままなら警告を一度出して以降の処理を止める
        if (spawnBlock == null)
        {
            if (!hasLoggedMissingSpawnBlock)
            {
                Debug.LogWarning($"{nameof(spawner_destruction)}: spawnBlock が設定されていないため処理を停止します。", this);
                hasLoggedMissingSpawnBlock = true;
            }
            return;
        }

        // spawnBlock が非アクティブ（activeSelf == false）かどうかを判定
        bool isBlockInactive = !spawnBlock.activeSelf;

        if (isBlockInactive)
        {
            // まだスポーンしていない状態でブロックが OFF になったら敵を生成
            if (!hasSpawned)
            {
                SpawnEnemies();
                hasSpawned = true; // 連続スポーンを防止
            }
        }
        else if (!spawnOnlyOnce)
        {
            // ブロックが再び ON になったら、次回 OFF になったときに再スポーンできるようリセット
            hasSpawned = false;
        }
    }

    /// <summary>
    /// インスペクタ設定の妥当性チェック。欠落があればプレイ中に気づけるよう警告を出す。
    /// </summary>
    private void ValidateConfiguration()
    {
        if (enemyPrefab == null)
        {
            Debug.LogWarning($"{nameof(spawner_destruction)}: enemyPrefab が設定されていません。", this);
        }

        if (spawnPoints == null || spawnPoints.Length == 0)
        {
            Debug.LogWarning($"{nameof(spawner_destruction)}: spawnPoints が空です。少なくとも 1 つ指定してください。", this);
        }
    }

    /// <summary>
    /// 公開 API: 外部スクリプトから直接スポーンさせたい場合に呼び出す想定。
    /// </summary>
    public void SpawnEnemies()
    {
        // プレハブやスポーン地点が未設定の場合は安全のため何もしない
        if (enemyPrefab == null || spawnPoints == null || spawnPoints.Length == 0)
        {
            return;
        }

        for (int i = 0; i < spawnPoints.Length; i++)
        {
            Transform point = spawnPoints[i];
            if (point == null)
            {
                Debug.LogWarning($"{nameof(spawner_destruction)}: spawnPoints[{i}] が設定されていません。", this);
                continue; // null の地点はスキップ
            }

            // 各スポーン地点の回転が単位行列 (identity) の場合は、親オブジェクト自体の回転を引き継ぐ
            Quaternion rotation = point.rotation == Quaternion.identity ? transform.rotation : point.rotation;

            // 敵プレハブを指定の位置・回転で生成し、spawnedParent があればその子に設定
            GameObject spawned = Instantiate(enemyPrefab, point.position, rotation, spawnedParent);

            // spawnedParent が null の場合も明示的にルートへ出す（Instantiate の第4引数で既に null の場合は不要だが念のため）
            if (spawnedParent == null)
            {
                spawned.transform.SetParent(null);
            }
        }
    }

    /// <summary>
    /// 一度スポーンした後に手動でスポーン状態をリセットしたい場合に呼ぶ。
    /// </summary>
    public void ResetSpawnState()
    {
        hasSpawned = false;
    }
}

