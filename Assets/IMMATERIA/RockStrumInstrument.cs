using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RockStrumInstrument : Cycle
{

    public DataForm info;

    public AudioClip[] clips;
    public string mixerName;

    public float volume;
 
    float id;
    float oID;

    public int[] steps;

    public override void WhileLiving(float v){

        oID = id;
        id = info.values[0];



        if( id != oID && id >= 0){

            int whichStep = (int)id;
            int whichOctave = whichStep / steps.Length;
            int inScale = whichStep % steps.Length;
            
            AudioClip clip = clips[Random.Range(0,clips.Length)];
            data.sound.Play( clip , whichOctave * 12 + steps[inScale] , volume, 0  , data.sound.master , mixerName );
        }

    }


}
