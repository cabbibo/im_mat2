using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DandelionPluckInstrument : Cycle
{



    public DataForm pluckForm;
  
    public int[] steps;
    public AudioClip[] clips;

    public float oNumPlucked;

    public float lastPlayTime;
    public string mixerName;

    public override void OnLive()
    {
        lastPlayTime = Time.time;
    }
    public override void WhileLiving(float v ){
        float numPlucked = pluckForm.values[0];

        float newPlucks = numPlucked - oNumPlucked;

        oNumPlucked = numPlucked;



        if( newPlucks > 0 && Time.time - lastPlayTime > .1f){
        
            AudioClip clip = clips[Random.Range(0,clips.Length)];
            int step = steps[Random.Range(0,steps.Length)];
            data.sound.Play( clip , step  , 1 , 0  , data.sound.master , mixerName );

            lastPlayTime = Time.time;

        }

    }

}
