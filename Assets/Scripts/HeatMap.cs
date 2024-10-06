/*AU Team 1 (SP & SU 2024) used the help of Auburn graduate student Jake Botello
to learn Unity and C# in integrating design ideas. The team applied background
knowledge of MATLAB and C++ coding languages to develop various tools and
functions used throughout this script. ChatGPT was also used as a troubleshooting
reference.*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This scripts controls the heatmap feature.
public class HeatMap : MonoBehaviour
{   
    public Material triggerMaterial;
    public GameObject HeatUI;
    public bool toggleColorOn = false;
    public float minPing = 0f;
    public float maxPing = 10f;

    //Button to the tell the heatmap to turn on.
    public void heatMapButton()
    {           
        if(toggleColorOn)
        {
            heatMapToggleOFF();
            HeatUI.SetActive(false);
        }
        else
        {   
            HeatUI.SetActive(!HeatUI.activeSelf);
            toggleColorOn = true;
        }
    }
    void Start()
    {
        CalculateColorGradient();
    }
    void Update()
    {
        if(toggleColorOn)
        {
            heatMapToggleON();   
        }
    }
    //once heatmap is on, this will run constantly, and updates the colors as bridges form.
    public void heatMapToggleON()
    {
        GameObject[] triggerObjects = GameObject.FindGameObjectsWithTag("ConductorTrigger");

        foreach (GameObject triggerObject in triggerObjects)
        {
            TriggerTracker triggerTracker = triggerObject.GetComponent<TriggerTracker>();
           
            if (triggerTracker != null)
            {
                int pingCount = triggerTracker.GetPingCount();
                Renderer renderer = triggerObject.GetComponent<Renderer>();
                
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
                }
            }
        }
        toggleColorOn = true;
    }

    //Turns heatmap off.
    public void heatMapToggleOFF()
    {
        GameObject[] triggerObjects = GameObject.FindGameObjectsWithTag("ConductorTrigger");
        foreach (GameObject triggerObject in triggerObjects)
        {
            Renderer renderer = triggerObject.GetComponent<Renderer>();
            renderer.material = triggerMaterial;
        }
        toggleColorOn = false;
    }
    Color[] colorGradient = new Color[5];
    void CalculateColorGradient()
    {
        Color startColor = Color.green;
        Color middleColor = Color.yellow;
        Color endColor = Color.red;
        colorGradient[0] = Color.Lerp(startColor, middleColor, 0.25f);   // Green to Yellow (25%)
        colorGradient[1] = Color.Lerp(startColor, middleColor, 0.5f);    // Green to Yellow (50%)
        colorGradient[2] = Color.Lerp(startColor, middleColor, 0.75f);   // Green to Yellow (75%)
        colorGradient[3] = Color.Lerp(middleColor, endColor, 0.25f);     // Yellow to Red (25%)
        colorGradient[4] = Color.Lerp(middleColor, endColor, 0.5f);      // Yellow to Red (50%)
    }
}

