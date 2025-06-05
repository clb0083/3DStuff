
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using org.mariuszgromada.math.mxparser;
using System.Collections.Generic;

[RequireComponent(typeof(CanvasRenderer))]
public class UIFunctionGrapher : Graphic
{
    public TMP_InputField inputField;
    public RectTransform graphArea;
    public float timeStart = 0;
    public float timeEnd = 10;
    public int resolution = 200;
    public float yScale = 50;
    public bool flipY = true;
    public int gridLineCount = 4;
    public Color gridLineColor = new Color(1f, 1f, 1f, 0.2f); // light grid lines

    private List<Vector2> points = new List<Vector2>();

    protected override void OnPopulateMesh(VertexHelper vh)
    {
        vh.Clear();

        RectTransform rt = GetComponent<RectTransform>();
        float width = rt.rect.width;
        float height = rt.rect.height;

        // Draw gridlines (horizontal only for now)
        float gridSpacing = height / (gridLineCount + 1);
        for (int i = 1; i <= gridLineCount; i++)
        {
            float y = i * gridSpacing;
            AddLine(vh, new Vector2(0, y), new Vector2(width, y), 1f, gridLineColor);
        }

        if (points.Count < 2) return;

        for (int i = 0; i < points.Count - 1; i++)
        {
            AddLine(vh, points[i], points[i + 1], 2f, color);
        }
    }

    private void AddLine(VertexHelper vh, Vector2 start, Vector2 end, float thickness, Color col)
    {
        Vector2 dir = (end - start).normalized;
        Vector2 normal = new Vector2(-dir.y, dir.x) * thickness * 0.5f;

        UIVertex[] quad = new UIVertex[4];
        quad[0].position = start - normal;
        quad[1].position = start + normal;
        quad[2].position = end + normal;
        quad[3].position = end - normal;

        for (int j = 0; j < 4; j++)
        {
            quad[j].color = col;
        }

        vh.AddUIVertexQuad(quad);
    }

    public void Plot()
    {
        points.Clear();

        if (inputField == null) return;
        if (string.IsNullOrWhiteSpace(inputField.text))
        {
            Debug.LogError("Function input is empty!");
            return;
        }

        string exprText = inputField.text;
        Argument tArg = new Argument("t", 0);
        Expression expr = new Expression(exprText, tArg);

        if (!expr.checkSyntax())
        {
            Debug.LogError("Invalid syntax: " + expr.getErrorMessage());
            return;
        }

        RectTransform rt = GetComponent<RectTransform>();
        float width = rt.rect.width;
        float height = rt.rect.height;

        float step = (timeEnd - timeStart) / resolution;

        // Origin at bottom-left quadrant vertically and touching left wall
        float originX = 0;
        float originY = height / 4f;

        for (int i = 0; i <= resolution; i++)
        {
            float t = timeStart + i * step;
            tArg.setArgumentValue(t);
            double y = expr.calculate();
            if (double.IsNaN(y) || double.IsInfinity(y)) y = 0;

            float x = Mathf.Lerp(originX, width, (t - timeStart) / (timeEnd - timeStart));
            float yVal = (float)y * yScale;
            float finalY = originY + (flipY ? -yVal : yVal);

            points.Add(new Vector2(x, finalY));
        }

        SetVerticesDirty();
    }
}
