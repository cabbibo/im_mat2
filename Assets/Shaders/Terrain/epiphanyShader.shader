﻿// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'

Shader "Terrain/epiphany" {
    Properties {
        _Color ("Color", Color) = (1,1,1,1)
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
        _Vertical("_Vertical",float)= 0

        _MainTex ("Texture", 2D) = "white" {}
    }

     SubShader {
    // COLOR PASS



       
    Pass {
      Tags{ "LightMode" = "ForwardBase" }
      Cull Off
        Tags { "Queue" = "Transparent" }
        LOD 200
        Blend One One
        ZWrite Off
        //Cull Back
        //Blend SrcAlpha OneMinusSrcAlpha 

        CGPROGRAM


        
        #pragma target 4.5
        #pragma vertex vert
        #pragma fragment frag


        #include "UnityCG.cginc"


 

        half _Glossiness;
        half _Metallic;
        fixed4 _Color;

        sampler2D _MainTex;
        float3 _PlayerPosition;

        sampler2D _HeightMap;
        float _MapSize;
        float _MapHeight;
        float _Vertical;


        float3 terrainWorldPos( float4 pos ){
            float3 wp = mul( unity_ObjectToWorld, pos ).xyz;
            float4 c = tex2Dlod(_HeightMap , float4(wp.xz * _MapSize,0,0) );
            wp.xyz += float3(0,1,0) * c.r * _MapHeight;
            return wp;
        }

        float4 terrainNewPos( float4 pos ){
            float4 wp = float4(terrainWorldPos( pos ) ,1 );
            return mul( unity_WorldToObject, wp);
        }



        float3 terrainGetNormal( float4 pos ){

            float delta =.0001;
            float4 dU = terrainNewPos( pos + float4(delta,0,0,0) );
            float4 dD = terrainNewPos( pos + float4(-delta,0,0,0) );
            float4 dL = terrainNewPos( pos + float4(0,delta,0,0) );
            float4 dR = terrainNewPos( pos + float4(0,-delta,0,0) );

            return normalize(cross(normalize(dU.xyz-dD.xyz),normalize(dR.xyz-dL.xyz)));


        }


        float4 terrainSampleColor( float4 pos ){
            float3 wp = mul( unity_ObjectToWorld, pos ).xyz;
            return tex2Dlod(_HeightMap , float4(wp.xz * _MapSize,0,0) );
        }

        #include "../Chunks/noise.cginc"
        #include "../Chunks/hsv.cginc"

         struct v2f {
            float4 pos : SV_POSITION;
            float3 nor : NORMAL;
            float3 worldPos : TEXCOORD1;
            float4 color : TEXCOORD3;
            float2 posUV : TEXCOORD4;
        };



         v2f vert(in appdata_full v) {

            v2f o;
            o.nor = terrainGetNormal( v.vertex );
            //o.uv = v.texcoord.xy;

            o.worldPos = terrainWorldPos( v.vertex ) - float4(0,_Vertical,0,0);
            o.posUV = float2(v.vertex.x , v.vertex.y);
            v.vertex = terrainNewPos( v.vertex )- float4(0,_Vertical,0,0);//mul( unity_WorldToObject, worldPos);
            o.color = terrainSampleColor( v.vertex );

            o.pos = mul(UNITY_MATRIX_VP, float4(o.worldPos,1));

            return o;
        }




        float _ScanTime;
        sampler2D _AudioMap;


        float4 frag(v2f v) : COLOR {

            
            float3 dif = v.worldPos - _PlayerPosition;

            float l = max(length( dif ) - 80,0);
            // Albedo comes from a texture tinted by color
            //fixed4 c = tex2D (_MainTex, v.uv_MainTex) * _Color;
            float3 viewDir = UNITY_MATRIX_IT_MV[2].xyz;
            //float3 viewDir  = mul(unity_CameraToWorld, float4(0,0,1,0));//UNITY_MATRIX_IT_MV[2].xyz;

            float m = dot( normalize(viewDir) , normalize(v.nor * 4  + float3(1,0,0) * sin(6* v.worldPos.x + sin( v.worldPos.y) * 30) + float3(1,0,0) * sin(6*v.worldPos.z)));
            float3 c1 = 0;
            float3 c2 = _Color.xyz* saturate(max( max( sin( v.worldPos.x ),0) , max( sin( v.worldPos.z ),0)) - .9)*2;// * sin(v.worldPos.z));

            float3 tCol = tex2D(_MainTex, v.posUV * 400).x;
            float on =  1-abs((length( dif )-_ScanTime * 1000)) * (.1-(pow(_ScanTime,.1) *.1)); //clamp(1-(length( dif )-_ScanTime * 2000) * .01,0,1);

            float scanVal = min( pow(_ScanTime,.3) , ( 1-_ScanTime));
            tCol = tex2D(_AudioMap,abs((tCol.x * .3+ length(dif) * .01 - .01 * _Time.y))  %.3).xyz;
            float fScan = (clamp(-(length(dif) - _ScanTime * _ScanTime*2000),0,100)/100) *scanVal *  clamp( (1-_ScanTime) * 1000 - length( dif) , 0 ,100)/100;
            //o.Emission.xyz =(clamp(-(length(dif) - _ScanTime * _ScanTime*2000),0,100)/100) *scanVal * 10* (v.nor * .5 + .5 ) * tCol * tCol;// saturate(on);//saturate(on) * min( pow(_ScanTime,.3) , ( 1-_ScanTime)) * 10* (v.nor * .5 + .5 ) * tCol * tCol;// / 1000;//lerp( 0 , c2 , l * .1);//_Color * (v.nor * .5 + .5)  - l;// hsv(v.normal.y * .5,1,1);
            float3 col = fScan * 4 * (v.nor * .5 + .5 ) * tCol * tCol;// saturate(on);//saturate(on) * min( pow(_ScanTime,.3) , ( 1-_ScanTime)) * 10* (v.nor * .5 + .5 ) * tCol * tCol;// / 1000;//lerp( 0 , c2 , l * .1);//_Color * (v.nor * .5 + .5)  - l;// hsv(v.normal.y * .5,1,1);

            
            return float4( col , 1);
        }
        
           ENDCG
    }
    
    


  }
    

    FallBack "Diffuse"
    
}