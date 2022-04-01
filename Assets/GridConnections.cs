using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class GridConnections : Form
{


    public Form toIndex;
    public int totalNumberConnections;
    public float connectionLengthMultiplier;



    public struct VertConnections{
        public int vertID;
        public int idInConnectionList;
        public int totalNumberConnections;

        public List<int> connectionIDs;
        public List<float> connectionLengths;
    }
    

    public List<VertConnections> connectionList;
    public List<int> fullConnectionID1List;
    public List<int> fullConnectionID2List;
    public List<float> fullConnectionLengthList;


    public int rows;
    public int cols;


    int getID( int x , int y ){
        return x * rows  + y ;
    }

    public override void SetStructSize(){
        structSize = 3;
    }
    

    public override async void SetCount(){
        

        GridVerts cv = (GridVerts)toIndex;

        rows = cv.rows;
        cols = cv.cols;

        count = 0;

        // building out our full connection list and info!
        for( int i = 0; i< cols; i++ ){
            for( int j = 0; j < rows; j++){


              
                for( int k = 0; k < 8; k++){
                    if( k == 0){
                        if( i != 0 && j != 0){
                            count ++;
                        }
                    }        

                    if( k == 1){
                        if( j != 0){
                            count ++;
                        }
                    }    


                    if( k == 2){
                        if( i < cols-1 && j != 0){
                            count ++;
                        }
                    }     


                     if( k == 3){
                        if( i != 0){
                            count ++;
                        }
                    } 

                    if( k == 4){
                        if( i < cols-1){
                            count ++;
                        }
                    }  


                    if( k == 5){
                        if( i != 0 && j < rows-1){
                            count ++;
                        }
                    }        

                    if( k == 6){
                        if( j < rows-1){
                            count ++;
                        }
                    }    


                    if( k == 7){
                        if( i < cols-1 && j < rows-1){
                            count ++;
                        }
                    }     
 
                    
                           
                }


            }
        }

    }
    public override void Embody(){


        float[] toIndexData = toIndex.GetData();

        connectionList = new List<VertConnections>();


        fullConnectionID1List = new List<int>();
        fullConnectionID2List = new List<int>();
        fullConnectionLengthList = new List<float>();

        // building out our full connection list and info!
        for( int i = 0; i< cols; i++ ){
            for( int j = 0; j < rows; j++){

                int id = i * rows + j;

                


                // Grabbing position form our original data
                Vector3 vertPos = new Vector3();
                vertPos.x = toIndexData[ id * toIndex.structSize + 0 ];
                vertPos.y = toIndexData[ id * toIndex.structSize + 1 ];
                vertPos.z = toIndexData[ id * toIndex.structSize + 2 ];


                Vector2 vertUV = new Vector2();
                vertUV.x = toIndexData[ id * toIndex.structSize + 12 ];
                vertUV.y = toIndexData[ id * toIndex.structSize + 13 ];

                float radius = 1;//Mathf.Lerp( Mathf.Pow(vertUV.y,.3f) , (vertUV.y*vertUV.y)*1.2f , vertUV.y);

                VertConnections vc = new VertConnections();
                vc.vertID = id;
                vc.idInConnectionList = fullConnectionLengthList.Count;
                vc.connectionIDs = new List<int>();
                vc.connectionLengths = new List<float>();

      
                int nX; int nY; 



                // Connections to row beneath
                if( j != 0 ){
                    nY = j-1;
                    
                    nX = i-1;
                    if( nX >= 0 ){ 
                        AddConnectionInfo( vertPos,radius , nX,nY,vc,toIndexData);
                    }

                    nX = i;
                    AddConnectionInfo( vertPos,radius , nX,nY,vc,toIndexData);

                    nX = i+1;
                    if( nX < cols ){ 
                        AddConnectionInfo( vertPos,radius , nX,nY,vc,toIndexData);        
                    }         

                }


                // connection to same row
                nY = j;
                
                nX = i-1;
                if( nX >= 0 ){ 
                    AddConnectionInfo( vertPos,radius , nX,nY,vc,toIndexData);
                }

                nX = i+1;
                if( nX < cols ){ 
                    AddConnectionInfo( vertPos,radius , nX,nY,vc,toIndexData);        
                }   

                // connection to rowAbove
                  if( j < rows-1 ){
                    nY = j+1;
                    

                    nX = i-1;
                    if( nX >= 0 ){ 
                        AddConnectionInfo( vertPos,radius , nX,nY,vc,toIndexData);
                    }

                    nX = i;
                    AddConnectionInfo( vertPos,radius , nX,nY,vc,toIndexData);
                
                    nX = i+1;
                    if( nX < cols ){ 
                        AddConnectionInfo( vertPos,radius , nX,nY,vc,toIndexData);        
                    }              

                }

                vc.totalNumberConnections = vc.connectionIDs.Count;

                
                connectionList.Add(vc);

            }

        }

        // Build our own buffer
        float[] values = new float[count * structSize];
        for( int i = 0; i < count; i++){
            values[i * 3 + 0 ] = fullConnectionID1List[i];
            values[i * 3 + 1 ] = fullConnectionID2List[i];
            values[i * 3 + 2 ] = fullConnectionLengthList[i];
        }

        SetData(values);


        print( toIndex.count);
        print(connectionList.Count);
        // Then we need to INSERT that information into the vert buffer
        for( int i = 0; i < toIndex.count; i++ ){
            VertConnections vc = connectionList[i];
            toIndexData[ (i+1) * toIndex.structSize - 2] = vc.idInConnectionList;
            toIndexData[ (i+1) * toIndex.structSize - 1] = vc.totalNumberConnections;
        }

        // And reset that data!
        toIndex.SetData(toIndexData);

    }

    public void AddConnectionInfo(  Vector3 vertPos , int nX , int nY  , VertConnections vc , float[] toIndexData ){
       int nID = getID(nX,nY);

        Vector3 otherVert = new Vector3();
        otherVert.x = toIndexData[ nID * toIndex.structSize + 0 ];
        otherVert.y = toIndexData[ nID * toIndex.structSize + 1 ];
        otherVert.z = toIndexData[ nID * toIndex.structSize + 2 ];


        float cLength = (vertPos - otherVert).magnitude;
        cLength *= connectionLengthMultiplier;
        
        AddConnectionInfo(vc, nID,cLength);

    }


    
    public void AddConnectionInfo(  Vector3 vertPos,float lengthMultiplier , int nX , int nY  , VertConnections vc , float[] toIndexData ){
       int nID = getID(nX,nY);

        Vector3 otherVert = new Vector3();
        otherVert.x = toIndexData[ nID * toIndex.structSize + 0 ];
        otherVert.y = toIndexData[ nID * toIndex.structSize + 1 ];
        otherVert.z = toIndexData[ nID * toIndex.structSize + 2 ];


        float cLength = (vertPos - otherVert).magnitude;
        cLength *= lengthMultiplier;
        cLength *= connectionLengthMultiplier;
        
        AddConnectionInfo(vc, nID,cLength);

    }


    public void AddConnectionInfo(  Vector3 vertPos , float length , int nX , int nY  , VertConnections vc  ){
       int nID = getID(nX,nY);
        AddConnectionInfo(vc, nID,length);

    }


     public void AddConnectionInfo( VertConnections vc , int connectedID , float length ){
       
        vc.connectionIDs.Add(connectedID);
        vc.connectionLengths.Add(length);

        fullConnectionID1List.Add(vc.vertID);
        fullConnectionID2List.Add(connectedID);
        fullConnectionLengthList.Add(length);

    }


    public void InsertInfo(){

    }


    public override void WhileDebug()
    {
        
        mpb.SetBuffer("_VertBuffer", toIndex._buffer);
        mpb.SetBuffer("_ConnectionBuffer", _buffer);
        mpb.SetInt("_Count",count);
        
        Graphics.DrawProcedural(debugMaterial,  new Bounds(transform.position, Vector3.one * 5000), MeshTopology.Triangles, count * 3 * 2 , 1, null, mpb, ShadowCastingMode.Off, true, LayerMask.NameToLayer("Debug"));

    }
}
