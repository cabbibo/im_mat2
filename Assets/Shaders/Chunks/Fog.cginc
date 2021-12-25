

float _FogCutoff;

float FogMultiplier(float3 worldPosition){

      float dif = length(worldPosition - _PlayerPosition );
      float fog = saturate( (_FogCutoff-dif)/_FogCutoff);

      return fog;

}

