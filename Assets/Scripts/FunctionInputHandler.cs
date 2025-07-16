using UnityEngine;
using TMPro;
using org.mariuszgromada.math.mxparser;

public class FunctionInputHandler : MonoBehaviour
{
    public TMP_InputField functionInputField;
    public TMP_InputField timeLengthInputField;

    private Expression parsedFunction;
    private Argument tArg;
    private Argument timeLengthArg;

    public float timeLength = 5f;

    void Start()
    {
        tArg = new Argument("t", 0);
        timeLengthArg = new Argument("timeLength", timeLength);
        RefreshFunction();
    }

    public void SetFunction(string functionString)
    {
        parsedFunction = new Expression(functionString, tArg, timeLengthArg);
        Debug.Log("Parsed function: " + functionString);
    }

    public void RefreshFunction()
    {
        SetFunction(functionInputField.text);
        UpdateTimeLength(timeLengthInputField.text);
    }
    public float GetValueAtTime(float t)
    {
        tArg.setArgumentValue(t);
        timeLengthArg.setArgumentValue(timeLength);

        double result = parsedFunction.calculate();
        if (double.IsNaN(result))
        {
            Debug.LogWarning("Invalid function at t = " + t);
            return 0f;
        }

        return (float)result;
    }

    public void OnFunctionChanged() => SetFunction(functionInputField.text);

    public void OnTimeLengthChanged()
    {
        UpdateTimeLength(timeLengthInputField.text);
        SetFunction(functionInputField.text); // Re-parse with new timeLength
    }

    private void UpdateTimeLength(string input)
    {
        if (float.TryParse(input, out float result) && result > 0f)
        {
            timeLength = result;
        }
        else
        {
            Debug.LogWarning("Invalid or zero time length entered. Defaulting to 5.");
            timeLength = 5f;
        }
    }

}

