using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NudiBrain : Cycle
{
    public Transform startPosition;
    public Transform spineBase;
    public Transform head;

    public Transform sensorTipR;
    public Transform sensorTipL;

    public Transform[] targets;

    public Transform currentTarget;
    public Transform oldTarget;


    public float maxVelocity;
    public float targetForceMultiplier;
    public float dampening;
    public float targetHitDistance;
    public float flapSize;
    public float flapSpeed;
    public float turnSpeed;


    public Vector3 velocity;

    public Vector3 force;

    public float distToTarget;
    public Vector3 targetDirection;

    public string currentState;
    public string oldState;

    public Vector3 antennaOut;
    public Vector3 headOut;

    public Transform tmpTarget;

    public int timeIdle;

    public Food food;

    Vector3 p1;

    Vector3 tv1;
    Vector3 tv2;
    Vector3 tv3;
    
    public override void Create(){
        spineBase.position = startPosition.position;
        
        head.position = spineBase.position + headOut.z * spineBase.forward + headOut.x * spineBase.right + headOut.y * spineBase.up;

        sensorTipR.position = head.position + antennaOut.z * head.forward + antennaOut.x * head.right + antennaOut.y * head.up ;
        sensorTipL.position = head.position + antennaOut.z * head.forward - antennaOut.x * head.right + antennaOut.y * head.up ;



    }


    public override void WhileLiving(float v)
    {

     

        force = Vector3.zero;


        Vector3 headTargetPos = spineBase.position + headOut.z * spineBase.forward + headOut.x * spineBase.right + headOut.y * spineBase.up;
        Quaternion headTargetRotation = spineBase.rotation;
        if( currentState == "moving"){
            
            if( currentTarget == null ){
                currentTarget = targets[ Random.Range(0,targets.Length)];
            }

            Vector3 spineGround = data.land.NewPosition( spineBase.transform.position );
            Vector3 targetGround =  data.land.NewPosition( currentTarget.position );

            p1 = spineGround - targetGround;
            p1 = -p1;
            targetDirection = p1.normalized;
            distToTarget = p1.magnitude;



            force += targetDirection *  targetForceMultiplier;

            velocity += force;
            velocity *= dampening;

            velocity = velocity.normalized * maxVelocity * (1+ flapSize*Mathf.Sin(Time.time *flapSpeed));
 

            float speed = maxVelocity * (1+ flapSize*Mathf.Sin(Time.time *flapSpeed));

            spineBase.position += spineBase.forward * speed;

            Quaternion desiredLook = Quaternion.LookRotation(targetDirection);

            spineBase.rotation = Quaternion.Slerp( spineBase.rotation , desiredLook , turnSpeed);
            spineBase.position = data.land.NewPosition( spineBase.position );



            if( distToTarget <= targetHitDistance ){
                currentTarget = null;
                currentState = "idle";
                timeIdle = 0;
               // currentState = "looking";
            }


        }



        if( currentState == "hunting"){
            
            if( currentTarget == null ){
                currentTarget = targets[ Random.Range(0,targets.Length)];
            }

            Vector3 spineGround = data.land.NewPosition( spineBase.transform.position );
            Vector3 targetGround =  data.land.NewPosition( currentTarget.position );

            p1 = spineGround - targetGround;
            p1 = -p1;
            targetDirection = p1.normalized;
            distToTarget = p1.magnitude;



            force += targetDirection *  targetForceMultiplier;

            velocity += force;
            velocity *= dampening;

            velocity = velocity.normalized * maxVelocity * (1+ flapSize*Mathf.Sin(Time.time *flapSpeed));
 

            float speed = maxVelocity * (1+ flapSize*Mathf.Sin(Time.time *flapSpeed));

            spineBase.position += spineBase.forward * speed;

            Quaternion desiredLook = Quaternion.LookRotation(targetDirection);

            spineBase.rotation = Quaternion.Slerp( spineBase.rotation , desiredLook , turnSpeed);
            spineBase.position = data.land.NewPosition( spineBase.position );



            if( distToTarget <= targetHitDistance ){
                currentTarget = null;
                currentState = "idle";
                timeIdle = 0;
               // currentState = "looking";
            }


        }

        if( currentState == "idle"){

            
            Vector3 targetDirection =  Vector3.forward;

            Quaternion desiredLook = Quaternion.LookRotation(targetDirection);


            headTargetPos += .13f* Vector3.up * Mathf.Sin( 20*Time.time * .1f );
            headTargetPos += .13f* Vector3.left * Mathf.Sin( 20*Time.time * .14f +1212 );
            headTargetPos += .13f* Vector3.forward * Mathf.Sin( 20*Time.time * .144f +121112 );
            

            headTargetRotation *= Quaternion.AngleAxis(50*Mathf.Sin( Time.time * 1.2f), Vector3.up  );
            timeIdle ++;

            if( timeIdle > 1000 ){
                currentState = "moving";
            }

            //head.rotation = Quaternion.Slerp( head.rotation , desiredLook , turnSpeed);

        }


         float shortest = 1000;
    int cID = 0;
    


    // Looking for closest food
    for( int i =0; i < food.foods.Length; i++ ){

      if( !food.canSpawn[i] ){
        tv1 = food.foods[i].position - head.position;
        if( tv1.magnitude < shortest ){
          shortest = tv1.magnitude;
          cID = i;
        }
      }
    }

    if( shortest < 10 ){

        if( currentState != "hunting" ){
            oldState = currentState;
            oldTarget = currentTarget;
        }

        currentState = "hunting";
        currentTarget = food.foods[cID];

    }else{
        if( currentState == "hunting"){
            currentState = oldState;
            currentTarget = oldTarget;
        }

    }




            head.rotation = Quaternion.Slerp( head.rotation ,  headTargetRotation , turnSpeed);


        head.position = Vector3.Lerp( head.position , headTargetPos ,.06f);
      

        sensorTipR.position = Vector3.Lerp( sensorTipR.position , head.position + antennaOut.z * head.forward + antennaOut.x * head.right + antennaOut.y * head.up  , .04f);
        sensorTipL.position = Vector3.Lerp( sensorTipL.position , head.position + antennaOut.z * head.forward - antennaOut.x * head.right + antennaOut.y * head.up  , .04f);

 

     


    }

}
