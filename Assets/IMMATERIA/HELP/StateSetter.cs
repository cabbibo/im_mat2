using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateSetter : MonoBehaviour
{


  public God god;
  public StorySetter storySetter;
    
  public int whichMonolithEmitting;
  public bool monolithParticlesEmitting;
  public bool bookPickedUp;
  public bool hasFallen;
  public StorySetter[] storiesVisited;
  public StorySetter[] storiesCompleted;


  public StorySetter[] storiesNotVisited;
  public StorySetter[] storiesNotCompleted;
  public int reincarnationNumber;


  public bool completeStory;



  public bool Check(){

    print("CHECKINGIN");

    bool canStart = true;
    for( int i = 0; i < storiesVisited.Length; i++ ){

        if( !god.data.state.storiesVisited.Contains(storiesVisited[i])){
          canStart = false;
        }

    }


    for( int i = 0; i < storiesNotVisited.Length; i++ ){

        if( god.data.state.storiesVisited.Contains(storiesNotVisited[i])){
          canStart = false;
        }

    }

    if( god.data.state.reincarnationNumber != reincarnationNumber ){
      canStart = false;
    }

    return canStart;
    

  }


  public void OnEnd(){


    if( completeStory ){
      god.data.state.storiesCompleted.Add(storySetter);
    }


    if( god.data.state.storiesVisited.IndexOf(storySetter) < 0 ){
      god.data.state.storiesVisited.Add(storySetter);
    }

  }



  // Need to set all of the values for each story we have visited too!
  public void Assign(){

    for( int i = 0; i < storiesVisited.Length; i++  ){
        
      if( god.data.state.storiesVisited.IndexOf( storiesVisited[i]) < 0 ){
        god.data.state.storiesVisited.Add( storiesVisited[i]);
      }

    }

  }


}
