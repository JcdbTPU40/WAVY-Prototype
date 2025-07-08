using UnityEngine;

public class EnemyScript : MonoBehaviour
{
     [Header("移動速度")]
    public float EnemySpeed;

    [Header("攻撃範囲")]
    public float AtDistance;

    GameObject Target;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
         if (Target != null)
        {
            float dis = Vector3.Distance(Target.transform.position, transform.position);

            transform.LookAt(Target.transform);

            if (dis > AtDistance)
            {
                transform.Translate(Vector3.forward * EnemySpeed * Time.deltaTime);
            }
            else
            {
                Attack();
            }
        }
    }

    void Attack()
    {
        Debug.Log("攻撃されました！");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Target = other.gameObject;
            Debug.Log("範囲内に入りました");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Target = null;
            Debug.Log("範囲外になりました");
        }
    }
    
}
