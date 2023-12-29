Shader "PaulMattern/PixelOutline"
{
    Properties
    {
        _MainTex ("Sprite Texture", 2D) = "white" {}
        _OutlineColor ("Outline Color", Color) = (1, 1, 1, 1)
        _OutlineColor2 ("Outline Color 2", Color) = (0.75, 0.75, 0.75, 1)
        [HideInInspector] _DisplayOutline ("Display Outline", Float) = 0
        [HideInInspector] _IncludeDiagonals ("Include Diagonals", Float) = 1
    }
    SubShader
    {
        Tags { "Queue"= "Transparent" "RenderType"="Transparent" "RenderPipeline"="UniversalPipeline" }
        LOD 100
        ZWrite Off
        Cull Off
        Pass
        {
            Name "PixelOutlinePass"
            Tags { "LightMode"="Universal2D" }
            Blend SrcAlpha OneMinusSrcAlpha

            HLSLPROGRAM
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            
            #pragma vertex Vert
            #pragma fragment Frag

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);

            float4 _MainTex_TexelSize;
            float4 _OutlineColor;
            float4 _OutlineColor2;
            float _DisplayOutline;
            float _IncludeDiagonals;

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
                float4 base_color = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv);

                float4 outline = float4(0, 0, 0, 0);
                if (_DisplayOutline == 1.0)
                {
                    outline += SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, saturate(float2(i.uv.x + _MainTex_TexelSize.x, i.uv.y)));
                    outline += SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, saturate(float2(i.uv.x - _MainTex_TexelSize.x, i.uv.y)));
                    outline += SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, saturate(float2(i.uv.x, i.uv.y + _MainTex_TexelSize.y)));
                    outline += SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, saturate(float2(i.uv.x, i.uv.y - _MainTex_TexelSize.y)));
                    if (_IncludeDiagonals == 1.0)
                    {
                        outline += SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, saturate(i.uv + _MainTex_TexelSize.xy));
                        outline += SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, saturate(i.uv - _MainTex_TexelSize.xy));
                        outline += SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, saturate(i.uv + float2(_MainTex_TexelSize.x, -_MainTex_TexelSize.y)));
                        outline += SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, saturate(i.uv + float2(-_MainTex_TexelSize.x, _MainTex_TexelSize.y)));
                    }
                    outline = step(0.001, outline);
                    outline *= _OutlineColor;
                    float2 pixel_position = i.uv * _MainTex_TexelSize.zw;
                    float checker_board = (pixel_position.x + pixel_position.y) % 2;
                    float ticker = round(frac(_Time.y));
                    outline *= checker_board == ticker ? _OutlineColor : _OutlineColor2;
                }

                float4 color = (base_color.a > 0) ? float4(0, 0, 0, 0) : outline;
                return color;
            }
            ENDHLSL
        }
    }
}