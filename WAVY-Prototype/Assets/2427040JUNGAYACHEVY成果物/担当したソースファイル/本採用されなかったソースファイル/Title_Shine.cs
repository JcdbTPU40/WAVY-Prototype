using UnityEngine;

public class Title_Shine : MonoBehaviour
{
    Renderer title_Renderer;
    Material title_Material;
    float emissionStrength = 0f;
    Color emissionColor = Color.white;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        title_Renderer = gameObject.GetComponent<Renderer>();
        title_Material = title_Renderer.material;
        title_Material.EnableKeyword("_EMISSION");
    }

    // Update is called once per frame
    void Update()
    {
        emissionStrength = Mathf.PingPong(Time.time * 2.0f, 2.0f);

        Color finalColor = emissionColor * emissionStrength;

        title_Material.SetColor("_EmissionColor", finalColor);
    }
}
