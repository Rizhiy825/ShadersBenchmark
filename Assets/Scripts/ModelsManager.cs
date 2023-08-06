using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

public class ModelsManager : MonoBehaviour
{
    public Object modelToUse;
    public List<GameObject> models { get; set; } = new List<GameObject>();
    private GameObject template;
    private GameObject parent;

    public void Init()
    {
        template = (GameObject) modelToUse;
        
        parent = new GameObject();
        parent.transform.rotation = Quaternion.Euler(0, 90, 0);
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
            models.Add(newModel);
        }
    }
    
    public void RemoveAllObjectsFromDisplay()
    {
        for (int i = 0; i < models.Count; i++)
        {
            Destroy(models[i]);
        }
        
        models.Clear();
    }
}