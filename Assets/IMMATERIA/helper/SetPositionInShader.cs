using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace IMMATERIA {
public class SetPositionInShader : Cycle
{

    public string nameInShader;
    public Transform target;

    private MaterialPropertyBlock mpb;
    private Renderer render;

    // Start is called before the first frame update
    public override void Create()
    {
      render = GetComponent<Renderer>();    
      mpb = new MaterialPropertyBlock();
    }

    // Update is called once per frame
    public override void WhileLiving( float v ){

     // print("setting");
     render.GetPropertyBlock(mpb);
     mpb.SetVector( "_EyePos" , target.position );
     render.SetPropertyBlock(mpb);
    }
}
}