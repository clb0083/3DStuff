using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using UnityEngine.UI;
using System.Data;
using System.Linq;
using System.IO;
 // Add this namespace for LINQ functionality

public class WhiskerControl : MonoBehaviour
{
    public Dropdown gravity;
    private ConstantForce cForce;
    private Vector3 forceDirection;
    public int selectedIndex = 0;
    public Material targetMaterial;
    public int bridges = 0;
    bool confirmGravity; // used to tell program if gravity has been added or not.
    private List<WhiskerData> bridgedWhiskers = new List<WhiskerData>();
    public GameObject UIObject;
    public UIScript uiScript;
    // Add new fields for file path and file name input and save button
    public TMP_InputField filePathInputField;
    public TMP_InputField fileNameInputField;
    public Button saveButton;
    public string directoryPath;
    public string fileName;
 
    
    // Start is called before the first frame update
    void Start()
    {
        uiScript = UIObject.GetComponent<UIScript>();
        //Initialize the dictionary for the conductor pad NEW NEW NEW
        if (!bridgesPerConductor.ContainsKey(gameObject.name))
        {
            bridgesPerConductor[gameObject.name] = 0;
        }
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
    public static Dictionary<string, int> bridgesPerConductor = new Dictionary<string, int>(); 
    private Renderer objectRenderer; // for highlighting
    public Color bridgeDetectedColor = Color.red; // for highlighting
    public Color defaultColor = Color.white; // for highlighting
    public HashSet<string> currentConnections = new HashSet<string>();
    public Color[] colors;
    private Dictionary<GameObject, int> triggerInteractionCounts = new Dictionary<GameObject, int>();

    // BRIDGING DETECTION ORIGINAL
    private void  OnTriggerStay(Collider trigger) 
    {
        if (trigger.gameObject.CompareTag("ConductorTrigger")) 
        {
            // Add the conductor to the set of current connections
            currentConnections.Add(trigger.gameObject.name);
           
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

                    TrackBridgedWhiskers(gameObject);
                    
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

     public void TrackBridgedWhiskers(GameObject whisker)
    {
        Vector3 scale = whisker.transform.localScale;
        float length = scale.y*2; // Y axis is the length
        float diameter = (scale.x + scale.z) / 2; // Average of X and Z axes for diameter

        // Access material properties based on currentMaterial from UIScript
        UIScript.MaterialProperties currentProps = uiScript.materialProperties[uiScript.currentMaterial];

        float resistance = CalculateResistance(length, diameter, currentProps);

        WhiskerData data = new WhiskerData(length*1000, diameter*1000, resistance);
        bridgedWhiskers.Add(data);
        foreach (WhiskerData whiskerdata in bridgedWhiskers)
        {
            SaveBridgedWhiskerData(whiskerdata);
        }
    }
    private float CalculateResistance(float length, float diameter, UIScript.MaterialProperties materialProps)
    {
        float area = Mathf.PI * Mathf.Pow((diameter*1000)/2, 2);
        return materialProps.resistivity * (length*1000)/area;
    }

    private void SaveBridgedWhiskerData(WhiskerData data)
    {
        //string directoryPath = @"D:/Unity";//remove
        string filePath = Path.Combine(directoryPath, fileName + ".csv");

        try
        {
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }
            var lines = File.ReadAllLines(filePath).ToList();

        // Start modifying lines from the third line (index 2) to skip headers
        for (int i = 2; i < lines.Count; i++)
        {
            var columns = lines[i].Split(',').ToList();

            // Check if the bridged whisker data is already present in this line
            if (columns.Count < 8)
            {
                // Add empty cells if necessary
                while (columns.Count < 5)
                {
                    columns.Add(string.Empty);
                }
                // Append bridged whisker data
                columns.Add(data.Length.ToString());
                columns.Add(data.Diameter.ToString());
                columns.Add(data.Resistance.ToString());

                // Join the columns back into a single line
                lines[i] = string.Join(",", columns);

                // Write back to the file
                File.WriteAllLines(filePath, lines);

                Debug.Log($"Bridged whisker data saved successfully to {filePath}");
                return; // Exit after saving the data
            }
        }
                // If the lines do not exist, append them
                lines.Add($",,,,,{data.Length/1000},{data.Diameter/1000},{data.Resistance/1000}");
            
        File.WriteAllLines(filePath, lines);
            /*using (StreamWriter writer = new StreamWriter(filePath, true))
            {
                writer.WriteLine($",,,,,{data.Length},{data.Diameter},{data.Resistance}");
            }*/
            Debug.Log($"Bridged whisker data saved successfully to {filePath}");
        }
        catch (Exception ex)
        {
            Debug.LogError($"Failed to save bridged whisker data: {ex.Message}");
        }
    }

    public void SaveButtonClicked()
    {
        directoryPath = filePathInputField.text;
        fileName = fileNameInputField.text;

        // Save all the bridged whiskers data to the specified path
        /*foreach (WhiskerData data in bridgedWhiskers)
        {
            SaveBridgedWhiskerData(data);
        }*/
    }

    public class WhiskerData
    {
        public float Length { get; set; }
        public float Diameter { get; set; }
        public float Resistance { get; set; }

        public WhiskerData(float length, float diameter, float resistance)
        {
            Length = length;
            Diameter = diameter;
            Resistance = resistance;
        }
    }
}
