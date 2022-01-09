Shader "FantasyLight/RaysProjection"
{
    Properties
    {
    
    _ValueMultiplier ("Value Multiplier", float) = .01
    _RaySize ("Ray Size", float) = .01
    
 
    
    
    _RayLocationOsscilationSpeed ("Ray Location Osscilation Speed", float) = .01
    _RayLocationOsscilationSize ("Ray Location Osscilation Size", float) = .01

    _CenterWeight("_CenterWeight",float) = 1
    _EdgeFadeSharpness("_EdgeFadeSharpness", float) = 1
    
    _DustMap("_DustMap", 2D) = "white" {}
    _DustMapSize("_DustMapSize", Vector) = (1,1,0,0)
    _EmissionMap("_EmissionMap", 2D) = "white" {}
 
    _StartColor ("StartColor", Color) = (1,1,1,1)
    _EndColor ("EndColor", Color) = (1,1,1,1)

    _FadeInSpeed("_FadeInSpeed", float) = 1
    _FadeOutSpeed("_FadeOutSpeed", float) = 1

    _DistanceFadeStart("_DistanceFadeStart" , float ) = 1
    _DistanceFadeSpeed("_DistanceFadeSpeed" , float ) = 1
    

    _RaySharpness ("_RaySharpness", float) = .01
    
    [Toggle(CIRCLE_OR_SQUARE)] _CircleOrSquare("Circle Or Square", Float) = 0

    }
    SubShader
    {
        // inside SubShader
Tags { "Queue"="Transparent+10" "RenderType"="Transparent" "IgnoreProjector"="True" }

// inside Pass
ZWrite Off
Blend One One
        LOD 100

        Pass
        {

            Cull Off
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"
            #pragma shader_feature CIRCLE_OR_SQUARE

      
            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float2 uv2 : TEXCOORD7;
                float depth : TEXCOORD1;
                float4 vertex : SV_POSITION;
                float3 endPos : TEXCOORD2;
                float3 startPos : TEXCOORD4;
                float3 world : TEXCOORD3;
                float edgeFade : TEXCOORD5;
                float4 emission : TEXCOORD6;
                float4 screenPos : TEXCOORD8;
            };

     
      float hash( float n ){
        return frac(sin(n)*4758.5453);
      }

       sampler2D _DepthTexture;

       sampler2D _EmissionMap;
       sampler2D _DustMap;

       float _RaySize;
       float _ValueMultiplier;
       float4 _Color;

       float _RayLocationOsscilationSpeed;
       float _RayLocationOsscilationSize;

        float4x4 _CameraMatrix;
        float4x4 _CameraProjectionInverse;
        float4x4 _CameraProjection;
        float2 _CameraSize;
        float _CameraNear;
        float _CameraFar;

        float _FadeIn;
        float _FadeOut;

        float _CenterWeight;
        float4 _StartColor;
        float4 _EndColor;

        float _FadeInSpeed;
        float _FadeInStart;

        float _FadeOutSpeed;
        float _FadeOutStart;

        float _EdgeFadeSharpness;

        float _DistanceFadeStart;
        float _DistanceFadeSpeed;
        float _RaySharpness;

        float2 _DustMapSize;

                        
float perspectiveDepthToViewZ( const in float invClipZ, const in float near, const in float far ) {
	return ( near * far ) / ( ( far - near ) * invClipZ - far );
}
    
        // Getting final position using info
        float3 getPos( float x , float y , float3 left , float3 up , float3 forward , float3 pos){
            return 2*(x - .5) * _CameraSize.x * left + 2*(y-.5) * _CameraSize.y * up + forward * _CameraNear + pos;
        }

            v2f vert ( uint vid : SV_VertexID )
            {
                v2f o;

                int pID = vid/6;


                // Getting our random position

                 #ifdef CIRCLE_OR_SQUARE
                    float x  = hash( pID * 100 ) * 2 -1;
                    float y  = hash( pID * 121 ) * 2 -1;

                    // Getting our value to fade out on the edges
                    float edgeFade =min(1-abs(x) , 1-abs(y) );
                #else
                
                    float r = pow(abs(sin(pID * 441.414)),_CenterWeight);
                    float a = hash(pID * 1212) * 2 * 3.14159;

                    float x = sin(a) * r;
                    float y = -cos(a) * r;

                    // Getting our value to fade out on the edges
                    float edgeFade = 1-r;
                #endif


            
                x = (float(pID) / 100)%2;
                x -= 1;
                y = hash(pID * 5512)  * 2 - 1;

                // Adding some movement if we want it
                //x += _RayLocationOsscilationSize * sin(_RayLocationOsscilationSpeed*_Time.y * hash(pID * 1521));
                //y += _RayLocationOsscilationSize * sin(_RayLocationOsscilationSpeed*_Time.y * hash(pID * 5131));

                //y = -y;
                float3 rayPers = mul(_CameraProjectionInverse, float4(-x,-y,1,1) * _CameraFar).xyz;
                // normalizing to sample correctly
                x +=1;
                x /=2;

                y +=1;
                y /=2;


                // Using the camera matrix to get our spread
                float3 forward  = normalize(mul( _CameraMatrix , float4(0,0,1,0)).xyz);
                float3 up       = normalize(mul( _CameraMatrix , float4(0,1,0,0)).xyz);
                float3 left     = normalize(mul( _CameraMatrix , float4(1,0,0,0)).xyz);
                float3 pos = mul(_CameraMatrix,float4(0,0,0,1)).xyz;


                float4 direction1 = mul( _CameraProjectionInverse, float4(x,y,_CameraNear,1.0));




                float3 direction = direction1.xyz;
                //direction /= direction1.w;
               /// direction = normalize(float3(x,y,_CameraNear));
                direction = mul(_CameraMatrix,float4(rayPers,0)).xyz;
                direction = normalize(direction);

               // direction /= direction.w;





                float depth = tex2Dlod(_DepthTexture,float4(x,y,0,0));

                depth = perspectiveDepthToViewZ(1-depth, _CameraNear , _CameraFar);
                o.depth = depth;



                // Getting the emission color at this location for 'stained glass' type effects
                float4 emission = tex2Dlod(_EmissionMap,float4(x,y,0,0));

                o.emission = emission;
                
                // Getting our ray origin
                float3 startPos = pos - direction * _CameraNear;//getPos( x , y , left, up , forward , pos );//2*(x - .5) * _CameraSize.x * left + 2*(y-.5) * _CameraSize.y * up + forward * _CameraNear + pos;

                // Getting where our ray hits by using the camera
                //float3 endPos = startPos + forward * (1-depth) * ( _CameraFar - _CameraNear);
                float3 endPos = startPos + direction.xyz * depth;// * ( _CameraFar - _CameraNear);;//* (1-depth) * ( _CameraFar - _CameraNear);

                // getting the up down direciton that will 'billboard' our ray
                float3 l = .1*_RaySize * normalize(cross(UNITY_MATRIX_V[2], forward));

                

                // Getting 4 positions of quad corners
                float3 p1 = startPos;
                float3 p2 = endPos   - l;
                float3 p3 = endPos   + l;


                float3 fPos = 0;
                float2 fUV = 0;


                int which = vid % 3;

                // Using our id to assign these quad positions
                if( which == 0 ){
                    fPos = p1; fUV = float2(.5,0);
                }else if( which == 1){
                    fPos = p2; fUV = float2(0,1);
                }else{
                    fPos = p3; fUV = float2(1,1);
                }
                
                o.uv = fUV;
                o.endPos = endPos;
                o.startPos = startPos;
                o.world = fPos;
                o.edgeFade = pow( edgeFade , _EdgeFadeSharpness);

                // Getting a seperate UV for our dust 
                o.uv2 = fUV  * _DustMapSize * 10 + float2( hash(pID *415) , hash(pID * 4651));// + float2(_Time.y,0);


                // if we hit a 0 point emission, don't show anything!
                if( length( emission) < .0001){
                    fPos = 0;
                }
                
                o.vertex = mul( UNITY_MATRIX_VP , float4(fPos,1));

                o.screenPos = ComputeScreenPos( o.vertex );//+ float4( hash(pID *415) , hash(pID * 4651),0,0);

                return o;
            }

            float3 hsv(float h, float s, float v)
{
  return lerp( float3( 1.0 , 1, 1 ) , clamp( ( abs( frac(
    h + float3( 3.0, 2.0, 1.0 ) / 3.0 ) * 6.0 - 3.0 ) - 1.0 ), 0.0, 1.0 ), s ) * v;
}
    
            fixed4 frag (v2f v) : SV_Target
            {


                 //depth = pow(Linear01Depth(depth), _DepthLevel);
                float d = length( v.world - v.endPos );
                float endFade =saturate((d) * _FadeOutSpeed);


                d = length( v.world - v.startPos );
                float startFade =  saturate((d) * _FadeInSpeed);

                float distanceFade = 1-saturate( (d - _DistanceFadeStart) * _DistanceFadeSpeed);

                float raySoftness = lerp( 2*(.5-abs(v.uv.x - .5)),1, _RaySharpness);

                
                float fade =raySoftness*distanceFade * endFade* startFade *v.edgeFade * _ValueMultiplier;


                //float4 dust = tex2Dproj( _DustMap , v.screenPos );
                float4 dust = tex2D( _DustMap , v.uv2 );


                float4 fCol = lerp( _StartColor , _EndColor,  length(v.startPos -v.world) / ( _CameraFar-_CameraNear));


                // sample the texture
                fixed4 col =0;
                col.xyz = dust * fCol * fade * v.emission * dust;// tex2D(_DepthTexture, i.uv).a;
       

                //col = 1;
                return col;
            }
            ENDCG
        }
    }
}
