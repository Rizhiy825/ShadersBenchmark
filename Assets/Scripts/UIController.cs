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
    private DropdownField dropField;
    
    public event Action changeShader = () => { };
    public event Action changeModel = () => { };
    public event Action<int> setModelsCount = (x) => { };
    public event Action<int> changeModelsCount = (x) => { };
    public event Action<int> changeResolution = (x) => { };
    
    public Chart ModelsCountChart { get; private set; }
    public Chart ResolutionChart { get; private set; }

    // Start is called before the first frame update
    public void Init()
    {
        root = gameObject.GetComponent<UIDocument>();
        rootVisualElement = root.rootVisualElement;
        
        InitFPSWindow();
        
        enterField = rootVisualElement.Q<TextField>("Count");
        ModelsCountChart = rootVisualElement.Q<Chart>("ModelsCountChart");
        ResolutionChart = rootVisualElement.Q<Chart>("ResolutionChart");
        var changeShaderButton = rootVisualElement.Q<Button>("ChangeShader");
        var changeModelButton = rootVisualElement.Q<Button>("ChangeModel");
        var changeCountButton =  rootVisualElement.Q<Button>("Render");
        var plusTen =  rootVisualElement.Q<Button>("Plus10");
        var plusHundred =  rootVisualElement.Q<Button>("Plus100");
        var minusTen =  rootVisualElement.Q<Button>("Minus10");
        var minusHundred =  rootVisualElement.Q<Button>("Minus100");
        
        ConfigureDropDownField(); 
        
        changeShaderButton.clicked += () =>
        {
            changeShader();
        };

        changeModelButton.clicked += () =>
        {
            changeModel();
        };

        changeCountButton.clicked += () =>
        {
            var newCount = Convert.ToInt32(enterField.text);
            StartCoroutine(DrawNextPoint(
                newCount, 
                ModelsCountChart, 
                () => fpsWindow.CurrentFrameTimeInMs));
            setModelsCount(newCount);
        };

        AddChangeCountClickCallback(plusTen, 10);
        AddChangeCountClickCallback(plusHundred, 100);
        AddChangeCountClickCallback(minusTen, -10);
        AddChangeCountClickCallback(minusHundred, -100);

        InitAnalyzers();
    }

    private void InitAnalyzers()
    {
        var countAnalyzer = gameObject.AddComponent<Analyzer>();
        countAnalyzer.chart = ModelsCountChart;
        countAnalyzer.valueChanged += newModelsCount =>
        {
            enterField.value = newModelsCount.ToString();
            setModelsCount(newModelsCount);
        };

        var countAnalysisButton = rootVisualElement.Q<Button>("CountAnalyzer");

        countAnalysisButton.clicked += () =>
            countAnalyzer.StartAnalyzing(
                //new List<int>() {0, 1500, 3000, 4500, 6000, 7500, 9000, 10500, 12000, 13500},
                new List<int>() {750, 1500, 2250, 3000, 3750, 4500, 5250, 6000, 6750, 7500},
                i => i, 
                () => fpsWindow.CurrentFrameTimeInMs);
        
        var resolutionAnalyzer = gameObject.AddComponent<Analyzer>();
        resolutionAnalyzer.chart = ResolutionChart;
        resolutionAnalyzer.valueChanged += newMultiplier =>
        {
            dropField.index = newMultiplier;
            changeResolution(newMultiplier);
        };
        
        var resolutionAnalysisButton = rootVisualElement.Q<Button>("ResolutionAnalyzer");
        
        resolutionAnalysisButton.clicked += () =>
            resolutionAnalyzer.StartAnalyzing(
                //new List<int>() {0, 1, 2, 3, 4, 5},
                new List<int>() {0, 1, 2, 3, 4, 5},
                i => (int)Math.Pow(2, i), 
                () => fpsWindow.CurrentFrameTimeInMs);
    }

    private void ConfigureDropDownField()
    {
        dropField = rootVisualElement.Q<DropdownField>("DropdownField");
        var resolutions = ResolutionController.ResolutionMultiplier;
        
        dropField.choices = new List<string>()
        {
            $"0.125x - {resolutions[0].Item1}x{resolutions[0].Item2} (259 200 pixels)",
            $"0.25x - {resolutions[1].Item1}x{resolutions[1].Item2} (518 400 pixels)",
            $"0.5x - {resolutions[2].Item1}x{resolutions[2].Item2} (1 036 800 pixels)",
            $"1x - {resolutions[3].Item1}x{resolutions[3].Item2} (2 073 600 pixels)",
            $"2x - {resolutions[4].Item1}x{resolutions[4].Item2} (4 147 200 pixels)",
            $"4x - {resolutions[5].Item1}x{resolutions[5].Item2} (8 294 400 pixels)"
        };

        dropField.index = ResolutionController.DefaultResolutionIndex;

        dropField.RegisterValueChangedCallback(ChangeResolutionEvent);
    }

    private void ChangeResolutionEvent(ChangeEvent<string> evt)
    {
        changeResolution(dropField.index);
    }

    private void AddChangeCountClickCallback(Button button, int changeCount)
    {
        button.clicked += () =>
        {
            var newCount = Convert.ToInt32(enterField.text) + changeCount;
            enterField.value = newCount.ToString();
            changeModelsCount(changeCount);
            StartCoroutine(DrawNextPoint(
                newCount, 
                ModelsCountChart, 
                () => fpsWindow.CurrentFrameTimeInMs));
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

    IEnumerator DrawNextPoint(float value, Chart chart, Func<float> valueGetter)
    {
        yield return new WaitForSeconds(1f);
        var y = valueGetter();
        chart.DrawNextPoint(value, y);
    }
}