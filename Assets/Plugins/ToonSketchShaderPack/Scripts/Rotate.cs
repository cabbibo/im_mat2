using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace ToonSketchShaderPack{

[ExecuteAlways]
public class Rotate : MonoBehaviour
{
    public float speed;

    public Transform lookAt;
    public float radius;
    public float up;
    public float lookUp;

    //Rotating a light around a point and looking at it
    void Update()
    {

        transform.position = lookAt.position + new Vector3( Mathf.Sin( Time.time * speed )* radius , up , -Mathf.Cos( Time.time * speed ) * radius );
        //transform.Rotate( Vector3.up * speed , Space.World);

        transform.LookAt( lookAt.position + lookUp  * Vector3.up );
    }
}
}