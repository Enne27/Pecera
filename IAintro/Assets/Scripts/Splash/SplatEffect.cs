using UnityEngine;

public class SplatEffect : MonoBehaviour
{
    [Header("Materials")]
    public Material waterMaterial;    // el que usa Custom/UnlitRipple
    public Material splatMaterial;    // el que usa Custom/RippleSplat
    public Material decayMaterial;    // el que usa Custom/RippleDecay

    [Header("Settings")]
    public int rtSize = 512;
    public float splatRadius = 0.05f;
    public float splatSoftness = 0.02f;
    public float splatIntensity = 1f;
    public float decay = 0.97f;

    private RenderTexture rippleRT;
    private RenderTexture tempRT;

    private Camera cam;

    void Start()
    {
        cam = Camera.main;

        rippleRT = new RenderTexture(rtSize, rtSize, 0, RenderTextureFormat.ARGB32);
        rippleRT.Create();
        tempRT = new RenderTexture(rtSize, rtSize, 0, RenderTextureFormat.ARGB32);
        tempRT.Create();

        waterMaterial.SetTexture("_RippleTex", rippleRT);
        decayMaterial.SetFloat("_Decay", decay);
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mouse = Input.mousePosition;
            Vector2 uv = new Vector2(mouse.x / Screen.width, mouse.y / Screen.height);
            PaintAt(uv);
        }

        // atenuar cada frame
        Graphics.Blit(rippleRT, tempRT, decayMaterial);
        Graphics.Blit(tempRT, rippleRT);
    }

    void PaintAt(Vector2 uv)
    {
        splatMaterial.SetVector("_Center", new Vector4(uv.x, uv.y, 0, 0));
        splatMaterial.SetFloat("_Radius", splatRadius);
        splatMaterial.SetFloat("_Softness", splatSoftness);
        splatMaterial.SetFloat("_Intensity", splatIntensity);

        Graphics.Blit(rippleRT, tempRT);
        Graphics.Blit(tempRT, rippleRT, splatMaterial);
    }
}
