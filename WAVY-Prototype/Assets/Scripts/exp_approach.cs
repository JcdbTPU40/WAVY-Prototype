using UnityEngine;

public class exp_approach : MonoBehaviour
{
    [SerializeField] GameObject exp;
    [SerializeField] float speed = 0.4f;
    [SerializeField] float maxTimer = 10.0f;
    [SerializeField] float collectDistance = 1.0f;
    private float timer = 0.0f;
    private bool isCollect = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        collect();
    }

    // Update is called once per frame
    void Update()
    {
        if (!isCollect)
        {
            return;
        }

        timer += Time.deltaTime;

        //回収の最大時間を超えていないかチェック
        if (timer > maxTimer)
        {
            FinishCollect();
            return;
        }

        GameObject player = GameObject.Find("Player");

        //プレイヤーに向かって進ませる
        transform.position = Vector3.MoveTowards(transform.position, player.transform.position, speed);

        float distance = (player.transform.position - transform.position).magnitude;
        Debug.Log($"Distance to player: {distance}");

        //特定の距離まで近づいたら回収完了
        //var diff = player.transform.position - transform.position;
        if (distance < collectDistance)
        {
            Debug.Log("Player is close enough. Finishing collect.");
            FinishCollect();
        }
    }

    public void collect()
    {
        timer = 0.0f;
        isCollect = true;
    }
    public void FinishCollect()
    {
        Debug.Log("FinishCollect called");
        isCollect = false;
        
        Destroy(this.gameObject);
	
    }
}
