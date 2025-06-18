using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using org.mariuszgromada.math.mxparser;
using UnityEngine.UIElements;

public class WhiskerAcceleration : MonoBehaviour
{
    public List<Vector2> functionData; // List of (time, acceleration)
    public float multiplier = 1f;      // Optional scale factor
    public Vector3 direction = Vector3.up; // Direction of acceleration

    private Rigidbody rb;
    private float startTime;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void SetFunctionData(List<Vector2> newFunctionData)
    {
        if (newFunctionData == null || newFunctionData.Count == 0)
        {
            Debug.LogWarning("No function data provided.");
            return;
        }
        functionData = newFunctionData;
        startTime = Time.time;
    }

    void FixedUpdate()
    {
        if (functionData == null || functionData.Count == 0) return;

        float elapsedTime = Time.time - startTime;


        // Find the acceleration value from functionData based on elapsedTime
        Vector2 accValue = GetAccelerationAtTime(elapsedTime);

        // Apply acceleration to Rigidbody
        rb.AddForce(new Vector3(accValue.y, 0, 0)); // example: apply acceleration on x axis
    }

    private Vector2 GetAccelerationAtTime(float t)
    {
        // Linear interpolation between points — assumes points are sorted by x (time)
        for (int i = 0; i < functionData.Count - 1; i++)
        {
            if (t >= functionData[i].x && t <= functionData[i + 1].x)
            {
                float alpha = (t - functionData[i].x) / (functionData[i + 1].x - functionData[i].x);
                float y = Mathf.Lerp(functionData[i].y, functionData[i + 1].y, alpha);
                return new Vector2(t, y);
            }
        }

        // If t is beyond last point, return last acceleration value
        return functionData[functionData.Count - 1];
    }

    public static List<Vector2> GetFunctionValues(string functionString, float tStart, float tEnd, int resolution)
    {
        List<Vector2> points = new List<Vector2>();

        // Define argument for mXparser
        Argument tArg = new Argument("t = 0");
        Expression expr = new Expression(functionString, tArg);

        for (int i = 0; i < resolution; i++)
        {
            float t = Mathf.Lerp(tStart, tEnd, i / (float)(resolution - 1));
            tArg.setArgumentValue(t);

            double result = expr.calculate();
            if (expr.checkSyntax() && !double.IsNaN(result) && !double.IsInfinity(result))
            {
                points.Add(new Vector2(t, (float)result));
            }
            else
            {
                points.Add(new Vector2(t, 0));
            }
        
        }

        return points;
    }

}

public static class FunctionValuesStore
{
    public static List<Vector2> functionPoints;
}
