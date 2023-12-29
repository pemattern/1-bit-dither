Shader "PaulMattern/DitheredShadows"
{
    Properties
    {
        _WorldTex ("World Texture", 2D) = "white" {}
        _LightTex ("Light Texture", 2D) = "white" {}
        _ColorPaletteTex ("Color Palette Texture", 2D) = "white" {}
        _OverlayTex ("Overlay Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" "RenderPipeline"="UniversalPipeline" }
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

            TEXTURE2D(_OverlayTex);
            SAMPLER(sampler_OverlayTex);

            int _DitheringPattern;

            float _DistortionSpeed;
            float _DistortionAmplitude;
            float _GradientModifier;

            float4 _WorldTex_TexelSize;
            
            static const float BAYER_2x2[2][2] = {
                { 0.0,  0.5  },
                { 0.75, 0.25 }
            };

            static const float BAYER_4x4[4][4] = {
                { 0.0,    0.5,    0.125,  0.625  },
                { 0.75,   0.25,   0.875,  0.375  },
                { 0.1875, 0.6875, 0.0625, 0.5625 },
                { 0.9375, 0.4375, 0.8125, 0.3125 },
            };

            static const float BAYER_8x8[8][8] = {
                { 0.0,      0.5,      0.125,    0.625,    0.03125,  0.53125,  0.15625,  0.65625  },
                { 0.75,     0.25,     0.875,    0.375,    0.78125,  0.28125,  0.90625,  0.40625  },
                { 0.1875,   0.6875,   0.0625,   0.5625,   0.21875,  0.71875,  0.09375,  0.59375  },
                { 0.9375,   0.4375,   0.96875,  0.46875,  0.8125,   0.3125,   0.84375,  0.34375  },
                { 0.046875, 0.546875, 0.171875, 0.671875, 0.015625, 0.515625, 0.171875, 0.671875 },
                { 0.796875, 0.296875, 0.921875, 0.421875, 0.78125,  0.28125,  0.90625,  0.40625  },
                { 0.234375, 0.734375, 0.109375, 0.609375, 0.203125, 0.703125, 0.078125, 0.578125 },
                { 0.984375, 0.484375, 0.953125, 0.453125, 0.828125, 0.328125, 0.859375, 0.359375 }
            };

            static const float SOBEL_X[3][3] = {
                { 1.0, 0.0, -1.0 },
                { 2.0, 0.0, -2.0 },
                { 1.0, 0.0, -1.0 },
            };

            static const float SOBEL_Y[3][3] = {
                { 1.0,  2.0,  1.0 },
                { 0.0,  0.0,  0.0 },
                { -1.0, -2.0, -1.0 },
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

            float GetSobelAt(float2 pixel_position)
            {
                float4 sum_x = 0, sum_y = 0;
                for (int x = -1; x <= 1; x++)
                {
                    for (int y = -1; y <= 1; y++)
                    {
                        float2 uv = (pixel_position + float2(x, y)) / _WorldTex_TexelSize.zw;
                        sum_x += SAMPLE_TEXTURE2D(_LightTex, sampler_LightTex, uv) * SOBEL_X[x + 1][y + 1];
                        sum_y += SAMPLE_TEXTURE2D(_LightTex, sampler_LightTex, uv) * SOBEL_Y[x + 1][y + 1];
                    }
                }

                float result = sqrt(sum_x * sum_x + sum_y * sum_y);
                return 1 - result;
            }

            float4 Frag(VertexOutput i) : SV_Target
            {
                float4 overlay_color = SAMPLE_TEXTURE2D(_OverlayTex, sampler_OverlayTex, i.uv);
                if (overlay_color.a > 0)
                    return overlay_color;

                float2 pixel_position = floor(i.uv * _WorldTex_TexelSize.zw);
                float light_color = pow(SAMPLE_TEXTURE2D(_LightTex, sampler_LightTex, i.uv), _GradientModifier);
                float sobel = GetSobelAt(pixel_position);
                float2 distortion = sobel < 0.2 ? float2(0, 0) : float2(sin(pixel_position.x * _Time.y * 0.01 * _DistortionSpeed), sin(pixel_position.y * _Time.y * 0.013 * _DistortionSpeed)) * _DistortionAmplitude;
                float2 distorted_uv = (pixel_position + distortion) / _WorldTex_TexelSize.zw;
                float4 world_color = SAMPLE_TEXTURE2D(_WorldTex, sampler_WorldTex, i.uv);
                light_color = pow(SAMPLE_TEXTURE2D(_LightTex, sampler_LightTex, distorted_uv), _GradientModifier);
                
                float dither_shade;
                if (_DitheringPattern == 0)
                    dither_shade = BAYER_2x2[int(pixel_position.x % 2)][int(pixel_position.y % 2)];
                else if (_DitheringPattern == 1)
                    dither_shade = BAYER_4x4[int(pixel_position.x % 4)][int(pixel_position.y % 4)];
                else if (_DitheringPattern == 2)
                    dither_shade = BAYER_8x8[int(pixel_position.x % 8)][int(pixel_position.y % 8)];   

                float4 color = (dither_shade < light_color) ?
                    world_color : 
                    SAMPLE_TEXTURE2D(_ColorPaletteTex, sampler_ColorPaletteTex, float2(dot(world_color.rgb, float3(0.2126, 0.7152, 0.0722)), 0.5));
                return color;
            }
            ENDHLSL
        }
    }
}
