Shader "LR_MeshRenderer_Rover"
{
    Properties
    {
        [NoScaleOffset] mainTexture("Main Texture", 2D) = "white" {}
        _MainColor("Color", Color) = (0.5019608, 0.5019608, 0.5019608, 0)
        [HDR]_EmissionColor("Emission", Color) = (0, 0, 0, 0)
        [HDR]_FrenelColor("FrenelColor", Color) = (1, 1, 1, 1)
        _Frenel("Frenel", Range(-2, 1)) = 0
        _Metalic("Metalic", Range(0, 1)) = 0
        _Smoothness("Smoothness", Range(0, 1)) = 0
        [NoScaleOffset]_Occ("Occoliusion", 2D) = "white" {}
        _Offset("Offset", Float) = 0
        _Range("Range", Float) = 0
        [ToggleUI]Neon("Neon", Float) = 0
        [ToggleUI]Neon_1("Primary", Float) = 0
        [HideInInspector][NoScaleOffset]unity_Lightmaps("unity_Lightmaps", 2DArray) = "" {}
        [HideInInspector][NoScaleOffset]unity_LightmapsInd("unity_LightmapsInd", 2DArray) = "" {}
        [HideInInspector][NoScaleOffset]unity_ShadowMasks("unity_ShadowMasks", 2DArray) = "" {}
    }
        SubShader
    {
        Tags
        {
            "RenderPipeline" = "UniversalPipeline"
            "RenderType" = "Transparent"
            "UniversalMaterialType" = "Lit"
            "Queue" = "Transparent"
        }
        Pass
        {
            Name "Universal Forward Only"
            Tags
            {
                "LightMode" = "UniversalForwardOnly"
            }

        // Render State
        Cull Back
    Blend SrcAlpha OneMinusSrcAlpha, One OneMinusSrcAlpha
    ZTest LEqual
    ZWrite On

        // Debug
        // <None>

        // --------------------------------------------------
        // Pass

        HLSLPROGRAM

        // Pragmas
        #pragma target 4.5
    #pragma exclude_renderers gles gles3 glcore
    #pragma multi_compile_instancing
    #pragma multi_compile_fog
    #pragma multi_compile _ DOTS_INSTANCING_ON
    #pragma vertex vert
    #pragma fragment frag

        // DotsInstancingOptions: <None>
        // HybridV1InjectedBuiltinProperties: <None>

        // Keywords
        #pragma multi_compile _ _SCREEN_SPACE_OCCLUSION
    #pragma multi_compile _ LIGHTMAP_ON
    #pragma multi_compile _ DIRLIGHTMAP_COMBINED
    #pragma multi_compile _ _MAIN_LIGHT_SHADOWS
    #pragma multi_compile _ _MAIN_LIGHT_SHADOWS_CASCADE
    #pragma multi_compile _ADDITIONAL_LIGHTS_VERTEX _ADDITIONAL_LIGHTS _ADDITIONAL_OFF
    #pragma multi_compile _ _ADDITIONAL_LIGHT_SHADOWS
    #pragma multi_compile _ _SHADOWS_SOFT
    #pragma multi_compile _ LIGHTMAP_SHADOW_MIXING
    #pragma multi_compile _ SHADOWS_SHADOWMASK
        // GraphKeywords: <None>

        // Defines
        #define _SURFACE_TYPE_TRANSPARENT 1
        #define _AlphaClip 1
        #define _NORMALMAP 1
        #define _NORMAL_DROPOFF_TS 1
        #define ATTRIBUTES_NEED_NORMAL
        #define ATTRIBUTES_NEED_TANGENT
        #define ATTRIBUTES_NEED_TEXCOORD0
        #define ATTRIBUTES_NEED_TEXCOORD1
        #define VARYINGS_NEED_POSITION_WS
        #define VARYINGS_NEED_NORMAL_WS
        #define VARYINGS_NEED_TANGENT_WS
        #define VARYINGS_NEED_TEXCOORD0
        #define VARYINGS_NEED_VIEWDIRECTION_WS
        #define VARYINGS_NEED_FOG_AND_VERTEX_LIGHT
        #define FEATURES_GRAPH_VERTEX
        /* WARNING: $splice Could not find named fragment 'PassInstancing' */
        #define SHADERPASS SHADERPASS_FORWARDONLY
    #define _CLEARCOAT 1
        /* WARNING: $splice Could not find named fragment 'DotsInstancingVars' */

        // Includes
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
    #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
    #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
    #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
    #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
    #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Shadows.hlsl"
    #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"

        // --------------------------------------------------
        // Structs and Packing

        struct Attributes
    {
        float3 positionOS : POSITION;
        float3 normalOS : NORMAL;
        float4 tangentOS : TANGENT;
        float4 uv0 : TEXCOORD0;
        float4 uv1 : TEXCOORD1;
        #if UNITY_ANY_INSTANCING_ENABLED
        uint instanceID : INSTANCEID_SEMANTIC;
        #endif
    };
    struct Varyings
    {
        float4 positionCS : SV_POSITION;
        float3 positionWS;
        float3 normalWS;
        float4 tangentWS;
        float4 texCoord0;
        float3 viewDirectionWS;
        #if defined(LIGHTMAP_ON)
        float2 lightmapUV;
        #endif
        #if !defined(LIGHTMAP_ON)
        float3 sh;
        #endif
        float4 fogFactorAndVertexLight;
        float4 shadowCoord;
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
        float3 WorldSpaceNormal;
        float3 TangentSpaceNormal;
        float3 WorldSpaceViewDirection;
        float3 WorldSpacePosition;
        float4 ScreenPosition;
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
        float3 interp0 : TEXCOORD0;
        float3 interp1 : TEXCOORD1;
        float4 interp2 : TEXCOORD2;
        float4 interp3 : TEXCOORD3;
        float3 interp4 : TEXCOORD4;
        #if defined(LIGHTMAP_ON)
        float2 interp5 : TEXCOORD5;
        #endif
        #if !defined(LIGHTMAP_ON)
        float3 interp6 : TEXCOORD6;
        #endif
        float4 interp7 : TEXCOORD7;
        float4 interp8 : TEXCOORD8;
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

        PackedVaryings PackVaryings(Varyings input)
    {
        PackedVaryings output;
        output.positionCS = input.positionCS;
        output.interp0.xyz = input.positionWS;
        output.interp1.xyz = input.normalWS;
        output.interp2.xyzw = input.tangentWS;
        output.interp3.xyzw = input.texCoord0;
        output.interp4.xyz = input.viewDirectionWS;
        #if defined(LIGHTMAP_ON)
        output.interp5.xy = input.lightmapUV;
        #endif
        #if !defined(LIGHTMAP_ON)
        output.interp6.xyz = input.sh;
        #endif
        output.interp7.xyzw = input.fogFactorAndVertexLight;
        output.interp8.xyzw = input.shadowCoord;
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
    Varyings UnpackVaryings(PackedVaryings input)
    {
        Varyings output;
        output.positionCS = input.positionCS;
        output.positionWS = input.interp0.xyz;
        output.normalWS = input.interp1.xyz;
        output.tangentWS = input.interp2.xyzw;
        output.texCoord0 = input.interp3.xyzw;
        output.viewDirectionWS = input.interp4.xyz;
        #if defined(LIGHTMAP_ON)
        output.lightmapUV = input.interp5.xy;
        #endif
        #if !defined(LIGHTMAP_ON)
        output.sh = input.interp6.xyz;
        #endif
        output.fogFactorAndVertexLight = input.interp7.xyzw;
        output.shadowCoord = input.interp8.xyzw;
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
float4 mainTexture_TexelSize;
float4 _MainColor;
float4 _EmissionColor;
float4 _FrenelColor;
float _Frenel;
float _Metalic;
float _Smoothness;
float4 _Occ_TexelSize;
float _Offset;
float _Range;
float Neon;
float Neon_1;
CBUFFER_END

// Object and Global properties
SAMPLER(SamplerState_Linear_Repeat);
SAMPLER(SamplerState_Trilinear_Clamp);
TEXTURE2D(mainTexture);
SAMPLER(samplermainTexture);
TEXTURE2D(_Occ);
SAMPLER(sampler_Occ);

// Graph Functions

void Unity_Blend_Overlay_float4(float4 Base, float4 Blend, out float4 Out, float Opacity)
{
    float4 result1 = 1.0 - 2.0 * (1.0 - Base) * (1.0 - Blend);
    float4 result2 = 2.0 * Base * Blend;
    float4 zeroOrOne = step(Base, 0.5);
    Out = result2 * zeroOrOne + (1 - zeroOrOne) * result1;
    Out = lerp(Base, Out, Opacity);
}

void Unity_Branch_float4(float Predicate, float4 True, float4 False, out float4 Out)
{
    Out = Predicate ? True : False;
}

void Unity_Remap_float4(float4 In, float2 InMinMax, float2 OutMinMax, out float4 Out)
{
    Out = OutMinMax.x + (In - InMinMax.x) * (OutMinMax.y - OutMinMax.x) / (InMinMax.y - InMinMax.x);
}

void Unity_OneMinus_float(float In, out float Out)
{
    Out = 1 - In;
}

void Unity_FresnelEffect_float(float3 Normal, float3 ViewDir, float Power, out float Out)
{
    Out = pow((1.0 - saturate(dot(normalize(Normal), normalize(ViewDir)))), Power);
}

void Unity_Power_float(float A, float B, out float Out)
{
    Out = pow(A, B);
}

void Unity_Blend_Overwrite_float(float Base, float Blend, out float Out, float Opacity)
{
    Out = lerp(Base, Blend, Opacity);
}

void Unity_Blend_Multiply_float4(float4 Base, float4 Blend, out float4 Out, float Opacity)
{
    Out = Base * Blend;
    Out = lerp(Base, Out, Opacity);
}

void Unity_Multiply_float(float4 A, float4 B, out float4 Out)
{
    Out = A * B;
}

void Unity_Blend_Lighten_float4(float4 Base, float4 Blend, out float4 Out, float Opacity)
{
    Out = max(Blend, Base);
    Out = lerp(Base, Out, Opacity);
}

void Unity_Subtract_float(float A, float B, out float Out)
{
    Out = A - B;
}

void Unity_Multiply_float(float A, float B, out float Out)
{
    Out = A * B;
}

void Unity_Clamp_float(float In, float Min, float Max, out float Out)
{
    Out = clamp(In, Min, Max);
}

void Unity_Blend_Overwrite_float4(float4 Base, float4 Blend, out float4 Out, float Opacity)
{
    Out = lerp(Base, Blend, Opacity);
}

void Unity_TilingAndOffset_float(float2 UV, float2 Tiling, float2 Offset, out float2 Out)
{
    Out = UV * Tiling + Offset;
}

void Unity_Remap_float(float In, float2 InMinMax, float2 OutMinMax, out float Out)
{
    Out = OutMinMax.x + (In - InMinMax.x) * (OutMinMax.y - OutMinMax.x) / (InMinMax.y - InMinMax.x);
}

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

// Graph Pixel
struct SurfaceDescription
{
    float3 BaseColor;
    float3 NormalTS;
    float3 Emission;
    float Metallic;
    float Smoothness;
    float Occlusion;
    float Alpha;
    float AlphaClipThreshold;
    float CoatMask;
    float CoatSmoothness;
};

SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
{
    SurfaceDescription surface = (SurfaceDescription)0;
    float _Property_db0d6ba2317f4ca885fffc369f6b7572_Out_0 = Neon_1;
    float4 _Property_404b2b1af018418b81f010f388917a24_Out_0 = _MainColor;
    UnityTexture2D _Property_9c598a886ed15f8aa999f5dbaa5a4e25_Out_0 = UnityBuildTexture2DStructNoScale(mainTexture);
    float4 _SampleTexture2D_9a0e43a036a3828e8771aff20cdda3ba_RGBA_0 = SAMPLE_TEXTURE2D(_Property_9c598a886ed15f8aa999f5dbaa5a4e25_Out_0.tex, _Property_9c598a886ed15f8aa999f5dbaa5a4e25_Out_0.samplerstate, IN.uv0.xy);
    float _SampleTexture2D_9a0e43a036a3828e8771aff20cdda3ba_R_4 = _SampleTexture2D_9a0e43a036a3828e8771aff20cdda3ba_RGBA_0.r;
    float _SampleTexture2D_9a0e43a036a3828e8771aff20cdda3ba_G_5 = _SampleTexture2D_9a0e43a036a3828e8771aff20cdda3ba_RGBA_0.g;
    float _SampleTexture2D_9a0e43a036a3828e8771aff20cdda3ba_B_6 = _SampleTexture2D_9a0e43a036a3828e8771aff20cdda3ba_RGBA_0.b;
    float _SampleTexture2D_9a0e43a036a3828e8771aff20cdda3ba_A_7 = _SampleTexture2D_9a0e43a036a3828e8771aff20cdda3ba_RGBA_0.a;
    float4 _Blend_07c1433efd838d88a6692dd2f3fa5cc1_Out_2;
    Unity_Blend_Overlay_float4(_SampleTexture2D_9a0e43a036a3828e8771aff20cdda3ba_RGBA_0, _Property_404b2b1af018418b81f010f388917a24_Out_0, _Blend_07c1433efd838d88a6692dd2f3fa5cc1_Out_2, 1);
    float4 _Branch_0d2a1beb339f47d7a3370ab32efd8e0c_Out_3;
    Unity_Branch_float4(_Property_db0d6ba2317f4ca885fffc369f6b7572_Out_0, _Property_404b2b1af018418b81f010f388917a24_Out_0, _Blend_07c1433efd838d88a6692dd2f3fa5cc1_Out_2, _Branch_0d2a1beb339f47d7a3370ab32efd8e0c_Out_3);
    float _Property_0d8e02a76989bb80ae497d44ac78381b_Out_0 = Neon;
    float4 _Remap_6823e14564e9eb8a8ef71bcec591d781_Out_3;
    Unity_Remap_float4(_SampleTexture2D_9a0e43a036a3828e8771aff20cdda3ba_RGBA_0, float2 (0, 1), float2 (0, 2), _Remap_6823e14564e9eb8a8ef71bcec591d781_Out_3);
    float4 Color_1d0b78cf07232483869322c6e5134511 = IsGammaSpace() ? float4(0.1885618, 0.6377826, 1.059274, 1) : float4(SRGBToLinear(float3(0.1885618, 0.6377826, 1.059274)), 1);
    float4 _Property_976c2b04f54b068091004d51cb39a1bb_Out_0 = _EmissionColor;
    float _Property_bc4a96833916af8fab203c56c0b44955_Out_0 = _Frenel;
    float _OneMinus_00c02da1a3e44983954a2b94c04bbe8d_Out_1;
    Unity_OneMinus_float(_Property_bc4a96833916af8fab203c56c0b44955_Out_0, _OneMinus_00c02da1a3e44983954a2b94c04bbe8d_Out_1);
    float _FresnelEffect_2e0aeb0e30852e8686f75ede044d7ce7_Out_3;
    Unity_FresnelEffect_float(IN.WorldSpaceNormal, IN.WorldSpaceViewDirection, _OneMinus_00c02da1a3e44983954a2b94c04bbe8d_Out_1, _FresnelEffect_2e0aeb0e30852e8686f75ede044d7ce7_Out_3);
    float _Power_c20579372975d58d9e956dc03e906821_Out_2;
    Unity_Power_float(_FresnelEffect_2e0aeb0e30852e8686f75ede044d7ce7_Out_3, 2, _Power_c20579372975d58d9e956dc03e906821_Out_2);
    float _Blend_927874c56b8f15888bfe6eb2b1d8c1f3_Out_2;
    Unity_Blend_Overwrite_float(0, _Power_c20579372975d58d9e956dc03e906821_Out_2, _Blend_927874c56b8f15888bfe6eb2b1d8c1f3_Out_2, 1);
    float4 _Property_337d8379e365b1859545f24d89eb283d_Out_0 = _FrenelColor;
    float4 _Blend_b52462f55ca3fa8e8123560ad96f591f_Out_2;
    Unity_Blend_Multiply_float4((_Blend_927874c56b8f15888bfe6eb2b1d8c1f3_Out_2.xxxx), _Property_337d8379e365b1859545f24d89eb283d_Out_0, _Blend_b52462f55ca3fa8e8123560ad96f591f_Out_2, 1);
    float4 _Multiply_8d1b6059d5faec85ae378ee724261100_Out_2;
    Unity_Multiply_float(_Blend_b52462f55ca3fa8e8123560ad96f591f_Out_2, float4(1, 1, 1, 1), _Multiply_8d1b6059d5faec85ae378ee724261100_Out_2);
    float4 _Blend_71fa99a490669a8789492c7ff5006154_Out_2;
    Unity_Blend_Lighten_float4(_Property_976c2b04f54b068091004d51cb39a1bb_Out_0, _Multiply_8d1b6059d5faec85ae378ee724261100_Out_2, _Blend_71fa99a490669a8789492c7ff5006154_Out_2, 1);
    float4 _ScreenPosition_724ad830a44b8582920d49bb9a3a8049_Out_0 = IN.ScreenPosition;
    float _Split_74ec02de1ed52489b7f5d8edf6bec915_R_1 = _ScreenPosition_724ad830a44b8582920d49bb9a3a8049_Out_0[0];
    float _Split_74ec02de1ed52489b7f5d8edf6bec915_G_2 = _ScreenPosition_724ad830a44b8582920d49bb9a3a8049_Out_0[1];
    float _Split_74ec02de1ed52489b7f5d8edf6bec915_B_3 = _ScreenPosition_724ad830a44b8582920d49bb9a3a8049_Out_0[2];
    float _Split_74ec02de1ed52489b7f5d8edf6bec915_A_4 = _ScreenPosition_724ad830a44b8582920d49bb9a3a8049_Out_0[3];
    float _Property_d93f8f362dde2481a1ff9b6a41471f97_Out_0 = _Offset;
    float _Subtract_63e4def5bfbcb0819237ae1e75f6f48f_Out_2;
    Unity_Subtract_float(_Split_74ec02de1ed52489b7f5d8edf6bec915_A_4, _Property_d93f8f362dde2481a1ff9b6a41471f97_Out_0, _Subtract_63e4def5bfbcb0819237ae1e75f6f48f_Out_2);
    float _Property_bf58377199188e8494396b8339c183d9_Out_0 = _Range;
    float _Multiply_52dbf97db03cea8683a73148cc653b01_Out_2;
    Unity_Multiply_float(_Subtract_63e4def5bfbcb0819237ae1e75f6f48f_Out_2, _Property_bf58377199188e8494396b8339c183d9_Out_0, _Multiply_52dbf97db03cea8683a73148cc653b01_Out_2);
    float _Clamp_64ffae1e14feae89b94596b9cba21a55_Out_3;
    Unity_Clamp_float(_Multiply_52dbf97db03cea8683a73148cc653b01_Out_2, 0, 1, _Clamp_64ffae1e14feae89b94596b9cba21a55_Out_3);
    float4 _Blend_2a8b33610ca45d8389ae5bca9784d7de_Out_2;
    Unity_Blend_Overwrite_float4(Color_1d0b78cf07232483869322c6e5134511, _Blend_71fa99a490669a8789492c7ff5006154_Out_2, _Blend_2a8b33610ca45d8389ae5bca9784d7de_Out_2, _Clamp_64ffae1e14feae89b94596b9cba21a55_Out_3);
    float4 _Branch_4e6bcacf8fcb5585abae3c176af03007_Out_3;
    Unity_Branch_float4(_Property_0d8e02a76989bb80ae497d44ac78381b_Out_0, _Remap_6823e14564e9eb8a8ef71bcec591d781_Out_3, _Blend_2a8b33610ca45d8389ae5bca9784d7de_Out_2, _Branch_4e6bcacf8fcb5585abae3c176af03007_Out_3);
    float _Property_e46677db9206b885982d9d37f05e425d_Out_0 = _Metalic;
    float _Property_d516b44f636fab859fe69246d5bf30e4_Out_0 = _Smoothness;
    UnityTexture2D _Property_b94b0b6634cc78818b2b2c8112d1f747_Out_0 = UnityBuildTexture2DStructNoScale(_Occ);
    float2 _TilingAndOffset_a211f9e6b171ae848225b4af08b251ef_Out_3;
    Unity_TilingAndOffset_float(IN.uv0.xy, float2 (1, 1), float2 (0, 0), _TilingAndOffset_a211f9e6b171ae848225b4af08b251ef_Out_3);
    float4 _SampleTexture2D_ab66c12e83ffe287a0e73a8ee8d8ab04_RGBA_0 = SAMPLE_TEXTURE2D(_Property_b94b0b6634cc78818b2b2c8112d1f747_Out_0.tex, UnityBuildSamplerStateStruct(SamplerState_Trilinear_Clamp).samplerstate, _TilingAndOffset_a211f9e6b171ae848225b4af08b251ef_Out_3);
    float _SampleTexture2D_ab66c12e83ffe287a0e73a8ee8d8ab04_R_4 = _SampleTexture2D_ab66c12e83ffe287a0e73a8ee8d8ab04_RGBA_0.r;
    float _SampleTexture2D_ab66c12e83ffe287a0e73a8ee8d8ab04_G_5 = _SampleTexture2D_ab66c12e83ffe287a0e73a8ee8d8ab04_RGBA_0.g;
    float _SampleTexture2D_ab66c12e83ffe287a0e73a8ee8d8ab04_B_6 = _SampleTexture2D_ab66c12e83ffe287a0e73a8ee8d8ab04_RGBA_0.b;
    float _SampleTexture2D_ab66c12e83ffe287a0e73a8ee8d8ab04_A_7 = _SampleTexture2D_ab66c12e83ffe287a0e73a8ee8d8ab04_RGBA_0.a;
    float _FresnelEffect_6dabda9e9184008dbdfce2ac745c66c0_Out_3;
    Unity_FresnelEffect_float(IN.WorldSpaceNormal, IN.WorldSpaceViewDirection, 10, _FresnelEffect_6dabda9e9184008dbdfce2ac745c66c0_Out_3);
    float _Remap_244e9fe231479c899040aad8bb5aa37c_Out_3;
    Unity_Remap_float(_FresnelEffect_6dabda9e9184008dbdfce2ac745c66c0_Out_3, float2 (0, 0.5), float2 (0.05, 1), _Remap_244e9fe231479c899040aad8bb5aa37c_Out_3);
    float _Blend_ec4b7034bfbcc28e9b21ae7806e93882_Out_2;
    Unity_Blend_Overwrite_float(_Remap_244e9fe231479c899040aad8bb5aa37c_Out_3, 1, _Blend_ec4b7034bfbcc28e9b21ae7806e93882_Out_2, _Clamp_64ffae1e14feae89b94596b9cba21a55_Out_3);
    float Slider_9331cbdecf72558e8c14e4d3f9a7360e = 0;
    surface.BaseColor = (_Branch_0d2a1beb339f47d7a3370ab32efd8e0c_Out_3.xyz);
    surface.NormalTS = IN.TangentSpaceNormal;
    surface.Emission = (_Branch_4e6bcacf8fcb5585abae3c176af03007_Out_3.xyz);
    surface.Metallic = _Property_e46677db9206b885982d9d37f05e425d_Out_0;
    surface.Smoothness = _Property_d516b44f636fab859fe69246d5bf30e4_Out_0;
    surface.Occlusion = (_SampleTexture2D_ab66c12e83ffe287a0e73a8ee8d8ab04_RGBA_0).x;
    surface.Alpha = _Blend_ec4b7034bfbcc28e9b21ae7806e93882_Out_2;
    surface.AlphaClipThreshold = Slider_9331cbdecf72558e8c14e4d3f9a7360e;
    surface.CoatMask = 0;
    surface.CoatSmoothness = 1;
    return surface;
}

// --------------------------------------------------
// Build Graph Inputs

VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
{
    VertexDescriptionInputs output;
    ZERO_INITIALIZE(VertexDescriptionInputs, output);

    output.ObjectSpaceNormal = input.normalOS;
    output.ObjectSpaceTangent = input.tangentOS.xyz;
    output.ObjectSpacePosition = input.positionOS;

    return output;
}
    SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
{
    SurfaceDescriptionInputs output;
    ZERO_INITIALIZE(SurfaceDescriptionInputs, output);

    // must use interpolated tangent, bitangent and normal before they are normalized in the pixel shader.
    float3 unnormalizedNormalWS = input.normalWS;
    const float renormFactor = 1.0 / length(unnormalizedNormalWS);


    output.WorldSpaceNormal = renormFactor * input.normalWS.xyz;		// we want a unit length Normal Vector node in shader graph
    output.TangentSpaceNormal = float3(0.0f, 0.0f, 1.0f);


    output.WorldSpaceViewDirection = input.viewDirectionWS; //TODO: by default normalized in HD, but not in universal
    output.WorldSpacePosition = input.positionWS;
    output.ScreenPosition = ComputeScreenPos(TransformWorldToHClip(input.positionWS), _ProjectionParams.x);
    output.uv0 = input.texCoord0;
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

    #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"
#include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
#include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/PBRForwardPass.hlsl"

    ENDHLSL
}
Pass
{
    Name "ShadowCaster"
    Tags
    {
        "LightMode" = "ShadowCaster"
    }

        // Render State
        Cull Back
    Blend SrcAlpha OneMinusSrcAlpha, One OneMinusSrcAlpha
    ZTest LEqual
    ZWrite On
    ColorMask 0

        // Debug
        // <None>

        // --------------------------------------------------
        // Pass

        HLSLPROGRAM

        // Pragmas
        #pragma target 4.5
    #pragma exclude_renderers gles gles3 glcore
    #pragma multi_compile_instancing
    #pragma multi_compile _ DOTS_INSTANCING_ON
    #pragma vertex vert
    #pragma fragment frag

        // DotsInstancingOptions: <None>
        // HybridV1InjectedBuiltinProperties: <None>

        // Keywords
        // PassKeywords: <None>
        // GraphKeywords: <None>

        // Defines
        #define _SURFACE_TYPE_TRANSPARENT 1
        #define _AlphaClip 1
        #define _NORMALMAP 1
        #define _NORMAL_DROPOFF_TS 1
        #define ATTRIBUTES_NEED_NORMAL
        #define ATTRIBUTES_NEED_TANGENT
        #define VARYINGS_NEED_POSITION_WS
        #define VARYINGS_NEED_NORMAL_WS
        #define VARYINGS_NEED_VIEWDIRECTION_WS
        #define FEATURES_GRAPH_VERTEX
        /* WARNING: $splice Could not find named fragment 'PassInstancing' */
        #define SHADERPASS SHADERPASS_SHADOWCASTER
        /* WARNING: $splice Could not find named fragment 'DotsInstancingVars' */

        // Includes
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
    #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
    #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
    #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
    #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
    #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"

        // --------------------------------------------------
        // Structs and Packing

        struct Attributes
    {
        float3 positionOS : POSITION;
        float3 normalOS : NORMAL;
        float4 tangentOS : TANGENT;
        #if UNITY_ANY_INSTANCING_ENABLED
        uint instanceID : INSTANCEID_SEMANTIC;
        #endif
    };
    struct Varyings
    {
        float4 positionCS : SV_POSITION;
        float3 positionWS;
        float3 normalWS;
        float3 viewDirectionWS;
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
        float3 WorldSpaceNormal;
        float3 WorldSpaceViewDirection;
        float3 WorldSpacePosition;
        float4 ScreenPosition;
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
        float3 interp0 : TEXCOORD0;
        float3 interp1 : TEXCOORD1;
        float3 interp2 : TEXCOORD2;
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

        PackedVaryings PackVaryings(Varyings input)
    {
        PackedVaryings output;
        output.positionCS = input.positionCS;
        output.interp0.xyz = input.positionWS;
        output.interp1.xyz = input.normalWS;
        output.interp2.xyz = input.viewDirectionWS;
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
    Varyings UnpackVaryings(PackedVaryings input)
    {
        Varyings output;
        output.positionCS = input.positionCS;
        output.positionWS = input.interp0.xyz;
        output.normalWS = input.interp1.xyz;
        output.viewDirectionWS = input.interp2.xyz;
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
float4 mainTexture_TexelSize;
float4 _MainColor;
float4 _EmissionColor;
float4 _FrenelColor;
float _Frenel;
float _Metalic;
float _Smoothness;
float4 _Occ_TexelSize;
float _Offset;
float _Range;
float Neon;
float Neon_1;
CBUFFER_END

// Object and Global properties
SAMPLER(SamplerState_Linear_Repeat);
SAMPLER(SamplerState_Trilinear_Clamp);
TEXTURE2D(mainTexture);
SAMPLER(samplermainTexture);
TEXTURE2D(_Occ);
SAMPLER(sampler_Occ);

// Graph Functions

void Unity_FresnelEffect_float(float3 Normal, float3 ViewDir, float Power, out float Out)
{
    Out = pow((1.0 - saturate(dot(normalize(Normal), normalize(ViewDir)))), Power);
}

void Unity_Remap_float(float In, float2 InMinMax, float2 OutMinMax, out float Out)
{
    Out = OutMinMax.x + (In - InMinMax.x) * (OutMinMax.y - OutMinMax.x) / (InMinMax.y - InMinMax.x);
}

void Unity_Subtract_float(float A, float B, out float Out)
{
    Out = A - B;
}

void Unity_Multiply_float(float A, float B, out float Out)
{
    Out = A * B;
}

void Unity_Clamp_float(float In, float Min, float Max, out float Out)
{
    Out = clamp(In, Min, Max);
}

void Unity_Blend_Overwrite_float(float Base, float Blend, out float Out, float Opacity)
{
    Out = lerp(Base, Blend, Opacity);
}

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

// Graph Pixel
struct SurfaceDescription
{
    float Alpha;
    float AlphaClipThreshold;
};

SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
{
    SurfaceDescription surface = (SurfaceDescription)0;
    float _FresnelEffect_6dabda9e9184008dbdfce2ac745c66c0_Out_3;
    Unity_FresnelEffect_float(IN.WorldSpaceNormal, IN.WorldSpaceViewDirection, 10, _FresnelEffect_6dabda9e9184008dbdfce2ac745c66c0_Out_3);
    float _Remap_244e9fe231479c899040aad8bb5aa37c_Out_3;
    Unity_Remap_float(_FresnelEffect_6dabda9e9184008dbdfce2ac745c66c0_Out_3, float2 (0, 0.5), float2 (0.05, 1), _Remap_244e9fe231479c899040aad8bb5aa37c_Out_3);
    float4 _ScreenPosition_724ad830a44b8582920d49bb9a3a8049_Out_0 = IN.ScreenPosition;
    float _Split_74ec02de1ed52489b7f5d8edf6bec915_R_1 = _ScreenPosition_724ad830a44b8582920d49bb9a3a8049_Out_0[0];
    float _Split_74ec02de1ed52489b7f5d8edf6bec915_G_2 = _ScreenPosition_724ad830a44b8582920d49bb9a3a8049_Out_0[1];
    float _Split_74ec02de1ed52489b7f5d8edf6bec915_B_3 = _ScreenPosition_724ad830a44b8582920d49bb9a3a8049_Out_0[2];
    float _Split_74ec02de1ed52489b7f5d8edf6bec915_A_4 = _ScreenPosition_724ad830a44b8582920d49bb9a3a8049_Out_0[3];
    float _Property_d93f8f362dde2481a1ff9b6a41471f97_Out_0 = _Offset;
    float _Subtract_63e4def5bfbcb0819237ae1e75f6f48f_Out_2;
    Unity_Subtract_float(_Split_74ec02de1ed52489b7f5d8edf6bec915_A_4, _Property_d93f8f362dde2481a1ff9b6a41471f97_Out_0, _Subtract_63e4def5bfbcb0819237ae1e75f6f48f_Out_2);
    float _Property_bf58377199188e8494396b8339c183d9_Out_0 = _Range;
    float _Multiply_52dbf97db03cea8683a73148cc653b01_Out_2;
    Unity_Multiply_float(_Subtract_63e4def5bfbcb0819237ae1e75f6f48f_Out_2, _Property_bf58377199188e8494396b8339c183d9_Out_0, _Multiply_52dbf97db03cea8683a73148cc653b01_Out_2);
    float _Clamp_64ffae1e14feae89b94596b9cba21a55_Out_3;
    Unity_Clamp_float(_Multiply_52dbf97db03cea8683a73148cc653b01_Out_2, 0, 1, _Clamp_64ffae1e14feae89b94596b9cba21a55_Out_3);
    float _Blend_ec4b7034bfbcc28e9b21ae7806e93882_Out_2;
    Unity_Blend_Overwrite_float(_Remap_244e9fe231479c899040aad8bb5aa37c_Out_3, 1, _Blend_ec4b7034bfbcc28e9b21ae7806e93882_Out_2, _Clamp_64ffae1e14feae89b94596b9cba21a55_Out_3);
    float Slider_9331cbdecf72558e8c14e4d3f9a7360e = 0;
    surface.Alpha = _Blend_ec4b7034bfbcc28e9b21ae7806e93882_Out_2;
    surface.AlphaClipThreshold = Slider_9331cbdecf72558e8c14e4d3f9a7360e;
    return surface;
}

// --------------------------------------------------
// Build Graph Inputs

VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
{
    VertexDescriptionInputs output;
    ZERO_INITIALIZE(VertexDescriptionInputs, output);

    output.ObjectSpaceNormal = input.normalOS;
    output.ObjectSpaceTangent = input.tangentOS.xyz;
    output.ObjectSpacePosition = input.positionOS;

    return output;
}
    SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
{
    SurfaceDescriptionInputs output;
    ZERO_INITIALIZE(SurfaceDescriptionInputs, output);

    // must use interpolated tangent, bitangent and normal before they are normalized in the pixel shader.
    float3 unnormalizedNormalWS = input.normalWS;
    const float renormFactor = 1.0 / length(unnormalizedNormalWS);


    output.WorldSpaceNormal = renormFactor * input.normalWS.xyz;		// we want a unit length Normal Vector node in shader graph


    output.WorldSpaceViewDirection = input.viewDirectionWS; //TODO: by default normalized in HD, but not in universal
    output.WorldSpacePosition = input.positionWS;
    output.ScreenPosition = ComputeScreenPos(TransformWorldToHClip(input.positionWS), _ProjectionParams.x);
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

    #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"
#include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
#include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShadowCasterPass.hlsl"

    ENDHLSL
}
Pass
{
    Name "DepthOnly"
    Tags
    {
        "LightMode" = "DepthOnly"
    }

        // Render State
        Cull Back
    Blend SrcAlpha OneMinusSrcAlpha, One OneMinusSrcAlpha
    ZTest LEqual
    ZWrite On
    ColorMask 0

        // Debug
        // <None>

        // --------------------------------------------------
        // Pass

        HLSLPROGRAM

        // Pragmas
        #pragma target 4.5
    #pragma exclude_renderers gles gles3 glcore
    #pragma multi_compile_instancing
    #pragma multi_compile _ DOTS_INSTANCING_ON
    #pragma vertex vert
    #pragma fragment frag

        // DotsInstancingOptions: <None>
        // HybridV1InjectedBuiltinProperties: <None>

        // Keywords
        // PassKeywords: <None>
        // GraphKeywords: <None>

        // Defines
        #define _SURFACE_TYPE_TRANSPARENT 1
        #define _AlphaClip 1
        #define _NORMALMAP 1
        #define _NORMAL_DROPOFF_TS 1
        #define ATTRIBUTES_NEED_NORMAL
        #define ATTRIBUTES_NEED_TANGENT
        #define VARYINGS_NEED_POSITION_WS
        #define VARYINGS_NEED_NORMAL_WS
        #define VARYINGS_NEED_VIEWDIRECTION_WS
        #define FEATURES_GRAPH_VERTEX
        /* WARNING: $splice Could not find named fragment 'PassInstancing' */
        #define SHADERPASS SHADERPASS_DEPTHONLY
        /* WARNING: $splice Could not find named fragment 'DotsInstancingVars' */

        // Includes
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
    #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
    #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
    #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
    #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
    #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"

        // --------------------------------------------------
        // Structs and Packing

        struct Attributes
    {
        float3 positionOS : POSITION;
        float3 normalOS : NORMAL;
        float4 tangentOS : TANGENT;
        #if UNITY_ANY_INSTANCING_ENABLED
        uint instanceID : INSTANCEID_SEMANTIC;
        #endif
    };
    struct Varyings
    {
        float4 positionCS : SV_POSITION;
        float3 positionWS;
        float3 normalWS;
        float3 viewDirectionWS;
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
        float3 WorldSpaceNormal;
        float3 WorldSpaceViewDirection;
        float3 WorldSpacePosition;
        float4 ScreenPosition;
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
        float3 interp0 : TEXCOORD0;
        float3 interp1 : TEXCOORD1;
        float3 interp2 : TEXCOORD2;
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

        PackedVaryings PackVaryings(Varyings input)
    {
        PackedVaryings output;
        output.positionCS = input.positionCS;
        output.interp0.xyz = input.positionWS;
        output.interp1.xyz = input.normalWS;
        output.interp2.xyz = input.viewDirectionWS;
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
    Varyings UnpackVaryings(PackedVaryings input)
    {
        Varyings output;
        output.positionCS = input.positionCS;
        output.positionWS = input.interp0.xyz;
        output.normalWS = input.interp1.xyz;
        output.viewDirectionWS = input.interp2.xyz;
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
float4 mainTexture_TexelSize;
float4 _MainColor;
float4 _EmissionColor;
float4 _FrenelColor;
float _Frenel;
float _Metalic;
float _Smoothness;
float4 _Occ_TexelSize;
float _Offset;
float _Range;
float Neon;
float Neon_1;
CBUFFER_END

// Object and Global properties
SAMPLER(SamplerState_Linear_Repeat);
SAMPLER(SamplerState_Trilinear_Clamp);
TEXTURE2D(mainTexture);
SAMPLER(samplermainTexture);
TEXTURE2D(_Occ);
SAMPLER(sampler_Occ);

// Graph Functions

void Unity_FresnelEffect_float(float3 Normal, float3 ViewDir, float Power, out float Out)
{
    Out = pow((1.0 - saturate(dot(normalize(Normal), normalize(ViewDir)))), Power);
}

void Unity_Remap_float(float In, float2 InMinMax, float2 OutMinMax, out float Out)
{
    Out = OutMinMax.x + (In - InMinMax.x) * (OutMinMax.y - OutMinMax.x) / (InMinMax.y - InMinMax.x);
}

void Unity_Subtract_float(float A, float B, out float Out)
{
    Out = A - B;
}

void Unity_Multiply_float(float A, float B, out float Out)
{
    Out = A * B;
}

void Unity_Clamp_float(float In, float Min, float Max, out float Out)
{
    Out = clamp(In, Min, Max);
}

void Unity_Blend_Overwrite_float(float Base, float Blend, out float Out, float Opacity)
{
    Out = lerp(Base, Blend, Opacity);
}

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

// Graph Pixel
struct SurfaceDescription
{
    float Alpha;
    float AlphaClipThreshold;
};

SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
{
    SurfaceDescription surface = (SurfaceDescription)0;
    float _FresnelEffect_6dabda9e9184008dbdfce2ac745c66c0_Out_3;
    Unity_FresnelEffect_float(IN.WorldSpaceNormal, IN.WorldSpaceViewDirection, 10, _FresnelEffect_6dabda9e9184008dbdfce2ac745c66c0_Out_3);
    float _Remap_244e9fe231479c899040aad8bb5aa37c_Out_3;
    Unity_Remap_float(_FresnelEffect_6dabda9e9184008dbdfce2ac745c66c0_Out_3, float2 (0, 0.5), float2 (0.05, 1), _Remap_244e9fe231479c899040aad8bb5aa37c_Out_3);
    float4 _ScreenPosition_724ad830a44b8582920d49bb9a3a8049_Out_0 = IN.ScreenPosition;
    float _Split_74ec02de1ed52489b7f5d8edf6bec915_R_1 = _ScreenPosition_724ad830a44b8582920d49bb9a3a8049_Out_0[0];
    float _Split_74ec02de1ed52489b7f5d8edf6bec915_G_2 = _ScreenPosition_724ad830a44b8582920d49bb9a3a8049_Out_0[1];
    float _Split_74ec02de1ed52489b7f5d8edf6bec915_B_3 = _ScreenPosition_724ad830a44b8582920d49bb9a3a8049_Out_0[2];
    float _Split_74ec02de1ed52489b7f5d8edf6bec915_A_4 = _ScreenPosition_724ad830a44b8582920d49bb9a3a8049_Out_0[3];
    float _Property_d93f8f362dde2481a1ff9b6a41471f97_Out_0 = _Offset;
    float _Subtract_63e4def5bfbcb0819237ae1e75f6f48f_Out_2;
    Unity_Subtract_float(_Split_74ec02de1ed52489b7f5d8edf6bec915_A_4, _Property_d93f8f362dde2481a1ff9b6a41471f97_Out_0, _Subtract_63e4def5bfbcb0819237ae1e75f6f48f_Out_2);
    float _Property_bf58377199188e8494396b8339c183d9_Out_0 = _Range;
    float _Multiply_52dbf97db03cea8683a73148cc653b01_Out_2;
    Unity_Multiply_float(_Subtract_63e4def5bfbcb0819237ae1e75f6f48f_Out_2, _Property_bf58377199188e8494396b8339c183d9_Out_0, _Multiply_52dbf97db03cea8683a73148cc653b01_Out_2);
    float _Clamp_64ffae1e14feae89b94596b9cba21a55_Out_3;
    Unity_Clamp_float(_Multiply_52dbf97db03cea8683a73148cc653b01_Out_2, 0, 1, _Clamp_64ffae1e14feae89b94596b9cba21a55_Out_3);
    float _Blend_ec4b7034bfbcc28e9b21ae7806e93882_Out_2;
    Unity_Blend_Overwrite_float(_Remap_244e9fe231479c899040aad8bb5aa37c_Out_3, 1, _Blend_ec4b7034bfbcc28e9b21ae7806e93882_Out_2, _Clamp_64ffae1e14feae89b94596b9cba21a55_Out_3);
    float Slider_9331cbdecf72558e8c14e4d3f9a7360e = 0;
    surface.Alpha = _Blend_ec4b7034bfbcc28e9b21ae7806e93882_Out_2;
    surface.AlphaClipThreshold = Slider_9331cbdecf72558e8c14e4d3f9a7360e;
    return surface;
}

// --------------------------------------------------
// Build Graph Inputs

VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
{
    VertexDescriptionInputs output;
    ZERO_INITIALIZE(VertexDescriptionInputs, output);

    output.ObjectSpaceNormal = input.normalOS;
    output.ObjectSpaceTangent = input.tangentOS.xyz;
    output.ObjectSpacePosition = input.positionOS;

    return output;
}
    SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
{
    SurfaceDescriptionInputs output;
    ZERO_INITIALIZE(SurfaceDescriptionInputs, output);

    // must use interpolated tangent, bitangent and normal before they are normalized in the pixel shader.
    float3 unnormalizedNormalWS = input.normalWS;
    const float renormFactor = 1.0 / length(unnormalizedNormalWS);


    output.WorldSpaceNormal = renormFactor * input.normalWS.xyz;		// we want a unit length Normal Vector node in shader graph


    output.WorldSpaceViewDirection = input.viewDirectionWS; //TODO: by default normalized in HD, but not in universal
    output.WorldSpacePosition = input.positionWS;
    output.ScreenPosition = ComputeScreenPos(TransformWorldToHClip(input.positionWS), _ProjectionParams.x);
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

    #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"
#include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
#include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/DepthOnlyPass.hlsl"

    ENDHLSL
}
Pass
{
    Name "DepthNormals"
    Tags
    {
        "LightMode" = "DepthNormals"
    }

        // Render State
        Cull Back
    Blend SrcAlpha OneMinusSrcAlpha, One OneMinusSrcAlpha
    ZTest LEqual
    ZWrite On

        // Debug
        // <None>

        // --------------------------------------------------
        // Pass

        HLSLPROGRAM

        // Pragmas
        #pragma target 4.5
    #pragma exclude_renderers gles gles3 glcore
    #pragma multi_compile_instancing
    #pragma multi_compile _ DOTS_INSTANCING_ON
    #pragma vertex vert
    #pragma fragment frag

        // DotsInstancingOptions: <None>
        // HybridV1InjectedBuiltinProperties: <None>

        // Keywords
        // PassKeywords: <None>
        // GraphKeywords: <None>

        // Defines
        #define _SURFACE_TYPE_TRANSPARENT 1
        #define _AlphaClip 1
        #define _NORMALMAP 1
        #define _NORMAL_DROPOFF_TS 1
        #define ATTRIBUTES_NEED_NORMAL
        #define ATTRIBUTES_NEED_TANGENT
        #define ATTRIBUTES_NEED_TEXCOORD1
        #define VARYINGS_NEED_POSITION_WS
        #define VARYINGS_NEED_NORMAL_WS
        #define VARYINGS_NEED_TANGENT_WS
        #define VARYINGS_NEED_VIEWDIRECTION_WS
        #define FEATURES_GRAPH_VERTEX
        /* WARNING: $splice Could not find named fragment 'PassInstancing' */
        #define SHADERPASS SHADERPASS_DEPTHNORMALSONLY
        /* WARNING: $splice Could not find named fragment 'DotsInstancingVars' */

        // Includes
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
    #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
    #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
    #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
    #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
    #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"

        // --------------------------------------------------
        // Structs and Packing

        struct Attributes
    {
        float3 positionOS : POSITION;
        float3 normalOS : NORMAL;
        float4 tangentOS : TANGENT;
        float4 uv1 : TEXCOORD1;
        #if UNITY_ANY_INSTANCING_ENABLED
        uint instanceID : INSTANCEID_SEMANTIC;
        #endif
    };
    struct Varyings
    {
        float4 positionCS : SV_POSITION;
        float3 positionWS;
        float3 normalWS;
        float4 tangentWS;
        float3 viewDirectionWS;
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
        float3 WorldSpaceNormal;
        float3 TangentSpaceNormal;
        float3 WorldSpaceViewDirection;
        float3 WorldSpacePosition;
        float4 ScreenPosition;
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
        float3 interp0 : TEXCOORD0;
        float3 interp1 : TEXCOORD1;
        float4 interp2 : TEXCOORD2;
        float3 interp3 : TEXCOORD3;
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

        PackedVaryings PackVaryings(Varyings input)
    {
        PackedVaryings output;
        output.positionCS = input.positionCS;
        output.interp0.xyz = input.positionWS;
        output.interp1.xyz = input.normalWS;
        output.interp2.xyzw = input.tangentWS;
        output.interp3.xyz = input.viewDirectionWS;
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
    Varyings UnpackVaryings(PackedVaryings input)
    {
        Varyings output;
        output.positionCS = input.positionCS;
        output.positionWS = input.interp0.xyz;
        output.normalWS = input.interp1.xyz;
        output.tangentWS = input.interp2.xyzw;
        output.viewDirectionWS = input.interp3.xyz;
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
float4 mainTexture_TexelSize;
float4 _MainColor;
float4 _EmissionColor;
float4 _FrenelColor;
float _Frenel;
float _Metalic;
float _Smoothness;
float4 _Occ_TexelSize;
float _Offset;
float _Range;
float Neon;
float Neon_1;
CBUFFER_END

// Object and Global properties
SAMPLER(SamplerState_Linear_Repeat);
SAMPLER(SamplerState_Trilinear_Clamp);
TEXTURE2D(mainTexture);
SAMPLER(samplermainTexture);
TEXTURE2D(_Occ);
SAMPLER(sampler_Occ);

// Graph Functions

void Unity_FresnelEffect_float(float3 Normal, float3 ViewDir, float Power, out float Out)
{
    Out = pow((1.0 - saturate(dot(normalize(Normal), normalize(ViewDir)))), Power);
}

void Unity_Remap_float(float In, float2 InMinMax, float2 OutMinMax, out float Out)
{
    Out = OutMinMax.x + (In - InMinMax.x) * (OutMinMax.y - OutMinMax.x) / (InMinMax.y - InMinMax.x);
}

void Unity_Subtract_float(float A, float B, out float Out)
{
    Out = A - B;
}

void Unity_Multiply_float(float A, float B, out float Out)
{
    Out = A * B;
}

void Unity_Clamp_float(float In, float Min, float Max, out float Out)
{
    Out = clamp(In, Min, Max);
}

void Unity_Blend_Overwrite_float(float Base, float Blend, out float Out, float Opacity)
{
    Out = lerp(Base, Blend, Opacity);
}

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

// Graph Pixel
struct SurfaceDescription
{
    float3 NormalTS;
    float Alpha;
    float AlphaClipThreshold;
};

SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
{
    SurfaceDescription surface = (SurfaceDescription)0;
    float _FresnelEffect_6dabda9e9184008dbdfce2ac745c66c0_Out_3;
    Unity_FresnelEffect_float(IN.WorldSpaceNormal, IN.WorldSpaceViewDirection, 10, _FresnelEffect_6dabda9e9184008dbdfce2ac745c66c0_Out_3);
    float _Remap_244e9fe231479c899040aad8bb5aa37c_Out_3;
    Unity_Remap_float(_FresnelEffect_6dabda9e9184008dbdfce2ac745c66c0_Out_3, float2 (0, 0.5), float2 (0.05, 1), _Remap_244e9fe231479c899040aad8bb5aa37c_Out_3);
    float4 _ScreenPosition_724ad830a44b8582920d49bb9a3a8049_Out_0 = IN.ScreenPosition;
    float _Split_74ec02de1ed52489b7f5d8edf6bec915_R_1 = _ScreenPosition_724ad830a44b8582920d49bb9a3a8049_Out_0[0];
    float _Split_74ec02de1ed52489b7f5d8edf6bec915_G_2 = _ScreenPosition_724ad830a44b8582920d49bb9a3a8049_Out_0[1];
    float _Split_74ec02de1ed52489b7f5d8edf6bec915_B_3 = _ScreenPosition_724ad830a44b8582920d49bb9a3a8049_Out_0[2];
    float _Split_74ec02de1ed52489b7f5d8edf6bec915_A_4 = _ScreenPosition_724ad830a44b8582920d49bb9a3a8049_Out_0[3];
    float _Property_d93f8f362dde2481a1ff9b6a41471f97_Out_0 = _Offset;
    float _Subtract_63e4def5bfbcb0819237ae1e75f6f48f_Out_2;
    Unity_Subtract_float(_Split_74ec02de1ed52489b7f5d8edf6bec915_A_4, _Property_d93f8f362dde2481a1ff9b6a41471f97_Out_0, _Subtract_63e4def5bfbcb0819237ae1e75f6f48f_Out_2);
    float _Property_bf58377199188e8494396b8339c183d9_Out_0 = _Range;
    float _Multiply_52dbf97db03cea8683a73148cc653b01_Out_2;
    Unity_Multiply_float(_Subtract_63e4def5bfbcb0819237ae1e75f6f48f_Out_2, _Property_bf58377199188e8494396b8339c183d9_Out_0, _Multiply_52dbf97db03cea8683a73148cc653b01_Out_2);
    float _Clamp_64ffae1e14feae89b94596b9cba21a55_Out_3;
    Unity_Clamp_float(_Multiply_52dbf97db03cea8683a73148cc653b01_Out_2, 0, 1, _Clamp_64ffae1e14feae89b94596b9cba21a55_Out_3);
    float _Blend_ec4b7034bfbcc28e9b21ae7806e93882_Out_2;
    Unity_Blend_Overwrite_float(_Remap_244e9fe231479c899040aad8bb5aa37c_Out_3, 1, _Blend_ec4b7034bfbcc28e9b21ae7806e93882_Out_2, _Clamp_64ffae1e14feae89b94596b9cba21a55_Out_3);
    float Slider_9331cbdecf72558e8c14e4d3f9a7360e = 0;
    surface.NormalTS = IN.TangentSpaceNormal;
    surface.Alpha = _Blend_ec4b7034bfbcc28e9b21ae7806e93882_Out_2;
    surface.AlphaClipThreshold = Slider_9331cbdecf72558e8c14e4d3f9a7360e;
    return surface;
}

// --------------------------------------------------
// Build Graph Inputs

VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
{
    VertexDescriptionInputs output;
    ZERO_INITIALIZE(VertexDescriptionInputs, output);

    output.ObjectSpaceNormal = input.normalOS;
    output.ObjectSpaceTangent = input.tangentOS.xyz;
    output.ObjectSpacePosition = input.positionOS;

    return output;
}
    SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
{
    SurfaceDescriptionInputs output;
    ZERO_INITIALIZE(SurfaceDescriptionInputs, output);

    // must use interpolated tangent, bitangent and normal before they are normalized in the pixel shader.
    float3 unnormalizedNormalWS = input.normalWS;
    const float renormFactor = 1.0 / length(unnormalizedNormalWS);


    output.WorldSpaceNormal = renormFactor * input.normalWS.xyz;		// we want a unit length Normal Vector node in shader graph
    output.TangentSpaceNormal = float3(0.0f, 0.0f, 1.0f);


    output.WorldSpaceViewDirection = input.viewDirectionWS; //TODO: by default normalized in HD, but not in universal
    output.WorldSpacePosition = input.positionWS;
    output.ScreenPosition = ComputeScreenPos(TransformWorldToHClip(input.positionWS), _ProjectionParams.x);
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

    #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"
#include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
#include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/DepthNormalsOnlyPass.hlsl"

    ENDHLSL
}
Pass
{
    Name "Meta"
    Tags
    {
        "LightMode" = "Meta"
    }

        // Render State
        Cull Off

        // Debug
        // <None>

        // --------------------------------------------------
        // Pass

        HLSLPROGRAM

        // Pragmas
        #pragma target 4.5
    #pragma exclude_renderers gles gles3 glcore
    #pragma vertex vert
    #pragma fragment frag

        // DotsInstancingOptions: <None>
        // HybridV1InjectedBuiltinProperties: <None>

        // Keywords
        #pragma shader_feature _ _SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A
        // GraphKeywords: <None>

        // Defines
        #define _SURFACE_TYPE_TRANSPARENT 1
        #define _AlphaClip 1
        #define _NORMALMAP 1
        #define _NORMAL_DROPOFF_TS 1
        #define ATTRIBUTES_NEED_NORMAL
        #define ATTRIBUTES_NEED_TANGENT
        #define ATTRIBUTES_NEED_TEXCOORD0
        #define ATTRIBUTES_NEED_TEXCOORD1
        #define ATTRIBUTES_NEED_TEXCOORD2
        #define VARYINGS_NEED_POSITION_WS
        #define VARYINGS_NEED_NORMAL_WS
        #define VARYINGS_NEED_TEXCOORD0
        #define VARYINGS_NEED_VIEWDIRECTION_WS
        #define FEATURES_GRAPH_VERTEX
        /* WARNING: $splice Could not find named fragment 'PassInstancing' */
        #define SHADERPASS SHADERPASS_META
        /* WARNING: $splice Could not find named fragment 'DotsInstancingVars' */

        // Includes
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
    #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
    #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
    #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
    #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
    #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
    #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/MetaInput.hlsl"

        // --------------------------------------------------
        // Structs and Packing

        struct Attributes
    {
        float3 positionOS : POSITION;
        float3 normalOS : NORMAL;
        float4 tangentOS : TANGENT;
        float4 uv0 : TEXCOORD0;
        float4 uv1 : TEXCOORD1;
        float4 uv2 : TEXCOORD2;
        #if UNITY_ANY_INSTANCING_ENABLED
        uint instanceID : INSTANCEID_SEMANTIC;
        #endif
    };
    struct Varyings
    {
        float4 positionCS : SV_POSITION;
        float3 positionWS;
        float3 normalWS;
        float4 texCoord0;
        float3 viewDirectionWS;
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
        float3 WorldSpaceNormal;
        float3 WorldSpaceViewDirection;
        float3 WorldSpacePosition;
        float4 ScreenPosition;
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
        float3 interp0 : TEXCOORD0;
        float3 interp1 : TEXCOORD1;
        float4 interp2 : TEXCOORD2;
        float3 interp3 : TEXCOORD3;
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

        PackedVaryings PackVaryings(Varyings input)
    {
        PackedVaryings output;
        output.positionCS = input.positionCS;
        output.interp0.xyz = input.positionWS;
        output.interp1.xyz = input.normalWS;
        output.interp2.xyzw = input.texCoord0;
        output.interp3.xyz = input.viewDirectionWS;
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
    Varyings UnpackVaryings(PackedVaryings input)
    {
        Varyings output;
        output.positionCS = input.positionCS;
        output.positionWS = input.interp0.xyz;
        output.normalWS = input.interp1.xyz;
        output.texCoord0 = input.interp2.xyzw;
        output.viewDirectionWS = input.interp3.xyz;
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
float4 mainTexture_TexelSize;
float4 _MainColor;
float4 _EmissionColor;
float4 _FrenelColor;
float _Frenel;
float _Metalic;
float _Smoothness;
float4 _Occ_TexelSize;
float _Offset;
float _Range;
float Neon;
float Neon_1;
CBUFFER_END

// Object and Global properties
SAMPLER(SamplerState_Linear_Repeat);
SAMPLER(SamplerState_Trilinear_Clamp);
TEXTURE2D(mainTexture);
SAMPLER(samplermainTexture);
TEXTURE2D(_Occ);
SAMPLER(sampler_Occ);

// Graph Functions

void Unity_Blend_Overlay_float4(float4 Base, float4 Blend, out float4 Out, float Opacity)
{
    float4 result1 = 1.0 - 2.0 * (1.0 - Base) * (1.0 - Blend);
    float4 result2 = 2.0 * Base * Blend;
    float4 zeroOrOne = step(Base, 0.5);
    Out = result2 * zeroOrOne + (1 - zeroOrOne) * result1;
    Out = lerp(Base, Out, Opacity);
}

void Unity_Branch_float4(float Predicate, float4 True, float4 False, out float4 Out)
{
    Out = Predicate ? True : False;
}

void Unity_Remap_float4(float4 In, float2 InMinMax, float2 OutMinMax, out float4 Out)
{
    Out = OutMinMax.x + (In - InMinMax.x) * (OutMinMax.y - OutMinMax.x) / (InMinMax.y - InMinMax.x);
}

void Unity_OneMinus_float(float In, out float Out)
{
    Out = 1 - In;
}

void Unity_FresnelEffect_float(float3 Normal, float3 ViewDir, float Power, out float Out)
{
    Out = pow((1.0 - saturate(dot(normalize(Normal), normalize(ViewDir)))), Power);
}

void Unity_Power_float(float A, float B, out float Out)
{
    Out = pow(A, B);
}

void Unity_Blend_Overwrite_float(float Base, float Blend, out float Out, float Opacity)
{
    Out = lerp(Base, Blend, Opacity);
}

void Unity_Blend_Multiply_float4(float4 Base, float4 Blend, out float4 Out, float Opacity)
{
    Out = Base * Blend;
    Out = lerp(Base, Out, Opacity);
}

void Unity_Multiply_float(float4 A, float4 B, out float4 Out)
{
    Out = A * B;
}

void Unity_Blend_Lighten_float4(float4 Base, float4 Blend, out float4 Out, float Opacity)
{
    Out = max(Blend, Base);
    Out = lerp(Base, Out, Opacity);
}

void Unity_Subtract_float(float A, float B, out float Out)
{
    Out = A - B;
}

void Unity_Multiply_float(float A, float B, out float Out)
{
    Out = A * B;
}

void Unity_Clamp_float(float In, float Min, float Max, out float Out)
{
    Out = clamp(In, Min, Max);
}

void Unity_Blend_Overwrite_float4(float4 Base, float4 Blend, out float4 Out, float Opacity)
{
    Out = lerp(Base, Blend, Opacity);
}

void Unity_Remap_float(float In, float2 InMinMax, float2 OutMinMax, out float Out)
{
    Out = OutMinMax.x + (In - InMinMax.x) * (OutMinMax.y - OutMinMax.x) / (InMinMax.y - InMinMax.x);
}

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

// Graph Pixel
struct SurfaceDescription
{
    float3 BaseColor;
    float3 Emission;
    float Alpha;
    float AlphaClipThreshold;
};

SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
{
    SurfaceDescription surface = (SurfaceDescription)0;
    float _Property_db0d6ba2317f4ca885fffc369f6b7572_Out_0 = Neon_1;
    float4 _Property_404b2b1af018418b81f010f388917a24_Out_0 = _MainColor;
    UnityTexture2D _Property_9c598a886ed15f8aa999f5dbaa5a4e25_Out_0 = UnityBuildTexture2DStructNoScale(mainTexture);
    float4 _SampleTexture2D_9a0e43a036a3828e8771aff20cdda3ba_RGBA_0 = SAMPLE_TEXTURE2D(_Property_9c598a886ed15f8aa999f5dbaa5a4e25_Out_0.tex, _Property_9c598a886ed15f8aa999f5dbaa5a4e25_Out_0.samplerstate, IN.uv0.xy);
    float _SampleTexture2D_9a0e43a036a3828e8771aff20cdda3ba_R_4 = _SampleTexture2D_9a0e43a036a3828e8771aff20cdda3ba_RGBA_0.r;
    float _SampleTexture2D_9a0e43a036a3828e8771aff20cdda3ba_G_5 = _SampleTexture2D_9a0e43a036a3828e8771aff20cdda3ba_RGBA_0.g;
    float _SampleTexture2D_9a0e43a036a3828e8771aff20cdda3ba_B_6 = _SampleTexture2D_9a0e43a036a3828e8771aff20cdda3ba_RGBA_0.b;
    float _SampleTexture2D_9a0e43a036a3828e8771aff20cdda3ba_A_7 = _SampleTexture2D_9a0e43a036a3828e8771aff20cdda3ba_RGBA_0.a;
    float4 _Blend_07c1433efd838d88a6692dd2f3fa5cc1_Out_2;
    Unity_Blend_Overlay_float4(_SampleTexture2D_9a0e43a036a3828e8771aff20cdda3ba_RGBA_0, _Property_404b2b1af018418b81f010f388917a24_Out_0, _Blend_07c1433efd838d88a6692dd2f3fa5cc1_Out_2, 1);
    float4 _Branch_0d2a1beb339f47d7a3370ab32efd8e0c_Out_3;
    Unity_Branch_float4(_Property_db0d6ba2317f4ca885fffc369f6b7572_Out_0, _Property_404b2b1af018418b81f010f388917a24_Out_0, _Blend_07c1433efd838d88a6692dd2f3fa5cc1_Out_2, _Branch_0d2a1beb339f47d7a3370ab32efd8e0c_Out_3);
    float _Property_0d8e02a76989bb80ae497d44ac78381b_Out_0 = Neon;
    float4 _Remap_6823e14564e9eb8a8ef71bcec591d781_Out_3;
    Unity_Remap_float4(_SampleTexture2D_9a0e43a036a3828e8771aff20cdda3ba_RGBA_0, float2 (0, 1), float2 (0, 2), _Remap_6823e14564e9eb8a8ef71bcec591d781_Out_3);
    float4 Color_1d0b78cf07232483869322c6e5134511 = IsGammaSpace() ? float4(0.1885618, 0.6377826, 1.059274, 1) : float4(SRGBToLinear(float3(0.1885618, 0.6377826, 1.059274)), 1);
    float4 _Property_976c2b04f54b068091004d51cb39a1bb_Out_0 = _EmissionColor;
    float _Property_bc4a96833916af8fab203c56c0b44955_Out_0 = _Frenel;
    float _OneMinus_00c02da1a3e44983954a2b94c04bbe8d_Out_1;
    Unity_OneMinus_float(_Property_bc4a96833916af8fab203c56c0b44955_Out_0, _OneMinus_00c02da1a3e44983954a2b94c04bbe8d_Out_1);
    float _FresnelEffect_2e0aeb0e30852e8686f75ede044d7ce7_Out_3;
    Unity_FresnelEffect_float(IN.WorldSpaceNormal, IN.WorldSpaceViewDirection, _OneMinus_00c02da1a3e44983954a2b94c04bbe8d_Out_1, _FresnelEffect_2e0aeb0e30852e8686f75ede044d7ce7_Out_3);
    float _Power_c20579372975d58d9e956dc03e906821_Out_2;
    Unity_Power_float(_FresnelEffect_2e0aeb0e30852e8686f75ede044d7ce7_Out_3, 2, _Power_c20579372975d58d9e956dc03e906821_Out_2);
    float _Blend_927874c56b8f15888bfe6eb2b1d8c1f3_Out_2;
    Unity_Blend_Overwrite_float(0, _Power_c20579372975d58d9e956dc03e906821_Out_2, _Blend_927874c56b8f15888bfe6eb2b1d8c1f3_Out_2, 1);
    float4 _Property_337d8379e365b1859545f24d89eb283d_Out_0 = _FrenelColor;
    float4 _Blend_b52462f55ca3fa8e8123560ad96f591f_Out_2;
    Unity_Blend_Multiply_float4((_Blend_927874c56b8f15888bfe6eb2b1d8c1f3_Out_2.xxxx), _Property_337d8379e365b1859545f24d89eb283d_Out_0, _Blend_b52462f55ca3fa8e8123560ad96f591f_Out_2, 1);
    float4 _Multiply_8d1b6059d5faec85ae378ee724261100_Out_2;
    Unity_Multiply_float(_Blend_b52462f55ca3fa8e8123560ad96f591f_Out_2, float4(1, 1, 1, 1), _Multiply_8d1b6059d5faec85ae378ee724261100_Out_2);
    float4 _Blend_71fa99a490669a8789492c7ff5006154_Out_2;
    Unity_Blend_Lighten_float4(_Property_976c2b04f54b068091004d51cb39a1bb_Out_0, _Multiply_8d1b6059d5faec85ae378ee724261100_Out_2, _Blend_71fa99a490669a8789492c7ff5006154_Out_2, 1);
    float4 _ScreenPosition_724ad830a44b8582920d49bb9a3a8049_Out_0 = IN.ScreenPosition;
    float _Split_74ec02de1ed52489b7f5d8edf6bec915_R_1 = _ScreenPosition_724ad830a44b8582920d49bb9a3a8049_Out_0[0];
    float _Split_74ec02de1ed52489b7f5d8edf6bec915_G_2 = _ScreenPosition_724ad830a44b8582920d49bb9a3a8049_Out_0[1];
    float _Split_74ec02de1ed52489b7f5d8edf6bec915_B_3 = _ScreenPosition_724ad830a44b8582920d49bb9a3a8049_Out_0[2];
    float _Split_74ec02de1ed52489b7f5d8edf6bec915_A_4 = _ScreenPosition_724ad830a44b8582920d49bb9a3a8049_Out_0[3];
    float _Property_d93f8f362dde2481a1ff9b6a41471f97_Out_0 = _Offset;
    float _Subtract_63e4def5bfbcb0819237ae1e75f6f48f_Out_2;
    Unity_Subtract_float(_Split_74ec02de1ed52489b7f5d8edf6bec915_A_4, _Property_d93f8f362dde2481a1ff9b6a41471f97_Out_0, _Subtract_63e4def5bfbcb0819237ae1e75f6f48f_Out_2);
    float _Property_bf58377199188e8494396b8339c183d9_Out_0 = _Range;
    float _Multiply_52dbf97db03cea8683a73148cc653b01_Out_2;
    Unity_Multiply_float(_Subtract_63e4def5bfbcb0819237ae1e75f6f48f_Out_2, _Property_bf58377199188e8494396b8339c183d9_Out_0, _Multiply_52dbf97db03cea8683a73148cc653b01_Out_2);
    float _Clamp_64ffae1e14feae89b94596b9cba21a55_Out_3;
    Unity_Clamp_float(_Multiply_52dbf97db03cea8683a73148cc653b01_Out_2, 0, 1, _Clamp_64ffae1e14feae89b94596b9cba21a55_Out_3);
    float4 _Blend_2a8b33610ca45d8389ae5bca9784d7de_Out_2;
    Unity_Blend_Overwrite_float4(Color_1d0b78cf07232483869322c6e5134511, _Blend_71fa99a490669a8789492c7ff5006154_Out_2, _Blend_2a8b33610ca45d8389ae5bca9784d7de_Out_2, _Clamp_64ffae1e14feae89b94596b9cba21a55_Out_3);
    float4 _Branch_4e6bcacf8fcb5585abae3c176af03007_Out_3;
    Unity_Branch_float4(_Property_0d8e02a76989bb80ae497d44ac78381b_Out_0, _Remap_6823e14564e9eb8a8ef71bcec591d781_Out_3, _Blend_2a8b33610ca45d8389ae5bca9784d7de_Out_2, _Branch_4e6bcacf8fcb5585abae3c176af03007_Out_3);
    float _FresnelEffect_6dabda9e9184008dbdfce2ac745c66c0_Out_3;
    Unity_FresnelEffect_float(IN.WorldSpaceNormal, IN.WorldSpaceViewDirection, 10, _FresnelEffect_6dabda9e9184008dbdfce2ac745c66c0_Out_3);
    float _Remap_244e9fe231479c899040aad8bb5aa37c_Out_3;
    Unity_Remap_float(_FresnelEffect_6dabda9e9184008dbdfce2ac745c66c0_Out_3, float2 (0, 0.5), float2 (0.05, 1), _Remap_244e9fe231479c899040aad8bb5aa37c_Out_3);
    float _Blend_ec4b7034bfbcc28e9b21ae7806e93882_Out_2;
    Unity_Blend_Overwrite_float(_Remap_244e9fe231479c899040aad8bb5aa37c_Out_3, 1, _Blend_ec4b7034bfbcc28e9b21ae7806e93882_Out_2, _Clamp_64ffae1e14feae89b94596b9cba21a55_Out_3);
    float Slider_9331cbdecf72558e8c14e4d3f9a7360e = 0;
    surface.BaseColor = (_Branch_0d2a1beb339f47d7a3370ab32efd8e0c_Out_3.xyz);
    surface.Emission = (_Branch_4e6bcacf8fcb5585abae3c176af03007_Out_3.xyz);
    surface.Alpha = _Blend_ec4b7034bfbcc28e9b21ae7806e93882_Out_2;
    surface.AlphaClipThreshold = Slider_9331cbdecf72558e8c14e4d3f9a7360e;
    return surface;
}

// --------------------------------------------------
// Build Graph Inputs

VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
{
    VertexDescriptionInputs output;
    ZERO_INITIALIZE(VertexDescriptionInputs, output);

    output.ObjectSpaceNormal = input.normalOS;
    output.ObjectSpaceTangent = input.tangentOS.xyz;
    output.ObjectSpacePosition = input.positionOS;

    return output;
}
    SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
{
    SurfaceDescriptionInputs output;
    ZERO_INITIALIZE(SurfaceDescriptionInputs, output);

    // must use interpolated tangent, bitangent and normal before they are normalized in the pixel shader.
    float3 unnormalizedNormalWS = input.normalWS;
    const float renormFactor = 1.0 / length(unnormalizedNormalWS);


    output.WorldSpaceNormal = renormFactor * input.normalWS.xyz;		// we want a unit length Normal Vector node in shader graph


    output.WorldSpaceViewDirection = input.viewDirectionWS; //TODO: by default normalized in HD, but not in universal
    output.WorldSpacePosition = input.positionWS;
    output.ScreenPosition = ComputeScreenPos(TransformWorldToHClip(input.positionWS), _ProjectionParams.x);
    output.uv0 = input.texCoord0;
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

    #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"
#include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
#include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/LightingMetaPass.hlsl"

    ENDHLSL
}
Pass
{
        // Name: <None>
        Tags
        {
            "LightMode" = "Universal2D"
        }

        // Render State
        Cull Back
    Blend SrcAlpha OneMinusSrcAlpha, One OneMinusSrcAlpha
    ZTest LEqual
    ZWrite Off

        // Debug
        // <None>

        // --------------------------------------------------
        // Pass

        HLSLPROGRAM

        // Pragmas
        #pragma target 4.5
    #pragma exclude_renderers gles gles3 glcore
    #pragma vertex vert
    #pragma fragment frag

        // DotsInstancingOptions: <None>
        // HybridV1InjectedBuiltinProperties: <None>

        // Keywords
        // PassKeywords: <None>
        // GraphKeywords: <None>

        // Defines
        #define _SURFACE_TYPE_TRANSPARENT 1
        #define _AlphaClip 1
        #define _NORMALMAP 1
        #define _NORMAL_DROPOFF_TS 1
        #define ATTRIBUTES_NEED_NORMAL
        #define ATTRIBUTES_NEED_TANGENT
        #define ATTRIBUTES_NEED_TEXCOORD0
        #define VARYINGS_NEED_POSITION_WS
        #define VARYINGS_NEED_NORMAL_WS
        #define VARYINGS_NEED_TEXCOORD0
        #define VARYINGS_NEED_VIEWDIRECTION_WS
        #define FEATURES_GRAPH_VERTEX
        /* WARNING: $splice Could not find named fragment 'PassInstancing' */
        #define SHADERPASS SHADERPASS_2D
        /* WARNING: $splice Could not find named fragment 'DotsInstancingVars' */

        // Includes
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
    #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
    #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
    #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
    #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
    #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"

        // --------------------------------------------------
        // Structs and Packing

        struct Attributes
    {
        float3 positionOS : POSITION;
        float3 normalOS : NORMAL;
        float4 tangentOS : TANGENT;
        float4 uv0 : TEXCOORD0;
        #if UNITY_ANY_INSTANCING_ENABLED
        uint instanceID : INSTANCEID_SEMANTIC;
        #endif
    };
    struct Varyings
    {
        float4 positionCS : SV_POSITION;
        float3 positionWS;
        float3 normalWS;
        float4 texCoord0;
        float3 viewDirectionWS;
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
        float3 WorldSpaceNormal;
        float3 WorldSpaceViewDirection;
        float3 WorldSpacePosition;
        float4 ScreenPosition;
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
        float3 interp0 : TEXCOORD0;
        float3 interp1 : TEXCOORD1;
        float4 interp2 : TEXCOORD2;
        float3 interp3 : TEXCOORD3;
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

        PackedVaryings PackVaryings(Varyings input)
    {
        PackedVaryings output;
        output.positionCS = input.positionCS;
        output.interp0.xyz = input.positionWS;
        output.interp1.xyz = input.normalWS;
        output.interp2.xyzw = input.texCoord0;
        output.interp3.xyz = input.viewDirectionWS;
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
    Varyings UnpackVaryings(PackedVaryings input)
    {
        Varyings output;
        output.positionCS = input.positionCS;
        output.positionWS = input.interp0.xyz;
        output.normalWS = input.interp1.xyz;
        output.texCoord0 = input.interp2.xyzw;
        output.viewDirectionWS = input.interp3.xyz;
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
float4 mainTexture_TexelSize;
float4 _MainColor;
float4 _EmissionColor;
float4 _FrenelColor;
float _Frenel;
float _Metalic;
float _Smoothness;
float4 _Occ_TexelSize;
float _Offset;
float _Range;
float Neon;
float Neon_1;
CBUFFER_END

// Object and Global properties
SAMPLER(SamplerState_Linear_Repeat);
SAMPLER(SamplerState_Trilinear_Clamp);
TEXTURE2D(mainTexture);
SAMPLER(samplermainTexture);
TEXTURE2D(_Occ);
SAMPLER(sampler_Occ);

// Graph Functions

void Unity_Blend_Overlay_float4(float4 Base, float4 Blend, out float4 Out, float Opacity)
{
    float4 result1 = 1.0 - 2.0 * (1.0 - Base) * (1.0 - Blend);
    float4 result2 = 2.0 * Base * Blend;
    float4 zeroOrOne = step(Base, 0.5);
    Out = result2 * zeroOrOne + (1 - zeroOrOne) * result1;
    Out = lerp(Base, Out, Opacity);
}

void Unity_Branch_float4(float Predicate, float4 True, float4 False, out float4 Out)
{
    Out = Predicate ? True : False;
}

void Unity_FresnelEffect_float(float3 Normal, float3 ViewDir, float Power, out float Out)
{
    Out = pow((1.0 - saturate(dot(normalize(Normal), normalize(ViewDir)))), Power);
}

void Unity_Remap_float(float In, float2 InMinMax, float2 OutMinMax, out float Out)
{
    Out = OutMinMax.x + (In - InMinMax.x) * (OutMinMax.y - OutMinMax.x) / (InMinMax.y - InMinMax.x);
}

void Unity_Subtract_float(float A, float B, out float Out)
{
    Out = A - B;
}

void Unity_Multiply_float(float A, float B, out float Out)
{
    Out = A * B;
}

void Unity_Clamp_float(float In, float Min, float Max, out float Out)
{
    Out = clamp(In, Min, Max);
}

void Unity_Blend_Overwrite_float(float Base, float Blend, out float Out, float Opacity)
{
    Out = lerp(Base, Blend, Opacity);
}

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

// Graph Pixel
struct SurfaceDescription
{
    float3 BaseColor;
    float Alpha;
    float AlphaClipThreshold;
};

SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
{
    SurfaceDescription surface = (SurfaceDescription)0;
    float _Property_db0d6ba2317f4ca885fffc369f6b7572_Out_0 = Neon_1;
    float4 _Property_404b2b1af018418b81f010f388917a24_Out_0 = _MainColor;
    UnityTexture2D _Property_9c598a886ed15f8aa999f5dbaa5a4e25_Out_0 = UnityBuildTexture2DStructNoScale(mainTexture);
    float4 _SampleTexture2D_9a0e43a036a3828e8771aff20cdda3ba_RGBA_0 = SAMPLE_TEXTURE2D(_Property_9c598a886ed15f8aa999f5dbaa5a4e25_Out_0.tex, _Property_9c598a886ed15f8aa999f5dbaa5a4e25_Out_0.samplerstate, IN.uv0.xy);
    float _SampleTexture2D_9a0e43a036a3828e8771aff20cdda3ba_R_4 = _SampleTexture2D_9a0e43a036a3828e8771aff20cdda3ba_RGBA_0.r;
    float _SampleTexture2D_9a0e43a036a3828e8771aff20cdda3ba_G_5 = _SampleTexture2D_9a0e43a036a3828e8771aff20cdda3ba_RGBA_0.g;
    float _SampleTexture2D_9a0e43a036a3828e8771aff20cdda3ba_B_6 = _SampleTexture2D_9a0e43a036a3828e8771aff20cdda3ba_RGBA_0.b;
    float _SampleTexture2D_9a0e43a036a3828e8771aff20cdda3ba_A_7 = _SampleTexture2D_9a0e43a036a3828e8771aff20cdda3ba_RGBA_0.a;
    float4 _Blend_07c1433efd838d88a6692dd2f3fa5cc1_Out_2;
    Unity_Blend_Overlay_float4(_SampleTexture2D_9a0e43a036a3828e8771aff20cdda3ba_RGBA_0, _Property_404b2b1af018418b81f010f388917a24_Out_0, _Blend_07c1433efd838d88a6692dd2f3fa5cc1_Out_2, 1);
    float4 _Branch_0d2a1beb339f47d7a3370ab32efd8e0c_Out_3;
    Unity_Branch_float4(_Property_db0d6ba2317f4ca885fffc369f6b7572_Out_0, _Property_404b2b1af018418b81f010f388917a24_Out_0, _Blend_07c1433efd838d88a6692dd2f3fa5cc1_Out_2, _Branch_0d2a1beb339f47d7a3370ab32efd8e0c_Out_3);
    float _FresnelEffect_6dabda9e9184008dbdfce2ac745c66c0_Out_3;
    Unity_FresnelEffect_float(IN.WorldSpaceNormal, IN.WorldSpaceViewDirection, 10, _FresnelEffect_6dabda9e9184008dbdfce2ac745c66c0_Out_3);
    float _Remap_244e9fe231479c899040aad8bb5aa37c_Out_3;
    Unity_Remap_float(_FresnelEffect_6dabda9e9184008dbdfce2ac745c66c0_Out_3, float2 (0, 0.5), float2 (0.05, 1), _Remap_244e9fe231479c899040aad8bb5aa37c_Out_3);
    float4 _ScreenPosition_724ad830a44b8582920d49bb9a3a8049_Out_0 = IN.ScreenPosition;
    float _Split_74ec02de1ed52489b7f5d8edf6bec915_R_1 = _ScreenPosition_724ad830a44b8582920d49bb9a3a8049_Out_0[0];
    float _Split_74ec02de1ed52489b7f5d8edf6bec915_G_2 = _ScreenPosition_724ad830a44b8582920d49bb9a3a8049_Out_0[1];
    float _Split_74ec02de1ed52489b7f5d8edf6bec915_B_3 = _ScreenPosition_724ad830a44b8582920d49bb9a3a8049_Out_0[2];
    float _Split_74ec02de1ed52489b7f5d8edf6bec915_A_4 = _ScreenPosition_724ad830a44b8582920d49bb9a3a8049_Out_0[3];
    float _Property_d93f8f362dde2481a1ff9b6a41471f97_Out_0 = _Offset;
    float _Subtract_63e4def5bfbcb0819237ae1e75f6f48f_Out_2;
    Unity_Subtract_float(_Split_74ec02de1ed52489b7f5d8edf6bec915_A_4, _Property_d93f8f362dde2481a1ff9b6a41471f97_Out_0, _Subtract_63e4def5bfbcb0819237ae1e75f6f48f_Out_2);
    float _Property_bf58377199188e8494396b8339c183d9_Out_0 = _Range;
    float _Multiply_52dbf97db03cea8683a73148cc653b01_Out_2;
    Unity_Multiply_float(_Subtract_63e4def5bfbcb0819237ae1e75f6f48f_Out_2, _Property_bf58377199188e8494396b8339c183d9_Out_0, _Multiply_52dbf97db03cea8683a73148cc653b01_Out_2);
    float _Clamp_64ffae1e14feae89b94596b9cba21a55_Out_3;
    Unity_Clamp_float(_Multiply_52dbf97db03cea8683a73148cc653b01_Out_2, 0, 1, _Clamp_64ffae1e14feae89b94596b9cba21a55_Out_3);
    float _Blend_ec4b7034bfbcc28e9b21ae7806e93882_Out_2;
    Unity_Blend_Overwrite_float(_Remap_244e9fe231479c899040aad8bb5aa37c_Out_3, 1, _Blend_ec4b7034bfbcc28e9b21ae7806e93882_Out_2, _Clamp_64ffae1e14feae89b94596b9cba21a55_Out_3);
    float Slider_9331cbdecf72558e8c14e4d3f9a7360e = 0;
    surface.BaseColor = (_Branch_0d2a1beb339f47d7a3370ab32efd8e0c_Out_3.xyz);
    surface.Alpha = _Blend_ec4b7034bfbcc28e9b21ae7806e93882_Out_2;
    surface.AlphaClipThreshold = Slider_9331cbdecf72558e8c14e4d3f9a7360e;
    return surface;
}

// --------------------------------------------------
// Build Graph Inputs

VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
{
    VertexDescriptionInputs output;
    ZERO_INITIALIZE(VertexDescriptionInputs, output);

    output.ObjectSpaceNormal = input.normalOS;
    output.ObjectSpaceTangent = input.tangentOS.xyz;
    output.ObjectSpacePosition = input.positionOS;

    return output;
}
    SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
{
    SurfaceDescriptionInputs output;
    ZERO_INITIALIZE(SurfaceDescriptionInputs, output);

    // must use interpolated tangent, bitangent and normal before they are normalized in the pixel shader.
    float3 unnormalizedNormalWS = input.normalWS;
    const float renormFactor = 1.0 / length(unnormalizedNormalWS);


    output.WorldSpaceNormal = renormFactor * input.normalWS.xyz;		// we want a unit length Normal Vector node in shader graph


    output.WorldSpaceViewDirection = input.viewDirectionWS; //TODO: by default normalized in HD, but not in universal
    output.WorldSpacePosition = input.positionWS;
    output.ScreenPosition = ComputeScreenPos(TransformWorldToHClip(input.positionWS), _ProjectionParams.x);
    output.uv0 = input.texCoord0;
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

    #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"
#include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
#include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/PBR2DPass.hlsl"

    ENDHLSL
}
    }
        SubShader
    {
        Tags
        {
            "RenderPipeline" = "UniversalPipeline"
            "RenderType" = "Transparent"
            "UniversalMaterialType" = "Lit"
            "Queue" = "Transparent"
        }
        Pass
        {
            Name "Universal Forward Only"
            Tags
            {
                "LightMode" = "UniversalForwardOnly"
            }

        // Render State
        Cull Back
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
    #pragma only_renderers gles gles3 glcore d3d11
    #pragma multi_compile_instancing
    #pragma multi_compile_fog
    #pragma vertex vert
    #pragma fragment frag

        // DotsInstancingOptions: <None>
        // HybridV1InjectedBuiltinProperties: <None>

        // Keywords
        #pragma multi_compile _ _SCREEN_SPACE_OCCLUSION
    #pragma multi_compile _ LIGHTMAP_ON
    #pragma multi_compile _ DIRLIGHTMAP_COMBINED
    #pragma multi_compile _ _MAIN_LIGHT_SHADOWS
    #pragma multi_compile _ _MAIN_LIGHT_SHADOWS_CASCADE
    #pragma multi_compile _ADDITIONAL_LIGHTS_VERTEX _ADDITIONAL_LIGHTS _ADDITIONAL_OFF
    #pragma multi_compile _ _ADDITIONAL_LIGHT_SHADOWS
    #pragma multi_compile _ _SHADOWS_SOFT
    #pragma multi_compile _ LIGHTMAP_SHADOW_MIXING
    #pragma multi_compile _ SHADOWS_SHADOWMASK
        // GraphKeywords: <None>

        // Defines
        #define _SURFACE_TYPE_TRANSPARENT 1
        #define _AlphaClip 1
        #define _NORMALMAP 1
        #define _NORMAL_DROPOFF_TS 1
        #define ATTRIBUTES_NEED_NORMAL
        #define ATTRIBUTES_NEED_TANGENT
        #define ATTRIBUTES_NEED_TEXCOORD0
        #define ATTRIBUTES_NEED_TEXCOORD1
        #define VARYINGS_NEED_POSITION_WS
        #define VARYINGS_NEED_NORMAL_WS
        #define VARYINGS_NEED_TANGENT_WS
        #define VARYINGS_NEED_TEXCOORD0
        #define VARYINGS_NEED_VIEWDIRECTION_WS
        #define VARYINGS_NEED_FOG_AND_VERTEX_LIGHT
        #define FEATURES_GRAPH_VERTEX
        /* WARNING: $splice Could not find named fragment 'PassInstancing' */
        #define SHADERPASS SHADERPASS_FORWARDONLY
        /* WARNING: $splice Could not find named fragment 'DotsInstancingVars' */

        // Includes
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
    #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
    #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
    #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
    #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
    #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Shadows.hlsl"
    #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"

        // --------------------------------------------------
        // Structs and Packing

        struct Attributes
    {
        float3 positionOS : POSITION;
        float3 normalOS : NORMAL;
        float4 tangentOS : TANGENT;
        float4 uv0 : TEXCOORD0;
        float4 uv1 : TEXCOORD1;
        #if UNITY_ANY_INSTANCING_ENABLED
        uint instanceID : INSTANCEID_SEMANTIC;
        #endif
    };
    struct Varyings
    {
        float4 positionCS : SV_POSITION;
        float3 positionWS;
        float3 normalWS;
        float4 tangentWS;
        float4 texCoord0;
        float3 viewDirectionWS;
        #if defined(LIGHTMAP_ON)
        float2 lightmapUV;
        #endif
        #if !defined(LIGHTMAP_ON)
        float3 sh;
        #endif
        float4 fogFactorAndVertexLight;
        float4 shadowCoord;
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
        float3 WorldSpaceNormal;
        float3 TangentSpaceNormal;
        float3 WorldSpaceViewDirection;
        float3 WorldSpacePosition;
        float4 ScreenPosition;
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
        float3 interp0 : TEXCOORD0;
        float3 interp1 : TEXCOORD1;
        float4 interp2 : TEXCOORD2;
        float4 interp3 : TEXCOORD3;
        float3 interp4 : TEXCOORD4;
        #if defined(LIGHTMAP_ON)
        float2 interp5 : TEXCOORD5;
        #endif
        #if !defined(LIGHTMAP_ON)
        float3 interp6 : TEXCOORD6;
        #endif
        float4 interp7 : TEXCOORD7;
        float4 interp8 : TEXCOORD8;
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

        PackedVaryings PackVaryings(Varyings input)
    {
        PackedVaryings output;
        output.positionCS = input.positionCS;
        output.interp0.xyz = input.positionWS;
        output.interp1.xyz = input.normalWS;
        output.interp2.xyzw = input.tangentWS;
        output.interp3.xyzw = input.texCoord0;
        output.interp4.xyz = input.viewDirectionWS;
        #if defined(LIGHTMAP_ON)
        output.interp5.xy = input.lightmapUV;
        #endif
        #if !defined(LIGHTMAP_ON)
        output.interp6.xyz = input.sh;
        #endif
        output.interp7.xyzw = input.fogFactorAndVertexLight;
        output.interp8.xyzw = input.shadowCoord;
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
    Varyings UnpackVaryings(PackedVaryings input)
    {
        Varyings output;
        output.positionCS = input.positionCS;
        output.positionWS = input.interp0.xyz;
        output.normalWS = input.interp1.xyz;
        output.tangentWS = input.interp2.xyzw;
        output.texCoord0 = input.interp3.xyzw;
        output.viewDirectionWS = input.interp4.xyz;
        #if defined(LIGHTMAP_ON)
        output.lightmapUV = input.interp5.xy;
        #endif
        #if !defined(LIGHTMAP_ON)
        output.sh = input.interp6.xyz;
        #endif
        output.fogFactorAndVertexLight = input.interp7.xyzw;
        output.shadowCoord = input.interp8.xyzw;
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
float4 mainTexture_TexelSize;
float4 _MainColor;
float4 _EmissionColor;
float4 _FrenelColor;
float _Frenel;
float _Metalic;
float _Smoothness;
float4 _Occ_TexelSize;
float _Offset;
float _Range;
float Neon;
float Neon_1;
CBUFFER_END

// Object and Global properties
SAMPLER(SamplerState_Linear_Repeat);
SAMPLER(SamplerState_Trilinear_Clamp);
TEXTURE2D(mainTexture);
SAMPLER(samplermainTexture);
TEXTURE2D(_Occ);
SAMPLER(sampler_Occ);

// Graph Functions

void Unity_Blend_Overlay_float4(float4 Base, float4 Blend, out float4 Out, float Opacity)
{
    float4 result1 = 1.0 - 2.0 * (1.0 - Base) * (1.0 - Blend);
    float4 result2 = 2.0 * Base * Blend;
    float4 zeroOrOne = step(Base, 0.5);
    Out = result2 * zeroOrOne + (1 - zeroOrOne) * result1;
    Out = lerp(Base, Out, Opacity);
}

void Unity_Branch_float4(float Predicate, float4 True, float4 False, out float4 Out)
{
    Out = Predicate ? True : False;
}

void Unity_Remap_float4(float4 In, float2 InMinMax, float2 OutMinMax, out float4 Out)
{
    Out = OutMinMax.x + (In - InMinMax.x) * (OutMinMax.y - OutMinMax.x) / (InMinMax.y - InMinMax.x);
}

void Unity_OneMinus_float(float In, out float Out)
{
    Out = 1 - In;
}

void Unity_FresnelEffect_float(float3 Normal, float3 ViewDir, float Power, out float Out)
{
    Out = pow((1.0 - saturate(dot(normalize(Normal), normalize(ViewDir)))), Power);
}

void Unity_Power_float(float A, float B, out float Out)
{
    Out = pow(A, B);
}

void Unity_Blend_Overwrite_float(float Base, float Blend, out float Out, float Opacity)
{
    Out = lerp(Base, Blend, Opacity);
}

void Unity_Blend_Multiply_float4(float4 Base, float4 Blend, out float4 Out, float Opacity)
{
    Out = Base * Blend;
    Out = lerp(Base, Out, Opacity);
}

void Unity_Multiply_float(float4 A, float4 B, out float4 Out)
{
    Out = A * B;
}

void Unity_Blend_Lighten_float4(float4 Base, float4 Blend, out float4 Out, float Opacity)
{
    Out = max(Blend, Base);
    Out = lerp(Base, Out, Opacity);
}

void Unity_Subtract_float(float A, float B, out float Out)
{
    Out = A - B;
}

void Unity_Multiply_float(float A, float B, out float Out)
{
    Out = A * B;
}

void Unity_Clamp_float(float In, float Min, float Max, out float Out)
{
    Out = clamp(In, Min, Max);
}

void Unity_Blend_Overwrite_float4(float4 Base, float4 Blend, out float4 Out, float Opacity)
{
    Out = lerp(Base, Blend, Opacity);
}

void Unity_TilingAndOffset_float(float2 UV, float2 Tiling, float2 Offset, out float2 Out)
{
    Out = UV * Tiling + Offset;
}

void Unity_Remap_float(float In, float2 InMinMax, float2 OutMinMax, out float Out)
{
    Out = OutMinMax.x + (In - InMinMax.x) * (OutMinMax.y - OutMinMax.x) / (InMinMax.y - InMinMax.x);
}

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

// Graph Pixel
struct SurfaceDescription
{
    float3 BaseColor;
    float3 NormalTS;
    float3 Emission;
    float Metallic;
    float Smoothness;
    float Occlusion;
    float Alpha;
    float AlphaClipThreshold;
};

SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
{
    SurfaceDescription surface = (SurfaceDescription)0;
    float _Property_db0d6ba2317f4ca885fffc369f6b7572_Out_0 = Neon_1;
    float4 _Property_404b2b1af018418b81f010f388917a24_Out_0 = _MainColor;
    UnityTexture2D _Property_9c598a886ed15f8aa999f5dbaa5a4e25_Out_0 = UnityBuildTexture2DStructNoScale(mainTexture);
    float4 _SampleTexture2D_9a0e43a036a3828e8771aff20cdda3ba_RGBA_0 = SAMPLE_TEXTURE2D(_Property_9c598a886ed15f8aa999f5dbaa5a4e25_Out_0.tex, _Property_9c598a886ed15f8aa999f5dbaa5a4e25_Out_0.samplerstate, IN.uv0.xy);
    float _SampleTexture2D_9a0e43a036a3828e8771aff20cdda3ba_R_4 = _SampleTexture2D_9a0e43a036a3828e8771aff20cdda3ba_RGBA_0.r;
    float _SampleTexture2D_9a0e43a036a3828e8771aff20cdda3ba_G_5 = _SampleTexture2D_9a0e43a036a3828e8771aff20cdda3ba_RGBA_0.g;
    float _SampleTexture2D_9a0e43a036a3828e8771aff20cdda3ba_B_6 = _SampleTexture2D_9a0e43a036a3828e8771aff20cdda3ba_RGBA_0.b;
    float _SampleTexture2D_9a0e43a036a3828e8771aff20cdda3ba_A_7 = _SampleTexture2D_9a0e43a036a3828e8771aff20cdda3ba_RGBA_0.a;
    float4 _Blend_07c1433efd838d88a6692dd2f3fa5cc1_Out_2;
    Unity_Blend_Overlay_float4(_SampleTexture2D_9a0e43a036a3828e8771aff20cdda3ba_RGBA_0, _Property_404b2b1af018418b81f010f388917a24_Out_0, _Blend_07c1433efd838d88a6692dd2f3fa5cc1_Out_2, 1);
    float4 _Branch_0d2a1beb339f47d7a3370ab32efd8e0c_Out_3;
    Unity_Branch_float4(_Property_db0d6ba2317f4ca885fffc369f6b7572_Out_0, _Property_404b2b1af018418b81f010f388917a24_Out_0, _Blend_07c1433efd838d88a6692dd2f3fa5cc1_Out_2, _Branch_0d2a1beb339f47d7a3370ab32efd8e0c_Out_3);
    float _Property_0d8e02a76989bb80ae497d44ac78381b_Out_0 = Neon;
    float4 _Remap_6823e14564e9eb8a8ef71bcec591d781_Out_3;
    Unity_Remap_float4(_SampleTexture2D_9a0e43a036a3828e8771aff20cdda3ba_RGBA_0, float2 (0, 1), float2 (0, 2), _Remap_6823e14564e9eb8a8ef71bcec591d781_Out_3);
    float4 Color_1d0b78cf07232483869322c6e5134511 = IsGammaSpace() ? float4(0.1885618, 0.6377826, 1.059274, 1) : float4(SRGBToLinear(float3(0.1885618, 0.6377826, 1.059274)), 1);
    float4 _Property_976c2b04f54b068091004d51cb39a1bb_Out_0 = _EmissionColor;
    float _Property_bc4a96833916af8fab203c56c0b44955_Out_0 = _Frenel;
    float _OneMinus_00c02da1a3e44983954a2b94c04bbe8d_Out_1;
    Unity_OneMinus_float(_Property_bc4a96833916af8fab203c56c0b44955_Out_0, _OneMinus_00c02da1a3e44983954a2b94c04bbe8d_Out_1);
    float _FresnelEffect_2e0aeb0e30852e8686f75ede044d7ce7_Out_3;
    Unity_FresnelEffect_float(IN.WorldSpaceNormal, IN.WorldSpaceViewDirection, _OneMinus_00c02da1a3e44983954a2b94c04bbe8d_Out_1, _FresnelEffect_2e0aeb0e30852e8686f75ede044d7ce7_Out_3);
    float _Power_c20579372975d58d9e956dc03e906821_Out_2;
    Unity_Power_float(_FresnelEffect_2e0aeb0e30852e8686f75ede044d7ce7_Out_3, 2, _Power_c20579372975d58d9e956dc03e906821_Out_2);
    float _Blend_927874c56b8f15888bfe6eb2b1d8c1f3_Out_2;
    Unity_Blend_Overwrite_float(0, _Power_c20579372975d58d9e956dc03e906821_Out_2, _Blend_927874c56b8f15888bfe6eb2b1d8c1f3_Out_2, 1);
    float4 _Property_337d8379e365b1859545f24d89eb283d_Out_0 = _FrenelColor;
    float4 _Blend_b52462f55ca3fa8e8123560ad96f591f_Out_2;
    Unity_Blend_Multiply_float4((_Blend_927874c56b8f15888bfe6eb2b1d8c1f3_Out_2.xxxx), _Property_337d8379e365b1859545f24d89eb283d_Out_0, _Blend_b52462f55ca3fa8e8123560ad96f591f_Out_2, 1);
    float4 _Multiply_8d1b6059d5faec85ae378ee724261100_Out_2;
    Unity_Multiply_float(_Blend_b52462f55ca3fa8e8123560ad96f591f_Out_2, float4(1, 1, 1, 1), _Multiply_8d1b6059d5faec85ae378ee724261100_Out_2);
    float4 _Blend_71fa99a490669a8789492c7ff5006154_Out_2;
    Unity_Blend_Lighten_float4(_Property_976c2b04f54b068091004d51cb39a1bb_Out_0, _Multiply_8d1b6059d5faec85ae378ee724261100_Out_2, _Blend_71fa99a490669a8789492c7ff5006154_Out_2, 1);
    float4 _ScreenPosition_724ad830a44b8582920d49bb9a3a8049_Out_0 = IN.ScreenPosition;
    float _Split_74ec02de1ed52489b7f5d8edf6bec915_R_1 = _ScreenPosition_724ad830a44b8582920d49bb9a3a8049_Out_0[0];
    float _Split_74ec02de1ed52489b7f5d8edf6bec915_G_2 = _ScreenPosition_724ad830a44b8582920d49bb9a3a8049_Out_0[1];
    float _Split_74ec02de1ed52489b7f5d8edf6bec915_B_3 = _ScreenPosition_724ad830a44b8582920d49bb9a3a8049_Out_0[2];
    float _Split_74ec02de1ed52489b7f5d8edf6bec915_A_4 = _ScreenPosition_724ad830a44b8582920d49bb9a3a8049_Out_0[3];
    float _Property_d93f8f362dde2481a1ff9b6a41471f97_Out_0 = _Offset;
    float _Subtract_63e4def5bfbcb0819237ae1e75f6f48f_Out_2;
    Unity_Subtract_float(_Split_74ec02de1ed52489b7f5d8edf6bec915_A_4, _Property_d93f8f362dde2481a1ff9b6a41471f97_Out_0, _Subtract_63e4def5bfbcb0819237ae1e75f6f48f_Out_2);
    float _Property_bf58377199188e8494396b8339c183d9_Out_0 = _Range;
    float _Multiply_52dbf97db03cea8683a73148cc653b01_Out_2;
    Unity_Multiply_float(_Subtract_63e4def5bfbcb0819237ae1e75f6f48f_Out_2, _Property_bf58377199188e8494396b8339c183d9_Out_0, _Multiply_52dbf97db03cea8683a73148cc653b01_Out_2);
    float _Clamp_64ffae1e14feae89b94596b9cba21a55_Out_3;
    Unity_Clamp_float(_Multiply_52dbf97db03cea8683a73148cc653b01_Out_2, 0, 1, _Clamp_64ffae1e14feae89b94596b9cba21a55_Out_3);
    float4 _Blend_2a8b33610ca45d8389ae5bca9784d7de_Out_2;
    Unity_Blend_Overwrite_float4(Color_1d0b78cf07232483869322c6e5134511, _Blend_71fa99a490669a8789492c7ff5006154_Out_2, _Blend_2a8b33610ca45d8389ae5bca9784d7de_Out_2, _Clamp_64ffae1e14feae89b94596b9cba21a55_Out_3);
    float4 _Branch_4e6bcacf8fcb5585abae3c176af03007_Out_3;
    Unity_Branch_float4(_Property_0d8e02a76989bb80ae497d44ac78381b_Out_0, _Remap_6823e14564e9eb8a8ef71bcec591d781_Out_3, _Blend_2a8b33610ca45d8389ae5bca9784d7de_Out_2, _Branch_4e6bcacf8fcb5585abae3c176af03007_Out_3);
    float _Property_e46677db9206b885982d9d37f05e425d_Out_0 = _Metalic;
    float _Property_d516b44f636fab859fe69246d5bf30e4_Out_0 = _Smoothness;
    UnityTexture2D _Property_b94b0b6634cc78818b2b2c8112d1f747_Out_0 = UnityBuildTexture2DStructNoScale(_Occ);
    float2 _TilingAndOffset_a211f9e6b171ae848225b4af08b251ef_Out_3;
    Unity_TilingAndOffset_float(IN.uv0.xy, float2 (1, 1), float2 (0, 0), _TilingAndOffset_a211f9e6b171ae848225b4af08b251ef_Out_3);
    float4 _SampleTexture2D_ab66c12e83ffe287a0e73a8ee8d8ab04_RGBA_0 = SAMPLE_TEXTURE2D(_Property_b94b0b6634cc78818b2b2c8112d1f747_Out_0.tex, UnityBuildSamplerStateStruct(SamplerState_Trilinear_Clamp).samplerstate, _TilingAndOffset_a211f9e6b171ae848225b4af08b251ef_Out_3);
    float _SampleTexture2D_ab66c12e83ffe287a0e73a8ee8d8ab04_R_4 = _SampleTexture2D_ab66c12e83ffe287a0e73a8ee8d8ab04_RGBA_0.r;
    float _SampleTexture2D_ab66c12e83ffe287a0e73a8ee8d8ab04_G_5 = _SampleTexture2D_ab66c12e83ffe287a0e73a8ee8d8ab04_RGBA_0.g;
    float _SampleTexture2D_ab66c12e83ffe287a0e73a8ee8d8ab04_B_6 = _SampleTexture2D_ab66c12e83ffe287a0e73a8ee8d8ab04_RGBA_0.b;
    float _SampleTexture2D_ab66c12e83ffe287a0e73a8ee8d8ab04_A_7 = _SampleTexture2D_ab66c12e83ffe287a0e73a8ee8d8ab04_RGBA_0.a;
    float _FresnelEffect_6dabda9e9184008dbdfce2ac745c66c0_Out_3;
    Unity_FresnelEffect_float(IN.WorldSpaceNormal, IN.WorldSpaceViewDirection, 10, _FresnelEffect_6dabda9e9184008dbdfce2ac745c66c0_Out_3);
    float _Remap_244e9fe231479c899040aad8bb5aa37c_Out_3;
    Unity_Remap_float(_FresnelEffect_6dabda9e9184008dbdfce2ac745c66c0_Out_3, float2 (0, 0.5), float2 (0.05, 1), _Remap_244e9fe231479c899040aad8bb5aa37c_Out_3);
    float _Blend_ec4b7034bfbcc28e9b21ae7806e93882_Out_2;
    Unity_Blend_Overwrite_float(_Remap_244e9fe231479c899040aad8bb5aa37c_Out_3, 1, _Blend_ec4b7034bfbcc28e9b21ae7806e93882_Out_2, _Clamp_64ffae1e14feae89b94596b9cba21a55_Out_3);
    float Slider_9331cbdecf72558e8c14e4d3f9a7360e = 0;
    surface.BaseColor = (_Branch_0d2a1beb339f47d7a3370ab32efd8e0c_Out_3.xyz);
    surface.NormalTS = IN.TangentSpaceNormal;
    surface.Emission = (_Branch_4e6bcacf8fcb5585abae3c176af03007_Out_3.xyz);
    surface.Metallic = _Property_e46677db9206b885982d9d37f05e425d_Out_0;
    surface.Smoothness = _Property_d516b44f636fab859fe69246d5bf30e4_Out_0;
    surface.Occlusion = (_SampleTexture2D_ab66c12e83ffe287a0e73a8ee8d8ab04_RGBA_0).x;
    surface.Alpha = _Blend_ec4b7034bfbcc28e9b21ae7806e93882_Out_2;
    surface.AlphaClipThreshold = Slider_9331cbdecf72558e8c14e4d3f9a7360e;
    return surface;
}

// --------------------------------------------------
// Build Graph Inputs

VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
{
    VertexDescriptionInputs output;
    ZERO_INITIALIZE(VertexDescriptionInputs, output);

    output.ObjectSpaceNormal = input.normalOS;
    output.ObjectSpaceTangent = input.tangentOS.xyz;
    output.ObjectSpacePosition = input.positionOS;

    return output;
}
    SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
{
    SurfaceDescriptionInputs output;
    ZERO_INITIALIZE(SurfaceDescriptionInputs, output);

    // must use interpolated tangent, bitangent and normal before they are normalized in the pixel shader.
    float3 unnormalizedNormalWS = input.normalWS;
    const float renormFactor = 1.0 / length(unnormalizedNormalWS);


    output.WorldSpaceNormal = renormFactor * input.normalWS.xyz;		// we want a unit length Normal Vector node in shader graph
    output.TangentSpaceNormal = float3(0.0f, 0.0f, 1.0f);


    output.WorldSpaceViewDirection = input.viewDirectionWS; //TODO: by default normalized in HD, but not in universal
    output.WorldSpacePosition = input.positionWS;
    output.ScreenPosition = ComputeScreenPos(TransformWorldToHClip(input.positionWS), _ProjectionParams.x);
    output.uv0 = input.texCoord0;
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

    #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"
#include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
#include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/PBRForwardPass.hlsl"

    ENDHLSL
}
Pass
{
    Name "ShadowCaster"
    Tags
    {
        "LightMode" = "ShadowCaster"
    }

        // Render State
        Cull Back
    Blend SrcAlpha OneMinusSrcAlpha, One OneMinusSrcAlpha
    ZTest LEqual
    ZWrite On
    ColorMask 0

        // Debug
        // <None>

        // --------------------------------------------------
        // Pass

        HLSLPROGRAM

        // Pragmas
        #pragma target 2.0
    #pragma only_renderers gles gles3 glcore d3d11
    #pragma multi_compile_instancing
    #pragma vertex vert
    #pragma fragment frag

        // DotsInstancingOptions: <None>
        // HybridV1InjectedBuiltinProperties: <None>

        // Keywords
        // PassKeywords: <None>
        // GraphKeywords: <None>

        // Defines
        #define _SURFACE_TYPE_TRANSPARENT 1
        #define _AlphaClip 1
        #define _NORMALMAP 1
        #define _NORMAL_DROPOFF_TS 1
        #define ATTRIBUTES_NEED_NORMAL
        #define ATTRIBUTES_NEED_TANGENT
        #define VARYINGS_NEED_POSITION_WS
        #define VARYINGS_NEED_NORMAL_WS
        #define VARYINGS_NEED_VIEWDIRECTION_WS
        #define FEATURES_GRAPH_VERTEX
        /* WARNING: $splice Could not find named fragment 'PassInstancing' */
        #define SHADERPASS SHADERPASS_SHADOWCASTER
        /* WARNING: $splice Could not find named fragment 'DotsInstancingVars' */

        // Includes
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
    #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
    #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
    #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
    #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
    #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"

        // --------------------------------------------------
        // Structs and Packing

        struct Attributes
    {
        float3 positionOS : POSITION;
        float3 normalOS : NORMAL;
        float4 tangentOS : TANGENT;
        #if UNITY_ANY_INSTANCING_ENABLED
        uint instanceID : INSTANCEID_SEMANTIC;
        #endif
    };
    struct Varyings
    {
        float4 positionCS : SV_POSITION;
        float3 positionWS;
        float3 normalWS;
        float3 viewDirectionWS;
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
        float3 WorldSpaceNormal;
        float3 WorldSpaceViewDirection;
        float3 WorldSpacePosition;
        float4 ScreenPosition;
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
        float3 interp0 : TEXCOORD0;
        float3 interp1 : TEXCOORD1;
        float3 interp2 : TEXCOORD2;
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

        PackedVaryings PackVaryings(Varyings input)
    {
        PackedVaryings output;
        output.positionCS = input.positionCS;
        output.interp0.xyz = input.positionWS;
        output.interp1.xyz = input.normalWS;
        output.interp2.xyz = input.viewDirectionWS;
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
    Varyings UnpackVaryings(PackedVaryings input)
    {
        Varyings output;
        output.positionCS = input.positionCS;
        output.positionWS = input.interp0.xyz;
        output.normalWS = input.interp1.xyz;
        output.viewDirectionWS = input.interp2.xyz;
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
float4 mainTexture_TexelSize;
float4 _MainColor;
float4 _EmissionColor;
float4 _FrenelColor;
float _Frenel;
float _Metalic;
float _Smoothness;
float4 _Occ_TexelSize;
float _Offset;
float _Range;
float Neon;
float Neon_1;
CBUFFER_END

// Object and Global properties
SAMPLER(SamplerState_Linear_Repeat);
SAMPLER(SamplerState_Trilinear_Clamp);
TEXTURE2D(mainTexture);
SAMPLER(samplermainTexture);
TEXTURE2D(_Occ);
SAMPLER(sampler_Occ);

// Graph Functions

void Unity_FresnelEffect_float(float3 Normal, float3 ViewDir, float Power, out float Out)
{
    Out = pow((1.0 - saturate(dot(normalize(Normal), normalize(ViewDir)))), Power);
}

void Unity_Remap_float(float In, float2 InMinMax, float2 OutMinMax, out float Out)
{
    Out = OutMinMax.x + (In - InMinMax.x) * (OutMinMax.y - OutMinMax.x) / (InMinMax.y - InMinMax.x);
}

void Unity_Subtract_float(float A, float B, out float Out)
{
    Out = A - B;
}

void Unity_Multiply_float(float A, float B, out float Out)
{
    Out = A * B;
}

void Unity_Clamp_float(float In, float Min, float Max, out float Out)
{
    Out = clamp(In, Min, Max);
}

void Unity_Blend_Overwrite_float(float Base, float Blend, out float Out, float Opacity)
{
    Out = lerp(Base, Blend, Opacity);
}

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

// Graph Pixel
struct SurfaceDescription
{
    float Alpha;
    float AlphaClipThreshold;
};

SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
{
    SurfaceDescription surface = (SurfaceDescription)0;
    float _FresnelEffect_6dabda9e9184008dbdfce2ac745c66c0_Out_3;
    Unity_FresnelEffect_float(IN.WorldSpaceNormal, IN.WorldSpaceViewDirection, 10, _FresnelEffect_6dabda9e9184008dbdfce2ac745c66c0_Out_3);
    float _Remap_244e9fe231479c899040aad8bb5aa37c_Out_3;
    Unity_Remap_float(_FresnelEffect_6dabda9e9184008dbdfce2ac745c66c0_Out_3, float2 (0, 0.5), float2 (0.05, 1), _Remap_244e9fe231479c899040aad8bb5aa37c_Out_3);
    float4 _ScreenPosition_724ad830a44b8582920d49bb9a3a8049_Out_0 = IN.ScreenPosition;
    float _Split_74ec02de1ed52489b7f5d8edf6bec915_R_1 = _ScreenPosition_724ad830a44b8582920d49bb9a3a8049_Out_0[0];
    float _Split_74ec02de1ed52489b7f5d8edf6bec915_G_2 = _ScreenPosition_724ad830a44b8582920d49bb9a3a8049_Out_0[1];
    float _Split_74ec02de1ed52489b7f5d8edf6bec915_B_3 = _ScreenPosition_724ad830a44b8582920d49bb9a3a8049_Out_0[2];
    float _Split_74ec02de1ed52489b7f5d8edf6bec915_A_4 = _ScreenPosition_724ad830a44b8582920d49bb9a3a8049_Out_0[3];
    float _Property_d93f8f362dde2481a1ff9b6a41471f97_Out_0 = _Offset;
    float _Subtract_63e4def5bfbcb0819237ae1e75f6f48f_Out_2;
    Unity_Subtract_float(_Split_74ec02de1ed52489b7f5d8edf6bec915_A_4, _Property_d93f8f362dde2481a1ff9b6a41471f97_Out_0, _Subtract_63e4def5bfbcb0819237ae1e75f6f48f_Out_2);
    float _Property_bf58377199188e8494396b8339c183d9_Out_0 = _Range;
    float _Multiply_52dbf97db03cea8683a73148cc653b01_Out_2;
    Unity_Multiply_float(_Subtract_63e4def5bfbcb0819237ae1e75f6f48f_Out_2, _Property_bf58377199188e8494396b8339c183d9_Out_0, _Multiply_52dbf97db03cea8683a73148cc653b01_Out_2);
    float _Clamp_64ffae1e14feae89b94596b9cba21a55_Out_3;
    Unity_Clamp_float(_Multiply_52dbf97db03cea8683a73148cc653b01_Out_2, 0, 1, _Clamp_64ffae1e14feae89b94596b9cba21a55_Out_3);
    float _Blend_ec4b7034bfbcc28e9b21ae7806e93882_Out_2;
    Unity_Blend_Overwrite_float(_Remap_244e9fe231479c899040aad8bb5aa37c_Out_3, 1, _Blend_ec4b7034bfbcc28e9b21ae7806e93882_Out_2, _Clamp_64ffae1e14feae89b94596b9cba21a55_Out_3);
    float Slider_9331cbdecf72558e8c14e4d3f9a7360e = 0;
    surface.Alpha = _Blend_ec4b7034bfbcc28e9b21ae7806e93882_Out_2;
    surface.AlphaClipThreshold = Slider_9331cbdecf72558e8c14e4d3f9a7360e;
    return surface;
}

// --------------------------------------------------
// Build Graph Inputs

VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
{
    VertexDescriptionInputs output;
    ZERO_INITIALIZE(VertexDescriptionInputs, output);

    output.ObjectSpaceNormal = input.normalOS;
    output.ObjectSpaceTangent = input.tangentOS.xyz;
    output.ObjectSpacePosition = input.positionOS;

    return output;
}
    SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
{
    SurfaceDescriptionInputs output;
    ZERO_INITIALIZE(SurfaceDescriptionInputs, output);

    // must use interpolated tangent, bitangent and normal before they are normalized in the pixel shader.
    float3 unnormalizedNormalWS = input.normalWS;
    const float renormFactor = 1.0 / length(unnormalizedNormalWS);


    output.WorldSpaceNormal = renormFactor * input.normalWS.xyz;		// we want a unit length Normal Vector node in shader graph


    output.WorldSpaceViewDirection = input.viewDirectionWS; //TODO: by default normalized in HD, but not in universal
    output.WorldSpacePosition = input.positionWS;
    output.ScreenPosition = ComputeScreenPos(TransformWorldToHClip(input.positionWS), _ProjectionParams.x);
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

    #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"
#include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
#include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShadowCasterPass.hlsl"

    ENDHLSL
}
Pass
{
    Name "DepthOnly"
    Tags
    {
        "LightMode" = "DepthOnly"
    }

        // Render State
        Cull Back
    Blend SrcAlpha OneMinusSrcAlpha, One OneMinusSrcAlpha
    ZTest LEqual
    ZWrite On
    ColorMask 0

        // Debug
        // <None>

        // --------------------------------------------------
        // Pass

        HLSLPROGRAM

        // Pragmas
        #pragma target 2.0
    #pragma only_renderers gles gles3 glcore d3d11
    #pragma multi_compile_instancing
    #pragma vertex vert
    #pragma fragment frag

        // DotsInstancingOptions: <None>
        // HybridV1InjectedBuiltinProperties: <None>

        // Keywords
        // PassKeywords: <None>
        // GraphKeywords: <None>

        // Defines
        #define _SURFACE_TYPE_TRANSPARENT 1
        #define _AlphaClip 1
        #define _NORMALMAP 1
        #define _NORMAL_DROPOFF_TS 1
        #define ATTRIBUTES_NEED_NORMAL
        #define ATTRIBUTES_NEED_TANGENT
        #define VARYINGS_NEED_POSITION_WS
        #define VARYINGS_NEED_NORMAL_WS
        #define VARYINGS_NEED_VIEWDIRECTION_WS
        #define FEATURES_GRAPH_VERTEX
        /* WARNING: $splice Could not find named fragment 'PassInstancing' */
        #define SHADERPASS SHADERPASS_DEPTHONLY
        /* WARNING: $splice Could not find named fragment 'DotsInstancingVars' */

        // Includes
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
    #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
    #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
    #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
    #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
    #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"

        // --------------------------------------------------
        // Structs and Packing

        struct Attributes
    {
        float3 positionOS : POSITION;
        float3 normalOS : NORMAL;
        float4 tangentOS : TANGENT;
        #if UNITY_ANY_INSTANCING_ENABLED
        uint instanceID : INSTANCEID_SEMANTIC;
        #endif
    };
    struct Varyings
    {
        float4 positionCS : SV_POSITION;
        float3 positionWS;
        float3 normalWS;
        float3 viewDirectionWS;
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
        float3 WorldSpaceNormal;
        float3 WorldSpaceViewDirection;
        float3 WorldSpacePosition;
        float4 ScreenPosition;
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
        float3 interp0 : TEXCOORD0;
        float3 interp1 : TEXCOORD1;
        float3 interp2 : TEXCOORD2;
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

        PackedVaryings PackVaryings(Varyings input)
    {
        PackedVaryings output;
        output.positionCS = input.positionCS;
        output.interp0.xyz = input.positionWS;
        output.interp1.xyz = input.normalWS;
        output.interp2.xyz = input.viewDirectionWS;
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
    Varyings UnpackVaryings(PackedVaryings input)
    {
        Varyings output;
        output.positionCS = input.positionCS;
        output.positionWS = input.interp0.xyz;
        output.normalWS = input.interp1.xyz;
        output.viewDirectionWS = input.interp2.xyz;
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
float4 mainTexture_TexelSize;
float4 _MainColor;
float4 _EmissionColor;
float4 _FrenelColor;
float _Frenel;
float _Metalic;
float _Smoothness;
float4 _Occ_TexelSize;
float _Offset;
float _Range;
float Neon;
float Neon_1;
CBUFFER_END

// Object and Global properties
SAMPLER(SamplerState_Linear_Repeat);
SAMPLER(SamplerState_Trilinear_Clamp);
TEXTURE2D(mainTexture);
SAMPLER(samplermainTexture);
TEXTURE2D(_Occ);
SAMPLER(sampler_Occ);

// Graph Functions

void Unity_FresnelEffect_float(float3 Normal, float3 ViewDir, float Power, out float Out)
{
    Out = pow((1.0 - saturate(dot(normalize(Normal), normalize(ViewDir)))), Power);
}

void Unity_Remap_float(float In, float2 InMinMax, float2 OutMinMax, out float Out)
{
    Out = OutMinMax.x + (In - InMinMax.x) * (OutMinMax.y - OutMinMax.x) / (InMinMax.y - InMinMax.x);
}

void Unity_Subtract_float(float A, float B, out float Out)
{
    Out = A - B;
}

void Unity_Multiply_float(float A, float B, out float Out)
{
    Out = A * B;
}

void Unity_Clamp_float(float In, float Min, float Max, out float Out)
{
    Out = clamp(In, Min, Max);
}

void Unity_Blend_Overwrite_float(float Base, float Blend, out float Out, float Opacity)
{
    Out = lerp(Base, Blend, Opacity);
}

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

// Graph Pixel
struct SurfaceDescription
{
    float Alpha;
    float AlphaClipThreshold;
};

SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
{
    SurfaceDescription surface = (SurfaceDescription)0;
    float _FresnelEffect_6dabda9e9184008dbdfce2ac745c66c0_Out_3;
    Unity_FresnelEffect_float(IN.WorldSpaceNormal, IN.WorldSpaceViewDirection, 10, _FresnelEffect_6dabda9e9184008dbdfce2ac745c66c0_Out_3);
    float _Remap_244e9fe231479c899040aad8bb5aa37c_Out_3;
    Unity_Remap_float(_FresnelEffect_6dabda9e9184008dbdfce2ac745c66c0_Out_3, float2 (0, 0.5), float2 (0.05, 1), _Remap_244e9fe231479c899040aad8bb5aa37c_Out_3);
    float4 _ScreenPosition_724ad830a44b8582920d49bb9a3a8049_Out_0 = IN.ScreenPosition;
    float _Split_74ec02de1ed52489b7f5d8edf6bec915_R_1 = _ScreenPosition_724ad830a44b8582920d49bb9a3a8049_Out_0[0];
    float _Split_74ec02de1ed52489b7f5d8edf6bec915_G_2 = _ScreenPosition_724ad830a44b8582920d49bb9a3a8049_Out_0[1];
    float _Split_74ec02de1ed52489b7f5d8edf6bec915_B_3 = _ScreenPosition_724ad830a44b8582920d49bb9a3a8049_Out_0[2];
    float _Split_74ec02de1ed52489b7f5d8edf6bec915_A_4 = _ScreenPosition_724ad830a44b8582920d49bb9a3a8049_Out_0[3];
    float _Property_d93f8f362dde2481a1ff9b6a41471f97_Out_0 = _Offset;
    float _Subtract_63e4def5bfbcb0819237ae1e75f6f48f_Out_2;
    Unity_Subtract_float(_Split_74ec02de1ed52489b7f5d8edf6bec915_A_4, _Property_d93f8f362dde2481a1ff9b6a41471f97_Out_0, _Subtract_63e4def5bfbcb0819237ae1e75f6f48f_Out_2);
    float _Property_bf58377199188e8494396b8339c183d9_Out_0 = _Range;
    float _Multiply_52dbf97db03cea8683a73148cc653b01_Out_2;
    Unity_Multiply_float(_Subtract_63e4def5bfbcb0819237ae1e75f6f48f_Out_2, _Property_bf58377199188e8494396b8339c183d9_Out_0, _Multiply_52dbf97db03cea8683a73148cc653b01_Out_2);
    float _Clamp_64ffae1e14feae89b94596b9cba21a55_Out_3;
    Unity_Clamp_float(_Multiply_52dbf97db03cea8683a73148cc653b01_Out_2, 0, 1, _Clamp_64ffae1e14feae89b94596b9cba21a55_Out_3);
    float _Blend_ec4b7034bfbcc28e9b21ae7806e93882_Out_2;
    Unity_Blend_Overwrite_float(_Remap_244e9fe231479c899040aad8bb5aa37c_Out_3, 1, _Blend_ec4b7034bfbcc28e9b21ae7806e93882_Out_2, _Clamp_64ffae1e14feae89b94596b9cba21a55_Out_3);
    float Slider_9331cbdecf72558e8c14e4d3f9a7360e = 0;
    surface.Alpha = _Blend_ec4b7034bfbcc28e9b21ae7806e93882_Out_2;
    surface.AlphaClipThreshold = Slider_9331cbdecf72558e8c14e4d3f9a7360e;
    return surface;
}

// --------------------------------------------------
// Build Graph Inputs

VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
{
    VertexDescriptionInputs output;
    ZERO_INITIALIZE(VertexDescriptionInputs, output);

    output.ObjectSpaceNormal = input.normalOS;
    output.ObjectSpaceTangent = input.tangentOS.xyz;
    output.ObjectSpacePosition = input.positionOS;

    return output;
}
    SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
{
    SurfaceDescriptionInputs output;
    ZERO_INITIALIZE(SurfaceDescriptionInputs, output);

    // must use interpolated tangent, bitangent and normal before they are normalized in the pixel shader.
    float3 unnormalizedNormalWS = input.normalWS;
    const float renormFactor = 1.0 / length(unnormalizedNormalWS);


    output.WorldSpaceNormal = renormFactor * input.normalWS.xyz;		// we want a unit length Normal Vector node in shader graph


    output.WorldSpaceViewDirection = input.viewDirectionWS; //TODO: by default normalized in HD, but not in universal
    output.WorldSpacePosition = input.positionWS;
    output.ScreenPosition = ComputeScreenPos(TransformWorldToHClip(input.positionWS), _ProjectionParams.x);
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

    #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"
#include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
#include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/DepthOnlyPass.hlsl"

    ENDHLSL
}
Pass
{
    Name "DepthNormals"
    Tags
    {
        "LightMode" = "DepthNormals"
    }

        // Render State
        Cull Back
    Blend SrcAlpha OneMinusSrcAlpha, One OneMinusSrcAlpha
    ZTest LEqual
    ZWrite On

        // Debug
        // <None>

        // --------------------------------------------------
        // Pass

        HLSLPROGRAM

        // Pragmas
        #pragma target 2.0
    #pragma only_renderers gles gles3 glcore d3d11
    #pragma multi_compile_instancing
    #pragma vertex vert
    #pragma fragment frag

        // DotsInstancingOptions: <None>
        // HybridV1InjectedBuiltinProperties: <None>

        // Keywords
        // PassKeywords: <None>
        // GraphKeywords: <None>

        // Defines
        #define _SURFACE_TYPE_TRANSPARENT 1
        #define _AlphaClip 1
        #define _NORMALMAP 1
        #define _NORMAL_DROPOFF_TS 1
        #define ATTRIBUTES_NEED_NORMAL
        #define ATTRIBUTES_NEED_TANGENT
        #define ATTRIBUTES_NEED_TEXCOORD1
        #define VARYINGS_NEED_POSITION_WS
        #define VARYINGS_NEED_NORMAL_WS
        #define VARYINGS_NEED_TANGENT_WS
        #define VARYINGS_NEED_VIEWDIRECTION_WS
        #define FEATURES_GRAPH_VERTEX
        /* WARNING: $splice Could not find named fragment 'PassInstancing' */
        #define SHADERPASS SHADERPASS_DEPTHNORMALSONLY
        /* WARNING: $splice Could not find named fragment 'DotsInstancingVars' */

        // Includes
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
    #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
    #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
    #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
    #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
    #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"

        // --------------------------------------------------
        // Structs and Packing

        struct Attributes
    {
        float3 positionOS : POSITION;
        float3 normalOS : NORMAL;
        float4 tangentOS : TANGENT;
        float4 uv1 : TEXCOORD1;
        #if UNITY_ANY_INSTANCING_ENABLED
        uint instanceID : INSTANCEID_SEMANTIC;
        #endif
    };
    struct Varyings
    {
        float4 positionCS : SV_POSITION;
        float3 positionWS;
        float3 normalWS;
        float4 tangentWS;
        float3 viewDirectionWS;
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
        float3 WorldSpaceNormal;
        float3 TangentSpaceNormal;
        float3 WorldSpaceViewDirection;
        float3 WorldSpacePosition;
        float4 ScreenPosition;
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
        float3 interp0 : TEXCOORD0;
        float3 interp1 : TEXCOORD1;
        float4 interp2 : TEXCOORD2;
        float3 interp3 : TEXCOORD3;
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

        PackedVaryings PackVaryings(Varyings input)
    {
        PackedVaryings output;
        output.positionCS = input.positionCS;
        output.interp0.xyz = input.positionWS;
        output.interp1.xyz = input.normalWS;
        output.interp2.xyzw = input.tangentWS;
        output.interp3.xyz = input.viewDirectionWS;
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
    Varyings UnpackVaryings(PackedVaryings input)
    {
        Varyings output;
        output.positionCS = input.positionCS;
        output.positionWS = input.interp0.xyz;
        output.normalWS = input.interp1.xyz;
        output.tangentWS = input.interp2.xyzw;
        output.viewDirectionWS = input.interp3.xyz;
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
float4 mainTexture_TexelSize;
float4 _MainColor;
float4 _EmissionColor;
float4 _FrenelColor;
float _Frenel;
float _Metalic;
float _Smoothness;
float4 _Occ_TexelSize;
float _Offset;
float _Range;
float Neon;
float Neon_1;
CBUFFER_END

// Object and Global properties
SAMPLER(SamplerState_Linear_Repeat);
SAMPLER(SamplerState_Trilinear_Clamp);
TEXTURE2D(mainTexture);
SAMPLER(samplermainTexture);
TEXTURE2D(_Occ);
SAMPLER(sampler_Occ);

// Graph Functions

void Unity_FresnelEffect_float(float3 Normal, float3 ViewDir, float Power, out float Out)
{
    Out = pow((1.0 - saturate(dot(normalize(Normal), normalize(ViewDir)))), Power);
}

void Unity_Remap_float(float In, float2 InMinMax, float2 OutMinMax, out float Out)
{
    Out = OutMinMax.x + (In - InMinMax.x) * (OutMinMax.y - OutMinMax.x) / (InMinMax.y - InMinMax.x);
}

void Unity_Subtract_float(float A, float B, out float Out)
{
    Out = A - B;
}

void Unity_Multiply_float(float A, float B, out float Out)
{
    Out = A * B;
}

void Unity_Clamp_float(float In, float Min, float Max, out float Out)
{
    Out = clamp(In, Min, Max);
}

void Unity_Blend_Overwrite_float(float Base, float Blend, out float Out, float Opacity)
{
    Out = lerp(Base, Blend, Opacity);
}

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

// Graph Pixel
struct SurfaceDescription
{
    float3 NormalTS;
    float Alpha;
    float AlphaClipThreshold;
};

SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
{
    SurfaceDescription surface = (SurfaceDescription)0;
    float _FresnelEffect_6dabda9e9184008dbdfce2ac745c66c0_Out_3;
    Unity_FresnelEffect_float(IN.WorldSpaceNormal, IN.WorldSpaceViewDirection, 10, _FresnelEffect_6dabda9e9184008dbdfce2ac745c66c0_Out_3);
    float _Remap_244e9fe231479c899040aad8bb5aa37c_Out_3;
    Unity_Remap_float(_FresnelEffect_6dabda9e9184008dbdfce2ac745c66c0_Out_3, float2 (0, 0.5), float2 (0.05, 1), _Remap_244e9fe231479c899040aad8bb5aa37c_Out_3);
    float4 _ScreenPosition_724ad830a44b8582920d49bb9a3a8049_Out_0 = IN.ScreenPosition;
    float _Split_74ec02de1ed52489b7f5d8edf6bec915_R_1 = _ScreenPosition_724ad830a44b8582920d49bb9a3a8049_Out_0[0];
    float _Split_74ec02de1ed52489b7f5d8edf6bec915_G_2 = _ScreenPosition_724ad830a44b8582920d49bb9a3a8049_Out_0[1];
    float _Split_74ec02de1ed52489b7f5d8edf6bec915_B_3 = _ScreenPosition_724ad830a44b8582920d49bb9a3a8049_Out_0[2];
    float _Split_74ec02de1ed52489b7f5d8edf6bec915_A_4 = _ScreenPosition_724ad830a44b8582920d49bb9a3a8049_Out_0[3];
    float _Property_d93f8f362dde2481a1ff9b6a41471f97_Out_0 = _Offset;
    float _Subtract_63e4def5bfbcb0819237ae1e75f6f48f_Out_2;
    Unity_Subtract_float(_Split_74ec02de1ed52489b7f5d8edf6bec915_A_4, _Property_d93f8f362dde2481a1ff9b6a41471f97_Out_0, _Subtract_63e4def5bfbcb0819237ae1e75f6f48f_Out_2);
    float _Property_bf58377199188e8494396b8339c183d9_Out_0 = _Range;
    float _Multiply_52dbf97db03cea8683a73148cc653b01_Out_2;
    Unity_Multiply_float(_Subtract_63e4def5bfbcb0819237ae1e75f6f48f_Out_2, _Property_bf58377199188e8494396b8339c183d9_Out_0, _Multiply_52dbf97db03cea8683a73148cc653b01_Out_2);
    float _Clamp_64ffae1e14feae89b94596b9cba21a55_Out_3;
    Unity_Clamp_float(_Multiply_52dbf97db03cea8683a73148cc653b01_Out_2, 0, 1, _Clamp_64ffae1e14feae89b94596b9cba21a55_Out_3);
    float _Blend_ec4b7034bfbcc28e9b21ae7806e93882_Out_2;
    Unity_Blend_Overwrite_float(_Remap_244e9fe231479c899040aad8bb5aa37c_Out_3, 1, _Blend_ec4b7034bfbcc28e9b21ae7806e93882_Out_2, _Clamp_64ffae1e14feae89b94596b9cba21a55_Out_3);
    float Slider_9331cbdecf72558e8c14e4d3f9a7360e = 0;
    surface.NormalTS = IN.TangentSpaceNormal;
    surface.Alpha = _Blend_ec4b7034bfbcc28e9b21ae7806e93882_Out_2;
    surface.AlphaClipThreshold = Slider_9331cbdecf72558e8c14e4d3f9a7360e;
    return surface;
}

// --------------------------------------------------
// Build Graph Inputs

VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
{
    VertexDescriptionInputs output;
    ZERO_INITIALIZE(VertexDescriptionInputs, output);

    output.ObjectSpaceNormal = input.normalOS;
    output.ObjectSpaceTangent = input.tangentOS.xyz;
    output.ObjectSpacePosition = input.positionOS;

    return output;
}
    SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
{
    SurfaceDescriptionInputs output;
    ZERO_INITIALIZE(SurfaceDescriptionInputs, output);

    // must use interpolated tangent, bitangent and normal before they are normalized in the pixel shader.
    float3 unnormalizedNormalWS = input.normalWS;
    const float renormFactor = 1.0 / length(unnormalizedNormalWS);


    output.WorldSpaceNormal = renormFactor * input.normalWS.xyz;		// we want a unit length Normal Vector node in shader graph
    output.TangentSpaceNormal = float3(0.0f, 0.0f, 1.0f);


    output.WorldSpaceViewDirection = input.viewDirectionWS; //TODO: by default normalized in HD, but not in universal
    output.WorldSpacePosition = input.positionWS;
    output.ScreenPosition = ComputeScreenPos(TransformWorldToHClip(input.positionWS), _ProjectionParams.x);
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

    #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"
#include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
#include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/DepthNormalsOnlyPass.hlsl"

    ENDHLSL
}
Pass
{
    Name "Meta"
    Tags
    {
        "LightMode" = "Meta"
    }

        // Render State
        Cull Off

        // Debug
        // <None>

        // --------------------------------------------------
        // Pass

        HLSLPROGRAM

        // Pragmas
        #pragma target 2.0
    #pragma only_renderers gles gles3 glcore d3d11
    #pragma vertex vert
    #pragma fragment frag

        // DotsInstancingOptions: <None>
        // HybridV1InjectedBuiltinProperties: <None>

        // Keywords
        #pragma shader_feature _ _SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A
        // GraphKeywords: <None>

        // Defines
        #define _SURFACE_TYPE_TRANSPARENT 1
        #define _AlphaClip 1
        #define _NORMALMAP 1
        #define _NORMAL_DROPOFF_TS 1
        #define ATTRIBUTES_NEED_NORMAL
        #define ATTRIBUTES_NEED_TANGENT
        #define ATTRIBUTES_NEED_TEXCOORD0
        #define ATTRIBUTES_NEED_TEXCOORD1
        #define ATTRIBUTES_NEED_TEXCOORD2
        #define VARYINGS_NEED_POSITION_WS
        #define VARYINGS_NEED_NORMAL_WS
        #define VARYINGS_NEED_TEXCOORD0
        #define VARYINGS_NEED_VIEWDIRECTION_WS
        #define FEATURES_GRAPH_VERTEX
        /* WARNING: $splice Could not find named fragment 'PassInstancing' */
        #define SHADERPASS SHADERPASS_META
        /* WARNING: $splice Could not find named fragment 'DotsInstancingVars' */

        // Includes
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
    #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
    #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
    #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
    #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
    #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
    #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/MetaInput.hlsl"

        // --------------------------------------------------
        // Structs and Packing

        struct Attributes
    {
        float3 positionOS : POSITION;
        float3 normalOS : NORMAL;
        float4 tangentOS : TANGENT;
        float4 uv0 : TEXCOORD0;
        float4 uv1 : TEXCOORD1;
        float4 uv2 : TEXCOORD2;
        #if UNITY_ANY_INSTANCING_ENABLED
        uint instanceID : INSTANCEID_SEMANTIC;
        #endif
    };
    struct Varyings
    {
        float4 positionCS : SV_POSITION;
        float3 positionWS;
        float3 normalWS;
        float4 texCoord0;
        float3 viewDirectionWS;
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
        float3 WorldSpaceNormal;
        float3 WorldSpaceViewDirection;
        float3 WorldSpacePosition;
        float4 ScreenPosition;
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
        float3 interp0 : TEXCOORD0;
        float3 interp1 : TEXCOORD1;
        float4 interp2 : TEXCOORD2;
        float3 interp3 : TEXCOORD3;
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

        PackedVaryings PackVaryings(Varyings input)
    {
        PackedVaryings output;
        output.positionCS = input.positionCS;
        output.interp0.xyz = input.positionWS;
        output.interp1.xyz = input.normalWS;
        output.interp2.xyzw = input.texCoord0;
        output.interp3.xyz = input.viewDirectionWS;
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
    Varyings UnpackVaryings(PackedVaryings input)
    {
        Varyings output;
        output.positionCS = input.positionCS;
        output.positionWS = input.interp0.xyz;
        output.normalWS = input.interp1.xyz;
        output.texCoord0 = input.interp2.xyzw;
        output.viewDirectionWS = input.interp3.xyz;
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
float4 mainTexture_TexelSize;
float4 _MainColor;
float4 _EmissionColor;
float4 _FrenelColor;
float _Frenel;
float _Metalic;
float _Smoothness;
float4 _Occ_TexelSize;
float _Offset;
float _Range;
float Neon;
float Neon_1;
CBUFFER_END

// Object and Global properties
SAMPLER(SamplerState_Linear_Repeat);
SAMPLER(SamplerState_Trilinear_Clamp);
TEXTURE2D(mainTexture);
SAMPLER(samplermainTexture);
TEXTURE2D(_Occ);
SAMPLER(sampler_Occ);

// Graph Functions

void Unity_Blend_Overlay_float4(float4 Base, float4 Blend, out float4 Out, float Opacity)
{
    float4 result1 = 1.0 - 2.0 * (1.0 - Base) * (1.0 - Blend);
    float4 result2 = 2.0 * Base * Blend;
    float4 zeroOrOne = step(Base, 0.5);
    Out = result2 * zeroOrOne + (1 - zeroOrOne) * result1;
    Out = lerp(Base, Out, Opacity);
}

void Unity_Branch_float4(float Predicate, float4 True, float4 False, out float4 Out)
{
    Out = Predicate ? True : False;
}

void Unity_Remap_float4(float4 In, float2 InMinMax, float2 OutMinMax, out float4 Out)
{
    Out = OutMinMax.x + (In - InMinMax.x) * (OutMinMax.y - OutMinMax.x) / (InMinMax.y - InMinMax.x);
}

void Unity_OneMinus_float(float In, out float Out)
{
    Out = 1 - In;
}

void Unity_FresnelEffect_float(float3 Normal, float3 ViewDir, float Power, out float Out)
{
    Out = pow((1.0 - saturate(dot(normalize(Normal), normalize(ViewDir)))), Power);
}

void Unity_Power_float(float A, float B, out float Out)
{
    Out = pow(A, B);
}

void Unity_Blend_Overwrite_float(float Base, float Blend, out float Out, float Opacity)
{
    Out = lerp(Base, Blend, Opacity);
}

void Unity_Blend_Multiply_float4(float4 Base, float4 Blend, out float4 Out, float Opacity)
{
    Out = Base * Blend;
    Out = lerp(Base, Out, Opacity);
}

void Unity_Multiply_float(float4 A, float4 B, out float4 Out)
{
    Out = A * B;
}

void Unity_Blend_Lighten_float4(float4 Base, float4 Blend, out float4 Out, float Opacity)
{
    Out = max(Blend, Base);
    Out = lerp(Base, Out, Opacity);
}

void Unity_Subtract_float(float A, float B, out float Out)
{
    Out = A - B;
}

void Unity_Multiply_float(float A, float B, out float Out)
{
    Out = A * B;
}

void Unity_Clamp_float(float In, float Min, float Max, out float Out)
{
    Out = clamp(In, Min, Max);
}

void Unity_Blend_Overwrite_float4(float4 Base, float4 Blend, out float4 Out, float Opacity)
{
    Out = lerp(Base, Blend, Opacity);
}

void Unity_Remap_float(float In, float2 InMinMax, float2 OutMinMax, out float Out)
{
    Out = OutMinMax.x + (In - InMinMax.x) * (OutMinMax.y - OutMinMax.x) / (InMinMax.y - InMinMax.x);
}

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

// Graph Pixel
struct SurfaceDescription
{
    float3 BaseColor;
    float3 Emission;
    float Alpha;
    float AlphaClipThreshold;
};

SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
{
    SurfaceDescription surface = (SurfaceDescription)0;
    float _Property_db0d6ba2317f4ca885fffc369f6b7572_Out_0 = Neon_1;
    float4 _Property_404b2b1af018418b81f010f388917a24_Out_0 = _MainColor;
    UnityTexture2D _Property_9c598a886ed15f8aa999f5dbaa5a4e25_Out_0 = UnityBuildTexture2DStructNoScale(mainTexture);
    float4 _SampleTexture2D_9a0e43a036a3828e8771aff20cdda3ba_RGBA_0 = SAMPLE_TEXTURE2D(_Property_9c598a886ed15f8aa999f5dbaa5a4e25_Out_0.tex, _Property_9c598a886ed15f8aa999f5dbaa5a4e25_Out_0.samplerstate, IN.uv0.xy);
    float _SampleTexture2D_9a0e43a036a3828e8771aff20cdda3ba_R_4 = _SampleTexture2D_9a0e43a036a3828e8771aff20cdda3ba_RGBA_0.r;
    float _SampleTexture2D_9a0e43a036a3828e8771aff20cdda3ba_G_5 = _SampleTexture2D_9a0e43a036a3828e8771aff20cdda3ba_RGBA_0.g;
    float _SampleTexture2D_9a0e43a036a3828e8771aff20cdda3ba_B_6 = _SampleTexture2D_9a0e43a036a3828e8771aff20cdda3ba_RGBA_0.b;
    float _SampleTexture2D_9a0e43a036a3828e8771aff20cdda3ba_A_7 = _SampleTexture2D_9a0e43a036a3828e8771aff20cdda3ba_RGBA_0.a;
    float4 _Blend_07c1433efd838d88a6692dd2f3fa5cc1_Out_2;
    Unity_Blend_Overlay_float4(_SampleTexture2D_9a0e43a036a3828e8771aff20cdda3ba_RGBA_0, _Property_404b2b1af018418b81f010f388917a24_Out_0, _Blend_07c1433efd838d88a6692dd2f3fa5cc1_Out_2, 1);
    float4 _Branch_0d2a1beb339f47d7a3370ab32efd8e0c_Out_3;
    Unity_Branch_float4(_Property_db0d6ba2317f4ca885fffc369f6b7572_Out_0, _Property_404b2b1af018418b81f010f388917a24_Out_0, _Blend_07c1433efd838d88a6692dd2f3fa5cc1_Out_2, _Branch_0d2a1beb339f47d7a3370ab32efd8e0c_Out_3);
    float _Property_0d8e02a76989bb80ae497d44ac78381b_Out_0 = Neon;
    float4 _Remap_6823e14564e9eb8a8ef71bcec591d781_Out_3;
    Unity_Remap_float4(_SampleTexture2D_9a0e43a036a3828e8771aff20cdda3ba_RGBA_0, float2 (0, 1), float2 (0, 2), _Remap_6823e14564e9eb8a8ef71bcec591d781_Out_3);
    float4 Color_1d0b78cf07232483869322c6e5134511 = IsGammaSpace() ? float4(0.1885618, 0.6377826, 1.059274, 1) : float4(SRGBToLinear(float3(0.1885618, 0.6377826, 1.059274)), 1);
    float4 _Property_976c2b04f54b068091004d51cb39a1bb_Out_0 = _EmissionColor;
    float _Property_bc4a96833916af8fab203c56c0b44955_Out_0 = _Frenel;
    float _OneMinus_00c02da1a3e44983954a2b94c04bbe8d_Out_1;
    Unity_OneMinus_float(_Property_bc4a96833916af8fab203c56c0b44955_Out_0, _OneMinus_00c02da1a3e44983954a2b94c04bbe8d_Out_1);
    float _FresnelEffect_2e0aeb0e30852e8686f75ede044d7ce7_Out_3;
    Unity_FresnelEffect_float(IN.WorldSpaceNormal, IN.WorldSpaceViewDirection, _OneMinus_00c02da1a3e44983954a2b94c04bbe8d_Out_1, _FresnelEffect_2e0aeb0e30852e8686f75ede044d7ce7_Out_3);
    float _Power_c20579372975d58d9e956dc03e906821_Out_2;
    Unity_Power_float(_FresnelEffect_2e0aeb0e30852e8686f75ede044d7ce7_Out_3, 2, _Power_c20579372975d58d9e956dc03e906821_Out_2);
    float _Blend_927874c56b8f15888bfe6eb2b1d8c1f3_Out_2;
    Unity_Blend_Overwrite_float(0, _Power_c20579372975d58d9e956dc03e906821_Out_2, _Blend_927874c56b8f15888bfe6eb2b1d8c1f3_Out_2, 1);
    float4 _Property_337d8379e365b1859545f24d89eb283d_Out_0 = _FrenelColor;
    float4 _Blend_b52462f55ca3fa8e8123560ad96f591f_Out_2;
    Unity_Blend_Multiply_float4((_Blend_927874c56b8f15888bfe6eb2b1d8c1f3_Out_2.xxxx), _Property_337d8379e365b1859545f24d89eb283d_Out_0, _Blend_b52462f55ca3fa8e8123560ad96f591f_Out_2, 1);
    float4 _Multiply_8d1b6059d5faec85ae378ee724261100_Out_2;
    Unity_Multiply_float(_Blend_b52462f55ca3fa8e8123560ad96f591f_Out_2, float4(1, 1, 1, 1), _Multiply_8d1b6059d5faec85ae378ee724261100_Out_2);
    float4 _Blend_71fa99a490669a8789492c7ff5006154_Out_2;
    Unity_Blend_Lighten_float4(_Property_976c2b04f54b068091004d51cb39a1bb_Out_0, _Multiply_8d1b6059d5faec85ae378ee724261100_Out_2, _Blend_71fa99a490669a8789492c7ff5006154_Out_2, 1);
    float4 _ScreenPosition_724ad830a44b8582920d49bb9a3a8049_Out_0 = IN.ScreenPosition;
    float _Split_74ec02de1ed52489b7f5d8edf6bec915_R_1 = _ScreenPosition_724ad830a44b8582920d49bb9a3a8049_Out_0[0];
    float _Split_74ec02de1ed52489b7f5d8edf6bec915_G_2 = _ScreenPosition_724ad830a44b8582920d49bb9a3a8049_Out_0[1];
    float _Split_74ec02de1ed52489b7f5d8edf6bec915_B_3 = _ScreenPosition_724ad830a44b8582920d49bb9a3a8049_Out_0[2];
    float _Split_74ec02de1ed52489b7f5d8edf6bec915_A_4 = _ScreenPosition_724ad830a44b8582920d49bb9a3a8049_Out_0[3];
    float _Property_d93f8f362dde2481a1ff9b6a41471f97_Out_0 = _Offset;
    float _Subtract_63e4def5bfbcb0819237ae1e75f6f48f_Out_2;
    Unity_Subtract_float(_Split_74ec02de1ed52489b7f5d8edf6bec915_A_4, _Property_d93f8f362dde2481a1ff9b6a41471f97_Out_0, _Subtract_63e4def5bfbcb0819237ae1e75f6f48f_Out_2);
    float _Property_bf58377199188e8494396b8339c183d9_Out_0 = _Range;
    float _Multiply_52dbf97db03cea8683a73148cc653b01_Out_2;
    Unity_Multiply_float(_Subtract_63e4def5bfbcb0819237ae1e75f6f48f_Out_2, _Property_bf58377199188e8494396b8339c183d9_Out_0, _Multiply_52dbf97db03cea8683a73148cc653b01_Out_2);
    float _Clamp_64ffae1e14feae89b94596b9cba21a55_Out_3;
    Unity_Clamp_float(_Multiply_52dbf97db03cea8683a73148cc653b01_Out_2, 0, 1, _Clamp_64ffae1e14feae89b94596b9cba21a55_Out_3);
    float4 _Blend_2a8b33610ca45d8389ae5bca9784d7de_Out_2;
    Unity_Blend_Overwrite_float4(Color_1d0b78cf07232483869322c6e5134511, _Blend_71fa99a490669a8789492c7ff5006154_Out_2, _Blend_2a8b33610ca45d8389ae5bca9784d7de_Out_2, _Clamp_64ffae1e14feae89b94596b9cba21a55_Out_3);
    float4 _Branch_4e6bcacf8fcb5585abae3c176af03007_Out_3;
    Unity_Branch_float4(_Property_0d8e02a76989bb80ae497d44ac78381b_Out_0, _Remap_6823e14564e9eb8a8ef71bcec591d781_Out_3, _Blend_2a8b33610ca45d8389ae5bca9784d7de_Out_2, _Branch_4e6bcacf8fcb5585abae3c176af03007_Out_3);
    float _FresnelEffect_6dabda9e9184008dbdfce2ac745c66c0_Out_3;
    Unity_FresnelEffect_float(IN.WorldSpaceNormal, IN.WorldSpaceViewDirection, 10, _FresnelEffect_6dabda9e9184008dbdfce2ac745c66c0_Out_3);
    float _Remap_244e9fe231479c899040aad8bb5aa37c_Out_3;
    Unity_Remap_float(_FresnelEffect_6dabda9e9184008dbdfce2ac745c66c0_Out_3, float2 (0, 0.5), float2 (0.05, 1), _Remap_244e9fe231479c899040aad8bb5aa37c_Out_3);
    float _Blend_ec4b7034bfbcc28e9b21ae7806e93882_Out_2;
    Unity_Blend_Overwrite_float(_Remap_244e9fe231479c899040aad8bb5aa37c_Out_3, 1, _Blend_ec4b7034bfbcc28e9b21ae7806e93882_Out_2, _Clamp_64ffae1e14feae89b94596b9cba21a55_Out_3);
    float Slider_9331cbdecf72558e8c14e4d3f9a7360e = 0;
    surface.BaseColor = (_Branch_0d2a1beb339f47d7a3370ab32efd8e0c_Out_3.xyz);
    surface.Emission = (_Branch_4e6bcacf8fcb5585abae3c176af03007_Out_3.xyz);
    surface.Alpha = _Blend_ec4b7034bfbcc28e9b21ae7806e93882_Out_2;
    surface.AlphaClipThreshold = Slider_9331cbdecf72558e8c14e4d3f9a7360e;
    return surface;
}

// --------------------------------------------------
// Build Graph Inputs

VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
{
    VertexDescriptionInputs output;
    ZERO_INITIALIZE(VertexDescriptionInputs, output);

    output.ObjectSpaceNormal = input.normalOS;
    output.ObjectSpaceTangent = input.tangentOS.xyz;
    output.ObjectSpacePosition = input.positionOS;

    return output;
}
    SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
{
    SurfaceDescriptionInputs output;
    ZERO_INITIALIZE(SurfaceDescriptionInputs, output);

    // must use interpolated tangent, bitangent and normal before they are normalized in the pixel shader.
    float3 unnormalizedNormalWS = input.normalWS;
    const float renormFactor = 1.0 / length(unnormalizedNormalWS);


    output.WorldSpaceNormal = renormFactor * input.normalWS.xyz;		// we want a unit length Normal Vector node in shader graph


    output.WorldSpaceViewDirection = input.viewDirectionWS; //TODO: by default normalized in HD, but not in universal
    output.WorldSpacePosition = input.positionWS;
    output.ScreenPosition = ComputeScreenPos(TransformWorldToHClip(input.positionWS), _ProjectionParams.x);
    output.uv0 = input.texCoord0;
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

    #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"
#include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
#include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/LightingMetaPass.hlsl"

    ENDHLSL
}
Pass
{
        // Name: <None>
        Tags
        {
            "LightMode" = "Universal2D"
        }

        // Render State
        Cull Back
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
    #pragma only_renderers gles gles3 glcore d3d11
    #pragma multi_compile_instancing
    #pragma vertex vert
    #pragma fragment frag

        // DotsInstancingOptions: <None>
        // HybridV1InjectedBuiltinProperties: <None>

        // Keywords
        // PassKeywords: <None>
        // GraphKeywords: <None>

        // Defines
        #define _SURFACE_TYPE_TRANSPARENT 1
        #define _AlphaClip 1
        #define _NORMALMAP 1
        #define _NORMAL_DROPOFF_TS 1
        #define ATTRIBUTES_NEED_NORMAL
        #define ATTRIBUTES_NEED_TANGENT
        #define ATTRIBUTES_NEED_TEXCOORD0
        #define VARYINGS_NEED_POSITION_WS
        #define VARYINGS_NEED_NORMAL_WS
        #define VARYINGS_NEED_TEXCOORD0
        #define VARYINGS_NEED_VIEWDIRECTION_WS
        #define FEATURES_GRAPH_VERTEX
        /* WARNING: $splice Could not find named fragment 'PassInstancing' */
        #define SHADERPASS SHADERPASS_2D
        /* WARNING: $splice Could not find named fragment 'DotsInstancingVars' */

        // Includes
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
    #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
    #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
    #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
    #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
    #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"

        // --------------------------------------------------
        // Structs and Packing

        struct Attributes
    {
        float3 positionOS : POSITION;
        float3 normalOS : NORMAL;
        float4 tangentOS : TANGENT;
        float4 uv0 : TEXCOORD0;
        #if UNITY_ANY_INSTANCING_ENABLED
        uint instanceID : INSTANCEID_SEMANTIC;
        #endif
    };
    struct Varyings
    {
        float4 positionCS : SV_POSITION;
        float3 positionWS;
        float3 normalWS;
        float4 texCoord0;
        float3 viewDirectionWS;
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
        float3 WorldSpaceNormal;
        float3 WorldSpaceViewDirection;
        float3 WorldSpacePosition;
        float4 ScreenPosition;
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
        float3 interp0 : TEXCOORD0;
        float3 interp1 : TEXCOORD1;
        float4 interp2 : TEXCOORD2;
        float3 interp3 : TEXCOORD3;
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

        PackedVaryings PackVaryings(Varyings input)
    {
        PackedVaryings output;
        output.positionCS = input.positionCS;
        output.interp0.xyz = input.positionWS;
        output.interp1.xyz = input.normalWS;
        output.interp2.xyzw = input.texCoord0;
        output.interp3.xyz = input.viewDirectionWS;
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
    Varyings UnpackVaryings(PackedVaryings input)
    {
        Varyings output;
        output.positionCS = input.positionCS;
        output.positionWS = input.interp0.xyz;
        output.normalWS = input.interp1.xyz;
        output.texCoord0 = input.interp2.xyzw;
        output.viewDirectionWS = input.interp3.xyz;
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
float4 mainTexture_TexelSize;
float4 _MainColor;
float4 _EmissionColor;
float4 _FrenelColor;
float _Frenel;
float _Metalic;
float _Smoothness;
float4 _Occ_TexelSize;
float _Offset;
float _Range;
float Neon;
float Neon_1;
CBUFFER_END

// Object and Global properties
SAMPLER(SamplerState_Linear_Repeat);
SAMPLER(SamplerState_Trilinear_Clamp);
TEXTURE2D(mainTexture);
SAMPLER(samplermainTexture);
TEXTURE2D(_Occ);
SAMPLER(sampler_Occ);

// Graph Functions

void Unity_Blend_Overlay_float4(float4 Base, float4 Blend, out float4 Out, float Opacity)
{
    float4 result1 = 1.0 - 2.0 * (1.0 - Base) * (1.0 - Blend);
    float4 result2 = 2.0 * Base * Blend;
    float4 zeroOrOne = step(Base, 0.5);
    Out = result2 * zeroOrOne + (1 - zeroOrOne) * result1;
    Out = lerp(Base, Out, Opacity);
}

void Unity_Branch_float4(float Predicate, float4 True, float4 False, out float4 Out)
{
    Out = Predicate ? True : False;
}

void Unity_FresnelEffect_float(float3 Normal, float3 ViewDir, float Power, out float Out)
{
    Out = pow((1.0 - saturate(dot(normalize(Normal), normalize(ViewDir)))), Power);
}

void Unity_Remap_float(float In, float2 InMinMax, float2 OutMinMax, out float Out)
{
    Out = OutMinMax.x + (In - InMinMax.x) * (OutMinMax.y - OutMinMax.x) / (InMinMax.y - InMinMax.x);
}

void Unity_Subtract_float(float A, float B, out float Out)
{
    Out = A - B;
}

void Unity_Multiply_float(float A, float B, out float Out)
{
    Out = A * B;
}

void Unity_Clamp_float(float In, float Min, float Max, out float Out)
{
    Out = clamp(In, Min, Max);
}

void Unity_Blend_Overwrite_float(float Base, float Blend, out float Out, float Opacity)
{
    Out = lerp(Base, Blend, Opacity);
}

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

// Graph Pixel
struct SurfaceDescription
{
    float3 BaseColor;
    float Alpha;
    float AlphaClipThreshold;
};

SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
{
    SurfaceDescription surface = (SurfaceDescription)0;
    float _Property_db0d6ba2317f4ca885fffc369f6b7572_Out_0 = Neon_1;
    float4 _Property_404b2b1af018418b81f010f388917a24_Out_0 = _MainColor;
    UnityTexture2D _Property_9c598a886ed15f8aa999f5dbaa5a4e25_Out_0 = UnityBuildTexture2DStructNoScale(mainTexture);
    float4 _SampleTexture2D_9a0e43a036a3828e8771aff20cdda3ba_RGBA_0 = SAMPLE_TEXTURE2D(_Property_9c598a886ed15f8aa999f5dbaa5a4e25_Out_0.tex, _Property_9c598a886ed15f8aa999f5dbaa5a4e25_Out_0.samplerstate, IN.uv0.xy);
    float _SampleTexture2D_9a0e43a036a3828e8771aff20cdda3ba_R_4 = _SampleTexture2D_9a0e43a036a3828e8771aff20cdda3ba_RGBA_0.r;
    float _SampleTexture2D_9a0e43a036a3828e8771aff20cdda3ba_G_5 = _SampleTexture2D_9a0e43a036a3828e8771aff20cdda3ba_RGBA_0.g;
    float _SampleTexture2D_9a0e43a036a3828e8771aff20cdda3ba_B_6 = _SampleTexture2D_9a0e43a036a3828e8771aff20cdda3ba_RGBA_0.b;
    float _SampleTexture2D_9a0e43a036a3828e8771aff20cdda3ba_A_7 = _SampleTexture2D_9a0e43a036a3828e8771aff20cdda3ba_RGBA_0.a;
    float4 _Blend_07c1433efd838d88a6692dd2f3fa5cc1_Out_2;
    Unity_Blend_Overlay_float4(_SampleTexture2D_9a0e43a036a3828e8771aff20cdda3ba_RGBA_0, _Property_404b2b1af018418b81f010f388917a24_Out_0, _Blend_07c1433efd838d88a6692dd2f3fa5cc1_Out_2, 1);
    float4 _Branch_0d2a1beb339f47d7a3370ab32efd8e0c_Out_3;
    Unity_Branch_float4(_Property_db0d6ba2317f4ca885fffc369f6b7572_Out_0, _Property_404b2b1af018418b81f010f388917a24_Out_0, _Blend_07c1433efd838d88a6692dd2f3fa5cc1_Out_2, _Branch_0d2a1beb339f47d7a3370ab32efd8e0c_Out_3);
    float _FresnelEffect_6dabda9e9184008dbdfce2ac745c66c0_Out_3;
    Unity_FresnelEffect_float(IN.WorldSpaceNormal, IN.WorldSpaceViewDirection, 10, _FresnelEffect_6dabda9e9184008dbdfce2ac745c66c0_Out_3);
    float _Remap_244e9fe231479c899040aad8bb5aa37c_Out_3;
    Unity_Remap_float(_FresnelEffect_6dabda9e9184008dbdfce2ac745c66c0_Out_3, float2 (0, 0.5), float2 (0.05, 1), _Remap_244e9fe231479c899040aad8bb5aa37c_Out_3);
    float4 _ScreenPosition_724ad830a44b8582920d49bb9a3a8049_Out_0 = IN.ScreenPosition;
    float _Split_74ec02de1ed52489b7f5d8edf6bec915_R_1 = _ScreenPosition_724ad830a44b8582920d49bb9a3a8049_Out_0[0];
    float _Split_74ec02de1ed52489b7f5d8edf6bec915_G_2 = _ScreenPosition_724ad830a44b8582920d49bb9a3a8049_Out_0[1];
    float _Split_74ec02de1ed52489b7f5d8edf6bec915_B_3 = _ScreenPosition_724ad830a44b8582920d49bb9a3a8049_Out_0[2];
    float _Split_74ec02de1ed52489b7f5d8edf6bec915_A_4 = _ScreenPosition_724ad830a44b8582920d49bb9a3a8049_Out_0[3];
    float _Property_d93f8f362dde2481a1ff9b6a41471f97_Out_0 = _Offset;
    float _Subtract_63e4def5bfbcb0819237ae1e75f6f48f_Out_2;
    Unity_Subtract_float(_Split_74ec02de1ed52489b7f5d8edf6bec915_A_4, _Property_d93f8f362dde2481a1ff9b6a41471f97_Out_0, _Subtract_63e4def5bfbcb0819237ae1e75f6f48f_Out_2);
    float _Property_bf58377199188e8494396b8339c183d9_Out_0 = _Range;
    float _Multiply_52dbf97db03cea8683a73148cc653b01_Out_2;
    Unity_Multiply_float(_Subtract_63e4def5bfbcb0819237ae1e75f6f48f_Out_2, _Property_bf58377199188e8494396b8339c183d9_Out_0, _Multiply_52dbf97db03cea8683a73148cc653b01_Out_2);
    float _Clamp_64ffae1e14feae89b94596b9cba21a55_Out_3;
    Unity_Clamp_float(_Multiply_52dbf97db03cea8683a73148cc653b01_Out_2, 0, 1, _Clamp_64ffae1e14feae89b94596b9cba21a55_Out_3);
    float _Blend_ec4b7034bfbcc28e9b21ae7806e93882_Out_2;
    Unity_Blend_Overwrite_float(_Remap_244e9fe231479c899040aad8bb5aa37c_Out_3, 1, _Blend_ec4b7034bfbcc28e9b21ae7806e93882_Out_2, _Clamp_64ffae1e14feae89b94596b9cba21a55_Out_3);
    float Slider_9331cbdecf72558e8c14e4d3f9a7360e = 0;
    surface.BaseColor = (_Branch_0d2a1beb339f47d7a3370ab32efd8e0c_Out_3.xyz);
    surface.Alpha = _Blend_ec4b7034bfbcc28e9b21ae7806e93882_Out_2;
    surface.AlphaClipThreshold = Slider_9331cbdecf72558e8c14e4d3f9a7360e;
    return surface;
}

// --------------------------------------------------
// Build Graph Inputs

VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
{
    VertexDescriptionInputs output;
    ZERO_INITIALIZE(VertexDescriptionInputs, output);

    output.ObjectSpaceNormal = input.normalOS;
    output.ObjectSpaceTangent = input.tangentOS.xyz;
    output.ObjectSpacePosition = input.positionOS;

    return output;
}
    SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
{
    SurfaceDescriptionInputs output;
    ZERO_INITIALIZE(SurfaceDescriptionInputs, output);

    // must use interpolated tangent, bitangent and normal before they are normalized in the pixel shader.
    float3 unnormalizedNormalWS = input.normalWS;
    const float renormFactor = 1.0 / length(unnormalizedNormalWS);


    output.WorldSpaceNormal = renormFactor * input.normalWS.xyz;		// we want a unit length Normal Vector node in shader graph


    output.WorldSpaceViewDirection = input.viewDirectionWS; //TODO: by default normalized in HD, but not in universal
    output.WorldSpacePosition = input.positionWS;
    output.ScreenPosition = ComputeScreenPos(TransformWorldToHClip(input.positionWS), _ProjectionParams.x);
    output.uv0 = input.texCoord0;
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

    #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"
#include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
#include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/PBR2DPass.hlsl"

    ENDHLSL
}
    }
        CustomEditor "ShaderGraph.PBRMasterGUI"
        FallBack "Hidden/Shader Graph/FallbackError"
}