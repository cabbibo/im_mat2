using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BindOtherTransform : Binder
{
    
    public Transform t;
    public string nameInShader;
    public string inverseName;

    public override void Bind()
    {
        if( nameInShader.Length> 0){
            toBind.BindMatrix(nameInShader, () => t.localToWorldMatrix);
        }

        if( inverseName.Length> 0){
            toBind.BindMatrix(inverseName, () => t.worldToLocalMatrix);
        }
    }


}
