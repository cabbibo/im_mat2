using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticlesOnTerrain : Simulation
{
      public Vector2 size;
    public float radius;


    public override void OnBirthed(){
        print("hmm");
        life.BindFloat("_Radius",()=>radius);
        life.BindVector3("_TargetPosition",()=>transform.position);
        data.BindPlayerData(life);
        life.YOLO();
    }
}
