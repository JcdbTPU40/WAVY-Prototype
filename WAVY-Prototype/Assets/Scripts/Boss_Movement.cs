using UnityEngine;
using UnityEngine.AI;

public class Boss_Movement : MonoBehaviour
{
    [Header("ボスのHP")]
    [SerializeField]
    private int boss_Max_HP = 100;//ボスの最大体力
    private int boss_CurrentHP;  //ボスの現在体力
    private bool boss_isDied;     //ボスの死亡判定

    [Header("移動先のプレハブ")]
    [SerializeField]
    private Transform target;

    [Header("移動速度")]
    [SerializeField]
    float speed = 10.0f;

    [Header("ボスの飛ぶ高さ")]
    [SerializeField]
    float flightHeight = 20.0f;

    private NavMeshAgent agent;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.updatePosition = false;
        agent.updateUpAxis = false;

        boss_CurrentHP = boss_Max_HP;
    }

    // Update is called once per frame
    void Update()
    {
        agent.SetDestination(target.position);

        Vector3 nextPos = agent.nextPosition;

        Vector3 targetPos = new Vector3(nextPos.x, flightHeight, nextPos.z);

        transform.position = Vector3.MoveTowards(transform.position, targetPos, speed * Time.deltaTime);
    }

    public void OnTriggerEnter(Collider other)
    {
        Debug.Log("Call");
        var damager = other.GetComponent<DamegeScript>();
        if (damager != null)
        {
            take_Damage(damager.damage);
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

        //boss死亡時の処理
        Destroy(gameObject);
    }
}
