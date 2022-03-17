using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using static Unity.Mathematics.math;
using Unity.Mathematics;

public class ArmTarget : Cycle
{
  
  public IK arm;
  public Critter c;


 public float armSize;


float l;
float oL;

public bool onGround = true;

public float targetPosition;
public float footSpeed;
public float footPickUpTime;
    public override void WhileLiving(float v)
    {

        float3 p = transform.position;

        float3 delta = arm.p[0] - p;

        oL = l;
        l = length(delta);

        // need to pick up and find new target position!
        if( l > armSize && oL < armSize ){

        }


        if( onGround ){
            data.land.NewPosition( transform.position );
        }

    }
}
