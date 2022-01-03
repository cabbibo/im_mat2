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
    

    public float maxVelocity;
    public float targetForceMultiplier;
    public float dampening;
    public float targetHitDistance;


    public Vector3 velocity;

    public Vector3 force;

    public float distToTarget;
    public Vector3 targetDirection;

    Vector3 p1;
    
    public override void Create(){
        SafePrepend(SpineHair);
        SafePrepend(SelfTransforms );
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

        force += targetDirection * .01f * targetForceMultiplier;

        velocity += force;
        velocity *= dampening;
        if( velocity.magnitude > maxVelocity){
            velocity = velocity.normalized * maxVelocity;
        }

        transform.position += velocity;

        transform.LookAt( transform.position + velocity );



        if( distToTarget <= targetHitDistance ){
            currentTarget = null;
        }

    }

}
