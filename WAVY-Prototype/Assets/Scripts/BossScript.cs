using UnityEngine;
using UnityEngine.AI;

public class BossScript : MonoBehaviour
{
    [Header("ボスのHP")]
    [SerializeField]
    private int boss_Max_HP = 100; // ボスの最大体力
    private int boss_CurrentHP;    // ボスの現在体力
    private bool boss_isDied;      // ボスの死亡判定    

    float speed=20f;

    

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

    public void OnTowerDestroyed(Vector3 towerWorldPos, float stayDuration )
    {
        return;
    }
}