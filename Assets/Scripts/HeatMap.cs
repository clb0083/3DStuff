using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeatMap : MonoBehaviour
{   
    public Material triggerMaterial;
    public GameObject HeatUI;
    public bool toggleColorOn = false;
    public float minPing = 0f;
    public float maxPing = 10f;

    public void heatMapButton()
    {           
        if(toggleColorOn)
        {
            heatMapToggleOFF();
            HeatUI.SetActive(false);
        }
        else
        {   
            //Color color = triggerMaterial.color;
            //color.a = 0;
            //triggerMaterial.color = color;
            HeatUI.SetActive(!HeatUI.activeSelf);
            toggleColorOn = true;
        }
    }
    void Start()
    {
        // Calculate the gradient colors
        CalculateColorGradient();
    }
    void Update()
    {
        if(toggleColorOn)
        {
            heatMapToggleON();
            
        }
    }
    public void heatMapToggleON()
    {
        // Find all GameObjects with collider and trigger components in the scene
        GameObject[] triggerObjects = GameObject.FindGameObjectsWithTag("ConductorTrigger");

        // Loop through each trigger object found
        foreach (GameObject triggerObject in triggerObjects)
        {
            // Get the TriggerTracker component
            TriggerTracker triggerTracker = triggerObject.GetComponent<TriggerTracker>();
            //Debug.Log("got1");
            // Check if TriggerTracker component exists
            if (triggerTracker != null)
            {
                // Example: Change color based on ping count
                int pingCount = triggerTracker.GetPingCount();

                // Change color based on ping count value
                Renderer renderer = triggerObject.GetComponent<Renderer>();
                //Debug.Log("got2");
                if (renderer != null)
                {
                    if (pingCount >= 5)
                    {
                        renderer.material.color = Color.red;
                    }
                    else if (pingCount >= 4)
                    {
                        renderer.material.color = colorGradient[4];
                    }
                    else if (pingCount >=3)
                    {
                        renderer.material.color = colorGradient[3];
                    }
                    else if (pingCount >= 2)
                    {
                        renderer.material.color = Color.yellow;
                    }
                    else if (pingCount >= 1)
                    {
                        renderer.material.color = colorGradient[2];
                    }
                    else
                    {
                        renderer.material.color = Color.green;
                    }
                    Debug.Log("done");
                }
                /*if (renderer != null)
                {
                    if (pingCount >= 3)
                    {
                        renderer.material.color = Color.red;
                    }
                    else if (pingCount >= 1)
                    {
                        renderer.material.color = Color.yellow;
                    }
                    else
                    {
                        renderer.material.color = Color.green;
                    }
                    Debug.Log("done");
                }*/
            }
        }
        toggleColorOn = true;
    }

    public void heatMapToggleOFF()
    {
        // Find all GameObjects with collider and trigger components in the scene
        GameObject[] triggerObjects = GameObject.FindGameObjectsWithTag("ConductorTrigger");

        // Loop through each trigger object found
        foreach (GameObject triggerObject in triggerObjects)
        {
            // Change color based on ping count value
            Renderer renderer = triggerObject.GetComponent<Renderer>();
            renderer.material = triggerMaterial;
        }
        toggleColorOn = false;
    }

   // Define the colors for the gradient
    Color[] colorGradient = new Color[5];

    void CalculateColorGradient()
    {
        // Define the starting and ending colors
        Color startColor = Color.green;
        Color middleColor = Color.yellow;
        Color endColor = Color.red;

        // Calculate the intermediate colors
        colorGradient[0] = Color.Lerp(startColor, middleColor, 0.25f);   // Green to Yellow (25%)
        colorGradient[1] = Color.Lerp(startColor, middleColor, 0.5f);    // Green to Yellow (50%)
        colorGradient[2] = Color.Lerp(startColor, middleColor, 0.75f);   // Green to Yellow (75%)
        colorGradient[3] = Color.Lerp(middleColor, endColor, 0.25f);     // Yellow to Red (25%)
        colorGradient[4] = Color.Lerp(middleColor, endColor, 0.5f);      // Yellow to Red (50%)

    }
}

