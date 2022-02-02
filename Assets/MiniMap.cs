using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniMap : TransferLifeForm
{


    public float moveMultiplier;
    public float dampening;
    public Vector2 mapCenter;
    public Vector3 mapSize;
    

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


    public GameObject storyMarkerPrefab;
    public StoryNode[] storyMarkers;
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

        for( int i = 0; i < storyMarkers.Length; i++ ){

            if( storyMarkers[i] != null ){
                JumpDeath(storyMarkers[i]);
                Cycles.Remove(storyMarkers[i]);
                data.inputEvents.RemoveTapWatcher( storyMarkers[i]);
                DestroyImmediate(storyMarkers[i].gameObject);  
            }
        }

        storyMarkers = null;

    }


     Vector2 centerVel;
    public void DownDelta( Vector2 dir ){

        centerVel -= dir * moveMultiplier;

    }


    public override void WhileLiving(float v)
    {

        mapCenter += centerVel;
        centerVel *= dampening;


        for( int i = 0; i < storyMarkers.Length; i++ ){

            storyMarkers[i].transform.position = GetMiniMapPosition( data.journey.monoSetters[i].transform.position );

        }

    }


    public Vector3 GetMiniMapPosition( Vector3 pos ){


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


        return transform.TransformPoint( mPos );


    }

    public override void _Activate()
    {
        print("IM BEIGN ACToVADO");
    }

    public override void _Deactivate()
    {

        print("DEAVTIVADO");
        DestroyNodes();
    }



    public void SetStoryNodeInfo(){



    }

}
