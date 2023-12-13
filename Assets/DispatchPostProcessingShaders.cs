using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class DispatchPostProcessingShaders : MonoBehaviour
{
    const int SHADES_COUNT = 4;
    [SerializeField] Vector2Int _internalResolution;
    [SerializeField] int _pixelsPerUnit;
    [SerializeField] Material _mat;
    [SerializeField] Camera _worldCamera, _lightCamera;
    [SerializeField] Color[] _lightColors;
    [SerializeField] Color[] _darkColors;
    RawImage _rawImage;
    RenderTexture _worldTex, _lightTex, _combinationTex;
    Texture2D _colorPaletteTex;
    
    void Awake()
    {
        if (_lightColors.Length != SHADES_COUNT || _darkColors.Length != SHADES_COUNT)
            throw new System.Exception("There must be exactly 4 light/dark colors.");

        _rawImage = GetComponent<RawImage>();

        _combinationTex = new RenderTexture(_internalResolution.x, _internalResolution.y, 32, RenderTextureFormat.ARGB32)
        {
            filterMode = FilterMode.Point
        };
        _worldTex = new RenderTexture(_combinationTex)
        {
            filterMode = FilterMode.Point
        };
        _lightTex = new RenderTexture(_combinationTex)
        {
            filterMode = FilterMode.Point
        };

        _colorPaletteTex = GenerateColorPaletteTex();

        _worldCamera.targetTexture = _worldTex;
        _lightCamera.targetTexture = _lightTex;

        float orthographicSize = _internalResolution.y / (float)_pixelsPerUnit / 2f;
        _worldCamera.orthographicSize = orthographicSize;
        _lightCamera.orthographicSize = orthographicSize;
        
        _rawImage.texture = _combinationTex;
        _mat.SetTexture("_WorldTex", _worldTex);
        _mat.SetTexture("_LightTex", _lightTex);
        _mat.SetTexture("_ColorPaletteTex", _colorPaletteTex);

        _mat.SetInteger("_InternalResolutionWidth", _internalResolution.x);
        _mat.SetInteger("_InternalResolutionHeight", _internalResolution.y);

        RenderPipelineManager.endContextRendering += OnEndContextRendering;
    }

    Texture2D GenerateColorPaletteTex()
    {
        Texture2D tex = new Texture2D(SHADES_COUNT, 2)
        {
            filterMode = FilterMode.Point,
            wrapMode = TextureWrapMode.Clamp
        };
        for (int x = 0; x < SHADES_COUNT; x++)
        {
            for (int y = 0; y < 2; y++)
            {
                Color color = (y == 0) ? _lightColors[x] : _darkColors[x];
                tex.SetPixel(x, y, color);
            }
        }
        tex.Apply();
        return tex;
    }

    void OnEndContextRendering(ScriptableRenderContext context, List<Camera> cameras)
    {
        Graphics.Blit(_combinationTex, _mat, 0);
    }

    void OnDestroy()
    {
        RenderPipelineManager.endContextRendering -= OnEndContextRendering;
    }
}
