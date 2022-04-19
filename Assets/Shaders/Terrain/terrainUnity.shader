Shader "Final/Terrain/terrainUnity"
{
    Properties
    {
          _Color ("Color", Color) = (1,1,1,1)

        _DebugTex ("Texture", 2D) = "white" {}
        //_DebugFalloff("falloff" ,)
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
    SubShader
    {

        Pass
        {

         
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
      sampler2D _DebugTex;
      sampler2D _ColorMap;
      sampler2D _NormalMap;

      float2 _PaintSize;

      #include "../Chunks/ComputeTerrainInfo.cginc"
      #include "../Chunks/Noise.cginc"


      #include "../Chunks/SampleAudio.cginc"
      #include "../Chunks/Reflection.cginc"
      #include "../Chunks/Fog.cginc"

      #include "../Chunks/PainterlyLight.cginc"
      #include "../Chunks/GetFullColor.cginc"


            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float3  normal : NORMAL;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float3 nor : NORMAL;
                float3 worldPos : TEXCOORD1;
                float3 eye : TEXCOORD2;
        UNITY_SHADOW_COORDS(3)
            };


            v2f vert (appdata v)
            {
                v2f o;

                
        UNITY_INITIALIZE_OUTPUT(v2f, o);
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;

                   o.worldPos = mul( unity_ObjectToWorld, v.vertex ).xyz;
        o.eye = _WorldSpaceCameraPos-o.worldPos;
                   o.nor = normalize( mul( unity_ObjectToWorld, v.normal ).xyz );

        UNITY_TRANSFER_SHADOW(o,o.worldPos);
                return o;
            }

            fixed4 frag (v2f v) : SV_Target
            {
                float4 color= 0;
        
        fixed shadow = UNITY_SHADOW_ATTENUATION(v,v.worldPos)  ;

        float3 fNor = tex2D(_NormalMap ,  v.worldPos.xz * _PaintSize );
              fNor = normalize(v.nor * fNor.z *2 + float3(1,0,0) * (fNor.x)  + float3(0,0,1) * (fNor.y-.5));//normalize( fNor );

        float m = dot(v.nor, _WorldSpaceLightPos0 );
        float v2 = -m * shadow;

        float4 terrainCol =  GetFullColor(.5 -v2 * .5 , v.worldPos.xz * _MapSize);
        float4 painterly = Painterly( v2, v.worldPos.xz * _PaintSize );
        float3 tCol = Reflection(normalize(v.eye),fNor);//texCUBE(_CubeMap,refl);

        tCol = length(tCol)*length(tCol)/3;

        float4 audio = SampleAudio(length(tCol.xyz) * .2 ) * 2;

        color = GetFullColor( v2 * .13 + .5, v.worldPos.xz * _MapSize);
        color *=  painterly * .7 + .5;
        color.xyz *= tCol * 1;
        color *= FogMultiplier( v.worldPos ) ;


        float holeVal = length( v.worldPos - _TerrainHole)  + noise( v.worldPos * 4.2 + float3(0,_Time.y * .2,0) )  * .2;
        if( holeVal < 2 ){
          discard;
        }
        if( holeVal < 2.3){ color = saturate((holeVal - 2) * 4) * color;}

        
        if( _Debug != 0 ){ color =   tex2D(_DebugTex,v.worldPos.xz * _PaintSize) * FogMultiplier( v.worldPos * 100); }



        return float4( color.xyz  , 1.);

            }
            ENDCG
        }
    }
}
