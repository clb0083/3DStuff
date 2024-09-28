using UnityEngine;
using TMPro; // Include this namespace for TextMeshPro

public class MovePlaneTMP : MonoBehaviour
{
    public TMP_InputField inputFieldX; // Assign this in the Inspector
    public TMP_InputField inputFieldY; // Assign this in the Inspector
    public TMP_InputField inputFieldZ; // Assign this in the Inspector

    void Update()
    {
        // Check if all input fields are not empty
        if (!string.IsNullOrEmpty(inputFieldX.text) && 
            !string.IsNullOrEmpty(inputFieldY.text) && 
            !string.IsNullOrEmpty(inputFieldZ.text))
        {
            // Try to parse the text as floats
            if (float.TryParse(inputFieldX.text, out float xValue) &&
                float.TryParse(inputFieldY.text, out float yValue) &&
                float.TryParse(inputFieldZ.text, out float zValue))
            {
                // Update the plane's position
                transform.localScale = new Vector3(xValue * 20f, yValue * 10f, zValue * 20f);
                transform.localPosition = new Vector3 (0,yValue/.2f ,0);
            }
        }
    }

    public void ClearInputs()
    {
        // Optionally, clear the input fields when needed
        inputFieldX.text = string.Empty;
        inputFieldY.text = string.Empty;
        inputFieldZ.text = string.Empty;
    }
}


