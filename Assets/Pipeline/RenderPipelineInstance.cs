
 using UnityEngine;
 using UnityEngine.Rendering;

 public class RenderPipelineInstance : RenderPipeline
 {
     private RenderPipelineAsset renderPipelineAsset;
     
     public RenderPipelineInstance(RenderPipelineAsset asset) {
         renderPipelineAsset = asset;
     }
     
     protected override void Render(ScriptableRenderContext context, Camera[] cameras)
     {
         Debug.Log(renderPipelineAsset.exampleString);
     }
 }