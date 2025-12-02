using UnityEngine;

public class BossShot : MonoBehaviour
{
    [Header("弾")]
    [SerializeField] GameObject BeamPrefab;
    [SerializeField] GameObject BeamArea;
    GameObject beamArea;
    [Header("射撃関連")]
    [SerializeField] float beamSpan=5f;
    
    [SerializeField] float shotSpan=2f;
    public float shotTime=0f;
    float spawnOffset=3f;
    Quaternion beamRotation;
    public bool areaSpawned=false;
    public bool beamSpawned=false;
    
    [SerializeField] Transform parent;

    Vector3 playerPosition;
    Vector3 sPawnPosition;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        shotTime+=Time.deltaTime;

        Debug.Log(shotTime);
        
        if(shotTime>=shotSpan)
        {
            if(!beamSpawned)
            {
            playerPosition=GameObject.FindWithTag("Player").transform.position;
            }
            Vector3 beamRay=playerPosition - transform.position;
            
            transform.rotation = Quaternion.LookRotation(beamRay);

            Vector3 ray=transform.forward;
            beamRotation=Quaternion.LookRotation(ray);

            sPawnPosition=transform.position + transform.forward * spawnOffset+ transform.up * 3f;
            Vector3 areaSpawnPosition=transform.position + transform.forward * 75 ;

            if(!areaSpawned)
            {
            beamArea=Instantiate(BeamArea,areaSpawnPosition,beamRotation,parent);

            Destroy(beamArea,5f);

            areaSpawned=true;
            }
        }
        if(shotTime>=beamSpan)
        {
            if(!beamSpawned)
            {
            Instantiate(BeamPrefab,sPawnPosition,beamRotation,parent);

            beamSpawned=true;
            }
            //shotTime=0f;
        }
    }
}
