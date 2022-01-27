using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GpuNextTouchGranularInstrument : Cycle
{

  public AudioClip clip;
  ClosestLife c;    

  public string mixerName;
  
  
  public float volume;
    public float pitch;
    public float pitchRandomness;
    public float location;
    public float locationRandomness;
    public float length;
    public float lengthRandomness;


  public float speed;
  public float speedRandomness;

  public float lastTime;
  public int currentStep;
  public int currentStepID;

  public float randomOffset;

  public float maxDist;

  public Form touchable;


  public override void OnBirthed(){

    if( touchable.structSize !=  16){
      DebugThis("YO NOT THE RIGHT STRUCT SIZE DOGGGIE! ");
    }
    
    if( data.gpuCollisions.ToBind != touchable){
      data.gpuCollisions.BindNewForm( touchable );
    }
    c = data.gpuCollisions.life;
    currentStep = 0;
    currentStepID = 0;
    lastTime = 0;
    randomOffset = 0;
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

    if( ( inAndNewClosest || nowIn ) && Time.time - lastTime  > speed + randomOffset ){
        lastTime = Time.time;
        PlayGrain();
    }

  }

  void PlayGrain(){


       randomOffset = speedRandomness * Random.Range( -.5f, .5f);

      float fLocation = location + locationRandomness * Random.Range( -.5f, .5f);
      float fLength = length + lengthRandomness * Random.Range( -.5f, .5f);
      float fPitch = pitch + pitchRandomness * Random.Range( -.5f, .5f);

      if( fLocation < 0 ){ fLocation = Mathf.Abs( fLocation ); }
      if( fLocation > clip.length ){ fLocation = clip.length-(fLocation - clip.length);}
        data.sound.Play( clip , fPitch , volume , fLocation , fLength , data.sound.master , mixerName  );


  }
}
