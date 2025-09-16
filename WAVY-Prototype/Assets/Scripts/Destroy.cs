using UnityEngine;

public class Destroy : MonoBehaviour
{
    [SerializeField] GameObject SpawnBlock;
    int time;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        time = 0;
    }

    // Update is called once per frame
    void Update()
    {
        time++;
        if (time > 1000)
        {
            SpawnBlock.SetActive(false);
        }
    }
}

