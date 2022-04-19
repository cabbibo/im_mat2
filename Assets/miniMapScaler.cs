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


    public override void WhileLiving(float v)
    {

        float height = Mathf.Lerp(minHeight, maxHeight,scale);
        float width = Mathf.Lerp(minWidth, maxWidth,scale);
        minimap.mapSize= new Vector3( width,height,width);

        if( data.inputEvents.Down == 1 ){

            print( "happen");
            print( data.inputEvents.downHitObject);

            if( data.inputEvents.downHitObject == handle ){
                print("hapen2");
                Vector3 dif = data.inputEvents.hit.point - handle.transform.position;
                handleRep.transform.position = data.inputEvents.hit.point;

            }

        }
    }


}
