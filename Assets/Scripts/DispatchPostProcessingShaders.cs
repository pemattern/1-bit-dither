using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

[RequireComponent(typeof(RawImage))]
public class DispatchPostProcessingShaders : MonoBehaviour
{
    [SerializeField] Vector2Int _internalResolution;
    [SerializeField] int _lightTextureResolutionFactor;
    [SerializeField] int _pixelsPerUnit;
    [SerializeField] Material _mat;
    [SerializeField] Camera _worldCamera, _lightCamera;
    [SerializeField] DitheringPattern _ditheringPattern;
    [SerializeField] float _distortionSpeed, _distortionAmplitude, _gradientModifier;
    [SerializeField] Color[] _shadowColors;
    RawImage _rawImage;
    RenderTexture _worldTex, _lightTex, _combinationTex;
    Texture2D _colorPaletteTex;
    
    void OnEnable()
    {
        RenderPipelineManager.endContextRendering += OnEndContextRendering;
    }

    void Start() {
        _rawImage = GetComponent<RawImage>();
        if (_rawImage != null) Setup();
    }

    void OnValidate()
    {
        if (_rawImage != null) Setup();
    }

    void Setup()
    {
        _combinationTex = new RenderTexture(_internalResolution.x, _internalResolution.y, 32, RenderTextureFormat.ARGB32)
        {
            filterMode = FilterMode.Point
        };
        _worldTex = new RenderTexture(_combinationTex)
        {
            filterMode = FilterMode.Point
        };
        _lightTex = new RenderTexture(_internalResolution.x * _lightTextureResolutionFactor, _internalResolution.y * _lightTextureResolutionFactor, 32, RenderTextureFormat.ARGB32)
        {
            filterMode = FilterMode.Point
        };

        _colorPaletteTex = GenerateColorPaletteTex();

        _worldCamera.targetTexture = _worldTex;
        _lightCamera.targetTexture = _lightTex;
        _rawImage.texture = _combinationTex;

        float orthographicSize = _internalResolution.y / (float)_pixelsPerUnit / 2f;
        _worldCamera.orthographicSize = orthographicSize;
        _lightCamera.orthographicSize = orthographicSize;

        _mat.SetTexture("_WorldTex", _worldTex);
        _mat.SetTexture("_LightTex", _lightTex);
        _mat.SetTexture("_ColorPaletteTex", _colorPaletteTex);

        _mat.SetFloat("_DistortionSpeed", _distortionSpeed);
        _mat.SetFloat("_DistortionAmplitude", _distortionAmplitude);
        _mat.SetFloat("_GradientModifier", _gradientModifier); 
        _mat.SetInteger("_DitheringPattern", (int)_ditheringPattern);
    }

    Texture2D GenerateColorPaletteTex()
    {
        Texture2D tex = new Texture2D(_shadowColors.Length, 1)
        {
            filterMode = FilterMode.Point,
            wrapMode = TextureWrapMode.Clamp
        };
        for (int x = 0; x < _shadowColors.Length; x++)
        {
            tex.SetPixel(x, 0, _shadowColors[x]);
        }
        tex.Apply();
        return tex;
    }

    void OnEndContextRendering(ScriptableRenderContext context, List<Camera> cameras)
    {
        CommandBufferPool.Get().Blit(null, _combinationTex, _mat, 0);
    }

    void OnDisable()
    {
        RenderPipelineManager.endContextRendering -= OnEndContextRendering;
    }

    enum DitheringPattern
    {
        Bayer2x2,
        Bayer4x4,
        Bayer8x8
    }
}