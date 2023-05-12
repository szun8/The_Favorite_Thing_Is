Shader "Custom/WavySpriteLitURP"
{
    Properties
    {
        [MainTexture] _BaseMap("Albedo", 2D) = "white" {}
        [MainColor] _BaseColor("Color", Color) = (1,1,1,1)
        [HideInInspector] _RendererColor("RendererColor",Color) = (1,1,1,1)
        _WaveDirection("Wave Direction",range(0,1)) = 0
        _StaticSide("Static Side",range(0,4)) = 3
        _WaveFrequency("Wave Frequency",float) = 10
        _WaveForce("Wave Force",float) = 0.1
        _WaveSpeed("Wave Speed",float) = 1
        _TextureSpeed("Texture Speed",float) = 0
    }

    SubShader
    {
        Tags
        {
            "RenderPipeline" = "UniversalRenderPipeline"
            "RenderType" = "Opaque"
            "IgnoreProjector" = "True"
        }

        Pass
        {
            Tags
            {
                "LightMode" = "UniversalForward"
            }

            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            Cull Off

            HLSLPROGRAM
            // Required to compile gles 2.0 with standard SRP library
            #pragma prefer_hlslcc gles
            #pragma exclude_renderers d3d11_9x
            #pragma target 2.0

            // Material Keywords
            #pragma shader_feature _NORMALMAP
            #pragma shader_feature _ALPHATEST_ON
            #pragma shader_feature _ALPHAPREMULTIPLY_ON
            #pragma shader_feature _EMISSION
            #pragma shader_feature _METALLICSPECGLOSSMAP
            #pragma shader_feature _SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A
            #pragma shader_feature _OCCLUSIONMAP

            #pragma shader_feature _SPECULARHIGHLIGHTS_OFF
            #pragma shader_feature _GLOSSYREFLECTIONS_OFF
            #pragma shader_feature _SPECULAR_SETUP
            #pragma shader_feature _RECEIVE_SHADOWS_OFF

            // Universal Render Pipeline keywords
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS_CASCADE
            #pragma multi_compile _ _ADDITIONAL_LIGHTS_VERTEX _ADDITIONAL_LIGHTS
            #pragma multi_compile _ _ADDITIONAL_LIGHT_SHADOWS
            #pragma multi_compile _ _SHADOWS_SOFT
            #pragma multi_compile _ _MIXED_LIGHTING_SUBTRACTIVE

            // Unity defined keywords
            #pragma multi_compile _ DIRLIGHTMAP_COMBINED
            #pragma multi_compile _ LIGHTMAP_ON
            #pragma multi_compile_fog

            // GPU Instancing
            #pragma multi_compile_instancing 

            #pragma vertex LitPassVertex
            #pragma fragment LitPassFragment

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/Shaders/LitInput.hlsl"

            struct Attributes
            {
                float4 positionOS   : POSITION;
                float3 normalOS     : NORMAL;
                float4 tangentOS    : TANGENT;
                float2 uv           : TEXCOORD0;
                float2 uvLM         : TEXCOORD1;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct Varyings
            {
                float2 uv                       : TEXCOORD0;
                float2 uvLM                     : TEXCOORD1;
                float4 positionWSAndFogFactor   : TEXCOORD2; // xyz: positionWS, w: vertex fog factor
                half3  normalWS                 : TEXCOORD3;
                float4 positionCS               : SV_POSITION;
            };

            float _WaveDirection;
            float _StaticSide;
            float _WaveFrequency;
            float _WaveForce;
            float _WaveSpeed;
            float _TextureSpeed;

            Varyings LitPassVertex(Attributes input)
            {
                Varyings output;
                //Decide a static side
                float multiplier = 0;
                if (_StaticSide == 1) multiplier = 1 - input.uv.y; //Top
                if (_StaticSide == 2) multiplier = 1 - input.uv.x; //Right
                if (_StaticSide == 3) multiplier = input.uv.y; //Bottom
                if (_StaticSide == 4) multiplier = input.uv.x; //Left
                if (_StaticSide == 5) multiplier = (0.5 - abs(input.uv.y - 0.5)) * 2; //Top and bottom
                if (_StaticSide == 6) multiplier = (0.5 - abs(input.uv.x - 0.5)) * 2; //Left and right
                if (_StaticSide == 0) multiplier = 1; //None
                //Based on wave direction decide a vector for oscilations and axis of movement
                float3 osc;
                float side;
                if (_WaveDirection == 0) {
                    osc = float3(1, 0, 0);
                    side = input.positionOS.y;
                }
                else {
                    osc = float3(0, 1, 0);
                    side = input.positionOS.x;
                }
                //Multiply by wave force
                osc *= _WaveForce;
                //Increment it with our sine waves
                input.positionOS.xyz += osc * multiplier * sin(side * _WaveFrequency - _Time.a * _WaveSpeed);

                //Default shader code
                VertexPositionInputs vertexInput = GetVertexPositionInputs(input.positionOS.xyz);
                VertexNormalInputs vertexNormalInput = GetVertexNormalInputs(input.normalOS, input.tangentOS);
                float fogFactor = ComputeFogFactor(vertexInput.positionCS.z);
                output.uv = TRANSFORM_TEX(input.uv, _BaseMap);
                output.uvLM = input.uvLM.xy * unity_LightmapST.xy + unity_LightmapST.zw;
                output.positionWSAndFogFactor = float4(vertexInput.positionWS, fogFactor);
                output.normalWS = vertexNormalInput.normalWS;
                output.positionCS = vertexInput.positionCS;
                return output;
            }

            half4 LitPassFragment(Varyings input) : SV_Target
            {
                SurfaceData surfaceData;
                //Moving texture
                float2 incrementUV = float2(0, 0);
                //Horizontal waves
                if (_WaveDirection == 0) incrementUV.y -= (_Time.a * _TextureSpeed) * 0.1;
                //Vertical waves
                else incrementUV.x -= (_Time.a * _TextureSpeed) * 0.1;

                //Default shader code
                InitializeStandardLitSurfaceData(input.uv + incrementUV, surfaceData);
                half3 normalWS = input.normalWS;
                normalWS = normalize(normalWS);
                half3 bakedGI = SampleSH(normalWS);
                float3 positionWS = input.positionWSAndFogFactor.xyz;
                half3 viewDirectionWS = SafeNormalize(GetCameraPositionWS() - positionWS);
                BRDFData brdfData;
                InitializeBRDFData(surfaceData.albedo, surfaceData.metallic, surfaceData.specular, surfaceData.smoothness, surfaceData.alpha, brdfData);
                Light mainLight = GetMainLight();
                half3 color = GlobalIllumination(brdfData, bakedGI, surfaceData.occlusion, normalWS, viewDirectionWS);
                color *= _BaseColor.rgb;
                color += LightingPhysicallyBased(brdfData, mainLight, normalWS, viewDirectionWS);
                #ifdef _ADDITIONAL_LIGHTS
                int additionalLightsCount = GetAdditionalLightsCount();
                for (int i = 0; i < additionalLightsCount; ++i)
                {
                    Light light = GetAdditionalLight(i, positionWS);
                    color += LightingPhysicallyBased(brdfData, light, normalWS, viewDirectionWS);
                }
                #endif
                color += surfaceData.emission;
                float fogFactor = input.positionWSAndFogFactor.w;
                color = MixFog(color, fogFactor);
                return half4(color, surfaceData.alpha * _BaseColor.a);
            }
            ENDHLSL
        }
        UsePass "Universal Render Pipeline/Lit/ShadowCaster"
        UsePass "Universal Render Pipeline/Lit/DepthOnly"
        UsePass "Universal Render Pipeline/Lit/Meta"
    }
}