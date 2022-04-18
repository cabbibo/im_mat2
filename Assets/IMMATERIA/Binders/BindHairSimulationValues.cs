using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BindHairSimulationValues : Binder
{

public float _NoiseSize;
public float _NoiseForce;
public Vector3 _NoiseSpeed;
    public override void Bind()
    {
        toBind.BindFloat("_NoiseSize"   ,() => _NoiseSize);
        toBind.BindFloat("_NoiseForce"  ,() => _NoiseForce);
        toBind.BindVector3("_NoiseSpeed",() => _NoiseSpeed);
    }
}
