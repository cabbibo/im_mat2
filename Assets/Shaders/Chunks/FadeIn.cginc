float _FadeValue;

#include "noise.cginc"

void FadeDiscard(float3 pos){


    if( noise(pos * .01) > _FadeValue){
        discard;
    }

}