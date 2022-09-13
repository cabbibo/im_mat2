using System;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

[Serializable]
[PostProcess(typeof(FogPostRenderer), PostProcessEvent.AfterStack, "Custom/FogPost")]
public sealed class FogPost : PostProcessEffectSettings
{
    [Range(0f, 1f), Tooltip("FogPost effect intensity.")]
    public FloatParameter blend = new FloatParameter { value = 0.5f };
    public Vector3Parameter fogParams = new Vector3Parameter { value =new Vector3( 0 , 0,1000)  };
    public ColorParameter fogColor = new ColorParameter { value = Color.blue  };
}

public sealed class FogPostRenderer : PostProcessEffectRenderer<FogPost>
{
    public override void Render(PostProcessRenderContext context)
    {

        Debug.Log(settings.fogParams.value.z);
        var sheet = context.propertySheets.Get(Shader.Find("Hidden/Custom/Fog2"));
        sheet.properties.SetFloat("_Blend", settings.blend);
        
        sheet.properties.SetVector("_FogColor", settings.fogColor);
        sheet.properties.SetVector("_FogParams",settings.fogParams);
        context.command.BlitFullscreenTriangle(context.source, context.destination, sheet, 0);
    }


 
}