using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalLooper : Cycle
{

  public AudioClip[] clips;

  public float[] clipVolumes;
  public float fadeOutSpeed;
  public float fadeInSpeed;

  public bool on;

  public override void Create(){
    if( clipVolumes == null || clipVolumes.Length != clips.Length ){
      clipVolumes = new float[clips.Length];
    }
  }

  public void FadeOut(){
    print("FADE OUT");
      //data.sound.FadeValue("GlobalLoopVolume",-100,data.sound.globalLooper.fadeInSpeed);
      on = false;
  }

  public void FadeIn(){
      on = true;
      print("FADEIN");
      //data.sound.FadeValue("GlobalLoopVolume",0,data.sound.globalLooper.fadeInSpeed);
  }


  public float maxVolume;


  public override void WhileLiving( float v ){


    if( on){
      maxVolume = Mathf.Lerp( maxVolume , 1 , .01f );
    }else{
      maxVolume = Mathf.Lerp( maxVolume , 0, .01f );
    }

    
    Color c = data.land.SampleTexture( data.player.position , 0 );


    for( int i = 0; i< clips.Length; i++){

      data.sound.globalLoopSources[i].clip = clips[i];
   

      float v2 = clipVolumes[i];
      if( i == 0 ){ v2 *= c.r; }
      if( i == 1 ){ v2 *= c.g; }
      if( i == 2 ){ v2 *= c.b; }
      if( i == 3 ){ v2 *= c.a; }


      v2 *= maxVolume;
      data.sound.globalLoopSources[i].volume = Mathf.Lerp(data.sound.globalLoopSources[i].volume,v2, .1f);
  
    }
  }

}
