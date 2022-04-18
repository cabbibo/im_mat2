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
    public Transform lastTarget;

    public float angleBetween;

    public float targetFOV;
    float lastHitTime;
    Quaternion lastHitLook;

    public bool lookingAtObject;
    public Transform objectLookedAt;


    public override void OnBirthed()
    {
        NewRandomTarget();

        data.inputEvents.OnTap.AddListener( OnTap );
    }

    public override void OnDie()
    {
        data.inputEvents.OnTap.RemoveListener( OnTap );
    }

    public override void WhileLiving(float v)
    {


        float t = (Time.time -lastHitTime)/eyeRotateSpeed;
        
        float fT = t * t * (3.0f - 2.0f * t);

        Vector3 forward = eye.position - currentTarget.position;
        targetLook = Quaternion.LookRotation(-forward);

        
 

        angleBetween =  Quaternion.Angle( eye.rotation , targetLook );
        eye.rotation = Quaternion.Slerp( eye.rotation, targetLook, eyeRotateSpeed + eyeRotateSpeed*10/angleBetween);


        forward = eye.position - lastTarget.position;
        targetLook = Quaternion.LookRotation(-forward);
        float oAngleBetween = Quaternion.Angle( eye.rotation , targetLook );

        //targetFOV = Mathf.Clamp( angleBetween * fovMultiplier, minFOV, maxFOV  );


        float minAngle = Mathf.Min(angleBetween ,oAngleBetween);

        targetFOV = Mathf.Lerp(minFOV,maxFOV, Mathf.Clamp(((minAngle*minAngle)/(360*360))*1000,0,1) );
        
        
        light.fov = Mathf.Lerp(light.fov, targetFOV ,fovChangeSpeed );
       
        if( angleBetween < 1 ){
            TargetHit();
        }
        
        

    }

    public void OnTap(){
        print(data.inputEvents.hitTag);
        if( data.inputEvents.hitTag == "Clickable" ){

        print("HASDasd");
            Transform t = data.inputEvents.hit.transform;

            if( t != currentTarget ){
                NewTarget(data.inputEvents.hit.transform);
            }
        }
        
    }


    public void NewRandomTarget(){
        
        Transform t = possibleLookTargets[Random.Range(0,possibleLookTargets.Length)];
        NewTarget(t);
    }

    public void NewTarget(Transform t){
        if( currentTarget == null ){
            lastTarget = t;
        }else{
            lastTarget = currentTarget;
        }
        
        lastHitLook = eye.rotation;
        lastHitTime = Time.time;

        currentTarget = t;//possibleLookTargets[Random.Range(0,possibleLookTargets.Length)];

        Vector3 forward = eye.position - currentTarget.position;
        targetLook = Quaternion.LookRotation(-forward);
        objectLookedAt = null;
        lookingAtObject = false;
    }

    public void TargetHit(){


        lookingAtObject = true;
        objectLookedAt = currentTarget;


    }


}
