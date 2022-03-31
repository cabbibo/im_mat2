using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CapsuleBuffer : TransformBuffer
{
    
    
    public List<CapsuleCollider> capsules;

    public Transform startTransform;

    

    public override void SetStructSize()
    {

        // ltw + wtl + 4 : vertical offset, radius, height, debug
        structSize = 36;
    }


    public override void SetCount(){

        if( startTransform == null ){
            startTransform = transform;
        }

        capsules = new List<CapsuleCollider>();

        CapsuleCollider c = startTransform.GetComponent<CapsuleCollider>();
        if( c != null ){
        capsules.Add( c );
        }
        
        Percolate(startTransform);

        print("count: " + capsules.Count );
        count = capsules.Count;

    }


    public override void Embody()
    {


        values = new float[ count * structSize ];
        tmpVals = new float[ 16 ];
        SetInfo();
    }


    public override void SetInfo(){

    for( int i = 0; i < capsules.Count; i++ ){
      tmpVals = HELP.GetMatrixFloats(capsules[i].transform.localToWorldMatrix);
      for( int j = 0; j < 16; j++ ){
        values[i * structSize + j ] = tmpVals[j];
      }

      tmpVals = HELP.GetMatrixFloats(capsules[i].transform.worldToLocalMatrix);
      for( int j = 0; j < 16; j++ ){
        values[i * structSize + j + 16 ] = tmpVals[j];
      }

      values[i * structSize + 32 + 0] = capsules[i].center.y;
      values[i * structSize + 32 + 1] = capsules[i].radius;
      values[i * structSize + 32 + 2] = capsules[i].height;
      values[i * structSize + 32 + 3] = Mathf.Sin(Time.time);
    }

    SetData(values);
  }





    void Percolate(Transform t){

        int count = t.childCount;
        for(int i = 0; i < count; i++)
        {
            Transform child = t.GetChild(i);
            CapsuleCollider c = child.GetComponent<CapsuleCollider>();
            if( c != null ){
                capsules.Add( c );
            }
            
            Percolate(child);
        }

    }
}
