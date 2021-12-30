using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TipsOnRope :TransferLifeForm{

  public Hair hair;
 
  public override void Bind(){

    Hair s = (Hair)skeleton;
    
    transfer.BindInt("_NumVertsPerHair" , () => s.numVertsPerHair );
    
  }



}
