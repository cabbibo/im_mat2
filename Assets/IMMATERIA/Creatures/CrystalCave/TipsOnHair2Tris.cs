using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class TipsOnHair2Tris : IndexForm {

  public override void SetCount(){
    count = (toIndex.count / 7) * (6 + 3);
  }

  public override void Embody(){

    int[] values = new int[count];
    int index = 0;

    /*
  
       2-3
       |/| 
       0-1
       
       4-5  
       \ /
        6

    */
    for( int i = 0; i < toIndex.count/7; i++ ){
        
        values[ index ++ ] = i * 7 + 0;
        values[ index ++ ] = i * 7 + 1;
        values[ index ++ ] = i * 7 + 3;
        values[ index ++ ] = i * 7 + 3;
        values[ index ++ ] = i * 7 + 2;
        values[ index ++ ] = i * 7 + 0;

        values[ index ++ ] = i * 7 + 4;
        values[ index ++ ] = i * 7 + 5;
        values[ index ++ ] = i * 7 + 6;


    }

    SetData(values);
  
  }

}

