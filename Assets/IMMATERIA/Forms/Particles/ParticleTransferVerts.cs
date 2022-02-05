using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleTransferVerts: Form {


  public Form particles;

  public float countMultiplier = 1;



    public override void _Create(){
    
    if( particles == null ){particles = GetComponent<Form>(); }
    SetStructSize();
    SetCount();
    SetBufferType();
    DoCreate();
    Create();
  }
  public override void SetStructSize(){ structSize = 16; }

  public override void SetCount(){
    
    // 0-1
    // |/|
    // 2-3
    count = (int)((float)particles.count * countMultiplier * 4);
  }

  

}



