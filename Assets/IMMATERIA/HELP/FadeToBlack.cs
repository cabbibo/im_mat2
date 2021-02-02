using UnityEngine;


public class FadeToBlack : Cycle
{
    public AnimationCurve FadeCurve = new AnimationCurve(new Keyframe(0, 1), new Keyframe(0.6f, 0.7f, -1.8f, -1.2f), new Keyframe(1, 0));

    private Material _material;
    public float startFadeSpeed;

    public override void OnLive()
    {

        _material = GetComponent<Renderer>().sharedMaterial;

        _material.SetColor("_Color", new Color(0, 0, 0, 0));

        fadeColor = new Color(0, 0, 0, 1); ;
        currentOpacity = 1;
        startOpacity = 1;
        fadeOpacity = 0;
        FadeIn(startFadeSpeed);

    }


    public bool fading;
    public float fadeStartTime;
    public float fadeSpeed;
    public float fadeOpacity;
    public Color fadeColor;

    public float startOpacity;
    public float currentOpacity;

    public void FadeOut(Color color, float time)
    {

        fading = true;
        fadeStartTime = Time.time;
        fadeSpeed = time;
        fadeColor = color;
        fadeOpacity = 1;
        startOpacity = currentOpacity;

    }

    public void FadeIn(float time)
    {

        fading = true;
        fadeStartTime = Time.time;
        fadeSpeed = time;
        fadeOpacity = 0;
        startOpacity = currentOpacity;

    }

    public override void WhileLiving(float val)
    {

        if (fading == true)
        {

            float v = (Time.time - fadeStartTime) / fadeSpeed;
            if (v >= 1)
            {
                fading = false;
            }
            else
            {
                currentOpacity = Mathf.Lerp(fadeOpacity, startOpacity, FadeCurve.Evaluate(v));
                _material.SetColor("_Color", new Color(fadeColor.r, fadeColor.g, fadeColor.b, currentOpacity));
            }

        }

    }
}