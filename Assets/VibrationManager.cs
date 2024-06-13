using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class VibrationManager : MonoBehaviour
{
    public TMP_InputField amplitudeInputField; // Input field for vibration amplitude
    public TMP_InputField durationInputField;  // Input field for vibration duration
    public TMP_InputField frequencyInputField; // Input field for vibration frequency
    public Transform circuitBoard; // Reference to the circuit board transform
    public GameObject ActualBoard;

    private Rigidbody circuitBoardRigidbody; // Rigidbody of the circuit board
    private Vector3 originalBoardPosition; // Original position of the circuit board

    private float amplitude;
    private float duration;
    private float frequency;
    private float startTime;
    private bool isVibrating = false;

    //private GameObject[] conductors; // Array of conductors
    //private Vector2[] originalConductorPositions; // Array to store original positions of conductors

    private void Start()
    {
        // Find the Rigidbody2D component of the circuit board
        circuitBoardRigidbody = circuitBoard.GetComponent<Rigidbody>();
        
        // Store the original position of the circuit board
        originalBoardPosition = circuitBoard.position;
    }

    private void FixedUpdate()
    {
        if (isVibrating)
        {
            float elapsedTime = Time.time - startTime;
            if (elapsedTime < duration)
            {
                // Calculate the current offset based on sine wave
                float sineValue = Mathf.Sin(elapsedTime * frequency * 2 * Mathf.PI);
                float offsetX = sineValue * amplitude;

                // Move the circuit board
                Vector3 targetBoardPosition = new Vector3(originalBoardPosition.x + offsetX, originalBoardPosition.y, originalBoardPosition.z);
                if (circuitBoardRigidbody != null)
                {
                    circuitBoardRigidbody.MovePosition(targetBoardPosition);
                }
            }
            else
            {
                isVibrating = false; // Stop vibrating after the duration
                Debug.Log("Vibration finished.");

                // Reset the position of the circuit board
                if (circuitBoard != null)
                {
                    circuitBoard.position = originalBoardPosition;
                }
            }
        }
        //ActualBoard.transform.position = new Vector3(circuitBoard.position.x, circuitBoard.position.y + 1, circuitBoard.position.z);
    }

    public void StartVibration()
    {
        // Parse user inputs for amplitude, duration, and frequency
        if (float.TryParse(amplitudeInputField.text, out amplitude) &&
            float.TryParse(durationInputField.text, out duration) &&
            float.TryParse(frequencyInputField.text, out frequency))
        {
            Debug.Log("Starting vibration with parameters:");
            Debug.Log("Amplitude: " + amplitude);
            Debug.Log("Duration: " + duration);
            Debug.Log("Frequency: " + frequency);

            startTime = Time.time;
            isVibrating = true;
            Debug.Log("Vibration started.");
        }
       
    }
}
