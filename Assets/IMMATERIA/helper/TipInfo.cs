using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace IMMATERIA {
public class TipInfo : Cycle
{

  public Hydra hydra;

  public int id;
  public float nID;

  public Transform closestFood;
  public float    closestDist;
  public int closestID;

  public Transform closestPlayer;

  public Transform currentTarget;


  public bool lookingAtPlayer;
  public bool lookingAtFood;

  public bool hunting;

  public float timeLastAte;

  public Vector3 force;
  public Vector3 vel;

  private Quaternion tmpQuat1;
  private Quaternion tmpQuat2;
  private Quaternion tmpQuat3;
  private Vector3 tv1;


  public override void WhileLiving(float v){

    force = Vector3.zero;
    

        float a = nID * (2 * Mathf.PI);

        float r = 2;

        Vector3 offset = (Mathf.Sin( a ) * r * hydra.brain.right +  Mathf.Cos( a ) * r * hydra.brain.up) * (1+ .2f * Mathf.Sin( nID * 301 + Time.time * .3f ));
        
        Vector3 targetPos = hydra.brain.position + offset * hydra.armRingSize + hydra.brain.forward * hydra.armRingForwardSize;

      
        
      
        closestDist = 10000;
        

        for( int i =0; i < hydra.food.numFoods; i++ ){

          //Vector3 dif = transform.position - hydra.food.foods[i].position;
          Vector3 dif = transform.position - hydra.food.foods[i].position;
          if( !hydra.food.canSpawn[i] ){
            if( dif.magnitude < closestDist ){
              closestFood = hydra.food.foods[i];
              closestDist = dif.magnitude;
              tv1 = dif;
              closestID = i;
            }
          }


        }


// force += (targetPos - transform.position) * hydra.armToRingForce *100;//  * (1.6f+Mathf.Sin( nID * 100 + Time.time * .3f ));


        if( transform.position.y < 1 ){ force += Vector3.up  * Mathf.Pow( (1-transform.position.y),3); }
        if( closestDist < hydra.armToFoodDist ){



          Vector3 pDif = hydra.person.position - transform.position;
         
          if( pDif.magnitude > closestDist ){
            force -= ((hydra.armToFoodDist-tv1.magnitude)/hydra.armToFoodDist) * hydra.armToFoodForce * tv1.normalized;
            currentTarget = closestFood;
            hunting = true;
          }
            

            if( closestDist < .1f ){
              hunting = false;
              hydra.food.EatFood(closestID);
            }


          force += (targetPos - transform.position) * .3f* hydra.armToRingForce  * (1.6f+Mathf.Sin( nID * 100 + Time.time * .3f ));
        }else{

          hunting = false;
          currentTarget = hydra.target;

          
          force += (targetPos - transform.position) * hydra.armToRingForce  * (1.6f+Mathf.Sin( nID * 100 + Time.time * .3f ));
        }


        //if( )


        
        float height =  data.land.SampleHeight( transform.position ) - transform.position.y;

        if( height+1 > 0 ){
          force += Vector3.up * 1* (height+1);
        }

        vel += force;
        transform.position += vel;





        //transform.position = transform.position; 
        tmpQuat1 = transform.rotation;
        transform.LookAt( transform.position + force );
        tmpQuat2 = transform.rotation;
        transform.LookAt( currentTarget );
        tmpQuat3 = transform.rotation;
        tmpQuat2 = Quaternion.Slerp( tmpQuat3 , tmpQuat2 , Mathf.Clamp( force.magnitude * .1f , 0 , 1));


        //tmpQuat1.Rotate( 90, 0, 0 );

        transform.rotation = Quaternion.Slerp( tmpQuat1 ,tmpQuat2 , hydra.armLookSlerpSpeed );
       // transform.LookAt( hydra.target.position );
        //transform.Rotate( -90, 0, 0 );


        vel *= hydra.armDampening;
    
  }


}
}
