﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainTap : Cycle
{
   
  public float emitTime;
  public float tapTime;
   

   public override void Create(){
    tapTime = Time.time;
   }
   public void Tap(){
    //print("helloos");
    //print(data.inputEvents.hitTag);

    data.inputEvents.DoRaycast();


      if( ( data.inputEvents.hitTag == "Untagged" || data.inputEvents.hitTag == "Frame") && !data.state.inPages ){
       // print("double hello");
   
      transform.position = data.land.Trace( data.inputEvents.ray.origin , data.inputEvents.ray.direction );
      data.playerControls.SetMoveTarget( transform );
      data.guideParticles.SetEmitterPosition( transform.position );

      data.guideParticles.EmitOn();
      tapTime = Time.time;
    }else if( ( data.inputEvents.hitTag == "TerrainObject") && !data.state.inPages ){


      Vector3 p =  data.land.Trace( data.inputEvents.ray.origin , data.inputEvents.ray.direction );

      

      transform.position = p;// data.land.Trace( data.inputEvents.ray.origin , data.inputEvents.ray.direction );
      
      print( data.inputEvents.hitPosition );
      if( data.inputEvents.hitPosition.y > p.y ){
        transform.position = data.inputEvents.hitPosition;
      }
      
      data.playerControls.SetMoveTarget( transform );
      data.guideParticles.SetEmitterPosition( transform.position );

      data.guideParticles.EmitOn();
      tapTime = Time.time;

    }


   }



   public void SetTransform( Transform t ){

      transform.position = t.position;
      data.playerControls.SetMoveTarget( transform );
      data.guideParticles.SetEmitterPosition( transform.position );

      data.guideParticles.EmitOn();
      tapTime = Time.time;
   }

  


  public override void WhileLiving(float f){



    if( Time.time - tapTime > emitTime ){
      data.guideParticles.EmitOff();
    }else{
     
    }

  }



}
