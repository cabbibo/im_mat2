using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BindTransferOffsetInfo : Binder
{

    public ParticleTransferVerts verts;
    public int offset;
    public override void Bind()
    {
        toBind.BindFloat("_CountMultiplier",() => 1/verts.countMultiplier);
        toBind.BindInt("_CountOffset" , () =>offset);
    }
}
