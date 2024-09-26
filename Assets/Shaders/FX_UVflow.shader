// Upgrade NOTE: commented out 'float4 unity_ShadowFadeCenterAndType', a built-in variable

Shader "FX/UVflow"
{
  Properties
  {
    _MainTex ("Base (RGB)", 2D) = "white" {}
    _XScrollSpeed ("X Scroll Speed", float) = 1
    _Metalness ("Metallic", Range(0, 1)) = 0.187
    _Glossiness ("Smoothness", Range(0, 1)) = 0.5
  }
  SubShader
  {
    Tags
    { 
      "RenderType" = "Opaque"
    }
    LOD 300
    Pass // ind: 1, name: FORWARD
    {
      Name "FORWARD"
      Tags
      { 
        "LIGHTMODE" = "FORWARDBASE"
        "RenderType" = "Opaque"
        "SHADOWSUPPORT" = "true"
      }
      LOD 300
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
      uniform float4 _MainTex_ST;
      //uniform float4 _ProjectionParams;
      //uniform float4 unity_4LightPosX0;
      //uniform float4 unity_4LightPosY0;
      //uniform float4 unity_4LightPosZ0;
      //uniform float4 unity_4LightAtten0;
      //uniform float4 unity_LightColor;
      //uniform float4 unity_SHBr;
      //uniform float4 unity_SHBg;
      //uniform float4 unity_SHBb;
      //uniform float4 unity_SHC;
      //uniform float4x4 unity_ObjectToWorld;
      //uniform float4x4 unity_WorldToObject;
      //uniform float4x4 unity_MatrixVP;
      uniform float4 _LightColor0;
      uniform float _XScrollSpeed;
      uniform float _Metalness;
      uniform float _Glossiness;
      //uniform float4 _Time;
      //uniform float3 _WorldSpaceCameraPos;
      //uniform float4 _WorldSpaceLightPos0;
      //uniform float4 unity_SHAr;
      //uniform float4 unity_SHAg;
      //uniform float4 unity_SHAb;
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
          u_xlat0 = (in_v.vertex.yyyy * conv_mxt4x4_-2(unity_WorldToObject));
          u_xlat0 = ((conv_mxt4x4_-3(unity_WorldToObject) * in_v.vertex.xxxx) + u_xlat0);
          u_xlat0 = ((conv_mxt4x4_-1(unity_WorldToObject) * in_v.vertex.zzzz) + u_xlat0);
          u_xlat1 = (u_xlat0 + conv_mxt4x4_0(unity_WorldToObject));
          u_xlat0.xyz = ((conv_mxt4x4_0(unity_WorldToObject).xyz * in_v.vertex.www) + u_xlat0.xyz);
          u_xlat1 = mul(unity_MatrixVP, u_xlat1);
          out_v.vertex = u_xlat1;
          out_v.texcoord4.x = u_xlat1.z;
          out_v.texcoord5.zw = u_xlat1.zw;
          out_v.texcoord.xy = TRANSFORM_TEX(in_v.texcoord.xy, _MainTex);
          u_xlat2.x = dot(in_v.normal.xyzx, conv_mxt4x4_1(unity_WorldToObject).xyzx);
          u_xlat2.y = dot(in_v.normal.xyzx, conv_mxt4x4_2(unity_WorldToObject).xyzx);
          u_xlat2.z = dot(in_v.normal.xyzx, conv_mxt4x4_3(unity_WorldToObject).xyzx);
          u_xlat0.w = dot(u_xlat2.xyzx, u_xlat2.xyzx);
          u_xlat0.w = rsqrt(u_xlat0.w);
          u_xlat2.xyz = (u_xlat0.www * u_xlat2.xyz);
          out_v.texcoord1.xyz = u_xlat2.xyz;
          out_v.texcoord2.xyz = u_xlat0.xyz;
          u_xlat3 = ((-u_xlat0.xxxx) + unity_SHC);
          u_xlat4 = ((-u_xlat0.yyyy) + unity_SHC);
          u_xlat0 = ((-u_xlat0.zzzz) + unity_SHC);
          u_xlat5 = (u_xlat2.yyyy * u_xlat4);
          u_xlat4 = (u_xlat4 * u_xlat4);
          u_xlat4 = ((u_xlat3 * u_xlat3) + u_xlat4);
          u_xlat3 = ((u_xlat3 * u_xlat2.xxxx) + u_xlat5);
          u_xlat3 = ((u_xlat0 * u_xlat2.zzzz) + u_xlat3);
          u_xlat0 = ((u_xlat0 * u_xlat0) + u_xlat4);
          u_xlat0 = max(u_xlat0, float4(1E-06, 1E-06, 1E-06, 1E-06));
          u_xlat4 = rsqrt(u_xlat0);
          u_xlat0 = ((u_xlat0 * unity_SHC) + float4(1, 1, 1, 1));
          u_xlat0 = (float4(1, 1, 1, 1) / u_xlat0);
          u_xlat3 = (u_xlat3 * u_xlat4);
          u_xlat3 = max(u_xlat3, float4(0, 0, 0, 0));
          u_xlat0 = (u_xlat0 * u_xlat3);
          u_xlat3.xyz = (u_xlat0.yyy * unity_SHC.xyz);
          u_xlat3.xyz = ((unity_SHC.xyz * u_xlat0.xxx) + u_xlat3.xyz);
          u_xlat0.xyz = ((unity_SHC.xyz * u_xlat0.zzz) + u_xlat3.xyz);
          u_xlat0.xyz = ((unity_SHC.xyz * u_xlat0.www) + u_xlat0.xyz);
          u_xlat3.xyz = ((u_xlat0.xyz * float3(0.305306, 0.305306, 0.305306)) + float3(0.682171, 0.682171, 0.682171));
          u_xlat3.xyz = ((u_xlat0.xyz * u_xlat3.xyz) + float3(0.012523, 0.012523, 0.012523));
          u_xlat0.w = (u_xlat2.y * u_xlat2.y);
          u_xlat0.w = ((u_xlat2.x * u_xlat2.x) - u_xlat0.w);
          u_xlat2 = (u_xlat2.yzzx * u_xlat2.xyzz);
          u_xlat4.x = dot(unity_SHC, u_xlat2);
          u_xlat4.y = dot(unity_SHC, u_xlat2);
          u_xlat4.z = dot(unity_SHC, u_xlat2);
          u_xlat2.xyz = ((unity_SHC.xyz * u_xlat0.www) + u_xlat4.xyz);
          out_v.texcoord3.xyz = ((u_xlat0.xyz * u_xlat3.xyz) + u_xlat2.xyz);
          u_xlat0.x = (u_xlat1.y * _ProjectionParams.x);
          u_xlat1.xz = (u_xlat1.xw * float2(0.5, 0.5));
          u_xlat1.w = (u_xlat0.x * 0.5);
          out_v.texcoord5.xy = (u_xlat1.zz + u_xlat1.xw);
          out_v.texcoord6 = float4(0, 0, 0, 0);
          return out_v;
      }
      
      #define CODE_BLOCK_FRAGMENT
      float4 u_xlat0_d;
      float4 u_xlat1_d;
      float4 u_xlat2_d;
      float4 u_xlat3_d;
      float4 u_xlat4_d;
      float4 u_xlat5_d;
      float4 u_xlat6;
      float4 u_xlat7;
      float4 u_xlat8;
      float4 u_xlat9;
      float4 u_xlat10;
      float4 u_xlat11;
      OUT_Data_Frag frag(v2f in_f)
      {
          OUT_Data_Frag out_f;
          u_xlat0_d.xyz = ((-in_f.texcoord2.xyz) + _ProjectionParams.xyz);
          u_xlat0_d.w = dot(u_xlat0_d.xyzx, u_xlat0_d.xyzx);
          u_xlat0_d.w = rsqrt(u_xlat0_d.w);
          u_xlat1_d.xyz = (u_xlat0_d.www * u_xlat0_d.xyz);
          u_xlat2_d.x = (_Glossiness.x * _ProjectionParams.x);
          u_xlat2_d.y = 0;
          u_xlat2_d.xy = (u_xlat2_d.xy + in_f.texcoord.xy);
          u_xlat2_d = tex2D(_MainTex, u_xlat2_d.xy);
          u_xlat3_d.x = conv_mxt4x4_1(unity_MatrixV).z;
          u_xlat3_d.y = conv_mxt4x4_2(unity_MatrixV).z;
          u_xlat3_d.z = conv_mxt4x4_3(unity_MatrixV).z;
          u_xlat1_d.w = dot(u_xlat0_d.xyzx, u_xlat3_d.xyzx);
          u_xlat3_d.xyz = (in_f.texcoord2.xyz - unity_ShadowFadeCenterAndType.xyz);
          u_xlat2_d.w = length(u_xlat3_d.xyzx);
          u_xlat2_d.w = ((-u_xlat1_d.w) + u_xlat2_d.w);
          u_xlat1_d.w = ((unity_ShadowFadeCenterAndType.w * u_xlat2_d.w) + u_xlat1_d.w);
          u_xlat1_d.w = saturate(((u_xlat1_d.w * unity_ShadowFadeCenterAndType.z) + unity_ShadowFadeCenterAndType.w));
          u_xlat2_d.w = (unity_ProbeVolumeMin.x==1);
          if((u_xlat2_d.w!=0))
          {
              u_xlat3_d.x = (unity_ProbeVolumeMin.y==1);
              u_xlat3_d.yzw = (in_f.texcoord2.yyy * unity_ProbeVolumeMin.xyz);
              u_xlat3_d.yzw = ((unity_ProbeVolumeMin.xyz * in_f.texcoord2.xxx) + u_xlat3_d.yzw);
              u_xlat3_d.yzw = ((unity_ProbeVolumeMin.xyz * in_f.texcoord2.zzz) + u_xlat3_d.yzw);
              u_xlat3_d.yzw = (u_xlat3_d.yzw + unity_ProbeVolumeMin.xyz);
              u_xlat3_d.xyz = (u_xlat3_d.xxx)?(u_xlat3_d.yzw):(in_f.texcoord2.xyz);
              u_xlat3_d.xyz = (u_xlat3_d.xyz - unity_ProbeVolumeMin.xyz);
              u_xlat3_d.yzw = (u_xlat3_d.xyz * unity_ProbeVolumeMin.xyz);
              u_xlat3_d.y = ((u_xlat3_d.y * 0.25) + 0.75);
              u_xlat4_d.x = ((unity_ProbeVolumeMin.z * 0.5) + 0.75);
              u_xlat3_d.x = max(u_xlat3_d.y, u_xlat4_d.x);
              u_xlat3_d = tex3D(unity_ProbeVolumeSH, u_xlat3_d.xz);
          }
          else
          {
              u_xlat3_d = float4(1, 1, 1, 1);
          }
          u_xlat3_d.x = saturate(dot(u_xlat3_d, unity_OcclusionMaskSelector));
          u_xlat3_d.yz = (in_f.texcoord5.xy / in_f.texcoord5.ww);
          u_xlat4_d = tex2D(_ShadowMapTexture, u_xlat3_d.yz);
          u_xlat3_d.x = (u_xlat3_d.x - u_xlat4_d.x);
          u_xlat1_d.w = ((u_xlat1_d.w * u_xlat3_d.x) + u_xlat4_d.x);
          u_xlat3_d.x = ((-_Glossiness.z) + 1);
          u_xlat3_d.y = dot((-u_xlat1_d.xyzx), in_f.texcoord1.xyzx);
          u_xlat3_d.y = (u_xlat3_d.y + u_xlat3_d.y);
          u_xlat3_d.yzw = ((in_f.texcoord1.xyz * (-u_xlat3_d.yyy)) - u_xlat1_d.xyz);
          u_xlat4_d.xyz = (u_xlat1_d.www * _Glossiness.xyz);
          if((u_xlat2_d.w!=0))
          {
              u_xlat1_d.w = (unity_ProbeVolumeMin.y==1);
              u_xlat5_d.xyz = (in_f.texcoord2.yyy * unity_ProbeVolumeMin.xyz);
              u_xlat5_d.xyz = ((unity_ProbeVolumeMin.xyz * in_f.texcoord2.xxx) + u_xlat5_d.xyz);
              u_xlat5_d.xyz = ((unity_ProbeVolumeMin.xyz * in_f.texcoord2.zzz) + u_xlat5_d.xyz);
              u_xlat5_d.xyz = (u_xlat5_d.xyz + unity_ProbeVolumeMin.xyz);
              u_xlat5_d.xyz = (u_xlat1_d.www)?(u_xlat5_d.xyz):(in_f.texcoord2.xyz);
              u_xlat5_d.xyz = (u_xlat5_d.xyz - unity_ProbeVolumeMin.xyz);
              u_xlat5_d.yzw = (u_xlat5_d.xyz * unity_ProbeVolumeMin.xyz);
              u_xlat1_d.w = (u_xlat5_d.y * 0.25);
              u_xlat2_d.w = (unity_ProbeVolumeMin.z * 0.5);
              u_xlat4_d.w = (((-unity_ProbeVolumeMin.z) * 0.5) + 0.25);
              u_xlat1_d.w = max(u_xlat1_d.w, u_xlat2_d.w);
              u_xlat5_d.x = min(u_xlat4_d.w, u_xlat1_d.w);
              u_xlat6 = tex3D(unity_ProbeVolumeSH, u_xlat5_d.xz);
              u_xlat7.xyz = (u_xlat5_d.xzw + float3(0.25, 0, 0));
              u_xlat7 = tex3D(unity_ProbeVolumeSH, u_xlat7.xy);
              u_xlat5_d.xyz = (u_xlat5_d.xzw + float3(0.5, 0, 0));
              u_xlat5_d = tex3D(unity_ProbeVolumeSH, u_xlat5_d.xy);
              u_xlat8.xyz = in_f.texcoord1.xyz;
              u_xlat8.w = 1;
              u_xlat6.x = dot(u_xlat6, u_xlat8);
              u_xlat6.y = dot(u_xlat7, u_xlat8);
              u_xlat6.z = dot(u_xlat5_d, u_xlat8);
          }
          else
          {
              u_xlat5_d.xyz = in_f.texcoord1.xyz;
              u_xlat5_d.w = 1;
              u_xlat6.x = dot(unity_OcclusionMaskSelector, u_xlat5_d);
              u_xlat6.y = dot(unity_OcclusionMaskSelector, u_xlat5_d);
              u_xlat6.z = dot(unity_OcclusionMaskSelector, u_xlat5_d);
          }
          u_xlat5_d.xyz = (u_xlat6.xyz + in_f.texcoord3.xyz);
          u_xlat5_d.xyz = max(u_xlat5_d.xyz, float3(0, 0, 0));
          u_xlat5_d.xyz = log(u_xlat5_d.xyz);
          u_xlat5_d.xyz = (u_xlat5_d.xyz * float3(0.416667, 0.416667, 0.416667));
          u_xlat5_d.xyz = exp(u_xlat5_d.xyzx);
          u_xlat5_d.xyz = ((u_xlat5_d.xyz * float3(1.055, 1.055, 1.055)) + float3(-0.055, (-0.055), (-0.055)));
          u_xlat5_d.xyz = max(u_xlat5_d.xyz, float3(0, 0, 0));
          u_xlat1_d.w = (0<unity_SpecCube1_HDR.w);
          if((u_xlat1_d.w!=0))
          {
              u_xlat1_d.w = dot(u_xlat3_d.yzwy, u_xlat3_d.yzwy);
              u_xlat1_d.w = rsqrt(u_xlat1_d.w);
              u_xlat6.xyz = (u_xlat1_d.www * u_xlat3_d.yzw);
              u_xlat7.xyz = ((-in_f.texcoord2.xyz) + unity_SpecCube1_HDR.xyz);
              u_xlat7.xyz = (u_xlat7.xyz / u_xlat6.xyz);
              u_xlat8.xyz = ((-in_f.texcoord2.xyz) + unity_SpecCube1_HDR.xyz);
              u_xlat8.xyz = (u_xlat8.xyz / u_xlat6.xyz);
              u_xlat9.xyz = (float3(0, 0, 0)<u_xlat6.xyz);
              u_xlat7.xyz = (u_xlat9.xyz)?(u_xlat7.xyz):(u_xlat8.xyz);
              u_xlat1_d.w = min(u_xlat7.y, u_xlat7.x);
              u_xlat1_d.w = min(u_xlat7.z, u_xlat1_d.w);
              u_xlat7.xyz = (in_f.texcoord2.xyz - unity_SpecCube1_HDR.xyz);
              u_xlat6.xyz = ((u_xlat6.xyz * u_xlat1_d.www) + u_xlat7.xyz);
          }
          else
          {
              u_xlat6.xyz = u_xlat3_d.yzw;
          }
          u_xlat1_d.w = (((-u_xlat3_d.x) * 0.7) + 1.7);
          u_xlat1_d.w = (u_xlat1_d.w * u_xlat3_d.x);
          u_xlat1_d.w = (u_xlat1_d.w * 6);
          u_xlat6 = tex2D(unity_SpecCube0, u_xlat6.xy);
          u_xlat2_d.w = (u_xlat6.w - 1);
          u_xlat2_d.w = ((unity_SpecCube1_HDR.w * u_xlat2_d.w) + 1);
          u_xlat2_d.w = (u_xlat2_d.w * unity_SpecCube1_HDR.x);
          u_xlat7.xyz = (u_xlat6.xyz * u_xlat2_d.www);
          u_xlat4_d.w = (unity_SpecCube1_HDR.w<0.99999);
          if((u_xlat4_d.w!=0))
          {
              u_xlat4_d.w = (0<unity_SpecCube1_HDR.w);
              if((u_xlat4_d.w!=0))
              {
                  u_xlat4_d.w = dot(u_xlat3_d.yzwy, u_xlat3_d.yzwy);
                  u_xlat4_d.w = rsqrt(u_xlat4_d.w);
                  u_xlat8.xyz = (u_xlat3_d.yzw * u_xlat4_d.www);
                  u_xlat9.xyz = ((-in_f.texcoord2.xyz) + unity_SpecCube1_HDR.xyz);
                  u_xlat9.xyz = (u_xlat9.xyz / u_xlat8.xyz);
                  u_xlat10.xyz = ((-in_f.texcoord2.xyz) + unity_SpecCube1_HDR.xyz);
                  u_xlat10.xyz = (u_xlat10.xyz / u_xlat8.xyz);
                  u_xlat11.xyz = (float3(0, 0, 0)<u_xlat8.xyz);
                  u_xlat9.xyz = (u_xlat11.xyz)?(u_xlat9.xyz):(u_xlat10.xyz);
                  u_xlat4_d.w = min(u_xlat9.y, u_xlat9.x);
                  u_xlat4_d.w = min(u_xlat9.z, u_xlat4_d.w);
                  u_xlat9.xyz = (in_f.texcoord2.xyz - unity_SpecCube1_HDR.xyz);
                  u_xlat3_d.yzw = ((u_xlat8.xyz * u_xlat4_d.www) + u_xlat9.xyz);
              }
              u_xlat8 = tex2D(unity_SpecCube1, u_xlat3_d.yz);
              u_xlat1_d.w = (u_xlat8.w - 1);
              u_xlat1_d.w = ((unity_SpecCube1_HDR.w * u_xlat1_d.w) + 1);
              u_xlat1_d.w = (u_xlat1_d.w * unity_SpecCube1_HDR.x);
              u_xlat3_d.yzw = (u_xlat8.xyz * u_xlat1_d.www);
              u_xlat6.xyz = ((u_xlat2_d.www * u_xlat6.xyz) - u_xlat3_d.yzw);
              u_xlat7.xyz = ((unity_SpecCube1_HDR.www * u_xlat6.xyz) + u_xlat3_d.yzw);
          }
          u_xlat1_d.w = dot(in_f.texcoord1.xyzx, in_f.texcoord1.xyzx);
          u_xlat1_d.w = rsqrt(u_xlat1_d.w);
          u_xlat3_d.yzw = (u_xlat1_d.www * in_f.texcoord1.xyz);
          u_xlat6.xyz = (u_xlat2_d.xyz + float3(-0.220916, (-0.220916), (-0.220916)));
          u_xlat6.xyz = ((_Glossiness.yyy * u_xlat6.xyz) + float3(0.220916, 0.220916, 0.220916));
          u_xlat1_d.w = (((-_Glossiness.y) * 0.779084) + 0.779084);
          u_xlat8.xyz = (u_xlat1_d.www * u_xlat2_d.xyz);
          u_xlat0_d.xyz = ((u_xlat0_d.xyz * u_xlat0_d.www) + unity_OcclusionMaskSelector.xyz);
          u_xlat0_d.w = dot(u_xlat0_d.xyzx, u_xlat0_d.xyzx);
          u_xlat0_d.w = max(u_xlat0_d.w, 0.001);
          u_xlat0_d.w = rsqrt(u_xlat0_d.w);
          u_xlat0_d.xyz = (u_xlat0_d.www * u_xlat0_d.xyz);
          u_xlat0_d.w = dot(u_xlat3_d.yzwy, u_xlat1_d.xyzx);
          u_xlat1_d.x = saturate(dot(u_xlat3_d.yzwy, unity_OcclusionMaskSelector.xyzx));
          u_xlat1_d.y = saturate(dot(u_xlat3_d.yzwy, u_xlat0_d.xyzx));
          u_xlat0_d.x = saturate(dot(unity_OcclusionMaskSelector.xyzx, u_xlat0_d.xyzx));
          u_xlat0_d.y = (u_xlat0_d.x * u_xlat0_d.x);
          u_xlat0_d.y = dot(u_xlat0_d.yyyy, u_xlat3_d.xxxx);
          u_xlat0_d.y = (u_xlat0_d.y - 0.5);
          u_xlat0_d.z = ((-u_xlat1_d.x) + 1);
          u_xlat1_d.z = (u_xlat0_d.z * u_xlat0_d.z);
          u_xlat1_d.z = (u_xlat1_d.z * u_xlat1_d.z);
          u_xlat0_d.z = (u_xlat0_d.z * u_xlat1_d.z);
          u_xlat0_d.z = ((u_xlat0_d.y * u_xlat0_d.z) + 1);
          u_xlat1_d.z = ((-abs(u_xlat0_d.w)) + 1);
          u_xlat2_d.w = (u_xlat1_d.z * u_xlat1_d.z);
          u_xlat2_d.w = (u_xlat2_d.w * u_xlat2_d.w);
          u_xlat1_d.z = (u_xlat1_d.z * u_xlat2_d.w);
          u_xlat0_d.y = ((u_xlat0_d.y * u_xlat1_d.z) + 1);
          u_xlat0_d.y = (u_xlat0_d.y * u_xlat0_d.z);
          u_xlat0_d.z = (u_xlat3_d.x * u_xlat3_d.x);
          u_xlat0_d.z = max(u_xlat0_d.z, 0.002);
          u_xlat2_d.w = ((-u_xlat0_d.z) + 1);
          u_xlat3_d.y = ((abs(u_xlat0_d.w) * u_xlat2_d.w) + u_xlat0_d.z);
          u_xlat2_d.w = ((u_xlat1_d.x * u_xlat2_d.w) + u_xlat0_d.z);
          u_xlat0_d.w = (abs(u_xlat0_d.w) * u_xlat2_d.w);
          u_xlat0_d.w = ((u_xlat1_d.x * u_xlat3_d.y) + u_xlat0_d.w);
          u_xlat0_d.w = (u_xlat0_d.w + 1E-05);
          u_xlat0_d.w = (0.5 / u_xlat0_d.w);
          u_xlat2_d.w = (u_xlat0_d.z * u_xlat0_d.z);
          u_xlat3_d.y = ((u_xlat1_d.y * u_xlat2_d.w) - u_xlat1_d.y);
          u_xlat1_d.y = ((u_xlat3_d.y * u_xlat1_d.y) + 1);
          u_xlat2_d.w = (u_xlat2_d.w * 0.31831);
          u_xlat1_d.y = ((u_xlat1_d.y * u_xlat1_d.y) + 0);
          u_xlat1_d.y = (u_xlat2_d.w / u_xlat1_d.y);
          u_xlat0_d.w = (u_xlat0_d.w * u_xlat1_d.y);
          u_xlat0_d.zw = (u_xlat0_d.zw * float2(0.28, 3.141593));
          u_xlat0_d.w = max(u_xlat0_d.w, 0.0001);
          u_xlat0_d.w = sqrt(u_xlat0_d.w);
          u_xlat0_d.yw = (u_xlat1_d.xx * u_xlat0_d.yw);
          u_xlat0_d.z = (((-u_xlat0_d.z) * u_xlat3_d.x) + 1);
          u_xlat1_d.x = dot(u_xlat6.xyzx, u_xlat6.xyzx);
          u_xlat1_d.x = (u_xlat1_d.x!=0);
          u_xlat1_d.x = u_xlat1_d.x;
          u_xlat0_d.w = (u_xlat0_d.w * u_xlat1_d.x);
          u_xlat1_d.x = ((-u_xlat1_d.w) + 1);
          u_xlat1_d.x = saturate((u_xlat1_d.x + _Glossiness.z));
          u_xlat3_d.xyz = ((u_xlat4_d.xyz * u_xlat0_d.yyy) + u_xlat5_d.xyz);
          u_xlat4_d.xyz = (u_xlat4_d.xyz * u_xlat0_d.www);
          u_xlat0_d.x = ((-u_xlat0_d.x) + 1);
          u_xlat0_d.y = (u_xlat0_d.x * u_xlat0_d.x);
          u_xlat0_d.y = (u_xlat0_d.y * u_xlat0_d.y);
          u_xlat0_d.x = (u_xlat0_d.x * u_xlat0_d.y);
          u_xlat5_d.xyz = ((-u_xlat6.xyz) + float3(1, 1, 1));
          u_xlat0_d.xyw = ((u_xlat5_d.xyz * u_xlat0_d.xxx) + u_xlat6.xyz);
          u_xlat0_d.xyw = (u_xlat0_d.xyw * u_xlat4_d.xyz);
          u_xlat0_d.xyw = ((u_xlat8.xyz * u_xlat3_d.xyz) + u_xlat0_d.xyw);
          u_xlat3_d.xyz = (u_xlat7.xyz * u_xlat0_d.zzz);
          u_xlat1_d.xyw = ((-u_xlat6.xyz) + u_xlat1_d.xxx);
          u_xlat1_d.xyz = ((u_xlat1_d.zzz * u_xlat1_d.xyw) + u_xlat6.xyz);
          u_xlat0_d.xyz = ((u_xlat3_d.xyz * u_xlat1_d.xyz) + u_xlat0_d.xyw);
          u_xlat0_d.xyz = (u_xlat2_d.xyz + u_xlat0_d.xyz);
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
      }
      LOD 300
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
      //uniform float4 _ProjectionParams;
      //uniform float4x4 unity_ObjectToWorld;
      //uniform float4x4 unity_WorldToObject;
      //uniform float4x4 unity_MatrixVP;
      //uniform float4 unity_FogParams;
      uniform float4 _LightColor0;
      uniform float _XScrollSpeed;
      uniform float _Metalness;
      uniform float _Glossiness;
      //uniform float4 _Time;
      //uniform float3 _WorldSpaceCameraPos;
      //uniform float4 _WorldSpaceLightPos0;
      //uniform float4 unity_OcclusionMaskSelector;
      //uniform float4x4 unity_ProbeVolumeWorldToObject;
      //uniform float4 unity_ProbeVolumeParams;
      //uniform float3 unity_ProbeVolumeSizeInv;
      //uniform float3 unity_ProbeVolumeMin;
      uniform sampler2D _MainTex;
      uniform sampler2D _LightTexture0;
      uniform sampler2D unity_NHxRoughness;
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
      };
      
      struct OUT_Data_Vert
      {
          float4 vertex :SV_POSITION;
          float2 texcoord :TEXCOORD0;
          float2 texcoord3 :TEXCOORD3;
          float3 texcoord1 :TEXCOORD1;
          float texcoord5 :TEXCOORD5;
          float3 texcoord2 :TEXCOORD2;
          float4 texcoord4 :TEXCOORD4;
      };
      
      struct v2f
      {
          float4 vertex :SV_POSITION;
          float2 texcoord :TEXCOORD0;
          float2 texcoord3 :TEXCOORD3;
          float3 texcoord1 :TEXCOORD1;
          float texcoord5 :TEXCOORD5;
          float3 texcoord2 :TEXCOORD2;
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
          u_xlat1.x = (u_xlat1.z / _ProjectionParams.y);
          u_xlat1.x = ((-u_xlat1.x) + 1);
          u_xlat1.x = (u_xlat1.x * _ProjectionParams.z);
          u_xlat1.x = max(u_xlat1.x, 0);
          u_xlat1.x = (u_xlat1.x * unity_FogParams.y);
          out_v.texcoord5.x = exp((-u_xlat1.x));
          u_xlat1 = ((conv_mxt4x4_0(unity_WorldToObject) * in_v.vertex.wwww) + u_xlat0);
          out_v.texcoord2.xyz = ((conv_mxt4x4_0(unity_WorldToObject).xyz * in_v.vertex.www) + u_xlat0.xyz);
          u_xlat0.xy = (u_xlat1.yy * _MainTex_ST.xy);
          u_xlat0.xy = ((_MainTex_ST.xy * u_xlat1.xx) + u_xlat0.xy);
          u_xlat0.xy = ((_MainTex_ST.xy * u_xlat1.zz) + u_xlat0.xy);
          out_v.texcoord3.xy = ((_MainTex_ST.xy * u_xlat1.ww) + u_xlat0.xy);
          out_v.texcoord.xy = TRANSFORM_TEX(in_v.texcoord.xy, _MainTex);
          u_xlat0.x = dot(in_v.normal.xyzx, conv_mxt4x4_1(unity_WorldToObject).xyzx);
          u_xlat0.y = dot(in_v.normal.xyzx, conv_mxt4x4_2(unity_WorldToObject).xyzx);
          u_xlat0.z = dot(in_v.normal.xyzx, conv_mxt4x4_3(unity_WorldToObject).xyzx);
          u_xlat0.w = dot(u_xlat0.xyzx, u_xlat0.xyzx);
          u_xlat0.w = rsqrt(u_xlat0.w);
          out_v.texcoord1.xyz = (u_xlat0.www * u_xlat0.xyz);
          out_v.texcoord4 = float4(0, 0, 0, 0);
          return out_v;
      }
      
      #define CODE_BLOCK_FRAGMENT
      float4 u_xlat0_d;
      float4 u_xlat1_d;
      float4 u_xlat2_d;
      float4 u_xlat3;
      float4 u_xlat4;
      OUT_Data_Frag frag(v2f in_f)
      {
          OUT_Data_Frag out_f;
          u_xlat0_d.xyz = ((-in_f.texcoord2.xyz) + _WorldSpaceCameraPos.xyz);
          u_xlat0_d.w = dot(u_xlat0_d.xyzx, u_xlat0_d.xyzx);
          u_xlat0_d.w = rsqrt(u_xlat0_d.w);
          u_xlat0_d.xyz = (u_xlat0_d.www * u_xlat0_d.xyz);
          u_xlat1_d.x = (_Glossiness.x * _WorldSpaceCameraPos.x);
          u_xlat1_d.y = 0;
          u_xlat1_d.xy = (u_xlat1_d.xy + in_f.texcoord.xy);
          u_xlat1_d = tex2D(_MainTex, u_xlat1_d.xy);
          u_xlat2_d.xy = (in_f.texcoord2.yy * _Glossiness.xy);
          u_xlat2_d.xy = ((_Glossiness.xy * in_f.texcoord2.xx) + u_xlat2_d.xy);
          u_xlat2_d.xy = ((_Glossiness.xy * in_f.texcoord2.zz) + u_xlat2_d.xy);
          u_xlat2_d.xy = (u_xlat2_d.xy + _Glossiness.xy);
          u_xlat0_d.w = (unity_ProbeVolumeMin.x==1);
          if((u_xlat0_d.w!=0))
          {
              u_xlat0_d.w = (unity_ProbeVolumeMin.y==1);
              u_xlat3.xyz = (in_f.texcoord2.yyy * unity_ProbeVolumeMin.xyz);
              u_xlat3.xyz = ((unity_ProbeVolumeMin.xyz * in_f.texcoord2.xxx) + u_xlat3.xyz);
              u_xlat3.xyz = ((unity_ProbeVolumeMin.xyz * in_f.texcoord2.zzz) + u_xlat3.xyz);
              u_xlat3.xyz = (u_xlat3.xyz + unity_ProbeVolumeMin.xyz);
              u_xlat3.xyz = (u_xlat0_d.www)?(u_xlat3.xyz):(in_f.texcoord2.xyz);
              u_xlat3.xyz = (u_xlat3.xyz - unity_ProbeVolumeMin.xyz);
              u_xlat3.yzw = (u_xlat3.xyz * unity_ProbeVolumeMin.xyz);
              u_xlat0_d.w = ((u_xlat3.y * 0.25) + 0.75);
              u_xlat1_d.w = ((unity_ProbeVolumeMin.z * 0.5) + 0.75);
              u_xlat3.x = max(u_xlat0_d.w, u_xlat1_d.w);
              u_xlat3 = tex3D(unity_ProbeVolumeSH, u_xlat3.xz);
          }
          else
          {
              u_xlat3 = float4(1, 1, 1, 1);
          }
          u_xlat0_d.w = saturate(dot(u_xlat3, unity_OcclusionMaskSelector));
          u_xlat2_d = tex2D(_LightTexture0, u_xlat2_d.xy);
          u_xlat0_d.w = (u_xlat0_d.w * u_xlat2_d.w);
          u_xlat2_d.xyz = (u_xlat0_d.www * _Glossiness.xyz);
          u_xlat0_d.w = dot(in_f.texcoord1.xyzx, in_f.texcoord1.xyzx);
          u_xlat0_d.w = rsqrt(u_xlat0_d.w);
          u_xlat3.xyz = (u_xlat0_d.www * in_f.texcoord1.xyz);
          u_xlat4.xyz = (u_xlat1_d.xyz + float3(-0.220916, (-0.220916), (-0.220916)));
          u_xlat4.xyz = ((_Glossiness.yyy * u_xlat4.xyz) + float3(0.220916, 0.220916, 0.220916));
          u_xlat0_d.w = (((-_Glossiness.y) * 0.779084) + 0.779084);
          u_xlat1_d.w = dot(u_xlat0_d.xyzx, u_xlat3.xyzx);
          u_xlat1_d.w = (u_xlat1_d.w + u_xlat1_d.w);
          u_xlat0_d.xyz = ((u_xlat3.xyz * (-u_xlat1_d.www)) + u_xlat0_d.xyz);
          u_xlat1_d.w = saturate(dot(u_xlat3.xyzx, unity_OcclusionMaskSelector.xyzx));
          u_xlat0_d.x = dot(u_xlat0_d.xyzx, unity_OcclusionMaskSelector.xyzx);
          u_xlat0_d.x = (u_xlat0_d.x * u_xlat0_d.x);
          u_xlat0_d.x = (u_xlat0_d.x * u_xlat0_d.x);
          u_xlat0_d.y = ((-_Glossiness.z) + 1);
          u_xlat3 = tex2D(unity_NHxRoughness, u_xlat0_d.xy);
          u_xlat0_d.x = (u_xlat3.x * 16);
          u_xlat0_d.xyz = (u_xlat4.xyz * u_xlat0_d.xxx);
          u_xlat0_d.xyz = ((u_xlat1_d.xyz * u_xlat0_d.www) + u_xlat0_d.xyz);
          u_xlat1_d.xyz = (u_xlat1_d.www * u_xlat2_d.xyz);
          u_xlat0_d.xyz = (u_xlat0_d.xyz * u_xlat1_d.xyz);
          u_xlat0_d.w = saturate(in_f.texcoord5.x);
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
      LOD 300
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
      uniform float4 _MainTex_ST;
      //uniform float4 unity_SHAr;
      //uniform float4 unity_SHAg;
      //uniform float4 unity_SHAb;
      //uniform float4 unity_SHBr;
      //uniform float4 unity_SHBg;
      //uniform float4 unity_SHBb;
      //uniform float4 unity_SHC;
      //uniform float4x4 unity_ObjectToWorld;
      //uniform float4x4 unity_WorldToObject;
      //uniform float4x4 unity_MatrixVP;
      uniform float _XScrollSpeed;
      uniform float _Metalness;
      uniform float _Glossiness;
      //uniform float4 _Time;
      uniform sampler2D _MainTex;
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
          float3 texcoord1 :TEXCOORD1;
          float3 texcoord2 :TEXCOORD2;
          float4 texcoord4 :TEXCOORD4;
          float3 texcoord5 :TEXCOORD5;
      };
      
      struct v2f
      {
          float4 vertex :SV_POSITION;
          float2 texcoord :TEXCOORD0;
          float3 texcoord1 :TEXCOORD1;
          float3 texcoord2 :TEXCOORD2;
          float4 texcoord4 :TEXCOORD4;
          float3 texcoord5 :TEXCOORD5;
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
          u_xlat0 = (in_v.vertex.yyyy * conv_mxt4x4_-2(unity_WorldToObject));
          u_xlat0 = ((conv_mxt4x4_-3(unity_WorldToObject) * in_v.vertex.xxxx) + u_xlat0);
          u_xlat0 = ((conv_mxt4x4_-1(unity_WorldToObject) * in_v.vertex.zzzz) + u_xlat0);
          u_xlat1 = (u_xlat0 + conv_mxt4x4_0(unity_WorldToObject));
          out_v.texcoord2.xyz = ((conv_mxt4x4_0(unity_WorldToObject).xyz * in_v.vertex.www) + u_xlat0.xyz);
          out_v.vertex = mul(unity_MatrixVP, u_xlat1);
          out_v.texcoord.xy = TRANSFORM_TEX(in_v.texcoord.xy, _MainTex);
          u_xlat0.x = dot(in_v.normal.xyzx, conv_mxt4x4_1(unity_WorldToObject).xyzx);
          u_xlat0.y = dot(in_v.normal.xyzx, conv_mxt4x4_2(unity_WorldToObject).xyzx);
          u_xlat0.z = dot(in_v.normal.xyzx, conv_mxt4x4_3(unity_WorldToObject).xyzx);
          u_xlat0.w = dot(u_xlat0.xyzx, u_xlat0.xyzx);
          u_xlat0.w = rsqrt(u_xlat0.w);
          u_xlat0.xyz = (u_xlat0.www * u_xlat0.xyz);
          out_v.texcoord1.xyz = u_xlat0.xyz;
          out_v.texcoord4 = float4(0, 0, 0, 0);
          u_xlat1.x = (u_xlat0.y * u_xlat0.y);
          u_xlat1.x = ((u_xlat0.x * u_xlat0.x) - u_xlat1.x);
          u_xlat2 = (u_xlat0.yzzx * u_xlat0.xyzz);
          u_xlat3.x = dot(unity_SHC, u_xlat2);
          u_xlat3.y = dot(unity_SHC, u_xlat2);
          u_xlat3.z = dot(unity_SHC, u_xlat2);
          u_xlat1.xyz = ((unity_SHC.xyz * u_xlat1.xxx) + u_xlat3.xyz);
          u_xlat0.w = 1;
          u_xlat2.x = dot(unity_SHC, u_xlat0);
          u_xlat2.y = dot(unity_SHC, u_xlat0);
          u_xlat2.z = dot(unity_SHC, u_xlat0);
          u_xlat0.xyz = (u_xlat1.xyz + u_xlat2.xyz);
          u_xlat0.xyz = max(u_xlat0.xyz, float3(0, 0, 0));
          u_xlat0.xyz = log(u_xlat0.xyz);
          u_xlat0.xyz = (u_xlat0.xyz * float3(0.416667, 0.416667, 0.416667));
          u_xlat0.xyz = exp(u_xlat0.xyzx);
          u_xlat0.xyz = ((u_xlat0.xyz * float3(1.055, 1.055, 1.055)) + float3(-0.055, (-0.055), (-0.055)));
          out_v.texcoord5.xyz = max(u_xlat0.xyz, float3(0, 0, 0));
          return out_v;
      }
      
      #define CODE_BLOCK_FRAGMENT
      float4 u_xlat0_d;
      float4 u_xlat1_d;
      OUT_Data_Frag frag(v2f in_f)
      {
          OUT_Data_Frag out_f;
          u_xlat0_d.x = (((-_Glossiness.y) * 0.779084) + 0.779084);
          u_xlat1_d.x = (_Glossiness.x * _Time.x);
          u_xlat1_d.y = 0;
          u_xlat0_d.yz = (u_xlat1_d.xy + in_f.texcoord.xy);
          u_xlat1_d = tex2D(_MainTex, u_xlat0_d.yz);
          u_xlat0_d.xyz = (u_xlat0_d.xxx * u_xlat1_d.xyz);
          out_f.color.xyz = u_xlat0_d.xyz;
          out_f.color3.xyz = ((in_f.texcoord5.xyz * u_xlat0_d.xyz) + u_xlat1_d.xyz);
          u_xlat0_d.xyz = (u_xlat1_d.xyz + float3(-0.220916, (-0.220916), (-0.220916)));
          out_f.color1.xyz = ((_Glossiness.yyy * u_xlat0_d.xyz) + float3(0.220916, 0.220916, 0.220916));
          out_f.color.w = 1;
          out_f.color1.w = _Glossiness.z;
          out_f.color2.xyz = ((in_f.texcoord1.xyz * float3(0.5, 0.5, 0.5)) + float3(0.5, 0.5, 0.5));
          out_f.color2.w = 1;
          out_f.color3.w = 1;
          return out_f;
      }
      
      
      ENDCG
      
    } // end phase
  }
  FallBack "Diffuse"
}
