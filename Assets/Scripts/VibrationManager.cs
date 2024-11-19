/*AU Team 1 (SP & SU 2024) used the help of Auburn graduate student Jake Botello
to learn Unity and C# in integrating design ideas. The team applied background
knowledge of MATLAB and C++ coding languages to develop various tools and
functions used throughout this script. ChatGPT was also used as a troubleshooting
reference.*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

//Controls the External Force: Vibration
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
    public UIScript uiScript;

    private void Start()
    {
  
    }

    //Applys the correct vibration in the appropriate axis depending on the user inputs
    private void FixedUpdate()
    {
        if (isVibratingX)
        {
            float elapsedTime = Time.time - startTime;
            if (elapsedTime < duration)
            {
                float sineValue = Mathf.Sin(elapsedTime * frequency * 2 * Mathf.PI);
                float offsetX = sineValue * amplitude;

                Vector3 targetBoardPosition = new Vector3(originalBoardPosition.x + offsetX, originalBoardPosition.y, originalBoardPosition.z);
                if (circuitBoardRigidbody != null)
                {
                    circuitBoardRigidbody.MovePosition(targetBoardPosition);
                }
            }
            else
            {
                isVibratingX = false; 
                Debug.Log("Vibration finished.");
                VibrationFinished();
              
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
                float sineValue = Mathf.Sin(elapsedTime * frequency * 2 * Mathf.PI);
                float offsetY = sineValue * amplitude;

                Vector3 targetBoardPosition = new Vector3(originalBoardPosition.x, originalBoardPosition.y  + offsetY, originalBoardPosition.z);
                if (circuitBoardRigidbody != null)
                {
                    circuitBoardRigidbody.MovePosition(targetBoardPosition);
                }
            }
            else
            {
                isVibratingY = false;
                Debug.Log("Vibration finished.");
                VibrationFinished();

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

                float sineValue = Mathf.Sin(elapsedTime * frequency * 2 * Mathf.PI);
                float offsetZ = sineValue * amplitude;

                Vector3 targetBoardPosition = new Vector3(originalBoardPosition.x, originalBoardPosition.y, originalBoardPosition.z + offsetZ);
                if (circuitBoardRigidbody != null)
                {
                    circuitBoardRigidbody.MovePosition(targetBoardPosition);
                }
            }
            else
            {
                isVibratingZ = false; 
                Debug.Log("Vibration finished.");
                VibrationFinished();

                if (circuitBoard != null)
                {
                    circuitBoard.position = originalBoardPosition;
                }
            }
        }
      
    }

    //These functions actually run the vibration
    public void StartVibrationX()
    {

        circuitBoardRigidbody = circuitBoard.GetComponent<Rigidbody>();
        originalBoardPosition = circuitBoard.position;

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
        circuitBoardRigidbody = circuitBoard.GetComponent<Rigidbody>();
        originalBoardPosition = circuitBoard.position;

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
        circuitBoardRigidbody = circuitBoard.GetComponent<Rigidbody>();
        originalBoardPosition = circuitBoard.position;

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

    //This runs whenever the vibration button is pressed or called.
    public void vibratePressed()
    {
        if(float.Parse(amplitudeInputField.text) > 35)
        {
            uiScript.SetErrorMessage("Vibration Amplitude is too high.");
            return;
        }
        if(float.Parse(frequencyInputField.text) > 150)
        {
            uiScript.SetErrorMessage("Vibration frequnecy is too high.");
            return;
        }
        if(float.Parse(durationInputField.text) > 30)
        {
            uiScript.SetErrorMessage("Vibration Amplitude is too high.");
            return;
        }
        
        GameObject circuitBoardObject = GameObject.FindGameObjectWithTag("CircuitBoard");
        if (circuitBoardObject != null)
        {
            circuitBoard = circuitBoardObject.transform;
        }
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
        vibrateButton.interactable = true; 
    }
}   
