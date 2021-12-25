using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightingController : Cycle
{

    public Transform MainLightTransform;
    public Light MainLight;





    public float fogFadeSpeed = .1f;
    public float defaultFogCutoff= 100000;



    public float fogCutoff;    

    public override void OnBirthed(){
        fogCutoff = 0;
    }
    public override void WhileLiving(float v)
    {

        // If we are in a story
        if( data.state.inStory ){
            fogCutoff = Mathf.Lerp( fogCutoff , data.state.setter.fogCutoff , fogFadeSpeed);
        }else{
            fogCutoff = Mathf.Lerp( fogCutoff , defaultFogCutoff , fogFadeSpeed);
        }

        data.SetGlobalFogCutoff( fogCutoff );

    }

}
