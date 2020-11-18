Shader "Final/Cave/Crystals"
{
    Properties
    {
        _ColorMap ("Color Map", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float3 eye : TEXCOORD1;
                float3 worldPos : TEXCOORD2;
                float id : TEXCOORD3;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            sampler2D _AudioMap;
            sampler2D _ColorMap;

            #include "../Chunks/noise.cginc"
            #include "../Chunks/hsv.cginc"

            v2f vert (appdata v , uint vid : SV_VertexID )
            {
                v2f o;
                o.worldPos = mul( unity_ObjectToWorld , v.vertex ).xyz;
                o.eye = o.worldPos - _WorldSpaceCameraPos;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);

                o.id = float(vid/(12*6*3));
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f v) : SV_Target
            {
                // sample the texture
                float3 col = 0;
                float3 ro = v.worldPos;
                float3 rd = normalize( v.eye);
                for( int i= 0; i < 20; i++ ){

                    float3 p = ro + .2*rd *  (float(i)/20);
                    float v = .3 * noise(p * 3);
                     v += noise(p);
                     v += noise( p * 20) * .01;

                    col += tex2D( _ColorMap ,  float2(v * .5 + .2 , 0 )) * tex2D(_AudioMap , float2(v * .5 + .2 , 0 )).xyz;//abs(v)/6;

                } 

                col = v.id;
                return float4( col , 1);
            }
            ENDCG
        }
    }
}
