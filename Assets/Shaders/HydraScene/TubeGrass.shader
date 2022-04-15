Shader "Scenes/TubeGrass16"
{
    Properties {

  
       _TextureMap ("Texture", 2D) = "white" {}
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

     _TextureMapDimensions( "TextureMapDimensions" , Vector ) = (1,1,0,0)


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


            #include "../Chunks/ShadowVertPassthrough.cginc"


            #include "../Chunks/PainterlyLight.cginc"
            #include "../Chunks/TriplanarTexture.cginc"
            #include "../Chunks/MapNormal.cginc"
            #include "../Chunks/Reflection.cginc"
            #include "../Chunks/noise.cginc"


            #include "../Chunks/SampleAudio.cginc"
      
            #include "../Chunks/ColorScheme.cginc"


            float _WhichColor;

            fixed4 frag (v2f v) : SV_Target
            {

                fixed shadow = UNITY_SHADOW_ATTENUATION(v,v.world) * .5 + .5;

                float3 normal = -v.nor;// + (v.uv.x -.5) * v.tan;

                
                //normal -= UNITY_MATRIX_IT_MV[2].xyz;
                normal = normalize( normal );

                float3 n = MapNormal( v , v.uv * _NormalSize , _NormalDepth );


                if( dot( n ,UNITY_MATRIX_IT_MV[2].xyz) < 0 ){
                    n = -n;
                }
                
                float3 reflectionColor = Reflection( v.pos , n );

                float m = dot( -n, _WorldSpaceLightPos0.xyz);
                float baseM = m;
                m =m;// saturate(( m +1 )/2);

             //   m *= shadow;

                m = saturate(m);

                m = 1-m;

                if( v.uv.x + noise( v.uv.y * 100 ) * 1> 1.2 ){
                    //discard;
                }

                

              



                float3 col  = GetGlobalColor( m * _ColorSize  + _ColorBase + sin(v.debug.x) );
                float3 p = Painterly( m, v.uv.xy * 10000 );



            

               //col.xyz *= p * .3+ p * r;
               ////col *= baseM;
               ////col *= 10.;
               //col.xyz *=   r.xyz * 2;
               /// col *= col;


               //col.yxz = p.xyz*tex * .6 + .4;


               //col = p;
               //col = r*audio * 15;
               // col = col*p;

                
                float3 refl = normalize(reflect( v.eye,n ));
                float rM = saturate(dot(refl,_WorldSpaceLightPos0.xyz));
              //  col += col *pow(rM,5)*20;
                
               // float3 audio = SampleAudio(v.uv.x * .1 + p.x * .03 );

            

                float4 tex = tex2D(_TextureMap , v.uv.yx );
               // col.xyz *= (audio *audio*10 + 1);
               col.xyz *= reflectionColor * 4;

                    //float4 audio = SampleAudio(length(reflectionColor.xyz) * .05 + v.uv.x * .2) * 2;
                    float4 audio = SampleAudio( v.uv.x *.5 + (sin(v.debug.x)+1) * .1 + tex.r * .1  ) * 2;
                //col  +=  (1-saturate(length(col.xyz)*10));* audio.xyz * 10;
             
          //col = tex.rgb;
           col=GetGlobalColor( m * _ColorSize + tex.r * .2  + _ColorBase );
           
               //col.xyz *= reflectionColor * 4;
                col *=  audio.xyz * 1;


               // col = p;


            //col = m;


                if( tex.r >  .9  ){
                    discard;
                }
                //col= normal * .5 + .5;//tex.r;
                //col = sin( v.debug.x );


                //col = audio.xyz;

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



      #pragma target 4.5
      #pragma vertex vert
      #pragma fragment frag
      #pragma multi_compile_shadowcaster
      #pragma fragmentoption ARB_precision_hint_fastest

      #include "UnityCG.cginc"
      sampler2D _MainTex;

      #include "../Chunks/ShadowCasterPos.cginc"
   
      #include "../Chunks/struct16.cginc"
            #include "../Chunks/SampleAudio.cginc"

#include "AutoLight.cginc"

struct v2f{ 
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

            
        V2F_SHADOW_CASTER;

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

float2 _TextureMapDimensions;

float2 convertUV( float2 uv , float2 dimensions , float id  ){

  // if we haven't assigned, just pass!
  if( length(dimensions) < 2 ){
    return uv;
  }else{

    float xID = floor(((sin( id * 102121 ) +1)/2) * dimensions.x ) / dimensions.x;
    float yID = floor(((sin( id * 540511 ) +1)/2) * dimensions.y ) / dimensions.y;

    float2 fUV = uv *(1/dimensions) + float2(xID, yID);

    return fUV;
  }

  
}

v2f vert ( appdata_base input,uint vid : SV_VertexID )
{
    v2f o;

    UNITY_INITIALIZE_OUTPUT(v2f, o);

    Vert v = _VertBuffer[_TriBuffer[vid]];

    o.world = v.pos;
    o.uv = convertUV( v.uv , _TextureMapDimensions, v.debug.x );

    
        float4 position = ShadowCasterPos(v.pos, -v.nor);
        o.pos = UnityApplyLinearShadowBias(position);


    o.nor = v.nor;//normalize(cross(v0.pos - v1.pos , v0.pos - v2.pos ));
    o.debug = v.debug;
    o.eye = v.pos - _WorldSpaceCameraPos;
    o.screenPos = ComputeScreenPos(o.pos);
    o.vel = v.vel;

    float3 bi = cross(v.nor, v.tan);
    
    
    return o;
}
   


      float4 frag(v2f v) : COLOR
      {

                float4 tex = tex2D(_TextureMap , v.uv.yx );

                    float4 audio = SampleAudio( v.uv.x *.5 + (sin(v.debug.x)+1) * .1 + tex.r * .1  ) * 2;

                                 if( tex.r >  .9 ){
                    discard;
                }


        SHADOW_CASTER_FRAGMENT(i)
      }
      ENDCG
    }


































               // SHADOW PASS

/*
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

        
                Vert v = _VertBuffer[_TriBuffer[vid]];
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

    */
  
  
  
  }







}