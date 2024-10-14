using UnityEngine;
using TMPro;

public class CircuitBoardFinder : MonoBehaviour
{
    // References to the TMP input fields for rotation
    public TMP_InputField CircuitRotateX;
    public TMP_InputField CircuitRotateY;
    public TMP_InputField CircuitRotateZ;

    // References to the TMP input fields for spin rates
    public TMP_InputField SpinRateX;
    public TMP_InputField SpinRateY;
    public TMP_InputField SpinRateZ;

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

            // Get the spin rates, defaulting to 0 if input is empty
            float xSpinRate = GetInputValue(SpinRateX);
            float ySpinRate = GetInputValue(SpinRateY);
            float zSpinRate = GetInputValue(SpinRateZ);

            // Start spinning the circuitBoard (you can implement a method to handle this)
            StartCoroutine(SpinCircuitBoard(circuitBoard, xSpinRate, ySpinRate, zSpinRate));
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

    // Coroutine to handle the spinning
    private System.Collections.IEnumerator SpinCircuitBoard(GameObject circuitBoard, float xSpinRate, float ySpinRate, float zSpinRate)
    {
        while (true)
        {
            circuitBoard.transform.Rotate(xSpinRate * Time.deltaTime, ySpinRate * Time.deltaTime, zSpinRate * Time.deltaTime);
            yield return null; // Wait for the next frame
        }
    }
}
