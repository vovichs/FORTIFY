Shader "FX/drawOverCutout"
{
  Properties
  {
    _MainTex ("Texture", 2D) = "white" {}
    _Color ("Color", Color) = (1,1,1,1)
    _Cutoff ("Alpha cutoff", Range(0, 1)) = 0.5
  }
  SubShader
  {
    Tags
    { 
      "IGNOREPROJECTOR" = "true"
      "QUEUE" = "AlphaTest"
      "RenderType" = "TransparentCutout"
    }
    Pass // ind: 1, name: 
    {
      Tags
      { 
        "IGNOREPROJECTOR" = "true"
        "QUEUE" = "AlphaTest"
        "RenderType" = "TransparentCutout"
      }
      ZWrite Off
      Cull Off
      Blend SrcAlpha OneMinusSrcAlpha
      // m_ProgramMask = 6
      CGPROGRAM
      //#pragma target 4.0
      
      #pragma vertex vert
      #pragma fragment frag
      
      #include "UnityCG.cginc"
      
      
      #define CODE_BLOCK_VERTEX
      uniform float4 _MainTex_ST;
      uniform float4 _Color;
      //uniform float4x4 unity_ObjectToWorld;
      //uniform float4x4 unity_MatrixVP;
      uniform float _Cutoff;
      uniform sampler2D _MainTex;
      struct appdata_t
      {
          float4 vertex :POSITION0;
          float4 color :COLOR;
          float2 texcoord :TEXCOORD0;
      };
      
      struct OUT_Data_Vert
      {
          float4 vertex :SV_POSITION;
          float4 color :COLOR;
          float2 texcoord :TEXCOORD0;
      };
      
      struct v2f
      {
          float4 vertex :SV_POSITION;
          float4 color :COLOR;
          float2 texcoord :TEXCOORD0;
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
          out_v.vertex = UnityObjectToClipPos(in_v.vertex);
          out_v.color = (in_v.color * _Color);
          out_v.texcoord.xy = ((in_v.texcoord.xy * _Color.xy) + _Color.zw);
          return out_v;
      }
      
      #define CODE_BLOCK_FRAGMENT
      float4 u_xlat0_d;
      float4 u_xlat1_d;
      OUT_Data_Frag frag(v2f in_f)
      {
          OUT_Data_Frag out_f;
          u_xlat0_d = tex2D(_MainTex, in_f.texcoord.xy);
          u_xlat0_d = (u_xlat0_d * in_f.color);
          u_xlat1_d.x = ((u_xlat0_d.w * _Cutoff.w) - _Cutoff.x);
          out_f.color = u_xlat0_d;
          u_xlat0_d.x = (u_xlat1_d.x<0);
          if((u_xlat0_d.x!=0))
          {
              discard;
          }
          return out_f;
      }
      
      
      ENDCG
      
    } // end phase
  }
  FallBack "Transparent"
}
