Shader "Scenes/Hydra/eyeBallCenter"
{
    Properties
    {
      _HueStart("_HueStart", float ) = 8
       _CubeMap( "Cube Map" , Cube )  = "defaulttexture" {}
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {

            // Lighting/ Texture Pass
Stencil
{
Ref 4
Comp always
Pass replace
ZFail keep
}
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work

            #include "UnityCG.cginc"

samplerCUBE _CubeMap;
            #include "../Chunks/Reflection.cginc"
      
      #include "../Chunks/ColorScheme.cginc"
            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float3 worldNor : TEXCOORD1;
                float3 eye : TEXCOORD2;
                float3 worldPos : TEXCOORD3;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;


            sampler2D _ColorMap;
            float _WhichColor;
            float _HueStart;


            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);

                o.worldNor = normalize(mul(unity_ObjectToWorld, float4(v.normal,0)).xyz);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                o.eye = o.worldPos - _WorldSpaceCameraPos;
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);


                return o;
            }

            fixed4 frag (v2f v) : SV_Target
            {


                float3 refl = normalize(reflect( v.eye,v.worldNor ));
                float3 tCol = texCUBE(_CubeMap,refl);

                float rM = saturate(dot(refl,_WorldSpaceLightPos0.xyz));
                
                // sample the texture
                fixed4 col = GetGlobalColor(  _HueStart +pow(rM,10)*10  );

                col.xyz *= tCol;

                col += col *pow(rM,10)*100;
                return col;
            }
            ENDCG
        }
    }

    
  FallBack "Diffuse"
}
