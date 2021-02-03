using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class Land2 : Cycle
{

    public int whichGrid;
    public int gridResolution;

    public LandTiler tiler;

    public MaterialPropertyBlock mpb;

    public Material terrainMaterial;

    public override void Create()
    {
        mpb = new MaterialPropertyBlock();
    }


    public override void WhileLiving(float v)
    {
        mpb.SetFloat("_Size", tiler.tileSize);
        mpb.SetFloat("_RingSize", tiler.tileSize * Mathf.Pow(3, (whichGrid + 1)));
        mpb.SetInt("_TileDimensions", tiler.tileDimensions);
        mpb.SetInt("_WhichGrid", whichGrid);
        mpb.SetVector("_Center", new Vector4(tiler.currentCenterX, tiler.currentCenterY, 0, 0));

        Graphics.DrawProcedural(terrainMaterial, new Bounds(transform.position, Vector3.one * 50000), MeshTopology.Triangles, 6 * 8 * (tiler.tileDimensions * tiler.tileDimensions), 1, null, mpb, ShadowCastingMode.TwoSided, true, gameObject.layer);

    }

}
