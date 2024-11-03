/*AU Team 1 (SP & SU 2024) used the help of Auburn graduate student Jake Botello
to learn Unity and C# in integrating design ideas. The team applied background
knowledge of MATLAB and C++ coding languages to develop various tools and
functions used throughout this script. ChatGPT was also used as a troubleshooting
reference.*/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

//Handles most of script that goes along with the whiskers, such as setting colliders and handling their collisions.
public class WhiskerControl : MonoBehaviour
{
    public Dropdown gravity;
    private ConstantForce cForce;
    private Vector3 forceDirection;
    public int selectedIndex = 0;
    public Material targetMaterial;
    public int bridges = 0;
    public bool confirmGravity;
    public GameObject UIObject;
    public UIScript uiScript;
    public TMP_InputField filePathInputField;
    public TMP_InputField fileNameInputField;
    public Button saveButton;
    public string directoryPath;
    public string fileName;
    public InputField customGravityInputX;
    public InputField customGravityInputY;
    public InputField customGravityInputZ;


    //Initializes the UiScript
    void Start()
    {
        uiScript = UIObject.GetComponent<UIScript>();
        if (uiScript == null)
        {
            Debug.LogError("UIScript not found on UIObject.");
        }
        if (!bridgesPerConductor.ContainsKey(gameObject.name))
        {
            bridgesPerConductor[gameObject.name] = 0;
        }
    }

    public float detectionRadius = 0.5f;
    public float rayDistance = 1.0f;
    public LayerMask conductorLayer;

    //Turns on the gravity
    void Update()
    {
        if (confirmGravity)
        {
            GetGravitySelection(gravity.value);
        }
    }

    //Confirm Gravity Button
    public void ConfirmButtonPressed()
    {
        GetGravitySelection(gravity.value);
        confirmGravity = true;
        uiScript.ReloadWhiskersButton();
        UIObject.GetComponent<UIScript>().startSim = true;
    }

    //Gets gravity selection from dropdown
    public void GetGravitySelection(int val)
    {
        ApplyGravity(val);
    }

    //Applys correct gravity 
    public void ApplyGravity(int val)
    {
        // Find all objects with the tag "whiskerClone"
        GameObject[] objectsWithTag = GameObject.FindGameObjectsWithTag("whiskerClone");

        Vector3 forceDirection = Vector3.zero;

        // Set default values to "0" if the InputFields are null or empty
        if (customGravityInputX == null || string.IsNullOrEmpty(customGravityInputX.text))
        {
            customGravityInputX.text = "0";
        }

        if (customGravityInputY == null || string.IsNullOrEmpty(customGravityInputY.text))
        {
            customGravityInputY.text = "0";
        }

        if (customGravityInputZ == null || string.IsNullOrEmpty(customGravityInputZ.text))
        {
            customGravityInputZ.text = "0";
        }

        // Validate the input fields
        if (customGravityInputX != null && customGravityInputY != null && customGravityInputZ != null)
        {
            if (float.TryParse(customGravityInputX.text, out float CGIX) &&
                float.TryParse(customGravityInputY.text, out float CGIY) &&
                float.TryParse(customGravityInputZ.text, out float CGIZ))
            {
                // Proceed with gravity application
                switch (val)
                {
                    case 0:
                        forceDirection = new Vector3(0, -100, 0); 
                        // This accleration is 0.1m/s^2 any faster and the whisker fly through the board

                        break;
                    case 1:
                        forceDirection = new Vector3(0, -20, 0);
                        // This accleration is 0.02m/s^2 any faster and the whisker fly through the board
                        break;
                    case 2:
                        forceDirection = new Vector3(0, -40, 0);
                        // This accleration is 0.04m/s^2 any faster and the whisker fly through the board
                        break;
                    case 3:
                        forceDirection = new Vector3(CGIX*10, CGIY*10, CGIZ*10);
                        break;
                    default:
                        Debug.LogWarning("Invalid gravity value provided.");
                        return;
                }

                // Apply the force to each object
                foreach (GameObject obj in objectsWithTag)
                {
                    ConstantForce cForce = obj.GetComponent<ConstantForce>() ?? obj.AddComponent<ConstantForce>();
                    cForce.force = forceDirection;
                }
            }
            else
            {
                Debug.LogError("Failed to parse custom gravity inputs.");
            }
        }
        else
        {
            Debug.LogError("One or more custom gravity input fields are not assigned.");
        }
    }

    //Resets gravity /ResetButton
    public void ResetGravity()
    {
        GameObject[] objectsWithTag = GameObject.FindGameObjectsWithTag("whiskerClone");
        foreach (GameObject obj in objectsWithTag)
        {
            cForce = GetComponent<ConstantForce>();
            forceDirection = new Vector3(0, 0, 0);
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
    private Renderer objectRenderer;
    public Color bridgeDetectedColor = Color.red;
    public Color defaultColor = Color.white;
    public HashSet<string> currentConnections = new HashSet<string>();
    public Color[] colors;
    private Dictionary<GameObject, int> triggerInteractionCounts = new Dictionary<GameObject, int>();
    public bool haveBridgedBefore;

    // BRIDGING DETECTION
    private void OnTriggerStay(Collider trigger)
    {
        if (trigger.gameObject.CompareTag("ConductorTrigger"))
        {
            currentConnections.Add(trigger.gameObject.name);

            if (currentConnections.Count >= 2)
            {
                if (!haveLoggedConnection && !haveBridgedBefore)
                {
                    Transform visualChild = transform.Find("visual");
                    Renderer childRenderer = visualChild.GetComponent<Renderer>();

                    objectRenderer = GetComponent<Renderer>();
                    bridgesPerConductor[gameObject.name]++;
                    UIObject.GetComponent<UIScript>().bridgesDetected++;
                    UIObject.GetComponent<UIScript>().bridgesPerRun++;
                    childRenderer.material.color = bridgeDetectedColor;
                    haveLoggedConnection = true;

                    TrackBridgedWhiskers(gameObject, currentConnections.ToList());
                    haveBridgedBefore = true; //uncomment for limit of one bridge.

                }
            }
        }
    }

    private void OnTriggerExit(Collider trigger)
    {
        if (trigger.gameObject.CompareTag("ConductorTrigger"))
        {
            currentConnections.Remove(trigger.gameObject.name);

            if (currentConnections.Count < 2 && haveLoggedConnection)
            {
                ResetConnectionState();
            }
        }
    }

    //Resets connection whenever the whisker stops being in contact
    private void ResetConnectionState()
    {
        Transform visualChild = transform.Find("visual");
        Renderer childRenderer = visualChild.GetComponent<Renderer>();

        currentConnections.Clear();
        haveLoggedConnection = false;
        bridgesPerConductor[gameObject.name]--;
        childRenderer.material.color = new Color(1.0f, 0.647f, 0.0f);
    }

    //Saves Bridged Whiskers
    public void TrackBridgedWhiskers(GameObject whisker, List<string> conductorNames)
    {
        //Debug.Log($"TrackBridgedWhiskers called for whisker {whisker.name}");

        Vector3 scale = whisker.transform.localScale;
        float length = scale.y * 2;
        float diameter = (scale.x + scale.z) / 2;

        UIScript.MaterialProperties currentProps = uiScript.materialProperties[uiScript.currentMaterial];

        float resistance = CalculateResistance(length, diameter, currentProps);

        // Extract whisker number from whisker's name
        string whiskerName = whisker.name;
        int whiskerNumber = int.Parse(whiskerName.Split('_')[1]);

        //Get conductor name and remove _ColliderCopy from names
        string conductor1 = conductorNames.Count > 0 ? CleanConductorName(conductorNames[0]) : "";
        string conductor2 = conductorNames.Count > 1 ? CleanConductorName(conductorNames[1]) : "";

        // Collect bridged whisker data
        WhiskerData data = new WhiskerData(whiskerNumber, length * 1000, diameter * 1000, resistance, uiScript.simIntComplete, conductor1, conductor2);

        //Add to datamangers bridgedwhiskerdata list
        DataManager.Instance.bridgedWhiskersData.Add(data);

        Debug.Log($"Bridged whisker added: {data.WhiskerNumber}");


        // Set the tag of the whisker to "bridgedWhisker"
        whisker.tag = "bridgedWhisker";

        // Handle critical pairs
        string pairKey = uiScript.CreatePairKey(conductor1, conductor2);
        if (uiScript.criticalPairs.Contains(pairKey))
        {
            // If this is a critical pair, save it separately
            SaveCriticalBridgedWhiskerData(data);
        }
    }

    //Calculates resistances
    private float CalculateResistance(float length, float diameter, UIScript.MaterialProperties materialProps)
    {
        float area = Mathf.PI * Mathf.Pow((diameter * 1000) / 2, 2);
        return materialProps.resistivity * (length * 1000) / area;
    }

    //Saves bridged whisker datas into the CSV files.
    private void SaveBridgedWhiskerData(WhiskerData data)
    {
        // Save to the custom CSV file specified by directoryPath and fileName
        string directoryPath = uiScript.whiskerControl.directoryPath;
        string customFilePath = Path.Combine(directoryPath, uiScript.whiskerControl.fileName + ".csv");

        // Save to the bridgeOutput.csv file in Application.dataPath
        string fixedFilePath = Path.Combine(Application.dataPath, "bridgeOutput.csv");

        // Save to both files
        SaveBridgedWhiskerDataToFile(data, customFilePath);
        SaveBridgedWhiskerDataToFile(data, fixedFilePath);
    }

    // Helper method to save data to a specified file
    private void SaveBridgedWhiskerDataToFile(WhiskerData data, string filePath)
    {
        try
        {
            if (!Directory.Exists(Path.GetDirectoryName(filePath)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(filePath));
            }

            var lines = File.Exists(filePath) ? File.ReadAllLines(filePath).ToList() : new List<string>();

            // If the file is empty, write the headers
            if (lines.Count == 0)
            {
                // Write the column titles
                lines.Add("All Whiskers,,,,,Bridged Whiskers");
                lines.Add("Whisker #,Length (um),Width (um),Resistance (ohm),Iteration,Whisker #,Length (um),Width (um),Resistance (ohm),Iteration,Conductor 1,Conductor 2");
            }

            // Ensure each line has at least 12 columns
            for (int i = 0; i < lines.Count; i++)
            {
                var columns = lines[i].Split(',').ToList();
                while (columns.Count < 12)
                {
                    columns.Add(string.Empty);
                }
                lines[i] = string.Join(",", columns);
            }

            // Find the line where the whisker number matches and insert bridged data
            bool dataInserted = false;

            // The bridged whisker number
            int bridgedWhiskerNumber = data.WhiskerNumber;

            // Start from line 2 because lines 0 and 1 are headers
            for (int i = 2; i < lines.Count; i++)
            {
                var columns = lines[i].Split(',').ToList();

                // Check if the All Whiskers column has this whisker number
                if (columns[0] == bridgedWhiskerNumber.ToString())
                {
                    // Fill in the bridged whisker data starting from column index 5
                    columns[5] = data.WhiskerNumber.ToString();
                    columns[6] = data.Length.ToString();
                    columns[7] = data.Diameter.ToString();
                    columns[8] = data.Resistance.ToString();
                    columns[9] = data.SimulationIndex.ToString();
                    columns[10] = data.Conductor1;
                    columns[11] = data.Conductor2;

                    lines[i] = string.Join(",", columns);
                    dataInserted = true;
                    break;
                }
            }

            if (!dataInserted)
            {
                // If no matching whisker number is found, append a new line with empty All Whiskers data
                string newLine = $",,,,,{data.WhiskerNumber},{data.Length},{data.Diameter},{data.Resistance},{data.SimulationIndex},{data.Conductor1},{data.Conductor2}";
                lines.Add(newLine);
            }

            // Write the updated lines back to the file
            File.WriteAllLines(filePath, lines);

            Debug.Log($"Bridged whisker data saved successfully to {filePath}");
        }
        catch (Exception ex)
        {
            Debug.LogError($"Failed to save bridged whisker data to {filePath}: {ex.Message}");
        }
    }

    // Saves the directory/filename
    public void SaveButtonClicked()
{
    directoryPath = filePathInputField.text;
    fileName = fileNameInputField.text;
}

    private void SaveCriticalBridgedWhiskerData(WhiskerData data) //crit pairs UI
    {
        // Add to DataManager's critical bridged whiskers list
        DataManager.Instance.criticalBridgedWhiskersData.Add(data);

        // Optionally, you can log or handle critical bridged whiskers here
        Debug.Log($"Critical bridged whisker detected: {data.WhiskerNumber}");
    }

    private string CleanConductorName(string name)
    {
        return name.Replace("_ColliderCopy", "");
    }    
}
