using UnityEngine;
using TMPro;

public class CircuitBoardFinder : MonoBehaviour
{
    // References to the TMP input fields
    public TMP_InputField CircuitRotateX;
    public TMP_InputField CircuitRotateY;
    public TMP_InputField CircuitRotateZ;

    // This method will be called when the button is clicked
    public void OnButtonClick()
    {
        // Find the GameObject with the tag "CircuitBoard"
        GameObject circuitBoard = GameObject.FindGameObjectWithTag("CircuitBoard");

        if (circuitBoard != null)
        {
            // Get the rotation values, defaulting to 0 if input is empty
            float xRotation = GetInputValue(CircuitRotateX);
            float yRotation = GetInputValue(CircuitRotateY);
            float zRotation = GetInputValue(CircuitRotateZ);

            // Set the rotation of the circuitBoard
            circuitBoard.transform.rotation = Quaternion.Euler(xRotation, yRotation, zRotation);
            Debug.Log($"CircuitBoard rotated to: X={xRotation}, Y={yRotation}, Z={zRotation}");
        }
        else
        {
            Debug.Log("No CircuitBoard found.");
        }
    }

    // Helper method to parse input values
    private float GetInputValue(TMP_InputField inputField)
    {
        // Check if the input field is empty
        if (string.IsNullOrWhiteSpace(inputField.text))
        {
            return 0f; // Default to 0 if empty
        }

        // Try to parse the input; if parsing fails, return 0
        if (float.TryParse(inputField.text, out float result))
        {
            return result;
        }
        
        return 0f; // Default to 0 if input is invalid
    }
}
