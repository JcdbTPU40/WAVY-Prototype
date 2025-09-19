using UnityEngine;
using UnityEngine.AI;

public class EnemyScript : MonoBehaviour
{
     [Header("移動速度")]
    public float EnemySpeed;

    [Header("攻撃範囲")]
    public float AtDistance;

    [Header("攻撃間隔")]
    public float AttackInterval;
    [Header("攻撃時間")]
    public float AttackTimer = 1.0f;

    [Header("最大HP")]
    public int MaxHP;
    private int currentHP;

    [Header("死亡時の死体を無くすまでの時間")]
    public float DeathTime = 2.0f;
    [Header("敵を倒した時のスコア")]
    public int ScoreOnDeath = 100;
    //private ScoreScript scoreScript;
    private GameObject Target;
    private NavMeshAgent agent;
    private Animator animator;
    private Rigidbody rb;
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        agent.speed = EnemySpeed;
        currentHP = MaxHP; // 初期HPを設定
        Target = GameObject.FindGameObjectWithTag("Player");
        //scoreScript = FindFirstObjectByType<ScoreScript>(); // ScoreScriptのインスタンスを取得
    }

    void Update()
    {
        if (Target != null)
        {
            float dis = Vector3.Distance(Target.transform.position, transform.position);
            transform.LookAt(Target.transform);

            if (dis > AtDistance)
            {
                if (agent != null && agent.enabled && agent.isOnNavMesh)
                {
                    agent.SetDestination(Target.transform.position);
                    //animator.SetBool("IsWalking", true);
                }
            }
            else
            {
                if (agent != null && agent.enabled && agent.isOnNavMesh)
                {
                    agent.ResetPath(); // 移動停止
                    agent.isStopped = true;
                }
                //animator.SetBool("IsWalking", false);
            }
        }
        else
        {
            //animator.SetBool("IsWalking", false);
        }
    }

    //void Attack()
    //{
    //animator.SetTrigger("Attack"); // 攻撃アニメーションをトリガーで発動
    //}

    private void OnTriggerEnter(Collider other)
    {
        var damager = other.GetComponent<DamegeScript>();
        if (damager != null)
        {
            //animator.SetTrigger("GetHit");
            Damage(damager.damage);
        }

        void Damage(int damage)
        {
            currentHP -= damage;
            if (currentHP <= 0)
            {
                currentHP = 0;
                //animator.SetTrigger("Die");

                //if (agent != null && agent.enabled && agent.isOnNavMesh)
                //{
                //agent.isStopped = true;
                //agent.enabled = false;
                //}

                Destroy(gameObject, DeathTime);
            }
        }
    }

    /*void OnDestroy()
    {
        if (scoreScript != null)
        {
            scoreScript.AddScore(ScoreOnDeath);
        }
    */
}
