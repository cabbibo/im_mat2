Shader "Debug/ConnectionList" {
    Properties {

    _Color ("Color", Color) = (1,1,1,1)
    _Size ("Size", float) = .01
    }


  SubShader{
//        Tags { "RenderType"="Transparent" "Queue" = "Transparent" }
    Cull Off
    Pass{

      //Blend SrcAlpha OneMinusSrcAlpha // Alpha blending

      CGPROGRAM
      #pragma target 4.5

      #pragma vertex vert
      #pragma fragment frag

      #include "UnityCG.cginc"

      #include "../Chunks/Struct16.cginc"

      uniform int _Count;
      uniform float _Size;
      uniform float3 _Color;


      struct connection{
        float id1;
        float id2;
        float length;
      };


      StructuredBuffer<Vert> _VertBuffer;
      StructuredBuffer<connection> _ConnectionBuffer;


      //uniform float4x4 worldMat;

      //A simple input struct for our pixel shader step containing a position.
      struct varyings {
          float4 pos      : SV_POSITION;
          float debug     : TEXCOORD0;
      };


      //Our vertex function simply fetches a point from the buffer corresponding to the vertex index
      //which we transform with the view-projection matrix before passing to the pixel program.
      varyings vert (uint id : SV_VertexID){

        varyings o;

        int base = id / 6;
        int alt = id % 6;


        connection c = _ConnectionBuffer[base];


        float3 v1 = _VertBuffer[int(c.id1)].pos;
        float3 v2 = _VertBuffer[int(c.id2)].pos;

        float3 dir = v1-v2;


        float3 l = normalize(cross(UNITY_MATRIX_V[2].xyz,dir)) * _Size;
        
        float2 uv = float2(0,0);

        float3 extra;

        if( alt == 0 ){ extra = -l + v1; uv = float2(0,0); }
        if( alt == 1 ){ extra =  l  + v1; uv = float2(1,0); }
        if( alt == 2 ){ extra =  l + v2; uv = float2(1,1); }
        if( alt == 3 ){ extra = -l + v1; uv = float2(0,0); }
        if( alt == 4 ){ extra =  l + v2; uv = float2(1,1); }
        if( alt == 5 ){ extra = -l + v2; uv = float2(0,1); }

        o.pos = mul (UNITY_MATRIX_VP, float4(extra,1.0f));
        
        return o;
      }




      //Pixel function returns a solid color for each point.
      float4 frag (varyings v) : COLOR {
          return float4( _Color , 1 );
      }

      ENDCG

    }
  }

  Fallback Off


}
