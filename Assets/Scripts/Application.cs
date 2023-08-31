using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

public class Application : MonoBehaviour
{
    private Camera camera;
    private UIController uiController;
    private ModelsManager manager;
    private MaterialsSetter materialsSetter;
    private ResolutionController resolutionController;
    
    void Start()
    {
        camera = GetComponent<Camera>();
        manager = gameObject.GetComponent<ModelsManager>();
        materialsSetter = gameObject.GetComponent<MaterialsSetter>();
        uiController = gameObject.GetComponent<UIController>();
        resolutionController = gameObject.GetComponent<ResolutionController>();
        
        manager.Init();
        materialsSetter.Init();
        uiController.Init();
        
        manager.UpdateModelsCount(1);
        materialsSetter.SetDefaultMaterial(manager.Models);
        uiController.SetShaderName(materialsSetter.CurrentMaterialName);

        RegisterCallBacks();
    }

    private void RegisterCallBacks()
    {
        uiController.changeShader += () =>
        {
            manager.UpdateModelsCount(manager.Models.Count);
            materialsSetter.ChangeMaterial(manager.Models);
            uiController.SetShaderName(materialsSetter.CurrentMaterialName);
        };

        uiController.changeModel += () =>
        {
            manager.ChangeModel();
            materialsSetter.SetCurrentMaterial(manager.Models);
        };

        uiController.setModelsCount += (newCount) =>
        {
            manager.UpdateModelsCount(newCount);
            materialsSetter.SetCurrentMaterial(manager.Models);
        };

        uiController.changeModelsCount += (changeCount) =>
        {
            var currentCount = manager.Models.Count;
            currentCount += changeCount;
            manager.UpdateModelsCount(currentCount);
            materialsSetter.SetCurrentMaterial(manager.Models);
        };

        uiController.changeResolution += (newResolutionIndex) =>
        {
            resolutionController.UpdateResolution(newResolutionIndex);
        };
    }
}
