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
            Debug.Log("Board detected: " + detectedBoard.name); //log the detected board
            Renderer boardRenderer = detectedBoard.GetComponent<Renderer>();
            //Debug.Log("detected Board Bounds: " + boardRenderer.bounds); //log the detected board bounds
        }
        else
        {
            Debug.LogError("Board could not be detected");
        }
    }

    // Method to find the board based on largest object in heirachy
    private GameObject FindBoard(GameObject rootObject)
    {
        GameObject largestFlatObject = null;
        float largestSurfaceArea = 0;

        // Iterate over all the children of the imported model (all the components)
        foreach (Renderer renderer in rootObject.GetComponentsInChildren<Renderer>())
        {
            Bounds bounds = renderer.bounds;

            //calculates the surface area
            float surfaceArea = bounds.size.x * bounds.size.z;

            //Debug.Log($"Checking Object: {renderer.gameObject.name}, Surface Area: {surfaceArea}, Thickness (Y): {bounds.size.y}, Center Y: {bounds.center.y}");
            //check if the object is realtively flat by comparing its height (Y) with its surface area
            // Example: thickness should be relatively small compared to surface area
            if (surfaceArea > largestSurfaceArea && bounds.size.y < (0.3f * Mathf.Min(bounds.size.x, bounds.size.z)))
            {
                largestSurfaceArea = surfaceArea;
                largestFlatObject = renderer.gameObject;

                //Debug.Log($"Potential Board Detected: {largestFlatObject.name}, Surface Area; {surfaceArea}, thickness (Y): {bounds.size.y}");
            }
        }

        if (largestFlatObject != null)
        {
            //Debug.Log("Largest Flat Object Detected: " + largestFlatObject.name);
        }
        else
        {
            Debug.LogWarning("No flat object detected as the board.");
        }
        return largestFlatObject;
    }
}
