Shader "Scenes/FullScene/TreeSurface"
{
    Properties
    {
        _BaseColor ("BaseColor", Color) = (1,1,1,1)
        _TipColor ("TipColor", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _BumpMap ("Bumpmap", 2D) = "bump" {}
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
        _TipColorMultiplier("Tip color multiplier" , Range(0,10)) = .5            
        _BaseColorMultiplier("Base color multiplier" , Range(0,10)) = .5          
        _AmountShown ("AmountShown", Range (0, 1)) = .5
        _ColorSize ("ColorSize", float) = .5
        _ColorBase ("ColorBase", float) = .5
    }
    SubShader
    {
        Cull Back
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Standard   vertex:vert addshadow

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

        sampler2D _MainTex;
            sampler2D _BumpMap;

        struct Input
        {
            float2 uv_MainTex;
            float2 uv_BumpMap;
            float  life;
            float playerDist;
        };

        half _Glossiness;
        half _Metallic;
        fixed4 _Color;


        

        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        // #pragma instancing_options assumeuniformscaling
        UNITY_INSTANCING_BUFFER_START(Props)
            // put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)

        // We use this value to blend every
        // vert from its center position
        // to its final position, based on 
        // how much of it should be 'show'
        // so it looks like its animating in!
        float3 lerpDown( float timeInBranch, float lerp){

            float lerpVal = lerp;

            lerpVal =  (.9-lerp * .9)-timeInBranch * .9;
            lerpVal = lerpVal;
            lerpVal *= 10;
            lerpVal = saturate(lerpVal);

            return 1-lerpVal;

        }
        



        
                  #include "UnityCG.cginc"
                  #include "AutoLight.cginc"


#include "../Chunks/convertUV.cginc"


            #include "../Chunks/Reflection.cginc"
            #include "../Chunks/noise.cginc"

            #include "../Chunks/SampleAudio.cginc"
            #include "../Chunks/ColorScheme.cginc"

        
    float _AmountShown;
    float _FadeFromTop;
    float _AlphaCutoff;
    float _TipColorMultiplier;
    float _BaseColorMultiplier;

    float _ColorSize;
    float _ColorBase;

    float4 _BaseColor;
    float4 _TipColor;

    float3 _PlayerSoul;
           
        void vert (inout appdata_full v,out Input o) {
            
            UNITY_INITIALIZE_OUTPUT(Input,o);

            // passing through our time created so we can chang our colors
            o.life = v.texcoord2.z;
            
            

                float4 t = length(SampleAudioLOD( o.life ));
            float fLerp = saturate(abs(-1+lerpDown(1-_AmountShown, v.texcoord2.z)));

            // lerping to zero so our shadow bias doesn't
            // falsely draw shapes
            v.normal.xyz = lerp( float3(0,0,0), v.normal , fLerp);

                                // Setting up the final position
                float3 fPos =  mul( unity_ObjectToWorld ,v.vertex).xyz;  


                float3 d = _PlayerSoul - fPos;
                o.playerDist = length(d);

                float3 extra = -normalize(d) * clamp(1 / length(d),0,.4);

                extra += length(t) * normalize(v.vertex.xyz) * .1;

                fPos += extra;
            v.vertex.xyz=  mul(unity_WorldToObject , float4( fPos,1)).xyz;

       
          
        }

        void surf (Input v, inout SurfaceOutputStandard o)
        {
            // Albedo comes from a texture tinted by color
            fixed4 c = tex2D (_MainTex, v.uv_MainTex ) * lerp( _BaseColor , _TipColor , v.life);


            float3 col  = GetGlobalColor( v.life * _ColorSize  + _ColorBase ) * SampleAudio(v.life);//* (spriteCol.a + v.playerDist);

            o.Albedo = col *  lerp(_BaseColorMultiplier, _TipColorMultiplier, v.life);
            // Metallic and smoothness come from slider variables
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
            o.Normal = UnpackNormal (tex2D (_BumpMap, v.uv_BumpMap));
            o.Alpha = c.a;

        }
        ENDCG
    }
    FallBack "Diffuse"
}
