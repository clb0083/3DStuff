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
    private Colliderstuff colliderstuff;
     private HashSet<GameObject> objectsInContact = new HashSet<GameObject>();//for accessing obj contact list
    

    // You can delete this variable--Jake added it to test the efficacy of the github pushes/pulls
    // Start is called before the first frame update
    void Start()
    {
        //for accessing target obj list
    }

    // Update is called once per frame
    void Update()
    {
      
    }

    //gravityStuff
    public void ConfirmButtonPressed()
    {
        GetGravitySelection(gravity.value);
    }
    public void GetGravitySelection(int val)
    {
        ApplyGravity(val); //selectedIndex
    }

    public void ApplyGravity(int val)
    {
        GameObject[] objectsWithTag = GameObject.FindGameObjectsWithTag("whiskerClone");

        foreach (GameObject obj in objectsWithTag)
        {
            Rigidbody rb = obj.GetComponent<Rigidbody>();
            if (val == 0)
            {
                cForce = GetComponent<ConstantForce>();
                forceDirection = new Vector3(0,-10,0);
                cForce.force = forceDirection;
                
            }

            if (val == 1)
            {
                cForce = GetComponent<ConstantForce>();
                forceDirection = new Vector3(0,-2,0);
                cForce.force = forceDirection;
                
            }

            if (val == 2)
            {
                cForce = GetComponent<ConstantForce>();
                forceDirection = new Vector3(0,-4,0);
                cForce.force = forceDirection;
            
            }
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
    }

    public bool haveLoggedConnection;
    public bool haveMadeOneConnection;
    public string firstConnection;
    public bool haveMadeSecondConnection;
    public string secondConnection;
    //public GameObject UIObject;
    //public Renderer objectRenderer; this is used to change whisker color/highlight bridge
    public int connectionsMade;
    private int conductorCollisions = 0; // Counter to track Conductor collisions
    private HashSet<string> currentConnections = new HashSet<string>();
    public GameObject UIObject;
    public UIScript uiScript;

    
    //BRIDGING DETECTION
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
                    UIObject.GetComponent<UIScript>().bridgesDetected++;
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
        UIObject.GetComponent<UIScript>().bridgesDetected--;
    }
    /*private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("Conductor"))
        {
            currentConnections.Add(collision.gameObject.name);

            if (!haveMadeOneConnection)
            {
                firstConnection = collision.gameObject.name;
                haveMadeOneConnection = true;
            }
            else if (collision.gameObject.name != firstConnection && !haveMadeSecondConnection)
            {
                haveMadeSecondConnection = true;
                secondConnection = collision.gameObject.name;
            }

            if (haveMadeOneConnection && haveMadeSecondConnection && !haveLoggedConnection)
            {
                connectionsMade++;
                print(connectionsMade);

                haveLoggedConnection = true;
            }
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Conductor"))
        {
            currentConnections.Remove(collision.gameObject.name);

            if (currentConnections.Count == 0)
            {
                ResetConnectionState();
            }
        }
    }

    private void ResetConnectionState()
    {
        haveMadeOneConnection = false;
        firstConnection = "";
        haveMadeSecondConnection = false;
        secondConnection = "";
        haveLoggedConnection = false;
    }
     /*private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("Conductor"))
        {
            if (!haveMadeOneConnection)
            {
                firstConnection = collision.gameObject.name;
                haveMadeOneConnection = true;
            }
            else if (collision.gameObject.name != firstConnection && !haveMadeSecondConnection)
            {
                haveMadeSecondConnection = true;
                secondConnection = collision.gameObject.name;
            }

            conductorCollisions++;

            if (haveMadeOneConnection && haveMadeSecondConnection && !haveLoggedConnection)
            {
                connectionsMade++;
                print(connectionsMade);

                haveLoggedConnection = true;
            }
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Conductor"))
        {
            conductorCollisions--;

            if (haveLoggedConnection)//conductorCollisions <= 0 && haveLoggedConnection)
            {
                if (connectionsMade > 0)
                {
                    connectionsMade--; // Optional: uncomment if you need to decrement
                    print(connectionsMade);    
                }

                ResetConnectionState();
            }
        }
    }

    private void ResetConnectionState()
    {
        haveMadeOneConnection = false;
        firstConnection = "";
        haveMadeSecondConnection = false;
        secondConnection = "";
        haveLoggedConnection = false;

        // Reset color to yellow
        //objectRenderer.material.color = Color.yellow;
    }*/

    /*private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("Conductor"))
        {
            if (!haveMadeOneConnection)
            {
                firstConnection = collision.gameObject.name;
                haveMadeOneConnection = true;
            }
            else if (collision.gameObject.name != firstConnection && !haveMadeSecondConnection)
            {
                haveMadeSecondConnection = true;
                secondConnection = collision.gameObject.name;

                if (!haveLoggedConnection)
                {
                    connectionsMade++;
                    print(connectionsMade);
                    //demoScript.connectionsMade++;
                    //demoScript.connectionsPerRun++;
                    //objectRenderer.material.color = Color.white; // Change color to white
                    haveLoggedConnection = true;
                }
            }

            conductorCollisions++;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Conductor"))
        {
            conductorCollisions--;

            if (conductorCollisions <= 0 && haveLoggedConnection)
            {
                if (connectionsMade > 0)//demoScript.connectionsMade > 0)
                {
                    //demoScript.connectionsMade--;
                }

                ResetConnectionState();
            }
        }
    }

    private void ResetConnectionState()
    {
        haveMadeOneConnection = false;
        firstConnection = "";
        haveMadeSecondConnection = false;
        secondConnection = "";
        haveLoggedConnection = false;
        //objectRenderer.material.color = Color.yellow; // Reset color to yellow
    }*/
}

