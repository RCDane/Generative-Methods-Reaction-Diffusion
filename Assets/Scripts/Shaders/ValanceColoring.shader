Shader "Unlit/ValanceColoring"
{
    Properties
    {
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
            };
 
            struct v2f
            {
                float4 vertex : SV_POSITION;
                float4 color : COLOR;
            };
 
            StructuredBuffer<float3> _ColorBuffer;
 
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.color = float4(_ColorBuffer[v.id], 1.0);
                return o;
            }
 
            fixed4 frag (v2f i) : SV_Target
            {
                return i.color;
            }
            ENDCG
        }
    }
}
