Shader "Unlit/ValanceColoring"
{
    Properties
    {
        _RedTex ("RedTexture", 2D) = "red" {}
        _GreenTex ("GreenTexture", 2D) = "bump" {}
        _BlueTex ("BlueTexture", 2D) = "gray" {}
        _BlackTex ("BlackTexture", 2D) = "black" {}
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
 
            #include "UnityCG.cginc"
 
            struct appdata
            {
                float4 vertex : POSITION;
                uint id : SV_VertexID;
                float2 uv : TEXCOORD0;
            };
 
            struct v2f
            {
                float4 vertex : SV_POSITION;
                float4 color : COLOR;
                float2 uv : TEXCOORD0;
            };
 
            StructuredBuffer<float3> _ColorBuffer;
 
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.color = float4(_ColorBuffer[v.id], 1.0);
                o.uv = v.uv;
                return o;
            }

            sampler2D _RedTex;
            sampler2D _GreenTex;
            sampler2D _BlueTex;
            sampler2D _BlackTex;

            fixed4 frag (v2f i) : SV_Target
            {
                float4 r = i.color.r*tex2D(_RedTex, i.uv);
                float4 g = i.color.g*tex2D(_GreenTex, i.uv);
                float4 b = i.color.b*tex2D(_BlueTex, i.uv);
                float4 z = max(0.0, (1 - i.color.r - i.color.g - i.color.b))*tex2D(_BlackTex, i.uv);
                return r+g+b+z;
            }
            ENDCG
        }
    }
}
