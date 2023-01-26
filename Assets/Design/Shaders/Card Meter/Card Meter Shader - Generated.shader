Shader "Card Meter Shader"
{ // https://forum.unity.com/threads/shader-graph-ui-image-shader-does-not-work.1202461/
    Properties
    {
        _Fill("Fill", Range(0, 1)) = 0.3
        _Radius("Radius", Float) = 1.59
        [NoScaleOffset]_MainTex("Main Tex", 2D) = "white" {}
        _Color("Color", Color) = (1, 1, 1, 0)
        _EmptyColor("Empty Color", Color) = (0, 0, 0, 0)
        [NoScaleOffset]_Empty_Tex("Empty Tex", 2D) = "white" {}
        _NoiseSpeed("Noise Speed", Float) = 0.25
        _NoiseScale("Noise Scale", Float) = 3.08
        _Bloom("Bloom", Float) = 4
        [HideInInspector][NoScaleOffset]unity_Lightmaps("unity_Lightmaps", 2DArray) = "" {}
        [HideInInspector][NoScaleOffset]unity_LightmapsInd("unity_LightmapsInd", 2DArray) = "" {}
        [HideInInspector][NoScaleOffset]unity_ShadowMasks("unity_ShadowMasks", 2DArray) = "" {}
    }
    SubShader
    {
        Tags
        {
            "RenderPipeline"="UniversalPipeline"
            "RenderType"="Transparent"
            "UniversalMaterialType" = "Unlit"
            "Queue"="Transparent"
            "ShaderGraphShader"="true"
            "ShaderGraphTargetId"=""
        }
        Pass
        {
            Name "Sprite Unlit"
            Tags
            {
                "LightMode" = "Universal2D"
            }
        
            // Render State
            Cull Off
        Blend SrcAlpha OneMinusSrcAlpha, One OneMinusSrcAlpha
        ZTest LEqual
        ZWrite Off
        
            // Debug
            // <None>
        
            // --------------------------------------------------
            // Pass
        
            HLSLPROGRAM
        
            // Pragmas
            #pragma target 2.0
        #pragma exclude_renderers d3d11_9x
        #pragma vertex vert
        #pragma fragment frag
        
            // DotsInstancingOptions: <None>
            // HybridV1InjectedBuiltinProperties: <None>
        
            // Keywords
            #pragma multi_compile_fragment _ DEBUG_DISPLAY
            // GraphKeywords: <None>
        
            // Defines
            #define _SURFACE_TYPE_TRANSPARENT 1
            #define ATTRIBUTES_NEED_NORMAL
            #define ATTRIBUTES_NEED_TANGENT
            #define ATTRIBUTES_NEED_TEXCOORD0
            #define ATTRIBUTES_NEED_COLOR
            #define VARYINGS_NEED_POSITION_WS
            #define VARYINGS_NEED_TEXCOORD0
            #define VARYINGS_NEED_COLOR
            #define FEATURES_GRAPH_VERTEX
            /* WARNING: $splice Could not find named fragment 'PassInstancing' */
            #define SHADERPASS SHADERPASS_SPRITEUNLIT
            /* WARNING: $splice Could not find named fragment 'DotsInstancingVars' */
        
            // Includes
            /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPreInclude' */
        
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"
        
            // --------------------------------------------------
            // Structs and Packing
        
            /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPrePacking' */
        
            struct Attributes
        {
             float3 positionOS : POSITION;
             float3 normalOS : NORMAL;
             float4 tangentOS : TANGENT;
             float4 uv0 : TEXCOORD0;
             float4 color : COLOR;
            #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : INSTANCEID_SEMANTIC;
            #endif
        };
        struct Varyings
        {
             float4 positionCS : SV_POSITION;
             float3 positionWS;
             float4 texCoord0;
             float4 color;
            #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
             uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
             uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
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
             float3 ObjectSpaceNormal;
             float3 ObjectSpaceTangent;
             float3 ObjectSpacePosition;
        };
        struct PackedVaryings
        {
             float4 positionCS : SV_POSITION;
             float3 interp0 : INTERP0;
             float4 interp1 : INTERP1;
             float4 interp2 : INTERP2;
            #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
             uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
             uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
             FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };
        
            PackedVaryings PackVaryings (Varyings input)
        {
            PackedVaryings output;
            ZERO_INITIALIZE(PackedVaryings, output);
            output.positionCS = input.positionCS;
            output.interp0.xyz =  input.positionWS;
            output.interp1.xyzw =  input.texCoord0;
            output.interp2.xyzw =  input.color;
            #if UNITY_ANY_INSTANCING_ENABLED
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
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
            output.positionWS = input.interp0.xyz;
            output.texCoord0 = input.interp1.xyzw;
            output.color = input.interp2.xyzw;
            #if UNITY_ANY_INSTANCING_ENABLED
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
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
        float _Fill;
        float _Radius;
        float4 _MainTex_TexelSize;
        float4 _EmptyColor;
        float4 _Color;
        float4 _Empty_Tex_TexelSize;
        float _NoiseSpeed;
        float _NoiseScale;
        float _Bloom;
        CBUFFER_END
        
        // Object and Global properties
        SAMPLER(SamplerState_Linear_Repeat);
        TEXTURE2D(_MainTex);
        SAMPLER(sampler_MainTex);
        TEXTURE2D(_Empty_Tex);
        SAMPLER(sampler_Empty_Tex);
        
            // Graph Includes
            #include "Packages/com.jimmycushnie.noisynodes/NoiseShader/HLSL/ClassicNoise3D.hlsl"
        
            // -- Property used by ScenePickingPass
            #ifdef SCENEPICKINGPASS
            float4 _SelectionID;
            #endif
        
            // -- Properties used by SceneSelectionPass
            #ifdef SCENESELECTIONPASS
            int _ObjectId;
            int _PassValue;
            #endif
        
            // Graph Functions
            
        void Unity_Comparison_GreaterOrEqual_float(float A, float B, out float Out)
        {
            Out = A >= B ? 1 : 0;
        }
        
        void Unity_Branch_float(float Predicate, float True, float False, out float Out)
        {
            Out = Predicate ? True : False;
        }
        
        void Unity_OneMinus_float(float In, out float Out)
        {
            Out = 1 - In;
        }
        
        void Unity_Multiply_float_float(float A, float B, out float Out)
        {
            Out = A * B;
        }
        
        void Unity_Add_float(float A, float B, out float Out)
        {
            Out = A + B;
        }
        
        void Unity_Rotate_Degrees_float(float2 UV, float2 Center, float Rotation, out float2 Out)
        {
            //rotation matrix
            Rotation = Rotation * (3.1415926f/180.0f);
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
        
        void Unity_Step_float(float Edge, float In, out float Out)
        {
            Out = step(Edge, In);
        }
        
        void Unity_Saturate_float(float In, out float Out)
        {
            Out = saturate(In);
        }
        
        void Unity_Add_float2(float2 A, float2 B, out float2 Out)
        {
            Out = A + B;
        }
        
        void Unity_Multiply_float2_float2(float2 A, float2 B, out float2 Out)
        {
            Out = A * B;
        }
        
        void Unity_Multiply_float4_float4(float4 A, float4 B, out float4 Out)
        {
            Out = A * B;
        }
        
        void Unity_Add_float4(float4 A, float4 B, out float4 Out)
        {
            Out = A + B;
        }
        
        void Unity_Add_float3(float3 A, float3 B, out float3 Out)
        {
            Out = A + B;
        }
        
        void Unity_Multiply_float3_float3(float3 A, float3 B, out float3 Out)
        {
        Out = A * B;
        }
        
        void Unity_Remap_float(float In, float2 InMinMax, float2 OutMinMax, out float Out)
        {
            Out = OutMinMax.x + (In - InMinMax.x) * (OutMinMax.y - OutMinMax.x) / (InMinMax.y - InMinMax.x);
        }
        
        struct Bindings_Perlinnoise3D_a9d0e810228171349a3ac07147d8e5a8_float
        {
        };
        
        void SG_Perlinnoise3D_a9d0e810228171349a3ac07147d8e5a8_float(float3 Vector3_7940555B, float Vector1_1B8B9078, Bindings_Perlinnoise3D_a9d0e810228171349a3ac07147d8e5a8_float IN, out float Value_0)
        {
        float3 _Property_44999cc87708de82a26b39ae1da975ec_Out_0 = Vector3_7940555B;
        float _Property_dad5add45a7fa785be976f925bc5a5da_Out_0 = Vector1_1B8B9078;
        float3 _Multiply_1d17f1db9ddb2d8481679237f2442ac2_Out_2;
        Unity_Multiply_float3_float3(_Property_44999cc87708de82a26b39ae1da975ec_Out_0, (_Property_dad5add45a7fa785be976f925bc5a5da_Out_0.xxx), _Multiply_1d17f1db9ddb2d8481679237f2442ac2_Out_2);
        float _PerlinNoise3DCustomFunction_1d714aea6ba122808f5efcabfce18252_Out_1;
        PerlinNoise3D_float(_Multiply_1d17f1db9ddb2d8481679237f2442ac2_Out_2, _PerlinNoise3DCustomFunction_1d714aea6ba122808f5efcabfce18252_Out_1);
        float _Remap_af84172fa44e378facaf1384fe5d8f4d_Out_3;
        Unity_Remap_float(_PerlinNoise3DCustomFunction_1d714aea6ba122808f5efcabfce18252_Out_1, float2 (-1.15, 1.15), float2 (0, 1), _Remap_af84172fa44e378facaf1384fe5d8f4d_Out_3);
        Value_0 = _Remap_af84172fa44e378facaf1384fe5d8f4d_Out_3;
        }
        
            /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPreVertex' */
        
            // Graph Vertex
            struct VertexDescription
        {
            float3 Position;
            float3 Normal;
            float3 Tangent;
        };
        
        VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
        {
            VertexDescription description = (VertexDescription)0;
            description.Position = IN.ObjectSpacePosition;
            description.Normal = IN.ObjectSpaceNormal;
            description.Tangent = IN.ObjectSpaceTangent;
            return description;
        }
        
            #ifdef FEATURES_GRAPH_VERTEX
        Varyings CustomInterpolatorPassThroughFunc(inout Varyings output, VertexDescription input)
        {
        return output;
        }
        #define CUSTOMINTERPOLATOR_VARYPASSTHROUGH_FUNC
        #endif
        
            // Graph Pixel
            struct SurfaceDescription
        {
            float3 BaseColor;
            float Alpha;
        };
        
        SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
        {
            SurfaceDescription surface = (SurfaceDescription)0;
            float _Property_e8cb50a9b15344508d552688ee4bdb34_Out_0 = _Fill;
            float _Comparison_aaad254b05274d83b913f5d11794e148_Out_2;
            Unity_Comparison_GreaterOrEqual_float(_Property_e8cb50a9b15344508d552688ee4bdb34_Out_0, 0.5, _Comparison_aaad254b05274d83b913f5d11794e148_Out_2);
            float _Branch_e86959f81b164075977764051a462ffe_Out_3;
            Unity_Branch_float(_Comparison_aaad254b05274d83b913f5d11794e148_Out_2, 1, 0, _Branch_e86959f81b164075977764051a462ffe_Out_3);
            float _OneMinus_43ad10cdcb1045ae9938c3f74939af70_Out_1;
            Unity_OneMinus_float(_Branch_e86959f81b164075977764051a462ffe_Out_3, _OneMinus_43ad10cdcb1045ae9938c3f74939af70_Out_1);
            float2 _Vector2_298e76f075b14160ac3a279eaed63282_Out_0 = float2(0.5, 0.5);
            float _Property_2ffb56baa537444cb86c5a014a62d661_Out_0 = _Fill;
            float _OneMinus_a81bfa2593a042fd82835971186c7628_Out_1;
            Unity_OneMinus_float(_Property_2ffb56baa537444cb86c5a014a62d661_Out_0, _OneMinus_a81bfa2593a042fd82835971186c7628_Out_1);
            float _Multiply_4ac41190b7234643810310516d25525d_Out_2;
            Unity_Multiply_float_float(_OneMinus_a81bfa2593a042fd82835971186c7628_Out_1, 360, _Multiply_4ac41190b7234643810310516d25525d_Out_2);
            float _Add_4218b2e119834bf4b7ea718a09e0bc70_Out_2;
            Unity_Add_float(_Multiply_4ac41190b7234643810310516d25525d_Out_2, -90, _Add_4218b2e119834bf4b7ea718a09e0bc70_Out_2);
            float2 _Rotate_4deb61d059f0450386177b917dbb5917_Out_3;
            Unity_Rotate_Degrees_float(IN.uv0.xy, _Vector2_298e76f075b14160ac3a279eaed63282_Out_0, _Add_4218b2e119834bf4b7ea718a09e0bc70_Out_2, _Rotate_4deb61d059f0450386177b917dbb5917_Out_3);
            float _Split_7de377ec2ebb42f8bea18f90a9ea63e3_R_1 = _Rotate_4deb61d059f0450386177b917dbb5917_Out_3[0];
            float _Split_7de377ec2ebb42f8bea18f90a9ea63e3_G_2 = _Rotate_4deb61d059f0450386177b917dbb5917_Out_3[1];
            float _Split_7de377ec2ebb42f8bea18f90a9ea63e3_B_3 = 0;
            float _Split_7de377ec2ebb42f8bea18f90a9ea63e3_A_4 = 0;
            float _Step_f073c4123d604162acd6d3836365c397_Out_2;
            Unity_Step_float(_Split_7de377ec2ebb42f8bea18f90a9ea63e3_R_1, 0.5, _Step_f073c4123d604162acd6d3836365c397_Out_2);
            float4 _UV_731b573185b243d598936f418be86070_Out_0 = IN.uv0;
            float _Split_7ba1400ea4d54a4fb5cb8e274c6969e2_R_1 = _UV_731b573185b243d598936f418be86070_Out_0[0];
            float _Split_7ba1400ea4d54a4fb5cb8e274c6969e2_G_2 = _UV_731b573185b243d598936f418be86070_Out_0[1];
            float _Split_7ba1400ea4d54a4fb5cb8e274c6969e2_B_3 = _UV_731b573185b243d598936f418be86070_Out_0[2];
            float _Split_7ba1400ea4d54a4fb5cb8e274c6969e2_A_4 = _UV_731b573185b243d598936f418be86070_Out_0[3];
            float _Step_d4c4c3a8605d4fe38123baadd5469e88_Out_2;
            Unity_Step_float(_Split_7ba1400ea4d54a4fb5cb8e274c6969e2_G_2, 0.5, _Step_d4c4c3a8605d4fe38123baadd5469e88_Out_2);
            float _Multiply_2ef5ef9f6396455faa1ec942a867d74b_Out_2;
            Unity_Multiply_float_float(_Step_f073c4123d604162acd6d3836365c397_Out_2, _Step_d4c4c3a8605d4fe38123baadd5469e88_Out_2, _Multiply_2ef5ef9f6396455faa1ec942a867d74b_Out_2);
            float _Multiply_0bed9e24c12b47d88cad217ea2d69f64_Out_2;
            Unity_Multiply_float_float(_OneMinus_43ad10cdcb1045ae9938c3f74939af70_Out_1, _Multiply_2ef5ef9f6396455faa1ec942a867d74b_Out_2, _Multiply_0bed9e24c12b47d88cad217ea2d69f64_Out_2);
            float _Add_a9a75ddab1a049d9bad2cc6b20edce7d_Out_2;
            Unity_Add_float(_Step_f073c4123d604162acd6d3836365c397_Out_2, _Step_d4c4c3a8605d4fe38123baadd5469e88_Out_2, _Add_a9a75ddab1a049d9bad2cc6b20edce7d_Out_2);
            float _Saturate_7fce64b5685e43aab0dba63e09581448_Out_1;
            Unity_Saturate_float(_Add_a9a75ddab1a049d9bad2cc6b20edce7d_Out_2, _Saturate_7fce64b5685e43aab0dba63e09581448_Out_1);
            float _Multiply_7972fe66b5324125a53ddc59a842ab44_Out_2;
            Unity_Multiply_float_float(_Saturate_7fce64b5685e43aab0dba63e09581448_Out_1, _Branch_e86959f81b164075977764051a462ffe_Out_3, _Multiply_7972fe66b5324125a53ddc59a842ab44_Out_2);
            float _Add_78dfd349d36d4620951b68532f84062d_Out_2;
            Unity_Add_float(_Multiply_0bed9e24c12b47d88cad217ea2d69f64_Out_2, _Multiply_7972fe66b5324125a53ddc59a842ab44_Out_2, _Add_78dfd349d36d4620951b68532f84062d_Out_2);
            float _Property_e45b02e562a24017b7a87a1189fe35a5_Out_0 = _Radius;
            float2 _Vector2_7e23ada0516143e98ff1d6cbfd999292_Out_0 = float2(_Split_7ba1400ea4d54a4fb5cb8e274c6969e2_R_1, _Split_7ba1400ea4d54a4fb5cb8e274c6969e2_G_2);
            float2 _Add_32259af577cf498886aa598ecd38e1e4_Out_2;
            Unity_Add_float2(_Vector2_7e23ada0516143e98ff1d6cbfd999292_Out_0, float2(-0.5, -0.5), _Add_32259af577cf498886aa598ecd38e1e4_Out_2);
            float2 _Multiply_151d9716b9c24c7bae5b399d41635747_Out_2;
            Unity_Multiply_float2_float2((_Property_e45b02e562a24017b7a87a1189fe35a5_Out_0.xx), _Add_32259af577cf498886aa598ecd38e1e4_Out_2, _Multiply_151d9716b9c24c7bae5b399d41635747_Out_2);
            float2 _Multiply_76985cf886ec4692b8ea6d73ac47620c_Out_2;
            Unity_Multiply_float2_float2(_Multiply_151d9716b9c24c7bae5b399d41635747_Out_2, _Multiply_151d9716b9c24c7bae5b399d41635747_Out_2, _Multiply_76985cf886ec4692b8ea6d73ac47620c_Out_2);
            float _Split_299fa1120e78426fb5046e093eab31fc_R_1 = _Multiply_76985cf886ec4692b8ea6d73ac47620c_Out_2[0];
            float _Split_299fa1120e78426fb5046e093eab31fc_G_2 = _Multiply_76985cf886ec4692b8ea6d73ac47620c_Out_2[1];
            float _Split_299fa1120e78426fb5046e093eab31fc_B_3 = 0;
            float _Split_299fa1120e78426fb5046e093eab31fc_A_4 = 0;
            float _Add_703cbbc3dd9f4eecbc780ad6920814ea_Out_2;
            Unity_Add_float(_Split_299fa1120e78426fb5046e093eab31fc_R_1, _Split_299fa1120e78426fb5046e093eab31fc_G_2, _Add_703cbbc3dd9f4eecbc780ad6920814ea_Out_2);
            float _OneMinus_3b290e50f079482b9272eae3b6c78f7a_Out_1;
            Unity_OneMinus_float(_Add_703cbbc3dd9f4eecbc780ad6920814ea_Out_2, _OneMinus_3b290e50f079482b9272eae3b6c78f7a_Out_1);
            float _Saturate_ed07f1686214432ebb7cfcb8279f1f33_Out_1;
            Unity_Saturate_float(_OneMinus_3b290e50f079482b9272eae3b6c78f7a_Out_1, _Saturate_ed07f1686214432ebb7cfcb8279f1f33_Out_1);
            float _Multiply_4830023b205b4902b3449031d377de44_Out_2;
            Unity_Multiply_float_float(_Add_78dfd349d36d4620951b68532f84062d_Out_2, _Saturate_ed07f1686214432ebb7cfcb8279f1f33_Out_1, _Multiply_4830023b205b4902b3449031d377de44_Out_2);
            float4 _Property_6df5d881bcc7416c9cf69296e3c0fe79_Out_0 = _Color;
            UnityTexture2D _Property_997a4b9fcf624e219b7dbc5143e5cdcc_Out_0 = UnityBuildTexture2DStructNoScale(_MainTex);
            float4 _SampleTexture2D_2c419302513e439c8a099e51822e71ed_RGBA_0 = SAMPLE_TEXTURE2D(_Property_997a4b9fcf624e219b7dbc5143e5cdcc_Out_0.tex, _Property_997a4b9fcf624e219b7dbc5143e5cdcc_Out_0.samplerstate, _Property_997a4b9fcf624e219b7dbc5143e5cdcc_Out_0.GetTransformedUV(IN.uv0.xy));
            float _SampleTexture2D_2c419302513e439c8a099e51822e71ed_R_4 = _SampleTexture2D_2c419302513e439c8a099e51822e71ed_RGBA_0.r;
            float _SampleTexture2D_2c419302513e439c8a099e51822e71ed_G_5 = _SampleTexture2D_2c419302513e439c8a099e51822e71ed_RGBA_0.g;
            float _SampleTexture2D_2c419302513e439c8a099e51822e71ed_B_6 = _SampleTexture2D_2c419302513e439c8a099e51822e71ed_RGBA_0.b;
            float _SampleTexture2D_2c419302513e439c8a099e51822e71ed_A_7 = _SampleTexture2D_2c419302513e439c8a099e51822e71ed_RGBA_0.a;
            float4 _Multiply_cb681bb10e7c4ecc9abebd7fd82a09db_Out_2;
            Unity_Multiply_float4_float4(_Property_6df5d881bcc7416c9cf69296e3c0fe79_Out_0, _SampleTexture2D_2c419302513e439c8a099e51822e71ed_RGBA_0, _Multiply_cb681bb10e7c4ecc9abebd7fd82a09db_Out_2);
            float4 _Multiply_fb18646c648b4d9385ac572a3839cc7f_Out_2;
            Unity_Multiply_float4_float4((_Multiply_4830023b205b4902b3449031d377de44_Out_2.xxxx), _Multiply_cb681bb10e7c4ecc9abebd7fd82a09db_Out_2, _Multiply_fb18646c648b4d9385ac572a3839cc7f_Out_2);
            float _Multiply_060926eb5cb84e47ab335363f7c2fc92_Out_2;
            Unity_Multiply_float_float(_Multiply_4830023b205b4902b3449031d377de44_Out_2, -1, _Multiply_060926eb5cb84e47ab335363f7c2fc92_Out_2);
            float _Add_face73a4c9c64ab9a1849a0714ec9ebc_Out_2;
            Unity_Add_float(_Multiply_060926eb5cb84e47ab335363f7c2fc92_Out_2, _Saturate_ed07f1686214432ebb7cfcb8279f1f33_Out_1, _Add_face73a4c9c64ab9a1849a0714ec9ebc_Out_2);
            float4 _Property_e0679f3fca954d959f30fceed9b2f0ab_Out_0 = _EmptyColor;
            UnityTexture2D _Property_35a95420d8f84f608348a62170d4ba0d_Out_0 = UnityBuildTexture2DStructNoScale(_Empty_Tex);
            float4 _SampleTexture2D_5b4ee92a21b4498983942cf9d09732d3_RGBA_0 = SAMPLE_TEXTURE2D(_Property_35a95420d8f84f608348a62170d4ba0d_Out_0.tex, _Property_35a95420d8f84f608348a62170d4ba0d_Out_0.samplerstate, _Property_35a95420d8f84f608348a62170d4ba0d_Out_0.GetTransformedUV(IN.uv0.xy));
            float _SampleTexture2D_5b4ee92a21b4498983942cf9d09732d3_R_4 = _SampleTexture2D_5b4ee92a21b4498983942cf9d09732d3_RGBA_0.r;
            float _SampleTexture2D_5b4ee92a21b4498983942cf9d09732d3_G_5 = _SampleTexture2D_5b4ee92a21b4498983942cf9d09732d3_RGBA_0.g;
            float _SampleTexture2D_5b4ee92a21b4498983942cf9d09732d3_B_6 = _SampleTexture2D_5b4ee92a21b4498983942cf9d09732d3_RGBA_0.b;
            float _SampleTexture2D_5b4ee92a21b4498983942cf9d09732d3_A_7 = _SampleTexture2D_5b4ee92a21b4498983942cf9d09732d3_RGBA_0.a;
            float4 _Multiply_5722f24b04dd469d8eadda1a6f742ed0_Out_2;
            Unity_Multiply_float4_float4(_Property_e0679f3fca954d959f30fceed9b2f0ab_Out_0, _SampleTexture2D_5b4ee92a21b4498983942cf9d09732d3_RGBA_0, _Multiply_5722f24b04dd469d8eadda1a6f742ed0_Out_2);
            float4 _Multiply_8c439f606f924d2887b4e6bdeaaa97c1_Out_2;
            Unity_Multiply_float4_float4((_Add_face73a4c9c64ab9a1849a0714ec9ebc_Out_2.xxxx), _Multiply_5722f24b04dd469d8eadda1a6f742ed0_Out_2, _Multiply_8c439f606f924d2887b4e6bdeaaa97c1_Out_2);
            float4 _Add_134caef305844e19a0e99aaa50155988_Out_2;
            Unity_Add_float4(_Multiply_fb18646c648b4d9385ac572a3839cc7f_Out_2, _Multiply_8c439f606f924d2887b4e6bdeaaa97c1_Out_2, _Add_134caef305844e19a0e99aaa50155988_Out_2);
            float _Split_2247f2cc6ef3439daef20cb6a8c44e0e_R_1 = _Add_134caef305844e19a0e99aaa50155988_Out_2[0];
            float _Split_2247f2cc6ef3439daef20cb6a8c44e0e_G_2 = _Add_134caef305844e19a0e99aaa50155988_Out_2[1];
            float _Split_2247f2cc6ef3439daef20cb6a8c44e0e_B_3 = _Add_134caef305844e19a0e99aaa50155988_Out_2[2];
            float _Split_2247f2cc6ef3439daef20cb6a8c44e0e_A_4 = _Add_134caef305844e19a0e99aaa50155988_Out_2[3];
            float _Multiply_ebc4f8fd8bd2414aa4f95a1206df1d9d_Out_2;
            Unity_Multiply_float_float(_Split_2247f2cc6ef3439daef20cb6a8c44e0e_G_2, _Saturate_ed07f1686214432ebb7cfcb8279f1f33_Out_1, _Multiply_ebc4f8fd8bd2414aa4f95a1206df1d9d_Out_2);
            float _Property_93e16b605d3d4a61b92cad2a2390ae1e_Out_0 = _NoiseSpeed;
            float _Multiply_7e6a5129be3145efb21c988db7610b82_Out_2;
            Unity_Multiply_float_float(_Property_93e16b605d3d4a61b92cad2a2390ae1e_Out_0, IN.TimeParameters.x, _Multiply_7e6a5129be3145efb21c988db7610b82_Out_2);
            float3 _Vector3_b0aacf08d93e4c5b9b097f4d989124c9_Out_0 = float3(0, 0, _Multiply_7e6a5129be3145efb21c988db7610b82_Out_2);
            float4 _UV_44f7483fd4e840bdb439894d36780fda_Out_0 = IN.uv0;
            float3 _Add_c1d447ed7a0a43e3a57845df7c6c0708_Out_2;
            Unity_Add_float3(_Vector3_b0aacf08d93e4c5b9b097f4d989124c9_Out_0, (_UV_44f7483fd4e840bdb439894d36780fda_Out_0.xyz), _Add_c1d447ed7a0a43e3a57845df7c6c0708_Out_2);
            float _Property_2aeca7f5d1964627bba569224e26e93b_Out_0 = _NoiseScale;
            Bindings_Perlinnoise3D_a9d0e810228171349a3ac07147d8e5a8_float _Perlinnoise3D_9d6c435e6669424199f891ea116ff44d;
            float _Perlinnoise3D_9d6c435e6669424199f891ea116ff44d_Value_0;
            SG_Perlinnoise3D_a9d0e810228171349a3ac07147d8e5a8_float(_Add_c1d447ed7a0a43e3a57845df7c6c0708_Out_2, _Property_2aeca7f5d1964627bba569224e26e93b_Out_0, _Perlinnoise3D_9d6c435e6669424199f891ea116ff44d, _Perlinnoise3D_9d6c435e6669424199f891ea116ff44d_Value_0);
            float _Multiply_95bf499cef1647aa87f2ef1406572a69_Out_2;
            Unity_Multiply_float_float(_Multiply_ebc4f8fd8bd2414aa4f95a1206df1d9d_Out_2, _Perlinnoise3D_9d6c435e6669424199f891ea116ff44d_Value_0, _Multiply_95bf499cef1647aa87f2ef1406572a69_Out_2);
            float _Property_a479584411884039a90f17f2a024efbd_Out_0 = _Bloom;
            float _Multiply_7da2e1d7ffff4679b40e230759c44972_Out_2;
            Unity_Multiply_float_float(_Multiply_95bf499cef1647aa87f2ef1406572a69_Out_2, _Property_a479584411884039a90f17f2a024efbd_Out_0, _Multiply_7da2e1d7ffff4679b40e230759c44972_Out_2);
            surface.BaseColor = (_Add_134caef305844e19a0e99aaa50155988_Out_2.xyz);
            surface.Alpha = _Multiply_7da2e1d7ffff4679b40e230759c44972_Out_2;
            return surface;
        }
        
            // --------------------------------------------------
            // Build Graph Inputs
        
            VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
        {
            VertexDescriptionInputs output;
            ZERO_INITIALIZE(VertexDescriptionInputs, output);
        
            output.ObjectSpaceNormal =                          input.normalOS;
            output.ObjectSpaceTangent =                         input.tangentOS.xyz;
            output.ObjectSpacePosition =                        input.positionOS;
        
            return output;
        }
            SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
        {
            SurfaceDescriptionInputs output;
            ZERO_INITIALIZE(SurfaceDescriptionInputs, output);
        
            
        
        
        
        
        
            output.uv0 =                                        input.texCoord0;
            output.TimeParameters =                             _TimeParameters.xyz; // This is mainly for LW as HD overwrite this value
        #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN                output.FaceSign =                                   IS_FRONT_VFACE(input.cullFace, true, false);
        #else
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
        #endif
        #undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
        
            return output;
        }
        
            // --------------------------------------------------
            // Main
        
            #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/2D/ShaderGraph/Includes/SpriteUnlitPass.hlsl"
        
            ENDHLSL
        }
        Pass
        {
            Name "Sprite Unlit"
            Tags
            {
                "LightMode" = "UniversalForward"
            }
        
            // Render State
            Cull Off
        Blend SrcAlpha OneMinusSrcAlpha, One OneMinusSrcAlpha
        ZTest LEqual
        ZWrite Off
        
            // Debug
            // <None>
        
            // --------------------------------------------------
            // Pass
        
            HLSLPROGRAM
        
            // Pragmas
            #pragma target 2.0
        #pragma exclude_renderers d3d11_9x
        #pragma vertex vert
        #pragma fragment frag
        
            // DotsInstancingOptions: <None>
            // HybridV1InjectedBuiltinProperties: <None>
        
            // Keywords
            #pragma multi_compile_fragment _ DEBUG_DISPLAY
            // GraphKeywords: <None>
        
            // Defines
            #define _SURFACE_TYPE_TRANSPARENT 1
            #define ATTRIBUTES_NEED_NORMAL
            #define ATTRIBUTES_NEED_TANGENT
            #define ATTRIBUTES_NEED_TEXCOORD0
            #define ATTRIBUTES_NEED_COLOR
            #define VARYINGS_NEED_POSITION_WS
            #define VARYINGS_NEED_TEXCOORD0
            #define VARYINGS_NEED_COLOR
            #define FEATURES_GRAPH_VERTEX
            /* WARNING: $splice Could not find named fragment 'PassInstancing' */
            #define SHADERPASS SHADERPASS_SPRITEFORWARD
            /* WARNING: $splice Could not find named fragment 'DotsInstancingVars' */
        
            // Includes
            /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPreInclude' */
        
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"
        
            // --------------------------------------------------
            // Structs and Packing
        
            /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPrePacking' */
        
            struct Attributes
        {
             float3 positionOS : POSITION;
             float3 normalOS : NORMAL;
             float4 tangentOS : TANGENT;
             float4 uv0 : TEXCOORD0;
             float4 color : COLOR;
            #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : INSTANCEID_SEMANTIC;
            #endif
        };
        struct Varyings
        {
             float4 positionCS : SV_POSITION;
             float3 positionWS;
             float4 texCoord0;
             float4 color;
            #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
             uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
             uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
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
             float3 ObjectSpaceNormal;
             float3 ObjectSpaceTangent;
             float3 ObjectSpacePosition;
        };
        struct PackedVaryings
        {
             float4 positionCS : SV_POSITION;
             float3 interp0 : INTERP0;
             float4 interp1 : INTERP1;
             float4 interp2 : INTERP2;
            #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
             uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
             uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
             FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };
        
            PackedVaryings PackVaryings (Varyings input)
        {
            PackedVaryings output;
            ZERO_INITIALIZE(PackedVaryings, output);
            output.positionCS = input.positionCS;
            output.interp0.xyz =  input.positionWS;
            output.interp1.xyzw =  input.texCoord0;
            output.interp2.xyzw =  input.color;
            #if UNITY_ANY_INSTANCING_ENABLED
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
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
            output.positionWS = input.interp0.xyz;
            output.texCoord0 = input.interp1.xyzw;
            output.color = input.interp2.xyzw;
            #if UNITY_ANY_INSTANCING_ENABLED
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
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
        float _Fill;
        float _Radius;
        float4 _MainTex_TexelSize;
        float4 _EmptyColor;
        float4 _Color;
        float4 _Empty_Tex_TexelSize;
        float _NoiseSpeed;
        float _NoiseScale;
        float _Bloom;
        CBUFFER_END
        
        // Object and Global properties
        SAMPLER(SamplerState_Linear_Repeat);
        TEXTURE2D(_MainTex);
        SAMPLER(sampler_MainTex);
        TEXTURE2D(_Empty_Tex);
        SAMPLER(sampler_Empty_Tex);
        
            // Graph Includes
            #include "Packages/com.jimmycushnie.noisynodes/NoiseShader/HLSL/ClassicNoise3D.hlsl"
        
            // -- Property used by ScenePickingPass
            #ifdef SCENEPICKINGPASS
            float4 _SelectionID;
            #endif
        
            // -- Properties used by SceneSelectionPass
            #ifdef SCENESELECTIONPASS
            int _ObjectId;
            int _PassValue;
            #endif
        
            // Graph Functions
            
        void Unity_Comparison_GreaterOrEqual_float(float A, float B, out float Out)
        {
            Out = A >= B ? 1 : 0;
        }
        
        void Unity_Branch_float(float Predicate, float True, float False, out float Out)
        {
            Out = Predicate ? True : False;
        }
        
        void Unity_OneMinus_float(float In, out float Out)
        {
            Out = 1 - In;
        }
        
        void Unity_Multiply_float_float(float A, float B, out float Out)
        {
            Out = A * B;
        }
        
        void Unity_Add_float(float A, float B, out float Out)
        {
            Out = A + B;
        }
        
        void Unity_Rotate_Degrees_float(float2 UV, float2 Center, float Rotation, out float2 Out)
        {
            //rotation matrix
            Rotation = Rotation * (3.1415926f/180.0f);
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
        
        void Unity_Step_float(float Edge, float In, out float Out)
        {
            Out = step(Edge, In);
        }
        
        void Unity_Saturate_float(float In, out float Out)
        {
            Out = saturate(In);
        }
        
        void Unity_Add_float2(float2 A, float2 B, out float2 Out)
        {
            Out = A + B;
        }
        
        void Unity_Multiply_float2_float2(float2 A, float2 B, out float2 Out)
        {
            Out = A * B;
        }
        
        void Unity_Multiply_float4_float4(float4 A, float4 B, out float4 Out)
        {
            Out = A * B;
        }
        
        void Unity_Add_float4(float4 A, float4 B, out float4 Out)
        {
            Out = A + B;
        }
        
        void Unity_Add_float3(float3 A, float3 B, out float3 Out)
        {
            Out = A + B;
        }
        
        void Unity_Multiply_float3_float3(float3 A, float3 B, out float3 Out)
        {
        Out = A * B;
        }
        
        void Unity_Remap_float(float In, float2 InMinMax, float2 OutMinMax, out float Out)
        {
            Out = OutMinMax.x + (In - InMinMax.x) * (OutMinMax.y - OutMinMax.x) / (InMinMax.y - InMinMax.x);
        }
        
        struct Bindings_Perlinnoise3D_a9d0e810228171349a3ac07147d8e5a8_float
        {
        };
        
        void SG_Perlinnoise3D_a9d0e810228171349a3ac07147d8e5a8_float(float3 Vector3_7940555B, float Vector1_1B8B9078, Bindings_Perlinnoise3D_a9d0e810228171349a3ac07147d8e5a8_float IN, out float Value_0)
        {
        float3 _Property_44999cc87708de82a26b39ae1da975ec_Out_0 = Vector3_7940555B;
        float _Property_dad5add45a7fa785be976f925bc5a5da_Out_0 = Vector1_1B8B9078;
        float3 _Multiply_1d17f1db9ddb2d8481679237f2442ac2_Out_2;
        Unity_Multiply_float3_float3(_Property_44999cc87708de82a26b39ae1da975ec_Out_0, (_Property_dad5add45a7fa785be976f925bc5a5da_Out_0.xxx), _Multiply_1d17f1db9ddb2d8481679237f2442ac2_Out_2);
        float _PerlinNoise3DCustomFunction_1d714aea6ba122808f5efcabfce18252_Out_1;
        PerlinNoise3D_float(_Multiply_1d17f1db9ddb2d8481679237f2442ac2_Out_2, _PerlinNoise3DCustomFunction_1d714aea6ba122808f5efcabfce18252_Out_1);
        float _Remap_af84172fa44e378facaf1384fe5d8f4d_Out_3;
        Unity_Remap_float(_PerlinNoise3DCustomFunction_1d714aea6ba122808f5efcabfce18252_Out_1, float2 (-1.15, 1.15), float2 (0, 1), _Remap_af84172fa44e378facaf1384fe5d8f4d_Out_3);
        Value_0 = _Remap_af84172fa44e378facaf1384fe5d8f4d_Out_3;
        }
        
            /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPreVertex' */
        
            // Graph Vertex
            struct VertexDescription
        {
            float3 Position;
            float3 Normal;
            float3 Tangent;
        };
        
        VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
        {
            VertexDescription description = (VertexDescription)0;
            description.Position = IN.ObjectSpacePosition;
            description.Normal = IN.ObjectSpaceNormal;
            description.Tangent = IN.ObjectSpaceTangent;
            return description;
        }
        
            #ifdef FEATURES_GRAPH_VERTEX
        Varyings CustomInterpolatorPassThroughFunc(inout Varyings output, VertexDescription input)
        {
        return output;
        }
        #define CUSTOMINTERPOLATOR_VARYPASSTHROUGH_FUNC
        #endif
        
            // Graph Pixel
            struct SurfaceDescription
        {
            float3 BaseColor;
            float Alpha;
        };
        
        SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
        {
            SurfaceDescription surface = (SurfaceDescription)0;
            float _Property_e8cb50a9b15344508d552688ee4bdb34_Out_0 = _Fill;
            float _Comparison_aaad254b05274d83b913f5d11794e148_Out_2;
            Unity_Comparison_GreaterOrEqual_float(_Property_e8cb50a9b15344508d552688ee4bdb34_Out_0, 0.5, _Comparison_aaad254b05274d83b913f5d11794e148_Out_2);
            float _Branch_e86959f81b164075977764051a462ffe_Out_3;
            Unity_Branch_float(_Comparison_aaad254b05274d83b913f5d11794e148_Out_2, 1, 0, _Branch_e86959f81b164075977764051a462ffe_Out_3);
            float _OneMinus_43ad10cdcb1045ae9938c3f74939af70_Out_1;
            Unity_OneMinus_float(_Branch_e86959f81b164075977764051a462ffe_Out_3, _OneMinus_43ad10cdcb1045ae9938c3f74939af70_Out_1);
            float2 _Vector2_298e76f075b14160ac3a279eaed63282_Out_0 = float2(0.5, 0.5);
            float _Property_2ffb56baa537444cb86c5a014a62d661_Out_0 = _Fill;
            float _OneMinus_a81bfa2593a042fd82835971186c7628_Out_1;
            Unity_OneMinus_float(_Property_2ffb56baa537444cb86c5a014a62d661_Out_0, _OneMinus_a81bfa2593a042fd82835971186c7628_Out_1);
            float _Multiply_4ac41190b7234643810310516d25525d_Out_2;
            Unity_Multiply_float_float(_OneMinus_a81bfa2593a042fd82835971186c7628_Out_1, 360, _Multiply_4ac41190b7234643810310516d25525d_Out_2);
            float _Add_4218b2e119834bf4b7ea718a09e0bc70_Out_2;
            Unity_Add_float(_Multiply_4ac41190b7234643810310516d25525d_Out_2, -90, _Add_4218b2e119834bf4b7ea718a09e0bc70_Out_2);
            float2 _Rotate_4deb61d059f0450386177b917dbb5917_Out_3;
            Unity_Rotate_Degrees_float(IN.uv0.xy, _Vector2_298e76f075b14160ac3a279eaed63282_Out_0, _Add_4218b2e119834bf4b7ea718a09e0bc70_Out_2, _Rotate_4deb61d059f0450386177b917dbb5917_Out_3);
            float _Split_7de377ec2ebb42f8bea18f90a9ea63e3_R_1 = _Rotate_4deb61d059f0450386177b917dbb5917_Out_3[0];
            float _Split_7de377ec2ebb42f8bea18f90a9ea63e3_G_2 = _Rotate_4deb61d059f0450386177b917dbb5917_Out_3[1];
            float _Split_7de377ec2ebb42f8bea18f90a9ea63e3_B_3 = 0;
            float _Split_7de377ec2ebb42f8bea18f90a9ea63e3_A_4 = 0;
            float _Step_f073c4123d604162acd6d3836365c397_Out_2;
            Unity_Step_float(_Split_7de377ec2ebb42f8bea18f90a9ea63e3_R_1, 0.5, _Step_f073c4123d604162acd6d3836365c397_Out_2);
            float4 _UV_731b573185b243d598936f418be86070_Out_0 = IN.uv0;
            float _Split_7ba1400ea4d54a4fb5cb8e274c6969e2_R_1 = _UV_731b573185b243d598936f418be86070_Out_0[0];
            float _Split_7ba1400ea4d54a4fb5cb8e274c6969e2_G_2 = _UV_731b573185b243d598936f418be86070_Out_0[1];
            float _Split_7ba1400ea4d54a4fb5cb8e274c6969e2_B_3 = _UV_731b573185b243d598936f418be86070_Out_0[2];
            float _Split_7ba1400ea4d54a4fb5cb8e274c6969e2_A_4 = _UV_731b573185b243d598936f418be86070_Out_0[3];
            float _Step_d4c4c3a8605d4fe38123baadd5469e88_Out_2;
            Unity_Step_float(_Split_7ba1400ea4d54a4fb5cb8e274c6969e2_G_2, 0.5, _Step_d4c4c3a8605d4fe38123baadd5469e88_Out_2);
            float _Multiply_2ef5ef9f6396455faa1ec942a867d74b_Out_2;
            Unity_Multiply_float_float(_Step_f073c4123d604162acd6d3836365c397_Out_2, _Step_d4c4c3a8605d4fe38123baadd5469e88_Out_2, _Multiply_2ef5ef9f6396455faa1ec942a867d74b_Out_2);
            float _Multiply_0bed9e24c12b47d88cad217ea2d69f64_Out_2;
            Unity_Multiply_float_float(_OneMinus_43ad10cdcb1045ae9938c3f74939af70_Out_1, _Multiply_2ef5ef9f6396455faa1ec942a867d74b_Out_2, _Multiply_0bed9e24c12b47d88cad217ea2d69f64_Out_2);
            float _Add_a9a75ddab1a049d9bad2cc6b20edce7d_Out_2;
            Unity_Add_float(_Step_f073c4123d604162acd6d3836365c397_Out_2, _Step_d4c4c3a8605d4fe38123baadd5469e88_Out_2, _Add_a9a75ddab1a049d9bad2cc6b20edce7d_Out_2);
            float _Saturate_7fce64b5685e43aab0dba63e09581448_Out_1;
            Unity_Saturate_float(_Add_a9a75ddab1a049d9bad2cc6b20edce7d_Out_2, _Saturate_7fce64b5685e43aab0dba63e09581448_Out_1);
            float _Multiply_7972fe66b5324125a53ddc59a842ab44_Out_2;
            Unity_Multiply_float_float(_Saturate_7fce64b5685e43aab0dba63e09581448_Out_1, _Branch_e86959f81b164075977764051a462ffe_Out_3, _Multiply_7972fe66b5324125a53ddc59a842ab44_Out_2);
            float _Add_78dfd349d36d4620951b68532f84062d_Out_2;
            Unity_Add_float(_Multiply_0bed9e24c12b47d88cad217ea2d69f64_Out_2, _Multiply_7972fe66b5324125a53ddc59a842ab44_Out_2, _Add_78dfd349d36d4620951b68532f84062d_Out_2);
            float _Property_e45b02e562a24017b7a87a1189fe35a5_Out_0 = _Radius;
            float2 _Vector2_7e23ada0516143e98ff1d6cbfd999292_Out_0 = float2(_Split_7ba1400ea4d54a4fb5cb8e274c6969e2_R_1, _Split_7ba1400ea4d54a4fb5cb8e274c6969e2_G_2);
            float2 _Add_32259af577cf498886aa598ecd38e1e4_Out_2;
            Unity_Add_float2(_Vector2_7e23ada0516143e98ff1d6cbfd999292_Out_0, float2(-0.5, -0.5), _Add_32259af577cf498886aa598ecd38e1e4_Out_2);
            float2 _Multiply_151d9716b9c24c7bae5b399d41635747_Out_2;
            Unity_Multiply_float2_float2((_Property_e45b02e562a24017b7a87a1189fe35a5_Out_0.xx), _Add_32259af577cf498886aa598ecd38e1e4_Out_2, _Multiply_151d9716b9c24c7bae5b399d41635747_Out_2);
            float2 _Multiply_76985cf886ec4692b8ea6d73ac47620c_Out_2;
            Unity_Multiply_float2_float2(_Multiply_151d9716b9c24c7bae5b399d41635747_Out_2, _Multiply_151d9716b9c24c7bae5b399d41635747_Out_2, _Multiply_76985cf886ec4692b8ea6d73ac47620c_Out_2);
            float _Split_299fa1120e78426fb5046e093eab31fc_R_1 = _Multiply_76985cf886ec4692b8ea6d73ac47620c_Out_2[0];
            float _Split_299fa1120e78426fb5046e093eab31fc_G_2 = _Multiply_76985cf886ec4692b8ea6d73ac47620c_Out_2[1];
            float _Split_299fa1120e78426fb5046e093eab31fc_B_3 = 0;
            float _Split_299fa1120e78426fb5046e093eab31fc_A_4 = 0;
            float _Add_703cbbc3dd9f4eecbc780ad6920814ea_Out_2;
            Unity_Add_float(_Split_299fa1120e78426fb5046e093eab31fc_R_1, _Split_299fa1120e78426fb5046e093eab31fc_G_2, _Add_703cbbc3dd9f4eecbc780ad6920814ea_Out_2);
            float _OneMinus_3b290e50f079482b9272eae3b6c78f7a_Out_1;
            Unity_OneMinus_float(_Add_703cbbc3dd9f4eecbc780ad6920814ea_Out_2, _OneMinus_3b290e50f079482b9272eae3b6c78f7a_Out_1);
            float _Saturate_ed07f1686214432ebb7cfcb8279f1f33_Out_1;
            Unity_Saturate_float(_OneMinus_3b290e50f079482b9272eae3b6c78f7a_Out_1, _Saturate_ed07f1686214432ebb7cfcb8279f1f33_Out_1);
            float _Multiply_4830023b205b4902b3449031d377de44_Out_2;
            Unity_Multiply_float_float(_Add_78dfd349d36d4620951b68532f84062d_Out_2, _Saturate_ed07f1686214432ebb7cfcb8279f1f33_Out_1, _Multiply_4830023b205b4902b3449031d377de44_Out_2);
            float4 _Property_6df5d881bcc7416c9cf69296e3c0fe79_Out_0 = _Color;
            UnityTexture2D _Property_997a4b9fcf624e219b7dbc5143e5cdcc_Out_0 = UnityBuildTexture2DStructNoScale(_MainTex);
            float4 _SampleTexture2D_2c419302513e439c8a099e51822e71ed_RGBA_0 = SAMPLE_TEXTURE2D(_Property_997a4b9fcf624e219b7dbc5143e5cdcc_Out_0.tex, _Property_997a4b9fcf624e219b7dbc5143e5cdcc_Out_0.samplerstate, _Property_997a4b9fcf624e219b7dbc5143e5cdcc_Out_0.GetTransformedUV(IN.uv0.xy));
            float _SampleTexture2D_2c419302513e439c8a099e51822e71ed_R_4 = _SampleTexture2D_2c419302513e439c8a099e51822e71ed_RGBA_0.r;
            float _SampleTexture2D_2c419302513e439c8a099e51822e71ed_G_5 = _SampleTexture2D_2c419302513e439c8a099e51822e71ed_RGBA_0.g;
            float _SampleTexture2D_2c419302513e439c8a099e51822e71ed_B_6 = _SampleTexture2D_2c419302513e439c8a099e51822e71ed_RGBA_0.b;
            float _SampleTexture2D_2c419302513e439c8a099e51822e71ed_A_7 = _SampleTexture2D_2c419302513e439c8a099e51822e71ed_RGBA_0.a;
            float4 _Multiply_cb681bb10e7c4ecc9abebd7fd82a09db_Out_2;
            Unity_Multiply_float4_float4(_Property_6df5d881bcc7416c9cf69296e3c0fe79_Out_0, _SampleTexture2D_2c419302513e439c8a099e51822e71ed_RGBA_0, _Multiply_cb681bb10e7c4ecc9abebd7fd82a09db_Out_2);
            float4 _Multiply_fb18646c648b4d9385ac572a3839cc7f_Out_2;
            Unity_Multiply_float4_float4((_Multiply_4830023b205b4902b3449031d377de44_Out_2.xxxx), _Multiply_cb681bb10e7c4ecc9abebd7fd82a09db_Out_2, _Multiply_fb18646c648b4d9385ac572a3839cc7f_Out_2);
            float _Multiply_060926eb5cb84e47ab335363f7c2fc92_Out_2;
            Unity_Multiply_float_float(_Multiply_4830023b205b4902b3449031d377de44_Out_2, -1, _Multiply_060926eb5cb84e47ab335363f7c2fc92_Out_2);
            float _Add_face73a4c9c64ab9a1849a0714ec9ebc_Out_2;
            Unity_Add_float(_Multiply_060926eb5cb84e47ab335363f7c2fc92_Out_2, _Saturate_ed07f1686214432ebb7cfcb8279f1f33_Out_1, _Add_face73a4c9c64ab9a1849a0714ec9ebc_Out_2);
            float4 _Property_e0679f3fca954d959f30fceed9b2f0ab_Out_0 = _EmptyColor;
            UnityTexture2D _Property_35a95420d8f84f608348a62170d4ba0d_Out_0 = UnityBuildTexture2DStructNoScale(_Empty_Tex);
            float4 _SampleTexture2D_5b4ee92a21b4498983942cf9d09732d3_RGBA_0 = SAMPLE_TEXTURE2D(_Property_35a95420d8f84f608348a62170d4ba0d_Out_0.tex, _Property_35a95420d8f84f608348a62170d4ba0d_Out_0.samplerstate, _Property_35a95420d8f84f608348a62170d4ba0d_Out_0.GetTransformedUV(IN.uv0.xy));
            float _SampleTexture2D_5b4ee92a21b4498983942cf9d09732d3_R_4 = _SampleTexture2D_5b4ee92a21b4498983942cf9d09732d3_RGBA_0.r;
            float _SampleTexture2D_5b4ee92a21b4498983942cf9d09732d3_G_5 = _SampleTexture2D_5b4ee92a21b4498983942cf9d09732d3_RGBA_0.g;
            float _SampleTexture2D_5b4ee92a21b4498983942cf9d09732d3_B_6 = _SampleTexture2D_5b4ee92a21b4498983942cf9d09732d3_RGBA_0.b;
            float _SampleTexture2D_5b4ee92a21b4498983942cf9d09732d3_A_7 = _SampleTexture2D_5b4ee92a21b4498983942cf9d09732d3_RGBA_0.a;
            float4 _Multiply_5722f24b04dd469d8eadda1a6f742ed0_Out_2;
            Unity_Multiply_float4_float4(_Property_e0679f3fca954d959f30fceed9b2f0ab_Out_0, _SampleTexture2D_5b4ee92a21b4498983942cf9d09732d3_RGBA_0, _Multiply_5722f24b04dd469d8eadda1a6f742ed0_Out_2);
            float4 _Multiply_8c439f606f924d2887b4e6bdeaaa97c1_Out_2;
            Unity_Multiply_float4_float4((_Add_face73a4c9c64ab9a1849a0714ec9ebc_Out_2.xxxx), _Multiply_5722f24b04dd469d8eadda1a6f742ed0_Out_2, _Multiply_8c439f606f924d2887b4e6bdeaaa97c1_Out_2);
            float4 _Add_134caef305844e19a0e99aaa50155988_Out_2;
            Unity_Add_float4(_Multiply_fb18646c648b4d9385ac572a3839cc7f_Out_2, _Multiply_8c439f606f924d2887b4e6bdeaaa97c1_Out_2, _Add_134caef305844e19a0e99aaa50155988_Out_2);
            float _Split_2247f2cc6ef3439daef20cb6a8c44e0e_R_1 = _Add_134caef305844e19a0e99aaa50155988_Out_2[0];
            float _Split_2247f2cc6ef3439daef20cb6a8c44e0e_G_2 = _Add_134caef305844e19a0e99aaa50155988_Out_2[1];
            float _Split_2247f2cc6ef3439daef20cb6a8c44e0e_B_3 = _Add_134caef305844e19a0e99aaa50155988_Out_2[2];
            float _Split_2247f2cc6ef3439daef20cb6a8c44e0e_A_4 = _Add_134caef305844e19a0e99aaa50155988_Out_2[3];
            float _Multiply_ebc4f8fd8bd2414aa4f95a1206df1d9d_Out_2;
            Unity_Multiply_float_float(_Split_2247f2cc6ef3439daef20cb6a8c44e0e_G_2, _Saturate_ed07f1686214432ebb7cfcb8279f1f33_Out_1, _Multiply_ebc4f8fd8bd2414aa4f95a1206df1d9d_Out_2);
            float _Property_93e16b605d3d4a61b92cad2a2390ae1e_Out_0 = _NoiseSpeed;
            float _Multiply_7e6a5129be3145efb21c988db7610b82_Out_2;
            Unity_Multiply_float_float(_Property_93e16b605d3d4a61b92cad2a2390ae1e_Out_0, IN.TimeParameters.x, _Multiply_7e6a5129be3145efb21c988db7610b82_Out_2);
            float3 _Vector3_b0aacf08d93e4c5b9b097f4d989124c9_Out_0 = float3(0, 0, _Multiply_7e6a5129be3145efb21c988db7610b82_Out_2);
            float4 _UV_44f7483fd4e840bdb439894d36780fda_Out_0 = IN.uv0;
            float3 _Add_c1d447ed7a0a43e3a57845df7c6c0708_Out_2;
            Unity_Add_float3(_Vector3_b0aacf08d93e4c5b9b097f4d989124c9_Out_0, (_UV_44f7483fd4e840bdb439894d36780fda_Out_0.xyz), _Add_c1d447ed7a0a43e3a57845df7c6c0708_Out_2);
            float _Property_2aeca7f5d1964627bba569224e26e93b_Out_0 = _NoiseScale;
            Bindings_Perlinnoise3D_a9d0e810228171349a3ac07147d8e5a8_float _Perlinnoise3D_9d6c435e6669424199f891ea116ff44d;
            float _Perlinnoise3D_9d6c435e6669424199f891ea116ff44d_Value_0;
            SG_Perlinnoise3D_a9d0e810228171349a3ac07147d8e5a8_float(_Add_c1d447ed7a0a43e3a57845df7c6c0708_Out_2, _Property_2aeca7f5d1964627bba569224e26e93b_Out_0, _Perlinnoise3D_9d6c435e6669424199f891ea116ff44d, _Perlinnoise3D_9d6c435e6669424199f891ea116ff44d_Value_0);
            float _Multiply_95bf499cef1647aa87f2ef1406572a69_Out_2;
            Unity_Multiply_float_float(_Multiply_ebc4f8fd8bd2414aa4f95a1206df1d9d_Out_2, _Perlinnoise3D_9d6c435e6669424199f891ea116ff44d_Value_0, _Multiply_95bf499cef1647aa87f2ef1406572a69_Out_2);
            float _Property_a479584411884039a90f17f2a024efbd_Out_0 = _Bloom;
            float _Multiply_7da2e1d7ffff4679b40e230759c44972_Out_2;
            Unity_Multiply_float_float(_Multiply_95bf499cef1647aa87f2ef1406572a69_Out_2, _Property_a479584411884039a90f17f2a024efbd_Out_0, _Multiply_7da2e1d7ffff4679b40e230759c44972_Out_2);
            surface.BaseColor = (_Add_134caef305844e19a0e99aaa50155988_Out_2.xyz);
            surface.Alpha = _Multiply_7da2e1d7ffff4679b40e230759c44972_Out_2;
            return surface;
        }
        
            // --------------------------------------------------
            // Build Graph Inputs
        
            VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
        {
            VertexDescriptionInputs output;
            ZERO_INITIALIZE(VertexDescriptionInputs, output);
        
            output.ObjectSpaceNormal =                          input.normalOS;
            output.ObjectSpaceTangent =                         input.tangentOS.xyz;
            output.ObjectSpacePosition =                        input.positionOS;
        
            return output;
        }
            SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
        {
            SurfaceDescriptionInputs output;
            ZERO_INITIALIZE(SurfaceDescriptionInputs, output);
        
            
        
        
        
        
        
            output.uv0 =                                        input.texCoord0;
            output.TimeParameters =                             _TimeParameters.xyz; // This is mainly for LW as HD overwrite this value
        #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN                output.FaceSign =                                   IS_FRONT_VFACE(input.cullFace, true, false);
        #else
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
        #endif
        #undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
        
            return output;
        }
        
            // --------------------------------------------------
            // Main
        
            #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/2D/ShaderGraph/Includes/SpriteUnlitPass.hlsl"
        
            ENDHLSL
        }
    }
    CustomEditor "UnityEditor.ShaderGraph.GenericShaderGraphMaterialGUI"
    FallBack "Hidden/Shader Graph/FallbackError"
}