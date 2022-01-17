Shader "Debug/MarchingCubes"
{
    Properties {

  
       _TextureMap("TextureMap", 2D) = "white" {}
       _PainterlyLightMap ("Painterly", 2D) = "white" {}
       _NormalMap ("Normal", 2D) = "white" {}
       _CubeMap( "Cube Map" , Cube )  = "defaulttexture" {}

      _ColorSize("_ColorSize", float ) = 0.5
      _ColorBase("_ColorBase", float ) = 0
      _OutlineColor("_OutlineColor", float ) = 0
      _OutlineAmount("_OutlineAmount", float ) = .16
      _PaintSize("_PaintSize", Vector ) = (1,1,1,1)
      _NormalSize("_NormalSize", Vector ) = (1,1,1,1)
      _NormalDepth("_NormalDepth", float ) = .4
      _OverallMultiplier("_OverallMultiplier", float ) = 1

  }
    SubShader
    {
        
      



        Pass
        {

          Tags { "RenderType"="Opaque" }
          LOD 100

          Cull Off
          // Lighting/ Texture Pass
          Stencil
          {
            Ref 4
            Comp always
            Pass replace
            ZFail keep
          }

          Tags{ "LightMode" = "ForwardBase" }

            CGPROGRAM
            
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 4.5
            #pragma multi_compile_fwdbase nolightmap nodirlightmap nodynlightmap novertexlight
          
            #include "../Chunks/struct8.cginc"


            
#include "UnityCG.cginc"
#include "AutoLight.cginc"

struct v2f{ 
  float4 pos        : SV_POSITION; 
  float3 nor        : NORMAL; 

  float debug       : TEXCOORD0; 
  
  float3 eye        : TEXCOORD1;
  float3 world      : TEXCOORD2;  
  float2 uv         : TEXCOORD3; 
  float4 screenPos  : TEXCOORD4;

  // For our matrix
  float3 t1         : TEXCOORD5;
  float3 t2         : TEXCOORD6;
  float3 t3         : TEXCOORD7;
  float3 noiseVal   : TEXCOORD10;

            float3 vel : TEXCOORD8;
  
            UNITY_SHADOW_COORDS(9)

};




StructuredBuffer<Vert> _VertBuffer;
StructuredBuffer<int> _TriBuffer;

sampler2D _ColorMap;
sampler2D _TextureMap;
sampler2D _NormalMap;


float2 _NormalSize;
float2 _PaintSize;
float _NormalDepth;

float _ColorBase;
float _ColorSize;


#include "../Chunks/noise.cginc"



float getNoise( float3 p ){

    float n;
    float speed = .1;
    n  = noise( p * .1 + float3(0,_Time.y * speed     ,0) );
    n += noise( p * .3 + float3(0,_Time.y * speed  * 2,0) ) * .8;
    n += noise( p * .8 + float3(0,_Time.y * speed  * 4,0) ) * .5;
    n += noise( p * 1.8 + float3(0,_Time.y * speed  * 4,0) ) * .3;
    return n;
}
float3 doNoise( float3 p ){

    float d = getNoise(p);


    float3 eps = float3(0.1,0,0);

    float3 nor = normalize(float3(
        getNoise(p+eps.xyy) - getNoise(p-eps.xyy),
        getNoise(p+eps.yxy) - getNoise(p-eps.yxy),
        getNoise(p+eps.yyx) - getNoise(p-eps.yyx)
    ) * 1000);

    return nor * d;
}

v2f vert ( uint vid : SV_VertexID )
{
    v2f o;

    UNITY_INITIALIZE_OUTPUT(v2f, o);

    Vert v = _VertBuffer[vid];

    if( v.pos.x > .001 ){


        float3 noiseVal = doNoise(v.pos );


    o.world = v.pos;
    o.uv = v.uv;
    o.pos = mul (UNITY_MATRIX_VP, float4(v.pos,1.0f));
    o.nor = v.nor;//normalize(cross(v0.pos - v1.pos , v0.pos - v2.pos ));
    //o.debug = v.debug;
    o.eye = v.pos - _WorldSpaceCameraPos;
    o.screenPos = ComputeScreenPos(o.pos);
    o.noiseVal = noiseVal;

    
   // o.vel = v.vel;

   /* float3 bi = cross(v.nor, v.tan);
    
    // output the tangent space matrix
    o.t1 =  float3(v.tan.x, bi.x, v.nor.x);
    o.t2 =  float3(v.tan.y, bi.y, v.nor.y);
    o.t3 =  float3(v.tan.z, bi.z, v.nor.z);*/

    TRANSFER_SHADOW(o);

    }
    
    return o;
}




            #include "../Chunks/PainterlyLight.cginc"
            #include "../Chunks/MapNormal.cginc"
            #include "../Chunks/Reflection.cginc"
            #include "../Chunks/SampleAudio.cginc"
            #include "../Chunks/ColorScheme.cginc"
            #include "../Chunks/noise.cginc"


            float _WhichColor;
            float _OverallMultiplier;


            
// Generic algorithm to desaturate images used in most game engines
float3 Desaturate(float3 color, float factor)
{
	float3 lum = float3(0.299, 0.587, 0.114);
	float3 gray = dot(lum, color);
	return lerp(color, gray, factor);
}


float4 TriplanarTexture( float3 pos , float3 nor , float size){

    float n = noise( pos + float3(0,_Time.y,0)) * .4;
  float4 t1 = tex2D(_TextureMap , pos.zy * size ) * abs(nor.x);
  float4 t2 = tex2D(_TextureMap , pos.xz * size ) * abs(nor.y);
  float4 t3 = tex2D(_TextureMap , pos.xy * size ) * abs(nor.z);
  return t1 + t2 + t3;
}
            fixed4 frag (v2f v) : SV_Target
            {

                fixed shadow = UNITY_SHADOW_ATTENUATION(v,v.world) * .5 + .5;

          
                float3 col;

                float3 noiseVal = doNoise( v.world * 2 );

                
                //float3 p = Painterly( m, v.uv.xy * _PaintSize );

                
       

                float3 n = normalize(v.nor + normalize(noiseVal ) * .3);//noise( v.world + float3(0,_Time.y,0));;//MapNormal( v , v.uv * _NormalSize , _NormalDepth );
                float3 reflectionColor = Reflection( v.pos , n );
                float4 tri = TriplanarTexture(v.world , v.nor , _PaintSize);

                float m = dot( n, _WorldSpaceLightPos0.xyz);
                m *= shadow;
                m = saturate(m);
                m = 1-m;
                
                float3 p = Painterly( m * .99 , tri);

                

                p = 1-p;


                
                float3 mapColor = GetGlobalColor( m * _ColorSize  + _ColorBase );


                float3 refl = normalize(reflect( v.eye,n ));
                float rM = saturate(dot(refl,_WorldSpaceLightPos0.xyz));
                
                float4 audio = SampleAudio(length(reflectionColor.xyz) * .05 + v.uv.x * .2) * 2;
                
                
                col = mapColor;//*p;     
                col.xyz *= reflectionColor * 2;
                col  +=  (1-saturate(length(col.xyz)*10))* audio.xyz;
                col *= _OverallMultiplier;
                
                col = Desaturate(col, .8);
              
              //col = reflectionColor;


                return float4(col,1);
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

      float DoShadowDiscard( float3 pos , float2 uv ){
         return 1;
      }

      #pragma target 4.5
      #pragma vertex vert
      #pragma fragment frag
      #pragma multi_compile_shadowcaster
      #pragma fragmentoption ARB_precision_hint_fastest

      #include "UnityCG.cginc"
      sampler2D _MainTex;


      #include "../Chunks/struct8.cginc"

      
      #include "../Chunks/ShadowCasterPos.cginc"
   

      StructuredBuffer<Vert> _VertBuffer;

      struct v2f {
        V2F_SHADOW_CASTER;
        float3 nor : NORMAL;
        float3 worldPos : TEXCOORD1;
        float2 uv : TEXCOORD0;
      };


      v2f vert(appdata_base input, uint id : SV_VertexID)
      {
        v2f o;
        Vert v = _VertBuffer[id];

        float4 position = ShadowCasterPos(v.pos, -v.nor);
        o.pos = UnityApplyLinearShadowBias(position);
        o.worldPos = v.pos;
        o.uv = v.uv;
        return o;
      }

      float4 frag(v2f i) : COLOR
      {

        if( DoShadowDiscard(i.worldPos,i.uv) < .5 ){ discard; }

        SHADOW_CASTER_FRAGMENT(i)
      }

      ENDCG
    }




               // SHADOW PASS

    Pass
    {

    // Outline Pass
    Cull OFF
    ZWrite OFF
    ZTest ON

    Stencil
    {
      Ref 4
      Comp notequal
      Fail keep
      Pass replace
    }
          
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 4.5
            // make fog work
            #pragma multi_compile_fogV
 #pragma multi_compile_fwdbase nolightmap nodirlightmap nodynlightmap novertexlight

      #include "UnityCG.cginc"
      #include "AutoLight.cginc"
    


            #include "../Chunks/Struct8.cginc"


            struct v2f { 
              float4 pos : SV_POSITION; 
            };
            float4 _Color;

            StructuredBuffer<Vert> _VertBuffer;
            StructuredBuffer<int> _TriBuffer;
            sampler2D _ColorMap;
            float _OutlineColor;
            float _OutlineAmount;
            float _WhichColor;

            v2f vert ( uint vid : SV_VertexID )
            {
                v2f o;

        
                Vert v = _VertBuffer[vid];
                float3 fPos = v.pos + v.nor * _OutlineAmount;
                o.pos = mul (UNITY_MATRIX_VP, float4(fPos,1.0f));


                return o;
            }

      
            #include "../Chunks/ColorScheme.cginc"
            fixed4 frag (v2f v) : SV_Target
            {
              
                fixed4 col =GetGlobalColor( _OutlineColor );
                col *= 1;
                return col;
            }

            ENDCG
        }

    
  
  
  
  }


}