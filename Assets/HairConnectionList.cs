using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HairConnectionList : IndexForm
{


    public Hair hair;
    


    int fullID( int i , int j ){
        return i * hair.numVertsPerHair + j;
    }

    public override void Embody(){


        int totalConnections = (hair.numVertsPerHair -1) * hair.numHairs;

        for( int i = 0; i < hair.numHairs; i++ ){
            for( int j = 0; j < hair.numVertsPerHair; j++){

                
                
                if( j > 0 ){

                }


            }
        }
            

    }


}
