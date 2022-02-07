using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crystal : Cycle
{
    
    
    public Transform touchRepresent;

    public override void Create(){
        data.inputEvents.WhileDownDelta.AddListener( WhileDown );
    }


    public void WhileDown( Vector2 d ){

        if( data.inputEvents.hit.transform != null ){
            if( data.inputEvents.hit.transform.gameObject == this.gameObject){
                print(gameObject.name);
                touchRepresent.position = data.inputEvents.hit.point;
                print( data.inputEvents.hit.textureCoord);
            }
        }
    }


}
