using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CopyLightDireciton : Cycle
{
 
 public Transform target;
 public float lengthFromTarget;

 public LightingController lighting;


    public override void WhileLiving(float v)
    {
        transform.position = target.position - lighting.MainLightTransform.forward * lengthFromTarget;
        transform.LookAt( target.position );
    }
}
