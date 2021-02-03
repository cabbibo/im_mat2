
Shader "Debug/LandTileRing" {
    Properties {

        _Color ("Color", Color) = (1,1,1,1)
        _Size ("Size", float) = .01
    }


    SubShader{
        Cull Off
        Pass{

            CGPROGRAM
            
            #pragma target 4.5

            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            #include "../Chunks/terrain.cginc"




            uniform float3 _Center;
            uniform float _Size;
            uniform float _RingSize;

            uniform int _TileDimensions;
            uniform int _WhichGrid;

            
            


            //uniform float4x4 worldMat;

            //A simple input struct for our pixel shader step containing a position.
            struct varyings {
                float4 pos      : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            //Our vertex function simply fetches a point from the buffer corresponding to the vertex index
            //which we transform with the view-projection matrix before passing to the pixel program.
            varyings vert (uint id : SV_VertexID){

                varyings o;

                int base = id / 6;
                int alternate = id %6;

                int whichGrid = base / ( _TileDimensions * _TileDimensions);
                int whichInGrid = base % (_TileDimensions * _TileDimensions);
                int whichXInGrid = whichInGrid / _TileDimensions;
                int whichYInGrid = whichInGrid % _TileDimensions;

                float xInGrid = float( whichXInGrid)/float(_TileDimensions);
                float yInGrid = float( whichYInGrid)/float(_TileDimensions);

                float x;
                float y;

                if( whichGrid == 0){
                    x = -1;
                    y = -1;
                }

                if( whichGrid == 1 ){
                    x = 0;
                    y = -1;
                }

                if( whichGrid == 2 ){
                    x = 1;
                    y = -1;
                }

                if( whichGrid == 3 ){
                    x = -1;
                    y = 0;
                }
                
                if( whichGrid == 4 ){
                    x = 1;
                    y = 0;
                }


                if( whichGrid == 5){
                    x = -1;
                    y = 1;
                }

                if( whichGrid == 6 ){
                    x = 0;
                    y = 1;
                }

                if( whichGrid == 7 ){
                    x = 1;
                    y = 1;
                }

                x += xInGrid;
                y += yInGrid;

                

                float3 worldPos = float3((_Center.x) * _Size , 0 , (_Center.y)  * _Size );

                if( alternate == 0 || alternate == 3 ){
                    x += 0/float(_TileDimensions);
                    y += 0/float(_TileDimensions);
                    }else if( alternate == 1 ){
                    x += 1/float(_TileDimensions);
                    y += 0/float(_TileDimensions);
                    }else if( alternate == 2 || alternate == 5 ){
                    x += 1/float(_TileDimensions);
                    y += 1/float(_TileDimensions);
                    }else{
                    x += 0/float(_TileDimensions);
                    y += 1/float(_TileDimensions);
                }
                



                worldPos += (x- (2/3)) * _RingSize * float3( 1,0,0);
                worldPos += (y- (2/3)) * _RingSize * float3( 0,0,1);

                worldPos = terrainWorldPos(worldPos);
                o.uv = (float2(x,y) + 1) / 3;

                

                

                o.pos = mul (UNITY_MATRIX_VP, float4(worldPos,1.0f));

                
                return o;

            }




            //Pixel function returns a solid color for each point.
            float4 frag (varyings v) : COLOR {
                return float4(v.uv.x,0,v.uv.y,1 );
            }

            ENDCG

        }
    }


}
