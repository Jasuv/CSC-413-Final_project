using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

[System.Serializable]
[PostProcess(typeof(WavyEffectRenderer), PostProcessEvent.AfterStack, "Custom/wavy")]
public sealed class WavyEffect : PostProcessEffectSettings
{
    [Range(0f, 10f), Tooltip("Wave Distortion")]
    public FloatParameter distorion = new FloatParameter { value = 0f };

    [Range(0f, 10f), Tooltip("Wave speed")]
    public FloatParameter speed = new FloatParameter { value = 0f };

    [Range(0f, 10f), Tooltip("Wave Frequency")]
    public FloatParameter frequency = new FloatParameter { value = 0f };
}

public sealed class WavyEffectRenderer : PostProcessEffectRenderer<WavyEffect>
{
    private Shader _shader;
    private Material _material;

    public override void Init()
    {
        _shader = Shader.Find("Custom/wavy");

        if (_shader != null)
        {
            _material = new Material(_shader);
        }
    }

    public override void Render(PostProcessRenderContext context)
    {
        if (_material == null) return;

        _material.SetFloat("_Distortion", settings.distorion);
        _material.SetFloat("_Speed", settings.speed);
        _material.SetFloat("_Frequency", settings.frequency);

        var sheet = context.command;
        var source = context.source;
        var destination = context.destination;

        sheet.Blit(source, destination, _material);
    }
}