﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace FantasyTree{

[ExecuteAlways]
public class PlaceTreesOnTerrain : MonoBehaviour
{

    public bool replace;

    public GameObject prefab;

    public int numberTrees;

    public GameObject[] trees;

    public float radius;

    public Data data;
    

    // Start is called before the first frame update
    void OnEnable()
    {


        if( !replace ){
            for( int i = 0; i < trees.Length; i++ ) {
                DestroyImmediate(trees[i]);
            }  

            trees = new GameObject[numberTrees];
        }

        

        for(int i = 0; i< numberTrees;i++ ){

            if(!replace){
                trees[i] = GameObject.Instantiate(prefab);
         

                Vector2 newPosXY = Random.insideUnitCircle * radius;

                Vector3 newPos = new Vector3( newPosXY.x , 0 , newPosXY.y ) + data.player.position;

                newPos = data.land.NewPosition(newPos);
                trees[i].transform.parent = transform;

                trees[i].transform.position = newPos;
                trees[i].transform.rotation = Quaternion.identity;
            
        
                trees[i].GetComponent<Tree>().BuildBranches();
                trees[i].SetActive(true);
            }else{
                
                 Vector2 newPosXY = Random.insideUnitCircle * radius;

                Vector3 newPos = new Vector3( newPosXY.x , 0 , newPosXY.y ) + transform.position;
                newPos = data.land.NewPosition(newPos);
                trees[i].transform.position = newPos;
                trees[i].transform.rotation = Quaternion.identity;
                trees[i].SetActive(true);

            }

        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
}