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


    // Update is called once per frame
    void Update()
    {

        barkMaterial.SetFloat("_AmountShown",barkShown);
      
        flowersMaterial.SetFloat("_AmountShown",flowersShown);
        flowersMaterial.SetFloat("_FallingAmount",flowersFallen);
    
    
    
    
    }
}}