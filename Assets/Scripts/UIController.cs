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
    public event Action changeModel = () => { };
    public event Action<int> setModelsCount = (x) => { };
    public event Action<int> changeModelsCount = (x) => { };
    public event Action<int> changeResolution = (x) => { };
    
    // Start is called before the first frame update
    public void Init()
    {
        root = gameObject.GetComponent<UIDocument>();
        rootVisualElement = root.rootVisualElement;
        
        InitFPSWindow();
        
        enterField = rootVisualElement.Q<TextField>("Count");
        var changeShaderButton = rootVisualElement.Q<Button>("ChangeShader");
        var changeModelButton = rootVisualElement.Q<Button>("ChangeModel");
        var changeCountButton =  rootVisualElement.Q<Button>("Render");
        var plusTen =  rootVisualElement.Q<Button>("Plus10");
        var plusHoundred =  rootVisualElement.Q<Button>("Plus100");
        var minusTen =  rootVisualElement.Q<Button>("Minus10");
        var minusHoundred =  rootVisualElement.Q<Button>("Minus100");

        ConfigureDropDownField(); 
        
        changeShaderButton.clicked += () =>
        {
            changeShader();
            fpsWindow.ResetFPS();
        };

        changeModelButton.clicked += () =>
        {
            changeModel();
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

    private void ConfigureDropDownField()
    {
        var dropField = rootVisualElement.Q<DropdownField>("DropdownField");
        var resolutions = ResolutionController.ResolutionMultiplier;
        
        dropField.choices = new List<string>()
        {
            $"1x - {resolutions[0].Item1}x{resolutions[0].Item2} (259 200 pixels)",
            $"2x - {resolutions[1].Item1}x{resolutions[1].Item2} (518 400 pixels)",
            $"4x - {resolutions[2].Item1}x{resolutions[2].Item2} (1 036 800 pixels)",
            $"8x - {resolutions[3].Item1}x{resolutions[3].Item2} (2 073 600 pixels)",
            $"16x - {resolutions[4].Item1}x{resolutions[4].Item2} (4 147 200 pixels)",
            $"32x - {resolutions[5].Item1}x{resolutions[5].Item2} (8 294 400 pixels)"
        };

        dropField.index = ResolutionController.DefaultResolutionIndex;

        dropField.RegisterValueChangedCallback(ChangeResolutionEvent);
    }

    private void ChangeResolutionEvent(ChangeEvent<string> evt)
    {
        var dropField = rootVisualElement.Q<DropdownField>("DropdownField");
        changeResolution(dropField.index);
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