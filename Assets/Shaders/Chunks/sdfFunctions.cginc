
/*

  Primatives

*/

float sdBox( float3 p, float3 b ){

  float3 d = abs(p) - b;

  return min(max(d.x,max(d.y,d.z)),0.0) +
         length(max(d,0.0));

}

float sdSphere( float3 p, float s ){
  return length(p)-s;
}

float sdPlane( float3 p, float4 n )
{
  // n must be normalized
  return dot(p,n.xyz) + n.w;
}

float sdPlane( float3 p, float3 n, float h )
{
  // n must be normalized
  return dot(p,n) + h;
}
// checks to see which intersection is closer
// and makes the y of the vec2 be the proper id
float2 opU( float2 d1, float2 d2 ){
    
	return (d1.x<d2.x) ? d1 : d2;
    
}
float opS( float d1, float d2 ) {
    return max(-d1,d2);// ? d1 : d2; 
}


float sdCapsule( float3 p, float3 a, float3 b, float r )
{
    float3 pa = p - a, ba = b - a;
    float h = clamp( dot(pa,ba)/dot(ba,ba), 0.0, 1.0 );
    return length( pa - ba*h ) - r;
}

float sdCone( float3 p, float2 c )
{
    // c must be normalized
    float q = length(p.xy);
    return dot(c,float2(q,p.z));
}

float sdCappedCylinder( float3 p, float2 h )
{
  float2 d = abs(float2(length(p.xz),p.y)) - h;
  return min(max(d.x,d.y),0.0) + length(max(d,0.0));
}



float sdCylinderZ( float3 p, float3 c )
{
  return length(p.xz-c.xy)-c.z;
}




/*

  Operations

*/


float2 smoothU( float2 d1, float2 d2, float k)
{
    float a = d1.x;
    float b = d2.x;
    float h = clamp(0.5+0.5*(b-a)/k, 0.0, 1.0);
    return float2( lerp(b, a, h) - k*h*(1.0-h), lerp(d2.y, d1.y, pow(h, 2.0)));
}

float smax(float a, float b, float k)
{
    return log(exp(k*a)+exp(k*b))/k;
}

float smin(float a, float b, float k)
{
    return -(log(exp(k*-a)+exp(k*-b))/k);
}

float2 smoothS( float2 d1, float2 d2, float k)
{
    return  float2( smax( -d1.x , d2.x , k ) , d2.y );
}




float2 hardU( float2 d1, float2 d2 ){
    
  return (d1.x<d2.x) ? d1 : d2;
    
}

float hardU( float d1, float d2 ){
    
  return max(d1,d2);
    
}

float2 hardS( float2 d1, float2 d2 )
{
    return  -d1.x > d2.x ? d2 : d1;
}

float hardS( float d1, float d2 )
{
    return max(-d1,d2);
}



float3 rotatedBox( float3 p, float4x4 m )
{
    float3 q = mul( m , float4( p , 1 )).xyz;
    return sdBox(q,float3(.6,.6,.6));
}

float3 modit(float3 x, float3 m) {
          float3 r = x%m;
          return r<0 ? r+m : r;
      }


/*

  Combinations

*/

float opRepSphere( float3 p, float3 c , float r)
{
    float3 q = modit(p,c)-0.5*c;
    float3 re = (q-p)/c;
    return sdSphere( q  , r * 1.9 - .1 * length(re) );
}


/*float subCube( float3 pos , float size ){

  float r = opRepSphere( pos , float3( .05 * size * 2. )  , .025 * size * 2.8);
  r = hardS( r ,sdBox( pos , float3( .125 * size * 2. )) );

  return r;

}*/







// ROTATION FUNCTIONS TAKEN FROM
//https://www.shadertoy.com/view/XsSSzG
float3x3 xrotate(float t) {
  return float3x3(1.0, 0.0, 0.0,
                0.0, cos(t), -sin(t),
                0.0, sin(t), cos(t));
}

float3x3 yrotate(float t) {
  return float3x3(cos(t), 0.0, -sin(t),
                0.0, 1.0, 0.0,
                sin(t), 0.0, cos(t));
}

float3x3 zrotate(float t) {
    return float3x3(cos(t), -sin(t), 0.0,
                sin(t), cos(t), 0.0,
                0.0, 0.0, 1.0);
}


float3x3 fullRotate( float3 r ){
 
   return xrotate( r.x ) * yrotate( r.y ) * zrotate( r.z );
    
}

float sdTorus( float3 p, float2 t )
{
  float2 q = float2(length(p.xz)-t.x,p.y);
  return length(q)-t.y;
}





float opSmoothUnion( float d1, float d2, float k ) {
    float h = clamp( 0.5 + 0.5*(d2-d1)/k, 0.0, 1.0 );
    return lerp( d2, d1, h ) - k*h*(1.0-h); }

float opSmoothSubtraction( float d1, float d2, float k ) {
    float h = clamp( 0.5 - 0.5*(d2+d1)/k, 0.0, 1.0 );
    return lerp( d2, -d1, h ) + k*h*(1.0-h); }

float opSmoothIntersection( float d1, float d2, float k ) {
    float h = clamp( 0.5 - 0.5*(d2-d1)/k, 0.0, 1.0 );
    return lerp( d2, d1, h ) + k*h*(1.0-h); }


float sdEllipsoid( float3 p, float3 r )
{
  float k0 = length(p/r);
  float k1 = length(p/(r*r));
  return k0*(k0-1.0)/k1;
}

float dot2(in float3 v ) {return dot(v,v);}
float dot2(in float2 v ) {return dot(v,v);}
float sdCappedCone( float3 p, float h, float r1, float r2 )
{
  float2 q = float2( length(p.xz), p.y );
  float2 k1 = float2(r2,h);
  float2 k2 = float2(r2-r1,2.0*h);
  float2 ca = float2(q.x-min(q.x,(q.y<0.0)?r1:r2), abs(q.y)-h);
  float2 cb = q - k1 + k2*clamp( dot(k1-q,k2)/dot2(k2), 0.0, 1.0 );
  float s = (cb.x<0.0 && ca.y<0.0) ? -1.0 : 1.0;
  return s*sqrt( min(dot2(ca),dot2(cb)) );
}



float sdVerticalCapsule( float3 p, float h, float r )
{
  p.y -= clamp( p.y, 0.0, h );
  return length( p ) - r;
}

float3 opTwist(  in float3 p )
{
    const float k = 10.0; // or some other amount
    float c = cos(k*p.y);
    float s = sin(k*p.y);
    float2x2  m = float2x2(c,-s,s,c);
    float3  q = float3(mul(m,p.xz),p.y);
    return q;
}



float mod(float x, float y)
{
  return x - y * floor(x/y);
}