Shader "PaulMattern/DitheredShadowsShader"
{
    Properties
    {
        _BaseMap ("Base Texture", 2D) = "white" {}
        _LightTex ("Light Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" "RenderPipeline"="UniversialPipeline" }
        LOD 100
        ZWrite Off Cull Off

        Pass
        {
            Name "DitheredShadowsPass"

            HLSLPROGRAM
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            //#include "Packages/com.unity.render-pipelines.core/Runtime/Utilities/Blit.hlsl"
            
            #pragma vertex Vert
            #pragma fragment Frag

            TEXTURE2D(_BaseMap);
            SAMPLER(sampler_BaseMap);
            TEXTURE2D(_LightTex);
            SAMPLER(sampler_LightTex);

            //float4 _ScreenParams;

            float bayer2(float2 uv)
            {
                return frac(uv.x / 2.0 + uv.y * uv.y * 0.75);
            }
            float bayer4(float2 uv)
            {
                return bayer2(0.5 * uv) * 0.25 + bayer2(uv);
            }
            float bayer8(float2 uv)
            {
                return bayer4(0.5 * uv) * 0.25 + bayer2(uv);
            }
            float bayer16(float2 uv)
            {
                return bayer8(0.5 * uv) * 0.25 + bayer2(uv);
            }
            float bayer32(float2 uv)
            {
                return bayer16(0.5 * uv) * 0.25 + bayer2(uv);
            }
            float bayer64(float2 uv)
            {
                return bayer32(0.5 * uv) * 0.25 + bayer2(uv);
            }

            struct VertexInput
            {
                float4 position : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct VertexOutput
            {
                float4 position : SV_POSITION;
                float2 uv : TEXCOORD0;    
            };

            VertexOutput Vert(VertexInput i)
            {
                VertexOutput o;
                o.position = TransformObjectToHClip(i.position.xyz);
                o.uv = i.uv;
                return o;
            }

            float4 Frag(VertexOutput i) : SV_Target
            {
                float2 screen_size = float2(320, 180);
                float2 pixel_position = floor(i.uv * screen_size);
                float2 distortion = round(float2(sin(pixel_position.x * _Time.y * 0.01), sin(pixel_position.y * _Time.y * 0.013)));
                float2 distorted_uv = (pixel_position + distortion) / screen_size;
                float4 main_color = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, i.uv);
                float4 light_color = SAMPLE_TEXTURE2D(_LightTex, sampler_LightTex, distorted_uv);
                
                float4 color = (bayer64(pixel_position) < light_color) ? main_color : float4(0, 0, 0, 1);
                return color;
            }
            ENDHLSL
        }
    }
}
