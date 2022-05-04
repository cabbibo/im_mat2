using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniMap : TransferLifeForm
{


    public float moveMultiplier;
    public float dampening;
    public Vector2 mapCenter;
    public Vector3 mapSize;

    public Book book;

    public GameObject storyMarkerPrefab;
    public StoryNode[] storyMarkers;


    public GameObject playerMarkerPrefab;
    public PlayerNode[] playerMarker;

    public Transform playerMarkerTransform;


    

    public override void Bind()
    {

        transfer.BindVector2("_MiniMapCenter"   , ()=> mapCenter);
        transfer.BindVector3("_MiniMapSize"     , ()=> mapSize );
        data.land.BindData(transfer);

    }


    public override void OnBirthed()
    {

        centerVel = Vector2.zero;

        mapCenter.x = data.player.position.x;
        mapCenter.y = data.player.position.z;
        
    }


    public override void Create(){
        DestroyNodes();
        CreateNodes();
    }

    public void CreateNodes(){
        storyMarkers = new StoryNode[ data.journey.monoSetters.Length ];
        for( int i = 0; i < storyMarkers.Length; i++ ){
            storyMarkers[i] = (StoryNode)(Instantiate(storyMarkerPrefab)).GetComponent<Cycle>();
            storyMarkers[i].transform.rotation = Quaternion.LookRotation( transform.forward, transform.up);

            
            storyMarkers[i].transform.parent = transform;
            storyMarkers[i].setter = data.journey.monoSetters[i];
            storyMarkers[i].id = i;
            storyMarkers[i].miniMap = this;
            SafeInsert(storyMarkers[i]);

            data.inputEvents.AddTapWatcher( storyMarkers[i]);
        }
    }

    public void DestroyNodes(){

        if( storyMarkers != null ){
            for( int i = 0; i < storyMarkers.Length; i++ ){

                if( storyMarkers[i] != null ){
                    JumpDeath(storyMarkers[i]);
                    Cycles.Remove(storyMarkers[i]);
                    data.inputEvents.RemoveTapWatcher( storyMarkers[i]);
                    DestroyImmediate(storyMarkers[i].gameObject);  
                }
            }
        }

        storyMarkers = null;

    }


     Vector2 centerVel;
    public void DownDelta( Vector2 dir ){

    
        if( data.inputEvents.downHitObject == book.frame.collider.gameObject ){
    
            centerVel -= dir * moveMultiplier;
        }


    }


    public float textSizeMultiplier;
    public override void WhileLiving(float v)
    {

        mapCenter += centerVel;
        centerVel *= dampening;

        
        float iSize = 1/data.land.size;

        if( mapCenter.x < 0 ){ centerVel -= Vector2.left * 1;   }
        if( mapCenter.x > iSize){ centerVel += Vector2.left * 1;  }

        
        if( mapCenter.y < 0 ){ centerVel += Vector2.up * 1;  }
        if( mapCenter.y > iSize){ centerVel -= Vector2.up * 1;  }


        for( int i = 0; i < storyMarkers.Length; i++ ){
            Vector3 t1 = Vector3.one;

            float lP = GetLocalPoint(data.journey.monoSetters[i].transform.position).magnitude;
            lP=Mathf.Clamp((lP - .0f)/(1-.0f),0,1);
            t1*=(1 / (20+(lP*mapSize.x)));
            storyMarkers[i].transform.localScale = t1 * textSizeMultiplier;
            storyMarkers[i].transform.position = GetMiniMapPosition( data.journey.monoSetters[i].transform.position ) + transform.up * .1f;


        }
        
        playerMarkerTransform.position = GetMiniMapPosition( data.player.position );

        playerMarkerTransform.LookAt( GetMiniMapPosition( data.player.position + data.player.forward * 3 ) );
        
        playerMarkerTransform.position = GetMiniMapPosition( data.player.position ) + transform.up * .05f;

    }


    public Vector3 GetLocalPoint(Vector3 pos){
       Vector3 mPos = new Vector3();

        mPos.x = pos.x - mapCenter.x;
        mPos.y = 0;
        mPos.z = pos.z - mapCenter.y;


        mPos.x /= mapSize.x;
        mPos.z /= mapSize.z;

        if( mPos.magnitude > 1 ){
            mPos = mPos.normalized;
        }

        Vector3 fullPos = mPos;
        fullPos.x *= mapSize.x;
        fullPos.z *= mapSize.z;

        fullPos.x = fullPos.x + mapCenter.x;
        fullPos.y = 0;
        fullPos.z = fullPos.z + mapCenter.y;


        mPos.y = data.land.SampleHeight(fullPos);
        mPos.y /= mapSize.y;
        return mPos;

    }
    public Vector3 GetMiniMapPosition( Vector3 pos ){


        Vector3 mPos = GetLocalPoint(pos);
        return transform.TransformPoint( mPos );


    }

    public override void _Activate()
    {
        gameObject.SetActive(true);
        playerMarkerTransform.gameObject.SetActive(true);
    }

    public override void _Deactivate()
    {

        gameObject.SetActive(false);
        playerMarkerTransform.gameObject.SetActive(false);
        DestroyNodes();
    }



    public void SetStoryNodeInfo(){



    }

}
