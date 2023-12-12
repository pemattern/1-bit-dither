using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class DispatchDitheredShadowsShader : MonoBehaviour
{
    [SerializeField] Vector2Int _internalResolution;
    [SerializeField] int _pixelsPerUnit;
    [SerializeField] Material _mat;
    [SerializeField] Camera _worldCamera, _lightCamera;
    RawImage _rawImage;
    RenderTexture _worldTex, _lightTex, _combinationTex;
    
    void Awake()
    {
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

        _worldCamera.targetTexture = _worldTex;
        _lightCamera.targetTexture = _lightTex;

        float orthographicSize = _internalResolution.y / (float)_pixelsPerUnit / 2f;
        _worldCamera.orthographicSize = orthographicSize;
        _lightCamera.orthographicSize = orthographicSize;
        
        _rawImage.texture = _combinationTex;
        _mat.SetTexture("_WorldTex", _worldTex);
        _mat.SetTexture("_LightTex", _lightTex);

        _mat.SetInteger("_InternalResolutionWidth", _internalResolution.x);
        _mat.SetInteger("_InternalResolutionHeight", _internalResolution.y);

        RenderPipelineManager.endContextRendering += OnEndContextRendering;
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
