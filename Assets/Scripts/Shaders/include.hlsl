//UNITY_SHADER_NO_UPGRADE

#ifndef READ_COLOR_BUFFER
#define READ_COLOR_BUFFER

StructuredBuffer<float3> _ColorBuffer;

void ReadColorBuffer_float(float v, out float3 i)
{
    i = _ColorBuffer[(uint)v];
}

#endif //READ_COLOR_BUFFER

#ifndef INTERPOLATE_TEXTURE
#define INTERPOLATE_TEXTURE

void InterpolateTexture_float(float3 i, float4 red, float4 green, float4 blue, float4 black, out float4 c)
{
    float4 r = i.r*red;
    float4 g = i.g*green;
    float4 b = i.b*blue;
    float4 a = max(0.0, (1 - i.r - i.g - i.b))*black;
    c = r+g+b+a;
}

#endif //INTERPOLATE_TEXTURE

#ifndef INTERPOLATE_NORMAL
#define INTERPOLATE_NORMAL

void InterpolateNormal_float(float3 i, float3 red, float3 green, float3 blue, float3 black, out float3 c)
{
    float3 r = i.r*red;
    float3 g = i.g*green;
    float3 b = i.b*blue;
    float3 a = max(0.0, (1 - i.r - i.g - i.b))*black;
    c = normalize(r+g+b+a);
}

#endif //INTERPOLATE_NORMAL


// #ifndef TRIPLANAR_ARRAY
// #define TRIPLANAR_ARRAY
// void TriplanarArray_float(2D texArray, out float4 Out) {
//     float3 Node_UV = IN.AbsoluteWorldSpacePosition;
//     float3 Node_Blend = SafePositivePow_float(IN.WorldSpaceNormal, min(float(1), floor(log2(Min_float())/log2(1/sqrt(3)))) );
//     Node_Blend /= dot(Node_Blend, 1.0);
//     float4 Node_X = SAMPLE_TEXTURE2D(UnityBuildTexture2DStructNoScale(texArray).tex, UnityBuildTexture2DStructNoScale(texArray).samplerstate, Node_UV.zy);
//     float4 Node_Y = SAMPLE_TEXTURE2D(UnityBuildTexture2DStructNoScale(texArray).tex, UnityBuildTexture2DStructNoScale(texArray).samplerstate, Node_UV.xz);
//     float4 Node_Z = SAMPLE_TEXTURE2D(UnityBuildTexture2DStructNoScale(texArray).tex, UnityBuildTexture2DStructNoScale(texArray).samplerstate, Node_UV.xy);
//     Out = Node_X * Node_Blend.x + Node_Y * Node_Blend.y + Node_Z * Node_Blend.z;

// }

/*
SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
{
SurfaceDescription surface = (SurfaceDescription)0;
float3 Triplanar_b1367512169e41a4beea52116dad88df_UV = IN.AbsoluteWorldSpacePosition * float(1);
float3 Triplanar_b1367512169e41a4beea52116dad88df_Blend = SafePositivePow_float(IN.WorldSpaceNormal, min(float(1), floor(log2(Min_float())/log2(1/sqrt(3)))) );
Triplanar_b1367512169e41a4beea52116dad88df_Blend /= dot(Triplanar_b1367512169e41a4beea52116dad88df_Blend, 1.0);
float4 Triplanar_b1367512169e41a4beea52116dad88df_X = SAMPLE_TEXTURE2D(UnityBuildTexture2DStructNoScale(_Triplanar_b1367512169e41a4beea52116dad88df_Texture_1_Texture2D).tex, UnityBuildTexture2DStructNoScale(_Triplanar_b1367512169e41a4beea52116dad88df_Texture_1_Texture2D).samplerstate, Triplanar_b1367512169e41a4beea52116dad88df_UV.zy);
float4 Triplanar_b1367512169e41a4beea52116dad88df_Y = SAMPLE_TEXTURE2D(UnityBuildTexture2DStructNoScale(_Triplanar_b1367512169e41a4beea52116dad88df_Texture_1_Texture2D).tex, UnityBuildTexture2DStructNoScale(_Triplanar_b1367512169e41a4beea52116dad88df_Texture_1_Texture2D).samplerstate, Triplanar_b1367512169e41a4beea52116dad88df_UV.xz);
float4 Triplanar_b1367512169e41a4beea52116dad88df_Z = SAMPLE_TEXTURE2D(UnityBuildTexture2DStructNoScale(_Triplanar_b1367512169e41a4beea52116dad88df_Texture_1_Texture2D).tex, UnityBuildTexture2DStructNoScale(_Triplanar_b1367512169e41a4beea52116dad88df_Texture_1_Texture2D).samplerstate, Triplanar_b1367512169e41a4beea52116dad88df_UV.xy);
float4 _Triplanar_b1367512169e41a4beea52116dad88df_Out_0_Vector4 = Triplanar_b1367512169e41a4beea52116dad88df_X * Triplanar_b1367512169e41a4beea52116dad88df_Blend.x + Triplanar_b1367512169e41a4beea52116dad88df_Y * Triplanar_b1367512169e41a4beea52116dad88df_Blend.y + Triplanar_b1367512169e41a4beea52116dad88df_Z * Triplanar_b1367512169e41a4beea52116dad88df_Blend.z;
surface.Out = all(isfinite(float4 (0, 0, 0, 0))) ? half4(_Triplanar_b1367512169e41a4beea52116dad88df_Out_0_Vector4.x, _Triplanar_b1367512169e41a4beea52116dad88df_Out_0_Vector4.y, _Triplanar_b1367512169e41a4beea52116dad88df_Out_0_Vector4.z, 1.0) : float4(1.0f, 0.0f, 1.0f, 1.0f);
return surface;
}
*/

// void TriplanarArray(Texture2DArray texArray, float3, float3 WorldSpaceNormal, float blendFactor, out float4 result) {
//     float3 Node_UV = IN.AbsoluteWorldSpacePosition;
//     float3 Node_Blend = SafePositivePow_float(IN.WorldSpaceNormal, min(float(1), floor(log2(Min_float())/log2(1/sqrt(3)))) );
//     Node_Blend /= (Node_Blend.x + Node_Blend.y + Node_Blend.z ).xxx;
//     float3 Node_X = UnpackNormal(SAMPLE_TEXTURE2D(Texture, Texture.samplerstate, Node_UV.zy));
//     float3 Node_Y = UnpackNormal(SAMPLE_TEXTURE2D(Texture, Texture.samplerstate, Node_UV.xz));
//     float3 Node_Z = UnpackNormal(SAMPLE_TEXTURE2D(Texture, Texture.samplerstate, Node_UV.xy));
//     Node_X = float3(Node_X.xy + IN.WorldSpaceNormal.zy, abs(Node_X.z) * IN.WorldSpaceNormal.x);
//     Node_Y = float3(Node_Y.xy + IN.WorldSpaceNormal.xz, abs(Node_Y.z) * IN.WorldSpaceNormal.y);
//     Node_Z = float3(Node_Z.xy + IN.WorldSpaceNormal.xy, abs(Node_Z.z) * IN.WorldSpaceNormal.z);
//     float4 Out = float4(normalize(Node_X.zyx * Node_Blend.x + Node_Y.xzy * Node_Blend.y + Node_Z.xyz * Node_Blend.z), 1);
//     float3x3 Node_Transform = float3x3(IN.WorldSpaceTangent, IN.WorldSpaceBiTangent, IN.WorldSpaceNormal);
//     Out.rgb = TransformWorldToTangent(Out.rgb, Node_Transform);
// }

/*
SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
{
SurfaceDescription surface = (SurfaceDescription)0;
UnityTexture2D _Property_7c1567d0571043bdb3af1f815c23cb0a_Out_0_Texture2D = UnityBuildTexture2DStructNoScale(_RedNormal);
float3 Triplanar_11a406afd71b4cc2a8075317a13e9b76_UV = IN.AbsoluteWorldSpacePosition * float(1);
float3 Triplanar_11a406afd71b4cc2a8075317a13e9b76_Blend = SafePositivePow_float(IN.WorldSpaceNormal, min(float(1), floor(log2(Min_float())/log2(1/sqrt(3)))) );
Triplanar_11a406afd71b4cc2a8075317a13e9b76_Blend /= (Triplanar_11a406afd71b4cc2a8075317a13e9b76_Blend.x + Triplanar_11a406afd71b4cc2a8075317a13e9b76_Blend.y + Triplanar_11a406afd71b4cc2a8075317a13e9b76_Blend.z ).xxx;
float3 Triplanar_11a406afd71b4cc2a8075317a13e9b76_X = UnpackNormal(SAMPLE_TEXTURE2D(_Property_7c1567d0571043bdb3af1f815c23cb0a_Out_0_Texture2D.tex, _Property_7c1567d0571043bdb3af1f815c23cb0a_Out_0_Texture2D.samplerstate, Triplanar_11a406afd71b4cc2a8075317a13e9b76_UV.zy));
float3 Triplanar_11a406afd71b4cc2a8075317a13e9b76_Y = UnpackNormal(SAMPLE_TEXTURE2D(_Property_7c1567d0571043bdb3af1f815c23cb0a_Out_0_Texture2D.tex, _Property_7c1567d0571043bdb3af1f815c23cb0a_Out_0_Texture2D.samplerstate, Triplanar_11a406afd71b4cc2a8075317a13e9b76_UV.xz));
float3 Triplanar_11a406afd71b4cc2a8075317a13e9b76_Z = UnpackNormal(SAMPLE_TEXTURE2D(_Property_7c1567d0571043bdb3af1f815c23cb0a_Out_0_Texture2D.tex, _Property_7c1567d0571043bdb3af1f815c23cb0a_Out_0_Texture2D.samplerstate, Triplanar_11a406afd71b4cc2a8075317a13e9b76_UV.xy));
Triplanar_11a406afd71b4cc2a8075317a13e9b76_X = float3(Triplanar_11a406afd71b4cc2a8075317a13e9b76_X.xy + IN.WorldSpaceNormal.zy, abs(Triplanar_11a406afd71b4cc2a8075317a13e9b76_X.z) * IN.WorldSpaceNormal.x);
Triplanar_11a406afd71b4cc2a8075317a13e9b76_Y = float3(Triplanar_11a406afd71b4cc2a8075317a13e9b76_Y.xy + IN.WorldSpaceNormal.xz, abs(Triplanar_11a406afd71b4cc2a8075317a13e9b76_Y.z) * IN.WorldSpaceNormal.y);
Triplanar_11a406afd71b4cc2a8075317a13e9b76_Z = float3(Triplanar_11a406afd71b4cc2a8075317a13e9b76_Z.xy + IN.WorldSpaceNormal.xy, abs(Triplanar_11a406afd71b4cc2a8075317a13e9b76_Z.z) * IN.WorldSpaceNormal.z);
float4 _Triplanar_11a406afd71b4cc2a8075317a13e9b76_Out_0_Vector4 = float4(Triplanar_11a406afd71b4cc2a8075317a13e9b76_X.zyx * Triplanar_11a406afd71b4cc2a8075317a13e9b76_Blend.x + Triplanar_11a406afd71b4cc2a8075317a13e9b76_Y.xzy * Triplanar_11a406afd71b4cc2a8075317a13e9b76_Blend.y + Triplanar_11a406afd71b4cc2a8075317a13e9b76_Z.xyz * Triplanar_11a406afd71b4cc2a8075317a13e9b76_Blend.z, 1);
{
// Converting Normal from AbsoluteWorld to Tangent via world space
float3 world;
world = _Triplanar_11a406afd71b4cc2a8075317a13e9b76_Out_0_Vector4.rgb;
{
float3x3 tangentTransform = float3x3(IN.WorldSpaceTangent, IN.WorldSpaceBiTangent, IN.WorldSpaceNormal);
_Triplanar_11a406afd71b4cc2a8075317a13e9b76_Out_0_Vector4.rgb = TransformWorldToTangent(world, tangentTransform, true);
}
}
surface.Out = all(isfinite(_Triplanar_11a406afd71b4cc2a8075317a13e9b76_Out_0_Vector4)) ? half4(_Triplanar_11a406afd71b4cc2a8075317a13e9b76_Out_0_Vector4.x, _Triplanar_11a406afd71b4cc2a8075317a13e9b76_Out_0_Vector4.y, _Triplanar_11a406afd71b4cc2a8075317a13e9b76_Out_0_Vector4.z, 1.0) : float4(1.0f, 0.0f, 1.0f, 1.0f);
return surface;
}
*/


// #endif //TRIPLANAR_ARRAY
