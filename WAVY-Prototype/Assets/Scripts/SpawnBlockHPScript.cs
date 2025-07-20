using UnityEngine;
using UnityEngine.Timeline;

public class SpawnBlockHP : MonoBehaviour
{
    [SerializeField] public int hp = 100;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Damage(int Attack)
    {
        hp -= Attack;

        if (hp <= 0)
        {
            Destroy(gameObject);

            Debug.Log($"{gameObject.name}が破壊されました。");
        }
    }
}
