using UnityEngine;
using UnityEngine.AI;

public class BossScript : MonoBehaviour
{
    [Header("ボスのHP")]
    [SerializeField]
    private int boss_Max_HP = 100; // ボスの最大体力
    private int boss_CurrentHP;    // ボスの現在体力
    private bool boss_isDied;      // ボスの死亡判定

    [Header("移動速度")]
    [SerializeField]
    float speed = 10.0f;
    [Header("円移動関連")]
    private int angle;
    private int aSPeed;
    

    [Header("ボスの飛ぶ高さ")]
    [SerializeField]
    float flightHeight = 20.0f;

    private NavMeshAgent agent;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        if (agent != null)
        {
            agent.updatePosition = false;
            agent.updateUpAxis = false;
        }

        boss_CurrentHP = boss_Max_HP;
    }

    void Update()
    {
        // 調査モード等は削除。高さを flightHeight に保つだけの挙動にする。
        Vector3 maintainPos = new Vector3(transform.position.x, flightHeight, transform.position.z);

        if (agent != null)
        {
            // NavMeshAgent があれば agent.nextPosition を使って自然に高低を保つ
            Vector3 nextPos = agent.nextPosition;
            Vector3 targetPos = new Vector3(nextPos.x, flightHeight, nextPos.z);
            transform.position = Vector3.MoveTowards(transform.position, targetPos, speed * Time.deltaTime);
        }
        else
        {
            transform.position = Vector3.MoveTowards(transform.position, maintainPos, speed * Time.deltaTime);
        }
    }

    // NavMeshAgent 優先で目的地へ移動するユーティリティ（残しておく）
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

    // 互換性確保のためメソッドは残すが、調査モードは実行しない（無効化スタブ）
    public void OnTowerDestroyed(Vector3 towerWorldPosition, float stayDuration = -1f)
    {
        // 調査モードは削除されたため、ここでは何もしません（呼び出し側の呼び出しエラー回避用スタブ）
    }

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

        // boss死亡時の処理
        Destroy(gameObject);
    }
}