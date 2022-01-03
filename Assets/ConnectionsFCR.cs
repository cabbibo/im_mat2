
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class ConnectionsFCR : LifeForm {

  public Life set;
  public Life force;
  public Life constraint;
  public Life pass;
  public Life resolve;


    
  public Form Points;
  public Form Connections;

  public float[] transformArray;

  public int numIterations = 1;

  public override void Create(){

    transformArray = new float[16];

    
    /*  
      All of this info should be visualizable!
    */

    SafePrepend( set );
    SafePrepend( force );
    SafePrepend( constraint );
    SafePrepend( pass );
    SafePrepend( resolve );
    SafePrepend( Connections );
    SafePrepend( Points );

    //Cycles.Insert( 4 , Base );


  }


  public override void Bind(){


    set.BindPrimaryForm("_VertBuffer", Points);
    set.BindForm("_ConnectionBuffer", Connections );
    
    
    force.BindPrimaryForm("_VertBuffer", Points);
    force.BindForm("_ConnectionBuffer", Connections );
    
    
constraint.BindPrimaryForm("_VertBuffer", Points);
constraint.BindForm("_ConnectionBuffer", Connections );


resolve.BindPrimaryForm("_VertBuffer", Points);
resolve.BindForm("_ConnectionBuffer", Connections );


pass.BindPrimaryForm("_VertBuffer", Points);
pass.BindForm("_ConnectionBuffer", Connections );



    force.BindFloats( "_Transform" , () => this.transformArray );

    data.BindCameraData(force);

  }


  public override void OnBirth(){
    set.active = true;
    force.active = false;
    constraint.active = false;
    resolve.active = false;
    pass.active = false;
  }

  public override void Activate(){
    set.active = true;
    force.active = false;
    constraint.active = false;
    resolve.active = false;
    pass.active = false;
  }

  public override void WhileLiving(float v){
    

    
    //set.active = false;
    force.active = false;
    constraint.active = false;
    resolve.active = false;
    pass.active = false;

    //set.active = false;
    transformArray = HELP.GetMatrixFloats( transform.localToWorldMatrix );
  




  resolve._SetUpDispatch();
  resolve._DispatchShader();
    for( int i = 0; i < numIterations; i++ ){

      force._SetUpDispatch();
      force._DispatchShader();

      constraint._SetUpDispatch();
      constraint._DispatchShader();

      pass._SetUpDispatch();
      pass._DispatchShader();


    }
  
  }




  public void Set(){
    print("HIII");
    set.YOLO();
  }




}