using JetBrains.Annotations;
using UnityEngine;

public class TailScalebyHealth : MonoBehaviour
{
    [SerializeField] private PlayerHealth playerHealth;  // 参照する PlayerHealth
    [SerializeField] private float smoothSpeed = 5f;
    public float size;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (playerHealth == null)
            return;

        size=(float)playerHealth.CurrentHealth/10;

        // 変化後のサイズ
        Vector3 SIZE=new Vector3(size,size,size);
            transform.localScale = Vector3.Lerp(transform.localScale, SIZE, Time.deltaTime * smoothSpeed);
            transform.localPosition= Vector3.Lerp(transform.localPosition, new Vector3(-(SIZE.z/8f)+0.3f, 0, 0), Time.deltaTime * smoothSpeed);
    }
}
