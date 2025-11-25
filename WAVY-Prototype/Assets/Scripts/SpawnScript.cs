using UnityEngine;

/// <summary>
/// 一定間隔で敵プレハブを生成するシンプルなスポーナー。
/// ・repeat 秒ごとに SpawnEnemy() を呼び出す
/// ・敵プレハブ未設定時は警告を一度出して処理停止
/// ・spawnOffset で「自分の右方向」にオフセット生成
/// </summary>
public class SpawnScript : MonoBehaviour
{
    [Header("敵プレハブ (生成したい Enemy のプレハブ)")]
    [SerializeField] private GameObject enemy;            // 生成対象。未設定だと起動時に警告を出し停止

    [Header("湧きペース：秒 (この秒数ごとに敵を生成)")]
    [SerializeField, Min(MinRepeatInterval)] private float repeat = 2.0f; // 最小値は定数で管理し、Inspector上でも同じ制約を共有

    [Header("スポーン位置の右方向オフセット (ローカルX方向)")]
    [SerializeField] private float spawnOffset = 1.5f;    // 自身の transform.right * offset だけずらして配置

    [Header("時間経過：秒 (経過時間の内部カウンタ)")]
    private float elapsedTime = 0f;                       // 経過秒数を累積し、repeat 到達でリセット

    private bool isEnemyPrefabMissing = false;            // プレハブ欠落時に true → Update 内で早期 return して無駄呼び出し防止

    // 設定下限の安全値（極端な高速生成防止用のガード値）
    private const float MinRepeatInterval = 0.1f;

    private void Awake()
    {
        // インスペクタ設定の妥当性チェック（欠落や 0 以下補正など）
        ValidateConfiguration();
    }

    private void Update()
    {
        // プレハブ未設定なら何もしない（警告は Awake/実行時欠落で一回表示済み）
        if (isEnemyPrefabMissing)
        {
            return;
        }

        // フレームごとの経過時間を加算
        elapsedTime += Time.deltaTime;

        // まだ設定間隔に達していなければ待機
        if (elapsedTime >= repeat)
        {
            // 指定間隔に達したので 1 体スポーン
            SpawnEnemy();

            // カウンタをリセット。ただし極端なフレーム落ちで repeat を大幅に超えた場合は余剰時間を保持
            elapsedTime -= repeat;
            if (elapsedTime < 0f)
            {
                elapsedTime = 0f;
            }
        }
    }

    /// <summary>
    /// 敵を 1 体生成。プレハブ欠落が判明した場合は以降停止。
    /// </summary>
    private void SpawnEnemy()
    {
        if (enemy == null)
        {
            // 実行中にプレハブが削除/アンロード等で失われたケース
            Debug.LogWarning($"{nameof(SpawnScript)}: 敵プレハブが設定されておらず、スポーンを中断しました。", this);
            isEnemyPrefabMissing = true;
            return;
        }

        // 自身の位置から右方向（ローカル X 軸）にオフセットした地点に生成
        Vector3 spawnPosition = transform.position + transform.forward * spawnOffset;

        // 回転はこのオブジェクトの回転をそのまま継承
        Instantiate(enemy, spawnPosition, transform.rotation);
    }

    /// <summary>
    /// 起動時の設定チェック。
    /// ・プレハブ未設定 → 警告 & 停止
    /// ・repeat 0 以下 → 最小値へ補正
    /// </summary>
    private void ValidateConfiguration()
    {
        if (enemy == null)
        {
            Debug.LogWarning($"{nameof(SpawnScript)}: 敵プレハブがインスペクターで設定されていません。", this);
            isEnemyPrefabMissing = true;
        }

        if (repeat <= 0f)
        {
            Debug.LogWarning($"{nameof(SpawnScript)}: repeat が 0 以下のため {MinRepeatInterval} 秒に補正します。", this);
            repeat = MinRepeatInterval;
        }
    }
}
