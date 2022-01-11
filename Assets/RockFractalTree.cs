using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RockFractalTree : Form
{



    public int layers;

    public override void SetStructSize()
    {
        structSize = 16;
    }


    public override void SetCount(){

    
        RebuildFractal();
      

    }

    public struct Point{
        public Vector3 pos;
        public Quaternion rot;
        public Vector3 scale;
    }
    public List<Point> fractalPoints;

    public override void Embody()
    {

        float[] values = new float[count * structSize];
        int index = 0;
        foreach( Point p in fractalPoints){


            values[ index * structSize + 0  ] = p.pos.x;
            values[ index * structSize + 1  ] = p.pos.y;
            values[ index * structSize + 2  ] = p.pos.z;

            index ++;
        }

        //MakeTree(Vector3.zero, Quaternion.identity,layers)
    }





    public float reductionFactor;
    public float minWidth;
    public Vector3[] directions;
    public void RebuildFractal(){

        directions = new Vector3[ 6 ];
        directions[0] = Vector3.left; 
        directions[1] = Vector3.right; 
        directions[2] = Vector3.up; 
        directions[3] = Vector3.down;
        directions[4] = Vector3.forward;
        directions[5] = Vector3.back;


        count = 0;
        fractalPoints = new List<Point>();




        count = fractalPoints.Count;

    }



    void  place(Vector3 pos, float boxWidth, int oldDirection){
	var newWidth=boxWidth/reductionFactor;
	
    int oppOld;

	if (oldDirection>=3){
		oppOld=oldDirection-3;	
	}else{
		oppOld=oldDirection+3;	
	}
	
	for(var i=0;i<directions.Length;i++){
		
		if(i!=oppOld){
			var thisBox= new Vector3();
			thisBox.x=(directions[i].x*(boxWidth+newWidth)*.5f)+pos.x;
			thisBox.y=(directions[i].y*(boxWidth+newWidth)*.5f)+pos.y;
			thisBox.z=(directions[i].z*(boxWidth+newWidth)*.5f)+pos.z;

            Point p = new Point();
            p.pos = thisBox;
            p.rot = Quaternion.identity;
            p.scale =  newWidth * Vector3.one;
            fractalPoints.Add(p);
		
			if(newWidth>=minWidth){
				place(thisBox,newWidth,i);
			}	
		}
	}
}

   // public void MakeTree

}
