using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace IMMATERIA {
public class Food : Cycle
{

    public GameObject foodPrefab;
    public int numFoods;
    public float radius;
    public float verticalStart;

    public Transform[] foods;
    public Renderer[] renderers;
    public Vector3[] vels;
    public Collider[] colliders;
    public float[] spawnTimes;
    public bool[] canSpawn;

    public TransformBuffer buffer;

    public float lastSpawnTime;
    public float timeBetweenSpawns;
    public float foodLifeTime;

    public Vector3 force;

    public override void Create(){

      SafeInsert( buffer );
      lastSpawnTime = Time.time;


    for( int i = 0; i < foods.Length; i++ ){
      
      if( foods[i] != null ){ 
        DestroyImmediate( foods[i].gameObject );
      }
    }

      foods = new Transform[ numFoods ];
      spawnTimes = new float[ numFoods ];
      canSpawn = new bool[ numFoods ];
      renderers = new Renderer[ numFoods ];
      colliders = new Collider[ numFoods ];
      vels = new Vector3[ numFoods ];

      buffer.count = numFoods;
      buffer.transforms = new Transform[numFoods];

    for( int i = 0; i < numFoods; i++ ){
      GameObject go = Instantiate( foodPrefab ) as GameObject;
      go.transform.parent = this.transform;
      foods[i] = go.transform;
      canSpawn[i] = true;
      spawnTimes[i] = 10000;
      renderers[i] = go.GetComponent<Renderer>();
      colliders[i] = go.GetComponent<Collider>();
      buffer.transforms[i] = go.transform;

    }



    }

    public override void WhileLiving(float v){

      for( int i = 0; i < numFoods; i++ ){
        
        if( canSpawn[i] ){ 
          renderers[i].enabled = false; 
          colliders[i].enabled = false; 
        }else{
          renderers[i].enabled = true; 
          colliders[i].enabled = true; 
        }

        force = Vector3.zero;

        force += Vector3.up * .06f;
        vels[i] += force * .002f;
        foods[i].position += vels[i];

        vels[i] *= .9f;
        foods[i].Rotate(0,1.1f,0);

        float val = ((foodLifeTime-(Time.time - spawnTimes[i]) )/foodLifeTime);

        foods[i].localScale = Vector3.one * Mathf.Min((1-val)*3, val ) * .8f;

        if( Time.time - spawnTimes[i] > foodLifeTime ){
          EatFood(i);
        }

      }

      if( Time.time - lastSpawnTime > timeBetweenSpawns * (1.1f + Mathf.Sin(Time.time * .3f)) ){
        Spawn();
      }

    }

    public void Spawn(){
      for( int i = 0; i < numFoods; i++ ){
        if( canSpawn[i] ){
          canSpawn[i] = false;
          spawnTimes[i] = Time.time;
          foods[i].position = transform.position;
          foods[i].position += transform.right *radius* Random.Range(-0.5f , 0.5f);
          foods[i].position += transform.forward *radius* Random.Range(-0.5f , 0.5f);
          foods[i].position += transform.up *verticalStart;
          foods[i].rotation = Random.rotation;
          vels[i] = Vector3.zero;
          lastSpawnTime = Time.time;
          break;
        }
      }
    }

    public void EatFood( int id ){
      canSpawn[id] = true;
    }



}
}
