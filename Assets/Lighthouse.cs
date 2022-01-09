using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lighthouse : Cycle
{

    public Transform[] possibleLookTargets;

    public Transform eye;
    public float eyeRotateSpeed;
    public float fovChangeSpeed;
    public float fovMultiplier;
    public float minFOV;
    public float maxFOV;

    public VolumetricLightRays light;



    Quaternion targetLook;

    public Transform currentTarget;

    public float angleBetween;

    public float targetFOV;


    public override void OnBirthed()
    {
        TargetHit();
    }
    public override void WhileLiving(float v)
    {
        

        eye.rotation = Quaternion.Slerp( eye.rotation, targetLook, eyeRotateSpeed );
        angleBetween =  Quaternion.Angle( eye.rotation , targetLook );


        targetFOV = Mathf.Clamp( angleBetween * fovMultiplier, minFOV, maxFOV  );
        light.fov = Mathf.Lerp(light.fov, targetFOV ,fovChangeSpeed );

        if( angleBetween < 1 ){
            TargetHit();
        }
        
    }

    public void TargetHit(){

        currentTarget = possibleLookTargets[Random.Range(0,possibleLookTargets.Length)];

        Vector3 forward = eye.position - currentTarget.position;
        targetLook = Quaternion.LookRotation(-forward);


    }


}
