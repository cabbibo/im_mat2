// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'

Shader "Debug/Terrain" {
    Properties {
        _Color ("Color", Color) = (1,1,1,1)
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
        _Vertical("_Vertical",float)= 0
        _FullColorMap ("Full Color Map", 2D) = "white" {}
    }

    SubShader{
        LOD 200

        CGPROGRAM


        #pragma target 4.5

        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Standard
        #pragma vertex vert 

        #include "UnityCG.cginc"


        struct Input {
            float2 uv1 : TEXCOORD5;
            float3 worldPosition;
            float4 color : TEXCOORD3;
            float3 nor : NORMAL;
            float4 colorMapData : TEXCOORD4;
        };

        half _Glossiness;
        half _Metallic;
        fixed4 _Color;

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


        void vert (inout appdata_full v, out Input o) {
                UNITY_INITIALIZE_OUTPUT(Input,o);
                o.nor = terrainGetNormal( v.vertex );
                o.uv1 = v.texcoord.xy;

                o.worldPosition = terrainWorldPos( v.vertex ) - float4(0,0,_Vertical,0);
                o.nor = terrainGetNormal( v.vertex );
                v.vertex = terrainNewPos( v.vertex )- float4(0,0,_Vertical,0);//mul( unity_WorldToObject, worldPos);
                o.color = terrainSampleColor( v.vertex );

           
      }




        sampler2D _TerrainInfo1;
        sampler2D _TerrainInfo2;
        sampler2D _TerrainInfo3;
        sampler2D _TerrainInfo4;
    sampler2D _FullColorMap;

    float4 getFullColor( float lookupVal , float2 uvPos  ){


                float maxID = 0;
                float secondMaxID = 0;
                float maxIDWeight = 0;
                float secondMaxIDWeight = 0;

                float4 info1 = tex2D( _TerrainInfo1 , float4( uvPos.x , uvPos.y , 0 , 0 ));
                float4 info2 = tex2D( _TerrainInfo2 , float4( uvPos.x , uvPos.y , 0 , 0 ));
                float4 info3 = tex2D( _TerrainInfo3 , float4( uvPos.x , uvPos.y , 0 , 0 ));
                float4 info4 = tex2D( _TerrainInfo4 , float4( uvPos.x , uvPos.y , 0 , 0 ));


                if( info1.r > maxIDWeight ){ maxID = 0; maxIDWeight = info1.r; }
                if( info1.g > maxIDWeight ){ maxID = 1; maxIDWeight = info1.g; }
                if( info1.b > maxIDWeight ){ maxID = 2; maxIDWeight = info1.b; }
                if( info1.a > maxIDWeight ){ maxID = 3; maxIDWeight = info1.a; }
                if( info2.r > maxIDWeight ){ maxID = 4; maxIDWeight = info2.r; }
                if( info2.g > maxIDWeight ){ maxID = 5; maxIDWeight = info2.g; }
                if( info2.b > maxIDWeight ){ maxID = 6; maxIDWeight = info2.b; }
                if( info2.a > maxIDWeight ){ maxID = 7; maxIDWeight = info2.a; }
                if( info3.r > maxIDWeight ){ maxID = 8; maxIDWeight = info3.r; }
                if( info3.g > maxIDWeight ){ maxID = 9; maxIDWeight = info3.g; }
                if( info3.b > maxIDWeight ){ maxID = 10; maxIDWeight = info3.b; }
                if( info3.a > maxIDWeight ){ maxID = 11; maxIDWeight = info3.a; }                
                if( info4.r > maxIDWeight ){ maxID = 12; maxIDWeight = info4.r; }
                if( info4.g > maxIDWeight ){ maxID = 13; maxIDWeight = info4.g; }
                if( info4.b > maxIDWeight ){ maxID = 14; maxIDWeight = info4.b; }
                if( info4.a > maxIDWeight ){ maxID = 15; maxIDWeight = info4.a; }


                if( info1.r > secondMaxIDWeight && info1.r != maxIDWeight ){ secondMaxID = 0; secondMaxIDWeight = info1.r; }
                if( info1.g > secondMaxIDWeight && info1.g != maxIDWeight ){ secondMaxID = 1; secondMaxIDWeight = info1.g; }
                if( info1.b > secondMaxIDWeight && info1.b != maxIDWeight ){ secondMaxID = 2; secondMaxIDWeight = info1.b; }
                if( info1.a > secondMaxIDWeight && info1.a != maxIDWeight ){ secondMaxID = 3; secondMaxIDWeight = info1.a; }
                if( info2.r > secondMaxIDWeight && info2.r != maxIDWeight ){ secondMaxID = 4; secondMaxIDWeight = info2.r; }
                if( info2.g > secondMaxIDWeight && info2.g != maxIDWeight ){ secondMaxID = 5; secondMaxIDWeight = info2.g; }
                if( info2.b > secondMaxIDWeight && info2.b != maxIDWeight ){ secondMaxID = 6; secondMaxIDWeight = info2.b; }
                if( info2.a > secondMaxIDWeight && info2.a != maxIDWeight ){ secondMaxID = 7; secondMaxIDWeight = info2.a; }
                if( info3.r > secondMaxIDWeight && info3.r != maxIDWeight ){ secondMaxID = 8; secondMaxIDWeight = info3.r; }
                if( info3.g > secondMaxIDWeight && info3.g != maxIDWeight ){ secondMaxID = 9; secondMaxIDWeight = info3.g; }
                if( info3.b > secondMaxIDWeight && info3.b != maxIDWeight ){ secondMaxID = 10; secondMaxIDWeight = info3.b; }
                if( info3.a > secondMaxIDWeight && info3.a != maxIDWeight ){ secondMaxID = 11; secondMaxIDWeight = info3.a; }                
                if( info4.r > secondMaxIDWeight && info4.r != maxIDWeight ){ secondMaxID = 12; secondMaxIDWeight = info4.r; }
                if( info4.g > secondMaxIDWeight && info4.g != maxIDWeight ){ secondMaxID = 13; secondMaxIDWeight = info4.g; }
                if( info4.b > secondMaxIDWeight && info4.b != maxIDWeight ){ secondMaxID = 14; secondMaxIDWeight = info4.b; }
                if( info4.a > secondMaxIDWeight && info4.a != maxIDWeight ){ secondMaxID = 15; secondMaxIDWeight = info4.a; }





           float4 cMap1 = tex2D( _FullColorMap , float2( lookupVal, 1-(maxID  + .5) / 16));
            float4 cMap2 = tex2D( _FullColorMap , float2( lookupVal, 1-(secondMaxID  + .5) / 16));

           return  cMap1  * maxIDWeight + cMap2 * secondMaxIDWeight;//(v.nor * .5 + .5) * v.color.w * _Color;//1;///_Color;//saturate(sin(length(dif) * .1 - _Time.y * 3));// / 1000;//lerp( 0 , c2 , l * .1);//_Color * (v.nor * .5 + .5)  - l;// hsv(v.normal.y * .5,1,1);


    }

        void surf (Input v, inout SurfaceOutputStandard o) {

            
            float3 dif = v.worldPosition - _PlayerPosition;

            float l = max(length( dif ) - 80,0);
            // Albedo comes from a texture tinted by color
            //fixed4 c = tex2D (_MainTex, v.uv_MainTex) * _Color;
 float3 viewDir = UNITY_MATRIX_IT_MV[2].xyz;
            //float3 viewDir  = mul(unity_CameraToWorld, float4(0,0,1,0));//UNITY_MATRIX_IT_MV[2].xyz;

                float m = dot( normalize(viewDir) , normalize(v.nor * 4  + float3(1,0,0) * sin(6* v.worldPosition.x + sin( v.worldPosition.y) * 30) + float3(1,0,0) * sin(6*v.worldPosition.z)));
            float3 c1 = 0;
            float3 c2 = _Color.xyz* saturate(max( max( sin( v.worldPosition.x ),0) , max( sin( v.worldPosition.z ),0)) - .9)*2;// * sin(v.worldPosition.z));


            o.Emission.xyz = getFullColor ( sin( v.worldPosition.y * .01) , v.worldPosition.xz * _MapSize);// cMap1  * v.colorMapData.z + cMap2 * v.colorMapData.w;//(v.nor * .5 + .5) * v.color.w * _Color;//1;///_Color;//saturate(sin(length(dif) * .1 - _Time.y * 3));// / 1000;//lerp( 0 , c2 , l * .1);//_Color * (v.nor * .5 + .5)  - l;// hsv(v.normal.y * .5,1,1);

            //if( ((v.worldPosition.y * .3)+ noise( v.worldPosition * .2 ) * .1)  % 1 < .8 ){ discard; }
            //v.color.w * 1;//float3(1,1,1);//c.rgb;
            // Metallic and smoothness come from slider variables
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;

    
        }

            ENDCG


    }

    FallBack "Diffuse"
}
