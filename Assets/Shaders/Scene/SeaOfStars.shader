﻿Shader "Final/SeaOfStars/16" {
  
  Properties {
    _Color ("Color", Color) = (1,1,1,1)
    _MainTex ("Sprite Texture", 2D) = "white" {}
    _ColorMap ("Color Map", 2D) = "white" {}
    _CubeMap( "Cube Map" , Cube )  = "defaulttexture" {}
        _HueStart("Hue start", float ) = 1
        _HueSize("Hue Size", float ) = 1
        _BrightnessMultiplier("Brightness", float ) = 1
  }

    SubShader {

        // COLOR PASS

        Pass {
            Tags{ "LightMode" = "ForwardBase" }
            Cull Off

            CGPROGRAM
            #pragma target 4.5
            #pragma vertex vert
            #pragma fragment frag
            
            #include "UnityCG.cginc"

               #include "Lighting.cginc"

            // compile shader into multiple variants, with and without shadows
            // (we don't care about any lightmaps yet, so skip these variants)
            #pragma multi_compile_fwdbase nolightmap nodirlightmap nodynlightmap novertexlight
            // shadow helper functions and macros
            #include "AutoLight.cginc" 

            struct Vert{
          float3 pos;
          float3 vel;
          float3 nor;
          float3 tan;
          float2 uv;
          float2 debug;
        };


        StructuredBuffer<Vert> _VertBuffer;
        StructuredBuffer<int> _TriBuffer;

        uniform sampler2D _MainTex;
        uniform sampler2D _ColorMap;
        uniform sampler2D _AudioMap;
        uniform samplerCUBE _CubeMap;


        float _HueStart;
        float _HueSize;
    
    
        float3 _Color;
    
        float3 _PlayerPosition;

        float _ClosestGPUCollisionID;
        float _ClosestGPUCollisionTime;
        float _BrightnessMultiplier;

        struct varyings {
            float4 pos      : SV_POSITION;
            float3 nor      : NORMAL;
            float2 uv       : TEXCOORD1;
            float3 eye      : TEXCOORD5;
            float3 worldPos : TEXCOORD6;
            float3 debug    : TEXCOORD7;
            float3 closest    : TEXCOORD8;
            float3 tan    : TEXCOORD9;
            float3 vel    : TEXCOORD10;
            float3 player    : TEXCOORD11;
            float2 uv2    : TEXCOORD12;
            UNITY_SHADOW_COORDS(2)
        };

        #include "../Chunks/hsv.cginc"
        #include "../Chunks/hash.cginc"

            varyings vert(uint id : SV_VertexID) {


                Vert v = _VertBuffer[_TriBuffer[id]];

                float3 fPos     =  v.pos;
                float3 fNor     =  v.nor;
                float2 fUV      =  v.uv;
                float3 fTan     =  v.tan;
                float2 debug    =  v.debug;

                varyings o;

                UNITY_INITIALIZE_OUTPUT(varyings, o);

                o.pos = mul(UNITY_MATRIX_VP, float4(fPos,1));
                o.worldPos = fPos;
                o.eye = _WorldSpaceCameraPos - fPos;
                o.nor = fNor;

                float2 center = fUV-float2(.5 , .5);
                float r = length(center);
                float a =  atan2(center.x, center.y);
                o.tan = normalize(normalize(fTan) *  -cos(a) +   normalize(cross(fNor,fTan))  * sin(a)) * r;
                o.uv =  fUV * (1./6.)+ floor(float2(hash(debug.x*10), hash(debug*20)) * 6)/6;

                o.uv2 = fUV;
                o.debug = float3(debug.x,debug.y,r);
                o.vel = v.vel;
                o.player = o.worldPos - _PlayerPosition;

                TRANSFER_SHADOW(o);

                return o;
            }

            float4 frag(varyings v) : COLOR {
        
                fixed shadow = UNITY_SHADOW_ATTENUATION(v,v.worldPos ) * .9 + .1 ;
                float4 d = tex2D(_MainTex,v.uv);
        //if( d.a < .9 ){discard;}
        if( length(d.xyz) > 1.5 ){discard;}



       // if( length(v.tan)> .5){ discard;}
        
        float3 fNor = normalize(v.nor + 1*sin(length(v.tan) * 30) *normalize(v.tan) );

        float3 lDir = normalize(_PlayerPosition - v.worldPos);
        float3 refl = reflect( lDir , v.nor );
        float rM  = dot( normalize( v.eye) , refl );
        float3 col = normalize(lDir) * .5 + .5;

        float3 eyeRefl = reflect( normalize(v.eye),v.nor);
        //float3 eyeRefl = refract( normalize(v.eye), fNor , .8 );

        float3 tCol = texCUBE( _CubeMap , eyeRefl );

        float cVal = max(length(v.player.xz) * .003 , -.2) * _HueSize+ _HueStart + rM * .1* _HueSize -.22;
        cVal = saturate( cVal );
        col = tCol*tCol* tex2D(_ColorMap, float2(cVal ,0)) / (.4 + ( .05 * length( v.player.xz)));//-rM;//v.nor * .5 + .5;// tCol;//*tCol * tex2D(_ColorMap,float2(rM*.1+.7 - v.debug.y * .1 ,.5 )).rgb;// *(1-rM);//hsv(rM*rM*rM * 2.1,.5,rM);// + normalize(refl) * .5+.5;
        //col = v.tan * .5 + .5;

        float3 aCol = tex2D(_AudioMap,float2(length(v.uv2-.5) * .2 + sin(v.debug.x) * .2 + .2,0)).xyz;
        col *= (length(v.vel)-4) * (length(v.vel)-4) + .4;
        //if( abs(v.debug.x - _ClosestGPUCollisionID) < .1 ){ col += 1 * saturate( 10 / (_Time.y - _ClosestGPUCollisionTime)); }
       // col = _Time.y -_ClosestGPUCollisionTime;
        //col += hsv(dot(v.eye,v.nor) * -.1,.6,1) * (1-length(col));
        col *= shadow;
        col *= 2*aCol;
        col *= _BrightnessMultiplier;
        
        return float4( col , 1.);
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
#include "../Chunks/hash.cginc"

sampler2D _MainTex;
  struct v2f {
        V2F_SHADOW_CASTER;
        float2 uv : TEXCOORD1;
      };

            struct Vert{
          float3 pos;
          float3 vel;
          float3 nor;
          float3 tan;
          float2 uv;
          float2 debug;
        };


        StructuredBuffer<Vert> _VertBuffer;
        StructuredBuffer<int> _TriBuffer;


      v2f vert(appdata_base i, uint id : SV_VertexID)
      {
        v2f o;


        Vert v = _VertBuffer[_TriBuffer[id]];
       
        o.uv =  v.uv;

        float2 debug = v.debug;
        o.uv = o.uv * (1./6.)+ floor(float2(hash(debug.x*10), hash(debug.x*20)) * 6)/6;
        o.pos = mul(UNITY_MATRIX_VP, float4(v.pos, 1));
        return o;
      }

      float4 frag(v2f i) : COLOR
      {
        float4 col = tex2D(_MainTex,i.uv);
        //if( col.a < .4){discard;}
         if( length(col.xyz) > 1.5 ){discard;}
        SHADOW_CASTER_FRAGMENT(i)
      }
      ENDCG
    }
  


    }

     FallBack "Diffuse"

}
