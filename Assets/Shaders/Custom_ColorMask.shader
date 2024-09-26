// Upgrade NOTE: commented out 'float4 unity_ShadowFadeCenterAndType', a built-in variable

Shader "Custom/ColorMask"
{
  Properties
  {
    _Color ("Color", Color) = (1,1,1,1)
    _MainTex ("Albedo (RGB)", 2D) = "white" {}
    _Glossiness ("Smoothness", Range(0, 1)) = 0.5
    _Metallic ("Metallic", Range(0, 1)) = 0
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
      #define conv_mxt4x4_1(mat4x4) float4(mat4x4[0].y,mat4x4[1].y,mat4x4[2].y,mat4x4[3].y)
      #define conv_mxt4x4_2(mat4x4) float4(mat4x4[0].z,mat4x4[1].z,mat4x4[2].z,mat4x4[3].z)
      #define conv_mxt4x4_3(mat4x4) float4(mat4x4[0].w,mat4x4[1].w,mat4x4[2].w,mat4x4[3].w)
      
      
      #define CODE_BLOCK_VERTEX
      uniform float4 _MainTex_ST;
      //uniform float4 _ProjectionParams;
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
      uniform float4 _LightColor0;
      uniform float _Glossiness;
      uniform float _Metallic;
      uniform float4 _Color;
      //uniform float3 _WorldSpaceCameraPos;
      //uniform float4 _WorldSpaceLightPos0;
      //uniform float4 unity_OcclusionMaskSelector;
      //uniform float4 _LightShadowData;
      // uniform float4 unity_ShadowFadeCenterAndType;
      //uniform float4x4 unity_MatrixV;
      //uniform float4 unity_FogColor;
      //uniform float4 unity_FogParams;
      //uniform float4 unity_SpecCube0_BoxMax;
      //uniform float4 unity_SpecCube0_BoxMin;
      //uniform float4 unity_SpecCube0_ProbePosition;
      //uniform float4 unity_SpecCube0_HDR;
      //uniform float4 unity_SpecCube1_BoxMax;
      //uniform float4 unity_SpecCube1_BoxMin;
      //uniform float4 unity_SpecCube1_ProbePosition;
      //uniform float4 unity_SpecCube1_HDR;
      //uniform float4x4 unity_ProbeVolumeWorldToObject;
      //uniform float4 unity_ProbeVolumeParams;
      //uniform float3 unity_ProbeVolumeSizeInv;
      //uniform float3 unity_ProbeVolumeMin;
      uniform sampler2D _MainTex;
      uniform sampler2D _ShadowMapTexture;
      //uniform sampler2D unity_SpecCube0;
      //uniform sampler2D unity_SpecCube1;
      SamplerState samplerunity_SpecCube1;
      //uniform sampler3D unity_ProbeVolumeSH;
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
          float texcoord4 :TEXCOORD4;
          float3 texcoord1 :TEXCOORD1;
          float3 texcoord2 :TEXCOORD2;
          float3 texcoord3 :TEXCOORD3;
          float4 texcoord5 :TEXCOORD5;
          float4 texcoord6 :TEXCOORD6;
          uint SV_InstanceID :InstanceID;
      };
      
      struct v2f
      {
          float4 vertex :SV_POSITION;
          float2 texcoord :TEXCOORD0;
          float texcoord4 :TEXCOORD4;
          float3 texcoord1 :TEXCOORD1;
          float3 texcoord2 :TEXCOORD2;
          float3 texcoord3 :TEXCOORD3;
          float4 texcoord5 :TEXCOORD5;
          float4 texcoord6 :TEXCOORD6;
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
          out_v.texcoord4.x = u_xlat2.z;
          out_v.texcoord5.zw = u_xlat2.zw;
          out_v.texcoord.xy = TRANSFORM_TEX(in_v.texcoord.xy, _MainTex);
          u_xlat3.x = dot(in_v.normal.xyzx, conv_mxt4x4_(u_xlat0.y + (-7))(unity_WorldToObjectArray).xyzx);
          u_xlat3.y = dot(in_v.normal.xyzx, conv_mxt4x4_(u_xlat0.y + (-6))(unity_WorldToObjectArray).xyzx);
          u_xlat3.z = dot(in_v.normal.xyzx, conv_mxt4x4_(u_xlat0.y + (-5))(unity_WorldToObjectArray).xyzx);
          u_xlat0.y = dot(u_xlat3.xyzx, u_xlat3.xyzx);
          u_xlat0.y = rsqrt(u_xlat0.y);
          u_xlat0.yzw = (u_xlat0.yyy * u_xlat3.xyz);
          out_v.texcoord1.xyz = u_xlat0.yzw;
          out_v.texcoord2.xyz = u_xlat1.xyz;
          u_xlat1.w = (u_xlat0.z * u_xlat0.z);
          u_xlat1.w = ((u_xlat0.y * u_xlat0.y) - u_xlat1.w);
          u_xlat3 = (u_xlat0.zwwy * u_xlat0.yzww);
          u_xlat4.x = dot(unity_SHCArray, u_xlat3);
          u_xlat4.y = dot(unity_SHCArray, u_xlat3);
          u_xlat4.z = dot(unity_SHCArray, u_xlat3);
          u_xlat3.xyz = ((unity_SHCArray.xyz * u_xlat1.www) + u_xlat4.xyz);
          u_xlat4 = ((-u_xlat1.yyyy) + unity_LightColor);
          u_xlat5 = (u_xlat0.zzzz * u_xlat4);
          u_xlat4 = (u_xlat4 * u_xlat4);
          u_xlat6 = ((-u_xlat1.xxxx) + unity_LightColor);
          u_xlat1 = ((-u_xlat1.zzzz) + unity_LightColor);
          u_xlat5 = ((u_xlat6 * u_xlat0.yyyy) + u_xlat5);
          u_xlat0 = ((u_xlat1 * u_xlat0.wwww) + u_xlat5);
          u_xlat4 = ((u_xlat6 * u_xlat6) + u_xlat4);
          u_xlat1 = ((u_xlat1 * u_xlat1) + u_xlat4);
          u_xlat1 = max(u_xlat1, float4(1E-06, 1E-06, 1E-06, 1E-06));
          u_xlat4 = rsqrt(u_xlat1);
          u_xlat1 = ((u_xlat1 * unity_LightColor) + float4(1, 1, 1, 1));
          u_xlat1 = (float4(1, 1, 1, 1) / u_xlat1);
          u_xlat0 = (u_xlat0 * u_xlat4);
          u_xlat0 = max(u_xlat0, float4(0, 0, 0, 0));
          u_xlat0 = (u_xlat1 * u_xlat0);
          u_xlat1.xyz = (u_xlat0.yyy * unity_LightColor.xyz);
          u_xlat1.xyz = ((unity_LightColor.xyz * u_xlat0.xxx) + u_xlat1.xyz);
          u_xlat0.xyz = ((unity_LightColor.xyz * u_xlat0.zzz) + u_xlat1.xyz);
          u_xlat0.xyz = ((unity_LightColor.xyz * u_xlat0.www) + u_xlat0.xyz);
          u_xlat1.xyz = ((u_xlat0.xyz * float3(0.305306, 0.305306, 0.305306)) + float3(0.682171, 0.682171, 0.682171));
          u_xlat1.xyz = ((u_xlat0.xyz * u_xlat1.xyz) + float3(0.012523, 0.012523, 0.012523));
          out_v.texcoord3.xyz = ((u_xlat0.xyz * u_xlat1.xyz) + u_xlat3.xyz);
          u_xlat0.x = (u_xlat2.y * _ProjectionParams.x);
          u_xlat1.xz = (u_xlat2.xw * float2(0.5, 0.5));
          u_xlat1.w = (u_xlat0.x * 0.5);
          out_v.texcoord5.xy = (u_xlat1.zz + u_xlat1.xw);
          out_v.texcoord6 = float4(0, 0, 0, 0);
          in_v.SV_InstanceID.x = in_v.SV_InstanceID.x;
          return out_v;
      }
      
      #define CODE_BLOCK_FRAGMENT
      float4 u_xlat0_d;
      float4 u_xlat1_d;
      float4 u_xlat2_d;
      float4 u_xlat3_d;
      float4 u_xlat4_d;
      float4 u_xlat5_d;
      float4 u_xlat6_d;
      float4 u_xlat7;
      float4 u_xlat8;
      float4 u_xlat9;
      float4 u_xlat10;
      float4 u_xlat11;
      OUT_Data_Frag frag(v2f in_f)
      {
          OUT_Data_Frag out_f;
          u_xlat0_d.x = (in_f.SV_InstanceID.x + unity_BaseInstanceID.x);
          u_xlat0_d.yzw = ((-in_f.texcoord2.xyz) + _ProjectionParams.xyz);
          u_xlat1_d.x = dot(u_xlat0_d.yzwy, u_xlat0_d.yzwy);
          u_xlat1_d.x = rsqrt(u_xlat1_d.x);
          u_xlat1_d.yzw = (u_xlat0_d.yzw * u_xlat1_d.xxx);
          u_xlat2_d = tex2D(_MainTex, in_f.texcoord.xy);
          u_xlat3_d.xyz = ((u_xlat2_d.xyz * _Color.xyz) - u_xlat2_d.xyz);
          u_xlat2_d.xyz = ((u_xlat2_d.www * u_xlat3_d.xyz) + u_xlat2_d.xyz);
          u_xlat3_d.x = conv_mxt4x4_1(unity_MatrixV).z;
          u_xlat3_d.y = conv_mxt4x4_2(unity_MatrixV).z;
          u_xlat3_d.z = conv_mxt4x4_3(unity_MatrixV).z;
          u_xlat2_d.w = dot(u_xlat0_d.yzwy, u_xlat3_d.xyzx);
          u_xlat3_d.xyz = (in_f.texcoord2.xyz - unity_ShadowFadeCenterAndType.xyz);
          u_xlat3_d.x = length(u_xlat3_d.xyzx);
          u_xlat3_d.x = ((-u_xlat2_d.w) + u_xlat3_d.x);
          u_xlat2_d.w = ((unity_ShadowFadeCenterAndType.w * u_xlat3_d.x) + u_xlat2_d.w);
          u_xlat2_d.w = saturate(((u_xlat2_d.w * unity_ShadowFadeCenterAndType.z) + unity_ShadowFadeCenterAndType.w));
          u_xlat0_d.x = ((u_xlat0_d.x * 7) & -1);
          u_xlat3_d.x = (unity_ProbeVolumeMin.x==1);
          if((u_xlat3_d.x!=0))
          {
              u_xlat3_d.y = (unity_ProbeVolumeMin.y==1);
              u_xlat4_d.xyz = (in_f.texcoord2.yyy * unity_ProbeVolumeMin.xyz);
              u_xlat4_d.xyz = ((unity_ProbeVolumeMin.xyz * in_f.texcoord2.xxx) + u_xlat4_d.xyz);
              u_xlat4_d.xyz = ((unity_ProbeVolumeMin.xyz * in_f.texcoord2.zzz) + u_xlat4_d.xyz);
              u_xlat4_d.xyz = (u_xlat4_d.xyz + unity_ProbeVolumeMin.xyz);
              u_xlat3_d.yzw = (u_xlat3_d.yyy)?(u_xlat4_d.xyz):(in_f.texcoord2.xyz);
              u_xlat3_d.yzw = (u_xlat3_d.yzw - unity_ProbeVolumeMin.xyz);
              u_xlat4_d.yzw = (u_xlat3_d.yzw * unity_ProbeVolumeMin.xyz);
              u_xlat3_d.y = ((u_xlat4_d.y * 0.25) + 0.75);
              u_xlat3_d.z = ((unity_ProbeVolumeMin.z * 0.5) + 0.75);
              u_xlat4_d.x = max(u_xlat3_d.z, u_xlat3_d.y);
              u_xlat4_d = tex3D(unity_ProbeVolumeSH, u_xlat4_d.xz);
          }
          else
          {
              u_xlat4_d = unity_SHCArray.wwww;
          }
          u_xlat3_d.y = saturate(dot(u_xlat4_d, unity_OcclusionMaskSelector));
          u_xlat3_d.zw = (in_f.texcoord5.xy / in_f.texcoord5.ww);
          u_xlat4_d = tex2D(_ShadowMapTexture, u_xlat3_d.zw);
          u_xlat3_d.y = (u_xlat3_d.y - u_xlat4_d.x);
          u_xlat2_d.w = ((u_xlat2_d.w * u_xlat3_d.y) + u_xlat4_d.x);
          u_xlat3_d.y = ((-_Color.x) + 1);
          u_xlat3_d.z = dot((-u_xlat1_d.yzwy), in_f.texcoord1.xyzx);
          u_xlat3_d.z = (u_xlat3_d.z + u_xlat3_d.z);
          u_xlat4_d.xyz = ((in_f.texcoord1.xyz * (-u_xlat3_d.zzz)) - u_xlat1_d.yzw);
          u_xlat5_d.xyz = (u_xlat2_d.www * _Color.xyz);
          if((u_xlat3_d.x!=0))
          {
              u_xlat2_d.w = (unity_ProbeVolumeMin.y==1);
              u_xlat3_d.xzw = (in_f.texcoord2.yyy * unity_ProbeVolumeMin.xyz);
              u_xlat3_d.xzw = ((unity_ProbeVolumeMin.xyz * in_f.texcoord2.xxx) + u_xlat3_d.xzw);
              u_xlat3_d.xzw = ((unity_ProbeVolumeMin.xyz * in_f.texcoord2.zzz) + u_xlat3_d.xzw);
              u_xlat3_d.xzw = (u_xlat3_d.xzw + unity_ProbeVolumeMin.xyz);
              u_xlat3_d.xzw = (u_xlat2_d.www)?(u_xlat3_d.xzw):(in_f.texcoord2.xyz);
              u_xlat3_d.xzw = (u_xlat3_d.xzw - unity_ProbeVolumeMin.xyz);
              u_xlat6_d.yzw = (u_xlat3_d.xzw * unity_ProbeVolumeMin.xyz);
              u_xlat2_d.w = (u_xlat6_d.y * 0.25);
              u_xlat3_d.x = (unity_ProbeVolumeMin.z * 0.5);
              u_xlat3_d.z = (((-unity_ProbeVolumeMin.z) * 0.5) + 0.25);
              u_xlat2_d.w = max(u_xlat2_d.w, u_xlat3_d.x);
              u_xlat6_d.x = min(u_xlat3_d.z, u_xlat2_d.w);
              u_xlat7 = tex3D(unity_ProbeVolumeSH, u_xlat6_d.xz);
              u_xlat3_d.xzw = (u_xlat6_d.xzw + float3(0.25, 0, 0));
              u_xlat8 = tex3D(unity_ProbeVolumeSH, u_xlat3_d.xz);
              u_xlat3_d.xzw = (u_xlat6_d.xzw + float3(0.5, 0, 0));
              u_xlat6_d = tex3D(unity_ProbeVolumeSH, u_xlat3_d.xz);
              u_xlat9.xyz = in_f.texcoord1.xyz;
              u_xlat9.w = 1;
              u_xlat7.x = dot(u_xlat7, u_xlat9);
              u_xlat7.y = dot(u_xlat8, u_xlat9);
              u_xlat7.z = dot(u_xlat6_d, u_xlat9);
          }
          else
          {
              u_xlat6_d.xyz = in_f.texcoord1.xyz;
              u_xlat6_d.w = 1;
              u_xlat7.x = dot(unity_SHCArray, u_xlat6_d);
              u_xlat7.y = dot(unity_SHCArray, u_xlat6_d);
              u_xlat7.z = dot(unity_SHCArray, u_xlat6_d);
          }
          u_xlat3_d.xzw = (u_xlat7.xyz + in_f.texcoord3.xyz);
          u_xlat3_d.xzw = max(u_xlat3_d.xzw, float3(0, 0, 0));
          u_xlat3_d.xzw = log(u_xlat3_d.xzw);
          u_xlat3_d.xzw = (u_xlat3_d.xzw * float3(0.416667, 0.416667, 0.416667));
          u_xlat3_d.xzw = exp(u_xlat3_d.xxzw);
          u_xlat3_d.xzw = ((u_xlat3_d.xzw * float3(1.055, 1.055, 1.055)) + float3(-0.055, (-0.055), (-0.055)));
          u_xlat3_d.xzw = max(u_xlat3_d.xzw, float3(0, 0, 0));
          u_xlat0_d.x = (0<unity_SpecCube1_HDR.w);
          if((u_xlat0_d.x!=0))
          {
              u_xlat0_d.x = dot(u_xlat4_d.xyzx, u_xlat4_d.xyzx);
              u_xlat0_d.x = rsqrt(u_xlat0_d.x);
              u_xlat6_d.xyz = (u_xlat0_d.xxx * u_xlat4_d.xyz);
              u_xlat7.xyz = ((-in_f.texcoord2.xyz) + unity_SpecCube1_HDR.xyz);
              u_xlat7.xyz = (u_xlat7.xyz / u_xlat6_d.xyz);
              u_xlat8.xyz = ((-in_f.texcoord2.xyz) + unity_SpecCube1_HDR.xyz);
              u_xlat8.xyz = (u_xlat8.xyz / u_xlat6_d.xyz);
              u_xlat9.xyz = (float3(0, 0, 0)<u_xlat6_d.xyz);
              u_xlat7.xyz = (u_xlat9.xyz)?(u_xlat7.xyz):(u_xlat8.xyz);
              u_xlat0_d.x = min(u_xlat7.y, u_xlat7.x);
              u_xlat0_d.x = min(u_xlat7.z, u_xlat0_d.x);
              u_xlat7.xyz = (in_f.texcoord2.xyz - unity_SpecCube1_HDR.xyz);
              u_xlat6_d.xyz = ((u_xlat6_d.xyz * u_xlat0_d.xxx) + u_xlat7.xyz);
          }
          else
          {
              u_xlat6_d.xyz = u_xlat4_d.xyz;
          }
          u_xlat0_d.x = (((-u_xlat3_d.y) * 0.7) + 1.7);
          u_xlat0_d.x = (u_xlat0_d.x * u_xlat3_d.y);
          u_xlat0_d.x = (u_xlat0_d.x * 6);
          u_xlat6_d = tex2D(unity_SpecCube0, u_xlat6_d.xy);
          u_xlat2_d.w = (u_xlat6_d.w - 1);
          u_xlat2_d.w = ((unity_SpecCube1_HDR.w * u_xlat2_d.w) + 1);
          u_xlat2_d.w = (u_xlat2_d.w * unity_SpecCube1_HDR.x);
          u_xlat7.xyz = (u_xlat6_d.xyz * u_xlat2_d.www);
          u_xlat4_d.w = (unity_SpecCube1_HDR.w<0.99999);
          if((u_xlat4_d.w!=0))
          {
              u_xlat4_d.w = (0<unity_SpecCube1_HDR.w);
              if((u_xlat4_d.w!=0))
              {
                  u_xlat4_d.w = dot(u_xlat4_d.xyzx, u_xlat4_d.xyzx);
                  u_xlat4_d.w = rsqrt(u_xlat4_d.w);
                  u_xlat8.xyz = (u_xlat4_d.www * u_xlat4_d.xyz);
                  u_xlat9.xyz = ((-in_f.texcoord2.xyz) + unity_SpecCube1_HDR.xyz);
                  u_xlat9.xyz = (u_xlat9.xyz / u_xlat8.xyz);
                  u_xlat10.xyz = ((-in_f.texcoord2.xyz) + unity_SpecCube1_HDR.xyz);
                  u_xlat10.xyz = (u_xlat10.xyz / u_xlat8.xyz);
                  u_xlat11.xyz = (float3(0, 0, 0)<u_xlat8.xyz);
                  u_xlat9.xyz = (u_xlat11.xyz)?(u_xlat9.xyz):(u_xlat10.xyz);
                  u_xlat4_d.w = min(u_xlat9.y, u_xlat9.x);
                  u_xlat4_d.w = min(u_xlat9.z, u_xlat4_d.w);
                  u_xlat9.xyz = (in_f.texcoord2.xyz - unity_SpecCube1_HDR.xyz);
                  u_xlat4_d.xyz = ((u_xlat8.xyz * u_xlat4_d.www) + u_xlat9.xyz);
              }
              u_xlat4_d = tex2D(unity_SpecCube1, u_xlat4_d.xy);
              u_xlat0_d.x = (u_xlat4_d.w - 1);
              u_xlat0_d.x = ((unity_SpecCube1_HDR.w * u_xlat0_d.x) + 1);
              u_xlat0_d.x = (u_xlat0_d.x * unity_SpecCube1_HDR.x);
              u_xlat4_d.xyz = (u_xlat4_d.xyz * u_xlat0_d.xxx);
              u_xlat6_d.xyz = ((u_xlat2_d.www * u_xlat6_d.xyz) - u_xlat4_d.xyz);
              u_xlat7.xyz = ((unity_SpecCube1_HDR.www * u_xlat6_d.xyz) + u_xlat4_d.xyz);
          }
          u_xlat0_d.x = dot(in_f.texcoord1.xyzx, in_f.texcoord1.xyzx);
          u_xlat0_d.x = rsqrt(u_xlat0_d.x);
          u_xlat4_d.xyz = (u_xlat0_d.xxx * in_f.texcoord1.xyz);
          u_xlat6_d.xyz = (u_xlat2_d.xyz + float3(-0.220916, (-0.220916), (-0.220916)));
          u_xlat6_d.xyz = ((_Color.yyy * u_xlat6_d.xyz) + float3(0.220916, 0.220916, 0.220916));
          u_xlat0_d.x = (((-_Color.y) * 0.779084) + 0.779084);
          u_xlat2_d.xyz = (u_xlat0_d.xxx * u_xlat2_d.xyz);
          u_xlat0_d.yzw = ((u_xlat0_d.yzw * u_xlat1_d.xxx) + unity_OcclusionMaskSelector.xyz);
          u_xlat1_d.x = dot(u_xlat0_d.yzwy, u_xlat0_d.yzwy);
          u_xlat1_d.x = max(u_xlat1_d.x, 0.001);
          u_xlat1_d.x = rsqrt(u_xlat1_d.x);
          u_xlat0_d.yzw = (u_xlat0_d.yzw * u_xlat1_d.xxx);
          u_xlat1_d.x = dot(u_xlat4_d.xyzx, u_xlat1_d.yzwy);
          u_xlat1_d.y = saturate(dot(u_xlat4_d.xyzx, unity_OcclusionMaskSelector.xyzx));
          u_xlat1_d.z = saturate(dot(u_xlat4_d.xyzx, u_xlat0_d.yzwy));
          u_xlat0_d.y = saturate(dot(unity_OcclusionMaskSelector.xyzx, u_xlat0_d.yzwy));
          u_xlat0_d.z = (u_xlat0_d.y * u_xlat0_d.y);
          u_xlat0_d.z = dot(u_xlat0_d.zzzz, u_xlat3_d.yyyy);
          u_xlat0_d.z = (u_xlat0_d.z - 0.5);
          u_xlat0_d.w = ((-u_xlat1_d.y) + 1);
          u_xlat1_d.w = (u_xlat0_d.w * u_xlat0_d.w);
          u_xlat1_d.w = (u_xlat1_d.w * u_xlat1_d.w);
          u_xlat0_d.w = (u_xlat0_d.w * u_xlat1_d.w);
          u_xlat0_d.w = ((u_xlat0_d.z * u_xlat0_d.w) + 1);
          u_xlat1_d.w = ((-abs(u_xlat1_d.x)) + 1);
          u_xlat2_d.w = (u_xlat1_d.w * u_xlat1_d.w);
          u_xlat2_d.w = (u_xlat2_d.w * u_xlat2_d.w);
          u_xlat1_d.w = (u_xlat1_d.w * u_xlat2_d.w);
          u_xlat0_d.z = ((u_xlat0_d.z * u_xlat1_d.w) + 1);
          u_xlat0_d.z = (u_xlat0_d.z * u_xlat0_d.w);
          u_xlat0_d.z = (u_xlat1_d.y * u_xlat0_d.z);
          u_xlat0_d.w = (u_xlat3_d.y * u_xlat3_d.y);
          u_xlat0_d.w = max(u_xlat0_d.w, 0.002);
          u_xlat2_d.w = ((-u_xlat0_d.w) + 1);
          u_xlat4_d.x = ((abs(u_xlat1_d.x) * u_xlat2_d.w) + u_xlat0_d.w);
          u_xlat2_d.w = ((u_xlat1_d.y * u_xlat2_d.w) + u_xlat0_d.w);
          u_xlat1_d.x = (abs(u_xlat1_d.x) * u_xlat2_d.w);
          u_xlat1_d.x = ((u_xlat1_d.y * u_xlat4_d.x) + u_xlat1_d.x);
          u_xlat1_d.x = (u_xlat1_d.x + 1E-05);
          u_xlat1_d.x = (0.5 / u_xlat1_d.x);
          u_xlat2_d.w = (u_xlat0_d.w * u_xlat0_d.w);
          u_xlat4_d.x = ((u_xlat1_d.z * u_xlat2_d.w) - u_xlat1_d.z);
          u_xlat1_d.z = ((u_xlat4_d.x * u_xlat1_d.z) + 1);
          u_xlat2_d.w = (u_xlat2_d.w * 0.31831);
          u_xlat1_d.z = ((u_xlat1_d.z * u_xlat1_d.z) + 0);
          u_xlat1_d.z = (u_xlat2_d.w / u_xlat1_d.z);
          u_xlat1_d.x = (u_xlat1_d.z * u_xlat1_d.x);
          u_xlat1_d.x = (u_xlat1_d.x * 3.141593);
          u_xlat1_d.x = max(u_xlat1_d.x, 0.0001);
          u_xlat1_d.x = sqrt(u_xlat1_d.x);
          u_xlat1_d.x = (u_xlat1_d.y * u_xlat1_d.x);
          u_xlat0_d.w = (u_xlat0_d.w * 0.28);
          u_xlat0_d.w = (((-u_xlat0_d.w) * u_xlat3_d.y) + 1);
          u_xlat1_d.y = dot(u_xlat6_d.xyzx, u_xlat6_d.xyzx);
          u_xlat1_d.y = (u_xlat1_d.y!=0);
          u_xlat1_d.y = u_xlat1_d.y;
          u_xlat1_d.x = (u_xlat1_d.y * u_xlat1_d.x);
          u_xlat0_d.x = ((-u_xlat0_d.x) + _Color.x);
          u_xlat0_d.x = saturate((u_xlat0_d.x + 1));
          u_xlat3_d.xyz = ((u_xlat5_d.xyz * u_xlat0_d.zzz) + u_xlat3_d.xzw);
          u_xlat1_d.xyz = (u_xlat5_d.xyz * u_xlat1_d.xxx);
          u_xlat0_d.y = ((-u_xlat0_d.y) + 1);
          u_xlat0_d.z = (u_xlat0_d.y * u_xlat0_d.y);
          u_xlat0_d.z = (u_xlat0_d.z * u_xlat0_d.z);
          u_xlat0_d.y = (u_xlat0_d.y * u_xlat0_d.z);
          u_xlat4_d.xyz = ((-u_xlat6_d.xyz) + float3(1, 1, 1));
          u_xlat4_d.xyz = ((u_xlat4_d.xyz * u_xlat0_d.yyy) + u_xlat6_d.xyz);
          u_xlat1_d.xyz = (u_xlat1_d.xyz * u_xlat4_d.xyz);
          u_xlat1_d.xyz = ((u_xlat2_d.xyz * u_xlat3_d.xyz) + u_xlat1_d.xyz);
          u_xlat0_d.yzw = (u_xlat7.xyz * u_xlat0_d.www);
          u_xlat2_d.xyz = ((-u_xlat6_d.xyz) + u_xlat0_d.xxx);
          u_xlat2_d.xyz = ((u_xlat1_d.www * u_xlat2_d.xyz) + u_xlat6_d.xyz);
          u_xlat0_d.xyz = ((u_xlat0_d.yzw * u_xlat2_d.xyz) + u_xlat1_d.xyz);
          u_xlat0_d.w = (in_f.texcoord4.x / _ProjectionParams.y);
          u_xlat0_d.w = ((-u_xlat0_d.w) + 1);
          u_xlat0_d.w = (u_xlat0_d.w * _ProjectionParams.z);
          u_xlat0_d.w = max(u_xlat0_d.w, 0);
          u_xlat0_d.w = (u_xlat0_d.w * unity_FogParams.y);
          u_xlat0_d.w = exp((-u_xlat0_d.w));
          u_xlat0_d.w = min(u_xlat0_d.w, 1);
          u_xlat0_d.xyz = (u_xlat0_d.xyz - unity_FogParams.xyz);
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
        "SHADOWSUPPORT" = "true"
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
      #define conv_mxt4x4_0(mat4x4) float4(mat4x4[0].x,mat4x4[1].x,mat4x4[2].x,mat4x4[3].x)
      #define conv_mxt4x4_1(mat4x4) float4(mat4x4[0].y,mat4x4[1].y,mat4x4[2].y,mat4x4[3].y)
      #define conv_mxt4x4_2(mat4x4) float4(mat4x4[0].z,mat4x4[1].z,mat4x4[2].z,mat4x4[3].z)
      #define conv_mxt4x4_3(mat4x4) float4(mat4x4[0].w,mat4x4[1].w,mat4x4[2].w,mat4x4[3].w)
      
      
      #define CODE_BLOCK_VERTEX
      uniform float4x4 unity_WorldToLight;
      uniform float4 _MainTex_ST;
      //uniform float4x4 unity_ObjectToWorld;
      //uniform float4x4 unity_WorldToObject;
      //uniform float4x4 unity_MatrixVP;
      uniform float4 _LightColor0;
      uniform float _Glossiness;
      uniform float _Metallic;
      uniform float4 _Color;
      //uniform float3 _WorldSpaceCameraPos;
      //uniform float4 _ProjectionParams;
      //uniform float4 _WorldSpaceLightPos0;
      //uniform float4 _LightPositionRange;
      uniform float4 _LightProjectionParams;
      //uniform float4 unity_OcclusionMaskSelector;
      //uniform float4 _LightShadowData;
      // uniform float4 unity_ShadowFadeCenterAndType;
      //uniform float4x4 unity_MatrixV;
      //uniform float4 unity_FogParams;
      //uniform float4x4 unity_ProbeVolumeWorldToObject;
      //uniform float4 unity_ProbeVolumeParams;
      //uniform float3 unity_ProbeVolumeSizeInv;
      //uniform float3 unity_ProbeVolumeMin;
      uniform sampler2D _MainTex;
      uniform sampler2D _LightTextureB0;
      uniform sampler2D _LightTexture0;
      //uniform sampler3D unity_ProbeVolumeSH;
      uniform sampler2D _ShadowMapTexture;
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
          float texcoord5 :TEXCOORD5;
          float3 texcoord1 :TEXCOORD1;
          float3 texcoord2 :TEXCOORD2;
          float3 texcoord3 :TEXCOORD3;
          float4 texcoord4 :TEXCOORD4;
      };
      
      struct v2f
      {
          float4 vertex :SV_POSITION;
          float2 texcoord :TEXCOORD0;
          float texcoord5 :TEXCOORD5;
          float3 texcoord1 :TEXCOORD1;
          float3 texcoord2 :TEXCOORD2;
          float3 texcoord3 :TEXCOORD3;
          float4 texcoord4 :TEXCOORD4;
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
          out_v.texcoord5.x = u_xlat1.z;
          out_v.texcoord.xy = TRANSFORM_TEX(in_v.texcoord.xy, _MainTex);
          u_xlat1.x = dot(in_v.normal.xyzx, conv_mxt4x4_1(unity_WorldToObject).xyzx);
          u_xlat1.y = dot(in_v.normal.xyzx, conv_mxt4x4_2(unity_WorldToObject).xyzx);
          u_xlat1.z = dot(in_v.normal.xyzx, conv_mxt4x4_3(unity_WorldToObject).xyzx);
          u_xlat1.w = dot(u_xlat1.xyzx, u_xlat1.xyzx);
          u_xlat1.w = rsqrt(u_xlat1.w);
          out_v.texcoord1.xyz = (u_xlat1.www * u_xlat1.xyz);
          out_v.texcoord2.xyz = ((conv_mxt4x4_0(unity_WorldToObject).xyz * in_v.vertex.www) + u_xlat0.xyz);
          u_xlat0 = ((conv_mxt4x4_0(unity_WorldToObject) * in_v.vertex.wwww) + u_xlat0);
          u_xlat1.xyz = (u_xlat0.yyy * _MainTex_ST.xyz);
          u_xlat1.xyz = ((_MainTex_ST.xyz * u_xlat0.xxx) + u_xlat1.xyz);
          u_xlat0.xyz = ((_MainTex_ST.xyz * u_xlat0.zzz) + u_xlat1.xyz);
          out_v.texcoord3.xyz = ((_MainTex_ST.xyz * u_xlat0.www) + u_xlat0.xyz);
          out_v.texcoord4 = float4(0, 0, 0, 0);
          return out_v;
      }
      
      #define CODE_BLOCK_FRAGMENT
      float4 u_xlat0_d;
      float4 u_xlat1_d;
      float4 u_xlat2_d;
      float4 u_xlat3;
      float4 u_xlat4;
      float4 u_xlat5;
      float4 u_xlat6;
      float4 u_xlat7;
      OUT_Data_Frag frag(v2f in_f)
      {
          OUT_Data_Frag out_f;
          u_xlat0_d.xyz = ((-in_f.texcoord2.xyz) + unity_OcclusionMaskSelector.xyz);
          u_xlat0_d.w = dot(u_xlat0_d.xyzx, u_xlat0_d.xyzx);
          u_xlat0_d.w = rsqrt(u_xlat0_d.w);
          u_xlat1_d.xyz = (u_xlat0_d.www * u_xlat0_d.xyz);
          u_xlat2_d.xyz = ((-in_f.texcoord2.xyz) + _ProjectionParams.xyz);
          u_xlat1_d.w = dot(u_xlat2_d.xyzx, u_xlat2_d.xyzx);
          u_xlat1_d.w = rsqrt(u_xlat1_d.w);
          u_xlat3.xyz = (u_xlat1_d.www * u_xlat2_d.xyz);
          u_xlat4 = tex2D(_MainTex, in_f.texcoord.xy);
          u_xlat5.xyz = ((u_xlat4.xyz * _Color.xyz) - u_xlat4.xyz);
          u_xlat4.xyz = ((u_xlat4.www * u_xlat5.xyz) + u_xlat4.xyz);
          u_xlat5.xyz = (in_f.texcoord2.yyy * _Color.xyz);
          u_xlat5.xyz = ((_Color.xyz * in_f.texcoord2.xxx) + u_xlat5.xyz);
          u_xlat5.xyz = ((_Color.xyz * in_f.texcoord2.zzz) + u_xlat5.xyz);
          u_xlat5.xyz = (u_xlat5.xyz + _Color.xyz);
          u_xlat6.x = conv_mxt4x4_1(unity_MatrixV).z;
          u_xlat6.y = conv_mxt4x4_2(unity_MatrixV).z;
          u_xlat6.z = conv_mxt4x4_3(unity_MatrixV).z;
          u_xlat1_d.w = dot(u_xlat2_d.xyzx, u_xlat6.xyzx);
          u_xlat2_d.xyz = (in_f.texcoord2.xyz - unity_ShadowFadeCenterAndType.xyz);
          u_xlat2_d.x = length(u_xlat2_d.xyzx);
          u_xlat2_d.x = ((-u_xlat1_d.w) + u_xlat2_d.x);
          u_xlat1_d.w = ((unity_ShadowFadeCenterAndType.w * u_xlat2_d.x) + u_xlat1_d.w);
          u_xlat1_d.w = saturate(((u_xlat1_d.w * unity_ShadowFadeCenterAndType.z) + unity_ShadowFadeCenterAndType.w));
          u_xlat2_d.x = (unity_ProbeVolumeMin.x==1);
          if((u_xlat2_d.x!=0))
          {
              u_xlat2_d.x = (unity_ProbeVolumeMin.y==1);
              u_xlat2_d.yzw = (in_f.texcoord2.yyy * unity_ProbeVolumeMin.xyz);
              u_xlat2_d.yzw = ((unity_ProbeVolumeMin.xyz * in_f.texcoord2.xxx) + u_xlat2_d.yzw);
              u_xlat2_d.yzw = ((unity_ProbeVolumeMin.xyz * in_f.texcoord2.zzz) + u_xlat2_d.yzw);
              u_xlat2_d.yzw = (u_xlat2_d.yzw + unity_ProbeVolumeMin.xyz);
              u_xlat2_d.xyz = (u_xlat2_d.xxx)?(u_xlat2_d.yzw):(in_f.texcoord2.xyz);
              u_xlat2_d.xyz = (u_xlat2_d.xyz - unity_ProbeVolumeMin.xyz);
              u_xlat2_d.yzw = (u_xlat2_d.xyz * unity_ProbeVolumeMin.xyz);
              u_xlat2_d.y = ((u_xlat2_d.y * 0.25) + 0.75);
              u_xlat3.w = ((unity_ProbeVolumeMin.z * 0.5) + 0.75);
              u_xlat2_d.x = max(u_xlat2_d.y, u_xlat3.w);
              u_xlat2_d = tex3D(unity_ProbeVolumeSH, u_xlat2_d.xz);
          }
          else
          {
              u_xlat2_d = float4(1, 1, 1, 1);
          }
          u_xlat2_d.x = saturate(dot(u_xlat2_d, unity_OcclusionMaskSelector));
          u_xlat2_d.y = (u_xlat1_d.w<0.99);
          if((u_xlat2_d.y!=0))
          {
              u_xlat2_d.yzw = (in_f.texcoord2.xyz - unity_OcclusionMaskSelector.xyz);
              u_xlat3.w = max(abs(u_xlat2_d.z), abs(u_xlat2_d.y));
              u_xlat3.w = max(abs(u_xlat2_d.w), u_xlat3.w);
              u_xlat3.w = (u_xlat3.w - unity_OcclusionMaskSelector.z);
              u_xlat3.w = max(u_xlat3.w, 1E-05);
              u_xlat3.w = (u_xlat3.w * unity_OcclusionMaskSelector.w);
              u_xlat3.w = (unity_OcclusionMaskSelector.y / u_xlat3.w);
              u_xlat3.w = (u_xlat3.w - unity_OcclusionMaskSelector.x);
              u_xlat3.w = ((-u_xlat3.w) + 1);
              u_xlat6.xyz = (u_xlat2_d.yzw + float3(0.007812, 0.007812, 0.007812));
              u_xlat6.x = tex2D(_ShadowMapTexture, u_xlat6.xy).x;
              u_xlat7.xyz = (u_xlat2_d.yzw + float3(-0.007812, (-0.007812), 0.007812));
              u_xlat6.y = tex2D(_ShadowMapTexture, u_xlat7.xy).x;
              u_xlat7.xyz = (u_xlat2_d.yzw + float3(-0.007812, 0.007812, (-0.007812)));
              u_xlat6.z = tex2D(_ShadowMapTexture, u_xlat7.xy).x;
              u_xlat2_d.yzw = (u_xlat2_d.yzw + float3(0.007812, (-0.007812), (-0.007812)));
              u_xlat6.w = tex2D(_ShadowMapTexture, u_xlat2_d.yz).x;
              u_xlat2_d.y = dot(u_xlat6, float4(0.25, 0.25, 0.25, 0.25));
              u_xlat2_d.z = ((-unity_ShadowFadeCenterAndType.x) + 1);
              u_xlat2_d.y = ((u_xlat2_d.y * u_xlat2_d.z) + unity_ShadowFadeCenterAndType.x);
          }
          else
          {
              u_xlat2_d.y = 1;
          }
          u_xlat2_d.x = ((-u_xlat2_d.y) + u_xlat2_d.x);
          u_xlat1_d.w = ((u_xlat1_d.w * u_xlat2_d.x) + u_xlat2_d.y);
          u_xlat2_d.x = dot(u_xlat5.xyzx, u_xlat5.xyzx);
          u_xlat2_d = tex2D(_LightTextureB0, u_xlat2_d.xx);
          u_xlat5 = tex2D(_LightTexture0, u_xlat5.xy);
          u_xlat2_d.x = (u_xlat2_d.x * u_xlat5.w);
          u_xlat1_d.w = (u_xlat1_d.w * u_xlat2_d.x);
          u_xlat2_d.xyz = (u_xlat1_d.www * _Color.xyz);
          u_xlat1_d.w = dot(in_f.texcoord1.xyzx, in_f.texcoord1.xyzx);
          u_xlat1_d.w = rsqrt(u_xlat1_d.w);
          u_xlat5.xyz = (u_xlat1_d.www * in_f.texcoord1.xyz);
          u_xlat6.xyz = (u_xlat4.xyz + float3(-0.220916, (-0.220916), (-0.220916)));
          u_xlat6.xyz = ((_Color.yyy * u_xlat6.xyz) + float3(0.220916, 0.220916, 0.220916));
          u_xlat1_d.w = (((-_Color.y) * 0.779084) + 0.779084);
          u_xlat4.xyz = (u_xlat1_d.www * u_xlat4.xyz);
          u_xlat1_d.w = ((-_Color.x) + 1);
          u_xlat0_d.xyz = ((u_xlat0_d.xyz * u_xlat0_d.www) + u_xlat3.xyz);
          u_xlat0_d.w = dot(u_xlat0_d.xyzx, u_xlat0_d.xyzx);
          u_xlat0_d.w = max(u_xlat0_d.w, 0.001);
          u_xlat0_d.w = rsqrt(u_xlat0_d.w);
          u_xlat0_d.xyz = (u_xlat0_d.www * u_xlat0_d.xyz);
          u_xlat0_d.w = dot(u_xlat5.xyzx, u_xlat3.xyzx);
          u_xlat2_d.w = saturate(dot(u_xlat5.xyzx, u_xlat1_d.xyzx));
          u_xlat3.x = saturate(dot(u_xlat5.xyzx, u_xlat0_d.xyzx));
          u_xlat0_d.x = saturate(dot(u_xlat1_d.xyzx, u_xlat0_d.xyzx));
          u_xlat0_d.y = (u_xlat0_d.x * u_xlat0_d.x);
          u_xlat0_d.y = dot(u_xlat0_d.yyyy, u_xlat1_d.wwww);
          u_xlat0_d.y = (u_xlat0_d.y - 0.5);
          u_xlat0_d.z = ((-u_xlat2_d.w) + 1);
          u_xlat1_d.x = (u_xlat0_d.z * u_xlat0_d.z);
          u_xlat1_d.x = (u_xlat1_d.x * u_xlat1_d.x);
          u_xlat0_d.z = (u_xlat0_d.z * u_xlat1_d.x);
          u_xlat0_d.z = ((u_xlat0_d.y * u_xlat0_d.z) + 1);
          u_xlat1_d.x = ((-abs(u_xlat0_d.w)) + 1);
          u_xlat1_d.y = (u_xlat1_d.x * u_xlat1_d.x);
          u_xlat1_d.y = (u_xlat1_d.y * u_xlat1_d.y);
          u_xlat1_d.x = (u_xlat1_d.x * u_xlat1_d.y);
          u_xlat0_d.y = ((u_xlat0_d.y * u_xlat1_d.x) + 1);
          u_xlat0_d.y = (u_xlat0_d.y * u_xlat0_d.z);
          u_xlat0_d.z = (u_xlat1_d.w * u_xlat1_d.w);
          u_xlat0_d.z = max(u_xlat0_d.z, 0.002);
          u_xlat1_d.x = ((-u_xlat0_d.z) + 1);
          u_xlat1_d.y = ((abs(u_xlat0_d.w) * u_xlat1_d.x) + u_xlat0_d.z);
          u_xlat1_d.x = ((u_xlat2_d.w * u_xlat1_d.x) + u_xlat0_d.z);
          u_xlat0_d.w = (abs(u_xlat0_d.w) * u_xlat1_d.x);
          u_xlat0_d.w = ((u_xlat2_d.w * u_xlat1_d.y) + u_xlat0_d.w);
          u_xlat0_d.w = (u_xlat0_d.w + 1E-05);
          u_xlat0_d.w = (0.5 / u_xlat0_d.w);
          u_xlat0_d.z = (u_xlat0_d.z * u_xlat0_d.z);
          u_xlat1_d.x = ((u_xlat3.x * u_xlat0_d.z) - u_xlat3.x);
          u_xlat1_d.x = ((u_xlat1_d.x * u_xlat3.x) + 1);
          u_xlat0_d.z = (u_xlat0_d.z * 0.31831);
          u_xlat1_d.x = ((u_xlat1_d.x * u_xlat1_d.x) + 0);
          u_xlat0_d.z = (u_xlat0_d.z / u_xlat1_d.x);
          u_xlat0_d.z = (u_xlat0_d.z * u_xlat0_d.w);
          u_xlat0_d.z = (u_xlat0_d.z * 3.141593);
          u_xlat0_d.z = max(u_xlat0_d.z, 0.0001);
          u_xlat0_d.z = sqrt(u_xlat0_d.z);
          u_xlat0_d.yz = (u_xlat2_d.ww * u_xlat0_d.yz);
          u_xlat0_d.w = dot(u_xlat6.xyzx, u_xlat6.xyzx);
          u_xlat0_d.w = (u_xlat0_d.w!=0);
          u_xlat0_d.w = u_xlat0_d.w;
          u_xlat0_d.z = (u_xlat0_d.w * u_xlat0_d.z);
          u_xlat1_d.xyz = (u_xlat0_d.yyy * u_xlat2_d.xyz);
          u_xlat0_d.yzw = (u_xlat2_d.xyz * u_xlat0_d.zzz);
          u_xlat0_d.x = ((-u_xlat0_d.x) + 1);
          u_xlat1_d.w = (u_xlat0_d.x * u_xlat0_d.x);
          u_xlat1_d.w = (u_xlat1_d.w * u_xlat1_d.w);
          u_xlat0_d.x = (u_xlat0_d.x * u_xlat1_d.w);
          u_xlat2_d.xyz = ((-u_xlat6.xyz) + float3(1, 1, 1));
          u_xlat2_d.xyz = ((u_xlat2_d.xyz * u_xlat0_d.xxx) + u_xlat6.xyz);
          u_xlat0_d.xyz = (u_xlat0_d.yzw * u_xlat2_d.xyz);
          u_xlat0_d.xyz = ((u_xlat4.xyz * u_xlat1_d.xyz) + u_xlat0_d.xyz);
          u_xlat0_d.w = (in_f.texcoord5.x / _ProjectionParams.y);
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
    Pass // ind: 3, name: DEFERRED
    {
      Name "DEFERRED"
      Tags
      { 
        "LIGHTMODE" = "DEFERRED"
        "RenderType" = "Opaque"
      }
      LOD 200
      // m_ProgramMask = 6
      CGPROGRAM
      //#pragma target 4.0
      
      #pragma vertex vert
      #pragma fragment frag
      
      #include "UnityCG.cginc"
      
      
      #define CODE_BLOCK_VERTEX
      uniform float4 _MainTex_ST;
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
      uniform float _Glossiness;
      uniform float _Metallic;
      uniform float4 _Color;
      //uniform float4x4 unity_ProbeVolumeWorldToObject;
      //uniform float4 unity_ProbeVolumeParams;
      //uniform float3 unity_ProbeVolumeSizeInv;
      //uniform float3 unity_ProbeVolumeMin;
      uniform sampler2D _MainTex;
      //uniform sampler3D unity_ProbeVolumeSH;
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
          float3 texcoord1 :TEXCOORD1;
          float3 texcoord2 :TEXCOORD2;
          float4 texcoord4 :TEXCOORD4;
          float3 texcoord5 :TEXCOORD5;
          uint SV_InstanceID :InstanceID;
      };
      
      struct v2f
      {
          float4 vertex :SV_POSITION;
          float2 texcoord :TEXCOORD0;
          float3 texcoord1 :TEXCOORD1;
          float3 texcoord2 :TEXCOORD2;
          float4 texcoord4 :TEXCOORD4;
          float3 texcoord5 :TEXCOORD5;
          uint SV_InstanceID :InstanceID;
      };
      
      struct OUT_Data_Frag
      {
          float4 color :SV_Target;
          float4 color1 :SV_Target1;
          float4 color2 :SV_Target2;
          float4 color3 :SV_Target3;
      };
      
      float4 u_xlat0;
      float4 u_xlat1;
      float4 u_xlat2;
      float4 u_xlat3;
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
          out_v.texcoord2.xyz = ((conv_mxt4x4_(u_xlat0.y + (-8))(unity_WorldToObjectArray).xyz * in_v.vertex.www) + u_xlat1.xyz);
          out_v.vertex = mul(unity_MatrixVP, u_xlat2);
          out_v.texcoord.xy = TRANSFORM_TEX(in_v.texcoord.xy, _MainTex);
          u_xlat1.x = dot(in_v.normal.xyzx, conv_mxt4x4_(u_xlat0.y + (-7))(unity_WorldToObjectArray).xyzx);
          u_xlat1.y = dot(in_v.normal.xyzx, conv_mxt4x4_(u_xlat0.y + (-6))(unity_WorldToObjectArray).xyzx);
          u_xlat1.z = dot(in_v.normal.xyzx, conv_mxt4x4_(u_xlat0.y + (-5))(unity_WorldToObjectArray).xyzx);
          u_xlat0.y = dot(u_xlat1.xyzx, u_xlat1.xyzx);
          u_xlat0.y = rsqrt(u_xlat0.y);
          u_xlat0.yzw = (u_xlat0.yyy * u_xlat1.xyz);
          out_v.texcoord1.xyz = u_xlat0.yzw;
          out_v.texcoord4 = float4(0, 0, 0, 0);
          u_xlat1.x = (u_xlat0.z * u_xlat0.z);
          u_xlat1.x = ((u_xlat0.y * u_xlat0.y) - u_xlat1.x);
          u_xlat2 = (u_xlat0.zwwy * u_xlat0.yzww);
          u_xlat3.x = dot(unity_SHCArray, u_xlat2);
          u_xlat3.y = dot(unity_SHCArray, u_xlat2);
          u_xlat3.z = dot(unity_SHCArray, u_xlat2);
          out_v.texcoord5.xyz = ((unity_SHCArray.xyz * u_xlat1.xxx) + u_xlat3.xyz);
          in_v.SV_InstanceID.x = in_v.SV_InstanceID.x;
          return out_v;
      }
      
      #define CODE_BLOCK_FRAGMENT
      float4 u_xlat0_d;
      float4 u_xlat1_d;
      float4 u_xlat2_d;
      float4 u_xlat3_d;
      float4 u_xlat4;
      OUT_Data_Frag frag(v2f in_f)
      {
          OUT_Data_Frag out_f;
          u_xlat0_d = tex2D(_MainTex, in_f.texcoord.xy);
          u_xlat1_d.xyz = ((u_xlat0_d.xyz * _Color.xyz) - u_xlat0_d.xyz);
          u_xlat0_d.xyz = ((u_xlat0_d.www * u_xlat1_d.xyz) + u_xlat0_d.xyz);
          u_xlat0_d.w = (unity_ProbeVolumeMin.x==1);
          if((u_xlat0_d.w!=0))
          {
              u_xlat0_d.w = (unity_ProbeVolumeMin.y==1);
              u_xlat1_d.xyz = (in_f.texcoord2.yyy * unity_ProbeVolumeMin.xyz);
              u_xlat1_d.xyz = ((unity_ProbeVolumeMin.xyz * in_f.texcoord2.xxx) + u_xlat1_d.xyz);
              u_xlat1_d.xyz = ((unity_ProbeVolumeMin.xyz * in_f.texcoord2.zzz) + u_xlat1_d.xyz);
              u_xlat1_d.xyz = (u_xlat1_d.xyz + unity_ProbeVolumeMin.xyz);
              u_xlat1_d.xyz = (u_xlat0_d.www)?(u_xlat1_d.xyz):(in_f.texcoord2.xyz);
              u_xlat1_d.xyz = (u_xlat1_d.xyz - unity_ProbeVolumeMin.xyz);
              u_xlat1_d.yzw = (u_xlat1_d.xyz * unity_ProbeVolumeMin.xyz);
              u_xlat0_d.w = (u_xlat1_d.y * 0.25);
              u_xlat1_d.y = (unity_ProbeVolumeMin.z * 0.5);
              u_xlat2_d.x = (((-unity_ProbeVolumeMin.z) * 0.5) + 0.25);
              u_xlat0_d.w = max(u_xlat0_d.w, u_xlat1_d.y);
              u_xlat1_d.x = min(u_xlat2_d.x, u_xlat0_d.w);
              u_xlat2_d = tex3D(unity_ProbeVolumeSH, u_xlat1_d.xz);
              u_xlat3_d.xyz = (u_xlat1_d.xzw + float3(0.25, 0, 0));
              u_xlat3_d = tex3D(unity_ProbeVolumeSH, u_xlat3_d.xy);
              u_xlat1_d.xyz = (u_xlat1_d.xzw + float3(0.5, 0, 0));
              u_xlat1_d = tex3D(unity_ProbeVolumeSH, u_xlat1_d.xy);
              u_xlat4.xyz = in_f.texcoord1.xyz;
              u_xlat4.w = 1;
              u_xlat2_d.x = dot(u_xlat2_d, u_xlat4);
              u_xlat2_d.y = dot(u_xlat3_d, u_xlat4);
              u_xlat2_d.z = dot(u_xlat1_d, u_xlat4);
          }
          else
          {
              u_xlat0_d.w = (in_f.SV_InstanceID.x + unity_BaseInstanceID.x);
              u_xlat0_d.w = ((u_xlat0_d.w * 7) & -1);
              u_xlat1_d.xyz = in_f.texcoord1.xyz;
              u_xlat1_d.w = 1;
              u_xlat2_d.x = dot(unity_SHCArray, u_xlat1_d);
              u_xlat2_d.y = dot(unity_SHCArray, u_xlat1_d);
              u_xlat2_d.z = dot(unity_SHCArray, u_xlat1_d);
          }
          u_xlat1_d.xyz = (u_xlat2_d.xyz + in_f.texcoord5.xyz);
          u_xlat1_d.xyz = max(u_xlat1_d.xyz, float3(0, 0, 0));
          u_xlat1_d.xyz = log(u_xlat1_d.xyz);
          u_xlat1_d.xyz = (u_xlat1_d.xyz * float3(0.416667, 0.416667, 0.416667));
          u_xlat1_d.xyz = exp(u_xlat1_d.xyzx);
          u_xlat1_d.xyz = ((u_xlat1_d.xyz * float3(1.055, 1.055, 1.055)) + float3(-0.055, (-0.055), (-0.055)));
          u_xlat1_d.xyz = max(u_xlat1_d.xyz, float3(0, 0, 0));
          u_xlat2_d.xyz = (u_xlat0_d.xyz + float3(-0.220916, (-0.220916), (-0.220916)));
          out_f.color1.xyz = ((_Color.yyy * u_xlat2_d.xyz) + float3(0.220916, 0.220916, 0.220916));
          u_xlat0_d.w = (((-_Color.y) * 0.779084) + 0.779084);
          u_xlat0_d.xyz = (u_xlat0_d.www * u_xlat0_d.xyz);
          out_f.color3.xyz = (u_xlat1_d.xyz * u_xlat0_d.xyz);
          out_f.color.xyz = u_xlat0_d.xyz;
          out_f.color.w = 1;
          out_f.color1.w = _Color.x;
          out_f.color2.xyz = ((in_f.texcoord1.xyz * float3(0.5, 0.5, 0.5)) + float3(0.5, 0.5, 0.5));
          out_f.color2.w = 1;
          out_f.color3.w = 1;
          return out_f;
      }
      
      
      ENDCG
      
    } // end phase
  }
  FallBack "Standard"
}
