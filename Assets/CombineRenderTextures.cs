using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

[ExecuteInEditMode]
public class CombineRenderTextures : MonoBehaviour
{
    [SerializeField] Material _mat;
    RawImage _rawImage;
    RenderTexture _dest;
    void Start()
    {
        _rawImage = GetComponent<RawImage>();
        _dest = (RenderTexture) _rawImage.texture;
        RenderPipelineManager.endContextRendering += OnEndContextRendering;
    }

    void OnEndContextRendering(ScriptableRenderContext context, List<Camera> cameras)
    {
        Graphics.Blit(_dest, _mat, 0);
    }

    void OnDestroy()
    {
        RenderPipelineManager.endContextRendering -= OnEndContextRendering;
    }
}
