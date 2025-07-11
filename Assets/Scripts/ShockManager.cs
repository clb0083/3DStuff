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

//This script controls one of the external forces: Shock
public class ShockManager : MonoBehaviour
{
    public TMP_InputField amplitudeInputField;
    public TMP_InputField halfPeriodInput;
    public Transform circuitBoard; 
    private Rigidbody circuitBoardRigidbody; 
    private Vector3 originalBoardPosition; 
    private float amplitude;
    private float duration; 
    private float frequency; 
    private float startTime;
    private bool isShockingX = false;
    private bool isShockingY = false;
    private bool isShockingZ = false;
    public TMP_Dropdown shockAxis;
    public Button shockButton;
    public UIScript uiScript;

    private void Start()
    {
        
    }
    
    //Runs the shock force every 2 seconds based on what axis the user has selected.
    private void FixedUpdate()
    {
        
        if (isShockingX)
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
                isShockingX = false;
                Debug.Log("Shock finished.");
                ShockFinished();
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
             
                float sineValue = Mathf.Sin(elapsedTime * frequency * 2 * Mathf.PI);
                float offsetY = sineValue * amplitude;

                Vector3 targetBoardPosition = new Vector3(originalBoardPosition.x, originalBoardPosition.y + offsetY, originalBoardPosition.z);
                if (circuitBoardRigidbody != null)
                {
                    circuitBoardRigidbody.MovePosition(targetBoardPosition);
                }
            }
            else
            {
                isShockingY = false; 
                Debug.Log("Shock finished.");
                ShockFinished();
               
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
                isShockingZ = false;
                Debug.Log("Shock finished.");
                ShockFinished();
               
                if (circuitBoard != null)
                {
                    circuitBoard.position = originalBoardPosition;
                }
            }
        }
    }

    //controls to tell the code which one of the previous functions to run. 
    public void StartShockX()
    {
        circuitBoardRigidbody = circuitBoard.GetComponent<Rigidbody>();
        originalBoardPosition = circuitBoard.position;

        if (float.TryParse(amplitudeInputField.text, out amplitude) && (float.TryParse(halfPeriodInput.text, out duration)))
        {
            frequency = 1 / (duration * 2);
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
        circuitBoardRigidbody = circuitBoard.GetComponent<Rigidbody>();
        originalBoardPosition = circuitBoard.position;

        if (float.TryParse(amplitudeInputField.text, out amplitude) && (float.TryParse(halfPeriodInput.text, out duration)))
        {
            frequency = 1 / (duration * 2);
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
        circuitBoardRigidbody = circuitBoard.GetComponent<Rigidbody>();
        originalBoardPosition = circuitBoard.position;

        if (float.TryParse(amplitudeInputField.text, out amplitude) && (float.TryParse(halfPeriodInput.text, out duration)))
        {
            frequency = 1 / (duration * 2);
            Debug.Log("Starting shock with parameters:");
            Debug.Log("Amplitude: " + amplitude);
            Debug.Log("Duration: " + duration);
            Debug.Log("Frequency: " + frequency);

            startTime = Time.time;
            isShockingZ = true;
            Debug.Log("Shock started.");
        }
    }

    //Button to begin or apply the Shock/Impact force to the board.
    public void shockPressed()
    {
        if(Convert.ToInt32(amplitudeInputField.text) > 25)
        {
            uiScript.SetErrorMessage("Shock Amplitude is too high.");
            return;
        }
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
        shockButton.interactable = true;
    }
}


