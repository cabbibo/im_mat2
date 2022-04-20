using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[ExecuteAlways]
public class RockCutter : CrystalCutter
{


    public Vector3 topPosition;
    public Vector3 topDirection;


    
    public Vector3 bottomPosition;
    public Vector3 bottomDirection;


    public override void Reset(){

        float h = crystalHeight*100;
        float r = crystalRadius*100;
         faces = new List<List<Vector3>>();
     
        List<Vector3> topFace = new List<Vector3>();
        List<Vector3> bottomFace = new List<Vector3>();
        for( int i = 0; i < 3; i++ ){

            float a = (float)i/(float)3;
            float a2 = (float)((i+1)%3)/(float)3;
            a *= Mathf.PI * 2;
            a2 *= Mathf.PI * 2;
            
            float fR = r;//  r * .5f * UnityEngine.Random.Range(.8f , 1.2f );

            var face = new List<Vector3>();
            
            Vector3 midPoint = (topPosition - bottomPosition) * .5f + bottomPosition;

            face.Add( midPoint + new Vector3(Mathf.Sin(a) *fR , -h , -Mathf.Cos(a) * fR ));
            face.Add( midPoint + new Vector3(Mathf.Sin(a2) *fR , -h , -Mathf.Cos(a2) * fR ));
            face.Add( midPoint + new Vector3(Mathf.Sin(a2) *fR , h , -Mathf.Cos(a2) * fR ));
            face.Add( midPoint + new Vector3(Mathf.Sin(a) *fR , h , -Mathf.Cos(a) * fR ));

            faces.Add(face);


            bottomFace.Add( midPoint +new Vector3(Mathf.Sin(a) *fR , -h , -Mathf.Cos(a) * fR ) );
            topFace.Add(   midPoint + new Vector3(Mathf.Sin(a) *fR , h , -Mathf.Cos(a) * fR ) );

        }


        faces.Add(topFace);
        bottomFace.Reverse();
        faces.Add(bottomFace);

    }


    public override void SetUpGemCut(){

        cutPositions = new List<Vector3>();
        cutDirections = new List<Vector3>();
        Vector3 topPoint = new Vector3( 0, crystalHeight, 0);
        Vector3 d; Vector3 p;

        Vector3 midPoint = (topPosition - bottomPosition) * .5f + bottomPosition;;



       cutPositions.Add(topPosition );
       cutDirections.Add(topDirection );

        cutPositions.Add(bottomPosition);
        cutDirections.Add(bottomDirection );




        Vector3[] ps = new Vector3[10];
        for( int i = 0; i< 40; i++ ){

            p = Random.onUnitSphere;

            p.Scale(new Vector3(1,1,1));
            p = p.normalized * crystalRadius*3 * Random.Range(.6f,1.6f);
        
        
            cutPositions.Add(p + midPoint );
            cutDirections.Add(p.normalized );

        }




        List<List<Vector3>> facesTmp = new List<List<Vector3>>();


       /* int id = 0;
        foreach( var face in faces){
            if( id %2 == 0){
           
                List<Vector3> faceTmp = new List<Vector3>();
                for(int i = 0; i < face.Count; i++ ){
                    faceTmp.Add(face[i]);
                }
                facesTmp.Add(faceTmp);
            }

            id ++;

        }

        foreach( var face in facesTmp){

            Vector3 nor =  -Vector3.Cross( face[0+1] - face[0] , face[0+2]- face[0]).normalized;
            for(int i = 0; i < face.Count; i++ ){

                Vector3 nor2 =  (face[i]- new Vector3(0,crystalHeight/2,0)).normalized;
              //  Cut(face[i] - nor2 * .3f , nor2 );

            }

        }*/


        
        // 3 more cuts to turn the triangular prism
        // into a hexagon. ( the randomness factor will make some sides bigger than others)
        for( int i = 0; i < 3; i++  ){
            float a = (((float)i)/(float)3) * 2 * Mathf.PI;

            float x = Mathf.Sin(a);
            float y = -Mathf.Cos(a);
             p  = new Vector3(x , 0 , y) * 1;// * .5f * UnityEngine.Random.Range(.5f , 1.5f );
             d = new Vector3(x , 0 , y );

           // cutPositions.Add(p);
           // cutDirections.Add(d);
        }

       




 
        // doing the top 'cuts' of the crystal
        for(int i = 0;  i< 6; i++){

            float a = (((float)i )/(float)6) * 2 * Mathf.PI;
            
            float r = crystalRadius * .5f;
            float x =  Mathf.Sin(a) * r;
            float y = -Mathf.Cos(a) * r;
            

            Vector3 dir = new Vector3( x , crystalRadius *  cutAngle , y ).normalized;

            p  = new Vector3(x , crystalHeight , y) ;
        
            // move the cut position off by the normal to create some diversity in cut
            p -= dir * UnityEngine.Random.Range( -crystalRadius * .3f, crystalRadius * .5f);


            d = topPoint - p;

            Vector3 tang = Vector3.Cross(dir, Vector3.up);
            d = Vector3.Cross(d , tang );
            d = d.normalized;


            // cutPositions.Add(p);
           //cutDirections.Add(dir.normalized);
        }





    }


}
