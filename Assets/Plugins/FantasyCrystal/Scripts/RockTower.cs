using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[ExecuteAlways]
public class RockTower : MonoBehaviour
{


    public RockCutter rock;
    public int numRocks;
   // Make sure first time we drag prefab into scene it generates for us :)
    public void OnEnable(){
        //if(  transform.GetComponent<MeshFilter>().sharedMesh == null ){
            RegenerateCluster();
        //}
    }
    public void RegenerateCluster()
    {
        
        CombineInstance[] combine = new CombineInstance[numRocks];

        // For each crystal, place, rotate, etc. the transform
        // and assign values that will effect the 'cut' of the crystal
        // THEN, cut the crystal, and put its mesh into our CombineInstance list
        // also add our desire location


        Vector3 oldTop = new Vector3(0,1,0);
        Vector3 basePos = new Vector3(0,0,0);

        float rockSize = 1;

        for( int i = 0; i < numRocks; i++ ){

            rockSize = 3 / ( (float)i + 3 );
            rock.bottomPosition = basePos;
            rock.bottomDirection = -oldTop;

            oldTop = Random.insideUnitSphere +  new Vector3(0,3,0);
            basePos += oldTop * rockSize;
            rock.topPosition = basePos;
            rock.topDirection = oldTop;
            basePos += oldTop * rockSize * .03f;
            rock.crystalHeight = rockSize;
            rock.crystalRadius = rockSize;

            rock.Cut();

            Vector3 pos = Vector3.zero;
            Quaternion rot = Quaternion.identity;
            Vector3 scale = Vector3.one;

            combine[i].transform = Matrix4x4.TRS( pos , rot , scale);
            combine[i].mesh = rock.mesh;

        }


        // GO ahead and combine the meshes 
        // ( using the big index incase there are too many verts! )
        transform.GetComponent<MeshFilter>().sharedMesh = new Mesh();
        transform.GetComponent<MeshFilter>().sharedMesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
        transform.GetComponent<MeshFilter>().sharedMesh.CombineMeshes(combine);
    }

}
