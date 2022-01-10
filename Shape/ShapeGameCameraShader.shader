Shader "Branch"
{
    Properties
    {
        [NoScaleOffset]_MainTex("MainTex", 2D) = "white" {}
        Vector1_0aae5b0514b74e79bbf1ee345ce4ac0b("Rotation", Float) = 0
        Vector2_99aa39318d324874879dc517ed7e89fc("Center", Vector) = (1, 1, 0, 0)
        Vector1_a44a4517f4134339b420a93b38bf26be("Blend", Float) = 0.5
        _AngleOffset("AngleOffset", Float) = 4
        _CellDensity("CellDensity", Float) = 5
        [ToggleUI]Boolean_f6173b63215d427cb6ea788d47270998("Blend?", Float) = 0
        [ToggleUI]Boolean_a568e08a1b09488fa21b347515fa4284("AddUV?", Float) = 0
        [ToggleUI]_MG4("MG4?", Float) = 0
        Vector2_5ba80fe69adf479b8cca8888f0f7e1e0("RectMask", Vector) = (0.75, 0.85, 0, 0)
        Vector2_7b07a19f770e4b34948d9b9179a42ca3("RectTilling", Vector) = (1, 1, 0, 0)
        Vector2_efc62a7fdfe445989652acaa267bac0a("RectOffset", Vector) = (0, 0, 0, 0)
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
            #define ATTRIBUTES_NEED_NORMAL
            #define ATTRIBUTES_NEED_TEXCOORD0
            #define VARYINGS_NEED_NORMAL_WS
            #define VARYINGS_NEED_TEXCOORD0
            #define VARYINGS_NEED_VIEWDIRECTION_WS
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
            float3 normalOS : NORMAL;
            float4 uv0 : TEXCOORD0;
            #if UNITY_ANY_INSTANCING_ENABLED
            uint instanceID : INSTANCEID_SEMANTIC;
            #endif
        };
        struct Varyings
        {
            float4 positionCS : SV_POSITION;
            float3 normalWS;
            float4 texCoord0;
            float3 viewDirectionWS;
            #if UNITY_ANY_INSTANCING_ENABLED
            uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };
        struct SurfaceDescriptionInputs
        {
            float3 WorldSpaceNormal;
            float3 WorldSpaceViewDirection;
            float4 uv0;
        };
        struct VertexDescriptionInputs
        {
        };
        struct PackedVaryings
        {
            float4 positionCS : SV_POSITION;
            float3 interp0 : TEXCOORD0;
            float4 interp1 : TEXCOORD1;
            float3 interp2 : TEXCOORD2;
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
            output.interp0.xyz =  input.normalWS;
            output.interp1.xyzw =  input.texCoord0;
            output.interp2.xyz =  input.viewDirectionWS;
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
            output.normalWS = input.interp0.xyz;
            output.texCoord0 = input.interp1.xyzw;
            output.viewDirectionWS = input.interp2.xyz;
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
        float4 _MainTex_TexelSize;
        float Vector1_0aae5b0514b74e79bbf1ee345ce4ac0b;
        float2 Vector2_99aa39318d324874879dc517ed7e89fc;
        float Vector1_a44a4517f4134339b420a93b38bf26be;
        float _AngleOffset;
        float _CellDensity;
        float Boolean_f6173b63215d427cb6ea788d47270998;
        float Boolean_a568e08a1b09488fa21b347515fa4284;
        float _MG4;
        float2 Vector2_5ba80fe69adf479b8cca8888f0f7e1e0;
        float2 Vector2_7b07a19f770e4b34948d9b9179a42ca3;
        float2 Vector2_efc62a7fdfe445989652acaa267bac0a;
        CBUFFER_END

        // Object and Global properties
        SAMPLER(SamplerState_Linear_Repeat);
        TEXTURE2D(_MainTex);
        SAMPLER(sampler_MainTex);

            // Graph Functions
            

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

        void Unity_FresnelEffect_float(float3 Normal, float3 ViewDir, float Power, out float Out)
        {
            Out = pow((1.0 - saturate(dot(normalize(Normal), normalize(ViewDir)))), Power);
        }

        void Unity_Fraction_float(float In, out float Out)
        {
            Out = frac(In);
        }

        void Unity_Branch_float4(float Predicate, float4 True, float4 False, out float4 Out)
        {
            Out = Predicate ? True : False;
        }

        void Unity_Blend_Overwrite_float4(float4 Base, float4 Blend, out float4 Out, float Opacity)
        {
            Out = lerp(Base, Blend, Opacity);
        }

        void Unity_Multiply_float(float4 A, float4 B, out float4 Out)
        {
            Out = A * B;
        }

        void Unity_Rotate_Radians_float(float2 UV, float2 Center, float Rotation, out float2 Out)
        {
            //rotation matrix
            UV -= Center;
            float s = sin(Rotation);
            float c = cos(Rotation);

            //center rotation matrix
            float2x2 rMatrix = float2x2(c, -s, s, c);
            rMatrix *= 0.5;
            rMatrix += 0.5;
            rMatrix = rMatrix*2 - 1;

            //multiply the UVs by the rotation matrix
            UV.xy = mul(UV.xy, rMatrix);
            UV += Center;

            Out = UV;
        }

        void Unity_ChannelMask_Red_float4 (float4 In, out float4 Out)
        {
            Out = float4(In.r, 0, 0, 0);
        }

        void Unity_Blend_Darken_float4(float4 Base, float4 Blend, out float4 Out, float Opacity)
        {
            Out = min(Blend, Base);
            Out = lerp(Base, Out, Opacity);
        }

        void Unity_TilingAndOffset_float(float2 UV, float2 Tiling, float2 Offset, out float2 Out)
        {
            Out = UV * Tiling + Offset;
        }

        void Unity_Rectangle_float(float2 UV, float Width, float Height, out float Out)
        {
            float2 d = abs(UV * 2 - 1) - float2(Width, Height);
            d = 1 - d / fwidth(d);
            Out = saturate(min(d.x, d.y));
        }

        void Unity_InvertColors_float(float In, float InvertColors, out float Out)
        {
            Out = abs(InvertColors - In);
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
            float _Property_52c8d4d0712a4fbd9f38d7da8a48e9f8_Out_0 = _MG4;
            float _Property_eb443152bf554b58aa77bf11f5c16209_Out_0 = Boolean_f6173b63215d427cb6ea788d47270998;
            UnityTexture2D _Property_44d06fad198c4ebbaa89081f9d46315d_Out_0 = UnityBuildTexture2DStructNoScale(_MainTex);
            float4 _UV_98d7f5358cfc482c95440692ee65b87c_Out_0 = IN.uv0;
            float _Property_b5121af13ece426e99f1c0d8509eb987_Out_0 = _AngleOffset;
            float _Property_50d2f2b7c85842d1874ceb530caefa78_Out_0 = _CellDensity;
            float _Voronoi_80c126846ac141cc92d3344a4ab7385a_Out_3;
            float _Voronoi_80c126846ac141cc92d3344a4ab7385a_Cells_4;
            Unity_Voronoi_float(IN.uv0.xy, _Property_b5121af13ece426e99f1c0d8509eb987_Out_0, _Property_50d2f2b7c85842d1874ceb530caefa78_Out_0, _Voronoi_80c126846ac141cc92d3344a4ab7385a_Out_3, _Voronoi_80c126846ac141cc92d3344a4ab7385a_Cells_4);
            float2 _Property_a73bc576b5344bd3bcd4b6b5f45f723a_Out_0 = Vector2_99aa39318d324874879dc517ed7e89fc;
            float _Split_bf90d988721441c3acef05a7ad0701dd_R_1 = _Property_a73bc576b5344bd3bcd4b6b5f45f723a_Out_0[0];
            float _Split_bf90d988721441c3acef05a7ad0701dd_G_2 = _Property_a73bc576b5344bd3bcd4b6b5f45f723a_Out_0[1];
            float _Split_bf90d988721441c3acef05a7ad0701dd_B_3 = 0;
            float _Split_bf90d988721441c3acef05a7ad0701dd_A_4 = 0;
            float _Multiply_818ee0c6a3cd402fb92a8c104fa38ec8_Out_2;
            Unity_Multiply_float(_Voronoi_80c126846ac141cc92d3344a4ab7385a_Out_3, _Split_bf90d988721441c3acef05a7ad0701dd_R_1, _Multiply_818ee0c6a3cd402fb92a8c104fa38ec8_Out_2);
            float _Multiply_47f238ffef7b4eff854ba33b764dabbd_Out_2;
            Unity_Multiply_float(_Voronoi_80c126846ac141cc92d3344a4ab7385a_Out_3, _Split_bf90d988721441c3acef05a7ad0701dd_G_2, _Multiply_47f238ffef7b4eff854ba33b764dabbd_Out_2);
            float4 _Combine_1ab6ca1f01c84a0bbe68163b77d95a96_RGBA_4;
            float3 _Combine_1ab6ca1f01c84a0bbe68163b77d95a96_RGB_5;
            float2 _Combine_1ab6ca1f01c84a0bbe68163b77d95a96_RG_6;
            Unity_Combine_float(_Multiply_818ee0c6a3cd402fb92a8c104fa38ec8_Out_2, _Multiply_47f238ffef7b4eff854ba33b764dabbd_Out_2, 0, 0, _Combine_1ab6ca1f01c84a0bbe68163b77d95a96_RGBA_4, _Combine_1ab6ca1f01c84a0bbe68163b77d95a96_RGB_5, _Combine_1ab6ca1f01c84a0bbe68163b77d95a96_RG_6);
            float _FresnelEffect_47bf68948ec84e679618c020a23217aa_Out_3;
            Unity_FresnelEffect_float(IN.WorldSpaceNormal, IN.WorldSpaceViewDirection, _Voronoi_80c126846ac141cc92d3344a4ab7385a_Out_3, _FresnelEffect_47bf68948ec84e679618c020a23217aa_Out_3);
            float _Fraction_409b02fb9b374f6187be4f77683c8eee_Out_1;
            Unity_Fraction_float(_FresnelEffect_47bf68948ec84e679618c020a23217aa_Out_3, _Fraction_409b02fb9b374f6187be4f77683c8eee_Out_1);
            float _Property_2787d43a9ebe4e3a95abdab4a400d92f_Out_0 = Boolean_a568e08a1b09488fa21b347515fa4284;
            float4 _SampleTexture2D_56e4ced5d81c4f949d9d6e5bd7f6c4a8_RGBA_0 = SAMPLE_TEXTURE2D(_Property_44d06fad198c4ebbaa89081f9d46315d_Out_0.tex, _Property_44d06fad198c4ebbaa89081f9d46315d_Out_0.samplerstate, IN.uv0.xy);
            float _SampleTexture2D_56e4ced5d81c4f949d9d6e5bd7f6c4a8_R_4 = _SampleTexture2D_56e4ced5d81c4f949d9d6e5bd7f6c4a8_RGBA_0.r;
            float _SampleTexture2D_56e4ced5d81c4f949d9d6e5bd7f6c4a8_G_5 = _SampleTexture2D_56e4ced5d81c4f949d9d6e5bd7f6c4a8_RGBA_0.g;
            float _SampleTexture2D_56e4ced5d81c4f949d9d6e5bd7f6c4a8_B_6 = _SampleTexture2D_56e4ced5d81c4f949d9d6e5bd7f6c4a8_RGBA_0.b;
            float _SampleTexture2D_56e4ced5d81c4f949d9d6e5bd7f6c4a8_A_7 = _SampleTexture2D_56e4ced5d81c4f949d9d6e5bd7f6c4a8_RGBA_0.a;
            float4 _Branch_3bbabc0064314d05b64b80844797c7f4_Out_3;
            Unity_Branch_float4(_Property_2787d43a9ebe4e3a95abdab4a400d92f_Out_0, _SampleTexture2D_56e4ced5d81c4f949d9d6e5bd7f6c4a8_RGBA_0, (_Multiply_818ee0c6a3cd402fb92a8c104fa38ec8_Out_2.xxxx), _Branch_3bbabc0064314d05b64b80844797c7f4_Out_3);
            float4 _Blend_85b180462dcd44a5a278ba54cd1fa501_Out_2;
            Unity_Blend_Overwrite_float4((_Fraction_409b02fb9b374f6187be4f77683c8eee_Out_1.xxxx), _Branch_3bbabc0064314d05b64b80844797c7f4_Out_3, _Blend_85b180462dcd44a5a278ba54cd1fa501_Out_2, _Voronoi_80c126846ac141cc92d3344a4ab7385a_Out_3);
            float _Property_08e59f0c43c544ef85b1a5cd40323afa_Out_0 = Vector1_0aae5b0514b74e79bbf1ee345ce4ac0b;
            float4 _Multiply_3ea526f6c63343b2ba51546662a5debe_Out_2;
            Unity_Multiply_float(_Blend_85b180462dcd44a5a278ba54cd1fa501_Out_2, (_Property_08e59f0c43c544ef85b1a5cd40323afa_Out_0.xxxx), _Multiply_3ea526f6c63343b2ba51546662a5debe_Out_2);
            float2 _Rotate_2f4f5ceb23d54ff787ca4171d0eb5536_Out_3;
            Unity_Rotate_Radians_float((_UV_98d7f5358cfc482c95440692ee65b87c_Out_0.xy), _Combine_1ab6ca1f01c84a0bbe68163b77d95a96_RG_6, (_Multiply_3ea526f6c63343b2ba51546662a5debe_Out_2).x, _Rotate_2f4f5ceb23d54ff787ca4171d0eb5536_Out_3);
            float4 _SampleTexture2D_e172b85a35e84189aa2719e5def2bf1f_RGBA_0 = SAMPLE_TEXTURE2D(_Property_44d06fad198c4ebbaa89081f9d46315d_Out_0.tex, _Property_44d06fad198c4ebbaa89081f9d46315d_Out_0.samplerstate, _Rotate_2f4f5ceb23d54ff787ca4171d0eb5536_Out_3);
            float _SampleTexture2D_e172b85a35e84189aa2719e5def2bf1f_R_4 = _SampleTexture2D_e172b85a35e84189aa2719e5def2bf1f_RGBA_0.r;
            float _SampleTexture2D_e172b85a35e84189aa2719e5def2bf1f_G_5 = _SampleTexture2D_e172b85a35e84189aa2719e5def2bf1f_RGBA_0.g;
            float _SampleTexture2D_e172b85a35e84189aa2719e5def2bf1f_B_6 = _SampleTexture2D_e172b85a35e84189aa2719e5def2bf1f_RGBA_0.b;
            float _SampleTexture2D_e172b85a35e84189aa2719e5def2bf1f_A_7 = _SampleTexture2D_e172b85a35e84189aa2719e5def2bf1f_RGBA_0.a;
            float4 _ChannelMask_c0259183000440a1b92f8d872534cad6_Out_1;
            Unity_ChannelMask_Red_float4 (_SampleTexture2D_56e4ced5d81c4f949d9d6e5bd7f6c4a8_RGBA_0, _ChannelMask_c0259183000440a1b92f8d872534cad6_Out_1);
            float _Property_6a1fe501ede04fc59f3b7a539fdb46d3_Out_0 = Vector1_a44a4517f4134339b420a93b38bf26be;
            float4 _Multiply_1f25d27e6b8e4be1ad2cb18903117c98_Out_2;
            Unity_Multiply_float(_ChannelMask_c0259183000440a1b92f8d872534cad6_Out_1, (_Property_6a1fe501ede04fc59f3b7a539fdb46d3_Out_0.xxxx), _Multiply_1f25d27e6b8e4be1ad2cb18903117c98_Out_2);
            float4 _Blend_99687bda4ec14df4b28058d351de2ad5_Out_2;
            Unity_Blend_Darken_float4(_SampleTexture2D_e172b85a35e84189aa2719e5def2bf1f_RGBA_0, _SampleTexture2D_56e4ced5d81c4f949d9d6e5bd7f6c4a8_RGBA_0, _Blend_99687bda4ec14df4b28058d351de2ad5_Out_2, (_Multiply_1f25d27e6b8e4be1ad2cb18903117c98_Out_2).x);
            float2 _Property_1df2fc054f4a451e93d9932aa0aa956d_Out_0 = Vector2_7b07a19f770e4b34948d9b9179a42ca3;
            float2 _Property_b96d93fb810742df86a7307fe1ed9847_Out_0 = Vector2_efc62a7fdfe445989652acaa267bac0a;
            float2 _TilingAndOffset_5714db3f1fe54aeb86e87efac1a07cdb_Out_3;
            Unity_TilingAndOffset_float(IN.uv0.xy, _Property_1df2fc054f4a451e93d9932aa0aa956d_Out_0, _Property_b96d93fb810742df86a7307fe1ed9847_Out_0, _TilingAndOffset_5714db3f1fe54aeb86e87efac1a07cdb_Out_3);
            float2 _Property_4e132d88e608479197d28dd3dd5f91a4_Out_0 = Vector2_5ba80fe69adf479b8cca8888f0f7e1e0;
            float _Split_d8da52e428364ada8026ddc9cc507bde_R_1 = _Property_4e132d88e608479197d28dd3dd5f91a4_Out_0[0];
            float _Split_d8da52e428364ada8026ddc9cc507bde_G_2 = _Property_4e132d88e608479197d28dd3dd5f91a4_Out_0[1];
            float _Split_d8da52e428364ada8026ddc9cc507bde_B_3 = 0;
            float _Split_d8da52e428364ada8026ddc9cc507bde_A_4 = 0;
            float _Rectangle_8f8d2a9aa27f41c8ba3ed0fb48a86691_Out_3;
            Unity_Rectangle_float(_TilingAndOffset_5714db3f1fe54aeb86e87efac1a07cdb_Out_3, _Split_d8da52e428364ada8026ddc9cc507bde_R_1, _Split_d8da52e428364ada8026ddc9cc507bde_G_2, _Rectangle_8f8d2a9aa27f41c8ba3ed0fb48a86691_Out_3);
            float _InvertColors_6d706ac5207c42279c5e338cb077a02c_Out_1;
            float _InvertColors_6d706ac5207c42279c5e338cb077a02c_InvertColors = float (1
        );    Unity_InvertColors_float(_Rectangle_8f8d2a9aa27f41c8ba3ed0fb48a86691_Out_3, _InvertColors_6d706ac5207c42279c5e338cb077a02c_InvertColors, _InvertColors_6d706ac5207c42279c5e338cb077a02c_Out_1);
            float4 _Blend_765382724a52440983ddd46abead10c4_Out_2;
            Unity_Blend_Overwrite_float4(_Blend_99687bda4ec14df4b28058d351de2ad5_Out_2, _SampleTexture2D_56e4ced5d81c4f949d9d6e5bd7f6c4a8_RGBA_0, _Blend_765382724a52440983ddd46abead10c4_Out_2, _InvertColors_6d706ac5207c42279c5e338cb077a02c_Out_1);
            float4 _Branch_26a67fd722d742ef8230c546559a3e6d_Out_3;
            Unity_Branch_float4(_Property_eb443152bf554b58aa77bf11f5c16209_Out_0, _Blend_765382724a52440983ddd46abead10c4_Out_2, _SampleTexture2D_e172b85a35e84189aa2719e5def2bf1f_RGBA_0, _Branch_26a67fd722d742ef8230c546559a3e6d_Out_3);
            UnityTexture2D _Property_e83d00c1d39c449a9427140e1cd38213_Out_0 = UnityBuildTexture2DStructNoScale(_MainTex);
            float4 _SampleTexture2D_537e2690b4a046798cf721c085c2f49d_RGBA_0 = SAMPLE_TEXTURE2D(_Property_e83d00c1d39c449a9427140e1cd38213_Out_0.tex, _Property_e83d00c1d39c449a9427140e1cd38213_Out_0.samplerstate, IN.uv0.xy);
            float _SampleTexture2D_537e2690b4a046798cf721c085c2f49d_R_4 = _SampleTexture2D_537e2690b4a046798cf721c085c2f49d_RGBA_0.r;
            float _SampleTexture2D_537e2690b4a046798cf721c085c2f49d_G_5 = _SampleTexture2D_537e2690b4a046798cf721c085c2f49d_RGBA_0.g;
            float _SampleTexture2D_537e2690b4a046798cf721c085c2f49d_B_6 = _SampleTexture2D_537e2690b4a046798cf721c085c2f49d_RGBA_0.b;
            float _SampleTexture2D_537e2690b4a046798cf721c085c2f49d_A_7 = _SampleTexture2D_537e2690b4a046798cf721c085c2f49d_RGBA_0.a;
            float4 _Branch_b382292dd5564181a8a8090093a801e0_Out_3;
            Unity_Branch_float4(_Property_52c8d4d0712a4fbd9f38d7da8a48e9f8_Out_0, _Branch_26a67fd722d742ef8230c546559a3e6d_Out_3, _SampleTexture2D_537e2690b4a046798cf721c085c2f49d_RGBA_0, _Branch_b382292dd5564181a8a8090093a801e0_Out_3);
            surface.Out = all(isfinite(_Branch_b382292dd5564181a8a8090093a801e0_Out_3)) ? half4(_Branch_b382292dd5564181a8a8090093a801e0_Out_3.x, _Branch_b382292dd5564181a8a8090093a801e0_Out_3.y, _Branch_b382292dd5564181a8a8090093a801e0_Out_3.z, 1.0) : float4(1.0f, 0.0f, 1.0f, 1.0f);
            return surface;
        }

            // --------------------------------------------------
            // Build Graph Inputs

            SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
        {
            SurfaceDescriptionInputs output;
            ZERO_INITIALIZE(SurfaceDescriptionInputs, output);

        	// must use interpolated tangent, bitangent and normal before they are normalized in the pixel shader.
        	float3 unnormalizedNormalWS = input.normalWS;
            const float renormFactor = 1.0 / length(unnormalizedNormalWS);


            output.WorldSpaceNormal =            renormFactor*input.normalWS.xyz;		// we want a unit length Normal Vector node in shader graph


            output.WorldSpaceViewDirection =     input.viewDirectionWS; //TODO: by default normalized in HD, but not in universal
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