
Texture2D<float4> _AudioMap;
SamplerState sampler_AudioMap;
float _MapSize;
float _MapHeight;


float4 sampleAudio(float v){
  return _AudioMap.SampleLevel(sampler_AudioMap,float2( v , .5) , 0);
}