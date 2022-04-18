using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfiniteTiler : Simulation
{   
    public Vector2 size;
    public float radius;


    public override void OnBirthed(){
        life.BindFloat("_Radius",()=>radius);
        data.BindPlayerData(life);
    }
  
}
