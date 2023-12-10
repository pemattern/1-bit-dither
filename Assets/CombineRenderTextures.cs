using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class CombineRenderTextures : MonoBehaviour
{
    [SerializeField] Material _mat;
    [SerializeField] Camera _baseCamera, _lightCamera;
    RawImage _rawImage;
    RenderTexture _baseTex, _lightTex, _combinationTex;
    void Start()
    {
        _rawImage = GetComponent<RawImage>();

        _combinationTex = new RenderTexture(Screen.width, Screen.height, 32, RenderTextureFormat.ARGB32);
        _baseTex = new RenderTexture(_combinationTex);
        _lightTex = new RenderTexture(_combinationTex);

        _baseCamera.targetTexture = _baseTex;
        _lightCamera.targetTexture = _lightTex;
        
        _rawImage.texture = _combinationTex;
        _mat.SetTexture("_BaseMap", _baseTex);
        _mat.SetTexture("_LightTex", _lightTex);
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
