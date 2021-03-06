﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering;
using UnityEngine.Rendering.PostProcessing;
 
[ExecuteAlways]
public class PostController : MonoBehaviour
{   

    public float focusDistance;
    public float depthFocusChangeSpeed;
    public float bloomValue;

    public float bloomChangeSpeed;

    public float vignetteIntensity;
    public float vignetteChangeSpeed;
    public PostProcessVolume volume;
    private Bloom thisBloom;
    private DepthOfField dof;
    private Vignette vignette;
    PostProcessProfile profile;
 
    public void OnEnable()
    {
        profile = volume.sharedProfile;
        volume.profile.TryGetSettings(out thisBloom);
        volume.profile.TryGetSettings(out dof);
        volume.profile.TryGetSettings(out vignette);
      
    }

    void Update(){
        thisBloom.intensity.value = Mathf.Lerp(thisBloom.intensity.value, bloomValue, bloomChangeSpeed);
        dof.focusDistance.value = Mathf.Lerp( dof.focusDistance.value , focusDistance , depthFocusChangeSpeed);
        vignette.intensity.value =  Mathf.Lerp(vignette.intensity.value,vignetteIntensity, vignetteChangeSpeed);
    }
}
