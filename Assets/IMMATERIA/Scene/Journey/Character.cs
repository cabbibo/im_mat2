using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Character : Cycle
{

    public Animator animator;
    public RuntimeAnimatorController runtime;

    public bool doTerrain;

    public Vector3 moveTarget;
    public Vector3 oMoveTarget;


    public float runMultiplier;
    public float maxSpeed;
    public float forwardCutoff;

    public Vector3 velocity;
    public Vector3 force;

    public float moveForce;
    public float dampening;

    private Vector3 tmpPos;
    private Quaternion tmpRot;


    private Vector3 oPos;
    private Quaternion oRot;

    private Vector3 deltaPos;
    private Quaternion deltaRot;

    public bool rotating;
    public float angleOffset;
    public float forwardOffset;

    public Transform moveTargetTransform;
    public bool movingTowardsTarget;

    public bool lerping;
    public float lerpSpeed;
    public float lerpStartTime;
    public Transform lerpTarget;
    private Vector3 startLerpPos;
    private Quaternion startLerpRot;

    public GameObject OnGroundBook;
    public GameObject InHandBook;
    public EpiphanyRing epiphanyRing;

    public bool canMove;



    public Transform forwardRep;
    public Transform leftRep;
    public Transform rightRep;
    public Transform backRep;
    public Transform baseRep;
    public Transform velRep;

    public Transform movementRep;

    public bool downToLeft;

    public float steepnessVal;

    public Transform head;

    public float minHitY;

    public bool onTerrainObject;
    public GameObject terrainObject;



    public float forwardMultiplier;
    public float turnMultiplier;

    public float steepnessMultiplier;

    public float velocityMultiplier;


    public float steepnessCutoff;

    public bool cantMove;


    public override void Create()
    {

        moveTarget = transform.position;
        oRot = Quaternion.identity;
        deltaRot = Quaternion.identity;
        oPos = Vector3.zero;
        velocity = Vector3.zero;
        animator.Play("Grounded");


    }

    public bool playAnimation;

    public override void WhileLiving(float v)
    {


        // fixing error ( usually in edit mode ) of there being no runtime controller
        if (animator.runtimeAnimatorController == null) { animator.runtimeAnimatorController = runtime; }

        if (lerping)
        {

            DoMovement();
        }
        else
        {
            MyPhysics();
        }

        float animationSpeedMultiplier = 1;
        // once we assign all the values, then we update the animator
        if( Application.isPlaying){
            animationSpeedMultiplier = .1f;
        }

        if( playAnimation ){
            animator.Update(Time.deltaTime * animationSpeedMultiplier);
        }


    }

    public void Fall()
    {
        data.state.hasFallen = false;
        animator.SetBool("Falling", true);
        animator.SetBool("FallAsleep", false);
        animator.SetBool("GetUp", false);
    }

    public void OnPickUp()
    {
        data.state.PickUpBook();
        animator.SetBool("PickUp", false);
    }

    public void PickUp()
    {
        animator.SetBool("PickUp", true);
    }


    public void FallAsleep()
    {
        data.state.hasFallen = true;
        animator.SetBool("FallAsleep", true);
        animator.SetBool("Falling", false);
        animator.SetBool("GetUp", false);
    }

    public void GetUp()
    {
        animator.SetBool("FallAsleep", false);
        animator.SetBool("Falling", false);
        animator.SetBool("GetUp", true);
    }

    public void FallBackAsleep()
    {
        animator.SetBool("FallAsleep", true);
        animator.SetBool("Falling", false);
        animator.SetBool("GetUp", false);
    }








    // Turns our player so that we are looking a new direction
    public void SwipeTurn(Vector2 delta)
    {

        if (data.book.started == false && data.state.inPages == false)
        {
            if (delta.magnitude > 10)
            {
                movingTowardsTarget = false;
            }
            angleOffset -= delta.x * .003f;
            forwardOffset -= delta.y * .004f;
            forwardOffset = Mathf.Max(forwardOffset, 0);
            rotVel += delta.x * .3f;
        }

    }

    /*

      Movement Stuff

    */

    void DoMovement()
    {


        oPos = transform.position;
        oRot = transform.rotation;

        force = Vector3.zero;

        // If we are moving towards something specific, 
        // Set our move target towards taht position
        if (movingTowardsTarget && moveTargetTransform)
        {
            moveTarget = moveTargetTransform.position;
        }

        // This is if we are moving towards a locked position
        if (lerping)
        {

            // get our current position in the lerp
            float v = Mathf.Clamp((Time.time - lerpStartTime) / lerpSpeed, 0, 1);
            if (lerpSpeed == 0) { v = 1; }

            // cubic curve it.
            v = v * v * (3 - 2 * v);

            // Getting our lerped position
            transform.position = Vector3.Lerp(startLerpPos, lerpTarget.position, v);
            transform.rotation = Quaternion.Slerp(startLerpRot, lerpTarget.rotation, v);

            // setting the values
            // TODO: WHy not 0?!?
            animator.SetFloat("Turn", 0, 0  , Time.deltaTime);
            animator.SetFloat("Forward", 0, 0, Time.deltaTime);

        }/*else{


      
      // When we click we are moving towards a target!
      if( movingTowardsTarget ){

        // Getting the difference between our current position
        // and the target position
        Vector3 dif = moveTarget-transform.position;
        
        // Using this to add to the force
        // in this case, the higher our velocity already is
        // the more the force will be, giving the speed up 
        force =  dif * moveForce * (velocity.magnitude + .01f);
        
        // dampenign our angle offset
        angleOffset *= .5f;

        // using that force to define how far forward our offset is

        forwardOffset = Mathf.Lerp( forwardOffset  , force.magnitude * .01f , .1f );


      }else{


        if( Mathf.Abs(angleOffset) < .1f ){
          angleOffset = 0;
        }

        // if we have moved towards our location, then we just need to figure our rotation
        // aka when you are swipe turning
        force = angleOffset  * transform.right   + forwardOffset * transform.forward;/// dif * .01f * moveForce;// * (velocity.magnitude+.13f);
      
      
      }
    
      force *= .1f;
    
      velocity += force;
      velocity *= dampening;

      if( velocity.magnitude > maxSpeed ){
        velocity = velocity.normalized * maxSpeed;
      }


  


    // Getting our turning and our forward amount by comparing out velocity
    // to our transforms direction
    Vector3 m = transform.InverseTransformDirection(velocity);
    float turn = Mathf.Atan2(m.x, m.z) * m.magnitude;
    float forward = m.z;

    canMove = true;
    float d = 1;

    // This is how we do the terrain!
    if( doTerrain ){


        // Getting the height where we are
        float h = data.land.SampleHeight( transform.position );

        // getting the height in front of us
        Vector3 f =  transform.position + transform.forward * 3 * .8f;
        float h2 = data.land.SampleHeight( f );
        forwardRep.position = new Vector3(f.x , h2 , f.z);


        // getting the height to the right of us
        f = transform.position + transform.forward * 3* .4f + transform.right* 3 * .4f;
        float h3 = data.land.SampleHeight( f );
        rightRep.position = new Vector3(f.x , h3 , f.z);


         // getting the height to the left of us
        f = transform.position + transform.forward* 3 * .4f - transform.right * 3* .4f;
        float h4 = data.land.SampleHeight( f );
        leftRep.position = new Vector3(f.x , h4 , f.z);


        // Getting the current normal of the terrain
        Vector3 normal = data.land.SampleNormal( transform.position );


        // dotting our normal with up vector 
        // to get how 'steep' the terrain is
        d = Vector3.Dot( normal , Vector3.up );
        steepnessVal = d;

        if( h4 < h3 ){ downToLeft = true; }else{ downToLeft = false;}


        // If the terrain is steep enough, we can't move up it!
        if( h2 > h && steepnessVal < .9 ){
         // print("can't move");
          canMove = false;
        }


    }

      // Automatically play the animator so we get less errors
      if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 1 && !animator.IsInTransition(0)){
        animator.Play("Grounded");
      }

      // Here is where we set the animations for the rotations
      Rotate(forward , turn );

      if( moveTargetTransform && movingTowardsTarget ){
         
         Vector3 dif = moveTarget-transform.position;

         // up or down tells us if we are looking left or right
         float upOrDown =Mathf.Sign(Vector3.Cross(transform.forward, moveTargetTransform.forward ).y);
        
         turn += upOrDown * Vector3.Angle(transform.forward, moveTargetTransform.forward ) * .05f   * Mathf.Clamp( 1-  3 *dif.magnitude ,0,1);

      }

     animator.SetFloat("Turn", turn, 0.1f, Time.deltaTime);


    float currentForward = animator.GetFloat("Forward");

      // if its not too steep
      if( canMove ){
        if( forward < forwardCutoff ){ forward = 0; }
        //Mathf.Lerp( currentForward , forward*runMultiplier, .2f)
        animator.SetFloat("Forward",forward*runMultiplier , 0.1f, Time.deltaTime);
      }else{

        float t = 1;
        if( downToLeft ){ t = -1; }

        angleOffset += t;
        //animator.SetFloat("Turn", turn, 0.1f, Time.deltaTime);
        
        animator.SetFloat("Forward", d*d*d, 0.1f, Time.deltaTime);
      }

      transform.rotation = Quaternion.Euler(0, transform.eulerAngles.y, 0);

      animator.runtimeAnimatorController = animator.runtimeAnimatorController;
      if( doTerrain ){
        float h = data.land.SampleHeight( transform.position );
        
        Vector3 normal = data.land.SampleNormal( transform.position );
        //float d = Vector3.Dot( normal , Vector3.up );

        float h2 = data.land.SampleHeight( transform.position + transform.forward * .5f );

        animator.SetFloat("Steepness", (1-d) * 4 );

        if( h2 > h ){
          animator.SetBool( "Uphill" , true);
        }else{
          animator.SetBool("Uphill", false);
      
          if( (1-d) * 10 > 1 ){
            transform.position += velocity;
          }
        }



        transform.position = new Vector3( transform.position.x , h , transform.position.z);
      }


      velRep.position = transform.position + velocity * 10;
      velRep.LookAt( transform.position );
      velRep.position += Vector3.up;
      velRep.localScale = new Vector3(velocity.magnitude, velocity.magnitude ,velocity.magnitude*20)* 7;
      deltaPos = transform.position - oPos;

      velocity = deltaPos;


      angleOffset *= .9f;
      forwardOffset *= .98f;

    }



    baseRep.position = transform.position;*/
    }

    void OnCollisionEnter(Collision c)
    {
        print("HIIII");

    }

    void OnCollisionStay(Collision c)
    {
        print("HIIII");

    }

    void OnTriggerEnter(Collider c)
    {
        print("HOOOy");
    }
    public void SetMoveTarget(Vector3 p)
    {

        oMoveTarget = transform.position;
        moveTarget = p;

        movementRep.position = (oMoveTarget - moveTarget) * .5f + oMoveTarget;
        movementRep.LookAt(moveTarget);

        movementRep.position += Vector3.up;
        movementRep.localScale = new Vector3(.1f, .1f, (oMoveTarget - moveTarget).magnitude);
        movingTowardsTarget = true;
        lerping = false;
    }

    public void SetMoveTarget(Transform p)
    {
        oMoveTarget = transform.position;
        moveTargetTransform = p;
        moveTarget = p.position;

        movementRep.position = -(oMoveTarget - moveTarget) * .5f + oMoveTarget;
        movementRep.LookAt(moveTarget);
        movementRep.position += Vector3.up;
        movementRep.localScale = new Vector3(.1f, .1f, (oMoveTarget - moveTarget).magnitude);

        movingTowardsTarget = true;
        lerping = false;
    }

    public void SetLerpTarget(Transform p, float speed)
    {
        lerping = true;
        movingTowardsTarget = false;
        lerpTarget = p;
        lerpStartTime = Time.time;
        lerpSpeed = speed;
        startLerpRot = transform.rotation;
        startLerpPos = transform.position;
    }






    void Rotate(float f, float t)
    {

        // help the character turn faster (this is in addition to root rotation in the animation)
        float turnSpeed = 1000;//Mathf.Lerp(0, 360, f);
        transform.Rotate(0, t * turnSpeed * Time.deltaTime, 0);

    }

    // TODO
    public void LookAt(Transform t)
    {

    }



    public float rotVel;
    public float rotForce;

    public float myMaxSpeed;
    public float myMaxRotSpeed;

    public bool locked;

    public void MyPhysics()
    {


if( !locked ){



        // Getting the height where we are
        float h = data.land.SampleHeight(transform.position);




        // Bit shift the index of the layer (0) to get a bit mask
        int layerMask = 1 << 11;

        // This would cast rays only against colliders in layer 0.
        // But instead we want to collide against everything except layer 0. The ~ operator does this, it inverts a bitmask.
        layerMask = ~layerMask;
        RaycastHit hit;
        if (Physics.Raycast(head.position, -transform.up, out hit, Mathf.Infinity, layerMask))
        {
            minHitY = hit.point.y;
            terrainObject = hit.collider.gameObject;
        }
        else
        {
            minHitY = 0;
        }


        if (minHitY > h)
        {
            onTerrainObject = true;
            h = minHitY;
        }
        else
        {
            terrainObject = null;
        }





        // getting the height in front of us
        Vector3 f = transform.position + transform.forward * 3 * .8f;
        float h2 = data.land.SampleHeight(f);
        forwardRep.position = new Vector3(f.x, h2, f.z);
        

        f = transform.position - transform.forward * 3 * .8f;
        float h3 = data.land.SampleHeight(f);
        backRep.position = new Vector3(f.x, h3, f.z);


        // getting the height to the right of us
        f = transform.position + transform.right * 3 * .8f;
        float h4 = data.land.SampleHeight(f);
        rightRep.position = new Vector3(f.x, h4, f.z);


        // getting the height to the left of us
        f = transform.position - transform.right * 3 * .8f;
        float h5 = data.land.SampleHeight(f);
        leftRep.position = new Vector3(f.x, h5, f.z);



        Vector3 d = forwardRep.position - transform.position;

        float forwardSteepness = Vector3.Dot(d,Vector3.up);







        // Getting the current normal of the terrain
        Vector3 normal = data.land.SampleNormal(transform.position);


        force = Vector3.zero;

        //  force += (leftRep.position - transform.position) * 10f;
        //  force += (rightRep.position - transform.position) * 10f;
        //


        //force += normal * 10f;

        float hDifFront = h - h2;

        // add a force that adds 'gravity'
        //force += transform.forward * hDifFront * 2;



        if (movingTowardsTarget)
        {
            Vector3 dif = moveTarget - transform.position;


            force += dif;



            float angle = Vector3.Angle(transform.forward, dif.normalized);
            float py = Vector3.Cross(transform.forward, dif.normalized).y;
            angle *= py;
            rotForce = angle * .3f;
            rotVel += rotForce;


            velocity += force * .0003f;



        }


        // Checking to see if we are hitting anything in front of us
        // to make it so we can block off sections
        if (Physics.Raycast(transform.position, transform.forward, out hit, 1, layerMask))
        {

            if (hit.collider.gameObject != terrainObject)
            {

                // making sure that the object in front of us isn't a terrain object
                // and also that we aren't on a terrain object!
                if (onTerrainObject == false || hit.collider.gameObject.tag != "TerrainObject")
                {
                    velocity = Vector3.zero;
                    cantMove = true;
                }

            }
            else
            {


                print("colliding with object im on");
            }
        }


        if( Mathf.Abs(forwardSteepness)  > steepnessCutoff ){
            velocity = Vector3.zero;
            cantMove = true;
        }




        if (velocity.magnitude > myMaxSpeed) { velocity = velocity.normalized * myMaxSpeed; }
        transform.Rotate(0, rotVel * Time.deltaTime, 0);
        transform.position += velocity * velocityMultiplier * Time.deltaTime;


            Quaternion q = Quaternion.FromToRotation(transform.up, Vector3.up) * transform.rotation;
            transform.rotation = Quaternion.Slerp(transform.rotation, q, .1f);




        transform.position += Vector3.up * (h - transform.position.y);


        rotVel *= .9f;
        velocity *= .9f;

        animator.SetFloat("Forward", velocity.magnitude *  forwardMultiplier, 0.1f, Time.deltaTime);
        animator.SetFloat("Turn", rotVel * turnMultiplier , 0.1f, Time.deltaTime);
        animator.SetBool("Uphill", hDifFront < 0);
        animator.SetFloat("Steepness", -hDifFront * steepnessMultiplier, 0.1f, Time.deltaTime);



}else{

    rotVel = 0;
    velocity = Vector3.zero;
    force =  Vector3.zero;
        animator.SetFloat("Forward", 0, 0.1f, Time.deltaTime);
        animator.SetFloat("Turn",0 , 0.1f, Time.deltaTime);
        animator.SetBool("Uphill",true);
        animator.SetFloat("Steepness", 0, 0.1f, Time.deltaTime);

}
    }




}
