using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class FunctionGraphManager : MonoBehaviour
{
    public TMP_InputField functionInput;
    public Button generateButton;
    public UILineRenderer lineRenderer;
    public TMP_InputField tEnd;
    public float tStart = 0f;
    public int resolution = 100;

    private void Start()
    {
        generateButton.onClick.AddListener(GenerateFunctionCurve);
    }

    void GenerateFunctionCurve()
    {
        string function = functionInput.text;
        float.TryParse(tEnd.text, out float parsedtEnd);
        float usedtEnd = parsedtEnd > tStart ? parsedtEnd : tStart;


        List<Vector2> points = WhiskerAcceleration.GetFunctionValues(function, tStart, usedtEnd, resolution);
        lineRenderer.SetPoints(points);
    }
}

