﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class gpuNextTouchInstrument : Cycle
{

  public AudioClip[] clips;
  ClosestLife c;
  public float lastPlayTime;

  public float minPlayTime;
  public float maxDist;

  public float randomness;

  public float closestVolumeDist;
  public float furthestVolumeDist;
  public float volumeMultiplier;

  public int[] steps;
  public Form touchable;


  public override void OnBirthed(){

    if( touchable.structSize !=  16){
      DebugThis("YO NOT THE RIGHT STRUCT SIZE DOGGGIE! ");
    }
    
    if( data.gpuCollisions.ToBind != touchable){
      data.gpuCollisions.BindNewForm( touchable );
    }
    c = data.gpuCollisions.life;
    lastPlayTime = 0;
  }

  public override void OnDied(){
    
    print("dyin");

    // only turn off if we haven't set it to something else
    if( data.gpuCollisions.ToBind == touchable){
      data.gpuCollisions.Unbind();
    }

  }

  public override void WhileLiving( float tmp ){
    bool inAndNewClosest = ((c.closestID != c.oClosestID) && (c.closest.magnitude < maxDist));
    bool nowIn = ((c.closest.magnitude < maxDist) && (c.oClosest.magnitude >= maxDist));
    bool overPlayTime = ((Time.time - lastPlayTime) > minPlayTime+ randomness * Random.Range(-.99f,.99f));

    if( ( inAndNewClosest || nowIn ) && overPlayTime ){


      float v = (c.closestDist - closestVolumeDist) / furthestVolumeDist;// , c.closestDist);

      //print( v );
      v = Mathf.Clamp(v,0,1);     
      //v /= (furthestVolumeDist - closestVolumeDist);
      v = 1-v;

      v = Mathf.SmoothStep( 0,1,v);
     // print(v);

      v = v*v;
      v = volumeMultiplier * v;

      v *= data.inputEvents.downTween2;
    
      AudioClip clip = clips[Random.Range(0,clips.Length)];
      int step = steps[Random.Range(0,steps.Length)];
      data.sound.Play( clip , step  , v , 0  , data.sound.master , "TouchSounds" );
      lastPlayTime = Time.time;
    }

  }
}
