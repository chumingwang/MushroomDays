using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class WaterScroll : MonoBehaviour
{
    [Header("Texture Scroll Speeds")]
    public float albedoSpeedX = 0.02f;
    public float albedoSpeedY = 0.01f;
    public float normalSpeedX = -0.015f;
    public float normalSpeedY = 0.008f;

    private Renderer rend;
    private Vector2 albedoOffset;
    private Vector2 normalOffset;

    void Awake()
    {
        rend = GetComponent<Renderer>();
    }

    void Update()
    {
        float t = Time.time;

        albedoOffset.x = t * albedoSpeedX;
        albedoOffset.y = t * albedoSpeedY;

        normalOffset.x = t * normalSpeedX;
        normalOffset.y = t * normalSpeedY;

        if (rend.material.HasProperty("_MainTex"))
            rend.material.SetTextureOffset("_MainTex", albedoOffset);

        if (rend.material.HasProperty("_BumpMap"))
            rend.material.SetTextureOffset("_BumpMap", normalOffset);
    }
}
