using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Jellyfish : Cycle
{
    public Transform startPosition;

    public Transform[] targets;

    public Transform currentTarget;

    public TransformBuffer SelfTransforms;
    public HairFCR SpineHair;
    public HairFCR SpineSecondaryHairs;
    

    public float maxVelocity;
    public float targetForceMultiplier;
    public float dampening;
    public float targetHitDistance;
    public float flapSize;
    public float flapSpeed;


    public Vector3 velocity;

    public Vector3 force;

    public float distToTarget;
    public Vector3 targetDirection;

    Vector3 p1;
    
    public override void Create(){
        SafePrepend(SpineSecondaryHairs);
        SafePrepend(SpineHair);
        SafePrepend(SelfTransforms );
        transform.position = startPosition.position;
    }


    public override void WhileLiving(float v)
    {

        if( currentTarget == null ){
            currentTarget = targets[ Random.Range(0,targets.Length)];
        }

        force = Vector3.zero;


        p1 = transform.position - currentTarget.position;
        p1 = -p1;
        targetDirection = p1.normalized;
        distToTarget = p1.magnitude;


        force += targetDirection *  targetForceMultiplier;

        velocity += force;
        velocity *= dampening;

        velocity = velocity.normalized * maxVelocity * (1+ flapSize*Mathf.Sin(Time.time *flapSpeed));
        /*if( velocity.magnitude > maxVelocity){
            velocity = velocity.normalized * maxVelocity;
        }*/



        transform.position += velocity;

        transform.LookAt( transform.position + velocity );



        if( distToTarget <= targetHitDistance ){
            currentTarget = null;
        }

    }

}
