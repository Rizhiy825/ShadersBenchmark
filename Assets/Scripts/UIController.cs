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

    private TextField enterField;
    
    public event Action changeShader = () => { };
    public event Action<int> setModelsCount = (x) => { };
    public event Action<int> changeModelsCount = (x) => { };
    
    // Start is called before the first frame update
    public void Init()
    {
        root = gameObject.GetComponent<UIDocument>();
        rootVisualElement = root.rootVisualElement;
        
        InitFPSWindow();
        
        enterField = rootVisualElement.Q<TextField>("Count");
        var changeShaderButton = rootVisualElement.Q<Button>("ChangeShader");
        var changeCountButton =  rootVisualElement.Q<Button>("Render");
        var plusTen =  rootVisualElement.Q<Button>("Plus10");
        var plusHoundred =  rootVisualElement.Q<Button>("Plus100");
        var minusTen =  rootVisualElement.Q<Button>("Minus10");
        var minusHoundred =  rootVisualElement.Q<Button>("Minus100");

        changeShaderButton.clicked += () =>
        {
            changeShader();
            fpsWindow.ResetFPS();
        };
        
        changeCountButton.clicked += () =>
        {
            setModelsCount(Convert.ToInt32(enterField.text));
            fpsWindow.ResetFPS();
        };

        AddChangeCountClickCallback(plusTen, 10);
        AddChangeCountClickCallback(plusHoundred, 100);
        AddChangeCountClickCallback(minusTen, -10);
        AddChangeCountClickCallback(minusHoundred, -100);
    }

    private void AddChangeCountClickCallback(Button button, int changeCount)
    {
        button.clicked += () =>
        {
            enterField.value = (Convert.ToInt32(enterField.text) + changeCount).ToString();
            changeModelsCount(changeCount);
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

