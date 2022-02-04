using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerNode : Cycle
{
   
   public MiniMap miniMap;
   int colorScheme;
   Vector3 worldLocation;

   public StorySetter setter;

   public int id;


   public Renderer renderer;


    public override void Create(){
        print("hmmm whats up");

        MaterialPropertyBlock mpb = new MaterialPropertyBlock();
        renderer.GetPropertyBlock(mpb);

        mpb.SetFloat("_ColorSchemeID", setter.colorType);
        renderer.enabled = true;

    }


   public override void WhileLiving(float v){
       
   }

    public void Deselect(){

        //selectedRenderer.enabled = false;

    }

   public override void OnTap(){
        //ya print("this object got clicked");
   }



}
