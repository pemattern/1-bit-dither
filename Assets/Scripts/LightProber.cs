using UnityEngine;

[RequireComponent(typeof(Camera))]
public class LightProber : MonoBehaviour
{
    public static LightProber Instance;
    Camera _lightCamera;
    public Texture2D _lightTexture2D;
    void Awake()
    {
        Instance = this;
    }
    void Start()
    {
        _lightCamera = GetComponent<Camera>();
    }

    public void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
            Debug.Log(GetIntensity(new Vector3(0, 0, 0)));
    }

    public float GetIntensity(Vector3 position)
    {
        int width = _lightCamera.targetTexture.width;
        int height = _lightCamera.targetTexture.height;

        RenderTexture.active = _lightCamera.targetTexture;
        _lightTexture2D = new Texture2D(width,
            height,
            UnityEngine.Experimental.Rendering.DefaultFormat.LDR, 
            UnityEngine.Experimental.Rendering.TextureCreationFlags.None);
        _lightTexture2D.ReadPixels(new Rect(0, 0, width, height), 0, 0);
        _lightTexture2D.Apply();
        RenderTexture.active = null;

        Vector3 uv = _lightCamera.WorldToViewportPoint(position);

        int texX = Mathf.RoundToInt(uv.x * width);
        int texY = Mathf.RoundToInt(uv.y * height);

        Color color = _lightTexture2D.GetPixel(texX, texY);
        return color.grayscale;
    }
}
