using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BindGridInfo :Binder
{

    public GridVerts verts;

    public override void Bind()
    {
        toBind.BindInt("_Row",verts.rows);
        toBind.BindInt("_Cols",verts.cols);
    }


}
