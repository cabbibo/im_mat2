
Shader "Terrain/Skybox1"
{

    Properties {

  
    _MainTex("_MainTex", 2D) = "white" {}
    _MapScale("MapScale", float) = 1
    _Lightness("Light_Lightness", float) = 1
    }


  SubShader{

            // Draw ourselves after all opaque geometry
       // Tags { "Queue" = "Geometry+10" }



     // Cull Off
    Pass{
CGPROGRAM
      
      #pragma target 4.5

      #pragma vertex vert
      #pragma fragment frag

      #include "UnityCG.cginc"
      
  

    sampler2D _SampleTexture;
    float _SampleSize;


            #include "../Chunks/SampleAudio.cginc"
            #include "../Chunks/ColorScheme.cginc"


      //A simple input struct for our pixel shader step containing a position.
      struct varyings {
          float4 pos      : SV_POSITION;
          float3 nor : NORMAL;
          float3 ro : TEXCOORD1;
          float3 rd : TEXCOORD2;
          float3 eye : TEXCOORD3;
          float3 localPos : TEXCOORD4;
          float3 worldNor : TEXCOORD5;
          float3 lightDir : TEXCOORD6;
          float4 grabPos : TEXCOORD7;
          float3 unrefracted : TEXCOORD8;
          
          
      };



            sampler2D _BackgroundTexture;


             struct appdata
            {
                float4 position : POSITION;
                float3 normal : NORMAL;
            };

//Our vertex function simply fetches a point from the buffer corresponding to the vertex index
//which we transform with the view-projection matrix before passing to the pixel program.
varyings vert ( appdata vertex ){



  varyings o;
     float4 p = vertex.position;
     float3 n =  vertex.normal;//_NormBuffer[id/3];

        float3 worldPos = mul (unity_ObjectToWorld, float4(p.xyz,1.0f)).xyz;
        o.pos = UnityObjectToClipPos (float4(p.xyz,1.0f));
        o.nor = n;//normalize(mul (unity_ObjectToWorld, float4(n.xyz,0.0f)));; 
        o.ro = p;//worldPos.xyz;
        o.rd  = mul(unity_ObjectToWorld, vertex.position).xyz - _WorldSpaceCameraPos;
        o.localPos = p.xyz;
      


      
  return o;

}



float3 hsv(float h, float s, float v)
{
  return lerp( float3( 1.0 , 1, 1 ) , clamp( ( abs( frac(
    h + float3( 3.0, 2.0, 1.0 ) / 3.0 ) * 6.0 - 3.0 ) - 1.0 ), 0.0, 1.0 ), s ) * v;
}


float _MapScale;

sampler2D _MainTex;
float _Lightness;
#include "../Chunks/noise.cginc"


//Pixel function returns a solid color for each point.
float4 frag (varyings v) : COLOR {
  float3 col =0;//hsv( float(v.face) * .3 , 1,1);

   float3 bf = normalize(abs(v.rd));
            bf /= dot(bf, (float3)1);

            float scale = 20;

    float2 tx = v.rd.yz * _MapScale;
    float2 ty = v.rd.zx * _MapScale;
    float2 tz = v.rd.xy * _MapScale;

    float n = noise( v.rd * .0002 + float3( 0, _Time.y * .4,0) ) +  .4 * noise (v.rd * .0006+ float3( 0, _Time.y * .2,0)) + .1 * noise(v.rd * .001+ float3( 0, _Time.y * .3,0))  ;//* .3 + noise(v.rd * .0001) * .6 + noise(v.rd * .0003);


    float4 cx = tex2D(_MainTex, tx )* bf.x* bf.x;
    float4 cy = tex2D(_MainTex, ty )* bf.y* bf.y;
    float4 cz = tex2D(_MainTex, tz )* bf.z* bf.z;

    col = (cx + cy + cz).xyz;
    col *= 10;

    col = GetGlobalColor( col.x * .1 ) * col.x;

    col *= SampleAudio(n*.1);//(_AudioMap,n * .1);

  //  col = saturate(col);
    //col *= saturate(normalize(v.rd).y * .3);

    //col *= 5;


    return fixed4( saturate(col.xyz) * _Lightness , 1);//saturate(float4(col,3*length(col) ));




}

      ENDCG

    }
  }

  //Fallback Off


}
