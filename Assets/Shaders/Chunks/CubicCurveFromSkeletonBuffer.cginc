


#include "../Chunks/cubicCurve.cginc"

float3 cubicFromValue( in float val , in int particleID, in int curveLength , out float3 upPos , out float3 doPos){

  float3 p0 = float3( 0. , 0. , 0. );
  float3 v0 = float3( 0. , 0. , 0. );
  float3 p1 = float3( 0. , 0. , 0. );
  float3 v1 = float3( 0. , 0. , 0. );

  float3 p2 = float3( 0. , 0. , 0. );

  float vPP = float(curveLength);

  float base = val * (vPP-1);

  int baseUp   = floor( base );
  int baseDown = ceil( base );
  float amount = base - float(baseUp);


  int bladeBase = (curveLength) * particleID;

  if( baseUp == 0 ){

    p0 = _SkeletonBuffer[ safeID( baseUp       + bladeBase , _SkeletonBuffer_COUNT) ].pos;
    p1 = _SkeletonBuffer[ safeID( baseDown     + bladeBase , _SkeletonBuffer_COUNT) ].pos;
    p2 = _SkeletonBuffer[ safeID( baseDown + 1 + bladeBase , _SkeletonBuffer_COUNT) ].pos;

    v1 = .5 * ( p2 - p0 );

  }else if( baseDown == vPP-1 ){

    p0 = _SkeletonBuffer[ safeID( baseUp     + bladeBase , _SkeletonBuffer_COUNT) ].pos;
    p1 = _SkeletonBuffer[ safeID( baseDown   + bladeBase , _SkeletonBuffer_COUNT) ].pos;
    p2 = _SkeletonBuffer[ safeID( baseUp - 1 + bladeBase , _SkeletonBuffer_COUNT) ].pos;

    v0 = .5 * ( p1 - p2 );

  }else{

    p0 = _SkeletonBuffer[ safeID( baseUp   + bladeBase , _SkeletonBuffer_COUNT) ].pos;
    p1 = _SkeletonBuffer[ safeID( baseDown + bladeBase , _SkeletonBuffer_COUNT) ].pos;


    float3 pMinus = float3(0,0,0);

    pMinus = _SkeletonBuffer[ safeID( baseUp   - 1 + bladeBase, _SkeletonBuffer_COUNT) ].pos;
    p2 =     _SkeletonBuffer[ safeID( baseDown + 1 + bladeBase, _SkeletonBuffer_COUNT) ].pos;

    v1 = .5 * ( p2 - p0 );
    v0 = .5 * ( p1 - pMinus );

  }

  float3 c0 = p0;
  float3 c1 = p0 + v0/3.;
  float3 c2 = p1 - v1/3.;
  float3 c3 = p1;

  float3 pos = cubicCurve( amount , c0 , c1 , c2 , c3 );

  upPos = cubicCurve( amount  + .001 , c0 , c1 , c2 , c3 );
  doPos = cubicCurve( amount  - .001 , c0 , c1 , c2 , c3 );

  return pos;

}



float3 cubicFromValue( in float val , in int curveLength , out float3 upPos , out float3 doPos){

  float3 p0 = float3( 0. , 0. , 0. );
  float3 v0 = float3( 0. , 0. , 0. );
  float3 p1 = float3( 0. , 0. , 0. );
  float3 v1 = float3( 0. , 0. , 0. );

  float3 p2 = float3( 0. , 0. , 0. );

  float vPP = float(curveLength);

  float base = val * (vPP-1);

  int baseUp   = floor( base );
  int baseDown = ceil( base );
  float amount = base - float(baseUp);


  if( baseUp == 0 ){

    p0 = _SkeletonBuffer[ safeID( baseUp        , _SkeletonBuffer_COUNT) ].pos;
    p1 = _SkeletonBuffer[ safeID( baseDown      , _SkeletonBuffer_COUNT) ].pos;
    p2 = _SkeletonBuffer[ safeID( baseDown + 1  , _SkeletonBuffer_COUNT) ].pos;

    v1 = .5 * ( p2 - p0 );

  }else if( baseDown == vPP-1 ){

    p0 = _SkeletonBuffer[ safeID( baseUp      , _SkeletonBuffer_COUNT) ].pos;
    p1 = _SkeletonBuffer[ safeID( baseDown    , _SkeletonBuffer_COUNT) ].pos;
    p2 = _SkeletonBuffer[ safeID( baseUp - 1  , _SkeletonBuffer_COUNT) ].pos;

    v0 = .5 * ( p1 - p2 );

  }else{

    p0 = _SkeletonBuffer[ safeID( baseUp    , _SkeletonBuffer_COUNT) ].pos;
    p1 = _SkeletonBuffer[ safeID( baseDown  , _SkeletonBuffer_COUNT) ].pos;


    float3 pMinus = float3(0,0,0);

    pMinus = _SkeletonBuffer[ safeID( baseUp   - 1 , _SkeletonBuffer_COUNT) ].pos;
    p2 =     _SkeletonBuffer[ safeID( baseDown + 1 , _SkeletonBuffer_COUNT) ].pos;

    v1 = .5 * ( p2 - p0 );
    v0 = .5 * ( p1 - pMinus );

  }

  float3 c0 = p0;
  float3 c1 = p0 + v0/3.;
  float3 c2 = p1 - v1/3.;
  float3 c3 = p1;

  float3 pos = cubicCurve( amount , c0 , c1 , c2 , c3 );

  upPos = cubicCurve( amount  + .001 , c0 , c1 , c2 , c3 );
  doPos = cubicCurve( amount  - .001 , c0 , c1 , c2 , c3 );

  return pos;

}