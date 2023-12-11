using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class CombineRenderTextures : MonoBehaviour
{
    const int FogOfWarTexWidth = 21, FogOfWarTexHeight = 12;
    [SerializeField] Material _mat;
    [SerializeField] Camera _baseCamera, _lightCamera;
    [SerializeField] Transform _playerTransform;
    RawImage _rawImage;
    RenderTexture _worldTex, _lightTex, _combinationTex;
    [SerializeField] Texture2D _fogOfWarTex;
    HashSet<Vector2> _visited;
    void Start()
    {
        _rawImage = GetComponent<RawImage>();

        _combinationTex = new RenderTexture(Screen.width, Screen.height, 32, RenderTextureFormat.ARGB32);
        _worldTex = new RenderTexture(_combinationTex);
        _lightTex = new RenderTexture(_combinationTex);

        _fogOfWarTex = new Texture2D(FogOfWarTexWidth, FogOfWarTexHeight)
        {
            filterMode = FilterMode.Trilinear
        };
        FillFogOfWarTex();
        _fogOfWarTex.Apply();
        
        _baseCamera.targetTexture = _worldTex;
        _lightCamera.targetTexture = _lightTex;
        
        _rawImage.texture = _combinationTex;
        _mat.SetTexture("_WorldTex", _worldTex);
        _mat.SetTexture("_LightTex", _lightTex);
        _mat.SetTexture("_FogOfWarTex", _fogOfWarTex);

        _visited = new HashSet<Vector2>();

        RenderPipelineManager.endContextRendering += OnEndContextRendering;
        PlayerController.onMove += UpdateFogOfWarTexture;
    }

    void FillFogOfWarTex()
    {
        _fogOfWarTex.SetPixels32(Enumerable.Repeat(new Color32(0, 0, 0, 1), FogOfWarTexWidth * FogOfWarTexHeight).ToArray());
    }

    void UpdateFogOfWarTexture(Vector2 position)
    {
        _mat.SetFloatArray("_PlayerPosition", new [] { position.x, position.y });
        _visited.Add(new Vector2((int)position.x, (int)position.y));

        for (int x = 0; x < FogOfWarTexWidth; x++)
        {
            for (int y = 0; y < FogOfWarTexHeight; y++)
            {
                Vector2 screenSpacePosition = position + new Vector2(x, y) - new Vector2(11, 5);

                Debug.Log(_visited.Count);
                Color color = _visited.Contains(screenSpacePosition) ? Color.white : Color.black;
                _fogOfWarTex.SetPixel(x, y, color);
            }
        }
        _fogOfWarTex.Apply();
        _mat.SetTexture("_FogOfWarTex", _fogOfWarTex);
        
    }

    void OnEndContextRendering(ScriptableRenderContext context, List<Camera> cameras)
    {
        Graphics.Blit(_combinationTex, _mat, 0);
    }

    void OnDestroy()
    {
        RenderPipelineManager.endContextRendering -= OnEndContextRendering;
        PlayerController.onMove -= UpdateFogOfWarTexture;
    }
}
