using UnityEngine;

public class BeamScript : MonoBehaviour
{
    [Header("サイズ")]
    [SerializeField]const int width=7;
    float length;
    [SerializeField] float beamLimit =80f; 

    [SerializeField] float beamSpeed=1f;
    float prevLength;

    
     // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        length=1f;
        prevLength=length;

        transform.localScale = new Vector3(width, width, length);
    }

    // Update is called once per frame
    void Update()
    {
         length+=Time.deltaTime*beamSpeed;
         transform.localScale = new Vector3(width,width,length);

         float delta = length - prevLength;

         transform.localPosition+=new Vector3(0f,0f,delta);
        
        prevLength=length;
        if(length>=beamLimit)
        {
        Destroy(gameObject);

        initialize();
        }
    }

    public void initialize()
    {
        BossShot beam=GameObject.Find("Boss_Object").GetComponent<BossShot>();

        beam.beamSpawned=false;
        beam.areaSpawned=false;
        beam.shotTime=0f;
    }
}
