using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircleTris: IndexForm
{

  protected CircleVerts cv;

  public override void SetCount(){
    cv = (CircleVerts)toIndex;
    count = (cv.rows-1) * (cv.cols) * 3 * 2;
  }

  public override void Embody(){

    int[] values = new int[count];
    int index = 0;

    for( int i = 0; i < (cv.cols); i++ ){
      for( int j = 0; j < (cv.rows-1); j++ ){

        int id1 = (i%cv.cols) * cv.rows + j;
        int id2 = ((i+1)%cv.cols ) * cv.rows + j;
        int id3 = (i%cv.cols) * cv.rows + j+1;
        int id4 = ((i+1)%cv.cols) * cv.rows + j+1;

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
