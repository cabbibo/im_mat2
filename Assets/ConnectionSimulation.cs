using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConnectionSimulation :  Cycle {



    public Form Particles;
    public Form ConnectionList;

    public Life Set;
    public Life Forces;
    public Life Constraint;
    public Life Resolve;

    public override void Create(){
        SafePrepend( Particles );
        SafePrepend( ConnectionList  );
        SafePrepend( Set );
        SafePrepend( Constraint );
        SafePrepend( Resolve );
    }


    public override void Bind()
    {
        Set.BindPrimaryForm("_VertBuffer", Particles );
        Set.BindForm("_ConnectionList", ConnectionList );

        Forces.BindPrimaryForm("_VertBuffer",Particles );
        Forces.BindForm("_ConnectionList",ConnectionList ); 
        
        Constraint.BindPrimaryForm("_VertBuffer",Particles );
        Constraint.BindForm("_ConnectionList",ConnectionList ); 

        Resolve.BindPrimaryForm("_VertBuffer",Particles );
        Resolve.BindForm("_ConnectionList",ConnectionList ); 
        
    }
}
