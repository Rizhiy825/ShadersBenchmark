using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Chart : VisualElement
{
    public int XDivisionsCount { get; set; }
    public int YDivisionsCount { get; set; }
    public float XMaxValue { get; set; }
    public float YMaxValue { get; set; }
    public string XName { get; set; }
    public string YName { get; set; }

    public List<Label> XLabels { get; set; }
    public Label XZeroLabel { get; set; }
    public Label XNameLabel { get; set; }
    public List<Label> YLabels { get; set; }
    public Label YZeroLabel { get; set; }
    public Label YNameLabel { get; set; }
    public VisualElement YCoord { get; set; }
    public VisualElement LeftPanel { get; set; }
    public VisualElement RightPanel { get; set; }
    public VisualElement BottomPanel { get; set; }
    public VisualElement XCoord { get; set; }
    public VisualElement Graph { get; set; }
    public float PixelsPerXUnit { get; set; }
    public float PixelsPerYUnit { get; set; }
    private float XLabelsStep { get; set; }
    private float YLabelsStep { get; set; }
    private Rect GraphRect { get; set; }
    private List<KeyValuePair<float, float>> Points { get; set; } = new ()
    {
        // For chart calibrating
        /*new KeyValuePair<float, float>(1500, 6),
        new KeyValuePair<float, float>(3000, 18),
        new KeyValuePair<float, float>(12000, 60)*/
    };
    
    // Factory class, required to expose this custom control to UXML
    public new class UxmlFactory : UxmlFactory<Chart, UxmlTraits> { }

    // Traits class
    public new class UxmlTraits : VisualElement.UxmlTraits
    {
        private readonly UxmlIntAttributeDescription _xDivisionsCount = new() {name = "XDivisionsCount", defaultValue = 5};
        private readonly UxmlIntAttributeDescription _yDivisionsCount = new() {name = "YDivisionsCount", defaultValue = 5};
        private readonly UxmlFloatAttributeDescription _xMaxValue = new() {name = "XMaxValue", defaultValue = 5};
        private readonly UxmlFloatAttributeDescription _yMaxValue = new() {name = "YMaxValue", defaultValue = 5};
        private readonly UxmlStringAttributeDescription _xName = new() {name = "XName", defaultValue = "x"};
        private readonly UxmlStringAttributeDescription _yName = new() {name = "YName", defaultValue = "y"};
 
        public override void Init(VisualElement vr, IUxmlAttributes bag, CreationContext cc)
        {
            base.Init(vr, bag, cc);
            var chart = (Chart) vr;
            chart.YDivisionsCount = _yDivisionsCount.GetValueFromBag(bag, cc);
            chart.XDivisionsCount = _xDivisionsCount.GetValueFromBag(bag, cc);

            chart.YMaxValue = _yMaxValue.GetValueFromBag(bag, cc);
            chart.XMaxValue = _xMaxValue.GetValueFromBag(bag, cc);
            
            var xName = _xName.GetValueFromBag(bag, cc);
            chart.XName = xName;

            var yName = _yName.GetValueFromBag(bag, cc);
            chart.YName = yName;
            
            chart.FillCoordValues(chart.YCoord, chart.YDivisionsCount, chart.YMaxValue, yName, true);
            chart.FillCoordValues(chart.XCoord, chart.XDivisionsCount, chart.XMaxValue, xName);
        }
    }
    
    public Chart()
    {
        BuildHierarchy();
        generateVisualContent += OnGenerateVisualContent;
    }

    public void ClearChart()
    {
        Points = new List<KeyValuePair<float, float>>();
    }

    public void DrawNextPoint(float x, float y)
    {
        Points.Add(new KeyValuePair<float, float>(x, y));
        MarkDirtyRepaint();
    }
    
    private void BuildHierarchy()
    {
        LeftPanel = new VisualElement();
        YCoord = new VisualElement();
        RightPanel = new VisualElement();
        BottomPanel = new VisualElement();
        XCoord = new VisualElement();
        Graph = new VisualElement();
        
        Add(LeftPanel);
        Add(RightPanel);

        var zeroLabel = new Label("0");
        zeroLabel.style.unityTextAlign = new StyleEnum<TextAnchor>(TextAnchor.UpperRight);
        
        YNameLabel = new Label(YName);
        LeftPanel.Add(zeroLabel);
        LeftPanel.Add(YCoord);
        LeftPanel.Add(YNameLabel);
        
        RightPanel.Add(Graph);
        RightPanel.Add(BottomPanel);
        
        XNameLabel = new Label(XName);
        BottomPanel.Add(XCoord);
        BottomPanel.Add(XNameLabel);

        style.flexDirection = new StyleEnum<FlexDirection>(FlexDirection.Row);

        LeftPanel.style.flexDirection = new StyleEnum<FlexDirection>(FlexDirection.ColumnReverse);
        
        RightPanel.style.flexGrow = new StyleFloat(1);
        BottomPanel.style.flexDirection = new StyleEnum<FlexDirection>(FlexDirection.Row);
        
        YCoord.style.flexDirection = new StyleEnum<FlexDirection>(FlexDirection.ColumnReverse);
        YCoord.style.alignItems = new StyleEnum<Align>(Align.FlexEnd);
        YCoord.style.marginRight = new StyleLength(5);
        YCoord.style.flexGrow = new StyleFloat(1f);
        
        XCoord.style.flexDirection = new StyleEnum<FlexDirection>(FlexDirection.Row);
        XCoord.style.justifyContent = new StyleEnum<Justify>(Justify.SpaceBetween);
        XCoord.style.flexGrow = new StyleFloat(1f);
        
        Graph.style.flexGrow = new StyleFloat(1);
    }
    private void FillCoordValues(VisualElement valuesContainer, float divisionsCount, float maxValue, string coordName, bool isYCoordinate = false)
    {
        valuesContainer.Clear();

        if (isYCoordinate)
        {
            YLabels = new List<Label>();
        }
        else
        {
            XLabels = new List<Label>();
        }
        
        var step = maxValue / divisionsCount;
        
        if (isYCoordinate)
        {
            AddPlug(valuesContainer, true);
        }
        else
        {
            AddPlug(valuesContainer);
        }
        
        for (int i = 0; i < divisionsCount; i++)
        {
            var newLabel = new Label((step * (i + 1)).ToString());
            newLabel.style.unityTextAlign = new StyleEnum<TextAnchor>(TextAnchor.MiddleCenter);

            if (isYCoordinate)
            {
                YLabels.Add(newLabel);
            }
            else
            {
                XLabels.Add(newLabel);
            }
            
            valuesContainer.Add(newLabel);
        }

        if (isYCoordinate)
        {
            YNameLabel.text = coordName;
        }
        else
        {
            XNameLabel.text = coordName;
        }
    }

    private void UpdateLabelsSizes()
    {
        XLabelsStep = XCoord.localBound.width / (XDivisionsCount * 2 + 1) * 2;
        YLabelsStep = YCoord.localBound.height / (YDivisionsCount * 2 + 1) * 2;
        SetNewSizeForLabels(XZeroLabel, XLabels, XLabelsStep, false);
        SetNewSizeForLabels(YZeroLabel, YLabels, YLabelsStep, true);
    }

    private void SetNewSizeForLabels(Label zeroLabel, List<Label> labels, float newSize, bool isVertical)
    {
        if (isVertical)
        {
            zeroLabel.style.height = newSize / 2;
        }
        else
        {
            zeroLabel.style.width = newSize / 2;
        }
 
        foreach (var label in labels)
        {
            if (isVertical)
            {
                label.style.height = newSize;
            }
            else
            {
                label.style.width = newSize;
            }
        }
    }
    
    private void AddPlug(VisualElement valuesContainer, bool isVertical = false)
    {
        var plug = new Label();

        if (isVertical)
        {
            YZeroLabel = plug;
        }
        else
        {
            XZeroLabel = plug;
        }
        
        valuesContainer.Add(plug);
    }
    
    void OnGenerateVisualContent(MeshGenerationContext mgc)
    {
        var zeroPoint = new Vector2(RightPanel.localBound.x, Graph.contentRect.height);
        var diagonal = new Vector2(Graph.contentRect.width, Graph.contentContainer.localBound.height);
        GraphRect = new Rect(zeroPoint,diagonal);
        
        FillBackground(0, 0, contentRect.width, contentRect.height, mgc);
        var painter2D = mgc.painter2D;
        
        UpdateLabelsSizes();
        MakeDirectionLines(GraphRect, painter2D);
        DrawGrid(GraphRect, painter2D);
        CalculateScales();
        DrawPoints(GraphRect, painter2D);
    }

    private void DrawPoints(Rect graphRect, Painter2D painter2D)
    {
        painter2D.lineJoin = LineJoin.Round;
        painter2D.lineWidth = 1f;
        painter2D.fillColor = Color.yellow;

        painter2D.BeginPath();
        painter2D.MoveTo(new Vector2(graphRect.x, graphRect.y));

        foreach (var point in Points)
        {
            var pointInPixels = ConvertValueToPixelPosition(graphRect.position, point);
            painter2D.LineTo(pointInPixels);
        }
            
        painter2D.Stroke();
    }
    
    private Vector2 ConvertValueToPixelPosition(Vector2 graphOrigin, KeyValuePair<float, float> point)
    {
        var xCord = point.Key * PixelsPerXUnit;
        var yCord = point.Value * PixelsPerYUnit;

        return new Vector2(graphOrigin.x + (float)xCord, graphOrigin.y - (float)yCord);
    }

    private void DrawGrid(Rect graphRect, Painter2D painter2D)
    {
        painter2D.lineWidth = 1f;
        var currentPosition = XLabelsStep;
        
        foreach (var label in XLabels)
        {
            var currentX = graphRect.x + currentPosition;
            DrawLine( new Vector2(currentX, graphRect.y), 
                new Vector2(currentX, graphRect.y - graphRect.height + painter2D.lineWidth), painter2D);
            currentPosition += XLabelsStep;
        }

        currentPosition = YLabelsStep;
        
        foreach (var label in YLabels)
        {
            var currentY = graphRect.y - currentPosition;
            DrawLine( new Vector2(graphRect.x, currentY), 
                new Vector2(graphRect.x + graphRect.width - painter2D.lineWidth, currentY), painter2D);
            currentPosition += YLabelsStep;
        }
    }
    
    private void CalculateScales()
    {
        var xUnit = XMaxValue / XDivisionsCount;
        PixelsPerXUnit = XLabelsStep / xUnit;
        
        var yUnit = YMaxValue / YDivisionsCount;
        PixelsPerYUnit = YLabelsStep / yUnit;
    }


    private void MakeDirectionLines(Rect graphRect, Painter2D painter2D)
    {
        painter2D.lineWidth = 3.0f;
        painter2D.strokeColor = Color.black;
        painter2D.lineJoin = LineJoin.Round;
        painter2D.lineCap = LineCap.Round;
        
        // Horizontal
        DrawLine(new Vector2(graphRect.x, graphRect.y), new Vector2(graphRect.x + graphRect.width - painter2D.lineWidth, graphRect.y), painter2D);
        // Vertical
        DrawLine(new Vector2(graphRect.x, graphRect.y), new Vector2(graphRect.x, graphRect.y - graphRect.height + painter2D.lineWidth), painter2D);
    }

    private void DrawLine(Vector2 from, Vector2 to, Painter2D painter2D)
    {
        painter2D.BeginPath();
        painter2D.MoveTo(from);
        painter2D.LineTo(to);
        painter2D.Stroke();
    }
    
    void FillBackground(float left,
        float top,
        float right,
        float bottom,
        MeshGenerationContext mgc)
    {
        Vertex[] KVertices = new Vertex[4];
        ushort[] KIndices = {0, 1, 2, 2, 3, 0};

        KVertices[0].position = new Vector3(left, bottom, Vertex.nearZ);
        KVertices[1].position = new Vector3(left, top, Vertex.nearZ);
        KVertices[2].position = new Vector3(right, top, Vertex.nearZ);
        KVertices[3].position = new Vector3(right, bottom, Vertex.nearZ);

        KVertices[0].tint = new Color32(255, 255, 255, 255);
        KVertices[1].tint = new Color32(255, 255, 255, 255);
        KVertices[2].tint = new Color32(255, 255, 255, 255);
        KVertices[3].tint = new Color32(255, 255, 255, 255);

        MeshWriteData mwd = mgc.Allocate(KVertices.Length, KIndices.Length, new Texture2D(2, 2));

        // Since the texture may be stored in an atlas, the UV coordinates need to be
        // adjusted. Simply rescale them in the provided uvRegion.
        Rect uvRegion = mwd.uvRegion;
        KVertices[0].uv = new Vector2(0, 0) * uvRegion.size + uvRegion.min;
        KVertices[1].uv = new Vector2(0, 1) * uvRegion.size + uvRegion.min;
        KVertices[2].uv = new Vector2(1, 1) * uvRegion.size + uvRegion.min;
        KVertices[3].uv = new Vector2(1, 0) * uvRegion.size + uvRegion.min;

        mwd.SetAllVertices(KVertices);
        mwd.SetAllIndices(KIndices);
    }
}