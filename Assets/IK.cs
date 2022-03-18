using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Unity.Mathematics.math;
    using Unity.Mathematics;
public class IK : Cycle
{


    public float tolerance;
    public int iterations;

    public List<Transform> points;
    public List<float3> p;
    public List<float> lengths;

    public Transform target;

    public  float fullLength;

    public LineRenderer lr;

    public override void Create()
    {

       // lengths = new List<float>();
        p = new List<float3>();

        fullLength = 0;
        for( int i = 0; i < points.Count-1; i++ ){
            // print(points[i].position);
            //print(points[i+1].position);
            //lengths.Add((points[i].position - points[i+1].position).magnitude);
            fullLength += lengths[i];
            
        }

        for( int i = 0; i < points.Count; i++ ){
            p.Add(points[i].position);
        }


    }


    float3 v1;
    float3 v2;
    float3 v3;


    public override async void WhileLiving(float v)
    {


        float3 basePosition = p[0];
        for( var iter = 0; iter < iterations; iter++ ){


            v1 = float3(target.position-points[0].position);


            if( length(v1) > fullLength ){
                
                for( int i = 0; i < points.Count-1; i++){
                    p[i+1] = p[i] + normalize(v1) *  lengths[i];
                }

                break;

            }else{



                float3 dif =  p[points.Count-1] - float3(target.position);

                // if we are close enough, can break!
                if( length(dif) < tolerance ){
                    break;
                }else{
                    
                
                    p[points.Count-1] = target.position;
                    for( int i = points.Count-1; i>1; i-- ){

                        dif = p[i] - p[i-1];
                        float l = length(dif);
                        float d = lengths[i-1] / l;
                        p[i-1] = (1-d) * p[i] + d*p[i-1];

                    }



                   // p[0] = basePosition;
                    for( int i = 0; i<points.Count-1; i++ ){

                        dif = p[i] - p[i+1];
                        float l = length(dif);
                        float d = lengths[i] / l;
                        p[i+1] = (1-d) * p[i] + d*p[i+1];

                    }

                
           
                }



            }


        }


        lr.positionCount = points.Count;
        for( int i = 0; i<points.Count;i++){

            points[i].position = p[i];
            lr.SetPosition(i, p[i]);

        }



    }
}
