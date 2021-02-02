using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class Reincarnator : Cycle
{

    public StorySetter theFall;

    public Page firstPage;

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
        data.player.position = firstPage.lerpTarget.position + new Vector3(0, 10, 0);
        data.player.rotation = firstPage.lerpTarget.rotation;
        data.cameraControls.CameraHolder.position = firstPage.transform.position;
        data.cameraControls.CameraHolder.rotation = firstPage.transform.rotation;

        data.state.reincarnationNumber++;
        theFall.EnterOuter();
        theFall.EnterInner();
        theFall.StartStory();



    }

    public void Finished()
    {
        fadingIn = false;
        fadingOut = false;
        reincarnating = false;
    }
    public override void WhileLiving(float val)
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
