using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using Object = UnityEngine.Object;

public class ModelsManager : MonoBehaviour
{
    public List<Object> modelsToUse;
    public List<GameObject> Models { get; set; } = new List<GameObject>();
    
    private int templateIndex;
    private GameObject template;
    private GameObject parent;

    public void Init()
    {
        template = (GameObject)modelsToUse[0];
        
        parent = new GameObject();
        parent.transform.rotation = Quaternion.Euler(0, 90, 0);
    }

    public void ChangeModel()
    {
        var modelsCount = Models.Count;

        templateIndex++;

        if (templateIndex == modelsToUse.Count)
        {
            templateIndex = 0;
        }

        template = (GameObject)modelsToUse[templateIndex];
        UpdateModelsCount(modelsCount);
    }
    
    public void UpdateModelsCount(int count)
    {
        RemoveAllObjectsFromDisplay();
        
        var lastPosition = 0;
        
        for (int i = 0; i < count; i++)
        {
            var newModel = Instantiate(template, parent.transform);
            newModel.transform.Rotate(new Vector3(0, lastPosition * (float)Math.PI, 0));
            lastPosition++;
            Models.Add(newModel);
        }
    }
    
    public void RemoveAllObjectsFromDisplay()
    {
        for (int i = 0; i < Models.Count; i++)
        {
            Destroy(Models[i]);
        }
        
        Models.Clear();
    }
}