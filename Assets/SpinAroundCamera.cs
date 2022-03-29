using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[ExecuteInEditMode]
public class SpinAroundCamera : MonoBehaviour
{

    public float radius;
    public float height;
    public float speed;

    public float lookVerticalOffset;

    public Transform target;

    public Transform cameraHolder;

    // Update is called once per frame
    void Update()
    {

        float angle = Time.time  * speed;

        Vector3 outVec = Mathf.Sin(angle) * Vector3.left  + Mathf.Cos(angle) * Vector3.forward;
        
        cameraHolder.transform.position = target.position  + outVec * radius +  Vector3.up * height;
        cameraHolder.transform.LookAt( target.position +  Vector3.up * lookVerticalOffset , Vector3.up );
    }

  


}
