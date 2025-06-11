using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using org.mariuszgromada.math.mxparser;
using UnityEngine.UIElements;

public class WhiskerAcceleration : MonoBehaviour
{
    public TMP_InputField timeFunction;
    public float timeStart = 0;
    public TMP_InputField timeLength;
    public int resolution = 200;
    private ConstantForce cForce;
    private Vector3 forceDirection;

    public void ApplyAccel()
    {
        // Find all objects with the tag "whiskerClone"
        GameObject[] objectsWithTag = GameObject.FindGameObjectsWithTag("whiskerClone");

        Vector3 forceDirection = Vector3.zero;

        if (string.IsNullOrWhiteSpace(timeFunction.text))
        {
            Debug.LogError("No Funciton Input"); return;
        }
        if (float.TryParse(timeLength.text, out float timeEnd) && timeEnd > 0)
        {

        }

        string exprText = timeFunction.text;
        Argument tArg = new Argument("t", 0);
        Expression expr = new Expression(exprText, tArg);


        if (!expr.checkSyntax())
        {
            Debug.LogError("Invalid syntax: " + expr.getErrorMessage());
            return;
        }

        // time frame over resolution
        float step = (timeEnd - timeStart) / resolution;

        // 0 to end count
        for (int i = 0; i < resolution; i++)
        {
            // step time
            float time = timeStart + i * step;

            // get function data (mxparser)
            tArg.setArgumentValue(time);
            double accel = expr.calculate();

            // zero bad accel input
            if (double.IsNaN(accel) || double.IsInfinity(accel)) accel = 0;

            //apply accel in x direction
            forceDirection = new Vector3((float)accel, 0, 0);

            // Apply the force to each object
            foreach (GameObject obj in objectsWithTag)
            {
                ConstantForce cForce = obj.GetComponent<ConstantForce>() ?? obj.AddComponent<ConstantForce>();
                cForce.force = forceDirection;
            }
        }
    }
    //Resets gravity /ResetButton
    public void ResetAccel()
    {
        GameObject[] objectsWithTag = GameObject.FindGameObjectsWithTag("whiskerClone");
        foreach (GameObject obj in objectsWithTag)
        {
            cForce = GetComponent<ConstantForce>();
            forceDirection = new Vector3(0, 0, 0);
            cForce.force = forceDirection;
        }
    }
}
