using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LandSkirtTris : IndexForm
{

    private LandSkirtVerts lsv;


    public override void SetCount()
    {
        lsv = (LandSkirtVerts)toIndex;
        count = (lsv.dimensions - 1) * (lsv.dimensions - 1) * 3 * 2 * 8;
    }

    public override void Embody()
    {

        int index = 0;
        int[] values = new int[count];
        for (int i = 0; i < 8; i++)
        {
            int baseID = i * lsv.dimensions * lsv.dimensions;


            for (int j = 0; j < lsv.dimensions - 1; j++)
            {
                for (int k = 0; k < lsv.dimensions - 1; k++)
                {


                    int id1 = j * lsv.dimensions + k;
                    int id2 = (j + 1) * lsv.dimensions + k;
                    int id3 = j * lsv.dimensions + k + 1;
                    int id4 = (j + 1) * lsv.dimensions + k + 1;

                    values[index++] = baseID + id1;
                    values[index++] = baseID + id2;
                    values[index++] = baseID + id4;
                    values[index++] = baseID + id1;
                    values[index++] = baseID + id4;
                    values[index++] = baseID + id3;

                }

            }

        }

        SetData(values);
    }

}
