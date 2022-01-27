﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DandelionBunch : Cycle
{

  public CircleOnTerrain circle;
  public ParticlesOnCircle baseParticles;
  public HairBasic stalk;
  public TubeTransfer  stalkBody;
  public Dandelion tips;
  public DataForm pluckForm;

  public bool constant;

  public int releasing;

  public override void Create(){
    SafeInsert(circle);
    SafeInsert(baseParticles);
    SafeInsert(stalk);
    SafeInsert(stalkBody);
    SafeInsert(tips);
    SafeInsert(pluckForm);
  }

  public void Set(){
    circle.Set();
    baseParticles.Set();
    //kelp.Set();
  }

  public override void Activate(){
//    print("activado");
    Set();
    stalkBody.showBody = true;
  }


  public override void Deactivate(){
    stalkBody.showBody = false;
  }

  public override void WhileLiving(float v){
    if( constant ){ Set(); }
  }


  public override void Bind(){
    data.BindAllData( stalk.collision ); 
    stalk.collision.BindInt("_Releasing",()=>releasing);
  }


// Release States
// 0 nothing
// 1 just ursula
// 2 ursula and touch
// 3 full release
// 4 set disappeared
// 5 let go final

  public void SetRelease( int i ){
    releasing = i;
  }

  


  public void SetDisappeared(){
    releasing = 4;
  }

  public void ReleaseFinalDandelion(){
    releasing = 5;
  }


}
