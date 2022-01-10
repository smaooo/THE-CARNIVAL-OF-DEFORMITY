Shader "Blend"
{
    Properties
    {
        Vector1_19c64a0b93ee4a1893264e3c22ebd67f("NoiseScale", Float) = 500
        Vector2_02776a873fcc45d7a6654dc970924a90("NoiseTiling", Vector) = (1, 1, 0, 0)
        Vector2_e7d4e7c4b8624c2bb19ad2050fe0e36d("NoiseOffset", Vector) = (0, 0, 0, 0)
        [ToggleUI]Boolean_ddaca20621ea437ea30a78eeef9443df("Voronoi?", Float) = 0
        Vector1_d879f146beed4d0684cb13e90cebe1b3("AngleOffset(Voronoi)", Float) = 50.33
        Vector1_b4c89059f6a84cdfb7a7342989bbf1d7("CellDensity(Voronoi)", Float) = 135.97
        Vector1_a4aa88509466449fb9aff5a3957a2fdc("Speed", Float) = 0
        Vector1_156036b18e8e4c7b8fd37d24c10136b9("WhiteSpacesDistance", Float) = 0.65
        Vector1_36f44bc0498847b79adcf7ae73a92814("WhiteSpacesOpacity", Float) = -1
        Vector1_f29b9ea73958480ca2f3e58d2914a5df("WhiteSpacesBlend", Float) = 0.65
        Vector1_58d21763b2c14fb5945f9ffca8906fd6("WhiteSpacesScale", Float) = 10.5
        Color_ea45be10297c479c9682a0130465e19b("WhiteSpaceBlendColor", Color) = (1, 1, 1, 0)
        [ToggleUI]Boolean_08d4832afa3146bf934ea9747becbdfb("Invert", Float) = 0
        Vector1_8f4dd1578886465d97f24a30669f4101("Alpha", Float) = 1
        Color_0026b8c7960f4312a4f4dc09af7625bd("SnowColor", Color) = (0.754717, 0.754717, 0.754717, 0)
        [NonModifiableTextureData][NoScaleOffset]_SampleTexture2D_fcccb80658044d3687848cd2e3ba0546_Texture_1("Texture2D", 2D) = "white" {}
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
        float4 _SampleTexture2D_fcccb80658044d3687848cd2e3ba0546_Texture_1_TexelSize;
        float Vector1_19c64a0b93ee4a1893264e3c22ebd67f;
        float2 Vector2_02776a873fcc45d7a6654dc970924a90;
        float2 Vector2_e7d4e7c4b8624c2bb19ad2050fe0e36d;
        float Boolean_ddaca20621ea437ea30a78eeef9443df;
        float Vector1_d879f146beed4d0684cb13e90cebe1b3;
        float Vector1_b4c89059f6a84cdfb7a7342989bbf1d7;
        float Vector1_a4aa88509466449fb9aff5a3957a2fdc;
        float Vector1_156036b18e8e4c7b8fd37d24c10136b9;
        float Vector1_36f44bc0498847b79adcf7ae73a92814;
        float Vector1_f29b9ea73958480ca2f3e58d2914a5df;
        float Vector1_58d21763b2c14fb5945f9ffca8906fd6;
        float4 Color_ea45be10297c479c9682a0130465e19b;
        float Boolean_08d4832afa3146bf934ea9747becbdfb;
        float Vector1_8f4dd1578886465d97f24a30669f4101;
        float4 Color_0026b8c7960f4312a4f4dc09af7625bd;
        CBUFFER_END

        // Object and Global properties
        SAMPLER(SamplerState_Linear_Repeat);
        TEXTURE2D(_SampleTexture2D_fcccb80658044d3687848cd2e3ba0546_Texture_1);
        SAMPLER(sampler_SampleTexture2D_fcccb80658044d3687848cd2e3ba0546_Texture_1);

            // Graph Functions
            
        void Unity_Multiply_float(float A, float B, out float Out)
        {
            Out = A * B;
        }

        void Unity_Combine_float(float R, float G, float B, float A, out float4 RGBA, out float3 RGB, out float2 RG)
        {
            RGBA = float4(R, G, B, A);
            RGB = float3(R, G, B);
            RG = float2(R, G);
        }

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

        void Unity_Branch_float(float Predicate, float True, float False, out float Out)
        {
            Out = Predicate ? True : False;
        }

        void Unity_Polygon_float(float2 UV, float Sides, float Width, float Height, out float Out)
        {
            float pi = 3.14159265359;
            float aWidth = Width * cos(pi / Sides);
            float aHeight = Height * cos(pi / Sides);
            float2 uv = (UV * 2 - 1) / float2(aWidth, aHeight);
            uv.y *= -1;
            float pCoord = atan2(uv.x, uv.y);
            float r = 2 * pi / Sides;
            float distance = cos(floor(0.5 + pCoord / r) * r - pCoord) * length(uv);
            Out = saturate((1 - distance) / fwidth(distance));
        }

        void Unity_InvertColors_float(float In, float InvertColors, out float Out)
        {
            Out = abs(InvertColors - In);
        }

        void Unity_Add_float(float A, float B, out float Out)
        {
            Out = A + B;
        }

        void Unity_ColorMask_float(float3 In, float3 MaskColor, float Range, out float Out, float Fuzziness)
        {
            float Distance = distance(MaskColor, In);
            Out = saturate(1 - (Distance - Range) / max(Fuzziness, 1e-5));
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

        void Unity_Blend_Overlay_float(float Base, float Blend, out float Out, float Opacity)
        {
            float result1 = 1.0 - 2.0 * (1.0 - Base) * (1.0 - Blend);
            float result2 = 2.0 * Base * Blend;
            float zeroOrOne = step(Base, 0.5);
            Out = result2 * zeroOrOne + (1 - zeroOrOne) * result1;
            Out = lerp(Base, Out, Opacity);
        }

        void Unity_Subtract_float(float A, float B, out float Out)
        {
            Out = A - B;
        }

        void Unity_Blend_Difference_float4(float4 Base, float4 Blend, out float4 Out, float Opacity)
        {
            Out = abs(Blend - Base);
            Out = lerp(Base, Out, Opacity);
        }

        void Unity_Branch_float4(float Predicate, float4 True, float4 False, out float4 Out)
        {
            Out = Predicate ? True : False;
        }

        void Unity_Blend_Multiply_float4(float4 Base, float4 Blend, out float4 Out, float Opacity)
        {
            Out = Base * Blend;
            Out = lerp(Base, Out, Opacity);
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
            float4 _Property_46b298c15afd4aa69478ca1a3ed68225_Out_0 = Color_0026b8c7960f4312a4f4dc09af7625bd;
            float _Property_985f342cc1fa425fb47f18e73c8fdba9_Out_0 = Boolean_08d4832afa3146bf934ea9747becbdfb;
            float _Property_efd41a5057164c3485909c4dae7ea579_Out_0 = Boolean_ddaca20621ea437ea30a78eeef9443df;
            float2 _Property_5b5e39969cc140178a64fbeb620fb578_Out_0 = Vector2_02776a873fcc45d7a6654dc970924a90;
            float2 _Property_fe4f55e9486a42a29a2af59afcec385e_Out_0 = Vector2_e7d4e7c4b8624c2bb19ad2050fe0e36d;
            float _Split_38b0aa430cfc43dfb109baeb31286ad9_R_1 = _Property_fe4f55e9486a42a29a2af59afcec385e_Out_0[0];
            float _Split_38b0aa430cfc43dfb109baeb31286ad9_G_2 = _Property_fe4f55e9486a42a29a2af59afcec385e_Out_0[1];
            float _Split_38b0aa430cfc43dfb109baeb31286ad9_B_3 = 0;
            float _Split_38b0aa430cfc43dfb109baeb31286ad9_A_4 = 0;
            float _Property_e156e64e92e64424abf1410c896cb256_Out_0 = Vector1_a4aa88509466449fb9aff5a3957a2fdc;
            float _Multiply_b1c2acf332af4f869ab0fb61640fc880_Out_2;
            Unity_Multiply_float(IN.TimeParameters.x, _Property_e156e64e92e64424abf1410c896cb256_Out_0, _Multiply_b1c2acf332af4f869ab0fb61640fc880_Out_2);
            float4 _Combine_49e393ee0e734704b2c90eb4b5e9cba7_RGBA_4;
            float3 _Combine_49e393ee0e734704b2c90eb4b5e9cba7_RGB_5;
            float2 _Combine_49e393ee0e734704b2c90eb4b5e9cba7_RG_6;
            Unity_Combine_float(_Split_38b0aa430cfc43dfb109baeb31286ad9_R_1, _Multiply_b1c2acf332af4f869ab0fb61640fc880_Out_2, 0, 0, _Combine_49e393ee0e734704b2c90eb4b5e9cba7_RGBA_4, _Combine_49e393ee0e734704b2c90eb4b5e9cba7_RGB_5, _Combine_49e393ee0e734704b2c90eb4b5e9cba7_RG_6);
            float2 _TilingAndOffset_8f207c4901094b238c790b6839052dca_Out_3;
            Unity_TilingAndOffset_float(IN.uv0.xy, _Property_5b5e39969cc140178a64fbeb620fb578_Out_0, _Combine_49e393ee0e734704b2c90eb4b5e9cba7_RG_6, _TilingAndOffset_8f207c4901094b238c790b6839052dca_Out_3);
            float _Property_783295ea3ac24baa836d3cb56f5dafbe_Out_0 = Vector1_d879f146beed4d0684cb13e90cebe1b3;
            float _Property_a94efd28b62740db894c0eec14f0b569_Out_0 = Vector1_b4c89059f6a84cdfb7a7342989bbf1d7;
            float _Voronoi_14be7913c469411b8e329cf51a658f5c_Out_3;
            float _Voronoi_14be7913c469411b8e329cf51a658f5c_Cells_4;
            Unity_Voronoi_float(_TilingAndOffset_8f207c4901094b238c790b6839052dca_Out_3, _Property_783295ea3ac24baa836d3cb56f5dafbe_Out_0, _Property_a94efd28b62740db894c0eec14f0b569_Out_0, _Voronoi_14be7913c469411b8e329cf51a658f5c_Out_3, _Voronoi_14be7913c469411b8e329cf51a658f5c_Cells_4);
            float _Property_e7209232234b48d69a548f780597e2ab_Out_0 = Vector1_19c64a0b93ee4a1893264e3c22ebd67f;
            float _SimpleNoise_0bcef006d66c4ba0b122502a6d5d1c29_Out_2;
            Unity_SimpleNoise_float(_TilingAndOffset_8f207c4901094b238c790b6839052dca_Out_3, _Property_e7209232234b48d69a548f780597e2ab_Out_0, _SimpleNoise_0bcef006d66c4ba0b122502a6d5d1c29_Out_2);
            float _Branch_77b26083db9c4b1dbcd933f2d978440a_Out_3;
            Unity_Branch_float(_Property_efd41a5057164c3485909c4dae7ea579_Out_0, _Voronoi_14be7913c469411b8e329cf51a658f5c_Cells_4, _SimpleNoise_0bcef006d66c4ba0b122502a6d5d1c29_Out_2, _Branch_77b26083db9c4b1dbcd933f2d978440a_Out_3);
            float _Polygon_68b2f6734af14227b9b3b6b0b4987cba_Out_4;
            Unity_Polygon_float(IN.uv0.xy, 16.76, 0.54, 0.97, _Polygon_68b2f6734af14227b9b3b6b0b4987cba_Out_4);
            float _InvertColors_10eab5337bd047cc9e0b79247ffaa6df_Out_1;
            float _InvertColors_10eab5337bd047cc9e0b79247ffaa6df_InvertColors = float (1
        );    Unity_InvertColors_float(_Polygon_68b2f6734af14227b9b3b6b0b4987cba_Out_4, _InvertColors_10eab5337bd047cc9e0b79247ffaa6df_InvertColors, _InvertColors_10eab5337bd047cc9e0b79247ffaa6df_Out_1);
            float4 _SampleTexture2D_fcccb80658044d3687848cd2e3ba0546_RGBA_0 = SAMPLE_TEXTURE2D(UnityBuildTexture2DStructNoScale(_SampleTexture2D_fcccb80658044d3687848cd2e3ba0546_Texture_1).tex, UnityBuildTexture2DStructNoScale(_SampleTexture2D_fcccb80658044d3687848cd2e3ba0546_Texture_1).samplerstate, IN.uv0.xy);
            float _SampleTexture2D_fcccb80658044d3687848cd2e3ba0546_R_4 = _SampleTexture2D_fcccb80658044d3687848cd2e3ba0546_RGBA_0.r;
            float _SampleTexture2D_fcccb80658044d3687848cd2e3ba0546_G_5 = _SampleTexture2D_fcccb80658044d3687848cd2e3ba0546_RGBA_0.g;
            float _SampleTexture2D_fcccb80658044d3687848cd2e3ba0546_B_6 = _SampleTexture2D_fcccb80658044d3687848cd2e3ba0546_RGBA_0.b;
            float _SampleTexture2D_fcccb80658044d3687848cd2e3ba0546_A_7 = _SampleTexture2D_fcccb80658044d3687848cd2e3ba0546_RGBA_0.a;
            float _Add_9b968ecd183141f5b708ce7aff63bdcb_Out_2;
            Unity_Add_float(_InvertColors_10eab5337bd047cc9e0b79247ffaa6df_Out_1, _SampleTexture2D_fcccb80658044d3687848cd2e3ba0546_A_7, _Add_9b968ecd183141f5b708ce7aff63bdcb_Out_2);
            float _ColorMask_e595206bbe0a4cffbda447138fb7cfdd_Out_3;
            Unity_ColorMask_float((_Add_9b968ecd183141f5b708ce7aff63bdcb_Out_2.xxx), IsGammaSpace() ? float3(0, 0, 0) : SRGBToLinear(float3(0, 0, 0)), 0, _ColorMask_e595206bbe0a4cffbda447138fb7cfdd_Out_3, 0);
            float _ColorMask_b8bebb92fe7c448484593772aebd8578_Out_3;
            Unity_ColorMask_float((_Branch_77b26083db9c4b1dbcd933f2d978440a_Out_3.xxx), IsGammaSpace() ? float3(0, 0, 0) : SRGBToLinear(float3(0, 0, 0)), -0.14, _ColorMask_b8bebb92fe7c448484593772aebd8578_Out_3, _ColorMask_e595206bbe0a4cffbda447138fb7cfdd_Out_3);
            float4 _Property_21ee1c34f420464a9c244c243043c548_Out_0 = Color_ea45be10297c479c9682a0130465e19b;
            float _Property_119b073017e64995bf841e1b36de9406_Out_0 = Vector1_f29b9ea73958480ca2f3e58d2914a5df;
            float _Property_91ae4ef8b85049bd8deadc4ab6847a48_Out_0 = Vector1_58d21763b2c14fb5945f9ffca8906fd6;
            float _GradientNoise_207d09f70f25423ca33f67f711f70f93_Out_2;
            Unity_GradientNoise_float(IN.uv0.xy, _Property_91ae4ef8b85049bd8deadc4ab6847a48_Out_0, _GradientNoise_207d09f70f25423ca33f67f711f70f93_Out_2);
            float _Property_86dd9fd5e9924e27ac75c966800efc22_Out_0 = Vector1_156036b18e8e4c7b8fd37d24c10136b9;
            float _Property_7d054493caff48b9a246d92f775baaa6_Out_0 = Vector1_36f44bc0498847b79adcf7ae73a92814;
            float _Blend_c175ed42b52a4e9bbd9ef14c142d2adc_Out_2;
            Unity_Blend_Overlay_float(_GradientNoise_207d09f70f25423ca33f67f711f70f93_Out_2, _Property_86dd9fd5e9924e27ac75c966800efc22_Out_0, _Blend_c175ed42b52a4e9bbd9ef14c142d2adc_Out_2, _Property_7d054493caff48b9a246d92f775baaa6_Out_0);
            float _ColorMask_7379551c2ba3451b8720032c9a4d931f_Out_3;
            Unity_ColorMask_float((_ColorMask_b8bebb92fe7c448484593772aebd8578_Out_3.xxx), (_Property_21ee1c34f420464a9c244c243043c548_Out_0.xyz), _Property_119b073017e64995bf841e1b36de9406_Out_0, _ColorMask_7379551c2ba3451b8720032c9a4d931f_Out_3, _Blend_c175ed42b52a4e9bbd9ef14c142d2adc_Out_2);
            float _Subtract_7aaaab97f99c466f94e5bfc934c79d7d_Out_2;
            Unity_Subtract_float(_ColorMask_7379551c2ba3451b8720032c9a4d931f_Out_3, _Add_9b968ecd183141f5b708ce7aff63bdcb_Out_2, _Subtract_7aaaab97f99c466f94e5bfc934c79d7d_Out_2);
            float4 Color_eecd45193faa4e71b403e152fe614a22 = IsGammaSpace() ? float4(1, 1, 1, 0) : float4(SRGBToLinear(float3(1, 1, 1)), 0);
            float4 _Blend_c92d0bd82548482f80730cd916c7735c_Out_2;
            Unity_Blend_Difference_float4((_Subtract_7aaaab97f99c466f94e5bfc934c79d7d_Out_2.xxxx), Color_eecd45193faa4e71b403e152fe614a22, _Blend_c92d0bd82548482f80730cd916c7735c_Out_2, _ColorMask_e595206bbe0a4cffbda447138fb7cfdd_Out_3);
            float4 _Branch_aa88139b0929435780942fc790741955_Out_3;
            Unity_Branch_float4(_Property_985f342cc1fa425fb47f18e73c8fdba9_Out_0, _Blend_c92d0bd82548482f80730cd916c7735c_Out_2, (_Subtract_7aaaab97f99c466f94e5bfc934c79d7d_Out_2.xxxx), _Branch_aa88139b0929435780942fc790741955_Out_3);
            float4 _Blend_018cea0201524bbdb60ab404ad4afabc_Out_2;
            Unity_Blend_Multiply_float4(_Property_46b298c15afd4aa69478ca1a3ed68225_Out_0, _Branch_aa88139b0929435780942fc790741955_Out_3, _Blend_018cea0201524bbdb60ab404ad4afabc_Out_2, (_Branch_aa88139b0929435780942fc790741955_Out_3).x);
            surface.Out = all(isfinite(_Blend_018cea0201524bbdb60ab404ad4afabc_Out_2)) ? half4(_Blend_018cea0201524bbdb60ab404ad4afabc_Out_2.x, _Blend_018cea0201524bbdb60ab404ad4afabc_Out_2.y, _Blend_018cea0201524bbdb60ab404ad4afabc_Out_2.z, 1.0) : float4(1.0f, 0.0f, 1.0f, 1.0f);
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