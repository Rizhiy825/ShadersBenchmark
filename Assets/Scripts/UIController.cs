using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class UIController : MonoBehaviour
{
    private UIDocument root;
    private VisualElement rootVisualElement;
    private ProfilerController fpsWindow;
    
    public event Action changeShader = () => { };
    public event Action<int> changeModelsCount = (x) => { };
    
    // Start is called before the first frame update
    public void Init()
    {
        root = gameObject.GetComponent<UIDocument>();
        rootVisualElement = root.rootVisualElement;
        
        InitFPSWindow();
        
        var changeShaderButton = rootVisualElement.Q<Button>("ChangeShader");
        var changeCountButton =  rootVisualElement.Q<Button>("Render");
        var enterField = rootVisualElement.Q<TextField>("Count");

        changeShaderButton.clicked += () =>
        {
            changeShader();
            fpsWindow.ResetFPS();
        };
        
        changeCountButton.clicked += () =>
        {
            changeModelsCount(Convert.ToInt32(enterField.text));
            fpsWindow.ResetFPS();
        };
    }
    
    public void SetShaderName(string name)
    {
        var shaderName = rootVisualElement.Q<Label>("ShaderName");
        shaderName.text = "Current shader: " + name;
    }

    private void InitFPSWindow()
    {
        fpsWindow = gameObject.AddComponent<ProfilerController>();
        fpsWindow.ProfileData =  rootVisualElement.Q<Label>("ProfileData");
    }
}

