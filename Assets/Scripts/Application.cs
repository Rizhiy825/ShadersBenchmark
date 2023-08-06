using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

public class Application : MonoBehaviour
{
    private UIController uiController;
    private ModelsManager manager;
    private MaterialsSetter materialsSetter;
    
    void Start()
    {
        manager = gameObject.GetComponent<ModelsManager>();
        materialsSetter = gameObject.GetComponent<MaterialsSetter>();
        uiController = gameObject.GetComponent<UIController>();
        
        manager.Init();
        materialsSetter.Init();
        uiController.Init();
        
        manager.UpdateModelsCount(1);
        materialsSetter.SetDefaultMaterial(manager.models);

        uiController.changeShader += () =>
        {
            manager.UpdateModelsCount(manager.models.Count);
            materialsSetter.ChangeMaterial(manager.models);
            uiController.SetShaderName(materialsSetter.CurrentMaterialName);
        };

        uiController.changeModelsCount += (newCount) =>
        {
            manager.UpdateModelsCount(newCount);
            materialsSetter.SetCurrentMaterial(manager.models);
        };
    }
}
