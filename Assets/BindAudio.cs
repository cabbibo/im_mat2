using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BindAudio : Binder
{
   public AudioListenerTexture audioInfo;
   public override void Bind(){ 

       toBind.BindTexture("_AudioMap", ()=>audioInfo.texture);
       
   }
}
