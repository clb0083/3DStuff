using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class VibrationManager : MonoBehaviour
{
    public TMP_InputField amplitudeInputField; // Input field for vibration amplitude
    public TMP_InputField durationInputField;  // Input field for vibration duration
    public TMP_InputField frequencyInputField; // Input field for vibration frequency
    public Transform circuitBoard; // Reference to the circuit board transform
    private Rigidbody circuitBoardRigidbody; // Rigidbody of the circuit board
    private Vector3 originalBoardPosition; // Original position of the circuit board
    private float amplitude;
    private float duration;
    private float frequency;
    private float startTime;
    private bool isVibratingX = false;
    private bool isVibratingY = false;
    private bool isVibratingZ = false;
    public TMP_Dropdown fAxis;
    public Button vibrateButton;
    //private GameObject[] conductors; // Array of conductors
    //private Vector2[] originalConductorPositions; // Array to store original positions of conductors

    private void Start()
    {
        GameObject circuitBoardObject = GameObject.FindGameObjectWithTag("CircuitBoard");
        if (circuitBoardObject != null)
        {
            circuitBoard = circuitBoardObject.transform;
        }
    }

    private void FixedUpdate()
    {
        if (isVibratingX)
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
                isVibratingX = false; // Stop vibrating after the duration
                Debug.Log("Vibration finished.");
                VibrationFinished();
                // Reset the position of the circuit board
                if (circuitBoard != null)
                {
                    circuitBoard.position = originalBoardPosition;
                }
            }
        }

        if (isVibratingY)
        {
            float elapsedTime = Time.time - startTime;
            if (elapsedTime < duration)
            {
                // Calculate the current offset based on sine wave
                float sineValue = Mathf.Sin(elapsedTime * frequency * 2 * Mathf.PI);
                float offsetY = sineValue * amplitude;

                // Move the circuit board
                Vector3 targetBoardPosition = new Vector3(originalBoardPosition.x, originalBoardPosition.y  + offsetY, originalBoardPosition.z);
                if (circuitBoardRigidbody != null)
                {
                    circuitBoardRigidbody.MovePosition(targetBoardPosition);
                }
            }
            else
            {
                isVibratingY = false; // Stop vibrating after the duration
                Debug.Log("Vibration finished.");
                VibrationFinished();
                // Reset the position of the circuit board
                if (circuitBoard != null)
                {
                    circuitBoard.position = originalBoardPosition;
                }
            }
        }

        if (isVibratingZ)
        {
            float elapsedTime = Time.time - startTime;
            if (elapsedTime < duration)
            {
                // Calculate the current offset based on sine wave
                float sineValue = Mathf.Sin(elapsedTime * frequency * 2 * Mathf.PI);
                float offsetZ = sineValue * amplitude;

                // Move the circuit board
                Vector3 targetBoardPosition = new Vector3(originalBoardPosition.x, originalBoardPosition.y, originalBoardPosition.z + offsetZ);
                if (circuitBoardRigidbody != null)
                {
                    circuitBoardRigidbody.MovePosition(targetBoardPosition);
                }
            }
            else
            {
                isVibratingZ = false; // Stop vibrating after the duration
                Debug.Log("Vibration finished.");
                VibrationFinished();
                // Reset the position of the circuit board
                if (circuitBoard != null)
                {
                    circuitBoard.position = originalBoardPosition;
                }
            }
        }
      
    }

    public void StartVibrationX()
    {
        // Find the Rigidbody component of the circuit board
        circuitBoardRigidbody = circuitBoard.GetComponent<Rigidbody>();
        // Store the original position of the circuit board
        originalBoardPosition = circuitBoard.position;
        
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
            isVibratingX = true;
            Debug.Log("Vibration started.");
        }
       
    }

    public void StartVibrationY()
    {
        // Find the Rigidbody component of the circuit board
        circuitBoardRigidbody = circuitBoard.GetComponent<Rigidbody>();
        // Store the original position of the circuit board
        originalBoardPosition = circuitBoard.position;
        
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
            isVibratingY = true;
            Debug.Log("Vibration started.");
        }
       
    }

    public void StartVibrationZ()
    {
        // Find the Rigidbody component of the circuit board
        circuitBoardRigidbody = circuitBoard.GetComponent<Rigidbody>();
        // Store the original position of the circuit board
        originalBoardPosition = circuitBoard.position;
        
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
            isVibratingZ = true;
            Debug.Log("Vibration started.");
        }
       
    }
    public void vibratePressed()
    {
        vibrateButton.interactable = false;

        int selectedAxis = fAxis.value;
        switch (selectedAxis)
        {
            case 0:
                StartVibrationX();
                break;
            case 1:
                StartVibrationY();
                break;
            case 2:
                StartVibrationZ();
                break;
        }
    }
    private void VibrationFinished()
    {
        vibrateButton.interactable = true; // Re-enable the Vibrate button
    }
}   
