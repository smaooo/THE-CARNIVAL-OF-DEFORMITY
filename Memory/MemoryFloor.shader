Shader "Blend"
{
    Properties
    {
        Color_822539e8f7124cf0b91b8404302e4eb4("Color", Color) = (0.764151, 0.764151, 0.764151, 0)
        Vector1_5d78a6ec65f3486fab89281302daa414("NoiseScale", Float) = 500
        Vector2_1bd490e57d694d988e0111c4c8f24079("MinMax", Vector) = (0, 5, 0, 0)
        Vector2_58ccd7d4e76b48b987ec44839b8492a3("SpiralScale", Vector) = (1, 1, 0, 0)
        Vector1_c26db16e47624f4bbb0694a0b08822ea("Opacity", Float) = 0.5
    }
    SubShader
    {
        Tags
        {
            // RenderPipeline: <None>
            "RenderType"="Opaque"
            "Queue"="Geometry"
        }
        Pass
        {
            // Name: <None>
            Tags
            {
                // LightMode: <None>
            }

            // Render State
            // RenderState: <None>

            // Debug
            // <None>

            // --------------------------------------------------
            // Pass

            HLSLPROGRAM

            // Pragmas
            #pragma vertex vert
        #pragma fragment frag

            // DotsInstancingOptions: <None>
            // HybridV1InjectedBuiltinProperties: <None>

            // Keywords
            // PassKeywords: <None>
            // GraphKeywords: <None>

            // Defines
            #define ATTRIBUTES_NEED_TEXCOORD0
            #define VARYINGS_NEED_TEXCOORD0
            /* WARNING: $splice Could not find named fragment 'PassInstancing' */
            #define SHADERPASS SHADERPASS_PREVIEW
        #define SHADERGRAPH_PREVIEW
            /* WARNING: $splice Could not find named fragment 'DotsInstancingVars' */

            // Includes
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Packing.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/NormalSurfaceGradient.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/UnityInstancing.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/EntityLighting.hlsl"
        #include "Packages/com.unity.shadergraph/ShaderGraphLibrary/ShaderVariables.hlsl"
        #include "Packages/com.unity.shadergraph/ShaderGraphLibrary/ShaderVariablesFunctions.hlsl"
        #include "Packages/com.unity.shadergraph/ShaderGraphLibrary/Functions.hlsl"

            // --------------------------------------------------
            // Structs and Packing

            struct Attributes
        {
            float3 positionOS : POSITION;
            float4 uv0 : TEXCOORD0;
            #if UNITY_ANY_INSTANCING_ENABLED
            uint instanceID : INSTANCEID_SEMANTIC;
            #endif
        };
        struct Varyings
        {
            float4 positionCS : SV_POSITION;
            float4 texCoord0;
            #if UNITY_ANY_INSTANCING_ENABLED
            uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };
        struct SurfaceDescriptionInputs
        {
            float4 uv0;
            float3 TimeParameters;
        };
        struct VertexDescriptionInputs
        {
        };
        struct PackedVaryings
        {
            float4 positionCS : SV_POSITION;
            float4 interp0 : TEXCOORD0;
            #if UNITY_ANY_INSTANCING_ENABLED
            uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };

            PackedVaryings PackVaryings (Varyings input)
        {
            PackedVaryings output;
            output.positionCS = input.positionCS;
            output.interp0.xyzw =  input.texCoord0;
            #if UNITY_ANY_INSTANCING_ENABLED
            output.instanceID = input.instanceID;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }
        Varyings UnpackVaryings (PackedVaryings input)
        {
            Varyings output;
            output.positionCS = input.positionCS;
            output.texCoord0 = input.interp0.xyzw;
            #if UNITY_ANY_INSTANCING_ENABLED
            output.instanceID = input.instanceID;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }

            // --------------------------------------------------
            // Graph

            // Graph Properties
            CBUFFER_START(UnityPerMaterial)
        float4 Color_822539e8f7124cf0b91b8404302e4eb4;
        float Vector1_5d78a6ec65f3486fab89281302daa414;
        float2 Vector2_1bd490e57d694d988e0111c4c8f24079;
        float2 Vector2_58ccd7d4e76b48b987ec44839b8492a3;
        float Vector1_c26db16e47624f4bbb0694a0b08822ea;
        CBUFFER_END

        // Object and Global properties

            // Graph Functions
            
        void Unity_TilingAndOffset_float(float2 UV, float2 Tiling, float2 Offset, out float2 Out)
        {
            Out = UV * Tiling + Offset;
        }


        inline float Unity_SimpleNoise_RandomValue_float (float2 uv)
        {
            return frac(sin(dot(uv, float2(12.9898, 78.233)))*43758.5453);
        }

        inline float Unity_SimpleNnoise_Interpolate_float (float a, float b, float t)
        {
            return (1.0-t)*a + (t*b);
        }


        inline float Unity_SimpleNoise_ValueNoise_float (float2 uv)
        {
            float2 i = floor(uv);
            float2 f = frac(uv);
            f = f * f * (3.0 - 2.0 * f);

            uv = abs(frac(uv) - 0.5);
            float2 c0 = i + float2(0.0, 0.0);
            float2 c1 = i + float2(1.0, 0.0);
            float2 c2 = i + float2(0.0, 1.0);
            float2 c3 = i + float2(1.0, 1.0);
            float r0 = Unity_SimpleNoise_RandomValue_float(c0);
            float r1 = Unity_SimpleNoise_RandomValue_float(c1);
            float r2 = Unity_SimpleNoise_RandomValue_float(c2);
            float r3 = Unity_SimpleNoise_RandomValue_float(c3);

            float bottomOfGrid = Unity_SimpleNnoise_Interpolate_float(r0, r1, f.x);
            float topOfGrid = Unity_SimpleNnoise_Interpolate_float(r2, r3, f.x);
            float t = Unity_SimpleNnoise_Interpolate_float(bottomOfGrid, topOfGrid, f.y);
            return t;
        }
        void Unity_SimpleNoise_float(float2 UV, float Scale, out float Out)
        {
            float t = 0.0;

            float freq = pow(2.0, float(0));
            float amp = pow(0.5, float(3-0));
            t += Unity_SimpleNoise_ValueNoise_float(float2(UV.x*Scale/freq, UV.y*Scale/freq))*amp;

            freq = pow(2.0, float(1));
            amp = pow(0.5, float(3-1));
            t += Unity_SimpleNoise_ValueNoise_float(float2(UV.x*Scale/freq, UV.y*Scale/freq))*amp;

            freq = pow(2.0, float(2));
            amp = pow(0.5, float(3-2));
            t += Unity_SimpleNoise_ValueNoise_float(float2(UV.x*Scale/freq, UV.y*Scale/freq))*amp;

            Out = t;
        }

        void Unity_Add_float(float A, float B, out float Out)
        {
            Out = A + B;
        }

        void Unity_Multiply_float(float A, float B, out float Out)
        {
            Out = A * B;
        }

        void NoiseSineWave_float(float In, float2 MinMax, out float Out)
        {
            float sinIn = sin(In);
            float sinInOffset = sin(In + 1.0);
            float randomno =  frac(sin((sinIn - sinInOffset) * (12.9898 + 78.233))*43758.5453);
            float noise = lerp(MinMax.x, MinMax.y, randomno);
            Out = sinIn + noise;
        }


        float2 Unity_GradientNoise_Dir_float(float2 p)
        {
            // Permutation and hashing used in webgl-nosie goo.gl/pX7HtC
            p = p % 289;
            // need full precision, otherwise half overflows when p > 1
            float x = float(34 * p.x + 1) * p.x % 289 + p.y;
            x = (34 * x + 1) * x % 289;
            x = frac(x / 41) * 2 - 1;
            return normalize(float2(x - floor(x + 0.5), abs(x) - 0.5));
        }

        void Unity_GradientNoise_float(float2 UV, float Scale, out float Out)
        { 
            float2 p = UV * Scale;
            float2 ip = floor(p);
            float2 fp = frac(p);
            float d00 = dot(Unity_GradientNoise_Dir_float(ip), fp);
            float d01 = dot(Unity_GradientNoise_Dir_float(ip + float2(0, 1)), fp - float2(0, 1));
            float d10 = dot(Unity_GradientNoise_Dir_float(ip + float2(1, 0)), fp - float2(1, 0));
            float d11 = dot(Unity_GradientNoise_Dir_float(ip + float2(1, 1)), fp - float2(1, 1));
            fp = fp * fp * fp * (fp * (fp * 6 - 15) + 10);
            Out = lerp(lerp(d00, d01, fp.y), lerp(d10, d11, fp.y), fp.x) + 0.5;
        }

        void Unity_Blend_Difference_float(float Base, float Blend, out float Out, float Opacity)
        {
            Out = abs(Blend - Base);
            Out = lerp(Base, Out, Opacity);
        }

        void Unity_InvertColors_float(float In, float InvertColors, out float Out)
        {
            Out = abs(InvertColors - In);
        }

        void Unity_Blend_Overwrite_float4(float4 Base, float4 Blend, out float4 Out, float Opacity)
        {
            Out = lerp(Base, Blend, Opacity);
        }

            // Graph Vertex
            // GraphVertex: <None>

            // Graph Pixel
            struct SurfaceDescription
        {
            float4 Out;
        };

        SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
        {
            SurfaceDescription surface = (SurfaceDescription)0;
            float4 _Property_b08dc11a3c5d4961af1cc856478a6ed3_Out_0 = Color_822539e8f7124cf0b91b8404302e4eb4;
            float2 _TilingAndOffset_036f7a1957a14e018dab84e1a2d3f3c6_Out_3;
            Unity_TilingAndOffset_float(IN.uv0.xy, float2 (10, 10), float2 (0, 0), _TilingAndOffset_036f7a1957a14e018dab84e1a2d3f3c6_Out_3);
            float _Property_894d5bd9b5c549acbe7f57679c13050a_Out_0 = Vector1_5d78a6ec65f3486fab89281302daa414;
            float _SimpleNoise_b30788e1e9d2409fa282492752fccda9_Out_2;
            Unity_SimpleNoise_float(_TilingAndOffset_036f7a1957a14e018dab84e1a2d3f3c6_Out_3, _Property_894d5bd9b5c549acbe7f57679c13050a_Out_0, _SimpleNoise_b30788e1e9d2409fa282492752fccda9_Out_2);
            float _Add_a7c266a6f2ae475b9c63cf98afa605a5_Out_2;
            Unity_Add_float(IN.TimeParameters.x, 10, _Add_a7c266a6f2ae475b9c63cf98afa605a5_Out_2);
            float _Multiply_e271f9dc245144e3bf412558cc74f8e8_Out_2;
            Unity_Multiply_float(_Add_a7c266a6f2ae475b9c63cf98afa605a5_Out_2, 10, _Multiply_e271f9dc245144e3bf412558cc74f8e8_Out_2);
            float _Multiply_97f25c8186c54c068a1e69c208af7268_Out_2;
            Unity_Multiply_float(_SimpleNoise_b30788e1e9d2409fa282492752fccda9_Out_2, _Multiply_e271f9dc245144e3bf412558cc74f8e8_Out_2, _Multiply_97f25c8186c54c068a1e69c208af7268_Out_2);
            float2 _Property_f7ed3ca4fc1b4e64b5f4de8e2940007e_Out_0 = Vector2_1bd490e57d694d988e0111c4c8f24079;
            float _NoiseSineWave_2736b029ecce4ceaa12c2a0950913dac_Out_2;
            NoiseSineWave_float(_Multiply_97f25c8186c54c068a1e69c208af7268_Out_2, _Property_f7ed3ca4fc1b4e64b5f4de8e2940007e_Out_0, _NoiseSineWave_2736b029ecce4ceaa12c2a0950913dac_Out_2);
            float2 _Property_ef667881bda34f5c9bd41e8aa265daa2_Out_0 = Vector2_58ccd7d4e76b48b987ec44839b8492a3;
            float2 _TilingAndOffset_e5d159e43ea0483090134648a97ad276_Out_3;
            Unity_TilingAndOffset_float(IN.uv0.xy, _Property_ef667881bda34f5c9bd41e8aa265daa2_Out_0, float2 (0, 0), _TilingAndOffset_e5d159e43ea0483090134648a97ad276_Out_3);
            float _GradientNoise_5d78b421a95143e0ab572d8d492e3066_Out_2;
            Unity_GradientNoise_float(_TilingAndOffset_e5d159e43ea0483090134648a97ad276_Out_3, 10, _GradientNoise_5d78b421a95143e0ab572d8d492e3066_Out_2);
            float _Blend_22e9458a678749daaa12910907663b95_Out_2;
            Unity_Blend_Difference_float(_NoiseSineWave_2736b029ecce4ceaa12c2a0950913dac_Out_2, _GradientNoise_5d78b421a95143e0ab572d8d492e3066_Out_2, _Blend_22e9458a678749daaa12910907663b95_Out_2, 1);
            float _InvertColors_3775b0392d1543dc8e1de4f3a0213f61_Out_1;
            float _InvertColors_3775b0392d1543dc8e1de4f3a0213f61_InvertColors = float (1
        );    Unity_InvertColors_float(_Blend_22e9458a678749daaa12910907663b95_Out_2, _InvertColors_3775b0392d1543dc8e1de4f3a0213f61_InvertColors, _InvertColors_3775b0392d1543dc8e1de4f3a0213f61_Out_1);
            float _Property_6d7d7158914942a688f9ec3f2e533b75_Out_0 = Vector1_c26db16e47624f4bbb0694a0b08822ea;
            float4 _Blend_75cfcc4dc56549a3be788c10bf72054d_Out_2;
            Unity_Blend_Overwrite_float4(_Property_b08dc11a3c5d4961af1cc856478a6ed3_Out_0, (_InvertColors_3775b0392d1543dc8e1de4f3a0213f61_Out_1.xxxx), _Blend_75cfcc4dc56549a3be788c10bf72054d_Out_2, _Property_6d7d7158914942a688f9ec3f2e533b75_Out_0);
            surface.Out = all(isfinite(_Blend_75cfcc4dc56549a3be788c10bf72054d_Out_2)) ? half4(_Blend_75cfcc4dc56549a3be788c10bf72054d_Out_2.x, _Blend_75cfcc4dc56549a3be788c10bf72054d_Out_2.y, _Blend_75cfcc4dc56549a3be788c10bf72054d_Out_2.z, 1.0) : float4(1.0f, 0.0f, 1.0f, 1.0f);
            return surface;
        }

            // --------------------------------------------------
            // Build Graph Inputs

            SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
        {
            SurfaceDescriptionInputs output;
            ZERO_INITIALIZE(SurfaceDescriptionInputs, output);





            output.uv0 =                         input.texCoord0;
            output.TimeParameters =              _TimeParameters.xyz; // This is mainly for LW as HD overwrite this value
        #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN output.FaceSign =                    IS_FRONT_VFACE(input.cullFace, true, false);
        #else
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
        #endif
        #undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN

            return output;
        }

            // --------------------------------------------------
            // Main

            #include "Packages/com.unity.shadergraph/ShaderGraphLibrary/PreviewVaryings.hlsl"
        #include "Packages/com.unity.shadergraph/ShaderGraphLibrary/PreviewPass.hlsl"

            ENDHLSL
        }
    }
    FallBack "Hidden/Shader Graph/FallbackError"
}