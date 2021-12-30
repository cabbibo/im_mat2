Shader "Scenes/Dandelion/DandelionBody"
{
    Properties {

  
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
          
            #include "../Chunks/struct16.cginc"
            #include "../Chunks/ShadowVertPassthrough.cginc"
            #include "../Chunks/PainterlyLight.cginc"
            #include "../Chunks/TriplanarTexture.cginc"
            #include "../Chunks/MapNormal.cginc"
            #include "../Chunks/Reflection.cginc"
            #include "../Chunks/SampleAudio.cginc"
            #include "../Chunks/ColorScheme.cginc"


            float _WhichColor;
            float _OverallMultiplier;

            fixed4 frag (v2f v) : SV_Target
            {

                fixed shadow = UNITY_SHADOW_ATTENUATION(v,v.world) * .5 + .5;

                float3 n = MapNormal( v , v.uv * _NormalSize , _NormalDepth );
                float3 reflectionColor = Reflection( v.pos , n );

                float m = dot( n, _WorldSpaceLightPos0.xyz);
                m *= shadow;
                m = saturate(m);
                m = 1-m;

                float3 col;
                
                float3 mapColor = GetGlobalColor( m * _ColorSize  + _ColorBase );
                float3 p = Painterly( m, v.uv.xy * _PaintSize );

                float3 refl = normalize(reflect( v.eye,n ));
                float rM = saturate(dot(refl,_WorldSpaceLightPos0.xyz));
                
                float4 audio = SampleAudio(length(reflectionColor.xyz) * .05 + v.uv.x * .2) * 2;
                
                
                col = mapColor*p;     
                col.xyz *= reflectionColor * 4;
                col  +=  (1-saturate(length(col.xyz)*10))* audio.xyz;
                col *= _OverallMultiplier;
                


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

    
  
  
  
  }


}