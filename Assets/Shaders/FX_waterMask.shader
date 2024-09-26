Shader "FX/waterMask"
{
  Properties
  {
  }
  SubShader
  {
    Tags
    { 
      "QUEUE" = "Geometry+10"
    }
    Pass // ind: 1, name: 
    {
      Tags
      { 
        "QUEUE" = "Geometry+10"
      }
      Fog
      { 
        Mode  Off
      } 
      ColorMask 0
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
      //uniform float4x4 unity_ObjectToWorld;
      //uniform float4x4 unity_MatrixV;
      //uniform float4x4 unity_MatrixVP;
      //uniform float4 unity_FogParams;
      //uniform float4 unity_FogColor;
      struct appdata_t
      {
          float3 vertex :POSITION0;
          float4 color :COLOR;
      };
      
      struct OUT_Data_Vert
      {
          float4 color :COLOR;
          float texcoord :TEXCOORD0;
          float4 vertex :SV_POSITION;
      };
      
      struct v2f
      {
          float4 color :COLOR;
          float texcoord :TEXCOORD0;
          float4 vertex :SV_POSITION;
      };
      
      struct OUT_Data_Frag
      {
          float4 color :SV_Target;
      };
      
      float4 u_xlat0;
      float4 u_xlat1;
      OUT_Data_Vert vert(appdata_t in_v)
      {
          OUT_Data_Vert out_v;
          out_v.color = saturate(in_v.color);
          u_xlat0.xyz = (conv_mxt4x4_1(unity_ObjectToWorld).yyy * conv_mxt4x4_-7(unity_MatrixVP).xyz);
          u_xlat0.xyz = ((conv_mxt4x4_-8(unity_MatrixVP).xyz * conv_mxt4x4_1(unity_ObjectToWorld).xxx) + u_xlat0.xyz);
          u_xlat0.xyz = ((conv_mxt4x4_-6(unity_MatrixVP).xyz * conv_mxt4x4_1(unity_ObjectToWorld).zzz) + u_xlat0.xyz);
          u_xlat0.xyz = ((conv_mxt4x4_-5(unity_MatrixVP).xyz * conv_mxt4x4_1(unity_ObjectToWorld).www) + u_xlat0.xyz);
          u_xlat0.xyz = (u_xlat0.xyz * in_v.vertex.yyy);
          u_xlat1.xyz = (conv_mxt4x4_0(unity_ObjectToWorld).yyy * conv_mxt4x4_-7(unity_MatrixVP).xyz);
          u_xlat1.xyz = ((conv_mxt4x4_-8(unity_MatrixVP).xyz * conv_mxt4x4_0(unity_ObjectToWorld).xxx) + u_xlat1.xyz);
          u_xlat1.xyz = ((conv_mxt4x4_-6(unity_MatrixVP).xyz * conv_mxt4x4_0(unity_ObjectToWorld).zzz) + u_xlat1.xyz);
          u_xlat1.xyz = ((conv_mxt4x4_-5(unity_MatrixVP).xyz * conv_mxt4x4_0(unity_ObjectToWorld).www) + u_xlat1.xyz);
          u_xlat0.xyz = ((u_xlat1.xyz * in_v.vertex.xxx) + u_xlat0.xyz);
          u_xlat1.xyz = (conv_mxt4x4_2(unity_ObjectToWorld).yyy * conv_mxt4x4_-7(unity_MatrixVP).xyz);
          u_xlat1.xyz = ((conv_mxt4x4_-8(unity_MatrixVP).xyz * conv_mxt4x4_2(unity_ObjectToWorld).xxx) + u_xlat1.xyz);
          u_xlat1.xyz = ((conv_mxt4x4_-6(unity_MatrixVP).xyz * conv_mxt4x4_2(unity_ObjectToWorld).zzz) + u_xlat1.xyz);
          u_xlat1.xyz = ((conv_mxt4x4_-5(unity_MatrixVP).xyz * conv_mxt4x4_2(unity_ObjectToWorld).www) + u_xlat1.xyz);
          u_xlat0.xyz = ((u_xlat1.xyz * in_v.vertex.zzz) + u_xlat0.xyz);
          u_xlat1.xyz = (conv_mxt4x4_3(unity_ObjectToWorld).yyy * conv_mxt4x4_-7(unity_MatrixVP).xyz);
          u_xlat1.xyz = ((conv_mxt4x4_-8(unity_MatrixVP).xyz * conv_mxt4x4_3(unity_ObjectToWorld).xxx) + u_xlat1.xyz);
          u_xlat1.xyz = ((conv_mxt4x4_-6(unity_MatrixVP).xyz * conv_mxt4x4_3(unity_ObjectToWorld).zzz) + u_xlat1.xyz);
          u_xlat1.xyz = ((conv_mxt4x4_-5(unity_MatrixVP).xyz * conv_mxt4x4_3(unity_ObjectToWorld).www) + u_xlat1.xyz);
          u_xlat0.xyz = (u_xlat0.xyz + u_xlat1.xyz);
          u_xlat0.x = length(u_xlat0.xyzx);
          u_xlat0.x = (u_xlat0.x * unity_FogParams.y);
          u_xlat0.x = exp((-u_xlat0.x));
          out_v.texcoord.x = min(u_xlat0.x, 1);
          out_v.vertex = UnityObjectToClipPos(float4(in_v.vertex, 0));
          return out_v;
      }
      
      #define CODE_BLOCK_FRAGMENT
      float4 u_xlat0_d;
      OUT_Data_Frag frag(v2f in_f)
      {
          OUT_Data_Frag out_f;
          u_xlat0_d.xyz = (in_f.color.xyz - unity_FogColor.xyz);
          out_f.color.xyz = ((in_f.texcoord.xxx * u_xlat0_d.xyz) + unity_FogColor.xyz);
          out_f.color.w = in_f.color.w;
          return out_f;
      }
      
      
      ENDCG
      
    } // end phase
  }
  FallBack Off
}
