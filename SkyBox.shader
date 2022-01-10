Shader "Branch"
{
    Properties
    {
        Vector2_8291c64153af4b18b8674d9d5b09aed8("Tiling", Vector) = (8, 4, 0, 0)
        Vector2_d27d0179a6a44103beec5e7c9a6d2cb6("Offset", Vector) = (0, 0, 0, 0)
        Vector1_5b2b54696e054674a2c128405851b02a("Power", Float) = 70
        Vector1_dd91a08e9f6f4c81b9dbd643f3a87f57("Density", Float) = 100
        Vector1_a4d1c15887e941dbb3ef0900be84b2ef("AngleOffset", Float) = 0
        Color_0b7e568d05a3465aa19eab18b855dfed("BaseColor", Color) = (0, 0, 0, 0)
        Color_5f6305d0b1b543098fa032c61790aa0d("StarsColor", Color) = (1, 0.9460224, 0.2207547, 0)
        [ToggleUI]Boolean_72bd8ad90c5d4230af3b39ae5d93a86a("AddStars", Float) = 0
        Vector2_70f475e770324175893e132f50d69910("TillingG", Vector) = (1, 1, 0, 0)
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
        float2 Vector2_8291c64153af4b18b8674d9d5b09aed8;
        float2 Vector2_d27d0179a6a44103beec5e7c9a6d2cb6;
        float Vector1_5b2b54696e054674a2c128405851b02a;
        float Vector1_dd91a08e9f6f4c81b9dbd643f3a87f57;
        float Vector1_a4d1c15887e941dbb3ef0900be84b2ef;
        float4 Color_0b7e568d05a3465aa19eab18b855dfed;
        float4 Color_5f6305d0b1b543098fa032c61790aa0d;
        float Boolean_72bd8ad90c5d4230af3b39ae5d93a86a;
        float2 Vector2_70f475e770324175893e132f50d69910;
        CBUFFER_END

        // Object and Global properties

            // Graph Functions
            
        void Unity_TilingAndOffset_float(float2 UV, float2 Tiling, float2 Offset, out float2 Out)
        {
            Out = UV * Tiling + Offset;
        }


        inline float2 Unity_Voronoi_RandomVector_float (float2 UV, float offset)
        {
            float2x2 m = float2x2(15.27, 47.63, 99.41, 89.98);
            UV = frac(sin(mul(UV, m)));
            return float2(sin(UV.y*+offset)*0.5+0.5, cos(UV.x*offset)*0.5+0.5);
        }

        void Unity_Voronoi_float(float2 UV, float AngleOffset, float CellDensity, out float Out, out float Cells)
        {
            float2 g = floor(UV * CellDensity);
            float2 f = frac(UV * CellDensity);
            float t = 8.0;
            float3 res = float3(8.0, 0.0, 0.0);

            for(int y=-1; y<=1; y++)
            {
                for(int x=-1; x<=1; x++)
                {
                    float2 lattice = float2(x,y);
                    float2 offset = Unity_Voronoi_RandomVector_float(lattice + g, AngleOffset);
                    float d = distance(lattice + offset, f);

                    if(d < res.x)
                    {
                        res = float3(d, offset.x, offset.y);
                        Out = res.x;
                        Cells = res.y;
                    }
                }
            }
        }

        void Unity_Saturate_float(float In, out float Out)
        {
            Out = saturate(In);
        }

        void Unity_OneMinus_float(float In, out float Out)
        {
            Out = 1 - In;
        }

        void Unity_Power_float(float A, float B, out float Out)
        {
            Out = pow(A, B);
        }

        void Unity_Multiply_float(float4 A, float4 B, out float4 Out)
        {
            Out = A * B;
        }

        void Unity_Add_float4(float4 A, float4 B, out float4 Out)
        {
            Out = A + B;
        }

        void Unity_Branch_float4(float Predicate, float4 True, float4 False, out float4 Out)
        {
            Out = Predicate ? True : False;
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
            float _Property_bd18031df6e940c2858a2e55da6180ad_Out_0 = Boolean_72bd8ad90c5d4230af3b39ae5d93a86a;
            float4 _Property_bba1ecde16604dd09e8f214c70b0995d_Out_0 = Color_0b7e568d05a3465aa19eab18b855dfed;
            float2 _Property_c75f93277945433188000b7429a86631_Out_0 = Vector2_70f475e770324175893e132f50d69910;
            float2 _TilingAndOffset_c11a91d3aa654dff8e32da140cf106a8_Out_3;
            Unity_TilingAndOffset_float(IN.uv0.xy, _Property_c75f93277945433188000b7429a86631_Out_0, float2 (0, 0), _TilingAndOffset_c11a91d3aa654dff8e32da140cf106a8_Out_3);
            float2 _Property_b0ffa89686994809b146854e9f5cc885_Out_0 = Vector2_8291c64153af4b18b8674d9d5b09aed8;
            float2 _Property_ed54014ce5504b308772183f1e217fb6_Out_0 = Vector2_d27d0179a6a44103beec5e7c9a6d2cb6;
            float2 _TilingAndOffset_768af83d8d6b4c4da974e8adf473e583_Out_3;
            Unity_TilingAndOffset_float(_TilingAndOffset_c11a91d3aa654dff8e32da140cf106a8_Out_3, _Property_b0ffa89686994809b146854e9f5cc885_Out_0, _Property_ed54014ce5504b308772183f1e217fb6_Out_0, _TilingAndOffset_768af83d8d6b4c4da974e8adf473e583_Out_3);
            float _Property_2d319c73b4274a86ac3fa278af34ee21_Out_0 = Vector1_a4d1c15887e941dbb3ef0900be84b2ef;
            float _Property_04f9611dba364c2394a4bbb091ff7915_Out_0 = Vector1_dd91a08e9f6f4c81b9dbd643f3a87f57;
            float _Voronoi_70659c8bf7c54a3c9d56c4711e154b54_Out_3;
            float _Voronoi_70659c8bf7c54a3c9d56c4711e154b54_Cells_4;
            Unity_Voronoi_float(_TilingAndOffset_768af83d8d6b4c4da974e8adf473e583_Out_3, _Property_2d319c73b4274a86ac3fa278af34ee21_Out_0, _Property_04f9611dba364c2394a4bbb091ff7915_Out_0, _Voronoi_70659c8bf7c54a3c9d56c4711e154b54_Out_3, _Voronoi_70659c8bf7c54a3c9d56c4711e154b54_Cells_4);
            float _Saturate_5efa01172ac04f379c0be4fbda6f1b21_Out_1;
            Unity_Saturate_float(_Voronoi_70659c8bf7c54a3c9d56c4711e154b54_Out_3, _Saturate_5efa01172ac04f379c0be4fbda6f1b21_Out_1);
            float _OneMinus_fbb1c907752d4e2d991940d19b024354_Out_1;
            Unity_OneMinus_float(_Saturate_5efa01172ac04f379c0be4fbda6f1b21_Out_1, _OneMinus_fbb1c907752d4e2d991940d19b024354_Out_1);
            float _Property_3f3c6a7bb37a4d479bdbee705c40c373_Out_0 = Vector1_5b2b54696e054674a2c128405851b02a;
            float _Power_3a735b3c612044ecaec658a9cff2a58e_Out_2;
            Unity_Power_float(_OneMinus_fbb1c907752d4e2d991940d19b024354_Out_1, _Property_3f3c6a7bb37a4d479bdbee705c40c373_Out_0, _Power_3a735b3c612044ecaec658a9cff2a58e_Out_2);
            float4 _Property_3a0b5e0a1f4542cf8ece5223edfe8eb5_Out_0 = Color_5f6305d0b1b543098fa032c61790aa0d;
            float4 _Multiply_5fa792baa1ce43b4b704672f0cc01190_Out_2;
            Unity_Multiply_float((_Power_3a735b3c612044ecaec658a9cff2a58e_Out_2.xxxx), _Property_3a0b5e0a1f4542cf8ece5223edfe8eb5_Out_0, _Multiply_5fa792baa1ce43b4b704672f0cc01190_Out_2);
            float4 _Add_635219420dd8436f8e24d28852375ddb_Out_2;
            Unity_Add_float4(_Property_bba1ecde16604dd09e8f214c70b0995d_Out_0, _Multiply_5fa792baa1ce43b4b704672f0cc01190_Out_2, _Add_635219420dd8436f8e24d28852375ddb_Out_2);
            float4 _Branch_b5a00678aa204a6da8709708d00bae3d_Out_3;
            Unity_Branch_float4(_Property_bd18031df6e940c2858a2e55da6180ad_Out_0, _Add_635219420dd8436f8e24d28852375ddb_Out_2, _Property_bba1ecde16604dd09e8f214c70b0995d_Out_0, _Branch_b5a00678aa204a6da8709708d00bae3d_Out_3);
            surface.Out = all(isfinite(_Branch_b5a00678aa204a6da8709708d00bae3d_Out_3)) ? half4(_Branch_b5a00678aa204a6da8709708d00bae3d_Out_3.x, _Branch_b5a00678aa204a6da8709708d00bae3d_Out_3.y, _Branch_b5a00678aa204a6da8709708d00bae3d_Out_3.z, 1.0) : float4(1.0f, 0.0f, 1.0f, 1.0f);
            return surface;
        }

            // --------------------------------------------------
            // Build Graph Inputs

            SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
        {
            SurfaceDescriptionInputs output;
            ZERO_INITIALIZE(SurfaceDescriptionInputs, output);





            output.uv0 =                         input.texCoord0;
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