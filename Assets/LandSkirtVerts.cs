using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LandSkirtVerts : Form
{

    public LandSkirt skirt;
    public int dimensions;

    public override void SetStructSize()
    {
        structSize = 16;
    }


    public override void SetCount()
    {

        dimensions = skirt.tiler.tileDimensions;

        // 8 "tiles" in our skirt
        count = dimensions * dimensions * 8;

    }


    public override void Embody()
    {

        //    print("Creatings");

        float[] values = new float[count * structSize];
        int index = 0;


        for (int i = 0; i < 8; i++)
        {

            for (int j = 0; j < dimensions; j++)
            {
                for (int k = 0; k < dimensions; k++)
                {

                    Vector3 pos = Vector3.zero;
                    Vector3 nor = Vector3.up;

                    Vector2 uv = new Vector2(((float)j / ((float)dimensions - 2)), ((float)k / ((float)dimensions - 2)));

                    values[index++] = pos.x;
                    values[index++] = pos.y;
                    values[index++] = pos.z;

                    values[index++] = 0;
                    values[index++] = 0;
                    values[index++] = 0;

                    values[index++] = nor.x;
                    values[index++] = nor.y;
                    values[index++] = nor.z;

                    values[index++] = 0;
                    values[index++] = 0;
                    values[index++] = 0;

                    values[index++] = uv.x;
                    values[index++] = uv.y;
                    values[index++] = 0;
                    values[index++] = i;

                }

            }

        }

        SetData(values);

    }
}
