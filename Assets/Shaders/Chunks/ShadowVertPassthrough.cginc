
#include "UnityCG.cginc"
#include "AutoLight.cginc"

struct v2f{ 
  float4 pos        : SV_POSITION; 
  float3 nor        : NORMAL; 

  float debug       : TEXCOORD0; 
  
  float3 eye        : TEXCOORD1;
  float3 world      : TEXCOORD2;  
  float2 uv         : TEXCOORD3; 
  float2 uvBase         : TEXCOORD11; 
  float4 screenPos  : TEXCOORD4;

  // For our matrix
  float3 t1         : TEXCOORD5;
  float3 t2         : TEXCOORD6;
  float3 t3         : TEXCOORD7;
  float3 tan        : TEXCOORD10;

            float3 vel : TEXCOORD8;
  
            UNITY_SHADOW_COORDS(9)

};




StructuredBuffer<Vert> _VertBuffer;
StructuredBuffer<int> _TriBuffer;

sampler2D _ColorMap;
sampler2D _TextureMap;
sampler2D _NormalMap;


float2 _NormalSize;
float2 _PaintSize;
float _NormalDepth;

float _ColorBase;
float _ColorSize;

float2 _TextureMapDimensions;

float2 convertUV( float2 uv , float2 dimensions , float id  ){

  // if we haven't assigned, just pass!
  if( length(dimensions) < 2 ){
    return uv;
  }else{

    float xID = floor(((sin( id * 102121 ) +1)/2) * dimensions.x ) / dimensions.x;
    float yID = floor(((sin( id * 540511 ) +1)/2) * dimensions.y ) / dimensions.y;

    float2 fUV = uv *(1/dimensions) + float2(xID, yID);

    return fUV;
  }

  
}

v2f vert ( uint vid : SV_VertexID )
{
    v2f o;

    UNITY_INITIALIZE_OUTPUT(v2f, o);

    Vert v = _VertBuffer[_TriBuffer[vid]];

    o.world = v.pos;
    o.uvBase = v.uv;
    o.uv = convertUV( v.uv , _TextureMapDimensions, v.debug.x );
    o.pos = mul (UNITY_MATRIX_VP, float4(v.pos,1.0f));
    o.nor = v.nor;//normalize(cross(v0.pos - v1.pos , v0.pos - v2.pos ));
    o.tan = v.tan;
    o.debug = v.debug;
    o.eye = v.pos - _WorldSpaceCameraPos;
    o.screenPos = ComputeScreenPos(o.pos);
    o.vel = v.vel;

    float3 bi = cross(v.nor, v.tan);
    
    // output the tangent space matrix
    o.t1 =  float3(v.tan.x, bi.x, v.nor.x);
    o.t2 =  float3(v.tan.y, bi.y, v.nor.y);
    o.t3 =  float3(v.tan.z, bi.z, v.nor.z);

    TRANSFER_SHADOW(o);
    
    return o;
}