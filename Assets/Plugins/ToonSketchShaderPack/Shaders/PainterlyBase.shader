// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Painterly/PainterlyBase"
{
    Properties {

        _ColorMap ("ColorMap", 2D) = "white" {}
        _NormalMap ("NormalMap", 2D) = "bump" {}
        _ShinyMap ("ShinyMap", 2D) = "black" {}
        _PLightMap("Painterly Light Map", 2D) = "white" {}

        _NoiseSize("Noise Size" , float ) = 10  
        _NoiseSpeed("NoiseSpeed" , float ) = 5
        _NoiseStepRate(" Noise Step Rate" , float) = 10
        _NoiseNormalExtrude(" Noise Normal Extrude" , float ) = .01 
        _NoiseExtrudeSubtractor("Noise Extrude Subtractor" , float ) = 0
        _NoiseTextureOffset(" Noise Texture Offset " , float) = .01
        _NoisePow("Noise Pow" , float ) = 1


        _ShadowColorMultiplier("_ShadowColorMultiplier", float) = 0

        _ShininessMultiplier("_ShininessMultiplier" , float) = 2

        _ColorStart("_ColorStart",float) = 0
        _ColorSize("_ColorSize",float) = 1

        _OutlineAmount("_OutlineAmount",float) = .3
        _OutlineColor("_OutlineColor",Color) = (0,0,0,1)
        
        _TriplanarMultiplier( "Triplanar Multiplier" , Vector) = (10,10,10)
        _TriplanarSharpness( "Triplanar Sharpness" , float) = 1
        _FullDarkCutoff( "FullDarkCutoff" , Range (0, 1)) = 1

        _TextureShadingWeights( "Texture Shading Weight" , Vector ) = ( 0,1,2,3)


        
        [Toggle(TRIPLANAR)] _Triplanar ("TRIPLANAR COORDINATES", Float) = 0
        [Toggle(NORMAL_MAP)] _Normal ("NORMAL MAPPED", Float) = 0
        [Toggle(PAINTERLY_LIGHT)] _PainterlyLight ("PAINTERLY LIGHT", Float) = 0
        [Toggle(SKINNED_MESH)] _SkinnedMesh("SKINNED MESH", Float) = 0

        _StencilMask ("Stencil Mask", Int) = 9


        
        
    }
 


CGINCLUDE

// This is the code that does the 'displacement' of the mesh
// And will need to be in every pass of the shader looking the same

float _NoiseSize;
float _NoiseSpeed;
float _NoiseStepRate;
float _NoiseNormalExtrude;
float _NoiseTextureOffset;
float _NoiseExtrudeSubtractor;
float _NoisePow;


// Getting Some simple Triangular Noise
float tri(in float x){return abs(frac(x)-.5);}
float3 tri3(in float3 p){return float3( tri(p.y+tri(p.z)), tri(p.z+tri(p.x)), tri(p.y+tri(p.x)));}
           
float triAdd( in float3 p ){ return (tri(p.x+tri(p.y+tri(p.z)))); }

float triangularNoise( float3 p ){

    float totalFog = 0;

    float noiseScale = 1;

    float3 tmpPos = p;

    float noiseContribution = 1;

    float3 offset = 0;

    p *= _NoiseSize;
    p *= 2;

   float speed = 1.1;
 
   p +=  tri3(p.xyz * .3 ) *1.6;
   totalFog += triAdd(p.yxz * .3) * .35;
    
   p +=  tri3(p.xyz * .4 + 121 ) * 1;
   totalFog += triAdd(p.yxz * 1) * .25;
    
   p +=  tri3(p.xyz * .8 + 121 ) * 1;
   totalFog += triAdd(p.yxz* 1.3) * .15;

  return totalFog;

}


// use our parameters to get our noise value
float noiseVal( float3 p){
    float n = triangularNoise( p.xyz * _NoiseSize + _NoiseSpeed * floor( _Time.y  * _NoiseStepRate)/_NoiseStepRate );
 return n;
}

// Use this noise value to extrude along our normal
// The extra parameter is for our outline shaders extra offset
float3 offsetPos( float3 p  , float3 nor , float nExtrude){
    return p + ( pow((noiseVal(p)  * _NoiseNormalExtrude),_NoisePow)-_NoiseExtrudeSubtractor + nExtrude ) * nor;
} 

ENDCG
 

    SubShader
    {
        Tags { "LightMode" = "ForwardBase" }
        LOD 100

        Cull Off


        /*

            MAIN 
            COLOR 
            PASS

        */
        Pass
        {


            // Giving our selves stencil info 
            // for our outline shader to use
            Stencil
            {
                Ref [_StencilMask]
                Comp always
                Pass replace
                ZFail keep
            }
            CGPROGRAM


            #pragma vertex vert
            #pragma fragment frag
            #pragma target 4.5
            #include "UnityCG.cginc"

            // Our shader feature definitions
            // to make our shader an uber shader
            #pragma shader_feature TRIPLANAR
            #pragma shader_feature NORMAL_MAP
            #pragma shader_feature SKINNED_MESH
            #pragma shader_feature PAINTERLY_LIGHT

            #pragma multi_compile_fwdbase nolightmap nodynlightmap novertexlight
            #pragma multi_compile_fwdbase
            #pragma fragmentoption ARB_precision_hint_fastest

			#include "Lighting.cginc"
			#include "AutoLight.cginc"  


            // Our honkin-tonkin varyings
            struct v2f { 
                float4 pos : SV_POSITION; 
                float3 nor : NORMAL;  
                float3 worldPos : TEXCOORD3;
                float3 localPos : TEXCOORD1;
                float3 localNor : TEXCOORD4;
                float2 uv  : TEXCOORD2; 
                float2 painterlyUV : TEXCOORD7;
                float2 shinyUV : TEXCOORD8;
                float2 normalUV : TEXCOORD9;
                half3 tspace0 : TEXCOORD11; // tangent.x, bitangent.x, normal.x
                half3 tspace1 : TEXCOORD12; // tangent.y, bitangent.y, normal.y
                half3 tspace2 : TEXCOORD13; // tangent.z, bitangent.z, normal.z
                half3 tang : TEXCOORD14; // tangent.z, bitangent.z, normal.z
                // in v2f struct;
                LIGHTING_COORDS(5,6)

            };

            // UNIFORMS
            float4 _Color;
            sampler2D _ColorMap;

            sampler2D _NormalMap;
            float4 _NormalMap_ST;
            
            sampler2D _PLightMap;
            float4 _PLightMap_ST;

            sampler2D _ShinyMap;
            float4 _ShinyMap_ST;
            float _ShininessMultiplier;

            samplerCUBE _CubeMap;

            float _ColorStart;
            float _ColorRandomSize;
            float _ColorSize;

            float _FullDarkCutoff;

            float _ShadowColorMultiplier;

            float4 _TextureShadingWeights;



            v2f vert ( appdata_full v )
            {
                v2f o;
                
                UNITY_INITIALIZE_OUTPUT(v2f, o);
                
                // Getting our offset position and passing it straight through
                float3 fPos = offsetPos( v.vertex.xyz , v.normal,0 );
                o.pos = UnityObjectToClipPos( float4( fPos,1) );//mul (UNITY_MATRIX_VP, float4(v.pos,1.0f));
                
                // Get an offset for our UV coordinate 
                // so it draws different every frame 
                float nVal = triangularNoise( v.vertex.xyz * _NoiseSize + _NoiseSpeed * floor( _Time.y  * _NoiseStepRate)/_NoiseStepRate );
                float nVal2 = triangularNoise( v.vertex.xyz * _NoiseSize + 1212.4144 + _NoiseSpeed * floor( _Time.y  * _NoiseStepRate)/_NoiseStepRate );
                o.uv = v.texcoord + float2(nVal,nVal2) * _NoiseTextureOffset;

             

                float3 worldNormal = normalize(mul( unity_ObjectToWorld, float4(v.normal,0) ).xyz);
                o.nor = worldNormal;
                o.worldPos =mul( unity_ObjectToWorld, fPos).xyz;


                /*
                    Passing through information for normal mapping!
                */
                half3 wNormal = worldNormal;
                half3 wTangent = mul( unity_ObjectToWorld,float4(v.tangent.xyz,0) ).xyz* v.tangent.w;
                // compute bitangent from cross product of normal and tangent
                //half tangentSign = tangent.w * unity_WorldTransformParams.w;
                half3 wBitangent = cross(wNormal, wTangent);// * tangentSign;
                // output the tangent space matrix
                o.tspace0 = half3(wTangent.x, wBitangent.x, wNormal.x);
                o.tspace1 = half3(wTangent.y, wBitangent.y, wNormal.y);
                o.tspace2 = half3(wTangent.z, wBitangent.z, wNormal.z);

                o.painterlyUV = TRANSFORM_TEX(o.uv, _PLightMap);
                o.shinyUV = TRANSFORM_TEX(o.uv, _ShinyMap);
                o.normalUV = TRANSFORM_TEX(o.uv, _NormalMap);

                // if we have a skinned mesh make sure
                // we pass through the correct local position and normal
                // that we added with the 'AddOriginalValuesToSkinnedMesh' script
                #ifdef SKINNED_MESH 
                    o.localNor = v.texcoord3;
                    o.localPos = v.texcoord2;
                #else
                    o.localNor = fPos;
                    o.localPos = v.normal;
                #endif
                
                // Pass our lighting through to our fragment shader
                 UNITY_TRANSFER_LIGHTING(o , o.worldPos );
                return o;
            }




    // Sampling our triplanar texture for our 'sketch' shader
    float3 _TriplanarMultiplier;
    float _TriplanarSharpness;

    float4 triplanarSample(float3 p , float3 n){
        float nVal = triangularNoise( p.xyz * _NoiseSize + _NoiseSpeed * floor( _Time.y  * _NoiseStepRate)/_NoiseStepRate );
        float nVal2 = triangularNoise( p.xyz * _NoiseSize + 1212.4144 + _NoiseSpeed * floor( _Time.y  * _NoiseStepRate)/_NoiseStepRate );
             
             
        half3 blend = pow(abs(n),_TriplanarSharpness) ;;
        
        // make sure the weights sum up to 1 (divide by sum of x+y+z)
        blend /= dot(blend,1.0);
        // read the three texture projections, for x,y,z axes
        fixed4 cx = tex2D(_PLightMap,(p.yz * _TriplanarMultiplier.x + float2(nVal,nVal2) * _NoiseTextureOffset) % 1);
        fixed4 cy = tex2D(_PLightMap,(p.xz * _TriplanarMultiplier.y + float2(nVal,nVal2) * _NoiseTextureOffset) % 1);
        fixed4 cz = tex2D(_PLightMap,(p.xy * _TriplanarMultiplier.z + float2(nVal,nVal2) * _NoiseTextureOffset) % 1);
        // blend the textures based on weights
        fixed4 c = cx * blend.x + cy * blend.y + cz * blend.z;
        return c;

    }



    // Trying to get a normal map triplanr
    float3 triplanarNormal(float3 p , float3 n, float3 t0,float3 t1,float3 t2){

          float nVal = noise( p.xyz * _NoiseSize + _NoiseSpeed * floor( _Time.y  * _NoiseStepRate)/_NoiseStepRate );
        float nVal2 = noise( p.xyz * _NoiseSize + 1212.4144 + _NoiseSpeed * floor( _Time.y  * _NoiseStepRate)/_NoiseStepRate );
        
        
        half3 blend =  pow(abs(n),_TriplanarSharpness) ;
        // make sure the weights sum up to 1 (divide by sum of x+y+z)
        blend /= dot(blend,1.0);
        // read the three texture projections, for x,y,z axes
        half3 cx = UnpackNormal(tex2D(_NormalMap,(p.yz * _TriplanarMultiplier.x + float2(nVal,nVal2) * _NoiseTextureOffset) % 1));
        half3 cy = UnpackNormal(tex2D(_NormalMap,(p.xz * _TriplanarMultiplier.y + float2(nVal,nVal2) * _NoiseTextureOffset) % 1));
        half3 cz = UnpackNormal(tex2D(_NormalMap,(p.xy * _TriplanarMultiplier.z + float2(nVal,nVal2) * _NoiseTextureOffset) % 1));
        // blend the textures based on weights
        half3 c = cx * blend.x + cy * blend.y + cz * blend.z;

        half3 worldNormal;
        worldNormal.x = dot(t0, c);
        worldNormal.y = dot(t1, c);
        worldNormal.z = dot(t2, c);
        return c;
         


    }
 


            fixed4 frag (v2f v) : SV_Target
            {

                float3 fNor = v.nor;

                #if defined(NORMAL_MAP) && !defined(TRIPLANAR)

                    // Unpacking and setting our normal map
                    half3 tnormal =UnpackNormal(tex2D(_NormalMap, v.normalUV));
                    half3 worldNormal;
                    worldNormal.x = dot(v.tspace0, tnormal);
                    worldNormal.y = dot(v.tspace1, tnormal);
                    worldNormal.z = dot(v.tspace2, tnormal);
                    fNor = worldNormal;


                #endif

                #if defined(NORMAL_MAP) && defined(TRIPLANAR)
                
                    // Unpacking and setting our normal map as a triplanar cunction
                    fNor = triplanarNormal( v.localPos , v.localNor , v.tspace0 , v.tspace1, v.tspace2);
                #endif
        


                // Getting our shadows
                 float shadow = UNITY_SHADOW_ATTENUATION(v,v.worldPos);
               
                // getting the 'Match' value. in this case 
                // a combo of our shadows and how much our face matches
                // a directional light sorce
                float m =   1-dot(_WorldSpaceLightPos0.xyz , fNor);
                m = 1-((1-m)*shadow);
                m *= 3;


                float4 p;
                

                #ifdef TRIPLANAR 
                    p = triplanarSample( v.localPos , v.localNor );
                #else
                    p = tex2D( _PLightMap , v.painterlyUV );
                #endif
            
                // set our final lighting color
                // to the match color
                float fLCol = 1-m/3;

                // If we have painterly light set,
                // use the 'weights' of each part of the 
                // the texture. we have packed all the 
                // values into the r,g,b,a channels of a texture
                // so this essential unpacks them and gets how much 
                // each layer 'contributes' to the final lighting
                #ifdef PAINTERLY_LIGHT
                    float4 weights = 0;
                    if( m < _TextureShadingWeights.x){
                        weights = float4(1 , 0 , 0, 0);
                    }else if( m >= _TextureShadingWeights.x && m < _TextureShadingWeights.y){
                        weights = float4(1-(m-_TextureShadingWeights.x)/(_TextureShadingWeights.y-_TextureShadingWeights.x) ,(m-_TextureShadingWeights.x)/(_TextureShadingWeights.y-_TextureShadingWeights.x) , 0, 0);//lerp( p.x , p.y , m );
                    }else if( m >= _TextureShadingWeights.y && m < _TextureShadingWeights.z){
                        weights = float4(0,1-(m-_TextureShadingWeights.y)/(_TextureShadingWeights.z-_TextureShadingWeights.y) , (m-_TextureShadingWeights.y)/(_TextureShadingWeights.z-_TextureShadingWeights.y) ,  0);
                    }else if( m >= _TextureShadingWeights.z && m < _TextureShadingWeights.w){
                        weights = float4(0,0,1-(m-_TextureShadingWeights.z)/(_TextureShadingWeights.w-_TextureShadingWeights.z) , (m-_TextureShadingWeights.z)/(_TextureShadingWeights.w-_TextureShadingWeights.z) );
                    }else{
                        weights = float4(0,0,0 , 1);
                    }

                    fLCol  = p.x * weights.x;
                    fLCol += p.y * weights.y;
                    fLCol += p.z * weights.z;
                    fLCol += p.w * weights.w;
                    fLCol = 1-fLCol;
                #endif

                // Cutoff for full darkness 
                if( m > _FullDarkCutoff * 3){fLCol = 0;}


                // Using all our lighting info to get our toon color!
                float3 toonCol = tex2D( _ColorMap , float2( saturate((1-fLCol) * _ColorSize + _ColorStart) , 0) ).xyz;
                toonCol *= lerp(1,_ShadowColorMultiplier,1-fLCol);
               
               
                // Getting the 'shininess' value from the lighting in our scene
                // and how shiny it should be from our shiny map
                float3 shiny = tex2D(_ShinyMap,v.shinyUV);
                half3 worldViewDir = normalize(UnityWorldSpaceViewDir(v.worldPos));
                half3 worldRefl = refract(worldViewDir, fNor,.8);
                half4 skyData = UNITY_SAMPLE_TEXCUBE(unity_SpecCube0, -worldRefl);
                half3 skyColor = DecodeHDR (skyData, unity_SpecCube0_HDR); 
                float3 tCol = skyColor;

                float3 fCol= lerp( toonCol ,  tCol * toonCol * _ShininessMultiplier , saturate(shiny.x * shiny.x * shiny.x * 30));//*shiny.x * fLCol;//fLCol*s3* skyColor;//v.nor * .5 + .5;
                
          
                fixed4 col = float4(fCol,1);//fLCol;//float4( i.nor * .5 + .5 , 1);//tex2D(_MainTex, i.uv);
                return col;
            }

            ENDCG
        }

        


        // Outline Pass
        Pass
        {

            Cull OFF
            ZWrite ON
            ZTest ON

            // Here is where we set the values 
            // so the outline will only show *outside* 
            // the object
            Stencil
            {
                Ref [_StencilMask]
                Comp notequal
                Fail keep
                Pass replace
            }
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 4.5

            #include "UnityCG.cginc"
            


            struct Vert{
                float3 pos;
                float3 vel;
                float3 nor;
                float3 tan;
                float2 uv;
                float2 debug;
            };


            struct v2f { 
                float4 pos : SV_POSITION; 
            };
            float4 _OutlineColor;
            float _OutlineAmount;

    
            v2f vert ( appdata_full v )
            {
                v2f o;
                
                float3 fPos = offsetPos( v.vertex.xyz , v.normal , _OutlineAmount );
                o.pos = UnityObjectToClipPos( float4( fPos,1) );
                o.pos = UnityObjectToClipPos (float4(fPos,1.0f));


                return o;
            }

            fixed4 frag (v2f v) : SV_Target
            {
                fixed4 col = _OutlineColor;
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

            struct v2f {
                V2F_SHADOW_CASTER;
            };

            


            float4 ShadowCasterPos (float3 vertex, float3 normal) {
                float4 clipPos;
                
                // Important to match MVP transform precision exactly while rendering
                // into the depth texture, so branch on normal bias being zero.
                if (unity_LightShadowBias.z != 0.0) {
                    float3 wPos     = mul( unity_ObjectToWorld ,float4(vertex.xyz,1)).xyz;
                    float3 wNormal  = normalize(mul( unity_ObjectToWorld ,float4(normal,0)).xyz);
                    float3 wLight = normalize(UnityWorldSpaceLightDir(wPos));

                    // apply normal offset bias (inset position along the normal)
                    // bias needs to be scaled by sine between normal and light direction
                    // (http://the-witness.net/news/2013/09/shadow-mapping-summary-part-1/)
                    //
                    // unity_LightShadowBias.z contains user-specified normal offset amount
                    // scaled by world space texel size.

                    float shadowCos = dot(wNormal, wLight);
                    float shadowSine = sqrt(1 - shadowCos * shadowCos);
                    float normalBias = unity_LightShadowBias.z * shadowSine;

                    wPos -= wNormal * normalBias;

                    clipPos = mul(UNITY_MATRIX_VP, float4(wPos, 1));
                }else {
                    clipPos = UnityObjectToClipPos(vertex);
                }
                return clipPos;
            }


            v2f vert(appdata_base v)
            {
                v2f o;    
                float3 fPos = offsetPos(v.vertex.xyz,v.normal.xyz,0);

                float3 fNor = -v.normal;
                float4 position =  ShadowCasterPos(fPos, fNor);
                o.pos = UnityApplyLinearShadowBias(position);
              
                return o;
            }

            float4 frag(v2f i) : COLOR
            {
                SHADOW_CASTER_FRAGMENT(i)
            }
            
            ENDCG
        }

        

        
        
    }

    Fallback "VertexLit"
}