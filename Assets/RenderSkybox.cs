using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.PostProcessing;


[ExecuteAlways]
public class RenderSkybox : MonoBehaviour
{


    public Camera skyboxCam;
    
    private RenderTexture rt;

    private int tmpCullingMask;
    private float tmpDepth;
    private CameraClearFlags tmpClearFlags;

    private DepthTextureMode tmpDepthTextureMode;

    private RenderTexture tmpRT;
    public GameObject skybox;

    
    private FogPost fog;
    PostProcessProfile profile;
    public PostProcessVolume volume;

    public void OnEnable()
    {

        
        profile = volume.sharedProfile;
        volume.profile.TryGetSettings(out fog);

        if( rt != null ){
            rt.Release();
            rt = new RenderTexture( (Screen.width), (Screen.height ), 0);
        }else{
            rt = new RenderTexture( (Screen.width), (Screen.height ), 0);
        }
    }

    public void OnDisable()
    {

        print("death is happening");
        rt.Release();
    }
    void SetSkyboxCameraSettings(){
        
        tmpCullingMask = skyboxCam.cullingMask;
        tmpDepth = skyboxCam.depth;
        tmpClearFlags = skyboxCam.clearFlags;
        tmpDepthTextureMode = skyboxCam.depthTextureMode;
        tmpRT = skyboxCam.targetTexture;

        fog.enabled.value = false;

        skyboxCam.cullingMask = 1 << 13;
        skyboxCam.depth = -2;
        skyboxCam.clearFlags = CameraClearFlags.Skybox;
        skyboxCam.depthTextureMode = DepthTextureMode.None;
        skyboxCam.targetTexture = rt;
      
    
    
    }


    void ResetCameraSettings(){

        skyboxCam.cullingMask = tmpCullingMask;
        skyboxCam.depth = tmpDepth;
        skyboxCam.clearFlags = tmpClearFlags;
        skyboxCam.depthTextureMode = tmpDepthTextureMode;
        skyboxCam.targetTexture = tmpRT;
        fog.enabled.value = true;

    }
 
 
 
// command buffers will not work to get the skybox
    // ugh
    void Update(){
     
 
        SetSkyboxCameraSettings();
        //skybox.SetActive(true);
        skyboxCam.Render();
        //skybox.SetActive(false);
        Shader.SetGlobalTexture("_Skybox", rt);
        ResetCameraSettings();
 
    }

}
