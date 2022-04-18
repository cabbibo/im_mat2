using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BindAudio : Binder
{
   public AudioListenerTexture audioInfo;
   public override void Bind(){ 
       if( audioInfo == null ){
           audioInfo = data.sound.audioTexture;
       }
       toBind.BindTexture("_AudioMap", ()=> this.audioInfo.texture);
       
   }
}
