Shader "FX/CloudBase"
{
  Properties
  {
    _ColorTint ("Color Tint", Color) = (1,1,1,1)
    _BumpMap ("Normal", 2D) = "bump" {}
    _Ramp2D ("BRDF Ramp", 2D) = "gray" {}
    _RimPower ("Rim Exponent", Range(0.5, 25)) = 3
  }
  SubShader
  {
    Tags
    { 
      "RenderType" = "Opaque"
    }
    LOD 200
    Pass // ind: 1, name: FORWARD
    {
      Name "FORWARD"
      Tags
      { 
        "LIGHTMODE" = "FORWARDBASE"
        "RenderType" = "Opaque"
        "SHADOWSUPPORT" = "true"
      }
      LOD 200
      // m_ProgramMask = 6
      CGPROGRAM
      //#pragma target 4.0
      
      #pragma vertex vert
      #pragma fragment frag
      
      #include "UnityCG.cginc"
      
      
      #define CODE_BLOCK_VERTEX
      uniform float4 _BumpMap_ST;
      //uniform float4 _ProjectionParams;
      //uniform float4 unity_4LightPosX0;
      //uniform float4 unity_4LightPosY0;
      //uniform float4 unity_4LightPosZ0;
      //uniform float4 unity_4LightAtten0;
      //uniform float4 unity_LightColor;
      //uniform float4 unity_WorldTransformParams;
      //uniform float4x4 unity_MatrixVP;
      uniform int unity_BaseInstanceID;
      //uniform float4x4 unity_ObjectToWorldArray;
      //uniform float4x4 unity_WorldToObjectArray;
      uniform float4 unity_SHArArray;
      uniform float4 unity_SHAgArray;
      uniform float4 unity_SHAbArray;
      uniform float4 unity_SHBrArray;
      uniform float4 unity_SHBgArray;
      uniform float4 unity_SHBbArray;
      uniform float4 unity_SHCArray;
      uniform float4 _LightColor0;
      uniform float _RimPower;
      uniform float4 _ColorTint;
      //uniform float3 _WorldSpaceCameraPos;
      //uniform float4 _WorldSpaceLightPos0;
      //uniform float4 unity_FogColor;
      //uniform float4 unity_FogParams;
      uniform sampler2D _BumpMap;
      uniform sampler2D _Ramp2D;
      struct appdata_t
      {
          float4 vertex :POSITION0;
          float4 tangent :TANGENT;
          float3 normal :NORMAL;
          float4 texcoord :TEXCOORD0;
          float4 texcoord1 :TEXCOORD1;
          float4 texcoord2 :TEXCOORD2;
          float4 texcoord3 :TEXCOORD3;
          float4 color :COLOR;
          uint SV_InstanceID :InstanceID;
      };
      
      struct OUT_Data_Vert
      {
          float4 vertex :SV_POSITION;
          float2 texcoord :TEXCOORD0;
          float texcoord5 :TEXCOORD5;
          float4 texcoord1 :TEXCOORD1;
          float4 texcoord2 :TEXCOORD2;
          float4 texcoord3 :TEXCOORD3;
          float3 texcoord4 :TEXCOORD4;
          float4 texcoord6 :TEXCOORD6;
          float4 texcoord7 :TEXCOORD7;
          uint SV_InstanceID :InstanceID;
      };
      
      struct v2f
      {
          float4 vertex :SV_POSITION;
          float2 texcoord :TEXCOORD0;
          float texcoord5 :TEXCOORD5;
          float4 texcoord1 :TEXCOORD1;
          float4 texcoord2 :TEXCOORD2;
          float4 texcoord3 :TEXCOORD3;
          float3 texcoord4 :TEXCOORD4;
          float4 texcoord6 :TEXCOORD6;
          float4 texcoord7 :TEXCOORD7;
          uint SV_InstanceID :InstanceID;
      };
      
      struct OUT_Data_Frag
      {
          float4 color :SV_Target;
      };
      
      float4 u_xlat0;
      float4 u_xlat1;
      float4 u_xlat2;
      float4 u_xlat3;
      float4 u_xlat4;
      float4 u_xlat5;
      float4 u_xlat6;
      OUT_Data_Vert vert(appdata_t in_v)
      {
          OUT_Data_Vert out_v;
          u_xlat0.x = (in_v.SV_InstanceID.x + unity_BaseInstanceID.x);
          u_xlat0.y = (u_xlat0.x >> 3);
          u_xlat0.x = ((u_xlat0.x * 7) & -1);
          u_xlat1 = (in_v.vertex.yyyy * conv_mxt4x4_(u_xlat0.y + (-10))(unity_WorldToObjectArray));
          u_xlat1 = ((conv_mxt4x4_(u_xlat0.y + (-11))(unity_WorldToObjectArray) * in_v.vertex.xxxx) + u_xlat1);
          u_xlat1 = ((conv_mxt4x4_(u_xlat0.y + (-9))(unity_WorldToObjectArray) * in_v.vertex.zzzz) + u_xlat1);
          u_xlat2 = (u_xlat1 + conv_mxt4x4_(u_xlat0.y + (-8))(unity_WorldToObjectArray));
          u_xlat1.xyz = ((conv_mxt4x4_(u_xlat0.y + (-8))(unity_WorldToObjectArray).xyz * in_v.vertex.www) + u_xlat1.xyz);
          u_xlat2 = mul(unity_MatrixVP, u_xlat2);
          out_v.vertex = u_xlat2;
          out_v.texcoord.xy = TRANSFORM_TEX(in_v.texcoord.xy, _BumpMap);
          out_v.texcoord5.x = u_xlat2.z;
          out_v.texcoord6.zw = u_xlat2.zw;
          u_xlat3.xyz = (in_v.tangent.yyy * conv_mxt4x4_(u_xlat0.y + (-10))(unity_WorldToObjectArray).yzx);
          u_xlat3.xyz = ((conv_mxt4x4_(u_xlat0.y + (-11))(unity_WorldToObjectArray).yzx * in_v.tangent.xxx) + u_xlat3.xyz);
          u_xlat3.xyz = ((conv_mxt4x4_(u_xlat0.y + (-9))(unity_WorldToObjectArray).yzx * in_v.tangent.zzz) + u_xlat3.xyz);
          u_xlat0.z = dot(u_xlat3.xyzx, u_xlat3.xyzx);
          u_xlat0.z = rsqrt(u_xlat0.z);
          u_xlat3.xyz = (u_xlat0.zzz * u_xlat3.xyz);
          out_v.texcoord1.x = u_xlat3.z;
          u_xlat0.z = (in_v.tangent.w * unity_WorldTransformParams.w);
          u_xlat4.x = dot(in_v.normal.xyzx, conv_mxt4x4_(u_xlat0.y + (-7))(unity_WorldToObjectArray).xyzx);
          u_xlat4.y = dot(in_v.normal.xyzx, conv_mxt4x4_(u_xlat0.y + (-6))(unity_WorldToObjectArray).xyzx);
          u_xlat4.z = dot(in_v.normal.xyzx, conv_mxt4x4_(u_xlat0.y + (-5))(unity_WorldToObjectArray).xyzx);
          u_xlat0.y = dot(u_xlat4.xyzx, u_xlat4.xyzx);
          u_xlat0.y = rsqrt(u_xlat0.y);
          u_xlat4.xyz = (u_xlat0.yyy * u_xlat4.xyz);
          u_xlat5.xyz = (u_xlat3.xyz * u_xlat4.zxy);
          u_xlat5.xyz = ((u_xlat4.yzx * u_xlat3.yzx) - u_xlat5.xyz);
          u_xlat0.yzw = (u_xlat0.zzz * u_xlat5.xyz);
          out_v.texcoord1.y = u_xlat0.y;
          out_v.texcoord1.w = u_xlat1.x;
          out_v.texcoord1.z = u_xlat4.x;
          out_v.texcoord2.x = u_xlat3.x;
          out_v.texcoord3.x = u_xlat3.y;
          out_v.texcoord2.w = u_xlat1.y;
          out_v.texcoord2.y = u_xlat0.z;
          out_v.texcoord3.y = u_xlat0.w;
          out_v.texcoord2.z = u_xlat4.y;
          out_v.texcoord3.w = u_xlat1.z;
          out_v.texcoord3.z = u_xlat4.z;
          u_xlat0.y = (u_xlat4.y * u_xlat4.y);
          u_xlat0.y = ((u_xlat4.x * u_xlat4.x) - u_xlat0.y);
          u_xlat3 = (u_xlat4.yzzx * u_xlat4.xyzz);
          u_xlat5.x = dot(unity_SHCArray, u_xlat3);
          u_xlat5.y = dot(unity_SHCArray, u_xlat3);
          u_xlat5.z = dot(unity_SHCArray, u_xlat3);
          u_xlat0.yzw = ((unity_SHCArray.xyz * u_xlat0.yyy) + u_xlat5.xyz);
          u_xlat4.w = 1;
          u_xlat3.x = dot(unity_SHCArray, u_xlat4);
          u_xlat3.y = dot(unity_SHCArray, u_xlat4);
          u_xlat3.z = dot(unity_SHCArray, u_xlat4);
          u_xlat0.xyz = (u_xlat0.yzw + u_xlat3.xyz);
          u_xlat0.xyz = max(u_xlat0.xyz, float3(0, 0, 0));
          u_xlat0.xyz = log(u_xlat0.xyz);
          u_xlat0.xyz = (u_xlat0.xyz * float3(0.416667, 0.416667, 0.416667));
          u_xlat0.xyz = exp(u_xlat0.xyzx);
          u_xlat0.xyz = ((u_xlat0.xyz * float3(1.055, 1.055, 1.055)) + float3(-0.055, (-0.055), (-0.055)));
          u_xlat0.xyz = max(u_xlat0.xyz, float3(0, 0, 0));
          u_xlat3 = ((-u_xlat1.yyyy) + unity_LightColor);
          u_xlat5 = (u_xlat4.yyyy * u_xlat3);
          u_xlat3 = (u_xlat3 * u_xlat3);
          u_xlat6 = ((-u_xlat1.xxxx) + unity_LightColor);
          u_xlat1 = ((-u_xlat1.zzzz) + unity_LightColor);
          u_xlat5 = ((u_xlat6 * u_xlat4.xxxx) + u_xlat5);
          u_xlat4 = ((u_xlat1 * u_xlat4.zzzz) + u_xlat5);
          u_xlat3 = ((u_xlat6 * u_xlat6) + u_xlat3);
          u_xlat1 = ((u_xlat1 * u_xlat1) + u_xlat3);
          u_xlat1 = max(u_xlat1, float4(1E-06, 1E-06, 1E-06, 1E-06));
          u_xlat3 = rsqrt(u_xlat1);
          u_xlat1 = ((u_xlat1 * unity_LightColor) + float4(1, 1, 1, 1));
          u_xlat1 = (float4(1, 1, 1, 1) / u_xlat1);
          u_xlat3 = (u_xlat3 * u_xlat4);
          u_xlat3 = max(u_xlat3, float4(0, 0, 0, 0));
          u_xlat1 = (u_xlat1 * u_xlat3);
          u_xlat3.xyz = (u_xlat1.yyy * unity_LightColor.xyz);
          u_xlat3.xyz = ((unity_LightColor.xyz * u_xlat1.xxx) + u_xlat3.xyz);
          u_xlat1.xyz = ((unity_LightColor.xyz * u_xlat1.zzz) + u_xlat3.xyz);
          u_xlat1.xyz = ((unity_LightColor.xyz * u_xlat1.www) + u_xlat1.xyz);
          out_v.texcoord4.xyz = (u_xlat0.xyz + u_xlat1.xyz);
          u_xlat0.x = (u_xlat2.y * _ProjectionParams.x);
          u_xlat1.xz = (u_xlat2.xw * float2(0.5, 0.5));
          u_xlat1.w = (u_xlat0.x * 0.5);
          out_v.texcoord6.xy = (u_xlat1.zz + u_xlat1.xw);
          out_v.texcoord7 = float4(0, 0, 0, 0);
          in_v.SV_InstanceID.x = in_v.SV_InstanceID.x;
          return out_v;
      }
      
      #define CODE_BLOCK_FRAGMENT
      float4 u_xlat0_d;
      float4 u_xlat1_d;
      float4 u_xlat2_d;
      float4 u_xlat3_d;
      OUT_Data_Frag frag(v2f in_f)
      {
          OUT_Data_Frag out_f;
          u_xlat0_d = tex2D(_BumpMap, in_f.texcoord.xy);
          u_xlat0_d.x = (u_xlat0_d.w * u_xlat0_d.x);
          u_xlat0_d.xy = ((u_xlat0_d.xy * float2(2, 2)) + float2(-1, (-1)));
          u_xlat0_d.w = dot(u_xlat0_d.xyxx, u_xlat0_d.xyxx);
          u_xlat0_d.w = min(u_xlat0_d.w, 1);
          u_xlat0_d.w = ((-u_xlat0_d.w) + 1);
          u_xlat0_d.z = sqrt(u_xlat0_d.w);
          u_xlat1_d.x = dot(in_f.texcoord1.xyzx, u_xlat0_d.xyzx);
          u_xlat1_d.y = dot(in_f.texcoord2.xyzx, u_xlat0_d.xyzx);
          u_xlat1_d.z = dot(in_f.texcoord3.xyzx, u_xlat0_d.xyzx);
          u_xlat0_d.w = dot(u_xlat1_d.xyzx, u_xlat1_d.xyzx);
          u_xlat0_d.w = rsqrt(u_xlat0_d.w);
          u_xlat1_d.xyz = (u_xlat0_d.www * u_xlat1_d.xyz);
          u_xlat0_d.w = dot(u_xlat1_d.xyzx, _WorldSpaceLightPos0.xyzx);
          u_xlat2_d.y = ((u_xlat0_d.w * 0.33) + 0.58);
          u_xlat3_d.x = in_f.texcoord1.w;
          u_xlat3_d.y = in_f.texcoord2.w;
          u_xlat3_d.z = in_f.texcoord3.w;
          u_xlat3_d.xyz = ((-u_xlat3_d.xyz) + _ProjectionParams.xyz);
          u_xlat0_d.w = dot(u_xlat3_d.xyzx, u_xlat3_d.xyzx);
          u_xlat0_d.w = rsqrt(u_xlat0_d.w);
          u_xlat3_d.xyz = (u_xlat0_d.www * u_xlat3_d.xyz);
          u_xlat2_d.x = dot(u_xlat1_d.xyzx, u_xlat3_d.xyzx);
          u_xlat1_d = tex2D(_Ramp2D, u_xlat2_d.xy);
          u_xlat1_d.xyz = (u_xlat1_d.xyz * _ColorTint.xyz);
          u_xlat2_d.xyz = (_ColorTint.xyz * float3(0.5, 0.5, 0.5));
          u_xlat1_d.xyz = ((u_xlat2_d.xyz * in_f.texcoord4.xyz) + u_xlat1_d.xyz);
          u_xlat2_d.xyz = (u_xlat3_d.yyy * in_f.texcoord2.xyz);
          u_xlat2_d.xyz = ((in_f.texcoord1.xyz * u_xlat3_d.xxx) + u_xlat2_d.xyz);
          u_xlat2_d.xyz = ((in_f.texcoord3.xyz * u_xlat3_d.zzz) + u_xlat2_d.xyz);
          u_xlat0_d.w = dot(u_xlat2_d.xyzx, u_xlat2_d.xyzx);
          u_xlat0_d.w = rsqrt(u_xlat0_d.w);
          u_xlat2_d.xyz = (u_xlat0_d.www * u_xlat2_d.xyz);
          u_xlat0_d.x = saturate(dot(u_xlat2_d.xyzx, u_xlat0_d.xyzx));
          u_xlat0_d.x = ((-u_xlat0_d.x) + 1);
          u_xlat0_d.x = log(u_xlat0_d.x);
          u_xlat0_d.x = (u_xlat0_d.x * _ColorTint.x);
          u_xlat0_d.x = exp(u_xlat0_d.x);
          u_xlat0_d.xyz = ((u_xlat0_d.xxx * float3(0.5, 0.5, 0.5)) + u_xlat1_d.xyz);
          u_xlat0_d.xyz = (u_xlat0_d.xyz - unity_FogParams.xyz);
          u_xlat0_d.w = (in_f.texcoord5.x / _ProjectionParams.y);
          u_xlat0_d.w = ((-u_xlat0_d.w) + 1);
          u_xlat0_d.w = (u_xlat0_d.w * _ProjectionParams.z);
          u_xlat0_d.w = max(u_xlat0_d.w, 0);
          u_xlat0_d.w = (u_xlat0_d.w * unity_FogParams.y);
          u_xlat0_d.w = exp((-u_xlat0_d.w));
          u_xlat0_d.w = min(u_xlat0_d.w, 1);
          out_f.color.xyz = ((u_xlat0_d.www * u_xlat0_d.xyz) + unity_FogParams.xyz);
          out_f.color.w = 1;
          return out_f;
      }
      
      
      ENDCG
      
    } // end phase
    Pass // ind: 2, name: FORWARD
    {
      Name "FORWARD"
      Tags
      { 
        "LIGHTMODE" = "FORWARDADD"
        "RenderType" = "Opaque"
      }
      LOD 200
      ZWrite Off
      Blend One One
      // m_ProgramMask = 6
      CGPROGRAM
      //#pragma target 4.0
      
      #pragma vertex vert
      #pragma fragment frag
      
      #include "UnityCG.cginc"
      
      
      #define CODE_BLOCK_VERTEX
      uniform float4x4 unity_WorldToLight;
      uniform float4 _BumpMap_ST;
      //uniform float4x4 unity_ObjectToWorld;
      //uniform float4x4 unity_WorldToObject;
      //uniform float4 unity_WorldTransformParams;
      //uniform float4x4 unity_MatrixVP;
      uniform float4 _LightColor0;
      //uniform float3 _WorldSpaceCameraPos;
      //uniform float4 _ProjectionParams;
      //uniform float4 _WorldSpaceLightPos0;
      //uniform float4 unity_FogParams;
      uniform sampler2D _BumpMap;
      uniform sampler2D _Ramp2D;
      struct appdata_t
      {
          float4 vertex :POSITION0;
          float4 tangent :TANGENT;
          float3 normal :NORMAL;
          float4 texcoord :TEXCOORD0;
          float4 texcoord1 :TEXCOORD1;
          float4 texcoord2 :TEXCOORD2;
          float4 texcoord3 :TEXCOORD3;
          float4 color :COLOR;
      };
      
      struct OUT_Data_Vert
      {
          float4 vertex :SV_POSITION;
          float2 texcoord :TEXCOORD0;
          float2 texcoord5 :TEXCOORD5;
          float3 texcoord1 :TEXCOORD1;
          float texcoord7 :TEXCOORD7;
          float3 texcoord2 :TEXCOORD2;
          float3 texcoord3 :TEXCOORD3;
          float3 texcoord4 :TEXCOORD4;
          float4 texcoord6 :TEXCOORD6;
      };
      
      struct v2f
      {
          float4 vertex :SV_POSITION;
          float2 texcoord :TEXCOORD0;
          float2 texcoord5 :TEXCOORD5;
          float3 texcoord1 :TEXCOORD1;
          float texcoord7 :TEXCOORD7;
          float3 texcoord2 :TEXCOORD2;
          float3 texcoord3 :TEXCOORD3;
          float3 texcoord4 :TEXCOORD4;
          float4 texcoord6 :TEXCOORD6;
      };
      
      struct OUT_Data_Frag
      {
          float4 color :SV_Target;
      };
      
      float4 u_xlat0;
      float4 u_xlat1;
      float4 u_xlat2;
      OUT_Data_Vert vert(appdata_t in_v)
      {
          OUT_Data_Vert out_v;
          u_xlat0 = (in_v.vertex.yyyy * unity_WorldTransformParams);
          u_xlat0 = ((unity_WorldTransformParams * in_v.vertex.xxxx) + u_xlat0);
          u_xlat0 = ((unity_WorldTransformParams * in_v.vertex.zzzz) + u_xlat0);
          u_xlat1 = (u_xlat0 + unity_WorldTransformParams);
          u_xlat1 = mul(unity_MatrixVP, u_xlat1);
          out_v.vertex = u_xlat1;
          out_v.texcoord7.x = u_xlat1.z;
          u_xlat1 = ((unity_WorldTransformParams * in_v.vertex.wwww) + u_xlat0);
          out_v.texcoord4.xyz = ((unity_WorldTransformParams.xyz * in_v.vertex.www) + u_xlat0.xyz);
          u_xlat0.xy = (u_xlat1.yy * _BumpMap_ST.xy);
          u_xlat0.xy = ((_BumpMap_ST.xy * u_xlat1.xx) + u_xlat0.xy);
          u_xlat0.xy = ((_BumpMap_ST.xy * u_xlat1.zz) + u_xlat0.xy);
          out_v.texcoord5.xy = ((_BumpMap_ST.xy * u_xlat1.ww) + u_xlat0.xy);
          out_v.texcoord.xy = TRANSFORM_TEX(in_v.texcoord.xy, _BumpMap);
          u_xlat0.y = dot(in_v.normal.xyzx, unity_WorldTransformParams.xyzx);
          u_xlat0.z = dot(in_v.normal.xyzx, unity_WorldTransformParams.xyzx);
          u_xlat0.x = dot(in_v.normal.xyzx, unity_WorldTransformParams.xyzx);
          u_xlat0.w = dot(u_xlat0.xyzx, u_xlat0.xyzx);
          u_xlat0.w = rsqrt(u_xlat0.w);
          u_xlat0.xyz = (u_xlat0.www * u_xlat0.xyz);
          u_xlat1.xyz = (in_v.tangent.yyy * unity_WorldTransformParams.yzx);
          u_xlat1.xyz = ((unity_WorldTransformParams.yzx * in_v.tangent.xxx) + u_xlat1.xyz);
          u_xlat1.xyz = ((unity_WorldTransformParams.yzx * in_v.tangent.zzz) + u_xlat1.xyz);
          u_xlat0.w = dot(u_xlat1.xyzx, u_xlat1.xyzx);
          u_xlat0.w = rsqrt(u_xlat0.w);
          u_xlat1.xyz = (u_xlat0.www * u_xlat1.xyz);
          u_xlat2.xyz = (u_xlat0.xyz * u_xlat1.xyz);
          u_xlat2.xyz = ((u_xlat0.zxy * u_xlat1.yzx) - u_xlat2.xyz);
          u_xlat0.w = (in_v.tangent.w * unity_WorldTransformParams.w);
          u_xlat2.xyz = (u_xlat0.www * u_xlat2.xyz);
          out_v.texcoord1.y = u_xlat2.x;
          out_v.texcoord1.x = u_xlat1.z;
          out_v.texcoord1.z = u_xlat0.y;
          out_v.texcoord2.x = u_xlat1.x;
          out_v.texcoord3.x = u_xlat1.y;
          out_v.texcoord2.z = u_xlat0.z;
          out_v.texcoord3.z = u_xlat0.x;
          out_v.texcoord2.y = u_xlat2.y;
          out_v.texcoord3.y = u_xlat2.z;
          out_v.texcoord6 = float4(0, 0, 0, 0);
          return out_v;
      }
      
      #define CODE_BLOCK_FRAGMENT
      float4 u_xlat0_d;
      float4 u_xlat1_d;
      OUT_Data_Frag frag(v2f in_f)
      {
          OUT_Data_Frag out_f;
          u_xlat0_d = tex2D(_BumpMap, in_f.texcoord.xy);
          u_xlat0_d.x = (u_xlat0_d.w * u_xlat0_d.x);
          u_xlat0_d.xy = ((u_xlat0_d.xy * float2(2, 2)) + float2(-1, (-1)));
          u_xlat0_d.w = dot(u_xlat0_d.xyxx, u_xlat0_d.xyxx);
          u_xlat0_d.w = min(u_xlat0_d.w, 1);
          u_xlat0_d.w = ((-u_xlat0_d.w) + 1);
          u_xlat0_d.z = sqrt(u_xlat0_d.w);
          u_xlat1_d.x = dot(in_f.texcoord1.xyzx, u_xlat0_d.xyzx);
          u_xlat1_d.y = dot(in_f.texcoord2.xyzx, u_xlat0_d.xyzx);
          u_xlat1_d.z = dot(in_f.texcoord3.xyzx, u_xlat0_d.xyzx);
          u_xlat0_d.x = dot(u_xlat1_d.xyzx, u_xlat1_d.xyzx);
          u_xlat0_d.x = rsqrt(u_xlat0_d.x);
          u_xlat0_d.xyz = (u_xlat0_d.xxx * u_xlat1_d.xyz);
          u_xlat1_d.xyz = ((-in_f.texcoord4.xyz) + _ProjectionParams.xyz);
          u_xlat0_d.w = dot(u_xlat1_d.xyzx, u_xlat1_d.xyzx);
          u_xlat0_d.w = rsqrt(u_xlat0_d.w);
          u_xlat1_d.xyz = (u_xlat0_d.www * u_xlat1_d.xyz);
          u_xlat1_d.x = dot(u_xlat0_d.xyzx, u_xlat1_d.xyzx);
          u_xlat0_d.x = dot(u_xlat0_d.xyzx, _WorldSpaceLightPos0.xyzx);
          u_xlat1_d.y = ((u_xlat0_d.x * 0.33) + 0.58);
          u_xlat0_d = tex2D(_Ramp2D, u_xlat1_d.xy);
          u_xlat0_d.xyz = (u_xlat0_d.xyz * _LightColor0.xyz);
          u_xlat0_d.w = (in_f.texcoord7.x / _ProjectionParams.y);
          u_xlat0_d.w = ((-u_xlat0_d.w) + 1);
          u_xlat0_d.w = (u_xlat0_d.w * _ProjectionParams.z);
          u_xlat0_d.w = max(u_xlat0_d.w, 0);
          u_xlat0_d.w = (u_xlat0_d.w * unity_FogParams.y);
          u_xlat0_d.w = exp((-u_xlat0_d.w));
          u_xlat0_d.w = min(u_xlat0_d.w, 1);
          out_f.color.xyz = (u_xlat0_d.xyz * u_xlat0_d.www);
          out_f.color.w = 1;
          return out_f;
      }
      
      
      ENDCG
      
    } // end phase
  }
  FallBack "Bumped Diffuse"
}
