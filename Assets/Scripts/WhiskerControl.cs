using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using UnityEngine.UI;
using System.Data;
using System.Linq; // Add this namespace for LINQ functionality

public class WhiskerControl : MonoBehaviour
{
    public Dropdown gravity;
    private ConstantForce cForce;
    private Vector3 forceDirection;
    public int selectedIndex = 0;
    public Material targetMaterial;
    public int bridges = 0;
    bool confirmGravity; // used to tell program if gravity has been added or not.
    
    // Start is called before the first frame update
    void Start()
    {
        //Initialize the dictionary for the conductor pad NEW NEW NEW
        if (!bridgesPerConductor.ContainsKey(gameObject.name))
        {
            bridgesPerConductor[gameObject.name] = 0;
        }
        //for accessing target obj list
    }

    // Update is called once per frame
    public float detectionRadius = 0.5f; // Adjust this value based on your requirements
    public float rayDistance = 1.0f;
    public LayerMask conductorLayer; // Set this layer to the layer of your conductor objects

    void Update()
    {
        if(confirmGravity)
        {
            GetGravitySelection(gravity.value);
        }
    }
 
    //gravityStuff
public void ConfirmButtonPressed()
{
    GetGravitySelection(gravity.value);
    confirmGravity = true;
    UIObject.GetComponent<UIScript>().startSim = true;//NEW
}

public void GetGravitySelection(int val)
{
    ApplyGravity(val); //selectedIndex
}

public void ApplyGravity(int val)
{
    GameObject[] objectsWithTag = GameObject.FindGameObjectsWithTag("whiskerClone");

    Vector3 forceDirection = Vector3.zero;

    switch (val)
    {
        case 0:
            forceDirection = new Vector3(0, -10, 0);
            break;
        case 1:
            forceDirection = new Vector3(0, -2, 0);
            break;
        case 2:
            forceDirection = new Vector3(0, -4, 0);
            break;
    }

    foreach (GameObject obj in objectsWithTag)
    {
        ConstantForce cForce = obj.GetComponent<ConstantForce>();
        if (cForce == null)
        {
            // If the ConstantForce component is not found, add it
            cForce = obj.AddComponent<ConstantForce>();
        }
        cForce.force = forceDirection;
    }
}
    public void ResetGravity()
    {
        GameObject[] objectsWithTag = GameObject.FindGameObjectsWithTag("whiskerClone");

        foreach (GameObject obj in objectsWithTag)
        {
            cForce = GetComponent<ConstantForce>();
            forceDirection = new Vector3(0,0,0);
            cForce.force = forceDirection;
        }
        confirmGravity = false;
    }

    public bool haveLoggedConnection;
    public bool haveMadeOneConnection;
    public string firstConnection;
    public bool haveMadeSecondConnection;
    public string secondConnection;
    public int connectionsMade;
    public GameObject UIObject;
    public UIScript uiScript;
    public static Dictionary<string, int> bridgesPerConductor = new Dictionary<string, int>(); 
    private Renderer objectRenderer; // for highlighting
    public Color bridgeDetectedColor = Color.red; // for highlighting
    public Color defaultColor = Color.white; // for highlighting
    public HashSet<string> currentConnections = new HashSet<string>();
    public Color[] colors;
    //public HeatMapTracker heatMapTracker;
    private Dictionary<GameObject, int> triggerInteractionCounts = new Dictionary<GameObject, int>();
    

    // BRIDGING DETECTION ORIGINAL
    private void  OnTriggerStay(Collider trigger) // OnCollisionStay(Collision collision)// OnTriggerStay(Collider trigger)
    {
        if (trigger.gameObject.CompareTag("ConductorTrigger")) // collision // trigger
        {
            // Add the conductor to the set of current connections
            currentConnections.Add(trigger.gameObject.name);
            // Increment interaction count for this trigger
            //IncrementInteractionCount(trigger.gameObject);
            // Check if we have two or more connections
            if (currentConnections.Count >= 2)
            {
                // Only increment connectionsMade if this is the first time we're logging the connection
                if (!haveLoggedConnection)
                {
                    objectRenderer = GetComponent<Renderer>();
                    print("TRIGGER");
                    bridgesPerConductor[gameObject.name]++;
                    UIObject.GetComponent<UIScript>().bridgesDetected++;
                    UIObject.GetComponent<UIScript>().bridgesPerRun++;
                    objectRenderer.material.color = bridgeDetectedColor;
                    haveLoggedConnection = true;
                    //UpdateTriggerColor(trigger.transform);
                    //heatMapTracker.RegisterInteraction(trigger.gameObject);
                }
            }
        }
    }

     private void OnTriggerExit(Collider trigger)//OnCollisionExit(Collision collision)//OnTriggerExit(Collider trigger) 
    {
        if (trigger.gameObject.CompareTag("ConductorTrigger")) //collision
        {
            // Remove the conductor from the set of current connections
            currentConnections.Remove(trigger.gameObject.name); //collision

            // Reset state if there are fewer than 2 connections
            if (currentConnections.Count < 2 && haveLoggedConnection)
            {
                ResetConnectionState();
            }
        }
    }
    
    private void ResetConnectionState()
    {
        currentConnections.Clear();
        haveLoggedConnection = false;
        bridgesPerConductor[gameObject.name]--; //NEW
        UIObject.GetComponent<UIScript>().bridgesDetected--;
        UIObject.GetComponent<UIScript>().bridgesPerRun--;
        objectRenderer.material.color = defaultColor;
    }
}
