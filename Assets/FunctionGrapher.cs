
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;
using org.mariuszgromada.math.mxparser;

public class FunctionGrapher : MonoBehaviour
{
    [Header("UI References")]
    public TMP_InputField functionInput;
    public Button plotButton;
    public LineRenderer lineRenderer;

    [Header("Function Settings")]
    public float timeStart = 0f;
    public float timeEnd = 10f;
    public int resolution = 100;

    [Header("Graph Display Settings")]
    public float graphWidth = 10f;     // Width of the graph in world units
    public float graphHeight = 5f;     // Height of the graph in world units
    public float yScale = 1f;          // Vertical scale for the function (multiplier)
    public Vector3 graphOrigin = Vector3.zero; // Where the graph starts in world space

    void Start()
    {
        plotButton.onClick.AddListener(PlotFunction);
    }

    void PlotFunction()
    {
        string userExpr = functionInput.text;

        Argument tArg = new Argument("t", 0);
        Expression expr = new Expression(userExpr, tArg);

        if (!expr.checkSyntax())
        {
            Debug.LogError("Invalid syntax: " + expr.getErrorMessage());
            return;
        }

        List<Vector3> points = new List<Vector3>();
        float step = (timeEnd - timeStart) / resolution;

        float xScale = graphWidth / (timeEnd - timeStart);
        float xOffset = -graphWidth / 2f;
        float yOffset = -graphHeight / 2f;

        // Apply flipping
        bool flipX = true;  // <- flip left-to-right
        bool flipY = true;  // <- flip top-to-bottom

        // Move LineRenderer to the graph origin
        lineRenderer.transform.position = graphOrigin;

        for (int i = 0; i <= resolution; i++)
        {
            float t = timeStart + i * step;
            tArg.setArgumentValue(t);
            double y = expr.calculate();

            if (double.IsNaN(y) || double.IsInfinity(y)) y = 0;

            float x = t * xScale + xOffset;
            float yVal = (float)y * yScale + yOffset;

            if (flipX) x = -x;
            if (flipY) yVal = -yVal;

            points.Add(new Vector3(x, yVal, 0));
        }

        lineRenderer.positionCount = points.Count;
        lineRenderer.SetPositions(points.ToArray());
    }
}
