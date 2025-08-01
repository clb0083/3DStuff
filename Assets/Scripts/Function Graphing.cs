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
    public FunctionInputHandler functionHandler;
    public float tStart = 0f;
    public int resolution = 100;

    private void Start()
    {
        generateButton.onClick.AddListener(GenerateFunctionCurve);
    }

    void GenerateFunctionCurve()
    {
        functionHandler.RefreshFunction(); // Ensure it's using the latest function + time length

        float usedtEnd = Mathf.Max(tStart + 0.01f, functionHandler.timeLength);
        List<Vector2> points = new List<Vector2>();

        for (int i = 0; i < resolution; i++)
        {
            float t = Mathf.Lerp(tStart, usedtEnd, i / (float)(resolution - 1));
            float y = functionHandler.GetValueAtTime(t);
            points.Add(new Vector2(t, y));
        }

        lineRenderer.SetPoints(points);
    }
}


