Shader "PaulMattern/DitheredShadowsShader"
{
    Properties
    {
        _WorldTex ("World Texture", 2D) = "white" {}
        _LightTex ("Light Texture", 2D) = "white" {}
        _ColorPaletteTex ("Color Palette Texture", 2D) = "white" {}

        _DistortionSpeed ("Distortion Speed", Range(0, 5)) = 1
        _DistortionAmplitude ("Distortion Amplitude", Range(0, 5)) = 1
        _GradientModifier ("Gradient Modifier", Range(0, 2)) = 0.2
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

            TEXTURE2D(_ColorPaletteTex);
            SAMPLER(sampler_ColorPaletteTex);

            int _InternalResolutionWidth;
            int _InternalResolutionHeight;

            float _DistortionSpeed;
            float _DistortionAmplitude;

            float _GradientModifier;

            static const float bayer4x4[16] = {
                0.0,    0.5,    0.125,  0.625,
                0.75,   0.25,   0.875,  0.375,
                0.1875, 0.6875, 0.0625, 0.5625,
                0.9375, 0.4375, 0.8125, 0.3125
            };

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
                float2 internal_resolution = float2(_InternalResolutionWidth, _InternalResolutionHeight);
                float2 pixel_position = floor(i.uv * internal_resolution);
                float2 distortion = round(float2(sin(pixel_position.x * _Time.y * 0.01 * _DistortionSpeed), sin(pixel_position.y * _Time.y * 0.013 * _DistortionSpeed)) * _DistortionAmplitude);
                float2 distorted_uv = (pixel_position + distortion) / internal_resolution;
                float4 world_color = SAMPLE_TEXTURE2D(_WorldTex, sampler_WorldTex, i.uv);
                float light_color = pow(SAMPLE_TEXTURE2D(_LightTex, sampler_LightTex, distorted_uv), _GradientModifier);
                float dither_shade = bayer4x4[int(pixel_position.x % 4) + int(pixel_position.y % 4) * 4];
                float4 color = (dither_shade < light_color) ?
                    SAMPLE_TEXTURE2D(_ColorPaletteTex, sampler_ColorPaletteTex, float2(world_color.r, 0.25)) : 
                    SAMPLE_TEXTURE2D(_ColorPaletteTex, sampler_ColorPaletteTex, float2(world_color.b, 0.75));
                return color;
            }
            ENDHLSL
        }
    }
}
