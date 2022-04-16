using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FantasyTree {
[ExecuteAlways]
public class ControlTreeMaterialValues : MonoBehaviour
{




    [Range(0,1)]
    public float barkShown;
    
    [Range(0,1)]
    public float flowersShown;

    [Range(0,1)]
    public float flowersFallen;


    public Material flowersMaterial;
    public Material barkMaterial;

    public MaterialPropertyBlock barkMPB;
    public MaterialPropertyBlock flowersMPB;

    public Renderer renderer;
    void OnEnable(){

        renderer = GetComponent<Renderer>();

        barkMPB = new MaterialPropertyBlock();
        flowersMPB = new MaterialPropertyBlock();

       renderer.GetPropertyBlock( barkMPB , 0 );
       renderer.GetPropertyBlock( flowersMPB , 1 );
    }


    // Update is called once per frame
    void Update()
    {
       // barkShown = (Mathf.Sin(Time.time)+ 1) /2;
        
        //flowersShown = (Mathf.Sin(Time.time + 1.4f)+ 1) /2 ;

        barkMPB.SetFloat("_AmountShown",barkShown);
        flowersMPB.SetFloat("_AmountShown",flowersShown);
        flowersMPB.SetFloat("_FallingAmount",flowersFallen);
      
        renderer.SetPropertyBlock(barkMPB ,0);
        renderer.SetPropertyBlock(flowersMPB ,1);
    
    
    
    }
}}