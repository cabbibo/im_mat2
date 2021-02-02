﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;



[ExecuteInEditMode]
public class Reincarnator : MonoBehaviour
{


    public bool reincarnating;

    public float fadeInTime;
    public float fadeOutTime;

    public FadeToBlack fader;

    public float reincarnationStartTime;

    public Color fadeColor;


    public bool fadingOut;
    public bool fadingIn;

    public void Reincarnate()
    {
        print("Reincarnating");
        reincarnating = true;
        reincarnationStartTime = Time.time;

        fader.FadeOut(fadeColor, fadeOutTime);
        fadingOut = true;

    }

    public void Rebirth()
    {
        fadingOut = false;
        print("Rebirthiing");
        fader.FadeIn(fadeInTime);
        reincarnating = true;
        reincarnationStartTime = Time.time;
        fadingIn = true;

    }

    public void Finished()
    {
        fadingIn = false;
        fadingOut = false;
        reincarnating = false;
    }
    public void Update()
    {


        if (fadingOut)
        {
            float v = (Time.time - reincarnationStartTime) / fadeOutTime;

            if (v > 1)
            {
                Finished();
            }
        }

        if (fadingIn)
        {
            float v = (Time.time - reincarnationStartTime) / fadeInTime;

            if (v > 1)
            {
                Finished();
            }
        }

    }


}
