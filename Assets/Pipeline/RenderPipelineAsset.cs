using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

[CreateAssetMenu(menuName = "Rendering/RenderPipelineAsset")]
public class RenderPipelineAsset : UnityEngine.Rendering.RenderPipelineAsset
{
    public Color exampleColor;
    public string exampleString;
    
    protected override RenderPipeline CreatePipeline()
    {
        return new RenderPipelineInstance(this);
    }
}
