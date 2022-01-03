using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BindCircleInfo :Binder
{

    public CircleVerts verts;

    public override void Bind()
    {
        toBind.BindInt("_Row",verts.rows);
        toBind.BindInt("_Cols",verts.cols);
    }


}
