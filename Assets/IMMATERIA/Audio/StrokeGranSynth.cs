using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class StrokeGranSynth : Cycle
{

    public AudioPlayer player;
    public AudioClip clip;
    public string groupName;
    public AudioMixer mixer;

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


  public float distToPlay;
  public Vector3 playPosition;
  public Vector3 lastPosition;

  public bool on = true;


  public override void Create(){
    currentStep = 0;
    currentStepID = 0;
    lastTime = 0;
    randomOffset = 0;
    lastPosition = playPosition;
  }


  public override void WhileLiving( float v ){

   // if( on){
      if( (playPosition - lastPosition ).magnitude  > distToPlay){
        PlayGrain();
        lastPosition = playPosition;
      }    
    //}

    

  }

  public void PlayGrain(){

    
      randomOffset = speedRandomness * Random.Range( -.5f, .5f);

      float fLocation = location + locationRandomness * Random.Range( -.5f, .5f);
      float fLength = length + lengthRandomness * Random.Range( -.5f, .5f);
      float fPitch = pitch + pitchRandomness * Random.Range( -.5f, .5f);


      if( fLocation < 0 ){ fLocation = Mathf.Abs( fLocation ); }
      //if( fLocation > clip.length ){ fLocation = fLocation % clip.length;}
 
      fLocation = fLocation % clip.length;
      //print( fLocation );
      player.Play( clip , fPitch , volume , fLocation , fLength , mixer, groupName);

      lastTime = Time.time;
  }


   
}
