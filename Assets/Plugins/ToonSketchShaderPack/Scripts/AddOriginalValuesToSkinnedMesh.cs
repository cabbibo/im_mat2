using System.Collections;
using System.Collections.Generic;
using UnityEngine;



namespace ToonSketchShaderPack
{
public class AddOriginalValuesToSkinnedMesh : MonoBehaviour
{
    void OnEnable()
    {
        
        Mesh mesh = GetComponent<SkinnedMeshRenderer>().sharedMesh;

        // Adding our preskinned verts and normals to our mesh
        // so that we can do triplanar mapping with out it changing from skinning!
        
        mesh.SetUVs( 2 , mesh.vertices);
        mesh.SetUVs( 3 , mesh.normals);

        GetComponent<SkinnedMeshRenderer>().sharedMesh = mesh;

    }
}
}