using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RockFractalTree : TransformBuffer
{




    public int layers;
    public int maxCount;

    public override void SetStructSize()
    {
        structSize = 32;
    }


    public override void SetCount(){

        RebuildFractal();
     
    }

    public struct Point{
        public Vector3 pos;
        public Quaternion rot;
        public Vector3 scale;
    }
    public List<Point> fractalPoints;

    public override void Embody()
    {


        print(count);
        float[] values = new float[count * structSize];
      
       // Transform t= new Transform();

        
        for( int index = 0; index < fractalPoints.Count; index++ ){

            Point p = fractalPoints[index];
            
            Matrix4x4 m = Matrix4x4.TRS(p.pos,p.rot,p.scale);

            Matrix4x4 iM = m.inverse;



            for( int i = 0; i < 16; i++ ){
                values[ index * structSize + i  ] = m[i];
            }

            for( int i = 0; i < 16; i++ ){
                values[ index * structSize + i + 16 ] = iM[i];
            }

        }

        SetData(values);
        //MakeTree(Vector3.zero, Quaternion.identity,layers)
    }





    public float reductionFactor;
    public float startWidth;
    public float minWidth;
    public float meshScaleMultiplier;
    public Vector3[] directions;
    public void RebuildFractal(){

        //directions = new Vector3[ 6 ];
     // directions[0] = Vector3.left; 
     // directions[1] = Vector3.right; 
     // directions[2] = Vector3.up; 
     // directions[3] = Vector3.down;
     // directions[4] = Vector3.forward;
     // directions[5] = Vector3.back;


        count = 0;
        fractalPoints = new List<Point>();

        Point p = new Point();
        p.pos = transform.position;
        p.rot = Quaternion.identity;
        p.scale =  startWidth * Vector3.one * meshScaleMultiplier;
        fractalPoints.Add(p);

        place( transform.position, startWidth );



        count = fractalPoints.Count;

    }



    void  place(Vector3 pos, float boxWidth){

        if( fractalPoints.Count < maxCount ){
            var newWidth=boxWidth/reductionFactor;

            newWidth *= Random.Range(.5f,1.5f);
            
            

           /* if (oldDirection>=3){
                oppOld=oldDirection-3;	
            }else{
                oppOld=oldDirection+3;	
            }*/
            
            for(var i=0;i<directions.Length;i++){
                
             

                    if( fractalPoints.Count < maxCount ){
                        var thisBox= new Vector3();
                        thisBox.x=(directions[i].x*(boxWidth+newWidth)*.5f)+pos.x;
                        thisBox.y=(directions[i].y*(boxWidth+newWidth)*.5f)+pos.y;
                        thisBox.z=(directions[i].z*(boxWidth+newWidth)*.5f)+pos.z;

                        Point p = new Point();
                        p.pos = thisBox;
                        //p.rot = Quaternion.identity;
                        p.rot = Quaternion.AngleAxis(360*((float)i/(float)directions.Length),directions[i]);
                        p.scale =  newWidth * Vector3.one  * meshScaleMultiplier;
                        fractalPoints.Add(p);
                    
                        if(newWidth>=minWidth){
                            place(thisBox,newWidth);
                        }	
                    }
                
            }
        }
    }

   // public void MakeTree

}
