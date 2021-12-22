using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IMMATERIA {

public class Hydra : Cycle
{

  public Food food;


  public Transform eye;
  public Transform brain;
  public Transform anchor;

  public Transform person;

  public int numTips;
  public GameObject tipPrefab;

  public GameObject[] tipsGO;
  public TipInfo[] tipInfos;


  [Header("Params")]
  
  [Range(.7f,.9999f)]
  public float armDampening;

  [Range(.01f,.001f)]
  public float armToRingForce;


  [Range(.01f,10f)]
  public float armRingSize;

  [Range(.01f,10f)]
   public float armRingForwardSize;

  [Range(0f,1f)]
  public float armLookSlerpSpeed;


  [Range(0f,.01f)]
  public float armToFoodForce;

  [Range(0f,11f)]
  public float armToFoodDist;

    [Range(0f,.01f)]
  public float brainToFoodForce;




  [Range(0f,11f)]
  public float brainToFoodDist;

      [Range(0f,.01f)]
  public float brainToPersonForce;

    [Range(0f,11f)]
  public float brainToPersonDist;


  [Range(0f,.01f)]
  public float armToPersonForce;

  [Range(0f,11f)]
  public float armToPersonDist;



  [Range(0f,1f)]
  public float brainLookAtTipForceCutoff;


  [Range(0f,.1f)]
  public float headLookSlerpSpeed;


  [Range(0f,10)]
  public float verticalBrainHolderPos;

    [Range(0f,.1f)]
  public float verticalBrainHolderForce;

  
    [Range(0f,20f)]
  public float brainMinOut;




  public bool lookingAtPerson;



   // public GameObject 

   public TransformBuffer stalkBuffer;
   public TransformBuffer tipBuffer;


   public HairBasic stalkRope;
   public HairBasic tipRope;

   public Transform target;
   private Transform oTarget;


   public Vector3 vel;
   public Vector3 force;
   public float stateLastChangeTime;

   private Vector3 tv1;
   private Quaternion tq1;

   public override void Create(){

     print(tipsGO);

    for( int i = 0; i < tipsGO.Length; i++ ){
      
      if( tipsGO[i] != null ){ 
        Cycles.Remove(tipInfos[i]);
        DestroyImmediate( tipsGO[i] );
      }
    }

    tipsGO = new GameObject[numTips];
    tipInfos = new TipInfo[numTips];

    for( int i = 0; i < numTips; i++ ){
      GameObject go = Instantiate( tipPrefab ) as GameObject;
      go.transform.position = brain.position;
      go.transform.parent = tipBuffer.gameObject.transform;
      tipsGO[i] = go;
    
      
      tipInfos[i]     = tipsGO[i].GetComponent<TipInfo>();
      tipInfos[i].hydra  = this;
      tipInfos[i].id     = i;
      tipInfos[i].nID    = (float)i/(float)numTips;

      SafeInsert(tipInfos[i]);

    }


    print(tipBuffer);


    tipBuffer.transforms = new Transform[ numTips * 2 ];
    stalkBuffer.transforms = new Transform[ 2 ];

    for(int i = 0; i < numTips; i++ ){
      tipBuffer.transforms[i*2+0] = brain.transform;
      tipBuffer.transforms[i*2+1] = tipsGO[i].transform;
    }

    stalkBuffer.transforms[0] = anchor;
    stalkBuffer.transforms[1] = brain;

   
    SafeInsert( tipBuffer );
    SafeInsert( tipRope);

    SafeInsert( stalkBuffer );
    SafeInsert( stalkRope );
 

   
   }

   public override void Bind(){

   }

   public override void OnLive(){

    stalkRope.Set();
    tipRope.Set();
  
   }


   public override void WhileLiving(float v){

    if( active ){


      if( target!= oTarget){ stateLastChangeTime = Time.time; }
      oTarget = target;

    Vector3 force = new Vector3();

    Vector3 targetPos = anchor.position + Vector3.up * verticalBrainHolderPos* (1 + .2f * Mathf.Sin(Time.time * .9f + 200));;


    Vector3 outness = Vector3.Scale((brain.position - targetPos) ,new Vector3(1,0,1));

    // make it so we try not to be directly above hydra body
    targetPos += outness.normalized *  brainMinOut * (1 + .2f * Mathf.Sin(Time.time * 1));

    force += (targetPos - brain.position) * verticalBrainHolderForce;
  
  

    float shortest = 1000;
    int cID = 0;
    


    // Looking for closest food
    for( int i =0; i < food.foods.Length; i++ ){

      if( !food.canSpawn[i] ){
        tv1 = food.foods[i].position - brain.transform.position;
        if( tv1.magnitude < shortest ){
          shortest = tv1.magnitude;
          cID = i;
        }
      }
    }





    target = person;

   



    Vector3 f1 = Vector3.zero;
   // cID = 0;

    int totalHunting = 0;
    for( int i = 0; i < tipInfos.Length; i++ ){

      if( tipInfos[i].hunting == true ){ totalHunting ++; }
      if(tipInfos[i].force.magnitude > f1.magnitude){
        f1 = tipInfos[i].force;
      }

    }




    // if enough are hunting, it means we are looking at food
    // not at person
    Vector3 pDif = person.position - brain.position;

    if( (float)totalHunting/(float)tipInfos.Length > pDif.magnitude  ){ 
      lookingAtPerson = false;
    }else{
      lookingAtPerson = true;
    }

    // also if close enough to brain we also aren't looking at person!
    if( shortest < brainToFoodDist ){
      lookingAtPerson = false;
    }

    if( lookingAtPerson ){

      if((person.position - brain.position).magnitude < brainToPersonDist ){
        force += (person.position - brain.position).normalized *  brainToPersonForce;
        target = person;
      }
    }else{
      target =  food.foods[cID]; 

      force += (food.foods[cID].position - brain.transform.position) * brainToFoodForce *  Mathf.Clamp((Time.time - stateLastChangeTime),0,1);

      if( (food.foods[cID].position - brain.transform.position).magnitude < .01f ){
        food.EatFood(cID);
      } 
    }



    //force -= f1;// * .1f;

 





      vel += force * .4f;

      brain.position += vel;
      tq1 = brain.rotation;
      brain.LookAt( target.position );


      eye.position = brain.position;// -(brain.position - target.position ).normalized  + brain.position;
     

      //eye.LookAt( target.position );// = Quaternion.Slerp( eye.rotation , brain.rotation  ,1 );
      eye.rotation = Quaternion.Slerp( eye.rotation , brain.rotation  ,.1f  * Mathf.Clamp((Time.time - stateLastChangeTime)*3,0,1));
      
      brain.rotation = Quaternion.Slerp( tq1 , brain.rotation  , headLookSlerpSpeed  * Mathf.Clamp((Time.time - stateLastChangeTime),0,1));
      

      //stalkBuffer.transforms[1].transform.Rotate( -90, 0, 0 );

      vel *= .9f;  

    }

   }



}
}