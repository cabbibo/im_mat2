﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoryNode : Cycle
{
   
   public MiniMap miniMap;
   int colorScheme;
   Vector3 worldLocation;

   public StorySetter setter;

   public int id;


   public Renderer notVisitedRenderer;
   public Renderer visitedRenderer;
   public Renderer completedRenderer;
   public Renderer selectedRenderer;


    public override void Create(){
        print("hmmm whats up");

        MaterialPropertyBlock mpb = new MaterialPropertyBlock();
        visitedRenderer.GetPropertyBlock(mpb);

        mpb.SetFloat("_ColorSchemeID", setter.colorType);
        
        notVisitedRenderer.enabled = false;
        visitedRenderer.enabled = false;
        completedRenderer.enabled = false;
        selectedRenderer.enabled = false;



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
       
   }

    public void Deselect(){

        selectedRenderer.enabled = false;

    }

   public override void OnTap(){

           for( int i = 0; i < miniMap.storyMarkers.Length; i++ ){
            miniMap.storyMarkers[i].Deselect();
        }

        data.state.PlaySelection();
        data.state.ConnectMonolith( id );
        selectedRenderer.enabled = true;

    
        print("this object got clicked");
   }



}