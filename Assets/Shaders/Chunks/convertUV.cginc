float2 _TextureMapDimensions;

float2 convertUV( float2 uv , float id  ){

  // if we haven't assigned, just pass!
  if( length(_TextureMapDimensions) < 2 ){
    return uv;
  }else{

    float xID = floor(((sin( id * 102121 ) +1)/2) * _TextureMapDimensions.x ) / _TextureMapDimensions.x;
    float yID = floor(((sin( id * 540511 ) +1)/2) * _TextureMapDimensions.y ) / _TextureMapDimensions.y;

    float2 fUV = uv *(1/_TextureMapDimensions) + float2(xID, yID);

    return fUV;
  }

  
}