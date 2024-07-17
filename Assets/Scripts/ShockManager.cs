using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ShockManager : MonoBehaviour
{
    public TMP_InputField amplitudeInputField; // Input field for shock amplitude
    public Transform circuitBoard; // Reference to the circuit board transform
    private Rigidbody circuitBoardRigidbody; // Rigidbody of the circuit board
    private Vector3 originalBoardPosition; // Original position of the circuit board

    private float amplitude;
    private float duration = 0.1f; // Hardcoded duration of the shock
    private float frequency = 5f; // Hardcoded frequency of the shock
    private float startTime;
    private bool isShockingX = false;
    private bool isShockingY = false;
    private bool isShockingZ = false;
    public TMP_Dropdown shockAxis;
    public Button shockButton;

    private void Start()
    {
        /*GameObject circuitBoardObject = GameObject.FindGameObjectWithTag("CircuitBoard");
        if (circuitBoardObject != null)
        {
            circuitBoard = circuitBoardObject.transform;
        }*/
    }

    private void FixedUpdate()
    {
        if (isShockingX)
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
                isShockingX = false; // Stop shocking after the duration
                Debug.Log("Shock finished.");
                ShockFinished();
                // Reset the position of the circuit board
                if (circuitBoard != null)
                {
                    circuitBoard.position = originalBoardPosition;
                }
            }
        }

        if (isShockingY)
        {
            float elapsedTime = Time.time - startTime;
            if (elapsedTime < duration)
            {
                // Calculate the current offset based on sine wave
                float sineValue = Mathf.Sin(elapsedTime * frequency * 2 * Mathf.PI);
                float offsetY = sineValue * amplitude;

                // Move the circuit board
                Vector3 targetBoardPosition = new Vector3(originalBoardPosition.x, originalBoardPosition.y + offsetY, originalBoardPosition.z);
                if (circuitBoardRigidbody != null)
                {
                    circuitBoardRigidbody.MovePosition(targetBoardPosition);
                }
            }
            else
            {
                isShockingY = false; // Stop shocking after the duration
                Debug.Log("Shock finished.");
                ShockFinished();
                // Reset the position of the circuit board
                if (circuitBoard != null)
                {
                    circuitBoard.position = originalBoardPosition;
                }
            }
        }

        if (isShockingZ)
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
                isShockingZ = false; // Stop shocking after the duration
                Debug.Log("Shock finished.");
                ShockFinished();
                // Reset the position of the circuit board
                if (circuitBoard != null)
                {
                    circuitBoard.position = originalBoardPosition;
                }
            }
        }
    }

    public void StartShockX()
    {
        // Find the Rigidbody component of the circuit board
        circuitBoardRigidbody = circuitBoard.GetComponent<Rigidbody>();
        // Store the original position of the circuit board
        originalBoardPosition = circuitBoard.position;

        // Parse user input for amplitude
        if (float.TryParse(amplitudeInputField.text, out amplitude))
        {
            Debug.Log("Starting shock with parameters:");
            Debug.Log("Amplitude: " + amplitude);
            Debug.Log("Duration: " + duration);
            Debug.Log("Frequency: " + frequency);

            startTime = Time.time;
            isShockingX = true;
            Debug.Log("Shock started.");
        }
    }

    public void StartShockY()
    {
        // Find the Rigidbody component of the circuit board
        circuitBoardRigidbody = circuitBoard.GetComponent<Rigidbody>();
        // Store the original position of the circuit board
        originalBoardPosition = circuitBoard.position;

        // Parse user input for amplitude
        if (float.TryParse(amplitudeInputField.text, out amplitude))
        {
            Debug.Log("Starting shock with parameters:");
            Debug.Log("Amplitude: " + amplitude);
            Debug.Log("Duration: " + duration);
            Debug.Log("Frequency: " + frequency);

            startTime = Time.time;
            isShockingY = true;
            Debug.Log("Shock started.");
        }
    }

    public void StartShockZ()
    {
        // Find the Rigidbody component of the circuit board
        circuitBoardRigidbody = circuitBoard.GetComponent<Rigidbody>();
        // Store the original position of the circuit board
        originalBoardPosition = circuitBoard.position;

        // Parse user input for amplitude
        if (float.TryParse(amplitudeInputField.text, out amplitude))
        {
            Debug.Log("Starting shock with parameters:");
            Debug.Log("Amplitude: " + amplitude);
            Debug.Log("Duration: " + duration);
            Debug.Log("Frequency: " + frequency);

            startTime = Time.time;
            isShockingZ = true;
            Debug.Log("Shock started.");
        }
    }

    public void shockPressed()
    {
        GameObject circuitBoardObject = GameObject.FindGameObjectWithTag("CircuitBoard");
        if (circuitBoardObject != null)
        {
            circuitBoard = circuitBoardObject.transform;
        }
        shockButton.interactable = false;
        int selectedAxis = shockAxis.value;
        switch (selectedAxis)
        {
            case 0:
                StartShockX();
                break;
            case 1:
                StartShockY();
                break;
            case 2:
                StartShockZ();
                break;
        }
    }
    private void ShockFinished()
    {
        shockButton.interactable = true; // Re-enable the Vibrate button
    }
}


