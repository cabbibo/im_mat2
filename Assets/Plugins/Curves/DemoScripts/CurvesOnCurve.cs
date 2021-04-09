using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MagicCurve;
using static Unity.Mathematics.math;
using Unity.Mathematics;


[ExecuteInEditMode]
public class CurvesOnCurve : MonoBehaviour
{


    public Curve curve;

    public int numTubes = 8;
    public int lengthSegments = 50;
    public int radialSegments = 6;
    public float radius = 1;

    public float minTubeRadius;
    public float maxTubeRadius;
    public bool applyCurveSizeToTubeSize;
    
    public float tubeRadiusOsscilateSpeed;
    public float tubeRadiusOsscilateSpeedRandomness;

    public float minOffsetRadius;
    public float maxOffsetRadius;
    public float offsetOsscilateSpeed;
    public float offsetOsscilateSpeedRandomness;
    public bool applyCurveSizeToOffsetSize;

    public float spiralSpeed;
    public float spiralSpeedRandomness;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    Vector3[] positions;
    Vector3[] normals;
    Vector4[] tangents;
    Vector2[] uvs;
    int[] triangles;

    public int totalVertCount;
    public int totalTriCount;

    MeshFilter filter;   
    
    public void OnEnable(){
        filter = GetComponent<MeshFilter>();
        curve = GetComponent<Curve>();
        curve.BakeChanged.AddListener(BuildMesh);
    }

     public void OnDisable(){
        curve.BakeChanged.AddListener(BuildMesh);
    }


    void BuildMesh(Curve c){
        
        totalVertCount = numTubes * lengthSegments * radialSegments;
        totalTriCount = numTubes * (lengthSegments-1) * (radialSegments-1) * 3 * 2;

        positions = new Vector3[totalVertCount];
        normals = new Vector3[totalVertCount];
        tangents = new Vector4[totalVertCount];
        uvs = new Vector2[totalVertCount];
        triangles = new int[totalTriCount];

        // Building the triangles first
        int index  = 0;
        for( int i = 0; i < numTubes; i++){
            for( int j = 0; j < lengthSegments-1; j++ ){
                for( int k = 0; k < radialSegments-1; k++ ){
                    

                    // Getting indicies to build a tube
                    int baseID = i * (  lengthSegments*radialSegments) + radialSegments * j + k;
                    int id1 = baseID;
                    int id2 = baseID + 1;
                    int id3 = baseID + radialSegments;
                    int id4 = baseID + radialSegments + 1;

                    triangles[index++] = id1;
                    triangles[index++] = id2;
                    triangles[index++] = id4;
                    triangles[index++] = id1;
                    triangles[index++] = id4;
                    triangles[index++] = id3;

                }
            }
        }

        // reset index of array
        index  = 0;
        
       float3 pos; float3 fwd; float3 up; float3 rit; float scale;
        for( int i = 0; i < numTubes; i++ ){

            float randomVal1= UnityEngine.Random.Range( 0, .99f);
            float randomVal2= UnityEngine.Random.Range( 0, .99f);
            float randomVal3= UnityEngine.Random.Range( 0, .99f);

            for( int j = 0; j < lengthSegments; j++){
                
                float lengthAlongTube = (float)j/(lengthSegments-1);
                curve.GetDataFromValueAlongCurve( lengthAlongTube , out pos ,out fwd, out up, out rit, out scale );
                
                

                float outAngle = lengthAlongTube * ( spiralSpeed + spiralSpeedRandomness * randomVal1 );
                float offsetAngle = lengthAlongTube * ( offsetOsscilateSpeed + offsetOsscilateSpeedRandomness * randomVal2 );
                float outRad = Mathf.Lerp( minOffsetRadius , maxOffsetRadius , (sin(offsetAngle) + 1)/2);

                float useCurveWidth = applyCurveSizeToOffsetSize ? scale : 1;
                float3 centerPos = pos + (sin(outAngle) * rit - cos( outAngle) * up) * outRad * useCurveWidth;
                
                float tubeAngle =  lengthAlongTube * ( tubeRadiusOsscilateSpeed + tubeRadiusOsscilateSpeedRandomness * randomVal3 );
                useCurveWidth = applyCurveSizeToTubeSize ? scale : 1;
                float tubeRadius =  Mathf.Lerp( minTubeRadius , maxTubeRadius , (sin(tubeAngle) + 1)/2) * useCurveWidth;

              
                for( int k = 0; k < radialSegments; k++ ){
                    
                    float aroundness = ((float)k/(radialSegments-1));
                    float angle = aroundness * Mathf.PI*2;

                    float xAmount = Mathf.Sin(angle);
                    float yAmount = Mathf.Cos(angle);
                    float3 fPos = centerPos + (rit * xAmount + up * yAmount) * tubeRadius;// curve.GetOffsetPositionFromValueAlongCurve( lengthAlongTube , xAmount*w*radius, yAmount*w*radius );
                    float3 normal = fPos - centerPos;
                    float4 tangent = float4(cross(normal,fwd),1);
                    float2 uv = float2( lengthAlongTube, aroundness);

                    positions[index] = transform.InverseTransformPoint(fPos);
                    tangents[index] = float4(transform.InverseTransformDirection(tangent.xyz),1);
                    normals[index] = transform.InverseTransformDirection(normal);
                    uvs[ index] = uv;

                    index++;

                }
            }
        }


        Mesh m = new Mesh();

        
        m.Clear();

        m.vertices = positions;
        m.tangents = tangents;
        m.normals = normals;
        m.uv = uvs;
        m.triangles = triangles;

        filter.mesh = m;





    }

}
