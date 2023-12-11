Shader "PaulMattern/DitheredShadowsShader"
{
    Properties
    {
        _WorldTex ("World Texture", 2D) = "white" {}
        _LightTex ("Light Texture", 2D) = "white" {}
        _BlueNoiseTex ("Blue Noise Texture", 2D) = "white" {}
        _FogOfWarTex ("Fog of War Texture", 2D) = "white" {}
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
            
            #pragma vertex Vert
            #pragma fragment Frag

            TEXTURE2D(_WorldTex);
            SAMPLER(sampler_WorldTex);

            TEXTURE2D(_LightTex);
            SAMPLER(sampler_LightTex);

            TEXTURE2D(_BlueNoiseTex);
            SAMPLER(sampler_BlueNoiseTex);

            TEXTURE2D(_FogOfWarTex);
            SAMPLER(sampler_FogOfWarTex);

            float2 _PlayerPosition;

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
                float2 player_offset = frac(_PlayerPosition);
                float2 pixel_position = floor(i.uv * screen_size);
                float2 distortion = round(float2(sin(pixel_position.x * _Time.y * 0.01), sin(pixel_position.y * _Time.y * 0.013)));
                float2 distorted_uv = (pixel_position + distortion) / screen_size;
                float4 world_color = SAMPLE_TEXTURE2D(_WorldTex, sampler_WorldTex, i.uv);
                float4 light_color = SAMPLE_TEXTURE2D(_LightTex, sampler_LightTex, distorted_uv);
                float4 fog_color = SAMPLE_TEXTURE2D(_FogOfWarTex, sampler_FogOfWarTex, i.uv); // how to sample this?
                //float4 noise_color = SAMPLE_TEXTURE2D(_BlueNoiseTex, sampler_BlueNoiseTex, float2(pixel_position.x / 256, pixel_position.y / 256));
                
                //float4 color = (noise_color < light_color) ? main_color : main_color * float4(0.03, 0.03, 0.1, 1);
                float4 dark_color = (bayer64(pixel_position) < fog_color) ? world_color.b : 0.0;
                float4 color = (bayer64(pixel_position) < light_color) ? world_color.r : dark_color;
                return color;
            }
            ENDHLSL
        }
    }
}
