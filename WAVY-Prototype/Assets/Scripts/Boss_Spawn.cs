using UnityEngine;

public class Boss_Spawn : MonoBehaviour
{
    [SerializeField]
    private GameObject bossPrefab;
    const int spawnCount = 8;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Instantiate(bossPrefab, transform.position, Quaternion.identity);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
