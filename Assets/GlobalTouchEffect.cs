﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalTouchEffect : Cycle
{
    

    public StrokeGranSynth synth;
    
    public Transform touchRepresent;

    public GuideParticles particleEmitter;

    public ExtraParticlesBinder epb;

    public override void Create(){
        data.inputEvents.WhileDownDelta.AddListener( WhileDown );
        data.inputEvents.OnUp.AddListener( OnUp );

        epb =  particleEmitter.GetComponent<ExtraParticlesBinder>();
    }


    public override  void WhileLiving(float v ){

        if( data.inputEvents.hit.transform ){
            if( data.inputEvents.hit.transform.gameObject != this.gameObject){
                synth.on = false;
            }
        }
        if( data.inputEvents.Down  < 1){
           
            synth.on = false;
        }
    }
    public void WhileDown( Vector2 d ){

        if( data.inputEvents.hit.transform != null ){

        
                touchRepresent.position = data.inputEvents.hit.point;
                Vector2 uv = data.inputEvents.hit.textureCoord;
                synth.on = true;
                synth.location = (uv.x / 400) * 100;
                synth.speed = (1-uv.y);
                synth.length = (1-uv.y);
                synth.pitch = (1-uv.y) * 3 + 1;

                synth.playPosition = data.inputEvents.hit.point;

                particleEmitter.EmitOn();
                epb._Gravity = data.inputEvents.hit.normal * .001f;
                epb._SpawnRingRadius = 0.05f;  
                epb._CurlNoiseStrength = 1.1f;
                epb._CurlNoiseSpeed = 0;
                epb._CurlNoiseSize = 11;
                epb._DeathSpeed = .01f;
                particleEmitter.SetEmitterPosition(data.inputEvents.hit.point);

        }
    }

     public void OnUp( ){
        if( data.inputEvents.hit.transform != null ){
            if( data.inputEvents.hit.transform.gameObject == this.gameObject){
                particleEmitter.EmitOff();
            }
        }

    }


}