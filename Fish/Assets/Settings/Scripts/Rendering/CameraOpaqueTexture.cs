using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class CameraOpaqueTexture : ScriptableRendererFeature
{
    public enum TexQuality
    {
        High = 0,
        Middle,
        Low,
    }

    private class CustomRenderPass : ScriptableRenderPass
    {
        private static readonly string PROFILE_TAG = "Screen Opaque Texture";

        private static readonly int SCREEN_OPAQUE_TEX_PROP_ID = Shader.PropertyToID("_G_ScreenOpaqueTexture");

        private readonly List<ShaderTagId> m_ShaderTagIdList = new List<ShaderTagId>();

        private readonly CameraOpaqueTexture m_Owner;

        private FilteringSettings m_FilteringSettings;

        public CustomRenderPass(CameraOpaqueTexture owner)
        {
            renderPassEvent = RenderPassEvent.BeforeRenderingTransparents;
            m_Owner = owner;

            m_FilteringSettings = new FilteringSettings();
            m_FilteringSettings.layerMask = -1;
            m_FilteringSettings.renderingLayerMask = 0xffffffff;
            m_FilteringSettings.sortingLayerRange = SortingLayerRange.all;

            m_ShaderTagIdList.Add(new ShaderTagId("UniversalForward"));
            m_ShaderTagIdList.Add(new ShaderTagId("UniversalForwardOnly"));
        }

        public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor)
        {
            int width = cameraTextureDescriptor.width;
            int height = cameraTextureDescriptor.height;

            RenderTextureDescriptor desc = cameraTextureDescriptor;
            desc.colorFormat = RenderTextureFormat.ARGB32;
            cmd.GetTemporaryRT(SCREEN_OPAQUE_TEX_PROP_ID, desc, FilterMode.Bilinear);
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            ref CameraData cameraData = ref renderingData.cameraData;
            ref ScriptableRenderer renderer = ref cameraData.renderer;
            Camera camera = cameraData.camera;
#if UNITY_2022_1_OR_NEWER
            RTHandle colorTarget = renderer.cameraColorTargetHandle;
            RTHandle depthTarget = renderer.cameraDepthTargetHandle;
#else
            RenderTargetIdentifier colorTarget = renderer.cameraColorTarget;
            RenderTargetIdentifier depthTarget = renderer.cameraDepthTarget;
#endif

            CommandBuffer cmd = CommandBufferPool.Get();
            {
                cmd.Clear();
                cmd.BeginSample(PROFILE_TAG);
                cmd.Blit(colorTarget, SCREEN_OPAQUE_TEX_PROP_ID);
                cmd.SetRenderTarget(SCREEN_OPAQUE_TEX_PROP_ID, depthTarget);
                context.ExecuteCommandBuffer(cmd);

                m_FilteringSettings.layerMask = m_Owner.m_CullMask;
                m_FilteringSettings.renderQueueRange = RenderQueueRange.transparent;
                DrawingSettings drawingTransparentSettings = CreateDrawingSettings(m_ShaderTagIdList, ref renderingData, SortingCriteria.CommonTransparent);
                context.DrawRenderers(renderingData.cullResults, ref drawingTransparentSettings, ref m_FilteringSettings);

                cmd.Clear();
                cmd.SetRenderTarget(colorTarget, depthTarget);
                cmd.EndSample(PROFILE_TAG);
                context.ExecuteCommandBuffer(cmd);
            }
            CommandBufferPool.Release(cmd);
        }

        public override void FrameCleanup(CommandBuffer cmd)
        {
            cmd.ReleaseTemporaryRT(SCREEN_OPAQUE_TEX_PROP_ID);
        }
    }

    [SerializeField]
    private LayerMask m_CullMask = -1;

    private CustomRenderPass m_ScriptablePass;

    public override void Create()
    {
        m_ScriptablePass = new CustomRenderPass(this);
    }

    public override void AddRenderPasses(UnityEngine.Rendering.Universal.ScriptableRenderer renderer, ref UnityEngine.Rendering.Universal.RenderingData renderingData)
    {
        if (renderingData.cameraData.cameraType != CameraType.Preview)
            renderer.EnqueuePass(m_ScriptablePass);
    }
}
