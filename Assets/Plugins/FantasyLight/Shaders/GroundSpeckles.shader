Shader "FantasyLight/GroundSpeckles"
{
    Properties
    {
    _Color ("Color", Color) = (1,1,1,1)
    _ValueMultiplier ("Value Multiplier", float) = .01
    _RaySize ("Ray Size", float) = .01
    _NormalOffset ("Normal Offset", float) = .01
    _RayLocationOsscilationSpeed ("Ray Location Osscilation Speed", float) = .01
    _RayLocationOsscilationSize ("Ray Location Osscilation Size", float) = .01
   
    _CenterWeight("_CenterWeight",float) = 1
    _EdgeFadeSharpness("_EdgeFadeSharpness", float) = 1


    _DustMap("_DustMap", 2D) = "white" {}
    _EmissionMap("_EmissionMap", 2D) = "white" {}

    _LightMatchImportance("_LightMatchImportance", float) = 0
    _DistanceFadeStart("_DistanceFadeStart" , float ) = 1
    _DistanceFadeSpeed("_DistanceFadeSpeed" , float ) = 1
    
    _RaySharpness ("_RaySharpness", float) = .01
    [Toggle(CIRCLE_OR_SQUARE)] _CircleOrSquare("Circle Or Square", Float) = 0
    }
    SubShader
    {
        // inside SubShader
Tags { "Queue"="Transparent" "RenderType"="Transparent" "IgnoreProjector"="True" }

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
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
                float depth : TEXCOORD1;
                float matchVal : TEXCOORD2;
                float radius : TEXCOORD3;
                float edgeFade : TEXCOORD4;
                float4 emission : TEXCOORD5;
                float3 startPos : TEXCOORD6;
                float3 endPos : TEXCOORD7;
                float lightMatchFade : TEXCOORD8;
                float world : TEXCOORD9;
            };

     
      float hash( float n ){
        return frac(sin(n)*43758.5453);
      }

       sampler2D _DepthTexture;

       sampler2D _EmissionMap;
       sampler2D _DustMap;
       float _RaySize;
       float _NormalOffset;
       float _ValueMultiplier;
       float4 _Color;
       float _RayLocationOsscilationSpeed;
       float _RayLocationOsscilationSize;


        float4x4 _CameraMatrix;
        float2 _CameraSize;
        float _CameraNear;
        float _CameraFar;

        
        float _CenterWeight;
        float _EdgeFadeSharpness;

        float _DistanceFadeStart;
        float _DistanceFadeSpeed;
        float _LightMatchImportance;
        float _RaySharpness;


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
                    float edgeFade  =min(1-abs(x) , 1-abs(y) );
                #else
                
                    float r = pow(abs(sin(pID * 441.414)),_CenterWeight);
                    float a = hash(pID * 1212) * 2 * 3.14159;

                    float x = sin(a) * r;
                    float y = -cos(a) * r;

                    // Getting our value to fade out on the edges
                    float edgeFade = 1-r;
                #endif


               
                // Adding some movement if we want it
                x += _RayLocationOsscilationSize * sin(_RayLocationOsscilationSpeed*_Time.y * hash(pID * 1521));
                y += _RayLocationOsscilationSize * sin(_RayLocationOsscilationSpeed*_Time.y * hash(pID * 5131));


                // normalizing to sample correctly
                x +=1;
                x /=2;

                y +=1;
                y /=2;



                
                // Using the camera matrix to get our spread
                float3 forward  = normalize(mul( _CameraMatrix , float4(0,0,1,0)).xyz);
                float3 up       = normalize(mul( _CameraMatrix , float4(0,1,0,0)).xyz);
                float3 left     = normalize(mul( _CameraMatrix , float4(1,0,0,0)).xyz);
                float3 pos      = mul(_CameraMatrix,float4(0,0,0,1)).xyz;
                
                 // Getting the emission color at this location for 'stained glass' type effects
                float4 emission = tex2Dlod(_EmissionMap,float4(x,y,0,0));
                o.emission = emission;

                
                float depth = tex2Dlod(_DepthTexture,float4(x,y,0,0));
                o.depth = depth;
                
             

                float eps = .01;
                   // Getting our ray origin
                float3 startPos = getPos( x, y, left, up ,forward , pos );
                float3 startPosX = getPos( x+eps, y, left, up ,forward , pos  );
                float3 startPosX1 = getPos( x-eps, y, left, up ,forward , pos );
                float3 startPosY =  getPos( x, y+eps, left, up ,forward , pos );
                float3 startPosY1 = getPos( x, y-eps, left, up ,forward , pos );

                //float depth = tex2Dlod(_DepthTexture,float4(x,y,0,0));
                float depthX = tex2Dlod(_DepthTexture,float4(x+eps,y,0,0));
                float depthY = tex2Dlod(_DepthTexture,float4(x,y+eps,0,0));
                float depthX1 = tex2Dlod(_DepthTexture,float4(x-eps,y,0,0));
                float depthY1 = tex2Dlod(_DepthTexture,float4(x,y-eps,0,0));

                // Getting where our ray hits by using the camera
                float3 endPos =  startPos + forward * (1-depth) * ( _CameraFar - _CameraNear);
                float3 endPosX = startPosX + forward * (1-depthX) * ( _CameraFar - _CameraNear);
                float3 endPosX1 = startPosX1 + forward * (1-depthX1) * ( _CameraFar - _CameraNear);
                float3 endPosY = startPosY + forward * (1-depthY) * ( _CameraFar - _CameraNear);
                float3 endPosY1 = startPosY1 + forward * (1-depthY1) * ( _CameraFar - _CameraNear);

                // using the deltas to get the 'normal' of our collision point
                float3 nor = normalize(cross(endPosX - endPosX1 , endPosY-endPosY1));

                endPos -= nor * _NormalOffset;

                float m = abs(dot(forward,nor));

                float3 xDir = normalize(cross(nor,float3(1,0,0)));
                float3 yDir = normalize(cross(nor,xDir));
                 xDir = normalize(cross(nor,yDir));

                 float fSize = _RaySize;// * m;
                 o.lightMatchFade = pow( m, _LightMatchImportance);

                float3 p1 = endPos-nor *.1 *hash(pID*131) * (.8 +.2) - xDir * fSize - yDir* fSize;
                float3 p2 = endPos-nor *.1 *hash(pID*131) * (.8 +.2) + xDir * fSize - yDir* fSize;
                float3 p3 = endPos-nor *.1 *hash(pID*131) * (.8 +.2) - xDir * fSize + yDir* fSize;
                float3 p4 = endPos-nor *.1 *hash(pID*131) * (.8 +.2) + xDir * fSize + yDir* fSize;


                float3 fPos = 0;
                float2 fUV = 0;


                int which = vid % 6;

                if( which == 0 || which == 3 ){
                    fPos = p1; fUV = float2(0,0);
                }else if( which == 2 || which == 4 ){
                    fPos = p4; fUV = float2(1,1);
                }else if( which == 1 ){
                    fPos = p2; fUV = float2(1,0);
                }else{
                    fPos = p3; fUV = float2(0,1);
                }
                
                o.uv = fUV;
            
                o.startPos = startPos;
                o.endPos = endPos;
                o.world = fPos;

                o.edgeFade = pow( edgeFade , _EdgeFadeSharpness);
                
                o.vertex = mul( UNITY_MATRIX_VP , float4(fPos,1));

                return o;
            }
    
            fixed4 frag (v2f v) : SV_Target
            {

             
               //float distanceFade = 1-saturate( (d - _DistanceFadeStart) * _DistanceFadeSpeed);

               //float raySoftness = lerp( 2*(.5-abs(v.uv.x - .5)),1, _RaySharpness);

               //
               //float fade =raySoftness*distanceFade * endFade* startFade *v.edgeFade * _ValueMultiplier;


               //float4 dust = tex2D( _DustMap , v.uv2 );


               //float4 fCol = lerp( _StartColor , _EndColor,  length(v.startPos -v.world) / ( _CameraFar-_CameraNear));


               //// sample the texture
               //fixed4 col = dust * fCol * fade * v.emission * dust;// tex2D(_DepthTexture, i.uv).a;
       

            float raySoftness = lerp( saturate((.5-length(v.uv - .5))),1, _RaySharpness);

            float d = length( v.world - v.startPos );
            float distanceFade = 1-saturate( (d - _DistanceFadeStart) * _DistanceFadeSpeed);
            float fade =  v.lightMatchFade* raySoftness * _ValueMultiplier*distanceFade*v.edgeFade;

            float4 dust = tex2D( _DustMap , v.uv );

            fixed4 col = fade * dust * v.emission * _Color;// * fade;;
                //col = 1;
                //col /= (1+3*v.radius);
                return col;
            }
            ENDCG
        }
    }
}
