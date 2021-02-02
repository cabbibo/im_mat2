using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BindPosition : Binder
{

    public Transform t;
    public string nameInShader;

    public override void Bind()
    {
        toBind.BindVector3(nameInShader, () => t.position);
    }




}
