using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HairBunchTris : IndexForm
{



int rows;
int cols;
 public override void SetCount(){
    
      Hair h = (Hair)toIndex;
      rows = h.numVertsPerHair;
      cols = h.numHairs;
      count = (rows-1) * (cols) * 3 * 2;
  }

  public override void Embody(){

    int[] values = new int[count];
    int index = 0;

    for( int i = 0; i < (cols); i++ ){
      for( int j = 0; j < (rows-1); j++ ){

        int id1 = (i%cols) * rows + j;
        int id2 = ((i+1)%cols ) * rows + j;
        int id3 = (i%cols) * rows + j+1;
        int id4 = ((i+1)%cols) * rows + j+1;

        values[index++] = id1;
        values[index++] = id2;
        values[index++] = id4;
        values[index++] = id1;
        values[index++] = id4;
        values[index++] = id3;

      }
    }

    SetData( values );
  }


}
