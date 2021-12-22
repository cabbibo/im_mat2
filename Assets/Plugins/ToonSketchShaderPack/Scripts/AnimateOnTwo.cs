using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Animations;

namespace ToonSketchShaderPack{
public class AnimateOnTwo : MonoBehaviour
{


    public float FPS;
    public Animator animator;
    // Start is called before the first frame update
    
    float delta;
    public float speed;
    void Start(){
        delta = 0;
    }


    void Update () {
        
        // clamp time to fps
        delta += Time.deltaTime;
        float t = Mathf.Floor(delta * FPS);
        t /= (FPS / speed);

        
        // get currently playing animation
        AnimatorClipInfo[] st = animator.GetCurrentAnimatorClipInfo(0);
        if (st.Length > 0) {

            float frame = (1.0f/st[0].clip.length) * t;
            animator.speed =  0.0f;
            // Setting the parameter that *needs to be exposed* in the animation clip
            animator.SetFloat("Time",frame);

        }
        
    }
}
}
