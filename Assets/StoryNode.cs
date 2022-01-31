using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoryNode : Cycle
{
   
   int colorScheme;
   Vector3 worldLocation;

   public StorySetter setter;


   public Renderer notVisitedRenderer;
   public Renderer visitedRenderer;
   public Renderer completedRenderer;


    public override void Create(){
        print("hmmm whats up");

        MaterialPropertyBlock mpb = new MaterialPropertyBlock();
        visitedRenderer.GetPropertyBlock(mpb);

        mpb.SetFloat("_ColorSchemeID", setter.colorType);
        
        notVisitedRenderer.enabled = false;
        visitedRenderer.enabled = false;
        completedRenderer.enabled = false;



        notVisitedRenderer.enabled = true;
        notVisitedRenderer.SetPropertyBlock(mpb);

        for( int i = 0; i < data.state.storiesVisited.Count; i++ ){
            if( data.state.storiesVisited[i] == this.setter){
                visitedRenderer.enabled = true;
                visitedRenderer.SetPropertyBlock(mpb);
            }
        }

        for( int i = 0; i < data.state.storiesCompleted.Count; i++ ){
            if( data.state.storiesCompleted[i] == this.setter){
                completedRenderer.enabled = true;
                completedRenderer.SetPropertyBlock(mpb);
            }
        }
    }


   public override void WhileLiving(float v){
       print("hey");
   }



}
