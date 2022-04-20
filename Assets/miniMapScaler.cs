using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class miniMapScaler : Cycle
{
   
    
    public MiniMap minimap;

    public float scale;
    public float minWidth;
    public float maxWidth;

    public float minHeight;
    public float maxHeight;
    

    public GameObject handle;

    public GameObject handleRep;


    public LineRenderer lr;

    public float torusRadius;

    public override void WhileLiving(float v)
    {



        if( data.inputEvents.Down == 1 ){

            print( data.inputEvents.downHitObject);



            if( data.inputEvents.downHitObject == handle || data.inputEvents.downHitObject == handleRep ){
         
                Vector3 dif = data.inputEvents.hit.point - handle.transform.position;

               Vector3 localDif =  transform.InverseTransformDirection( dif );

               localDif.y = 0;
               localDif.x = Mathf.Clamp(localDif.x,0,10000);
               localDif *= 10000;

                float ang = Vector3.Angle(Vector3.forward, localDif );
           
                float val = ang / 180;
                print(ang);
                print(scale);
                scale = 1-val;




                
                handleRep.transform.position = handle.transform.position + transform.TransformDirection(localDif).normalized * torusRadius;//data.inputEvents.hit.point;

                //float angle = 


            }

        }

           if(lr== null ){
            lr = GetComponent<LineRenderer>();
            
        }

        lr.positionCount =  Mathf.FloorToInt( scale * 180 );
        
        for( int i = 0; i < lr.positionCount; i++ ){
            float a = ((float)i/180) * 3.14195f;
            lr.SetPosition( i , new Vector3( Mathf.Sin(a), .1f, -Mathf.Cos(a)) * torusRadius * 2);
        }
        

        float height = Mathf.Lerp(minHeight, maxHeight,scale);
        float width = Mathf.Lerp(minWidth, maxWidth,scale);
        minimap.mapSize = new Vector3( width,height,width);

    }


}
