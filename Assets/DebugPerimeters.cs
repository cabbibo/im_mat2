using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[ExecuteAlways]
public class DebugPerimeters : MonoBehaviour
{

    public Journey journey;

    public bool debug;
    // Update is called once per frame
    void OnDrawGizmos()
    {


if( debug ){
        for( int i = 0; i < journey.monoSetters.Length; i++ ){
            StorySetter s = journey.monoSetters[i];
            float inside = s.perimeter.innerRadius;
            float outside = s.perimeter.outerRadius;


            Vector3 p1;
            Vector3 p2;
            for( int j = 0; j < 20; j ++){

                float a1 = (float)j/20;
                float a2 = (float)((j+1)%20)/20;

                a1 *= 2 * Mathf.PI;
                a2 *= 2 * Mathf.PI;

                Vector3 o1 = new Vector3( Mathf.Sin(a1),  0 , -Mathf.Cos(a1));
                o1 *= outside;

                Vector3 o2 = new  Vector3( Mathf.Sin(a2),  0 , -Mathf.Cos(a2));
                o2 *= outside;

                p1 = s.transform.position + o1;
                p2 = s.transform.position + o2;

                Gizmos.DrawLine(p1,p2);


                
                o1 = new Vector3( Mathf.Sin(a1),  0 , -Mathf.Cos(a1));
                o1 *= inside;

                o2 = new  Vector3( Mathf.Sin(a2),  0 , -Mathf.Cos(a2));
                o2 *= inside;

                p1 = s.transform.position + o1;
                p2 = s.transform.position + o2;

                Gizmos.DrawLine(p1,p2);

                
            }

        }
}
    }
}
