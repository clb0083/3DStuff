using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using org.mariuszgromada.math.mxparser;
using UnityEngine.UIElements;

public class WhiskerAcceleration : MonoBehaviour
{
    public TMP_InputField inputField;
    public float timeStart = 0;
    public float timeEnd = 10;
    public int resolution = 200;

    public void GetAccelVals()
    {
        if (inputField == null) return;
        if (string.IsNullOrWhiteSpace(inputField.text))
        {
            Debug.LogError("No Funciton Input"); return;
        }

        string exprText = inputField.text;
        Argument tArg = new Argument("t", 0);
        Expression expr = new Expression(exprText, tArg);
       

        if (!expr.checkSyntax())
        {
            Debug.LogError("Invalid syntax: " + expr.getErrorMessage());
            return;
        }

        float step = (timeEnd - timeStart) / resolution;

        for (int i = 0; i < resolution; i++)
        {
            float t = timeStart + i * step;
            tArg.setArgumentValue(t);
            double y = expr.calculate();
            if (double.IsNaN(y) || double.IsInfinity(y)) y = 0;
            
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
