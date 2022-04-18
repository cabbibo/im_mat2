using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BindTransferOffsetInfo : Binder
{

    public ParticleTransferVerts verts;
    public int offset;
    public float amount = 1;
    public override void Bind()
    {
        toBind.BindFloat("_CountMultiplier",() => amount/verts.countMultiplier);
        toBind.BindInt("_CountOffset" , () =>offset);
    }
}
