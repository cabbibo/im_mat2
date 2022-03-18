using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using static Unity.Mathematics.math;
using Unity.Mathematics;

public class ArmTarget : Cycle
{
  
  public IK arm;
  public Critter critter;


 public float armSize;


public float l;
public float oL;

public bool onGround = true;

public float3 targetPosition;
public float3 oldStepPosition;
public float stepSpeed;
public float stepPickUpTime;
public float stepSize;
public float stepHeight;


public float3 lockedPosition;

public LineRenderer lr;


public override void OnBirthed(){
    transform.position =  data.land.NewPosition(arm.p[0]);
    lockedPosition = transform.position;
    onGround = true;

    armSize = arm.fullLength;

        float3 p = transform.position;
    
        float3 delta =  arm.p[0] - p;
        l = length(delta);
        oL = l;

}
    public override void WhileLiving(float v)
    {

        float3 p = transform.position;

        float3 delta =  arm.p[0] - p;

        

        oL = l;
        l = length(delta);



        // need to pick up and find new target position!
        if( l > armSize && oL < armSize && onGround == true ){


            stepPickUpTime = Time.time;
            onGround = false;

            oldStepPosition = transform.position;
            targetPosition = data.land.NewPosition( float3(critter.transform.position) + normalize(critter.transform.forward)  * 2 * stepSize );



        }


        if( onGround == false ){


            float lerpVal = (Time.time - stepPickUpTime)/stepSpeed;

            print(lerpVal);

             lockedPosition = lerp( oldStepPosition , targetPosition , lerpVal );


             //lockedPosition = lockedPosition+ float3(0,1,0) * (.5f -abs(lerpVal-.5f));

        
            transform.position = lockedPosition;
            lr.SetPosition(0,oldStepPosition );
            lr.SetPosition(1,targetPosition);
            if( lerpVal >= 1 ){
                

                lockedPosition = targetPosition;
                onGround = true;
            }
        }


        if( onGround ){

            transform.position = lockedPosition;

        }

    }
}
