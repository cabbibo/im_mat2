Shader "Scenes/DandelionScene/dandyTip"
{
    Properties {

    _MainTex ("Texture", 2D) = "white" {}

    
      _ColorSize("_ColorSize", float ) = 0.5
      _ColorBase("_ColorBase", float ) = 0
      _OutlineColor("_OutlineColor", float ) = 0
      _OutlineAmount("_OutlineAmount", float ) = .16
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
            // make fog work
            #pragma multi_compile_fogV
 #pragma multi_compile_fwdbase nolightmap nodirlightmap nodynlightmap novertexlight

      #include "UnityCG.cginc"
      #include "AutoLight.cginc"
    


            #include "../Chunks/Struct16.cginc"
            #include "../Chunks/Reflection.cginc"
            #include "../Chunks/SampleAudio.cginc"
            #include "../Chunks/ColorScheme.cginc"
            #include "../Chunks/PainterlyLight.cginc"
            #include "../Chunks/FadeIn.cginc"


            sampler2D _MainTex;
            float _OverallMultiplier;

            struct v2f { 
              float4 pos : SV_POSITION; 
              float3 nor : NORMAL;
              float2 uv :TEXCOORD0; 
              float3 worldPos :TEXCOORD1;
              float2 debug :TEXCOORD3;
              float id :TEXCOORD4;
              float4 whereInTip : TEXCOORD5;
              UNITY_SHADOW_COORDS(2)
            };
            float4 _Color;
            float _HueStart;

            StructuredBuffer<Vert> _VertBuffer;
            StructuredBuffer<int> _TriBuffer;

            v2f vert ( uint vid : SV_VertexID )
            {
                v2f o;


                UNITY_INITIALIZE_OUTPUT(v2f, o);
                Vert v = _VertBuffer[_TriBuffer[vid]];
                o.pos = mul (UNITY_MATRIX_VP, float4(v.pos,1.0f));


                o.whereInTip.x = vid / 9;
                o.whereInTip.y = vid % 9;
                o.whereInTip.z = hash(o.whereInTip.x);


                o.nor = v.nor;

                float oX = floor(hash( (o.whereInTip.x) * 10 )*6)/6;
                float oY = floor(hash( (o.whereInTip.x) * 51 )*6)/6;
                o.uv = (v.uv * 1/6) + float2(oX,oY);
                o.worldPos = v.pos;
                o.debug = v.debug;
                o.id = vid / 12;


                UNITY_TRANSFER_SHADOW(o,o.worldPos);

                return o;
            }

            fixed4 frag (v2f v) : SV_Target
            {
                // sample the texture
                fixed shadow = UNITY_SHADOW_ATTENUATION(v,v.worldPos) * .5 + .5;
                float val = -dot(normalize(_WorldSpaceLightPos0.xyz),normalize(v.nor));// -DoShadowDiscard( i.worldPos , i.uv , i.nor );

                 float4 tCol = tex2D(_MainTex, v.uv );

                
                 float vL = length(v.uv-.5) ;
                 if( length( tCol ) < .5 && v.whereInTip.y < 6 ){ discard; }

                float match = dot(normalize(_WorldSpaceLightPos0.xyz), normalize(v.nor));
                 //if( vL > .4 ){ discard; }
                fixed4 col =  0;
            
            
            
                col = GetGlobalColor( tCol.x * .2 + v.debug.y *.3+ .8  + sin( v.whereInTip.z ) * .04 );//tex2D(_ColorMap , float2(tCol.x * .3 + .2 + v.whereInTip.z  * .3,0)) * shadow;

              if( v.whereInTip.y >= 6 ){  
                col = GetGlobalColor( v.uv.y * .2 + .1  );//tex2D(_ColorMap , float2(tCol.x * .3 + .2 + v.whereInTip.z  * .3,0)) * shadow;
              }

              col *= shadow * shadow;
              col  *= _OverallMultiplier;

              
                FadeDiscard( v.worldPos * 100);
              
                 // if( v.debug.x > .5 ){ col =float4(1,0,0,1);}
                return col;
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

     

      #include "../Chunks/Struct16.cginc"

      #include "../Chunks/ShadowCasterPos.cginc"

            #include "../Chunks/FadeIn.cginc"
      StructuredBuffer<Vert> _VertBuffer;
      StructuredBuffer<int> _TriBuffer;

      struct v2f {
        V2F_SHADOW_CASTER;
        float3 nor : NORMAL;
        float3 worldPos : TEXCOORD1;
        float2 uv : TEXCOORD0;
        float  idInTip : TEXCOORD2;
      };


      v2f vert(appdata_base input, uint id : SV_VertexID)
      {
        v2f o;
        Vert v = _VertBuffer[_TriBuffer[id]];

        float4 position = ShadowCasterPos(v.pos, -v.nor);
        o.pos = UnityApplyLinearShadowBias(position);
        o.worldPos = v.pos;
       
        int tipID = id / 9;
        int idInTip = id % 9;

        float oX = floor(hash( (tipID) * 10 )*6)/6;
        float oY = floor(hash( (tipID) * 51 )*6)/6;
        o.uv = (v.uv * 1/6) + float2(oX,oY);
        o.idInTip = idInTip;

        return o;
      }

      float4 frag(v2f v) : COLOR
      {



                FadeDiscard( v.worldPos * 100);
        float4 tCol = tex2D(_MainTex, v.uv );
        if( tCol.a < .5 && v.idInTip < 6 ){ discard; }

        SHADOW_CASTER_FRAGMENT(i)
      }


      ENDCG
    }
  
    


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


            #include "../Chunks/FadeIn.cginc"

            struct v2f { 
              float4 pos : SV_POSITION; 
        float2 uv : TEXCOORD0;
        float  idInTip : TEXCOORD2;
        float3  worldPos : TEXCOORD3;
            };
            float4 _Color;

            StructuredBuffer<Vert> _VertBuffer;
            StructuredBuffer<int> _TriBuffer;
            sampler2D _MainTex;
            float _OutlineColor;
            float _OutlineAmount;
            float _WhichColor;

            v2f vert ( uint vid : SV_VertexID )
            {
                v2f o;
   
                Vert v = _VertBuffer[_TriBuffer[vid]];
                float3 fPos = v.pos + v.nor * _OutlineAmount;
                o.pos = mul (UNITY_MATRIX_VP, float4(fPos,1.0f));

                      int tipID = vid / 9;
            int idInTip =vid % 9;

            float oX = floor(hash( (tipID) * 10 )*6)/6;
            float oY = floor(hash( (tipID) * 51 )*6)/6;
            o.uv = (v.uv * 1/6) + float2(oX,oY);
            o.idInTip = idInTip;
            o.worldPos = v.pos;
            


                return o;
            }

      
            #include "../Chunks/ColorScheme.cginc"
            fixed4 frag (v2f v) : SV_Target
            {
              
                fixed4 col =GetGlobalColor( _OutlineColor );
                col *= 1;   
                float4 tCol = tex2D(_MainTex,v.uv );
                if( tCol.a < .5 && v.idInTip < 6 ){ discard; }
                
                FadeDiscard( v.worldPos * 100);

                return col;
            }

            ENDCG
        }


    }




}