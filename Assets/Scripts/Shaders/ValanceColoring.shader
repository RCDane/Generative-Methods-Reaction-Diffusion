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
// Upgrade NOTE: excluded shader from DX11; has structs without semantics (struct v2f members worldVertex)
#pragma exclude_renderers d3d11
            #pragma vertex vert
            #pragma fragment frag
 
            #include "UnityCG.cginc"
            
            struct TriplanarUV {
                float2 x,y,z;
            };


            TriplanarUV GetTriplanarUV(float3 worldPos, float3 normal) {
                TriplanarUV uv;
                uv.x = worldPos.yz;
                uv.y = worldPos.xz;
                uv.z = worldPos.xy;
                // if (normal.x < 0) {
                //     uv.x.x = -uv.x.x;
                // }
                // if (normal.y < 0) {
                //     uv.y.x = -uv.y.x;
                // }
                // if (normal.z >= 0) {
                //     uv.z.x = -uv.z.x;
                // }
                uv.x.y += 0.5;
	            uv.z.x += 0.5;
                return uv;
            }
            float3 GetTriplanarWeights (float3 normal) {
                float3 triW = abs(normal);
                return triW / (triW.x + triW.y + triW.z);
            }


            struct appdata
            {
                float4 vertex : POSITION;
                uint id : SV_VertexID;
                float2 uv : TEXCOORD0;
                float3 normal : NORMAL;
            };
 
            struct v2f
            {
                float4 worldVertex : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float4 color : COLOR;
                float3 normal : NORMAL;
            };
 
            StructuredBuffer<float3> _ColorBuffer;
 
            v2f vert (appdata v)
            {
                v2f o;
                o.worldVertex =  mul(unity_ObjectToWorld, v.vertex) ;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.normal = UnityObjectToWorldNormal(v.normal);
                o.color = float4(_ColorBuffer[v.id], 1.0);
                return o;
            }

            sampler2D _RedTex;
            sampler2D _GreenTex;
            sampler2D _BlueTex;
            sampler2D _BlackTex;

            float3 TriplanarLoopup(sampler2D tex, TriplanarUV uv, float3 weights){
                float3 x = tex2D(tex, uv.x).rgb;
                float3 y = tex2D(tex, uv.y).rgb;
                float3 z = tex2D(tex, uv.z).rgb;
                return x * weights.x + y * weights.y + z * weights.z;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                TriplanarUV triUV = GetTriplanarUV(i.worldVertex.xyz, i.normal);
                float3 triW = GetTriplanarWeights(i.normal);
                
                float4 red = float4(TriplanarLoopup(_RedTex, triUV, triW), 1.0);                
                float4 green = float4(TriplanarLoopup(_GreenTex, triUV, triW), 1.0);
                float4 blue = float4(TriplanarLoopup(_BlueTex, triUV, triW),1.0);
                float4 black = float4(TriplanarLoopup(_BlackTex, triUV, triW),1.0);

                float4 r = i.color.r*red;
                float4 g = i.color.g*green;
                float4 b = i.color.b*blue;
                float4 z = max(0.0, (1 - i.color.r - i.color.g - i.color.b))*black;
                return r+g+b+z;
                // return float4(triUV.x, 0.0,1.0);
                // return i.vertex;
            }
            ENDCG
        }
    }
}
