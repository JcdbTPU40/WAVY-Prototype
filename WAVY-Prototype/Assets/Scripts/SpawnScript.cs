using UnityEngine;

public class SpawnScript : MonoBehaviour
{
    [Header("敵プレハブ")]
    [SerializeField] GameObject enemy;

    [Header("湧きペース：秒")]
    [SerializeField] float repeat ;

    [Header("時間経過：秒")]
    float time;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        time = 0;
    }

    // Update is called once per frame
    void Update()
    {
        time += Time.deltaTime;

        Vector3 right = transform.right * 1.5f;

        if(time>repeat)
        {
            Instantiate(enemy, transform.position + right, transform.rotation);
            time = 0;
        }
    }
}
