using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class Chart : VisualElement
{
    public float XDivisionsCount { get; set; }
    public float YDivisionsCount { get; set; }
    public float XMaxValue { get; set; }
    public float YMaxValue { get; set; }
    public string XName { get; set; }
    public string YName { get; set; }

    public List<Label> XLabels { get; set; }
    public Label XNameLabel { get; set; }
    public List<Label> YLabels { get; set; }
    public Label YNameLabel { get; set; }
    public VisualElement YCoord { get; set; }
    public VisualElement LeftPanel { get; set; }
    public VisualElement RightPanel { get; set; }
    public VisualElement BottomPanel { get; set; }
    public VisualElement XCoord { get; set; }
    public VisualElement Graph { get; set; }
    public float PixelsPerXUnit { get; set; }
    public float PixelsPerYUnit { get; set; }
    private Rect GraphRect { get; set; }

    private List<KeyValuePair<float, float>> Points { get; set; } = new List<KeyValuePair<float, float>>()
    {
        new KeyValuePair<float, float>(8, 5),
        new KeyValuePair<float, float>(16, 15),
        new KeyValuePair<float, float>(24, 20)
    };
    // Factory class, required to expose this custom control to UXML
    public new class UxmlFactory : UxmlFactory<Chart, UxmlTraits> { }

    // Traits class
    public new class UxmlTraits : VisualElement.UxmlTraits
    {
        private UxmlFloatAttributeDescription XDivisionsCount = new UxmlFloatAttributeDescription {name = "XDivisionsCount", defaultValue = 5};
        private UxmlFloatAttributeDescription YDivisionsCount = new UxmlFloatAttributeDescription {name = "YDivisionsCount", defaultValue = 5};
        private UxmlFloatAttributeDescription XMaxValue = new UxmlFloatAttributeDescription {name = "XMaxValue", defaultValue = 5};
        private UxmlFloatAttributeDescription YMaxValue = new UxmlFloatAttributeDescription {name = "YMaxValue", defaultValue = 5};
        private UxmlStringAttributeDescription XName = new UxmlStringAttributeDescription {name = "XName", defaultValue = "x"};
        private UxmlStringAttributeDescription YName = new UxmlStringAttributeDescription {name = "YName", defaultValue = "y"};
 
        public override void Init(VisualElement vr, IUxmlAttributes bag, CreationContext cc)
        {
            base.Init(vr, bag, cc);
            var chart = (Chart) vr;
            chart.YDivisionsCount = YDivisionsCount.GetValueFromBag(bag, cc);
            chart.XDivisionsCount = XDivisionsCount.GetValueFromBag(bag, cc);

            chart.YMaxValue = YMaxValue.GetValueFromBag(bag, cc);
            chart.XMaxValue = XMaxValue.GetValueFromBag(bag, cc);
            
            var xName = XName.GetValueFromBag(bag, cc);
            chart.XName = xName;

            var yName = YName.GetValueFromBag(bag, cc);
            chart.YName = yName;
            
            //chart.ClearChart();
            chart.FillCoordValues(chart.YCoord, chart.YDivisionsCount, chart.YMaxValue, yName, true);
            chart.FillCoordValues(chart.XCoord, chart.XDivisionsCount, chart.XMaxValue, xName);
        }
    }

    private void CalculateScales()
    {
        var xUnit = XMaxValue / XDivisionsCount;
        var xUnitLenght = XLabels[0].contentRect.width;
        PixelsPerXUnit = xUnitLenght / xUnit;
        
        var yUnit = YMaxValue / YDivisionsCount;
        var yUnitLenght = YLabels[0].contentRect.height;
        PixelsPerYUnit = yUnitLenght / yUnit;
    }

    public Chart()
    {
        BuildHierarchy();
       
        //FillValues(xValues, XDivisionsCount, XMaxValue);
        generateVisualContent += OnGenerateVisualContent;
        RegisterCallback<CustomStyleResolvedEvent>(OnStylesResolved);
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

        //bottomPanel.style.flexGrow = new StyleFloat(1);

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

    public void DrawNextPoint(float x, float y)
    {
        Points.Add(new KeyValuePair<float, float>(x, y));
        MarkDirtyRepaint();
    }
    
    public void FillCoordValues(VisualElement valuesContainer, float divisionsCount, float maxValue, string coordName, bool isYCoordinate = false)
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
        var labelStepInPercent = 100 / (divisionsCount * 2 + 1);
        
        if (isYCoordinate)
        {
            AddPlug(valuesContainer, labelStepInPercent, true);
        }
        else
        {
            AddPlug(valuesContainer, labelStepInPercent);
        }
        
        for (int i = 0; i < divisionsCount + 0; i++)
        {
            var newLabel = new Label((step * (i + 1)).ToString());
            newLabel.style.flexGrow = new StyleFloat(1);
            newLabel.style.unityTextAlign = new StyleEnum<TextAnchor>(TextAnchor.MiddleCenter);

            if (isYCoordinate)
            {
                newLabel.style.height = new StyleLength(new Length(labelStepInPercent * 2, LengthUnit.Percent));
                YLabels.Add(newLabel);
            }
            else
            {
                newLabel.style.width = new StyleLength(new Length(labelStepInPercent * 2, LengthUnit.Percent));
                XLabels.Add(newLabel);
            }
            
            valuesContainer.Add(newLabel);
        }

        var nameLabel = new Label(coordName);

        if (isYCoordinate)
        {
            YNameLabel.text = coordName;
        }
        else
        {
            XNameLabel.text = coordName;
        }
    }

    private void AddPlug(VisualElement valuesContainer, float stepInPrecent, bool isVertical = false)
    {
        var plug = new Label();
        plug.style.flexGrow = new StyleFloat(1);

        if (isVertical)
        {
            plug.style.height = new StyleLength(new Length(stepInPrecent, LengthUnit.Percent));
        }
        else
        {            
            plug.style.width = new StyleLength(new Length(stepInPrecent, LengthUnit.Percent));
        }
        
        valuesContainer.Add(plug);
    }

    // When custom styles are known for this control, make a gradient from the colors.
    void OnStylesResolved(CustomStyleResolvedEvent evt)
    {
        var j = 1;
    }

    void OnGenerateVisualContent(MeshGenerationContext mgc)
    {
        var zeroPoint = new Vector2(RightPanel.contentContainer.localBound.x, Graph.contentContainer.localBound.height);
        var diagonal = new Vector2(Graph.contentRect.width, Graph.contentContainer.localBound.height);
        GraphRect = new Rect(zeroPoint,diagonal);
        
        FillBackground(0, 0, contentRect.width, contentRect.height, mgc);
        var painter2D = mgc.painter2D;
        
        MakeDirectionLines(GraphRect, painter2D);
        DrawGrid(GraphRect, painter2D);
        CalculateScales();
        DrawPoints(GraphRect, painter2D);
    }

    private void DrawPoints(Rect graphRect, Painter2D painter2D)
    {
        painter2D.lineJoin = LineJoin.Round;
        painter2D.lineWidth = 2f;
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

        return new Vector2(graphOrigin.x + xCord, graphOrigin.y - yCord);
    }

    private void DrawGrid(Rect graphRect, Painter2D painter2D)
    {
        painter2D.lineWidth = 1.0f;

        foreach (var label in XLabels)
        {
            var currentX = graphRect.x + label.localBound.center.x;
            DrawLine( new Vector2(currentX, graphRect.y), 
                new Vector2(currentX, graphRect.y - graphRect.height + painter2D.lineWidth), painter2D);
        }
        
        foreach (var label in YLabels)
        {
            var currentY = graphRect.y - YCoord.localBound.height + label.localBound.center.y;
            DrawLine( new Vector2(graphRect.x, currentY), 
                new Vector2(graphRect.x + graphRect.width - painter2D.lineWidth, currentY), painter2D);
        }
    }

    private void MakeDirectionLines(Rect graphRect, Painter2D painter2D)
    {
        painter2D.lineWidth = 4.0f;
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