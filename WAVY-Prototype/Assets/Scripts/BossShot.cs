using UnityEngine;

public class BossShot : MonoBehaviour
{
    [Header("SE")]
    [SerializeField] AudioSource seSource;
    [SerializeField] AudioClip chargeStartSE;
    [SerializeField] AudioClip beamShotSE;

    [Header("弾")]
    [SerializeField] GameObject BeamDecisionPrefab;
    [SerializeField] GameObject BeamPrefab;
    [SerializeField] GameObject BeamArea;
    [SerializeField] GameObject BeamChargePrefab;
    [SerializeField]float spawnOffset=3f;
    [SerializeField]float spawnOffsetA=50f;//赤い予測線の位置調整
    Quaternion beamRotation;
    GameObject beamArea;
    GameObject beamCharge;
    [Header("射撃関連")]
    [SerializeField] float beamSpan=5f;//beamを撃つまでの間隔（秒）
    [SerializeField] float shotSpan=2f;//チャージ開始までの間隔（秒）
    public float shotTime=0f;
    public bool areaSpawned=false;
    public bool beamSpawned=false;
    
    [SerializeField] Transform parent;

    Vector3 playerPosition;
    Vector3 sPawnPosition;
    
    void PlaySE(AudioClip clip)
    {
    if (clip == null || seSource == null) return;
    seSource.PlayOneShot(clip);
    }

    // Update is called once per frame
    void Update()
    {
        shotTime+=Time.deltaTime;

        //Debug.Log(shotTime);
        
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
            Vector3 areaSpawnPosition=transform.position + transform.forward * spawnOffsetA ;

            if(!areaSpawned)
            {
                PlaySE(chargeStartSE);

                beamArea=Instantiate(BeamArea,areaSpawnPosition,beamRotation,parent);
                beamCharge=Instantiate(BeamChargePrefab,sPawnPosition,beamRotation,parent);

                Destroy(beamArea,5f);
                Destroy(beamCharge,5f);

                areaSpawned=true;
            }
        }
        if(shotTime>=beamSpan)
        {
            if(!beamSpawned)
            {
            Instantiate(BeamPrefab,sPawnPosition,beamRotation* Quaternion.Euler(0, 180, 0));
            Instantiate(BeamDecisionPrefab,sPawnPosition,beamRotation,parent);

            PlaySE(beamShotSE);

            beamSpawned=true;
            }
            //shotTime=0f;
        }
    }
}
