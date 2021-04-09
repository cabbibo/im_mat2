Shader "Custom/Water"
{
	Properties
	{
		_Color("Color", Color) = (1, 1, 1, 1)
		_EdgeColor("Edge Color", Color) = (1, 1, 1, 1)
		_DepthFactor("Depth Factor", float) = 1.0
		_WaterSpeed("Water Speed", float) = 1.0
		_NoiseTex("Noise Texture", 2D) = "white" {}
		_MainTex("Main Texture", 2D) = "white" {}
	}

	SubShader
	{
        Tags
		{ 
			"Queue" = "Transparent"
		}

		// Grab the screen behind the object into _BackgroundTexture
        GrabPass
        {
            "_BackgroundTexture"
        }

		Pass
		{
            Blend SrcAlpha OneMinusSrcAlpha

			CGPROGRAM
            #include "UnityCG.cginc"

			#pragma vertex vert
			#pragma fragment frag
			
			// Properties
			float4 _Color;
			float4 _EdgeColor;
			float  _DepthFactor;
			float  _WaterSpeed;
			sampler2D _CameraDepthTexture;
			sampler2D _NoiseTex;
			sampler2D _MainTex;
			float4 _MainTex_ST;
			sampler2D _BackgroundTexture;

			struct vertexInput
			{
				float4 vertex : POSITION;
				float3 normal : NORMAL;
				float4 texCoord : TEXCOORD1;
			};

			struct vertexOutput
			{
				float4 pos : SV_POSITION;
				float2 texCoord : TEXCOORD0;
				float4 screenPos : TEXCOORD1;
                float3 nor : NORMAL;
			};

			vertexOutput vert(vertexInput input)
			{
				vertexOutput output;

				// convert to world space
				output.pos = UnityObjectToClipPos(input.vertex);

				// apply wave animation
				float noiseSample = tex2Dlod(_NoiseTex, float4(input.texCoord.xy, 0, 0));
				
				// compute depth
				output.screenPos = ComputeScreenPos(output.pos);

				// texture coordinates 
				output.texCoord = TRANSFORM_TEX(input.texCoord, _MainTex);

                output.nor = mul( unity_ObjectToWorld , float4( input.normal , 0 ) ).xyz;

				return output;
			}

			float4 frag(vertexOutput input) : COLOR
			{
				
                
                // apply depth texture
				float4 depthSample = SAMPLE_DEPTH_TEXTURE_PROJ(_CameraDepthTexture, input.screenPos);
				float depth = LinearEyeDepth(depthSample).r;


                float4 bgTex = tex2Dproj(_BackgroundTexture, input.screenPos);
				// create foamline
				float foamLine = 1 - saturate(_DepthFactor * (depth - input.screenPos.w));

				// sample main texture
				float4 albedo = tex2D(_MainTex, input.texCoord.xy + float2(-_Time.y * .01 * _WaterSpeed,0));
				float4 albedo2 = tex2D(_MainTex, input.texCoord.xy * 8 + float2(-_Time.y * .03 * _WaterSpeed,0));

                float depthVal = _DepthFactor*(depth - input.screenPos.w);
                float3 col = lerp( bgTex, _Color,depthVal);     
                if( depthVal - albedo.x - albedo2.x < .1 * dot(input.nor , float3(0,1,0))){
                    col = 1;
                }



                return float4(col,1);
			}

			ENDCG
		}
	}
}