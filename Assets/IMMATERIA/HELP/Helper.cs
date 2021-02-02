using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Helper : Cycle
{


    public AudioClip errorClip;
    public AudioClip successClip;


    public void OnPageCantGoBack()
    {

        data.sound.PlayOne(errorClip);
        print("Sorry but you can't go back from this position");
    }


    public void OnPageLocked()
    {

        data.sound.PlayOne(errorClip);
        print("Sorry but this page is locked! Youll have to do something to fix it! ");
    }

    public void OnSuccessUnlock()
    {
        data.sound.PlayOne(successClip);
        print("GOOD JOB U ONLOCKTIOD!");
    }

    public void NoCurrentStory()
    {
        print("NO CURRENT STORY");
    }


    public void NoStoriesDesired()
    {
        print("NO current stories desired! NO states have been verified as usuable");
    }
    public void MultipleStoriesDesired()
    {
        print("Multiple current stories desired! multiple states have been verified as usuable");
    }

    public GameObject nodePrefab;
    public GameObject[] nodes;


    public override void OnLive()
    {

        if (debug)
        {
            for (int i = 0; i < nodes.Length; i++)
            {
                DestroyImmediate(nodes[i]);
            }


        }

    }

}
