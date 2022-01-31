Shader "Final/Terrain/miniMap" {
  Properties {

    _Color ("Color", Color) = (1,1,1,1)

    _MainTex ("Texture", 2D) = "white" {}
    _NormalMap ("NormalMap", 2D) = "white" {}
    _CubeMap( "Cube Map" , Cube )  = "defaulttexture" {}
    _Debug("DEBUG",float) = 0
    _HueStart("_HueStart",float) = 0
    _GrassHueSize("_GrassHueSize",float) = 0
    _TextureHueSize("_TextureHueSize",float) = 0
    _PainterlyLightMap ("Painterly", 2D) = "white" {}
    _PaintSize("_PaintSize", Vector ) = (1,1,1,1)
    
  }

  SubShader {
    // COLOR PASS

    Pass {
      Tags{ "LightMode" = "ForwardBase" "Queue" = "Geometry" }
      Cull Off


      Stencil {
        Ref 1
        Comp Always 
        Pass Replace
      }

      CGPROGRAM
      #pragma target 4.5
      #pragma vertex vert
      #pragma fragment frag
      #pragma multi_compile_fwdbase nolightmap nodirlightmap nodynlightmap novertexlight

      #include "UnityCG.cginc"
      #include "AutoLight.cginc"
      
      struct Vert{
        float3 pos;
        float3 vel;
        float3 nor;
        float3 tan;
        float2 uv;
        float2 debug;
      };

      #include "../Chunks/hsv.cginc"
      #include "../Chunks/noise.cginc"

      float3 _Color;
      float3 _PlayerPosition;
      float3 _TerrainHole;

      bool _Debug;
      float _HueStart;
      float _GrassHueSize;
      float _TextureHueSize;
      sampler2D _MainTex;
      sampler2D _ColorMap;
      sampler2D _NormalMap;

      float2 _PaintSize;


      StructuredBuffer<Vert> _VertBuffer;
      StructuredBuffer<int> _TriBuffer;



      #include "../Chunks/ComputeTerrainInfo.cginc"
      #include "../Chunks/Noise.cginc"


      #include "../Chunks/SampleAudio.cginc"
      #include "../Chunks/Reflection.cginc"
      #include "../Chunks/Fog.cginc"

      #include "../Chunks/PainterlyLight.cginc"
      #include "../Chunks/GetFullColor.cginc"

      struct varyings {
        float4 pos    : SV_POSITION;
        float3 nor    : TEXCOORD0;
        float2 uv     : TEXCOORD1;
        float3 eye      : TEXCOORD5;
        float3 worldPos : TEXCOORD6;
        float3 debug    : TEXCOORD7;
        float3 closest    : TEXCOORD8;
        float3 fullPos   : TEXCOORD9;
        UNITY_SHADOW_COORDS(2)
      };

      varyings vert(uint id : SV_VertexID) {

        Vert v = _VertBuffer[_TriBuffer[id]];
        
        float3 fPos   = v.pos;
        float3 fNor   = v.nor;
        float2 fUV    = v.uv;
        float2 debug  = v.debug;

        varyings o;

        UNITY_INITIALIZE_OUTPUT(varyings, o);

        //fPos += float3(0,.1,0) * noise(fPos + float3(0,_Time.y,0));
        o.worldPos = fPos;



        o.pos = mul(UNITY_MATRIX_VP, float4(fPos,1));
        o.eye = _WorldSpaceCameraPos - fPos;
        o.nor = fNor;
        o.uv =  float2(.9,1)-fUV;
        o.debug = float3(debug.x,debug.y,0);
        o.fullPos = v.vel;

        UNITY_TRANSFER_SHADOW(o,o.worldPos);

        return o;
      }




float2 rotateUV(float2 uv, float rotation)
{
    float mid = 0.5;
    return float2(
        cos(rotation) * (uv.x - mid) + sin(rotation) * (uv.y - mid) + mid,
        cos(rotation) * (uv.y - mid) - sin(rotation) * (uv.x - mid) + mid
    );
}
   
      float4 frag(varyings v) : COLOR {

        float4 color= 0;
        
        fixed shadow = UNITY_SHADOW_ATTENUATION(v,v.worldPos)  ;

        float3 fNor = tex2D(_NormalMap ,  v.worldPos.xz * _PaintSize );
              fNor = normalize(v.nor * fNor.z *2 + float3(1,0,0) * (fNor.x)  + float3(0,0,1) * (fNor.y-.5));//normalize( fNor );

        float m = dot(v.nor, _WorldSpaceLightPos0 );
        float v2 = -m * shadow;

        float4 terrainCol =  GetFullColor(.5 -v2 * .5 , v.fullPos.xz * _MapSize);
        float4 painterly = Painterly( v2, v.worldPos.xz * _PaintSize );
        float3 tCol = Reflection(normalize(v.eye),fNor);//texCUBE(_CubeMap,refl);

        tCol = length(tCol)*length(tCol)/3;

        float4 audio = SampleAudio(length(tCol.xyz) * .2 ) * 2;

        color = GetFullColor( v.debug.x *.5, v.fullPos.xz * _MapSize);
   // color = sin(v.fullPos.x * _MapSize);
        if( sin( v.debug.x * 1000) < 0 ){
            color = 0;
            discard;
        }

        if( v.debug.x == 0 ){
            discard;
        }
       /* color *=  painterly * .7 + .5;
        color.xyz *= tCol;
        color *= FogMultiplier( v.worldPos ) ;*/


       /* float holeVal = length( v.worldPos - _TerrainHole)  + noise( v.worldPos * 4.2 + float3(0,_Time.y * .2,0) )  * .2;
        if( holeVal < 2 ){
          discard;
        }
        if( holeVal < 2.3){ color = saturate((holeVal - 2) * 4) * color;}

        
        if( _Debug != 0 ){ color.xyz = v.nor * .5 + .5; }*/

        return float4( color.xyz  , 1.);

      }

      ENDCG
    }


    // SHADOW PASS

    Pass
    {
      Tags{ "LightMode" = "ShadowCaster" }


      Fog{ Mode Off }
      ZWrite On
      ZTest LEqual
      Cull Off
      Offset 1, 1
      CGPROGRAM

      #pragma target 4.5
      #pragma vertex vert
      #pragma fragment frag
      #pragma multi_compile_shadowcaster
      #pragma fragmentoption ARB_precision_hint_fastest
      #include "UnityCG.cginc"

      

      #include "../Chunks/Struct16.cginc"
      #include "../Chunks/ShadowCasterPos.cginc"
      #include "../Chunks/noise.cginc"

      
      

      StructuredBuffer<Vert> _VertBuffer;
      StructuredBuffer<int> _TriBuffer;

      float3 _TerrainHole;

      struct v2f {
        V2F_SHADOW_CASTER;
        float3 nor : NORMAL;
        float3 worldPos : TEXCOORD0;
      };


      v2f vert(appdata_base input, uint id : SV_VertexID)
      {
        v2f o;
        Vert v = _VertBuffer[_TriBuffer[id]];

        float4 position = ShadowCasterPos(v.pos, -v.nor);
        o.pos = UnityApplyLinearShadowBias(position);
        o.worldPos = v.pos;
        return o;
      }

      float4 frag(v2f i) : COLOR
      {

        float holeVal = length( i.worldPos - _TerrainHole)  + noise( i.worldPos * 4.2 + float3(0,_Time.y * .2,0) )  * .2;
        if( holeVal < 2 ){
          discard;
        }
        SHADOW_CASTER_FRAGMENT(i)
      }

      ENDCG
    }
    


  }

}
