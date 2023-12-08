using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class DitheredShadows : ScriptableRendererFeature
{
    DitheredShadowsRenderPass _ditheredShadowsRenderPass;
    [SerializeField] Material _ditheredShadowsMaterial;
    [SerializeField] RenderPassEvent _renderPassEvent;
    class DitheredShadowsRenderPass : ScriptableRenderPass
    {
        private RTHandle _cameraColorTarget, _cameraColorTemp;
        private readonly int _baseMapID = Shader.PropertyToID("_BaseMap");
        private Material _ditheredShadowsMaterial;
        public DitheredShadowsRenderPass(RenderPassEvent renderPassEvent, Material ditheredShadowsMaterial)
        {
            this.renderPassEvent = renderPassEvent;
            _ditheredShadowsMaterial = ditheredShadowsMaterial;
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            CommandBuffer cmd = CommandBufferPool.Get();

            using (new ProfilingScope(cmd, new ProfilingSampler("Dithered Shadows")))
            {
                _ditheredShadowsMaterial.SetTexture("_BaseMap", _cameraColorTarget);
                Blitter.BlitCameraTexture(cmd, _cameraColorTarget, _cameraColorTemp, _ditheredShadowsMaterial, 0);
                Blitter.BlitCameraTexture(cmd, _cameraColorTemp, _cameraColorTarget);
            }
            context.ExecuteCommandBuffer(cmd);
            cmd.Clear();
            CommandBufferPool.Release(cmd);
        }

        public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
        {
            _cameraColorTarget = renderingData.cameraData.renderer.cameraColorTargetHandle;

            var colorCopy = renderingData.cameraData.cameraTargetDescriptor;
            colorCopy.depthBufferBits = 0;
            RenderingUtils.ReAllocateIfNeeded(ref _cameraColorTemp, colorCopy, name: "_TempTex");

            ConfigureTarget(_cameraColorTarget);
        }
    }

    public override void Create()
    {
        _ditheredShadowsRenderPass = new DitheredShadowsRenderPass(_renderPassEvent, _ditheredShadowsMaterial);
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        renderer.EnqueuePass(_ditheredShadowsRenderPass);
    }

    public override void SetupRenderPasses(ScriptableRenderer renderer, in RenderingData renderingData)
    {
        _ditheredShadowsRenderPass.ConfigureInput(ScriptableRenderPassInput.Color);
    }
}


