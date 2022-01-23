using UnityEngine;


public class FadeToBlack : Cycle
{
    public AnimationCurve FadeCurve = new AnimationCurve(new Keyframe(0, 1), new Keyframe(0.6f, 0.7f, -1.8f, -1.2f), new Keyframe(1, 0));

    private Material _material;
    public float startFadeSpeed;

    public bool startFadeOut;

    public override void OnLive()
    {

        _material = GetComponent<Renderer>().sharedMaterial;

        _material.SetColor("_Color", new Color(0, 0, 0, 1));

        fadeColor = new Color(0, 0, 0, 1); ;
        currentOpacity = 1;
        startOpacity = 1;
        fadeOpacity = 0;
       
        if( startFadeOut ){ FadeIn(startFadeSpeed); }

    }



    public bool fading;
    public float fadeStartTime;
    public float fadeSpeed;
    public float fadeOpacity;
    public Color fadeColor;

    public float startOpacity;
    public float currentOpacity;


    public void FadeOut( float time ){
        
        _material.SetInt("_Texture", 0);
        FadeOut( Color.black , time );
    }

    public void FadeToTexture(Texture t,float time){
        _material.SetTexture("_MainTex", t);
        _material.SetInt("_Texture", 1);
        FadeOut(Color.black,time);
    }

    public void SetBlack(){
         fadeColor = new Color(0, 0, 0, 1); ;
        currentOpacity = 1;
        startOpacity = 1;
        fadeOpacity = 0;
        _material.SetColor("_Color", new Color(0, 0, 0, 1));
    }
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
        _material.SetInt("_Texture", 0);
        fading = true;
        fadeStartTime = Time.time;
        fadeSpeed = time;
        fadeOpacity = 0;
        startOpacity = currentOpacity;

    }

    public float distFromCamera;
    public override void WhileLiving(float val)
    {

        transform.position = transform.parent.TransformPoint(new Vector3(0,0,distFromCamera));
        if (fading == true)
        {

            float v = (Time.time - fadeStartTime) / fadeSpeed;
            if (v >= 1)
            {
                fading = false;
                DoFadeEnd();
            }
            else
            {
                currentOpacity = Mathf.Lerp(fadeOpacity, startOpacity, FadeCurve.Evaluate(v));
                _material.SetFloat("_FadeValue", currentOpacity );
                _material.SetColor("_Color", new Color(fadeColor.r, fadeColor.g, fadeColor.b, currentOpacity));
            }

        }

    }

    public EventTypes.BaseEvent OnFadeEnd;

    public void DoFadeEnd(){
        OnFadeEnd.Invoke();
    }

}