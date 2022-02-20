Shader "Scenes/BaseShaderDiscardTexture"
{
    Properties {
       _TextureMap ("Texture", 2D) = "white" {}
       _PainterlyLightMap ("Painterly", 2D) = "white" {}
       _AlphaMap ("AlphaMap", 2D) = "white" {}
       _NormalMap ("Normal", 2D) = "white" {}
       _CubeMap( "Cube Map" , Cube )  = "defaulttexture" {}

      _ColorSize("_ColorSize", float ) = 0.5
      _ColorBase("_ColorBase", float ) = 0
      _OutlineColor("_OutlineColor", float ) = 0
      _OutlineAmount("_OutlineAmount", float ) = .16
      _PaintSize("_PaintSize", Vector ) = (1,1,1,1)
      _NormalSize("_NormalSize", Vector ) = (1,1,1,1)
      _NormalDepth("_NormalDepth", float ) = .4

      _TextureRepeat("TextureRepeat", float) = 1

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


            #include "../Chunks/struct16.cginc"

            #pragma vertex vert
            #pragma fragment frag
            #pragma target 4.5
            #pragma multi_compile_fwdbase nolightmap nodirlightmap nodynlightmap novertexlight


       
#include "UnityCG.cginc"
#include "AutoLight.cginc"
#include "../Chunks/hash.cginc"

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

float _TextureRepeat;

v2f vert ( uint vid : SV_VertexID )
{
    v2f o;

    UNITY_INITIALIZE_OUTPUT(v2f, o);

    Vert v = _VertBuffer[_TriBuffer[vid]];

    int pID = vid / 6;

    o.world = v.pos;
    o.uv = v.uv * (1./_TextureRepeat)+ floor(float2(hash(v.debug.x*10), hash(v.debug.x*20)) * _TextureRepeat)/_TextureRepeat;

    
    o.pos = mul (UNITY_MATRIX_VP, float4(v.pos,1.0f));
    o.nor = v.nor;//normalize(cross(v0.pos - v1.pos , v0.pos - v2.pos ));
    o.debug = v.debug;
    o.eye = v.pos - _WorldSpaceCameraPos;
    o.screenPos = ComputeScreenPos(o.pos);
    o.vel = v.vel;

    float3 bi = cross(v.nor, v.tan);
    
    // output the tangent space matrix
    o.t1 =  float3(v.tan.x, bi.x, v.nor.x);
    o.t2 =  float3(v.tan.y, bi.y, v.nor.y);
    o.t3 =  float3(v.tan.z, bi.z, v.nor.z);

    TRANSFER_SHADOW(o);
    
    return o;
}


            #include "../Chunks/PainterlyLight.cginc"
            #include "../Chunks/TriplanarTexture.cginc"
            #include "../Chunks/MapNormal.cginc"
            #include "../Chunks/Reflection.cginc"


            #include "../Chunks/SampleAudio.cginc"
      
            #include "../Chunks/ColorScheme.cginc"


            float _WhichColor;

            sampler2D _AlphaMap;

            fixed4 frag (v2f v) : SV_Target
            {



                fixed shadow = UNITY_SHADOW_ATTENUATION(v,v.world) * .5 + .5;

                float3 n = MapNormal( v , v.uv * _NormalSize , _NormalDepth );
                float3 reflectionColor = Reflection( v.pos , n );

                float m = dot( n, _WorldSpaceLightPos0.xyz);
                float baseM = m;
                m =m;// saturate(( m +1 )/2);

                m *= shadow;

                m = saturate(m);

                m = 1-m;

              



                float3 col  = GetGlobalColor( m * _ColorSize  + _ColorBase );
                float3 p = Painterly( m , v.uv.xy * _PaintSize );

            


                float4 tex = tex2D(_TextureMap , v.uv.xy );
                float4 alphaT = tex2D(_AlphaMap , v.uv.xy );

                if( tex.x < .5 ){
                    discard;
                }


               //col.xyz *= p * .3+ p * r;
               ////col *= baseM;
               ////col *= 10.;
               //col.xyz *=   r.xyz * 2;
               /// col *= col;


               //col.yxz = p.xyz*tex * .6 + .4;


               //col = p;
               //col = r*audio * 15;
                col = col*(p+1);

                
                float3 refl = normalize(reflect( v.eye,n ));
                float rM = saturate(dot(refl,_WorldSpaceLightPos0.xyz));
              //  col += col *pow(rM,5)*20;
                
               // float3 audio = SampleAudio(v.uv.x * .1 + p.x * .03 );


               // col = reflectionColor;

               // col.xyz *= (audio *audio*10 + 1);
                col.xyz *= reflectionColor * 4;

                    float4 audio = SampleAudio(length(reflectionColor.xyz) * .05 + v.uv.x * .2) * 2;
              //  col  +=  (1-saturate(length(col.xyz)*10))* audio.xyz;

                //col.xyz = p * p * col;//m;//normalize(_WorldSpaceLightPos0.xyz) * .5+ .5 ;//m;//p;
                //col = shadow;


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

          float2 fUV= uv * (1./_TextureRepeat)+ floor(float2(hash(v.debug.x*10), hash(v.debug.x*20)) * _TextureRepeat)/_TextureRepeat;
            
         return tex2D(_MainTex, fUV).x;;//float lookupVal =  max(min( uv.y * 2,( 1- uv.y ) ) * 1.5,0);//2 * tex2D(_MainTex,uv * float2(4 * saturate(min( uv.y * 4,( 1- uv.y ) )) ,.8) + float2(0,.2));
         // float4 tCol = tex2D(_MainTex, uv *   float2( 6,(lookupVal)* 1 ));
         // if( ( lookupVal + 1.3) - 1.2*length( tCol ) < .5 ){ return 0;}else{return 1;}
      }

      #pragma target 4.5
      #pragma vertex vert
      #pragma fragment frag
      #pragma multi_compile_shadowcaster
      #pragma fragmentoption ARB_precision_hint_fastest

      #include "UnityCG.cginc"
      sampler2D _MainTex;


            #include "../Chunks/struct16.cginc"
      #include "../Chunks/ShadowDiscardFunction.cginc"
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
    


            #include "../Chunks/Struct16.cginc"


            struct v2f { 
              float4 pos : SV_POSITION; 
              float2 uv : TEXCOORD0; 
            };
            float4 _Color;

            StructuredBuffer<Vert> _VertBuffer;
            StructuredBuffer<int> _TriBuffer;
            sampler2D _ColorMap;
            float _OutlineColor;
            float _OutlineAmount;
            float _WhichColor;

            float _TextureRepeat;
sampler2D _TextureMap;
      #include "../Chunks/hash.cginc"

            v2f vert ( uint vid : SV_VertexID )
            {
                v2f o;

        
                Vert v = _VertBuffer[_TriBuffer[vid]];
                float3 fPos = v.pos + v.nor * _OutlineAmount;
                o.pos = mul (UNITY_MATRIX_VP, float4(fPos,1.0f));

                
                o.uv = v.uv * (1./_TextureRepeat)+ floor(float2(hash(v.debug.x*10), hash(v.debug.x*20)) * _TextureRepeat)/_TextureRepeat;
            


                return o;
            }


      #include "../Chunks/ColorScheme.cginc"
            fixed4 frag (v2f v) : SV_Target
            {
                     float4 alphaT = tex2D(_TextureMap , v.uv.xy );

                if( alphaT.x < .5 ){
                    discard;
                }
                fixed4 col =GetGlobalColor( _OutlineColor );
                col *= 1;
                return col;
            }

            ENDCG
        }

    
  
  
  
  }

}



