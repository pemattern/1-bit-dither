using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class LightProber : MonoBehaviour
{
    private static LightProber _instance;
    private Camera _lightCamera;
    private Texture2D _lightTexture2D;
    private int _width, _height;
    void Awake()
    {
        _instance = this;
    }

    void Start()
    {
        _lightCamera = GetComponent<Camera>();
        _width = _lightCamera.targetTexture.width;
        _height = _lightCamera.targetTexture.height;
        UpdateLightTexture();
    }

    public static void UpdateLightTexture()
    {
        RenderTexture.active = _instance._lightCamera.targetTexture;
        _instance._lightTexture2D = new Texture2D(
            _instance._width,
            _instance._height,
            UnityEngine.Experimental.Rendering.DefaultFormat.LDR, 
            UnityEngine.Experimental.Rendering.TextureCreationFlags.None);
        _instance._lightTexture2D.ReadPixels(new Rect(0, 0, _instance._width, _instance._height), 0, 0);
        _instance._lightTexture2D.Apply();
        RenderTexture.active = null;
    }

    public static float GetIntensity(Vector3 position)
    {
        Vector3 uv = _instance._lightCamera.WorldToViewportPoint(position);

        int texX = Mathf.RoundToInt(uv.x * _instance._width);
        int texY = Mathf.RoundToInt(uv.y * _instance._height);

        Color color = _instance._lightTexture2D.GetPixel(texX, texY);
        return color.grayscale;
    }
}
