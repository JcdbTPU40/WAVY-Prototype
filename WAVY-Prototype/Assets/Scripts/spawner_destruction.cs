using UnityEngine;

public class spawner_destruction : MonoBehaviour
{
    [SerializeField] Transform[] Spawnpoint;
    [SerializeField] GameObject Enemy,SpawnBlock;

    bool a = true;
    void Update()
    {
        
        if (SpawnBlock.activeSelf==false && a)
        {
            BSpawn();
            a = false;
        }
    }
    public void BSpawn()
    {
        for (int i = 0; i < Spawnpoint.Length; i++)
        {
            Instantiate(Enemy, Spawnpoint[i].position, Quaternion.identity);
        }
    }
}

