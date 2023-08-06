
using System;
using System.Collections.Generic;
using UnityEngine;

public class MaterialsSetter : MonoBehaviour
{

    public List<Material> materials;
    public string CurrentMaterialName => materials[CurrentMaterialIndex].name;
    
    private Material defaultMaterial;
    private int CurrentMaterialIndex { get; set; }
    
    public void Init()
    {
        defaultMaterial = materials[0];
    }

    public void SetDefaultMaterial(List<GameObject> models)
    {
        SetMaterial(models, defaultMaterial);
        CurrentMaterialIndex = 0;
    }

    public void ChangeMaterial(List<GameObject> models)
    {
        CurrentMaterialIndex++;
        
        if (CurrentMaterialIndex == materials.Count)
        {
            CurrentMaterialIndex = 0;
        }
        
        var newMaterial = materials[CurrentMaterialIndex];
        SetMaterial(models, newMaterial);
    }

    public void SetCurrentMaterial(List<GameObject> models)
    {
        var currentMaterial = materials[CurrentMaterialIndex];
        SetMaterial(models, currentMaterial);
    }
    
    private void SetMaterial(List<GameObject> models, Material material)
    {
        foreach (var model in models)
        {
            var mesh = model.GetComponent<MeshRenderer>();
            mesh.material = material;
        }
    }
}