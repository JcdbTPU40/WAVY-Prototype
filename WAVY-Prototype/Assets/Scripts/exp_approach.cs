using UnityEngine;

public class exp_approach : MonoBehaviour
{
    [SerializeField] GameObject exp;
    [SerializeField] float speed = 0.4f;
    [SerializeField] float maxTimer = 10.0f;
    [SerializeField] float collectDistance = 1.0f;
    [SerializeField, Min(0)] int healAmount = 10;

    [Header("ビルボード")]
    public bool yAxisOnly = false;
    [Min(0f)] public float rotationLerpSpeed = 0f;　//回転スムーズ速度
    public string playerTag = "Player"; //Playerタグ配下から探索

    private Transform cam;
    private Transform player;
    private PlayerHealth playerHealth;
    private float timer = 0.0f;
    private bool isCollect = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ResolvePlayer();
        collect();
    }

    // Update is called once per frame
    void Update()
    {
        Billboard();

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

        if (player == null || !player.gameObject.activeInHierarchy)
        {
            if (!ResolvePlayer())
            {
                return;
            }
        }

        //プレイヤーに向かって進ませる
        transform.position = Vector3.MoveTowards(transform.position, player.position, speed * Time.deltaTime);

        float distance = (player.position - transform.position).magnitude;
        //Debug.Log($"Distance to player: {distance}");

        //特定の距離まで近づいたら回収完了
        //var diff = player.transform.position - transform.position;
        if (distance < collectDistance)
        {
            Debug.Log("Player is close enough. Finishing collect.");
            FinishCollect();
        }
    }

    private void Billboard() //オブジェクトが常にカメラの方を向くようにする
    {
          if (exp == null) return;

        if (cam == null || !cam.gameObject.activeInHierarchy)
        {
            ResolveCamera(false);
            if (cam == null) return;
        }

        Vector3 targetPos = cam.position;
        Vector3 dir = targetPos - exp.transform.position;

        if (yAxisOnly)
        {
            dir.y = 0f;
        }

        if (dir.sqrMagnitude < 1e-6f) return;

        Quaternion targetRot = Quaternion.LookRotation(dir.normalized, Vector3.up);

        if (rotationLerpSpeed > 0f)
        {
            exp.transform.rotation = Quaternion.Slerp(
                exp.transform.rotation,
                targetRot,
                1f - Mathf.Exp(-rotationLerpSpeed * Time.deltaTime)
            );
        }
        else
        {
            exp.transform.rotation = targetRot;
        }
    }

    private void ResolveCamera(bool firstTry)
    {
        // Playerタグ配下から探索
        if (!string.IsNullOrEmpty(playerTag))
        {
            var player = GameObject.FindGameObjectWithTag(playerTag);
            if (player != null)
            {
                var playerCam = player.GetComponentInChildren<Camera>(true);
                if (playerCam != null)
                {
                    cam = playerCam.transform;
                    return;
                }
            }
        }

        // Camera.main をフォールバックとして使用
        if (Camera.main != null)
        {
            cam = Camera.main.transform;
            return;
        }
    }

    bool ResolvePlayer()
    {
        GameObject candidate = null;

        if (!string.IsNullOrEmpty(playerTag))
        {
            candidate = GameObject.FindGameObjectWithTag(playerTag);
        }

        if (candidate == null)
        {
            candidate = GameObject.Find("Player");
        }

        if (candidate == null)
        {
            player = null;
            playerHealth = null;
            return false;
        }

        player = candidate.transform;
        playerHealth = candidate.GetComponent<PlayerHealth>();
        return true;
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
        TryHealPlayer();

        Destroy(this.gameObject);
	
    }

    void TryHealPlayer()
    {
        if (player == null || playerHealth == null)
        {
            if (!ResolvePlayer())
            {
                return;
            }
        }

        if (playerHealth == null)
        {
            return;
        }

        playerHealth.Heal(healAmount);
    }
}
