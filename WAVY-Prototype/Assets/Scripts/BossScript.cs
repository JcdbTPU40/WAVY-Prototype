using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
using UnityEngine.SocialPlatforms.GameCenter;
using UnityEngine.UI;

public class BossScript : MonoBehaviour
{
    [Header("ボスのHP")]
    [SerializeField]
    private int boss_Max_HP = 100; // ボスの最大体力
    private int boss_CurrentHP;    // ボスの現在体力
    private bool boss_isDied;      // ボスの死亡判定

    [Header("移動関連")]
    //private NavMeshAgent agent;
    [SerializeField]Transform O;
    [SerializeField]Transform O2;

    [SerializeField] Slider healthSlider;

    bool One;
    bool Two;
    void Start()
    {
        boss_CurrentHP = boss_Max_HP;
        boss_isDied = false;
        /*agent=GetComponent<NavMeshAgent>();

        if (!agent.isOnNavMesh)
        {
            Debug.LogError("BossがNavMesh上にいません！");
        }*/

        SyncSlider();

        One=false;
        Two=true;
    }

    void Update()
    {
        Debug.Log(boss_CurrentHP);
        //NavMeshAgent.SetDestination(O.position);
        if(Two)
        {
            transform.position+=new Vector3(0.01f,0,0);
        //agent.SetDestination(O.position);
        if(transform.position.x>=10)
        {
        Two=false;
        One=true;
        }
        }

        if(One)
        {
            transform.position+=new Vector3(-0.01f,0,0);
        //agent.SetDestination(O2.position);
        if(transform.position.x<=-10)
        {
        Two=true;
        One=false;
        }
        }
    }

    public void take_Damage(int damage)
    {
        if (boss_isDied || damage <= 0)
        {
            return;
        }

        boss_CurrentHP -= damage;

        SyncSlider();

        if (boss_CurrentHP <= 0)
        {
            died_process();
        }
    }

    public void SyncSlider()
    {
        if (healthSlider == null)
        {
            return;
        }

        if (!Mathf.Approximately(healthSlider.maxValue, boss_Max_HP))
        {
            healthSlider.maxValue = boss_Max_HP;
        }

        if (!Mathf.Approximately(healthSlider.minValue, 0f))
        {
            healthSlider.minValue = 0f;
        }

        if (!Mathf.Approximately(healthSlider.value, boss_CurrentHP))
        {
            healthSlider.value = boss_CurrentHP;
        }
    }

    public void died_process()
    {
        if (boss_isDied)
        {
            return;
        }

        boss_isDied = true;

        StartCoroutine(GameClearSquence());
    }

    IEnumerator GameClearSquence()
    {
        // ゲームクリア画面に遷移
        yield return new WaitForSeconds(2f); // 2秒待機（任意で調整可能）
        SceneManager.LoadScene("GameClear");
    }

    public void OnTowerDestroyed(Vector3 towerWorldPos, float stayDuration )
    {
        return;
    }
}