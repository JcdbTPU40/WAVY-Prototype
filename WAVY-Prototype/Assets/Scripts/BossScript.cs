// ...existing code...
using UnityEngine;
using UnityEngine.AI;

public class BossScript : MonoBehaviour
{
    [Header("ボスのHP")]
    [SerializeField]
    private int boss_Max_HP = 100;//ボスの最大体力
    private int boss_CurrentHP;  //ボスの現在体力
    private bool boss_isDied;     //ボスの死亡判定

    [Header("移動速度")]
    [SerializeField]
    float speed = 10.0f;

    [Header("ボスの飛ぶ高さ")]
    [SerializeField]
    float flightHeight = 20.0f;

    [Header("円移動設定")]
    [SerializeField]
    private Transform circleCenter = null;                  // 円の中心（未設定ならスポーン位置を中心に）
    [SerializeField]
    private Vector3 circleCenterOffset = Vector3.zero;      // 中心からのオフセット
    [SerializeField, Min(0f)]
    private float circleRadius = 10f;                       // 円半径
    [SerializeField]
    private float angularSpeed = 45f;                       // 角速度（度/秒）
    [SerializeField]
    private bool clockwise = true;                          // 時計回りフラグ

    [Header("調査（タワー到着）挙動")]
    [SerializeField, Min(0f)]
    private float investigateArrivalThreshold = 1.0f;      // 到着判定距離（水平）
    [SerializeField, Min(0f)]
    private float defaultInvestigateStayDuration = 5f;     // 指定が無い場合の滞在時間（秒）

    // 追加: 射撃設定
    [Header("射撃設定")]
    [SerializeField, Tooltip("プレイヤー方向へ発射する弾のプレハブ (Rigidbody 必須推奨)")]
    private GameObject projectilePrefab = null;
    [SerializeField, Min(0.01f), Tooltip("弾の速度")]
    private float projectileSpeed = 20f;
    [SerializeField, Min(0f), Tooltip("発射間隔（秒）")]
    private float shootInterval = 1.5f;
    [SerializeField, Min(0f), Tooltip("射程（この距離内のプレイヤーにのみ撃つ。0 なら距離無視）")]
    private float shootRange = 0f;
    [SerializeField, Tooltip("発射位置のオフセット（ボスの基準座標系）")]
    private Vector3 shootOffset = Vector3.forward * 1.5f;
    [SerializeField, Min(0f), Tooltip("生成した弾の自動破棄時間")]
    private float projectileLifetime = 8f;
    [SerializeField, Tooltip("調査モード中にも射撃するか")]
    private bool shootWhileInvestigating = false;

    private float currentAngleDeg = 0f;                     // 現在角度（度）
    private Vector3 initialCircleCenter = Vector3.zero;     // Start 時のデフォルト中心

    private NavMeshAgent agent;

    // --- 調査モード用 ---
    bool isInvestigating = false;
    bool investigateArrived = false;
    Vector3 investigatePosition = Vector3.zero; // 世界座標（水平成分のみ使用）
    float investigateStayDuration = 0f;
    float investigateEndTime = 0f;
    // --------------------

    // 射撃内部管理
    float shootTimer = 0f;
    Transform playerTransform = null;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        if (agent != null)
        {
            agent.updatePosition = false;
            agent.updateUpAxis = false;
        }

        boss_CurrentHP = boss_Max_HP;

        // 初期中心を記録（circleCenter 未設定時のフォールバック）
        initialCircleCenter = new Vector3(transform.position.x, 0f, transform.position.z);

        // 現在角度を初期化（中心から見た角度）
        Vector3 centerForInit = (circleCenter != null) ? (circleCenter.position + circleCenterOffset) : initialCircleCenter;
        Vector3 horizPos = new Vector3(transform.position.x, 0f, transform.position.z);
        Vector3 horizCenter = new Vector3(centerForInit.x, 0f, centerForInit.z);
        Vector3 dir = horizPos - horizCenter;
        if (dir.sqrMagnitude > Mathf.Epsilon)
        {
            currentAngleDeg = Mathf.Atan2(dir.z, dir.x) * Mathf.Rad2Deg;
        }
        else
        {
            currentAngleDeg = 0f;
        }

        // プレイヤー参照をキャッシュ（Player タグを利用）
        var playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            playerTransform = playerObj.transform;
        }

        shootTimer = shootInterval; // 起動直後にすぐ撃ちたい場合は 0 にする
    }

    void Update()
    {
        // --- 調査モード優先 ---
        if (isInvestigating)
        {
            // 調査先のワールドYは flightHeight とする
            Vector3 targetWorld = new Vector3(investigatePosition.x, flightHeight, investigatePosition.z);

            // 到着前は移動、到着後は滞在（滞在時間が経過したら調査終了）
            if (!investigateArrived)
            {
                MoveTowardsTarget(targetWorld);

                // 水平方向の到着判定
                Vector3 horizPos = new Vector3(transform.position.x, 0f, transform.position.z);
                Vector3 horizTarget = new Vector3(targetWorld.x, 0f, targetWorld.z);
                if ((horizPos - horizTarget).sqrMagnitude <= investigateArrivalThreshold * investigateArrivalThreshold)
                {
                    investigateArrived = true;
                    investigateEndTime = Time.time + Mathf.Max(0f, investigateStayDuration);
                }
            }
            else
            {
                // 到着して滞在中
                if (Time.time >= investigateEndTime)
                {
                    // 調査終了、円移動に戻る
                    isInvestigating = false;
                    investigateArrived = false;
                }
                // 滞在中は位置を厳密に維持する（NavMesh 対応）
                if (agent != null)
                {
                    Vector3 navTarget = new Vector3(targetWorld.x, 0f, targetWorld.z);
                    agent.SetDestination(navTarget);
                    Vector3 nextPos = agent.nextPosition;
                    Vector3 targetPos = new Vector3(nextPos.x, flightHeight, nextPos.z);
                    transform.position = Vector3.MoveTowards(transform.position, targetPos, speed * Time.deltaTime);
                }
                else
                {
                    transform.position = Vector3.MoveTowards(transform.position, targetWorld, speed * Time.deltaTime);
                }
            }

            // 調査中に射撃しない場合はここで early return
            if (!shootWhileInvestigating)
            {
                // タイマーは進めない／リセットしておく
                return;
            }
        }

        // --- 射撃タイマー更新 ---
        if (!boss_isDied && projectilePrefab != null && playerTransform != null)
        {
            shootTimer += Time.deltaTime;
            if (shootTimer >= Mathf.Max(0.0001f, shootInterval))
            {
                TryShootAtPlayer();
                shootTimer = 0f;
            }
        }

        // --- 円移動（通常運転） ---
        Vector3 center = (circleCenter != null) ? (circleCenter.position + circleCenterOffset) : initialCircleCenter;

        float dirSign = clockwise ? -1f : 1f;
        currentAngleDeg += angularSpeed * Time.deltaTime * dirSign;
        if (currentAngleDeg > 360f || currentAngleDeg < -360f)
        {
            currentAngleDeg = Mathf.Repeat(currentAngleDeg, 360f);
        }

        float rad = currentAngleDeg * Mathf.Deg2Rad;
        Vector3 desiredPos = new Vector3(center.x + Mathf.Cos(rad) * circleRadius,
                                         flightHeight,
                                         center.z + Mathf.Sin(rad) * circleRadius);

        // NavMesh には水平位置のみをセット
        if (agent != null)
        {
            Vector3 navTarget = new Vector3(desiredPos.x, 0f, desiredPos.z);
            agent.SetDestination(navTarget);

            Vector3 nextPos = agent.nextPosition;
            Vector3 targetPos = new Vector3(nextPos.x, flightHeight, nextPos.z);

            transform.position = Vector3.MoveTowards(transform.position, targetPos, speed * Time.deltaTime);
        }
        else
        {
            // NavMeshAgent が無い場合は直接移動
            transform.position = Vector3.MoveTowards(transform.position, desiredPos, speed * Time.deltaTime);
        }
    }

    // 目的地へ移動（NavMeshAgent優先、なければ直接移動）
    void MoveTowardsTarget(Vector3 worldTarget)
    {
        if (agent != null)
        {
            Vector3 navTarget = new Vector3(worldTarget.x, 0f, worldTarget.z);
            agent.SetDestination(navTarget);
            Vector3 nextPos = agent.nextPosition;
            Vector3 targetPos = new Vector3(nextPos.x, flightHeight, nextPos.z);
            transform.position = Vector3.MoveTowards(transform.position, targetPos, speed * Time.deltaTime);
        }
        else
        {
            transform.position = Vector3.MoveTowards(transform.position, worldTarget, speed * Time.deltaTime);
        }
    }

    // プレイヤーに向けて発射を試みる
    void TryShootAtPlayer()
    {
        if (playerTransform == null || projectilePrefab == null || boss_isDied) return;

        // 射程チェック（0 以下なら距離無視）
        if (shootRange > 0f)
        {
            float sqr = (new Vector3(playerTransform.position.x, 0f, playerTransform.position.z) - new Vector3(transform.position.x, 0f, transform.position.z)).sqrMagnitude;
            if (sqr > shootRange * shootRange) return;
        }

        // 発射位置（ボスの高さを flightHeight に揃える）
        Vector3 basePos = new Vector3(transform.position.x, flightHeight, transform.position.z);
        Vector3 spawnPos = basePos + transform.TransformDirection(shootOffset);

        Vector3 targetPos = playerTransform.position;
        // プレイヤーの高さに合わせて狙う（水平成分を使って飛行する場合は y を調整）
        targetPos = new Vector3(targetPos.x, playerTransform.position.y, targetPos.z);

        Vector3 dir = (targetPos - spawnPos);
        if (dir.sqrMagnitude <= Mathf.Epsilon)
        {
            dir = transform.forward;
        }
        dir.Normalize();

        // インスタンス化
        GameObject proj = Instantiate(projectilePrefab, spawnPos, Quaternion.LookRotation(dir));
        if (proj != null)
        {
            // Rigidbody があれば速度を与える
            Rigidbody rb = proj.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.linearVelocity = dir * projectileSpeed;
            }
            else
            {
                // Rigidbody が無ければ transform で前方方向に移動する可能性を残す
                proj.transform.forward = dir;
            }

            // 自動破棄
            if (projectileLifetime > 0f)
            {
                Destroy(proj, projectileLifetime);
            }
        }
    }

    // 外部から呼ばれる：タワー破壊時に呼び出してボスを調査モードへ切替える
    public void OnTowerDestroyed(Vector3 towerWorldPosition, float stayDuration = -1f)
    {
        // 滞在時間がマイナスならデフォルトを使う
        investigateStayDuration = (stayDuration >= 0f) ? stayDuration : defaultInvestigateStayDuration;

        // 水平座標のみ保存（Y は flightHeight で統一）
        investigatePosition = new Vector3(towerWorldPosition.x, 0f, towerWorldPosition.z);
        isInvestigating = true;
        investigateArrived = false;
        // 到着時に滞在時間がカウントされるように、investigateEndTime は到着時にセットする
    }
// ...existing code...
    public void take_Damage(int damage)
    {
        if (boss_isDied || damage <= 0)
        {
            return;
        }

        boss_CurrentHP -= damage;

        if (boss_CurrentHP <= 0)
        {
            died_process();
        }
    }

    public void died_process()
    {
        if (boss_isDied)
        {
            return;
        }

        boss_isDied = true;

        //boss死亡時の処理
        Destroy(gameObject);
    }
}
// ...existing code...