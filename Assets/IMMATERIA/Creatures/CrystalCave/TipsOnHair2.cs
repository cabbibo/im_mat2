using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TipsOnHair2: Form {

  public Form baseForm;
  public override void SetStructSize(){ structSize = 16; }
  public override void SetCount(){ count = (baseForm.count/2) * 7; }

}



