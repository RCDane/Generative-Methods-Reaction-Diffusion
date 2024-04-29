Shader "Custom/WaveShader"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Texture ("Texture", 2D) = "white" {}
        _Metallic ("Metallic", Range(0,1)) = 0.0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Standard fullforwardshadows
        #pragma vertex vert
        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0
        

        struct Input
        {
            float2 uv : TEXCOORD0;
            float height;
            float speed;
        };

        struct appdata {
            float4 vertex : POSITION;
            float4 tangent : TANGENT;
            float3 normal : NORMAL;
            float4 texcoord : TEXCOORD0;
            float4 texcoord1 : TEXCOORD1;
            float4 texcoord2 : TEXCOORD2;
            float4 texcoord3 : TEXCOORD3;
            float4 texcoord4 : TEXCOORD4;
            float4 texcoord5 : TEXCOORD5;
            float4 texcoord6 : TEXCOORD6;
            float4 texcoord7 : TEXCOORD7;
            fixed4 color : COLOR;
            uint id : SV_VertexID;
            UNITY_VERTEX_INPUT_INSTANCE_ID
        };

        #ifdef SHADER_API_VULKAN
            StructuredBuffer<float> _WaveHeights;
            StructuredBuffer<float> _WaveSpeeds;
        #endif
        void vert(inout appdata v, out Input o)
        {
            UNITY_INITIALIZE_OUTPUT(Input, o);
            o.uv = v.texcoord.xy;
            #ifdef SHADER_API_VULKAN
                o.height = _WaveHeights[v.id];
                o.speed = _WaveSpeeds[v.id];
            #endif
            // displace vertex along normal by height
            v.vertex.xyz += v.normal * o.height;
        }

        half _Glossiness;
        half _Metallic;
        fixed4 _Color;
        sampler2D _MainTex;

        
        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        // #pragma instancing_options assumeuniformscaling
            // put more per-instance properties here

        void surf (Input IN, inout SurfaceOutputStandard o)
        {

            // Albedo comes from a texture tinted by color
            fixed4 c = _Color;
            fixed4 tex = tex2D(_MainTex, IN.uv);
            c *= tex;
            o.Albedo = c.rgb;
            // Metallic and smoothness come from slider variables
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
            o.Alpha = c.a;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
