using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Analyzer : MonoBehaviour
{
    public Chart chart;
    public event Action<int> valueChanged;

    public void StartAnalyzing(List<int> xValues, Func<int, int> xValueGetter,  Func<float> yValueGetter)
    {
        StartCoroutine(DrawNextPoint(xValues,xValueGetter, yValueGetter, 0));
    }
    
    IEnumerator DrawNextPoint(List<int> xValues, Func<int, int> xValueGetter, Func<float> yValueGetter, int index)
    {
        if (index < xValues.Count)
        {
            var xValue = xValues[index];
            valueChanged(xValue);
            
            yield return new WaitForSeconds(1f);
            
            var y = yValueGetter();
            chart.DrawNextPoint(xValueGetter(xValue), y);
        
            StartCoroutine(DrawNextPoint(xValues,xValueGetter, yValueGetter, ++index));
        }
    }
}