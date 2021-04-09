using System.Collections;
using System.Collections.Generic;
using UnityEngine;


using UnityEngine.Rendering;

[ExecuteAlways]
public class VolumetricLightRays : MonoBehaviour
{

    public int numRays;
    public bool dynamic;


    public Camera cam;
    public int renderSizeX;
    public int renderSizeY;

    public float rayNear;
    public float rayFar;
    public Vector2 camSize;
    private RenderTexture texture;

    public string layerToRender;

    public RenderTextureDescriptor textureDescriptor;


    private MaterialPropertyBlock mpb;

    public Material material;

    public bool lightRays;

    public bool groundSpeckles;
    public Material groundSpeckleMat;

    public MeshRenderer debugRenderer;

    // Start is called before the first frame update

    

    // Getting our render texture
    void OnEnable()
    {
        textureDescriptor = new RenderTextureDescriptor( renderSizeX,renderSizeY,RenderTextureFormat.Depth,24);
        texture = new RenderTexture( textureDescriptor );
        UpdateDepth();
        UpdateMaterials();
    }

    // Rerednering our depth so 
    public void UpdateDepth(){
        cam.targetTexture = texture;
        cam.orthographicSize = camSize.y; 
        cam.aspect = camSize.x/camSize.y; 
        cam.nearClipPlane = rayNear;
        cam.farClipPlane = rayFar;
        cam.depthTextureMode = DepthTextureMode.DepthNormals;
        cam.SetTargetBuffers( texture.colorBuffer , texture.depthBuffer );
        cam.Render();

    }


    // Making sure our camera info fits well 
    // into our shader!
    public void UpdateMaterials(){
        
        if( mpb == null ){
            mpb = new MaterialPropertyBlock();
        }

        mpb.SetTexture("_DepthTexture", texture);
        mpb.SetTexture("_MainTex", texture);
        mpb.SetMatrix("_CameraMatrix",cam.transform.localToWorldMatrix);
        mpb.SetFloat("_CameraNear",cam.nearClipPlane);
        mpb.SetFloat("_CameraFar",cam.farClipPlane);
        mpb.SetVector("_CameraSize", camSize );
        

        if( debugRenderer ){
            debugRenderer.SetPropertyBlock(mpb);
        }

    }
    // Update is called once per frame
    void Update()
    {


        // only rerender if we want it to be dynamic!
        if( dynamic ){
            UpdateDepth();
            UpdateMaterials();
        }

        
    
            
        // draw the rays and light speckles
        if( lightRays ){
            Graphics.DrawProcedural( material ,  new Bounds(transform.position, Vector3.one * 5000), MeshTopology.Triangles, numRays *3 * 2 , 1, null, mpb, ShadowCastingMode.Off, true, LayerMask.NameToLayer("Default"));
        }
        
        if( groundSpeckles ){
              Graphics.DrawProcedural( groundSpeckleMat ,  new Bounds(transform.position, Vector3.one * 5000), MeshTopology.Triangles, numRays *3 * 2 , 1, null, mpb, ShadowCastingMode.Off, true, LayerMask.NameToLayer("Default"));
        }   
    }
}
