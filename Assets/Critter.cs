using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Unity.Mathematics.math;
    using Unity.Mathematics;

public class Critter : Cycle
{



    public List<IK> arms;

    public Transform target;

    public List<float3> armBasePositions;

    public float3 vel;


public float3 forward;
    public override void OnBirthed()
    {

        armBasePositions = new List<float3>();

        for( int i = 0; i < arms.Count; i++ ){

            armBasePositions.Add( transform.InverseTransformPoint( arms[i].p[0] ));

        }

    }

    public override void WhileLiving(float v)
    {

        

        float3 d = target.position - transform.position;

        vel = normalize(d) * .01f;

        transform.position = float3(transform.position) + vel;


         for( int i = 0; i < arms.Count; i++ ){

            arms[i].p[0] = transform.TransformPoint( armBasePositions[i] );

        }

        transform.LookAt(target.position);

    }

}
