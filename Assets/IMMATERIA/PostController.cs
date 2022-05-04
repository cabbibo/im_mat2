using System.Collections;
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
    public float chromaticAbberationIntensity;
    public float chromaticAbberationChangeSpeed;

    public float vignetteIntensity;
    public float vignetteChangeSpeed;
    public PostProcessVolume volume;
    private Bloom bloom;
    private ChromaticAberration chromaticAbberation;
    private DepthOfField dof;
    private Vignette vignette;
    PostProcessProfile profile;
 
    public void OnEnable()
    {
        profile = volume.sharedProfile;
        volume.profile.TryGetSettings(out bloom);
        volume.profile.TryGetSettings(out dof);
        volume.profile.TryGetSettings(out vignette);
        volume.profile.TryGetSettings(out chromaticAbberation);
      
    }

    void Update(){
        bloom.intensity.value = Mathf.Lerp(bloom.intensity.value, bloomValue, bloomChangeSpeed);
        dof.focusDistance.value = Mathf.Lerp( dof.focusDistance.value , focusDistance , depthFocusChangeSpeed);
        vignette.intensity.value =  Mathf.Lerp(vignette.intensity.value,vignetteIntensity, vignetteChangeSpeed);
        chromaticAbberation.intensity.value =  Mathf.Lerp(chromaticAbberation.intensity.value,chromaticAbberationIntensity, chromaticAbberationChangeSpeed);
    }
}
