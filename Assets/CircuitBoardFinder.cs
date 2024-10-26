using UnityEngine;
using TMPro;
using UnityEngine.UI;

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

    // Reference to the RotateSpinToggle for resetting
    public Toggle RotateSpinToggle;

    private Quaternion initialRotation;
    public float xSpinRate, ySpinRate, zSpinRate;
    private GameObject circuitBoard;
    private Coroutine spinCoroutine;

    public GameObject basePlane;

    void Start()
    {
        circuitBoard = GameObject.FindGameObjectWithTag("CircuitBoard");
        if (circuitBoard != null)
        {
            initialRotation = circuitBoard.transform.rotation;
        }

        // Add listeners to input fields
        CircuitRotateX.onValueChanged.AddListener(delegate { UpdateCircuitBoard(); });
        CircuitRotateY.onValueChanged.AddListener(delegate { UpdateCircuitBoard(); });
        CircuitRotateZ.onValueChanged.AddListener(delegate { UpdateCircuitBoard(); });
        SpinRateX.onValueChanged.AddListener(delegate { UpdateCircuitBoard(); });
        SpinRateY.onValueChanged.AddListener(delegate { UpdateCircuitBoard(); });
        SpinRateZ.onValueChanged.AddListener(delegate { UpdateCircuitBoard(); });
        RotateSpinToggle.onValueChanged.AddListener(delegate { UpdateCircuitBoard(); });
    }

    public void UpdateCircuitBoard()
    {
        ResetCircuitBoard(); // Reset before updating

        if (circuitBoard != null)
        {
            // Get the rotation values
            float xRotation = GetInputValue(CircuitRotateX);
            float yRotation = GetInputValue(CircuitRotateY);
            float zRotation = GetInputValue(CircuitRotateZ);

            // Set the rotation of the circuitBoard
            circuitBoard.transform.rotation = Quaternion.Euler(xRotation, yRotation, zRotation);
            Debug.Log($"CircuitBoard rotated to: X={xRotation}, Y={yRotation}, Z={zRotation}");

            // Get the spin rates
            xSpinRate = GetInputValue(SpinRateX);
            ySpinRate = GetInputValue(SpinRateY);
            zSpinRate = GetInputValue(SpinRateZ);

            // Start spinning the circuitBoard
            if (spinCoroutine != null) StopCoroutine(spinCoroutine);
            spinCoroutine = StartCoroutine(SpinBasePlane(basePlane, xSpinRate, ySpinRate, zSpinRate));
        }
        else
        {
            Debug.Log("No CircuitBoard found.");
        }
    }

    public float GetInputValue(TMP_InputField inputField)
    {
        if (string.IsNullOrWhiteSpace(inputField.text))
        {
            return 0f; // Default to 0 if empty
        }

        if (float.TryParse(inputField.text, out float result))
        {
            return result;
        }

        return 0f; // Default to 0 if input is invalid
    }

    private System.Collections.IEnumerator SpinBasePlane(GameObject basePlane , float xSpinRate, float ySpinRate, float zSpinRate)
    {
        while (true)
        {
            basePlane.transform.Rotate(xSpinRate * Time.deltaTime, ySpinRate * Time.deltaTime, zSpinRate * Time.deltaTime);
            yield return null; // Wait for the next frame
        }
    }

    public void ResetCircuitBoard()
    {
        if (RotateSpinToggle != null && RotateSpinToggle.isOn)
        {
            float xRotation = GetInputValue(CircuitRotateX);
            float yRotation = GetInputValue(CircuitRotateY);
            float zRotation = GetInputValue(CircuitRotateZ);

            circuitBoard.transform.rotation = Quaternion.Euler(xRotation, yRotation, zRotation);
            circuitBoard.transform.position = Vector3.zero; // Reset position if needed
            basePlane.transform.position= Vector3.zero;

            if (spinCoroutine != null) StopCoroutine(spinCoroutine);
            Debug.Log("CircuitBoard and BasePlane reset to initial rotation and spin.");
        }
    }
}
