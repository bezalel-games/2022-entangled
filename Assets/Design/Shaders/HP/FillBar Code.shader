Shader "FillBar"
{
    Properties
    {
        [NoScaleOffset]_MainTex("Main Texture", 2D) = "white" {}
        _Value("Value", Float) = 10
        _Max_Value("Max Value", Float) = 20
        _HPColor("HPColor", Color) = (0.4811321, 0.03404236, 0.03404236, 0)
        _EmptyColor("EmptyColor", Color) = (0.3207547, 0.3207547, 0.3207547, 0)
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
        float4 _MainTex_TexelSize;
        float _Value;
        float _Max_Value;
        float4 _HPColor;
        float4 _EmptyColor;
        CBUFFER_END
        
        // Object and Global properties
        SAMPLER(SamplerState_Linear_Repeat);
        TEXTURE2D(_MainTex);
        SAMPLER(sampler_MainTex);
        
            // Graph Includes
            // GraphIncludes: <None>
        
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
            
        void Unity_Divide_float(float A, float B, out float Out)
        {
            Out = A / B;
        }
        
        void Unity_Step_float(float Edge, float In, out float Out)
        {
            Out = step(Edge, In);
        }
        
        void Unity_OneMinus_float(float In, out float Out)
        {
            Out = 1 - In;
        }
        
        void Unity_Multiply_float4_float4(float4 A, float4 B, out float4 Out)
        {
            Out = A * B;
        }
        
        void Unity_Add_float4(float4 A, float4 B, out float4 Out)
        {
            Out = A + B;
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
            UnityTexture2D _Property_196af14883014aadbc52396cad9d33c5_Out_0 = UnityBuildTexture2DStructNoScale(_MainTex);
            float4 _SampleTexture2D_7bcad4b3d4ee43918341f6149d488dc9_RGBA_0 = SAMPLE_TEXTURE2D(_Property_196af14883014aadbc52396cad9d33c5_Out_0.tex, _Property_196af14883014aadbc52396cad9d33c5_Out_0.samplerstate, _Property_196af14883014aadbc52396cad9d33c5_Out_0.GetTransformedUV(IN.uv0.xy));
            float _SampleTexture2D_7bcad4b3d4ee43918341f6149d488dc9_R_4 = _SampleTexture2D_7bcad4b3d4ee43918341f6149d488dc9_RGBA_0.r;
            float _SampleTexture2D_7bcad4b3d4ee43918341f6149d488dc9_G_5 = _SampleTexture2D_7bcad4b3d4ee43918341f6149d488dc9_RGBA_0.g;
            float _SampleTexture2D_7bcad4b3d4ee43918341f6149d488dc9_B_6 = _SampleTexture2D_7bcad4b3d4ee43918341f6149d488dc9_RGBA_0.b;
            float _SampleTexture2D_7bcad4b3d4ee43918341f6149d488dc9_A_7 = _SampleTexture2D_7bcad4b3d4ee43918341f6149d488dc9_RGBA_0.a;
            float4 _Property_8bc83844c6a443d48e8f23638f98d49b_Out_0 = _EmptyColor;
            float4 _UV_c796cc0584ea4ef1820f0d5fa220ac6d_Out_0 = IN.uv0;
            float _Split_ee3408268cd240579693f6a6bcb187c7_R_1 = _UV_c796cc0584ea4ef1820f0d5fa220ac6d_Out_0[0];
            float _Split_ee3408268cd240579693f6a6bcb187c7_G_2 = _UV_c796cc0584ea4ef1820f0d5fa220ac6d_Out_0[1];
            float _Split_ee3408268cd240579693f6a6bcb187c7_B_3 = _UV_c796cc0584ea4ef1820f0d5fa220ac6d_Out_0[2];
            float _Split_ee3408268cd240579693f6a6bcb187c7_A_4 = _UV_c796cc0584ea4ef1820f0d5fa220ac6d_Out_0[3];
            float _Property_191fc8c36d0045beb4cee749bba0d1e1_Out_0 = _Value;
            float _Property_a457c016e94349a2b7444f8efccac9bd_Out_0 = _Max_Value;
            float _Divide_28e6ad83eb014a01a53cb6dfdbbdd259_Out_2;
            Unity_Divide_float(_Property_191fc8c36d0045beb4cee749bba0d1e1_Out_0, _Property_a457c016e94349a2b7444f8efccac9bd_Out_0, _Divide_28e6ad83eb014a01a53cb6dfdbbdd259_Out_2);
            float _Step_a3b4d94aed044eb9bc0eae1a3a465e9a_Out_2;
            Unity_Step_float(_Split_ee3408268cd240579693f6a6bcb187c7_R_1, _Divide_28e6ad83eb014a01a53cb6dfdbbdd259_Out_2, _Step_a3b4d94aed044eb9bc0eae1a3a465e9a_Out_2);
            float _OneMinus_9646df69058e44cbb8117d24e27fbe11_Out_1;
            Unity_OneMinus_float(_Step_a3b4d94aed044eb9bc0eae1a3a465e9a_Out_2, _OneMinus_9646df69058e44cbb8117d24e27fbe11_Out_1);
            float4 _Multiply_54fb3a95863043799ccdeb97433d8b97_Out_2;
            Unity_Multiply_float4_float4(_Property_8bc83844c6a443d48e8f23638f98d49b_Out_0, (_OneMinus_9646df69058e44cbb8117d24e27fbe11_Out_1.xxxx), _Multiply_54fb3a95863043799ccdeb97433d8b97_Out_2);
            float4 _Property_19d668dce1264630bad21f927729894c_Out_0 = _HPColor;
            float4 _Multiply_c00295d5e18547a7a93122d34afe91e8_Out_2;
            Unity_Multiply_float4_float4((_Step_a3b4d94aed044eb9bc0eae1a3a465e9a_Out_2.xxxx), _Property_19d668dce1264630bad21f927729894c_Out_0, _Multiply_c00295d5e18547a7a93122d34afe91e8_Out_2);
            float4 _Add_35b9f777ded8439f93524ac69f95a860_Out_2;
            Unity_Add_float4(_Multiply_54fb3a95863043799ccdeb97433d8b97_Out_2, _Multiply_c00295d5e18547a7a93122d34afe91e8_Out_2, _Add_35b9f777ded8439f93524ac69f95a860_Out_2);
            float4 _Multiply_2283d9d8d0014093b52706355356f6b0_Out_2;
            Unity_Multiply_float4_float4(_SampleTexture2D_7bcad4b3d4ee43918341f6149d488dc9_RGBA_0, _Add_35b9f777ded8439f93524ac69f95a860_Out_2, _Multiply_2283d9d8d0014093b52706355356f6b0_Out_2);
            surface.BaseColor = (_Multiply_2283d9d8d0014093b52706355356f6b0_Out_2.xyz);
            surface.Alpha = _SampleTexture2D_7bcad4b3d4ee43918341f6149d488dc9_A_7;
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
        float4 _MainTex_TexelSize;
        float _Value;
        float _Max_Value;
        float4 _HPColor;
        float4 _EmptyColor;
        CBUFFER_END
        
        // Object and Global properties
        SAMPLER(SamplerState_Linear_Repeat);
        TEXTURE2D(_MainTex);
        SAMPLER(sampler_MainTex);
        
            // Graph Includes
            // GraphIncludes: <None>
        
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
            
        void Unity_Divide_float(float A, float B, out float Out)
        {
            Out = A / B;
        }
        
        void Unity_Step_float(float Edge, float In, out float Out)
        {
            Out = step(Edge, In);
        }
        
        void Unity_OneMinus_float(float In, out float Out)
        {
            Out = 1 - In;
        }
        
        void Unity_Multiply_float4_float4(float4 A, float4 B, out float4 Out)
        {
            Out = A * B;
        }
        
        void Unity_Add_float4(float4 A, float4 B, out float4 Out)
        {
            Out = A + B;
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
            UnityTexture2D _Property_196af14883014aadbc52396cad9d33c5_Out_0 = UnityBuildTexture2DStructNoScale(_MainTex);
            float4 _SampleTexture2D_7bcad4b3d4ee43918341f6149d488dc9_RGBA_0 = SAMPLE_TEXTURE2D(_Property_196af14883014aadbc52396cad9d33c5_Out_0.tex, _Property_196af14883014aadbc52396cad9d33c5_Out_0.samplerstate, _Property_196af14883014aadbc52396cad9d33c5_Out_0.GetTransformedUV(IN.uv0.xy));
            float _SampleTexture2D_7bcad4b3d4ee43918341f6149d488dc9_R_4 = _SampleTexture2D_7bcad4b3d4ee43918341f6149d488dc9_RGBA_0.r;
            float _SampleTexture2D_7bcad4b3d4ee43918341f6149d488dc9_G_5 = _SampleTexture2D_7bcad4b3d4ee43918341f6149d488dc9_RGBA_0.g;
            float _SampleTexture2D_7bcad4b3d4ee43918341f6149d488dc9_B_6 = _SampleTexture2D_7bcad4b3d4ee43918341f6149d488dc9_RGBA_0.b;
            float _SampleTexture2D_7bcad4b3d4ee43918341f6149d488dc9_A_7 = _SampleTexture2D_7bcad4b3d4ee43918341f6149d488dc9_RGBA_0.a;
            float4 _Property_8bc83844c6a443d48e8f23638f98d49b_Out_0 = _EmptyColor;
            float4 _UV_c796cc0584ea4ef1820f0d5fa220ac6d_Out_0 = IN.uv0;
            float _Split_ee3408268cd240579693f6a6bcb187c7_R_1 = _UV_c796cc0584ea4ef1820f0d5fa220ac6d_Out_0[0];
            float _Split_ee3408268cd240579693f6a6bcb187c7_G_2 = _UV_c796cc0584ea4ef1820f0d5fa220ac6d_Out_0[1];
            float _Split_ee3408268cd240579693f6a6bcb187c7_B_3 = _UV_c796cc0584ea4ef1820f0d5fa220ac6d_Out_0[2];
            float _Split_ee3408268cd240579693f6a6bcb187c7_A_4 = _UV_c796cc0584ea4ef1820f0d5fa220ac6d_Out_0[3];
            float _Property_191fc8c36d0045beb4cee749bba0d1e1_Out_0 = _Value;
            float _Property_a457c016e94349a2b7444f8efccac9bd_Out_0 = _Max_Value;
            float _Divide_28e6ad83eb014a01a53cb6dfdbbdd259_Out_2;
            Unity_Divide_float(_Property_191fc8c36d0045beb4cee749bba0d1e1_Out_0, _Property_a457c016e94349a2b7444f8efccac9bd_Out_0, _Divide_28e6ad83eb014a01a53cb6dfdbbdd259_Out_2);
            float _Step_a3b4d94aed044eb9bc0eae1a3a465e9a_Out_2;
            Unity_Step_float(_Split_ee3408268cd240579693f6a6bcb187c7_R_1, _Divide_28e6ad83eb014a01a53cb6dfdbbdd259_Out_2, _Step_a3b4d94aed044eb9bc0eae1a3a465e9a_Out_2);
            float _OneMinus_9646df69058e44cbb8117d24e27fbe11_Out_1;
            Unity_OneMinus_float(_Step_a3b4d94aed044eb9bc0eae1a3a465e9a_Out_2, _OneMinus_9646df69058e44cbb8117d24e27fbe11_Out_1);
            float4 _Multiply_54fb3a95863043799ccdeb97433d8b97_Out_2;
            Unity_Multiply_float4_float4(_Property_8bc83844c6a443d48e8f23638f98d49b_Out_0, (_OneMinus_9646df69058e44cbb8117d24e27fbe11_Out_1.xxxx), _Multiply_54fb3a95863043799ccdeb97433d8b97_Out_2);
            float4 _Property_19d668dce1264630bad21f927729894c_Out_0 = _HPColor;
            float4 _Multiply_c00295d5e18547a7a93122d34afe91e8_Out_2;
            Unity_Multiply_float4_float4((_Step_a3b4d94aed044eb9bc0eae1a3a465e9a_Out_2.xxxx), _Property_19d668dce1264630bad21f927729894c_Out_0, _Multiply_c00295d5e18547a7a93122d34afe91e8_Out_2);
            float4 _Add_35b9f777ded8439f93524ac69f95a860_Out_2;
            Unity_Add_float4(_Multiply_54fb3a95863043799ccdeb97433d8b97_Out_2, _Multiply_c00295d5e18547a7a93122d34afe91e8_Out_2, _Add_35b9f777ded8439f93524ac69f95a860_Out_2);
            float4 _Multiply_2283d9d8d0014093b52706355356f6b0_Out_2;
            Unity_Multiply_float4_float4(_SampleTexture2D_7bcad4b3d4ee43918341f6149d488dc9_RGBA_0, _Add_35b9f777ded8439f93524ac69f95a860_Out_2, _Multiply_2283d9d8d0014093b52706355356f6b0_Out_2);
            surface.BaseColor = (_Multiply_2283d9d8d0014093b52706355356f6b0_Out_2.xyz);
            surface.Alpha = _SampleTexture2D_7bcad4b3d4ee43918341f6149d488dc9_A_7;
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