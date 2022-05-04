sampler2D _AudioMap;

float4 SampleAudio( float v){
  return tex2D(_AudioMap, float2(v,0));
}


float4 SampleAudioLOD( float v){
  return tex2Dlod(_AudioMap, float4(v,0,0,0));
}