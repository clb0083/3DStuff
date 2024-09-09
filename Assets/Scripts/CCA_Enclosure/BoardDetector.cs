using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BoardDetector : MonoBehaviour
{
    public GameObject importedModel; // parent gameobject of the entire imported board
    public GameObject detectedBoard { get; private set; } //stores the detected board

    private void Start()
    {
        // Call the method to find the board
        detectedBoard = FindBoard(importedModel);

        if (detectedBoard != null)
        {
            Debug.Log("Board detected: " + detectedBoard.name);
        }
        else
        {
            Debug.LogError("Board could not be detected");
        }
    }

    // Method to find the board based on largest object in heirachy
    private GameObject FindBoard(GameObject rootObject)
    {
        GameObject largestObject = null;
        float largestArea = 0;

        // Iterate over all the children of the imported model (all the components)
        foreach (Renderer renderer in rootObject.GetComponentsInChildren<Renderer>())
        {
            Bounds bounds = renderer.bounds;

            //calculates the surface area
            float surfaceArea = bounds.size.x * bounds.size.z;

            //Compare with the larges founds so far
            if (surfaceArea > largestArea)
            {
                largestArea = surfaceArea;
                largestObject = renderer.gameObject;
            }
        }
        return largestObject;
    }
}
