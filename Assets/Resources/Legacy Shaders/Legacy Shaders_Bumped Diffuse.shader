// Upgrade NOTE: commented out 'float4 unity_ShadowFadeCenterAndType', a built-in variable

Shader "Legacy Shaders/Bumped Diffuse"
{
  Properties
  {
    _Color ("Main Color", Color) = (1,1,1,1)
    _MainTex ("Base (RGB)", 2D) = "white" {}
    _BumpMap ("Normalmap", 2D) = "bump" {}
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
      #define conv_mxt4x4_1(mat4x4) float4(mat4x4[0].y,mat4x4[1].y,mat4x4[2].y,mat4x4[3].y)
      #define conv_mxt4x4_2(mat4x4) float4(mat4x4[0].z,mat4x4[1].z,mat4x4[2].z,mat4x4[3].z)
      #define conv_mxt4x4_3(mat4x4) float4(mat4x4[0].w,mat4x4[1].w,mat4x4[2].w,mat4x4[3].w)
      
      
      #define CODE_BLOCK_VERTEX
      uniform float4 _MainTex_ST;
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
      uniform float4 _Color;
      //uniform float3 _WorldSpaceCameraPos;
      //uniform float4 _WorldSpaceLightPos0;
      //uniform float4 unity_OcclusionMaskSelector;
      //uniform float4 _LightShadowData;
      // uniform float4 unity_ShadowFadeCenterAndType;
      //uniform float4x4 unity_MatrixV;
      //uniform float4 unity_FogColor;
      //uniform float4 unity_FogParams;
      //uniform float4x4 unity_ProbeVolumeWorldToObject;
      //uniform float4 unity_ProbeVolumeParams;
      //uniform float3 unity_ProbeVolumeSizeInv;
      //uniform float3 unity_ProbeVolumeMin;
      uniform sampler2D _MainTex;
      uniform sampler2D _BumpMap;
      uniform sampler2D _ShadowMapTexture;
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
          float4 texcoord :TEXCOORD0;
          float4 texcoord1 :TEXCOORD1;
          float4 texcoord2 :TEXCOORD2;
          float4 texcoord3 :TEXCOORD3;
          float3 texcoord4 :TEXCOORD4;
          float texcoord5 :TEXCOORD5;
          float4 texcoord6 :TEXCOORD6;
          float4 texcoord7 :TEXCOORD7;
          uint SV_InstanceID :InstanceID;
      };
      
      struct v2f
      {
          float4 vertex :SV_POSITION;
          float4 texcoord :TEXCOORD0;
          float4 texcoord1 :TEXCOORD1;
          float4 texcoord2 :TEXCOORD2;
          float4 texcoord3 :TEXCOORD3;
          float3 texcoord4 :TEXCOORD4;
          float texcoord5 :TEXCOORD5;
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
          out_v.texcoord.zw = TRANSFORM_TEX(in_v.texcoord.xy, _BumpMap);
          u_xlat3.xyz = (in_v.tangent.yyy * conv_mxt4x4_(u_xlat0.y + (-10))(unity_WorldToObjectArray).yzx);
          u_xlat3.xyz = ((conv_mxt4x4_(u_xlat0.y + (-11))(unity_WorldToObjectArray).yzx * in_v.tangent.xxx) + u_xlat3.xyz);
          u_xlat3.xyz = ((conv_mxt4x4_(u_xlat0.y + (-9))(unity_WorldToObjectArray).yzx * in_v.tangent.zzz) + u_xlat3.xyz);
          u_xlat0.z = dot(u_xlat3.xyzx, u_xlat3.xyzx);
          u_xlat0.z = rsqrt(u_xlat0.z);
          u_xlat3.xyz = (u_xlat0.zzz * u_xlat3.xyz);
          u_xlat4.x = dot(in_v.normal.xyzx, conv_mxt4x4_(u_xlat0.y + (-7))(unity_WorldToObjectArray).xyzx);
          u_xlat4.y = dot(in_v.normal.xyzx, conv_mxt4x4_(u_xlat0.y + (-6))(unity_WorldToObjectArray).xyzx);
          u_xlat4.z = dot(in_v.normal.xyzx, conv_mxt4x4_(u_xlat0.y + (-5))(unity_WorldToObjectArray).xyzx);
          u_xlat0.y = dot(u_xlat4.xyzx, u_xlat4.xyzx);
          u_xlat0.y = rsqrt(u_xlat0.y);
          u_xlat4 = (u_xlat0.yyyy * u_xlat4.xyzz);
          u_xlat0.yzw = (u_xlat3.xyz * u_xlat4.wxy);
          u_xlat0.yzw = ((u_xlat4.ywx * u_xlat3.yzx) - u_xlat0.yzw);
          u_xlat1.w = (in_v.tangent.w * unity_WorldTransformParams.w);
          u_xlat0.yzw = (u_xlat0.yzw * u_xlat1.www);
          out_v.texcoord1.y = u_xlat0.y;
          out_v.texcoord1.x = u_xlat3.z;
          out_v.texcoord1.z = u_xlat4.x;
          out_v.texcoord1.w = u_xlat1.x;
          out_v.texcoord2.x = u_xlat3.x;
          out_v.texcoord3.x = u_xlat3.y;
          out_v.texcoord2.w = u_xlat1.y;
          out_v.texcoord2.y = u_xlat0.z;
          out_v.texcoord3.y = u_xlat0.w;
          out_v.texcoord2.z = u_xlat4.y;
          out_v.texcoord3.w = u_xlat1.z;
          out_v.texcoord3.z = u_xlat4.w;
          u_xlat0.y = (u_xlat4.y * u_xlat4.y);
          u_xlat0.y = ((u_xlat4.x * u_xlat4.x) - u_xlat0.y);
          u_xlat3 = (u_xlat4.ywzx * u_xlat4);
          u_xlat5.x = dot(unity_SHCArray, u_xlat3);
          u_xlat5.y = dot(unity_SHCArray, u_xlat3);
          u_xlat5.z = dot(unity_SHCArray, u_xlat3);
          u_xlat0.xyz = ((unity_SHCArray.xyz * u_xlat0.yyy) + u_xlat5.xyz);
          u_xlat3 = ((-u_xlat1.yyyy) + unity_LightColor);
          u_xlat5 = (u_xlat4.yyyy * u_xlat3);
          u_xlat3 = (u_xlat3 * u_xlat3);
          u_xlat6 = ((-u_xlat1.xxxx) + unity_LightColor);
          u_xlat1 = ((-u_xlat1.zzzz) + unity_LightColor);
          u_xlat5 = ((u_xlat6 * u_xlat4.xxxx) + u_xlat5);
          u_xlat4 = ((u_xlat1 * u_xlat4.wwzw) + u_xlat5);
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
          u_xlat3.xyz = ((u_xlat1.xyz * float3(0.305306, 0.305306, 0.305306)) + float3(0.682171, 0.682171, 0.682171));
          u_xlat3.xyz = ((u_xlat1.xyz * u_xlat3.xyz) + float3(0.012523, 0.012523, 0.012523));
          out_v.texcoord4.xyz = ((u_xlat1.xyz * u_xlat3.xyz) + u_xlat0.xyz);
          out_v.texcoord5.x = u_xlat2.z;
          u_xlat0.x = (u_xlat2.y * _ProjectionParams.x);
          u_xlat0.w = (u_xlat0.x * 0.5);
          u_xlat0.xz = (u_xlat2.xw * float2(0.5, 0.5));
          out_v.texcoord6.zw = u_xlat2.zw;
          out_v.texcoord6.xy = (u_xlat0.zz + u_xlat0.xw);
          out_v.texcoord7 = float4(0, 0, 0, 0);
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
      OUT_Data_Frag frag(v2f in_f)
      {
          OUT_Data_Frag out_f;
          u_xlat0_d.x = (in_f.SV_InstanceID.x + unity_BaseInstanceID.x);
          u_xlat1_d = tex2D(_MainTex, in_f.texcoord.xy);
          u_xlat0_d.yzw = (u_xlat1_d.xyz * _Color.xyz);
          u_xlat1_d = tex2D(_BumpMap, in_f.texcoord.zw);
          u_xlat1_d.x = (u_xlat1_d.w * u_xlat1_d.x);
          u_xlat1_d.xy = ((u_xlat1_d.xy * float2(2, 2)) + float2(-1, (-1)));
          u_xlat1_d.w = dot(u_xlat1_d.xyxx, u_xlat1_d.xyxx);
          u_xlat1_d.w = min(u_xlat1_d.w, 1);
          u_xlat1_d.w = ((-u_xlat1_d.w) + 1);
          u_xlat1_d.z = sqrt(u_xlat1_d.w);
          u_xlat2_d.y = in_f.texcoord1.w;
          u_xlat2_d.z = in_f.texcoord2.w;
          u_xlat2_d.w = in_f.texcoord3.w;
          u_xlat3_d.xyz = ((-u_xlat2_d.yzw) + _ProjectionParams.xyz);
          u_xlat4_d.x = conv_mxt4x4_1(unity_MatrixV).z;
          u_xlat4_d.y = conv_mxt4x4_2(unity_MatrixV).z;
          u_xlat4_d.z = conv_mxt4x4_3(unity_MatrixV).z;
          u_xlat1_d.w = dot(u_xlat3_d.xyzx, u_xlat4_d.xyzx);
          u_xlat3_d.xyz = (u_xlat2_d.yzw - unity_ShadowFadeCenterAndType.xyz);
          u_xlat2_d.x = length(u_xlat3_d.xyzx);
          u_xlat2_d.x = ((-u_xlat1_d.w) + u_xlat2_d.x);
          u_xlat1_d.w = ((unity_ShadowFadeCenterAndType.w * u_xlat2_d.x) + u_xlat1_d.w);
          u_xlat1_d.w = saturate(((u_xlat1_d.w * unity_ShadowFadeCenterAndType.z) + unity_ShadowFadeCenterAndType.w));
          u_xlat0_d.x = ((u_xlat0_d.x * 7) & -1);
          u_xlat2_d.x = (unity_ProbeVolumeMin.x==1);
          if((u_xlat2_d.x!=0))
          {
              u_xlat3_d.x = (unity_ProbeVolumeMin.y==1);
              u_xlat3_d.yzw = (in_f.texcoord2.www * unity_ProbeVolumeMin.xyz);
              u_xlat3_d.yzw = ((unity_ProbeVolumeMin.xyz * in_f.texcoord1.www) + u_xlat3_d.yzw);
              u_xlat3_d.yzw = ((unity_ProbeVolumeMin.xyz * in_f.texcoord3.www) + u_xlat3_d.yzw);
              u_xlat3_d.yzw = (u_xlat3_d.yzw + unity_ProbeVolumeMin.xyz);
              u_xlat3_d.xyz = (u_xlat3_d.xxx)?(u_xlat3_d.yzw):(u_xlat2_d.yzw);
              u_xlat3_d.xyz = (u_xlat3_d.xyz - unity_ProbeVolumeMin.xyz);
              u_xlat3_d.yzw = (u_xlat3_d.xyz * unity_ProbeVolumeMin.xyz);
              u_xlat3_d.y = ((u_xlat3_d.y * 0.25) + 0.75);
              u_xlat4_d.x = ((unity_ProbeVolumeMin.z * 0.5) + 0.75);
              u_xlat3_d.x = max(u_xlat3_d.y, u_xlat4_d.x);
              u_xlat3_d = tex3D(unity_ProbeVolumeSH, u_xlat3_d.xz);
          }
          else
          {
              u_xlat3_d = unity_SHCArray.wwww;
          }
          u_xlat3_d.x = saturate(dot(u_xlat3_d, unity_OcclusionMaskSelector));
          u_xlat3_d.yz = (in_f.texcoord6.xy / in_f.texcoord6.ww);
          u_xlat4_d = tex2D(_ShadowMapTexture, u_xlat3_d.yz);
          u_xlat3_d.x = (u_xlat3_d.x - u_xlat4_d.x);
          u_xlat1_d.w = ((u_xlat1_d.w * u_xlat3_d.x) + u_xlat4_d.x);
          u_xlat3_d.x = dot(in_f.texcoord1.xyzx, u_xlat1_d.xyzx);
          u_xlat3_d.y = dot(in_f.texcoord2.xyzx, u_xlat1_d.xyzx);
          u_xlat3_d.z = dot(in_f.texcoord3.xyzx, u_xlat1_d.xyzx);
          u_xlat1_d.x = dot(u_xlat3_d.xyzx, u_xlat3_d.xyzx);
          u_xlat1_d.x = rsqrt(u_xlat1_d.x);
          u_xlat3_d.xyz = (u_xlat1_d.xxx * u_xlat3_d.xyz);
          u_xlat1_d.xyz = (u_xlat1_d.www * _Color.xyz);
          if((u_xlat2_d.x!=0))
          {
              u_xlat1_d.w = (unity_ProbeVolumeMin.y==1);
              u_xlat4_d.xyz = (in_f.texcoord2.www * unity_ProbeVolumeMin.xyz);
              u_xlat4_d.xyz = ((unity_ProbeVolumeMin.xyz * in_f.texcoord1.www) + u_xlat4_d.xyz);
              u_xlat4_d.xyz = ((unity_ProbeVolumeMin.xyz * in_f.texcoord3.www) + u_xlat4_d.xyz);
              u_xlat4_d.xyz = (u_xlat4_d.xyz + unity_ProbeVolumeMin.xyz);
              u_xlat2_d.xyz = (u_xlat1_d.www)?(u_xlat4_d.xyz):(u_xlat2_d.yzw);
              u_xlat2_d.xyz = (u_xlat2_d.xyz - unity_ProbeVolumeMin.xyz);
              u_xlat2_d.yzw = (u_xlat2_d.xyz * unity_ProbeVolumeMin.xyz);
              u_xlat1_d.w = (u_xlat2_d.y * 0.25);
              u_xlat2_d.y = (unity_ProbeVolumeMin.z * 0.5);
              u_xlat4_d.x = (((-unity_ProbeVolumeMin.z) * 0.5) + 0.25);
              u_xlat1_d.w = max(u_xlat1_d.w, u_xlat2_d.y);
              u_xlat2_d.x = min(u_xlat4_d.x, u_xlat1_d.w);
              u_xlat4_d = tex3D(unity_ProbeVolumeSH, u_xlat2_d.xz);
              u_xlat5_d.xyz = (u_xlat2_d.xzw + float3(0.25, 0, 0));
              u_xlat5_d = tex3D(unity_ProbeVolumeSH, u_xlat5_d.xy);
              u_xlat2_d.xyz = (u_xlat2_d.xzw + float3(0.5, 0, 0));
              u_xlat2_d = tex3D(unity_ProbeVolumeSH, u_xlat2_d.xy);
              u_xlat3_d.w = 1;
              u_xlat4_d.x = dot(u_xlat4_d, u_xlat3_d);
              u_xlat4_d.y = dot(u_xlat5_d, u_xlat3_d);
              u_xlat4_d.z = dot(u_xlat2_d, u_xlat3_d);
          }
          else
          {
              u_xlat3_d.w = 1;
              u_xlat4_d.x = dot(unity_SHCArray, u_xlat3_d);
              u_xlat4_d.y = dot(unity_SHCArray, u_xlat3_d);
              u_xlat4_d.z = dot(unity_SHCArray, u_xlat3_d);
          }
          u_xlat2_d.xyz = (u_xlat4_d.xyz + in_f.texcoord4.xyz);
          u_xlat2_d.xyz = max(u_xlat2_d.xyz, float3(0, 0, 0));
          u_xlat2_d.xyz = log(u_xlat2_d.xyz);
          u_xlat2_d.xyz = (u_xlat2_d.xyz * float3(0.416667, 0.416667, 0.416667));
          u_xlat2_d.xyz = exp(u_xlat2_d.xyzx);
          u_xlat2_d.xyz = ((u_xlat2_d.xyz * float3(1.055, 1.055, 1.055)) + float3(-0.055, (-0.055), (-0.055)));
          u_xlat2_d.xyz = max(u_xlat2_d.xyz, float3(0, 0, 0));
          u_xlat0_d.x = dot(u_xlat3_d.xyzx, unity_OcclusionMaskSelector.xyzx);
          u_xlat0_d.x = max(u_xlat0_d.x, 0);
          u_xlat1_d.xyz = (u_xlat0_d.yzw * u_xlat1_d.xyz);
          u_xlat0_d.yzw = (u_xlat0_d.yzw * u_xlat2_d.xyz);
          u_xlat0_d.xyz = ((u_xlat1_d.xyz * u_xlat0_d.xxx) + u_xlat0_d.yzw);
          u_xlat0_d.w = (in_f.texcoord5.x / _ProjectionParams.y);
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
      
      
      #define CODE_BLOCK_VERTEX
      uniform float4x4 unity_WorldToLight;
      uniform float4 _MainTex_ST;
      uniform float4 _BumpMap_ST;
      //uniform float4 _ProjectionParams;
      //uniform float4x4 unity_ObjectToWorld;
      //uniform float4x4 unity_WorldToObject;
      //uniform float4 unity_WorldTransformParams;
      //uniform float4x4 unity_MatrixVP;
      //uniform float4 unity_FogParams;
      uniform float4 _LightColor0;
      uniform float4 _Color;
      //uniform float4 _WorldSpaceLightPos0;
      //uniform float4 unity_OcclusionMaskSelector;
      //uniform float4x4 unity_ProbeVolumeWorldToObject;
      //uniform float4 unity_ProbeVolumeParams;
      //uniform float3 unity_ProbeVolumeSizeInv;
      //uniform float3 unity_ProbeVolumeMin;
      uniform sampler2D _MainTex;
      uniform sampler2D _BumpMap;
      uniform sampler2D _LightTexture0;
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
          float4 texcoord :TEXCOORD0;
          float3 texcoord1 :TEXCOORD1;
          float3 texcoord2 :TEXCOORD2;
          float3 texcoord3 :TEXCOORD3;
          float3 texcoord4 :TEXCOORD4;
          float2 texcoord5 :TEXCOORD5;
          float texcoord7 :TEXCOORD7;
          float4 texcoord6 :TEXCOORD6;
      };
      
      struct v2f
      {
          float4 vertex :SV_POSITION;
          float4 texcoord :TEXCOORD0;
          float3 texcoord1 :TEXCOORD1;
          float3 texcoord2 :TEXCOORD2;
          float3 texcoord3 :TEXCOORD3;
          float3 texcoord4 :TEXCOORD4;
          float2 texcoord5 :TEXCOORD5;
          float texcoord7 :TEXCOORD7;
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
      OUT_Data_Vert vert(appdata_t in_v)
      {
          OUT_Data_Vert out_v;
          u_xlat0 = (in_v.vertex.yyyy * unity_WorldTransformParams);
          u_xlat0 = ((unity_WorldTransformParams * in_v.vertex.xxxx) + u_xlat0);
          u_xlat0 = ((unity_WorldTransformParams * in_v.vertex.zzzz) + u_xlat0);
          u_xlat1 = (u_xlat0 + unity_WorldTransformParams);
          u_xlat1 = mul(unity_MatrixVP, u_xlat1);
          out_v.vertex = u_xlat1;
          u_xlat1.x = (u_xlat1.z / _ProjectionParams.y);
          u_xlat1.x = ((-u_xlat1.x) + 1);
          u_xlat1.x = (u_xlat1.x * _ProjectionParams.z);
          u_xlat1.x = max(u_xlat1.x, 0);
          u_xlat1.x = (u_xlat1.x * unity_FogParams.y);
          out_v.texcoord7.x = exp((-u_xlat1.x));
          out_v.texcoord.xy = TRANSFORM_TEX(in_v.texcoord.xy, _BumpMap);
          out_v.texcoord.zw = TRANSFORM_TEX(in_v.texcoord.xy, _BumpMap);
          u_xlat1.y = dot(in_v.normal.xyzx, unity_WorldTransformParams.xyzx);
          u_xlat1.z = dot(in_v.normal.xyzx, unity_WorldTransformParams.xyzx);
          u_xlat1.x = dot(in_v.normal.xyzx, unity_WorldTransformParams.xyzx);
          u_xlat1.w = dot(u_xlat1.xyzx, u_xlat1.xyzx);
          u_xlat1.w = rsqrt(u_xlat1.w);
          u_xlat1.xyz = (u_xlat1.www * u_xlat1.xyz);
          u_xlat2.xyz = (in_v.tangent.yyy * unity_WorldTransformParams.yzx);
          u_xlat2.xyz = ((unity_WorldTransformParams.yzx * in_v.tangent.xxx) + u_xlat2.xyz);
          u_xlat2.xyz = ((unity_WorldTransformParams.yzx * in_v.tangent.zzz) + u_xlat2.xyz);
          u_xlat1.w = dot(u_xlat2.xyzx, u_xlat2.xyzx);
          u_xlat1.w = rsqrt(u_xlat1.w);
          u_xlat2.xyz = (u_xlat1.www * u_xlat2.xyz);
          u_xlat3.xyz = (u_xlat1.xyz * u_xlat2.xyz);
          u_xlat3.xyz = ((u_xlat1.zxy * u_xlat2.yzx) - u_xlat3.xyz);
          u_xlat1.w = (in_v.tangent.w * unity_WorldTransformParams.w);
          u_xlat3.xyz = (u_xlat1.www * u_xlat3.xyz);
          out_v.texcoord1.y = u_xlat3.x;
          out_v.texcoord1.x = u_xlat2.z;
          out_v.texcoord1.z = u_xlat1.y;
          out_v.texcoord2.x = u_xlat2.x;
          out_v.texcoord3.x = u_xlat2.y;
          out_v.texcoord2.z = u_xlat1.z;
          out_v.texcoord3.z = u_xlat1.x;
          out_v.texcoord2.y = u_xlat3.y;
          out_v.texcoord3.y = u_xlat3.z;
          out_v.texcoord4.xyz = ((unity_WorldTransformParams.xyz * in_v.vertex.www) + u_xlat0.xyz);
          u_xlat0 = ((unity_WorldTransformParams * in_v.vertex.wwww) + u_xlat0);
          u_xlat1.xy = (u_xlat0.yy * _BumpMap_ST.xy);
          u_xlat0.xy = ((_BumpMap_ST.xy * u_xlat0.xx) + u_xlat1.xy);
          u_xlat0.xy = ((_BumpMap_ST.xy * u_xlat0.zz) + u_xlat0.xy);
          out_v.texcoord5.xy = ((_BumpMap_ST.xy * u_xlat0.ww) + u_xlat0.xy);
          out_v.texcoord6 = float4(0, 0, 0, 0);
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
          u_xlat0_d = tex2D(_MainTex, in_f.texcoord.xy);
          u_xlat0_d.xyz = (u_xlat0_d.xyz * _Color.xyz);
          u_xlat1_d = tex2D(_BumpMap, in_f.texcoord.zw);
          u_xlat1_d.x = (u_xlat1_d.w * u_xlat1_d.x);
          u_xlat1_d.xy = ((u_xlat1_d.xy * float2(2, 2)) + float2(-1, (-1)));
          u_xlat0_d.w = dot(u_xlat1_d.xyxx, u_xlat1_d.xyxx);
          u_xlat0_d.w = min(u_xlat0_d.w, 1);
          u_xlat0_d.w = ((-u_xlat0_d.w) + 1);
          u_xlat1_d.z = sqrt(u_xlat0_d.w);
          u_xlat2_d.xy = (in_f.texcoord4.yy * _Color.xy);
          u_xlat2_d.xy = ((_Color.xy * in_f.texcoord4.xx) + u_xlat2_d.xy);
          u_xlat2_d.xy = ((_Color.xy * in_f.texcoord4.zz) + u_xlat2_d.xy);
          u_xlat2_d.xy = (u_xlat2_d.xy + _Color.xy);
          u_xlat0_d.w = (unity_ProbeVolumeMin.x==1);
          if((u_xlat0_d.w!=0))
          {
              u_xlat0_d.w = (unity_ProbeVolumeMin.y==1);
              u_xlat3_d.xyz = (in_f.texcoord4.yyy * unity_ProbeVolumeMin.xyz);
              u_xlat3_d.xyz = ((unity_ProbeVolumeMin.xyz * in_f.texcoord4.xxx) + u_xlat3_d.xyz);
              u_xlat3_d.xyz = ((unity_ProbeVolumeMin.xyz * in_f.texcoord4.zzz) + u_xlat3_d.xyz);
              u_xlat3_d.xyz = (u_xlat3_d.xyz + unity_ProbeVolumeMin.xyz);
              u_xlat3_d.xyz = (u_xlat0_d.www)?(u_xlat3_d.xyz):(in_f.texcoord4.xyz);
              u_xlat3_d.xyz = (u_xlat3_d.xyz - unity_ProbeVolumeMin.xyz);
              u_xlat3_d.yzw = (u_xlat3_d.xyz * unity_ProbeVolumeMin.xyz);
              u_xlat0_d.w = ((u_xlat3_d.y * 0.25) + 0.75);
              u_xlat1_d.w = ((unity_ProbeVolumeMin.z * 0.5) + 0.75);
              u_xlat3_d.x = max(u_xlat0_d.w, u_xlat1_d.w);
              u_xlat3_d = tex3D(unity_ProbeVolumeSH, u_xlat3_d.xz);
          }
          else
          {
              u_xlat3_d = float4(1, 1, 1, 1);
          }
          u_xlat0_d.w = saturate(dot(u_xlat3_d, unity_OcclusionMaskSelector));
          u_xlat2_d = tex2D(_LightTexture0, u_xlat2_d.xy);
          u_xlat0_d.w = (u_xlat0_d.w * u_xlat2_d.w);
          u_xlat2_d.x = dot(in_f.texcoord1.xyzx, u_xlat1_d.xyzx);
          u_xlat2_d.y = dot(in_f.texcoord2.xyzx, u_xlat1_d.xyzx);
          u_xlat2_d.z = dot(in_f.texcoord3.xyzx, u_xlat1_d.xyzx);
          u_xlat1_d.x = dot(u_xlat2_d.xyzx, u_xlat2_d.xyzx);
          u_xlat1_d.x = rsqrt(u_xlat1_d.x);
          u_xlat1_d.xyz = (u_xlat1_d.xxx * u_xlat2_d.xyz);
          u_xlat2_d.xyz = (u_xlat0_d.www * _Color.xyz);
          u_xlat0_d.w = dot(u_xlat1_d.xyzx, unity_OcclusionMaskSelector.xyzx);
          u_xlat0_d.w = max(u_xlat0_d.w, 0);
          u_xlat0_d.xyz = (u_xlat0_d.xyz * u_xlat2_d.xyz);
          u_xlat0_d.xyz = (u_xlat0_d.www * u_xlat0_d.xyz);
          u_xlat0_d.w = saturate(in_f.texcoord7.x);
          out_f.color.xyz = (u_xlat0_d.xyz * u_xlat0_d.www);
          out_f.color.w = 1;
          return out_f;
      }
      
      
      ENDCG
      
    } // end phase
    Pass // ind: 3, name: PREPASS
    {
      Name "PREPASS"
      Tags
      { 
        "LIGHTMODE" = "PREPASSBASE"
        "RenderType" = "Opaque"
      }
      LOD 300
      // m_ProgramMask = 6
      CGPROGRAM
      //#pragma target 4.0
      
      #pragma vertex vert
      #pragma fragment frag
      
      #include "UnityCG.cginc"
      
      
      #define CODE_BLOCK_VERTEX
      uniform float4 _BumpMap_ST;
      //uniform float4 unity_WorldTransformParams;
      //uniform float4x4 unity_MatrixVP;
      uniform int unity_BaseInstanceID;
      //uniform float4x4 unity_ObjectToWorldArray;
      //uniform float4x4 unity_WorldToObjectArray;
      uniform sampler2D _BumpMap;
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
          float4 texcoord1 :TEXCOORD1;
          float4 texcoord2 :TEXCOORD2;
          float4 texcoord3 :TEXCOORD3;
          uint SV_InstanceID :InstanceID;
      };
      
      struct v2f
      {
          float4 vertex :SV_POSITION;
          float2 texcoord :TEXCOORD0;
          float4 texcoord1 :TEXCOORD1;
          float4 texcoord2 :TEXCOORD2;
          float4 texcoord3 :TEXCOORD3;
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
      OUT_Data_Vert vert(appdata_t in_v)
      {
          OUT_Data_Vert out_v;
          u_xlat0.x = (in_v.SV_InstanceID.x + unity_BaseInstanceID.x);
          u_xlat0.x = (u_xlat0.x >> 3);
          u_xlat1 = (in_v.vertex.yyyy * conv_mxt4x4_(u_xlat0.x + (-10))(unity_WorldToObjectArray));
          u_xlat1 = ((conv_mxt4x4_(u_xlat0.x + (-11))(unity_WorldToObjectArray) * in_v.vertex.xxxx) + u_xlat1);
          u_xlat1 = ((conv_mxt4x4_(u_xlat0.x + (-9))(unity_WorldToObjectArray) * in_v.vertex.zzzz) + u_xlat1);
          u_xlat2 = (u_xlat1 + conv_mxt4x4_(u_xlat0.x + (-8))(unity_WorldToObjectArray));
          u_xlat0.yzw = ((conv_mxt4x4_(u_xlat0.x + (-8))(unity_WorldToObjectArray).xyz * in_v.vertex.www) + u_xlat1.xyz);
          out_v.vertex = mul(unity_MatrixVP, u_xlat2);
          out_v.texcoord.xy = TRANSFORM_TEX(in_v.texcoord.xy, _BumpMap);
          u_xlat1.y = dot(in_v.normal.xyzx, conv_mxt4x4_(u_xlat0.x + (-7))(unity_WorldToObjectArray).xyzx);
          u_xlat1.z = dot(in_v.normal.xyzx, conv_mxt4x4_(u_xlat0.x + (-6))(unity_WorldToObjectArray).xyzx);
          u_xlat1.x = dot(in_v.normal.xyzx, conv_mxt4x4_(u_xlat0.x + (-5))(unity_WorldToObjectArray).xyzx);
          u_xlat1.w = dot(u_xlat1.xyzx, u_xlat1.xyzx);
          u_xlat1.w = rsqrt(u_xlat1.w);
          u_xlat1.xyz = (u_xlat1.www * u_xlat1.xyz);
          u_xlat2.xyz = (in_v.tangent.yyy * conv_mxt4x4_(u_xlat0.x + (-10))(unity_WorldToObjectArray).yzx);
          u_xlat2.xyz = ((conv_mxt4x4_(u_xlat0.x + (-11))(unity_WorldToObjectArray).yzx * in_v.tangent.xxx) + u_xlat2.xyz);
          u_xlat2.xyz = ((conv_mxt4x4_(u_xlat0.x + (-9))(unity_WorldToObjectArray).yzx * in_v.tangent.zzz) + u_xlat2.xyz);
          u_xlat0.x = dot(u_xlat2.xyzx, u_xlat2.xyzx);
          u_xlat0.x = rsqrt(u_xlat0.x);
          u_xlat2.xyz = (u_xlat0.xxx * u_xlat2.xyz);
          u_xlat3.xyz = (u_xlat1.xyz * u_xlat2.xyz);
          u_xlat3.xyz = ((u_xlat1.zxy * u_xlat2.yzx) - u_xlat3.xyz);
          u_xlat0.x = (in_v.tangent.w * unity_WorldTransformParams.w);
          u_xlat3.xyz = (u_xlat0.xxx * u_xlat3.xyz);
          out_v.texcoord1.y = u_xlat3.x;
          out_v.texcoord1.x = u_xlat2.z;
          out_v.texcoord1.z = u_xlat1.y;
          out_v.texcoord1.w = u_xlat0.y;
          out_v.texcoord2.x = u_xlat2.x;
          out_v.texcoord3.x = u_xlat2.y;
          out_v.texcoord2.z = u_xlat1.z;
          out_v.texcoord3.z = u_xlat1.x;
          out_v.texcoord2.w = u_xlat0.z;
          out_v.texcoord3.w = u_xlat0.w;
          out_v.texcoord2.y = u_xlat3.y;
          out_v.texcoord3.y = u_xlat3.z;
          in_v.SV_InstanceID.x = in_v.SV_InstanceID.x;
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
          out_f.color.xyz = ((u_xlat0_d.xyz * float3(0.5, 0.5, 0.5)) + float3(0.5, 0.5, 0.5));
          out_f.color.w = 0;
          return out_f;
      }
      
      
      ENDCG
      
    } // end phase
    Pass // ind: 4, name: PREPASS
    {
      Name "PREPASS"
      Tags
      { 
        "LIGHTMODE" = "PREPASSFINAL"
        "RenderType" = "Opaque"
      }
      LOD 300
      ZWrite Off
      // m_ProgramMask = 6
      CGPROGRAM
      //#pragma target 4.0
      
      #pragma vertex vert
      #pragma fragment frag
      
      #include "UnityCG.cginc"
      
      
      #define CODE_BLOCK_VERTEX
      uniform float4 _MainTex_ST;
      //uniform float4 _ProjectionParams;
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
      uniform float4 _Color;
      //uniform float4 unity_FogColor;
      //uniform float4 unity_FogParams;
      uniform sampler2D _MainTex;
      uniform sampler2D _LightBuffer;
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
          float3 texcoord1 :TEXCOORD1;
          float4 texcoord2 :TEXCOORD2;
          float4 texcoord3 :TEXCOORD3;
          float3 texcoord4 :TEXCOORD4;
          uint SV_InstanceID :InstanceID;
      };
      
      struct v2f
      {
          float4 vertex :SV_POSITION;
          float2 texcoord :TEXCOORD0;
          float texcoord5 :TEXCOORD5;
          float3 texcoord1 :TEXCOORD1;
          float4 texcoord2 :TEXCOORD2;
          float4 texcoord3 :TEXCOORD3;
          float3 texcoord4 :TEXCOORD4;
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
          out_v.texcoord1.xyz = ((conv_mxt4x4_(u_xlat0.y + (-8))(unity_WorldToObjectArray).xyz * in_v.vertex.www) + u_xlat1.xyz);
          u_xlat1 = mul(unity_MatrixVP, u_xlat2);
          out_v.vertex = u_xlat1;
          out_v.texcoord5.x = u_xlat1.z;
          out_v.texcoord2.zw = u_xlat1.zw;
          out_v.texcoord.xy = TRANSFORM_TEX(in_v.texcoord.xy, _MainTex);
          u_xlat0.z = (u_xlat1.y * _ProjectionParams.x);
          u_xlat1.xz = (u_xlat1.xw * float2(0.5, 0.5));
          u_xlat1.w = (u_xlat0.z * 0.5);
          out_v.texcoord2.xy = (u_xlat1.zz + u_xlat1.xw);
          out_v.texcoord3 = float4(0, 0, 0, 0);
          u_xlat1.x = dot(in_v.normal.xyzx, conv_mxt4x4_(u_xlat0.y + (-7))(unity_WorldToObjectArray).xyzx);
          u_xlat1.y = dot(in_v.normal.xyzx, conv_mxt4x4_(u_xlat0.y + (-6))(unity_WorldToObjectArray).xyzx);
          u_xlat1.z = dot(in_v.normal.xyzx, conv_mxt4x4_(u_xlat0.y + (-5))(unity_WorldToObjectArray).xyzx);
          u_xlat0.y = dot(u_xlat1.xyzx, u_xlat1.xyzx);
          u_xlat0.y = rsqrt(u_xlat0.y);
          u_xlat1.xyz = (u_xlat0.yyy * u_xlat1.xyz);
          u_xlat0.y = (u_xlat1.y * u_xlat1.y);
          u_xlat0.y = ((u_xlat1.x * u_xlat1.x) - u_xlat0.y);
          u_xlat2 = (u_xlat1.yzzx * u_xlat1.xyzz);
          u_xlat3.x = dot(unity_SHCArray, u_xlat2);
          u_xlat3.y = dot(unity_SHCArray, u_xlat2);
          u_xlat3.z = dot(unity_SHCArray, u_xlat2);
          u_xlat0.yzw = ((unity_SHCArray.xyz * u_xlat0.yyy) + u_xlat3.xyz);
          u_xlat1.w = 1;
          u_xlat2.x = dot(unity_SHCArray, u_xlat1);
          u_xlat2.y = dot(unity_SHCArray, u_xlat1);
          u_xlat2.z = dot(unity_SHCArray, u_xlat1);
          u_xlat0.xyz = (u_xlat0.yzw + u_xlat2.xyz);
          u_xlat0.xyz = max(u_xlat0.xyz, float3(0, 0, 0));
          u_xlat0.xyz = log(u_xlat0.xyz);
          u_xlat0.xyz = (u_xlat0.xyz * float3(0.416667, 0.416667, 0.416667));
          u_xlat0.xyz = exp(u_xlat0.xyzx);
          u_xlat0.xyz = ((u_xlat0.xyz * float3(1.055, 1.055, 1.055)) + float3(-0.055, (-0.055), (-0.055)));
          out_v.texcoord4.xyz = max(u_xlat0.xyz, float3(0, 0, 0));
          in_v.SV_InstanceID.x = in_v.SV_InstanceID.x;
          return out_v;
      }
      
      #define CODE_BLOCK_FRAGMENT
      float4 u_xlat0_d;
      float4 u_xlat1_d;
      OUT_Data_Frag frag(v2f in_f)
      {
          OUT_Data_Frag out_f;
          u_xlat0_d.x = (in_f.texcoord5.x / _ProjectionParams.y);
          u_xlat0_d.x = ((-u_xlat0_d.x) + 1);
          u_xlat0_d.x = (u_xlat0_d.x * _ProjectionParams.z);
          u_xlat0_d.x = max(u_xlat0_d.x, 0);
          u_xlat0_d.x = (u_xlat0_d.x * unity_FogParams.y);
          u_xlat0_d.x = exp((-u_xlat0_d.x));
          u_xlat0_d.x = min(u_xlat0_d.x, 1);
          u_xlat0_d.yz = (in_f.texcoord2.xy / in_f.texcoord2.ww);
          u_xlat1_d = tex2D(_LightBuffer, u_xlat0_d.yz);
          u_xlat0_d.yzw = (u_xlat1_d.xyz + in_f.texcoord4.xyz);
          u_xlat1_d = tex2D(_MainTex, in_f.texcoord.xy);
          u_xlat1_d.xyz = (u_xlat1_d.xyz * _Color.xyz);
          u_xlat0_d.yzw = ((u_xlat1_d.xyz * u_xlat0_d.yzw) - unity_FogParams.xyz);
          out_f.color.xyz = ((u_xlat0_d.xxx * u_xlat0_d.yzw) + unity_FogParams.xyz);
          out_f.color.w = 1;
          return out_f;
      }
      
      
      ENDCG
      
    } // end phase
    Pass // ind: 5, name: DEFERRED
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
      
      
      #define CODE_BLOCK_VERTEX
      uniform float4 _MainTex_ST;
      uniform float4 _BumpMap_ST;
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
      uniform float4 _Color;
      //uniform float4x4 unity_ProbeVolumeWorldToObject;
      //uniform float4 unity_ProbeVolumeParams;
      //uniform float3 unity_ProbeVolumeSizeInv;
      //uniform float3 unity_ProbeVolumeMin;
      uniform sampler2D _MainTex;
      uniform sampler2D _BumpMap;
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
          float4 texcoord :TEXCOORD0;
          float4 texcoord1 :TEXCOORD1;
          float4 texcoord2 :TEXCOORD2;
          float4 texcoord3 :TEXCOORD3;
          float4 texcoord4 :TEXCOORD4;
          float3 texcoord5 :TEXCOORD5;
          uint SV_InstanceID :InstanceID;
      };
      
      struct v2f
      {
          float4 vertex :SV_POSITION;
          float4 texcoord :TEXCOORD0;
          float4 texcoord1 :TEXCOORD1;
          float4 texcoord2 :TEXCOORD2;
          float4 texcoord3 :TEXCOORD3;
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
          u_xlat1.xyz = ((conv_mxt4x4_(u_xlat0.y + (-8))(unity_WorldToObjectArray).xyz * in_v.vertex.www) + u_xlat1.xyz);
          out_v.vertex = mul(unity_MatrixVP, u_xlat2);
          out_v.texcoord.xy = TRANSFORM_TEX(in_v.texcoord.xy, _BumpMap);
          out_v.texcoord.zw = TRANSFORM_TEX(in_v.texcoord.xy, _BumpMap);
          u_xlat2.xyz = (in_v.tangent.yyy * conv_mxt4x4_(u_xlat0.y + (-10))(unity_WorldToObjectArray).yzx);
          u_xlat2.xyz = ((conv_mxt4x4_(u_xlat0.y + (-11))(unity_WorldToObjectArray).yzx * in_v.tangent.xxx) + u_xlat2.xyz);
          u_xlat2.xyz = ((conv_mxt4x4_(u_xlat0.y + (-9))(unity_WorldToObjectArray).yzx * in_v.tangent.zzz) + u_xlat2.xyz);
          u_xlat0.z = dot(u_xlat2.xyzx, u_xlat2.xyzx);
          u_xlat0.z = rsqrt(u_xlat0.z);
          u_xlat2.xyz = (u_xlat0.zzz * u_xlat2.xyz);
          u_xlat3.x = dot(in_v.normal.xyzx, conv_mxt4x4_(u_xlat0.y + (-7))(unity_WorldToObjectArray).xyzx);
          u_xlat3.y = dot(in_v.normal.xyzx, conv_mxt4x4_(u_xlat0.y + (-6))(unity_WorldToObjectArray).xyzx);
          u_xlat3.z = dot(in_v.normal.xyzx, conv_mxt4x4_(u_xlat0.y + (-5))(unity_WorldToObjectArray).xyzx);
          u_xlat0.y = dot(u_xlat3.xyzx, u_xlat3.xyzx);
          u_xlat0.y = rsqrt(u_xlat0.y);
          u_xlat3 = (u_xlat0.yyyy * u_xlat3.xyzz);
          u_xlat0.yzw = (u_xlat2.xyz * u_xlat3.wxy);
          u_xlat0.yzw = ((u_xlat3.ywx * u_xlat2.yzx) - u_xlat0.yzw);
          u_xlat1.w = (in_v.tangent.w * unity_WorldTransformParams.w);
          u_xlat0.yzw = (u_xlat0.yzw * u_xlat1.www);
          out_v.texcoord1.y = u_xlat0.y;
          out_v.texcoord1.x = u_xlat2.z;
          out_v.texcoord1.z = u_xlat3.x;
          out_v.texcoord1.w = u_xlat1.x;
          out_v.texcoord2.x = u_xlat2.x;
          out_v.texcoord3.x = u_xlat2.y;
          out_v.texcoord2.w = u_xlat1.y;
          out_v.texcoord3.w = u_xlat1.z;
          out_v.texcoord2.y = u_xlat0.z;
          out_v.texcoord3.y = u_xlat0.w;
          out_v.texcoord2.z = u_xlat3.y;
          out_v.texcoord3.z = u_xlat3.w;
          out_v.texcoord4 = float4(0, 0, 0, 0);
          u_xlat0.y = (u_xlat3.y * u_xlat3.y);
          u_xlat0.y = ((u_xlat3.x * u_xlat3.x) - u_xlat0.y);
          u_xlat1 = (u_xlat3.ywzx * u_xlat3);
          u_xlat2.x = dot(unity_SHCArray, u_xlat1);
          u_xlat2.y = dot(unity_SHCArray, u_xlat1);
          u_xlat2.z = dot(unity_SHCArray, u_xlat1);
          out_v.texcoord5.xyz = ((unity_SHCArray.xyz * u_xlat0.yyy) + u_xlat2.xyz);
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
          u_xlat0_d.xyz = (u_xlat0_d.xyz * _Color.xyz);
          u_xlat1_d = tex2D(_BumpMap, in_f.texcoord.zw);
          u_xlat1_d.x = (u_xlat1_d.w * u_xlat1_d.x);
          u_xlat1_d.xy = ((u_xlat1_d.xy * float2(2, 2)) + float2(-1, (-1)));
          u_xlat0_d.w = dot(u_xlat1_d.xyxx, u_xlat1_d.xyxx);
          u_xlat0_d.w = min(u_xlat0_d.w, 1);
          u_xlat0_d.w = ((-u_xlat0_d.w) + 1);
          u_xlat1_d.z = sqrt(u_xlat0_d.w);
          u_xlat2_d.x = dot(in_f.texcoord1.xyzx, u_xlat1_d.xyzx);
          u_xlat2_d.y = dot(in_f.texcoord2.xyzx, u_xlat1_d.xyzx);
          u_xlat2_d.z = dot(in_f.texcoord3.xyzx, u_xlat1_d.xyzx);
          u_xlat0_d.w = dot(u_xlat2_d.xyzx, u_xlat2_d.xyzx);
          u_xlat0_d.w = rsqrt(u_xlat0_d.w);
          u_xlat1_d.xyz = (u_xlat0_d.www * u_xlat2_d.xyz);
          u_xlat0_d.w = (unity_ProbeVolumeMin.x==1);
          if((u_xlat0_d.w!=0))
          {
              u_xlat0_d.w = (unity_ProbeVolumeMin.y==1);
              u_xlat2_d.xyz = (in_f.texcoord2.www * unity_ProbeVolumeMin.xyz);
              u_xlat2_d.xyz = ((unity_ProbeVolumeMin.xyz * in_f.texcoord1.www) + u_xlat2_d.xyz);
              u_xlat2_d.xyz = ((unity_ProbeVolumeMin.xyz * in_f.texcoord3.www) + u_xlat2_d.xyz);
              u_xlat2_d.xyz = (u_xlat2_d.xyz + unity_ProbeVolumeMin.xyz);
              u_xlat3_d.y = in_f.texcoord1.w;
              u_xlat3_d.z = in_f.texcoord2.w;
              u_xlat3_d.w = in_f.texcoord3.w;
              u_xlat2_d.xyz = (u_xlat0_d.www)?(u_xlat2_d.xyz):(u_xlat3_d.yzw);
              u_xlat2_d.xyz = (u_xlat2_d.xyz - unity_ProbeVolumeMin.xyz);
              u_xlat2_d.yzw = (u_xlat2_d.xyz * unity_ProbeVolumeMin.xyz);
              u_xlat0_d.w = (u_xlat2_d.y * 0.25);
              u_xlat2_d.y = (unity_ProbeVolumeMin.z * 0.5);
              u_xlat3_d.x = (((-unity_ProbeVolumeMin.z) * 0.5) + 0.25);
              u_xlat0_d.w = max(u_xlat0_d.w, u_xlat2_d.y);
              u_xlat2_d.x = min(u_xlat3_d.x, u_xlat0_d.w);
              u_xlat3_d = tex3D(unity_ProbeVolumeSH, u_xlat2_d.xz);
              u_xlat4.xyz = (u_xlat2_d.xzw + float3(0.25, 0, 0));
              u_xlat4 = tex3D(unity_ProbeVolumeSH, u_xlat4.xy);
              u_xlat2_d.xyz = (u_xlat2_d.xzw + float3(0.5, 0, 0));
              u_xlat2_d = tex3D(unity_ProbeVolumeSH, u_xlat2_d.xy);
              u_xlat1_d.w = 1;
              u_xlat3_d.x = dot(u_xlat3_d, u_xlat1_d);
              u_xlat3_d.y = dot(u_xlat4, u_xlat1_d);
              u_xlat3_d.z = dot(u_xlat2_d, u_xlat1_d);
          }
          else
          {
              u_xlat0_d.w = (in_f.SV_InstanceID.x + unity_BaseInstanceID.x);
              u_xlat0_d.w = ((u_xlat0_d.w * 7) & -1);
              u_xlat1_d.w = 1;
              u_xlat3_d.x = dot(unity_SHCArray, u_xlat1_d);
              u_xlat3_d.y = dot(unity_SHCArray, u_xlat1_d);
              u_xlat3_d.z = dot(unity_SHCArray, u_xlat1_d);
          }
          u_xlat2_d.xyz = (u_xlat3_d.xyz + in_f.texcoord5.xyz);
          u_xlat2_d.xyz = max(u_xlat2_d.xyz, float3(0, 0, 0));
          u_xlat2_d.xyz = log(u_xlat2_d.xyz);
          u_xlat2_d.xyz = (u_xlat2_d.xyz * float3(0.416667, 0.416667, 0.416667));
          u_xlat2_d.xyz = exp(u_xlat2_d.xyzx);
          u_xlat2_d.xyz = ((u_xlat2_d.xyz * float3(1.055, 1.055, 1.055)) + float3(-0.055, (-0.055), (-0.055)));
          u_xlat2_d.xyz = max(u_xlat2_d.xyz, float3(0, 0, 0));
          out_f.color2.xyz = ((u_xlat1_d.xyz * float3(0.5, 0.5, 0.5)) + float3(0.5, 0.5, 0.5));
          out_f.color3.xyz = (u_xlat0_d.xyz * u_xlat2_d.xyz);
          out_f.color.xyz = u_xlat0_d.xyz;
          out_f.color.w = 1;
          out_f.color1 = float4(0, 0, 0, 0);
          out_f.color2.w = 1;
          out_f.color3.w = 1;
          return out_f;
      }
      
      
      ENDCG
      
    } // end phase
  }
  FallBack "Legacy Shaders/Diffuse"
}
