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

        Pass
        {
            HLSLPROGRAM
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.core/Runtime/Utilities/Blit.hlsl"
            
            #pragma vertex Vert
            #pragma fragment Frag

            //TEXTURE2D_X(_BaseMap);
            //SAMPLER(sampler_BaseMap);
            //TEXTURE2D_X(_LightTex);
            //SAMPLER(sampler_LightTex);

            float4 Frag(Varyings i) : SV_Target
            {
                //float4 main_color = SAMPLE_TEXTURE2D_X(_BaseMap, sampler_BaseMap, i.texcoord);
                //float4 light_color = SAMPLE_TEXTURE2D_X(_LightTex, sampler_LightTex, i.texcoord);
                return float4(1, 0, 0, 1);
                //return main_color * light_color;
            }
            ENDHLSL
        }
    }
}
