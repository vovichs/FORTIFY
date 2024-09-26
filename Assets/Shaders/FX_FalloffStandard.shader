Shader "FX/FalloffStandard"
{
  Properties
  {
    _ColorTint ("Color Tint", Color) = (1,1,1,1)
    _RimPower ("Rim Exponent", Range(0, 10)) = 3
  }
  SubShader
  {
    Tags
    { 
      "IGNOREPROJECTOR" = "true"
      "QUEUE" = "Transparent"
      "RenderType" = "Transparent"
    }
    Pass // ind: 1, name: FORWARD
    {
      Name "FORWARD"
      Tags
      { 
        "IGNOREPROJECTOR" = "true"
        "LIGHTMODE" = "FORWARDBASE"
        "QUEUE" = "Transparent"
        "RenderType" = "Transparent"
      }
      ZWrite Off
      Blend SrcAlpha OneMinusSrcAlpha
      ColorMask RGB
      // m_ProgramMask = 6
      CGPROGRAM
      //#pragma target 4.0
      
      #pragma vertex vert
      #pragma fragment frag
      
      #include "UnityCG.cginc"
      
      
      #define CODE_BLOCK_VERTEX
      //uniform float4 unity_4LightPosX0;
      //uniform float4 unity_4LightPosY0;
      //uniform float4 unity_4LightPosZ0;
      //uniform float4 unity_4LightAtten0;
      //uniform float4 unity_LightColor;
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
      uniform float4 _ColorTint;
      uniform float _RimPower;
      //uniform float3 _WorldSpaceCameraPos;
      //uniform float4 _ProjectionParams;
      //uniform float4 unity_FogColor;
      //uniform float4 unity_FogParams;
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
          float3 texcoord :TEXCOORD0;
          float texcoord3 :TEXCOORD3;
          float3 texcoord1 :TEXCOORD1;
          float3 texcoord2 :TEXCOORD2;
          float4 texcoord5 :TEXCOORD5;
          uint SV_InstanceID :InstanceID;
      };
      
      struct v2f
      {
          float4 vertex :SV_POSITION;
          float3 texcoord :TEXCOORD0;
          float texcoord3 :TEXCOORD3;
          float3 texcoord1 :TEXCOORD1;
          float3 texcoord2 :TEXCOORD2;
          float4 texcoord5 :TEXCOORD5;
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
          out_v.texcoord3.x = u_xlat2.z;
          u_xlat2.x = dot(in_v.normal.xyzx, conv_mxt4x4_(u_xlat0.y + (-7))(unity_WorldToObjectArray).xyzx);
          u_xlat2.y = dot(in_v.normal.xyzx, conv_mxt4x4_(u_xlat0.y + (-6))(unity_WorldToObjectArray).xyzx);
          u_xlat2.z = dot(in_v.normal.xyzx, conv_mxt4x4_(u_xlat0.y + (-5))(unity_WorldToObjectArray).xyzx);
          u_xlat0.y = dot(u_xlat2.xyzx, u_xlat2.xyzx);
          u_xlat0.y = rsqrt(u_xlat0.y);
          u_xlat2.xyz = (u_xlat0.yyy * u_xlat2.xyz);
          out_v.texcoord.xyz = u_xlat2.xyz;
          out_v.texcoord1.xyz = u_xlat1.xyz;
          u_xlat0.y = (u_xlat2.y * u_xlat2.y);
          u_xlat0.y = ((u_xlat2.x * u_xlat2.x) - u_xlat0.y);
          u_xlat3 = (u_xlat2.yzzx * u_xlat2.xyzz);
          u_xlat4.x = dot(unity_SHCArray, u_xlat3);
          u_xlat4.y = dot(unity_SHCArray, u_xlat3);
          u_xlat4.z = dot(unity_SHCArray, u_xlat3);
          u_xlat0.yzw = ((unity_SHCArray.xyz * u_xlat0.yyy) + u_xlat4.xyz);
          u_xlat2.w = 1;
          u_xlat3.x = dot(unity_SHCArray, u_xlat2);
          u_xlat3.y = dot(unity_SHCArray, u_xlat2);
          u_xlat3.z = dot(unity_SHCArray, u_xlat2);
          u_xlat0.xyz = (u_xlat0.yzw + u_xlat3.xyz);
          u_xlat0.xyz = max(u_xlat0.xyz, float3(0, 0, 0));
          u_xlat0.xyz = log(u_xlat0.xyz);
          u_xlat0.xyz = (u_xlat0.xyz * float3(0.416667, 0.416667, 0.416667));
          u_xlat0.xyz = exp(u_xlat0.xyzx);
          u_xlat0.xyz = ((u_xlat0.xyz * float3(1.055, 1.055, 1.055)) + float3(-0.055, (-0.055), (-0.055)));
          u_xlat0.xyz = max(u_xlat0.xyz, float3(0, 0, 0));
          u_xlat3 = ((-u_xlat1.yyyy) + unity_LightColor);
          u_xlat4 = (u_xlat2.yyyy * u_xlat3);
          u_xlat3 = (u_xlat3 * u_xlat3);
          u_xlat5 = ((-u_xlat1.xxxx) + unity_LightColor);
          u_xlat1 = ((-u_xlat1.zzzz) + unity_LightColor);
          u_xlat4 = ((u_xlat5 * u_xlat2.xxxx) + u_xlat4);
          u_xlat2 = ((u_xlat1 * u_xlat2.zzzz) + u_xlat4);
          u_xlat3 = ((u_xlat5 * u_xlat5) + u_xlat3);
          u_xlat1 = ((u_xlat1 * u_xlat1) + u_xlat3);
          u_xlat1 = max(u_xlat1, float4(1E-06, 1E-06, 1E-06, 1E-06));
          u_xlat3 = rsqrt(u_xlat1);
          u_xlat1 = ((u_xlat1 * unity_LightColor) + float4(1, 1, 1, 1));
          u_xlat1 = (float4(1, 1, 1, 1) / u_xlat1);
          u_xlat2 = (u_xlat2 * u_xlat3);
          u_xlat2 = max(u_xlat2, float4(0, 0, 0, 0));
          u_xlat1 = (u_xlat1 * u_xlat2);
          u_xlat2.xyz = (u_xlat1.yyy * unity_LightColor.xyz);
          u_xlat2.xyz = ((unity_LightColor.xyz * u_xlat1.xxx) + u_xlat2.xyz);
          u_xlat1.xyz = ((unity_LightColor.xyz * u_xlat1.zzz) + u_xlat2.xyz);
          u_xlat1.xyz = ((unity_LightColor.xyz * u_xlat1.www) + u_xlat1.xyz);
          out_v.texcoord2.xyz = (u_xlat0.xyz + u_xlat1.xyz);
          out_v.texcoord5 = float4(0, 0, 0, 0);
          in_v.SV_InstanceID.x = in_v.SV_InstanceID.x;
          return out_v;
      }
      
      #define CODE_BLOCK_FRAGMENT
      float4 u_xlat0_d;
      float4 u_xlat1_d;
      OUT_Data_Frag frag(v2f in_f)
      {
          OUT_Data_Frag out_f;
          u_xlat0_d.x = (in_f.texcoord3.x / _ProjectionParams.y);
          u_xlat0_d.x = ((-u_xlat0_d.x) + 1);
          u_xlat0_d.x = (u_xlat0_d.x * _ProjectionParams.z);
          u_xlat0_d.x = max(u_xlat0_d.x, 0);
          u_xlat0_d.x = (u_xlat0_d.x * unity_FogParams.y);
          u_xlat0_d.x = exp((-u_xlat0_d.x));
          u_xlat0_d.x = min(u_xlat0_d.x, 1);
          u_xlat0_d.yzw = ((-in_f.texcoord1.xyz) + _ProjectionParams.xyz);
          u_xlat1_d.x = dot(u_xlat0_d.yzwy, u_xlat0_d.yzwy);
          u_xlat1_d.x = rsqrt(u_xlat1_d.x);
          u_xlat0_d.yzw = (u_xlat0_d.yzw * u_xlat1_d.xxx);
          u_xlat0_d.y = saturate(dot(in_f.texcoord.xyzx, u_xlat0_d.yzwy));
          u_xlat0_d.y = log(u_xlat0_d.y);
          u_xlat0_d.y = (u_xlat0_d.y * _RimPower.x);
          u_xlat0_d.y = exp(u_xlat0_d.y);
          u_xlat0_d.y = ((-u_xlat0_d.y) + 1);
          u_xlat1_d.xyz = ((u_xlat0_d.yyy * _RimPower.xyz) + _RimPower.xyz);
          out_f.color.w = (u_xlat0_d.y * _RimPower.w);
          u_xlat0_d.yzw = (u_xlat1_d.xyz - unity_FogParams.xyz);
          out_f.color.xyz = ((u_xlat0_d.xxx * u_xlat0_d.yzw) + unity_FogParams.xyz);
          return out_f;
      }
      
      
      ENDCG
      
    } // end phase
    Pass // ind: 2, name: FORWARD
    {
      Name "FORWARD"
      Tags
      { 
        "IGNOREPROJECTOR" = "true"
        "LIGHTMODE" = "FORWARDADD"
        "QUEUE" = "Transparent"
        "RenderType" = "Transparent"
      }
      ZWrite Off
      Blend SrcAlpha One
      ColorMask RGB
      // m_ProgramMask = 6
      CGPROGRAM
      //#pragma target 4.0
      
      #pragma vertex vert
      #pragma fragment frag
      
      #include "UnityCG.cginc"
      #define conv_mxt4x4_0(mat4x4) float4(mat4x4[0].x,mat4x4[1].x,mat4x4[2].x,mat4x4[3].x)
      #define conv_mxt4x4_1(mat4x4) float4(mat4x4[0].y,mat4x4[1].y,mat4x4[2].y,mat4x4[3].y)
      #define conv_mxt4x4_2(mat4x4) float4(mat4x4[0].z,mat4x4[1].z,mat4x4[2].z,mat4x4[3].z)
      #define conv_mxt4x4_3(mat4x4) float4(mat4x4[0].w,mat4x4[1].w,mat4x4[2].w,mat4x4[3].w)
      
      
      #define CODE_BLOCK_VERTEX
      uniform float4x4 unity_WorldToLight;
      //uniform float4 _ProjectionParams;
      //uniform float4x4 unity_ObjectToWorld;
      //uniform float4x4 unity_WorldToObject;
      //uniform float4x4 unity_MatrixVP;
      //uniform float4 unity_FogParams;
      uniform float4 _ColorTint;
      uniform float _RimPower;
      //uniform float3 _WorldSpaceCameraPos;
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
          float3 texcoord :TEXCOORD0;
          float3 texcoord1 :TEXCOORD1;
          float2 texcoord2 :TEXCOORD2;
          float texcoord3 :TEXCOORD3;
      };
      
      struct v2f
      {
          float4 vertex :SV_POSITION;
          float3 texcoord :TEXCOORD0;
          float3 texcoord1 :TEXCOORD1;
          float2 texcoord2 :TEXCOORD2;
          float texcoord3 :TEXCOORD3;
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
          u_xlat0 = (in_v.vertex.yyyy * conv_mxt4x4_-2(unity_WorldToObject));
          u_xlat0 = ((conv_mxt4x4_-3(unity_WorldToObject) * in_v.vertex.xxxx) + u_xlat0);
          u_xlat0 = ((conv_mxt4x4_-1(unity_WorldToObject) * in_v.vertex.zzzz) + u_xlat0);
          u_xlat1 = (u_xlat0 + conv_mxt4x4_0(unity_WorldToObject));
          u_xlat1 = mul(unity_MatrixVP, u_xlat1);
          out_v.vertex = u_xlat1;
          u_xlat1.x = (u_xlat1.z / _ProjectionParams.y);
          u_xlat1.x = ((-u_xlat1.x) + 1);
          u_xlat1.x = (u_xlat1.x * _ProjectionParams.z);
          u_xlat1.x = max(u_xlat1.x, 0);
          u_xlat1.x = (u_xlat1.x * unity_FogParams.y);
          out_v.texcoord3.x = exp((-u_xlat1.x));
          u_xlat1.x = dot(in_v.normal.xyzx, conv_mxt4x4_1(unity_WorldToObject).xyzx);
          u_xlat1.y = dot(in_v.normal.xyzx, conv_mxt4x4_2(unity_WorldToObject).xyzx);
          u_xlat1.z = dot(in_v.normal.xyzx, conv_mxt4x4_3(unity_WorldToObject).xyzx);
          u_xlat1.w = dot(u_xlat1.xyzx, u_xlat1.xyzx);
          u_xlat1.w = rsqrt(u_xlat1.w);
          out_v.texcoord.xyz = (u_xlat1.www * u_xlat1.xyz);
          out_v.texcoord1.xyz = ((conv_mxt4x4_0(unity_WorldToObject).xyz * in_v.vertex.www) + u_xlat0.xyz);
          u_xlat0 = ((conv_mxt4x4_0(unity_WorldToObject) * in_v.vertex.wwww) + u_xlat0);
          u_xlat1.xy = (u_xlat0.yy * conv_mxt4x4_1(unity_WorldToLight).xy);
          u_xlat0.xy = ((conv_mxt4x4_0(unity_WorldToLight).xy * u_xlat0.xx) + u_xlat1.xy);
          u_xlat0.xy = ((conv_mxt4x4_2(unity_WorldToLight).xy * u_xlat0.zz) + u_xlat0.xy);
          out_v.texcoord2.xy = ((conv_mxt4x4_3(unity_WorldToLight).xy * u_xlat0.ww) + u_xlat0.xy);
          return out_v;
      }
      
      #define CODE_BLOCK_FRAGMENT
      float4 u_xlat0_d;
      OUT_Data_Frag frag(v2f in_f)
      {
          OUT_Data_Frag out_f;
          u_xlat0_d.xyz = ((-in_f.texcoord1.xyz) + _WorldSpaceCameraPos.xyz);
          u_xlat0_d.w = dot(u_xlat0_d.xyzx, u_xlat0_d.xyzx);
          u_xlat0_d.w = rsqrt(u_xlat0_d.w);
          u_xlat0_d.xyz = (u_xlat0_d.www * u_xlat0_d.xyz);
          u_xlat0_d.x = saturate(dot(in_f.texcoord.xyzx, u_xlat0_d.xyzx));
          u_xlat0_d.x = log(u_xlat0_d.x);
          u_xlat0_d.x = (u_xlat0_d.x * _RimPower.x);
          u_xlat0_d.x = exp(u_xlat0_d.x);
          u_xlat0_d.x = ((-u_xlat0_d.x) + 1);
          u_xlat0_d.yzw = (u_xlat0_d.xxx * _RimPower.xyz);
          out_f.color.w = (u_xlat0_d.x * _RimPower.w);
          u_xlat0_d.x = saturate(in_f.texcoord3.x);
          out_f.color.xyz = (u_xlat0_d.yzw * u_xlat0_d.xxx);
          return out_f;
      }
      
      
      ENDCG
      
    } // end phase
  }
  FallBack "Transparent"
}
