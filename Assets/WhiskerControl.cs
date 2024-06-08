using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using UnityEngine.UI;
using System.Data;

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
    //private int conductorCollisions = 0; // Counter to track Conductor collisions UNCOMMENT THIS IF IT FUCKED UP
    private HashSet<string> currentConnections = new HashSet<string>();
    public GameObject UIObject;
    public UIScript uiScript;
    public static Dictionary<string, int> bridgesPerConductor = new Dictionary<string, int>(); 
    private Renderer objectRenderer; // for highlighting
    public Color bridgeDetectedColor = Color.red; // for highlighting
    public Color defaultColor = Color.white; // for highlighting
    
    // BRIDGING DETECTION
    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("Conductor"))
        {
            // Add the conductor to the set of current connections
            currentConnections.Add(collision.gameObject.name);

            // Check if we have two or more connections
            if (currentConnections.Count >= 2)
            {
                // Only increment connectionsMade if this is the first time we're logging the connection
                if (!haveLoggedConnection)
                {
                    objectRenderer = GetComponent<Renderer>();

                    bridgesPerConductor[gameObject.name]++;
                    UIObject.GetComponent<UIScript>().bridgesDetected++;
                    UIObject.GetComponent<UIScript>().bridgesPerRun++;
                    objectRenderer.material.color = bridgeDetectedColor;
                    //print(connectionsMade);

                    haveLoggedConnection = true;
                }
            }
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Conductor"))
        {
            // Remove the conductor from the set of current connections
            currentConnections.Remove(collision.gameObject.name);

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
    
    /*//Function to move camera to bridges
    public void ViewBridges()
    {
        if(UIObject.GetComponent<UIScript>().bridgesDetected < 1)
        {
            Debug.Log("No whiskers are currently bridged.");
        }

        if(UIObject.GetComponent<UIScript>().bridgesDetected >= 1)
        {
            
        }

    }*/
}

