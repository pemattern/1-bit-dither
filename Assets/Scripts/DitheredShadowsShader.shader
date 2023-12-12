Shader "PaulMattern/DitheredShadowsShader"
{
    Properties
    {
        _WorldTex ("World Texture", 2D) = "white" {}
        _LightTex ("Light Texture", 2D) = "white" {}
        _BlueNoiseTex ("Blue Noise Texture", 2D) = "white" {}

        _LightColor1 ("Light Color 1", Color) = (1, 1, 1, 1)
        _LightColor2 ("Light Color 2", Color) = (1, 1, 1, 1)
        _LightColor3 ("Light Color 3", Color) = (1, 1, 1, 1)
        _LightColor4 ("Light Color 4", Color) = (1, 1, 1, 1)

        _DarkColor1 ("Dark Color 1", Color) = (0, 0, 0, 1)
        _DarkColor2 ("Dark Color 2", Color) = (0, 0, 0, 1)
        _DarkColor3 ("Dark Color 3", Color) = (0, 0, 0, 1)
        _DarkColor4 ("Dark Color 4", Color) = (0, 0, 0, 1)
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

            int _InternalResolutionWidth;
            int _InternalResolutionHeight;

            float4 _LightColor1;
            float4 _LightColor2;
            float4 _LightColor3;
            float4 _LightColor4;

            float4 _DarkColor1;
            float4 _DarkColor2;
            float4 _DarkColor3;
            float4 _DarkColor4;

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

            float4 GetLightColor(float shade)
            {
                if (shade > 0 && shade <= 0.25)
                    return _LightColor4;
                else if (shade > 0.25 && shade <= 0.5)
                    return _LightColor3;
                else if (shade > 0.5 && shade <= 0.75)
                    return _LightColor2;
                else if (shade > 0.75 && shade <= 1)
                    return _LightColor1;
                else
                    return _LightColor4;
            }

            float4 GetDarkColor(float shade)
            {
                if (distance(shade, 0.125) < 0.125)
                    return _DarkColor4;
                else if (distance(shade, 0.375) < 0.125)
                    return _DarkColor3;
                else if (distance(shade, 0.625) < 0.125)
                    return _DarkColor2;
                else if (distance(shade, 0.875) < 0.125)
                    return _DarkColor1;
                else
                    return _DarkColor4;
            }

            float4 Frag(VertexOutput i) : SV_Target
            {
                float2 internal_resolution = float2(_InternalResolutionWidth, _InternalResolutionHeight);
                float2 pixel_position = floor(i.uv * internal_resolution);
                float2 distortion = round(float2(sin(pixel_position.x * _Time.y * 0.01), sin(pixel_position.y * _Time.y * 0.013)));
                float2 distorted_uv = (pixel_position + distortion) / internal_resolution;
                float4 world_color = SAMPLE_TEXTURE2D(_WorldTex, sampler_WorldTex, i.uv);
                float4 light_color = SAMPLE_TEXTURE2D(_LightTex, sampler_LightTex, distorted_uv);
                //float4 noise_color = SAMPLE_TEXTURE2D(_BlueNoiseTex, sampler_BlueNoiseTex, float2(pixel_position.x / 256, pixel_position.y / 256));
                
                //float4 color = (noise_color < light_color) ? main_color : main_color * float4(0.03, 0.03, 0.1, 1);



                float4 color = (bayer4(pixel_position) < light_color) ? world_color.r : GetDarkColor(world_color.b);
                return color;
            }
            ENDHLSL
        }
    }
}
