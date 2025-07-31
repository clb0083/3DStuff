/*AU Team 1 (SP & SU 2024) used the help of Auburn graduate student Jake Botello
to learn Unity and C# in integrating design ideas. The team applied background
knowledge of MATLAB and C++ coding languages to develop various tools and
functions used throughout this script. ChatGPT was also used as a troubleshooting
reference.*/

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Unity.VisualScripting;
using System.Data.Common;



//This is the main script that controls many of the objects on the interface, namely the user inputs.
public class UIScript : MonoBehaviour
{
    private System.Random rand = new System.Random();
    //UI elements
    public TMP_InputField lengthMu;
    public TMP_InputField widthMu;
    public TMP_InputField lengthSigma;
    public TMP_InputField widthSigma;
    public TMP_InputField numWhiskers;
    public TMP_InputField xCoord;
    public TMP_InputField yCoord;
    public TMP_InputField zCoord;
    public TMP_InputField numIterations;
    public TMP_InputField shockAmp;
    public TMP_InputField shockFreq;
    public TMP_InputField shockDur;
    public TMP_InputField shockStartTime;
    public TMP_InputField vibrAmp;
    public TMP_InputField vibrFreq;
    public TMP_InputField vibrDur;
    public TMP_InputField vibrStartTime; 
    public TMP_InputField material_input;
    public TMP_InputField WhiskerSpawnPointX;
    public TMP_InputField WhiskerSpawnPointY;
    public TMP_InputField WhiskerSpawnPointZ;
    public TMP_InputField totalRuns;
    public TMP_InputField CircuitRotateX;
    public TMP_InputField CircuitRotateY;
    public TMP_InputField CircuitRotateZ;
    public TMP_InputField SpinRateX;
    public TMP_InputField SpinRateY;
    public TMP_InputField SpinRateZ;
    public TMP_InputField wallHeightInput;
    public TMP_Dropdown whiskMat;
    public TMP_Dropdown distributionDropdown;
    public Toggle isShockActiveToggle;
    public Toggle isVibrationActiveToggle;
    public Toggle electrostaticForceToggle;

    public GameObject whisker;
    public TextMeshProUGUI totalBridges;
    public int bridgesDetected;
    public TextMeshProUGUI bridgesEachRun;
    public int bridgesPerRun;
    public TextMeshProUGUI errorMessage;
    public float simtimeElapsed;
    private float nextLogTime = 0f;
    public bool startSim = false;
    public int simIntComplete = 1;
    public TMP_InputField simTimeLength;
    public string defaultTime = "10"; // Default sim time length 
    public float moveSpeed = 5f;
    public DistributionType distributionType = DistributionType.Lognormal;
    public bool UIisOn = true;
    public GameObject UIui;
    public bool GridIsOn = true;
    public GameObject grid;
    public TextMeshProUGUI iterationCounter;
    public TextMeshProUGUI iterationCompleteMessage;
    public TriggerControl triggerControl;
    public Dictionary<MaterialType, MaterialProperties> materialProperties = new Dictionary<MaterialType, MaterialProperties>()
    { //density (kg/um^3), resistivity (ohm*um), coefficient of friction (unitless)
        { MaterialType.Tin, new MaterialProperties(7.3e-15f, 1.09e-1f, 0.32f) },
        { MaterialType.Zinc, new MaterialProperties(7.14e-15f, 5.9e-2f, 0.6f) },
        { MaterialType.Cadmium, new MaterialProperties(8.65e-15f, 7.0e-2f, 0.5f) }
    };
    public MaterialType currentMaterial = MaterialType.Tin;
    private List<float> lengths;
    private List<float> widths;
    private List<float> volumes;
    private List<float> masses;
    private List<float> resistances;
    public WhiskerControl whiskerControl;   //new
    public bool isVibrationActive = false;
    public bool VibrIterationComplete = false; // Vibration completed for that iteration
    public bool isShockActive = false;
    public bool ShockIterationComplete = false; // Shock completed for that iteration
    public bool RotateSpinToggle = false;
    public VibrationManager vibrationManager;
    public ShockManager shockManager;
    public float vibrTimer = 0f;
    public float shockTimer = 0f;
    public float shockPressInterval = 2f;
    public SimulationController simulationController; //reference to the simulation controller script
    private int whiskerCounter; //variable to track whisker numbers
    public FunctionInputHandler functionHandler; // Function Input Math script for Acceleration and Graph
    private float simStartTime;

    public ScreenshotHandler screenshotManager; // Reference to the Screenshot script

    //references for critical pair UI
    public TMP_Dropdown conductor1Dropdown;
    public TMP_Dropdown conductor2Dropdown;
    public Button addPairButton;
    public Button removePairButton;
    public ScrollRect criticalPairsScrollView;
    public GameObject criticalPairItemPrefab; //prefab for list items
    public HashSet<string> criticalPairs = new HashSet<string>(); //data structure to store crit pairs
    private List<GameObject> criticalPairItems = new List<GameObject>(); // list to keep track of UI items
    public Button refreshDropdownsButton;


    private float CircuitRotateXValue;
    private float CircuitRotateYValue;
    private float CircuitRotateZValue;
    private GameObject circuitBoard;
    public WallCreator wallCreator;
    public CircuitBoardFinder circuitBoardFinder;
    public WhiskerAcceleration WhiskerAcceleration;

    private DataManager dataManager;
    private bool dataWritten = false;

    //Sets the lists for the dimensions/data to be stored in, as well as sets material properties from the dropdown.
    void Start()
    {
        simTimeLength.text = defaultTime;
        if (electrostaticForceToggle != null && whiskerControl != null)
        {
            electrostaticForceToggle.isOn = whiskerControl.applyElectrostaticForce;
        }

        whiskerCounter = 1; //initialize the counter to 1 when the script starts

        lengths = new List<float>();
        widths = new List<float>();
        volumes = new List<float>();
        masses = new List<float>();
        resistances = new List<float>();

        whiskMat.onValueChanged.AddListener(delegate
        {
            WhiskMatDropdownValueChanged(whiskMat);
        });

        UpdateMaterialPropertiesUI(currentMaterial);

        Time.fixedDeltaTime = 0.005f;
        Physics.defaultSolverIterations = 10;
        Physics.defaultSolverVelocityIterations = 10;

        StartCoroutine(DelayedPopulateConductorDropdowns()); //delay to call code

        addPairButton.onClick.AddListener(OnAddPairButtonClicked);
        removePairButton.onClick.AddListener(OnRemovePairButtonClicked);
        refreshDropdownsButton.onClick.AddListener(OnRefreshButtonClicked);
        electrostaticForceToggle.onValueChanged.AddListener(OnElectrostaticToggleChanged);

        if (electrostaticForceToggle != null && whiskerControl != null)
        {
            electrostaticForceToggle.isOn = whiskerControl.applyElectrostaticForce;
        }

        circuitBoard = GameObject.FindGameObjectWithTag("CircuitBoard");

        dataManager = DataManager.Instance;
        if (dataManager == null)
        {
            Debug.LogError("dataManager instance not found.");
        }

        if (whiskerControl == null)
        {
            whiskerControl = FindObjectOfType<WhiskerControl>();
            if (whiskerControl == null)
            {
                Debug.LogError("WhiskerControl not found in the scene.");
            }
        }
    }

    //Controls the dropdown for material selection.
    public void UpdateMaterialPropertiesUI(MaterialType materialType)
    {
        switch (materialType)
        {
            case MaterialType.Tin:
                whiskMat.captionText.text = "Tin";
                break;
            case MaterialType.Zinc:
                whiskMat.captionText.text = "Zinc";
                break;
            case MaterialType.Cadmium:
                whiskMat.captionText.text = "Cadmium";
                break;
            default:
                break;
        }

        Debug.Log($"UI updated for material: {materialType}");
    }

    //Method to handle whiskMat dropdown value change
    void WhiskMatDropdownValueChanged(TMP_Dropdown change)
    {
        currentMaterial = (MaterialType)change.value;
        UpdateMaterialPropertiesUI(currentMaterial);

        StartCoroutine(CloseDropdownAfterDelay());
    }

    //Coroutine to close dropdown after a short delay
    private IEnumerator CloseDropdownAfterDelay()
    {
        yield return null;
        TMP_Dropdown dropdown = whiskMat.GetComponent<TMP_Dropdown>();
        dropdown.Hide();
    }

    //Controls the iteration counters/bridge counters/applys shock/vibration throughout simulation
    void Update()
    {
        totalBridges.text = "Total Bridges: " + bridgesDetected.ToString(); //remove if needed.
        bridgesEachRun.text = "Bridges for Current Run: " + bridgesPerRun.ToString();

        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        Vector3 moveDirection = new Vector3(horizontalInput, 0f, verticalInput).normalized;
        transform.Translate(moveDirection * moveSpeed * Time.deltaTime);

        // If statement for start simulation button
        if (startSim)
        {
            CircuitBoardFinder circuitBoardFinder = FindObjectOfType<CircuitBoardFinder>();

            // Start Shock at start time, complete for one iteration
            if (float.TryParse(shockStartTime.text, out float shockStartTimeValue) && !ShockIterationComplete)
            {
                shockTimer += Time.deltaTime;
                if (shockTimer >= shockStartTimeValue)
                {
                    shockManager.shockPressed();
                    ShockIterationComplete = true;
                }
            }

            // Start Vibration at start time, complete for one iteration
            if (float.TryParse(vibrStartTime.text, out float vibrStartTimeValue) && !VibrIterationComplete)
            {
                vibrTimer += Time.deltaTime;
                if (vibrTimer >= vibrStartTimeValue)
                {
                    vibrationManager.vibratePressed();
                    VibrIterationComplete = true;
                }
            }

            // sim time steps with frame update
            simtimeElapsed += Time.deltaTime;
            if (float.TryParse(simTimeLength.text, out float simTimeThresh) && simTimeThresh > 0)
            {
                
            }
            else
            {
                Debug.Log("Invalid Time. Please enter a number > 0.");
            }

            // if statement for log time count
            if (simtimeElapsed >= nextLogTime)
            {
                Debug.Log("Sim time elapsed: " + simtimeElapsed.ToString("F2") + " seconds");
                nextLogTime += 2f; //Next log in 2 seconds
            }

            // if statement for sim iterations to continue
            if (simIntComplete <= Convert.ToInt32(totalRuns.text) - 1)
            {
                iterationCounter.text = "Iteration Counter: " + simIntComplete.ToString();
                
                // restart sim time, reload whiskers and add sim iteration count
                if (simtimeElapsed > simTimeThresh)
                {
                    simIntComplete++;
                    ReloadWhiskersButton();

                    // Start sim time and apply force to new whiskers
                    simStartTime = Time.fixedTime;

                    foreach (GameObject whisk in GameObject.FindGameObjectsWithTag("whiskerClone"))
                    {
                        WhiskerAcceleration forceScript = whisk.GetComponent<WhiskerAcceleration>();
                        if (forceScript != null)
                        {
                            forceScript.applyForce = true;
                            forceScript.simTimeStart = simStartTime;
                        }
                    }

                    simtimeElapsed = 0f;
                    VibrIterationComplete = false;
                    vibrTimer = 0f;
                    ShockIterationComplete = false;
                    shockTimer = 0f;

                    if (screenshotManager != null)
                    {
                        screenshotManager.OnIterationEnd();
                    }
                    if (circuitBoardFinder != null)
                    {
                        // Toggle the RotateSpinToggle
                        circuitBoardFinder.RotateSpinToggle.isOn = !circuitBoardFinder.RotateSpinToggle.isOn;
                    }
                }
            }
            // last sim iteration
            else if (simIntComplete == Convert.ToInt32(totalRuns.text))
            {
                iterationCounter.text = "Iteration Counter: " + simIntComplete.ToString();
                if (simtimeElapsed > simTimeThresh)
                {
                    foreach (GameObject whisk in GameObject.FindGameObjectsWithTag("whiskerClone"))
                    {
                        WhiskerAcceleration forceScript = whisk.GetComponent<WhiskerAcceleration>();
                        if (forceScript != null)
                        {
                            forceScript.applyForce = false;
                            
                        }
                    }

                    if (screenshotManager != null)
                    {
                        screenshotManager.OnIterationEnd();
                    }
                    iterationCompleteMessage.text = "Simulation Complete!";
                    startSim = false;

                    if (!dataWritten)
                    {
                        WriteDataToCSV();
                        dataWritten = true; // Prevents multiple writes
                    }
                }
            }
        }
    }


    //Resets the simulation counters back to original value
    public void resetSim()
    {
        simIntComplete = 1;
        simtimeElapsed = 0;
        VibrIterationComplete = false;
        vibrTimer = 0f;
        ShockIterationComplete = false;
        shockTimer = 0f;
        bridgesDetected = 0;
        bridgesPerRun = 0;
        iterationCompleteMessage.text = "";
        iterationCounter.text = "";
        startSim = false;
        nextLogTime = 0;
    }

    //Takes in Mu and Sigma values for Length to generate values.
    public float LengthDistributionGenerate()
    {
        float mu = float.Parse(lengthMu.text);
        float sigma = float.Parse(lengthSigma.text);
        float lengthVal;

        if (distributionType == DistributionType.Lognormal)
        {
            lengthVal = GenerateLogNormalValue(mu, sigma);
        }
        else
        {
            lengthVal = GenerateNormalValue(mu, sigma);
        }

        return lengthVal;
    }

    //Takes in Mu and Sigma values for Width to generate values.
    public float WidthDistributionGenerate()
    {
        float mu_log = float.Parse(widthMu.text);
        float sigma_log = float.Parse(widthSigma.text);
        float widthVal;


        if (distributionType == DistributionType.Lognormal)
        {
            widthVal = GenerateLogNormalValue(mu_log, sigma_log);
        }
        else
        {
            widthVal = GenerateNormalValue(mu_log, sigma_log);
        }

        return widthVal;
    }

    //Generates a lognormal value based off mu/sigma inputs.
    private float GenerateLogNormalValue(float mu_log, float sigma_log)
    {
        float normalVal = RandomFromDistribution.RandomNormalDistribution(mu_log, sigma_log);
        float logNormalVal = Mathf.Exp(normalVal);
        return logNormalVal;
    }

    //Generates a normal value based off mu/sigma inputs.
    private float GenerateNormalValue(float mu_norm, float sigma_norm)
    {
        return RandomFromDistribution.RandomNormalDistribution(mu_norm, sigma_norm);
    }

    //Handles many error messages and generates whiskers
    public void MakeWhiskerButton()
    {
        //error handling | Limits are subject to change
        float mu_log = float.Parse(widthMu.text);
        float sigma_log = float.Parse(widthSigma.text);
        float mu = float.Parse(lengthMu.text);
        float sigma = float.Parse(lengthSigma.text);
        float numWhiskersToCreate = float.Parse(numWhiskers.text);
        float numberIterations = float.Parse(totalRuns.text);

        float x = Convert.ToSingle(xCoord.text);
        float y = Convert.ToSingle(yCoord.text);
        float z = Convert.ToSingle(zCoord.text);

        if (distributionType == DistributionType.Lognormal)
        {
            if (mu_log > 9 || mu_log < 0)
            {
                SetErrorMessage("Width Mu value outside acceptable range.");
                return;
            }
            if (sigma_log > 4 || sigma_log < 0)
            {
                SetErrorMessage("Width Sigma value is outside acceptable range.");
                return;
            }
            if (mu > 20 || mu < 0)
            {
                SetErrorMessage("Length Mu value outside acceptable range.");
                return;
            }
            if (sigma > 15 || sigma < 0)
            {
                SetErrorMessage("Length Sigma value is outside acceptable range.");
                return;
            }
        }
        else
        {
            if (mu_log > 10000 || mu_log < 0)
            {
                SetErrorMessage("Width Mu value outside acceptable range.");
                return;
            }
            if (sigma_log > 10000 || sigma_log < 0)
            {
                SetErrorMessage("Width Sigma value is outside acceptable range.");
                return;
            }
            if (mu > 30000 || mu < 0)
            {
                SetErrorMessage("Length Mu value outside acceptable range.");
                return;
            }
            if (sigma > 10000 || sigma < 0)
            {
                SetErrorMessage("Length Sigma value is outside acceptable range.");
                return;
            }
        }

        if (numWhiskersToCreate > 2000 || numWhiskersToCreate < 0 || !Mathf.Approximately(numWhiskersToCreate, Mathf.Round(numWhiskersToCreate)))
        {
            SetErrorMessage("Whisker count too high or invalid. Limit is 2000 and must be a positive integer.");
            return;
        }

        if (x < 0 || y < 0 || z < 0)
        {
            SetErrorMessage("Coordinates cannot be negative.");
            return;
        }

        if (numberIterations < 0 || !Mathf.Approximately(numberIterations, Mathf.Round(numberIterations)))
        {
            SetErrorMessage("Iteration value must be a positive integer.");
            return;
        }

        lengths.Clear();
        widths.Clear();
        volumes.Clear();
        masses.Clear();
        resistances.Clear();


        GameObject whiskerSpawnPoint = GameObject.Find("WhiskerSpawnPoint");

        if (whiskerSpawnPoint == null)
        {
            SetErrorMessage("WhiskerSpawnPoint not found in the scene.");
            return;
        }
        // Move the WhiskerSpawnPoint to (0, 0, 0) before starting
        whiskerSpawnPoint.transform.position = Vector3.zero;

        // Ensure input fields are not null
        if (WhiskerSpawnPointX == null || WhiskerSpawnPointY == null || WhiskerSpawnPointZ == null)
        {
            SetErrorMessage("One or more input fields are not assigned.");
            return;
        }

        // Read target position from input fields
        if (!float.TryParse(WhiskerSpawnPointX.text, out float WSPX) ||
            !float.TryParse(WhiskerSpawnPointY.text, out float WSPY) ||
            !float.TryParse(WhiskerSpawnPointZ.text, out float WSPZ))
        {
            SetErrorMessage("Invalid input for target positions.");
            return;
        }

        for (int i = 0; i < numWhiskersToCreate; i++)
        {
            //Generate dimensions and spawn position
            float diameter = WidthDistributionGenerate() / 1000;
            float length = LengthDistributionGenerate() / 1000;
            float spawnPointX = UnityEngine.Random.Range(-float.Parse(xCoord.text) * 10, float.Parse(xCoord.text) * 10);
            float spawnPointY = UnityEngine.Random.Range(1, Convert.ToInt32(yCoord.text) * 10);
            float spawnPointZ = UnityEngine.Random.Range(-float.Parse(zCoord.text) * 10, float.Parse(zCoord.text) * 10);

            Vector3 spawnPos = whiskerSpawnPoint.transform.position + new Vector3(spawnPointX, spawnPointY, spawnPointZ);

            if (whisker == null)
            {
                SetErrorMessage("Whisker prefab is not assigned.");
                return;
            }

            GameObject whiskerClone = Instantiate(whisker, spawnPos, Quaternion.Euler(UnityEngine.Random.Range(0, 360), UnityEngine.Random.Range(0, 360), UnityEngine.Random.Range(0, 360)));
            whiskerClone.transform.SetParent(whiskerSpawnPoint.transform);
            whiskerClone.tag = "whiskerClone";

            whiskerClone.name = $"whisker_{whiskerCounter}"; //unique name for each whisker

            // Set up Rigidbody, scaling, and physics properties
            Rigidbody whiskerRigidbody = whiskerClone.GetComponent<Rigidbody>();
            if (whiskerRigidbody == null)
            {
                whiskerRigidbody = whiskerClone.AddComponent<Rigidbody>();
            }

            //Calculates properties
            MaterialProperties currentProps = materialProperties[currentMaterial];
            float volume = Mathf.PI * Mathf.Pow(diameter / 2, 2) * length;
            float mass = volume * currentProps.density;
            float resistance = (currentProps.resistivity * length * 1000) / (Mathf.PI * Mathf.Pow(diameter * 1000 / 2, 2));

            //Sets a minimum mass limit
            if (mass < 1f)
            {
                whiskerRigidbody.mass = 0.22f;
                whiskerRigidbody.drag = 20f;
                whiskerRigidbody.angularDrag = 2f;
            }
            else
            {
                whiskerRigidbody.mass = mass;
                whiskerRigidbody.drag = 20f;
                whiskerRigidbody.angularDrag = 2f;
            }

            whiskerRigidbody.collisionDetectionMode = CollisionDetectionMode.Continuous;

            //UPSIZING WIDTHS
            Transform visual = whiskerClone.transform.Find("visual");
            Transform collider = whiskerClone.transform.Find("collider");
            if (diameter * 1000 < 10)
            {
                visual.localScale = new Vector3(10, 1, 10); // scales relative to the parent object 
            } // so this is saying if the diameter is less than 50, it takes the diameter of the orignal whisker and *5.
            else
            {
                visual.localScale = new Vector3(1.2f, 1, 1.2f);
            }

            collider.localScale = new Vector3(1, 1, 1);

            whiskerClone.transform.localScale = new Vector3(diameter, length / 2, diameter);
            lengths.Add(length);
            widths.Add(diameter);

            Collider whiskerCollider = whiskerClone.GetComponent<Collider>();
            if (whiskerCollider == null)
            {
                Debug.LogError("Whisker prefab must have a Collider component.");
                return;
            }

            //Gets physics material and sets its friction properties
            PhysicMaterial whiskerPhysicsMaterial = whiskerCollider.sharedMaterial;
            if (whiskerPhysicsMaterial == null)
            {
                whiskerPhysicsMaterial = new PhysicMaterial();
                whiskerCollider.sharedMaterial = whiskerPhysicsMaterial;
            }
            UpdatePhysicsMaterialFriction(whiskerPhysicsMaterial);

            // Applies the function to the force/acceleration script,
            // ApplyForce is false until simulation start
            WhiskerAcceleration forceScript = whiskerClone.GetComponent<WhiskerAcceleration>();
            if (forceScript == null)
            {
                forceScript.FunctionSource = functionHandler;
                forceScript.applyForce = false;
            }

            whiskerCounter++; //increment counter for next whisker

            // Debug log for verification, currently commented out to avoid log spam
            //Debug.Log($"Whisker created with material: {currentMaterial}, Density: {currentProps.density}, Mass: {mass}, Resistance: {resistance}");
        }
        // Move the WhiskerSpawnPoint to the desired target position after spawning
        Vector3 targetPosition = new Vector3(WSPX * 10, WSPY * 10, WSPZ * 10);
        whiskerSpawnPoint.transform.position = targetPosition;
    }

    //Updates the friction on the whiskers
    private void UpdatePhysicsMaterialFriction(PhysicMaterial material)
    {
        MaterialProperties currentProps = materialProperties[currentMaterial];
        material.staticFriction = currentProps.coefficientOfFriction;
        material.dynamicFriction = currentProps.coefficientOfFriction;
    }

    // Deletes old whiskers, makes new whiskers, then applies the acceleration/force waiting for simulation start
    public void ReloadWhiskersButton()
    {
        // Delete all old whiskers (both types)
        foreach (GameObject whisk in GameObject.FindGameObjectsWithTag("whiskerClone")
                                              .Concat(GameObject.FindGameObjectsWithTag("bridgedWhisker")))
        {
            Destroy(whisk);
        }

        bridgesPerRun = 0;
        whiskerCounter = 1;

        // Generate new whiskers
        MakeWhiskerButton();

    }

    public void OnElectrostaticToggleChanged(bool isOn)
    {
        if (whiskerControl != null)
        {
            whiskerControl.applyElectrostaticForce = isOn;
        }
    }


    //Function that sets the error message.
    public void SetErrorMessage(string message)
    {
        errorMessage.text = message;
        StartCoroutine(ClearErrorMessageAfterDelay(6f));
    }

    //Clears the error message
    private IEnumerator ClearErrorMessageAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        errorMessage.text = "";
    }

    //Saving whiskers data;
    public void SaveWhiskerData(WhiskerData data)
    {
        DataManager.Instance.allWhiskersData.Add(data);

        Debug.Log($"Whisker data saved: {data.WhiskerNumber}");
    }

    public void WriteDataToCSV()
    {
        // Paths for both files
        string customDirectoryPath = whiskerControl.directoryPath;
        string customFilePath = Path.Combine(customDirectoryPath, whiskerControl.fileName + ".csv");
        string fixedFilePath = Path.Combine(Application.dataPath, "bridgeOutput.csv");

        // Write data to custom file path with all data
        WriteDataToCSVFile(customFilePath, includeAllWhiskersData: true);
        //brideOutput.csv file with only bridged data
        WriteDataToCSVFile(fixedFilePath, includeAllWhiskersData: false);
    }

    private void WriteDataToCSVFile(string filePath, bool includeAllWhiskersData)
    {
        if (string.IsNullOrEmpty(filePath))
        {
            SetErrorMessage("Failed to save data - File path cannot be empty");
            return;
        }

        try
        {
            if (!Directory.Exists(Path.GetDirectoryName(filePath)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(filePath));
            }

            using (StreamWriter writer = new StreamWriter(filePath, false)) // Overwrite the file
            {
                // Get data from dataManager
                List<WhiskerData> allWhiskersData = DataManager.Instance.allWhiskersData;
                List<WhiskerData> bridgedWhiskersData = DataManager.Instance.bridgedWhiskersData;
                List<WhiskerData> criticalBridgedWhiskersData = DataManager.Instance.criticalBridgedWhiskersData;

                if (includeAllWhiskersData)
                {
                    // Write the headers
                    writer.WriteLine("All Whiskers,,,,,,Bridged Whiskers,,,,,,,,Critical Bridged Whiskers,,,,,,,,Simulation Inputs,");
                    writer.WriteLine("Whisker #,Length (um),Width (um),Resistance (ohm),Iteration,"
                        + ",Whisker #,Length (um),Diameter (um),Resistance (ohm),Iteration,Conductor 1,Conductor 2,"
                        + ",Whisker #,Length (um),Diameter (um),Resistance (ohm),Iteration,Conductor 1,Conductor 2,"
                        + ",Parameter,Value");

                    //collect sim inputs
                    List<KeyValuePair<string, string>> simulationInputs = GetSimulationInputs();


                    // Determine the maximum number of rows
                    int maxRows = Mathf.Max(allWhiskersData.Count, bridgedWhiskersData.Count, criticalBridgedWhiskersData.Count, simulationInputs.Count);

                    string gap = ",,";
                    string gap1 = ",,";
                    string gap3 = ",,";

                    // Loop through all rows
                    for (int i = 0; i < maxRows; i++)
                    {
                        string allWhiskerDataLine = "";
                        string bridgedWhiskerDataLine = "";
                        string criticalBridgedWhiskerDataLine = "";
                        string simulationInputLine = "";

                        bool hasData = false;

                        // Get All Whisker data if available
                        if (i < allWhiskersData.Count)
                        {
                            var whisker = allWhiskersData[i];
                            allWhiskerDataLine = $"{whisker.WhiskerNumber},{whisker.Length * 1000},{whisker.Width * 1000},{whisker.Resistance},{whisker.Iteration}";
                            hasData = true;
                        }
                        else
                        {
                            allWhiskerDataLine = ",,,,";
                        }

                        // Get Bridged Whisker data if available
                        if (i < bridgedWhiskersData.Count)
                        {
                            var bridgedWhisker = bridgedWhiskersData[i];
                            bridgedWhiskerDataLine = $"{bridgedWhisker.WhiskerNumber},{bridgedWhisker.Length},{bridgedWhisker.Diameter},{bridgedWhisker.Resistance},{bridgedWhisker.SimulationIndex},{bridgedWhisker.Conductor1},{bridgedWhisker.Conductor2}";
                            hasData = true;
                        }
                        else
                        {
                            bridgedWhiskerDataLine = ",,,,,,";
                        }

                        // Get Critical Bridged Whisker data if available
                        if (i < criticalBridgedWhiskersData.Count)
                        {
                            var criticalWhisker = criticalBridgedWhiskersData[i];
                            criticalBridgedWhiskerDataLine = $"{criticalWhisker.WhiskerNumber},{criticalWhisker.Length},{criticalWhisker.Diameter},{criticalWhisker.Resistance},{criticalWhisker.SimulationIndex},{criticalWhisker.Conductor1},{criticalWhisker.Conductor2}";
                            hasData = true;
                        }
                        else
                        {
                            criticalBridgedWhiskerDataLine = ",,,,,,";
                        }

                        // Get Simulation Input data if available
                        if (i < simulationInputs.Count)
                        {
                            var input = simulationInputs[i];
                            if (!string.IsNullOrWhiteSpace(input.Key) && !string.IsNullOrWhiteSpace(input.Value))
                            {
                                simulationInputLine = $"{input.Key},{input.Value}";
                                hasData = true;
                            }
                            else
                            {
                                simulationInputLine = ",";
                            }
                        }
                        else
                        {
                            simulationInputLine = ",";
                        }

                        // Add debug statements to trace values
                        Debug.Log($"Row {i + 1}:");
                        Debug.Log($"  AllWhiskerDataLine: '{allWhiskerDataLine}'");
                        Debug.Log($"  BridgedWhiskerDataLine: '{bridgedWhiskerDataLine}'");
                        Debug.Log($"  CriticalBridgedWhiskerDataLine: '{criticalBridgedWhiskerDataLine}'");
                        Debug.Log($"  SimulationInputLine: '{simulationInputLine}'");
                        Debug.Log($"  hasData: {hasData}");

                        //skip writing line if all data sections are empty
                        if (!hasData)
                        {
                            Debug.Log("  Skipping row because hasData is false.");
                            continue;
                        }

                        // Write the combined data to the CSV file
                        string combinedLine = $"{allWhiskerDataLine}{gap1}{bridgedWhiskerDataLine}{gap}{criticalBridgedWhiskerDataLine}{gap3}{simulationInputLine}";
                        writer.WriteLine(combinedLine);

                    }
                }
                else
                {
                    // Only write the Bridged Whiskers section
                    writer.WriteLine("Bridged Whiskers");
                    writer.WriteLine("Whisker #,Length (um),Diameter (um),Resistance (ohm),Iteration,Conductor 1,Conductor 2");

                    foreach (var bridgedWhisker in bridgedWhiskersData)
                    {
                        string bridgedWhiskerDataLine = $"{bridgedWhisker.WhiskerNumber},{bridgedWhisker.Length},{bridgedWhisker.Diameter},{bridgedWhisker.Resistance},{bridgedWhisker.SimulationIndex},{bridgedWhisker.Conductor1},{bridgedWhisker.Conductor2}";
                        writer.WriteLine(bridgedWhiskerDataLine);
                    }
                }
            }
            Debug.Log($"Data saved successfully to {filePath}");
        }
        catch (Exception ex)
        {
            Debug.LogError($"Failed to save data to {filePath}: {ex.Message}");
        }

        Debug.Log($"Number of bridged whiskers: {DataManager.Instance.bridgedWhiskersData.Count}");

    }

    public TextMeshProUGUI bridgesCount;
    public Text conductorName;

    //Updates bridge counter
    public void UpdateConductorBridge(string conductorName, int pingCount)
    {
        bridgesCount.text = conductorName + ": " + pingCount; // prints UI instead of component name. Correct ping count though.
    }

    //Hides or Shows the UI.
    public void toggleUI()
    {
        if (UIisOn)
        {
            UIui.SetActive(false);
            UIisOn = false;
        }
        else
        {
            UIui.SetActive(!UIui.activeSelf);
            UIisOn = true;
        }
    }

    //Hides or Shows the Scale Grid
    public void toggleGrid()
    {
        if (GridIsOn)
        {
            grid.SetActive(false);
            GridIsOn = false;
        }
        else
        {
            grid.SetActive(!grid.activeSelf);
            GridIsOn = true;
        }
    }

    //Allows users to raise grid to fit their board.
    public void RaiseGrid()
    {
        if (grid != null)
        {
            grid.transform.position += new Vector3(0, 0.1f, 0);
        }
    }

    //Allows for lowering of grid.
    public void LowerGrid()
    {
        if (grid != null)
        {
            grid.transform.position -= new Vector3(0, 0.1f, 0);
        }
    }

    //Tells the progran to apply the shock throughout the simulation
    public void toggleShock()
    {
        if (!isShockActive)
        {
            isShockActive = true;
        }
        else
        {
            isShockActive = false;
        }
    }

    //Tells the program to apply the vibration throughout the simulation
    public void toggleVibration()
    {
        if (!isVibrationActive)
        {
            isVibrationActive = true;
        }
        else
        {
            isVibrationActive = false;
        }
    }

    //Properties class
    public class MaterialProperties
    {
        public float density;
        public float resistivity;
        public float coefficientOfFriction;

        public MaterialProperties(float density, float resistivity, float coefficientOfFriction)
        {
            this.density = density;
            this.resistivity = resistivity;
            this.coefficientOfFriction = coefficientOfFriction;
        }
    }

    public void OnSaveInputButtonClicked()
    {
        //calls the savesettings method from SimulationController script to save inputs
        simulationController.SaveSettings();
    }

    public void OnLoadInputButtonClicked()
    {
        simulationController.LoadSettings(); //Calls the LoadSettings method from SImulationController
    }

    public void PopulateConductorDropdowns() //Critical Pair UI
    {
        //Find all gameobjects with the tag "ConductorTrigger"
        GameObject[] conductorObjects = GameObject.FindGameObjectsWithTag("ConductorTrigger");

        //Extract unique conductor names
        List<string> conductorNames = new List<string>();
        foreach (GameObject obj in conductorObjects)
        {
            string name = obj.name.Replace("_ColliderCopy", "");
            if (!conductorNames.Contains(name))
            {
                conductorNames.Add(name);
            }
        }

        //sort names alphabetically
        conductorNames.Sort();

        //clear existing options
        conductor1Dropdown.ClearOptions();
        conductor2Dropdown.ClearOptions();

        //Add options to dropdowns
        conductor1Dropdown.AddOptions(conductorNames);
        conductor2Dropdown.AddOptions(conductorNames);
    }

    public string CreatePairKey(string conductorA, string conductorB) //critical pair UI
    {
        var orderedPair = new[] { conductorA, conductorB }.OrderBy(name => name).ToArray();
        return $"{orderedPair[0]}-{orderedPair[1]}";
    }

    public void OnAddPairButtonClicked() //critical pair UI
    {
        string conductorA = conductor1Dropdown.options[conductor1Dropdown.value].text;
        string conductorB = conductor2Dropdown.options[conductor2Dropdown.value].text;

        string pairKey = CreatePairKey(conductorA, conductorB);

        if (!criticalPairs.Contains(pairKey))
        {
            criticalPairs.Add(pairKey);
            AddCriticalPairToUI(conductorA, conductorB);
        }
        else
        {
            Debug.Log("Critical pair already exists.");
        }
    }

    public void OnRemovePairButtonClicked() //critical pair UI
    {
        string conductorA = conductor1Dropdown.options[conductor1Dropdown.value].text;
        string conductorB = conductor2Dropdown.options[conductor2Dropdown.value].text;

        string pairKey = CreatePairKey(conductorA, conductorB);

        if (criticalPairs.Contains(pairKey))
        {
            criticalPairs.Remove(pairKey);
            RemoveCriticalPairFromUI(pairKey);
        }
        else
        {
            Debug.Log("Critical pair does not exist.");
        }
    }

    public void AddCriticalPairToUI(string conductorA, string conductorB)
    {
        // Instantiate a new list item under the Content GameObject
        GameObject newItem = Instantiate(criticalPairItemPrefab, criticalPairsScrollView.content);

        // Set the text to display the pair
        TextMeshProUGUI itemText = newItem.GetComponentInChildren<TextMeshProUGUI>();
        if (itemText != null)
        {
            itemText.text = $"{conductorA} - {conductorB}";
        }
        else
        {
            Debug.LogError("TextMeshProUGUI component not found in CriticalPairItemPrefab.");
        }

        // Store the pair key in the item's name for easy removal
        newItem.name = CreatePairKey(conductorA, conductorB);

        // Add to the list of UI items
        criticalPairItems.Add(newItem);
    }

    public void RemoveCriticalPairFromUI(string pairKey)
    {
        // Find the UI item corresponding to the pairKey
        GameObject itemToRemove = criticalPairItems.FirstOrDefault(item => item.name == pairKey);

        if (itemToRemove != null)
        {
            criticalPairItems.Remove(itemToRemove);
            Destroy(itemToRemove);
        }
        else
        {
            Debug.Log($"No UI item found with name {pairKey}.");
        }
    }


    private IEnumerator DelayedPopulateConductorDropdowns()
    {
        // Wait until the end of the frame
        yield return new WaitForEndOfFrame();

        // Now populate the conductor dropdowns
        PopulateConductorDropdowns();
    }

    public void OnRefreshButtonClicked()
    {
        PopulateConductorDropdowns();
    }

    //Code for getting input variables and storing them so they can be inserted into .csv
    private List<KeyValuePair<string, string>> GetSimulationInputs()
    {
        List<KeyValuePair<string, string>> inputs = new List<KeyValuePair<string, string>>();

        // Collect inputs from UIScript
        inputs.Add(new KeyValuePair<string, string>("Length Mu", lengthMu.text));
        inputs.Add(new KeyValuePair<string, string>("Length Sigma", lengthSigma.text));
        inputs.Add(new KeyValuePair<string, string>("Width Mu", widthMu.text));
        inputs.Add(new KeyValuePair<string, string>("Width Sigma", widthSigma.text));
        inputs.Add(new KeyValuePair<string, string>("# Of Whiskers", numWhiskers.text));
        inputs.Add(new KeyValuePair<string, string>("Conductive Material Selection", material_input.text));
        inputs.Add(new KeyValuePair<string, string>("Whisker Material", whiskMat.options[whiskMat.value].text));
        inputs.Add(new KeyValuePair<string, string>("Distribution Selection", distributionDropdown.options[distributionDropdown.value].text));
        inputs.Add(new KeyValuePair<string, string>("Whisker Spawn Point X", WhiskerSpawnPointX.text));
        inputs.Add(new KeyValuePair<string, string>("Whisker Spawn Point Y", WhiskerSpawnPointY.text));
        inputs.Add(new KeyValuePair<string, string>("Whisker Spawn Point Z", WhiskerSpawnPointZ.text.Trim()));
        inputs.Add(new KeyValuePair<string, string>("X-Coord", xCoord.text));
        inputs.Add(new KeyValuePair<string, string>("Y-Coord", yCoord.text));
        inputs.Add(new KeyValuePair<string, string>("Z-Coord", zCoord.text));
        inputs.Add(new KeyValuePair<string, string>("Gravity", whiskerControl.gravity.options[whiskerControl.gravity.value].text));
        inputs.Add(new KeyValuePair<string, string>("# of Iterations", numIterations.text));
        inputs.Add(new KeyValuePair<string, string>("Directory Path", whiskerControl.directoryPath));
        inputs.Add(new KeyValuePair<string, string>("Save File Name", whiskerControl.fileName));

        // Add Wall Height from WallCreator
        inputs.Add(new KeyValuePair<string, string>("Wall Height", wallCreator.wallHeightInput.text));

        // Add Rotations from CircuitBoardFinder
        inputs.Add(new KeyValuePair<string, string>("Rotation-X", circuitBoardFinder.CircuitRotateX.text));
        inputs.Add(new KeyValuePair<string, string>("Rotation-Y", circuitBoardFinder.CircuitRotateY.text));
        inputs.Add(new KeyValuePair<string, string>("Rotation-Z", circuitBoardFinder.CircuitRotateZ.text));

        // Add Spin Rates from CircuitBoardFinder
        inputs.Add(new KeyValuePair<string, string>("Spin-X", circuitBoardFinder.SpinRateX.text));
        inputs.Add(new KeyValuePair<string, string>("Spin-Y", circuitBoardFinder.SpinRateY.text));
        inputs.Add(new KeyValuePair<string, string>("Spin-Z", circuitBoardFinder.SpinRateZ.text));

        inputs.Add(new KeyValuePair<string, string>("Shock Amplitude", shockAmp.text));

        // For checkboxes, output Y/N
        string shockActive = isShockActive ? "Y" : "N";
        inputs.Add(new KeyValuePair<string, string>("Shock Active", shockActive));

        inputs.Add(new KeyValuePair<string, string>("Vibration Amplitude", vibrAmp.text));
        inputs.Add(new KeyValuePair<string, string>("Vibration Frequency", vibrFreq.text));
        inputs.Add(new KeyValuePair<string, string>("Vibration Duration", vibrDur.text));

        string vibrationActive = isVibrationActive ? "Y" : "N";
        inputs.Add(new KeyValuePair<string, string>("Vibration Active", vibrationActive));

        // Add debug statements to log collected inputs before filtering
        Debug.Log("Collected Simulation Inputs (before filtering):");
        foreach (var input in inputs)
        {
            Debug.Log($"Key: '{input.Key}', Value: '{input.Value}'");
        }

        //remove empty entries
        inputs = inputs.Where(input => !string.IsNullOrWhiteSpace(input.Key) && !string.IsNullOrWhiteSpace(input.Value)).ToList();

        // Add debug statements to log inputs after filtering
        Debug.Log("Simulation Inputs (after filtering empty entries):");
        foreach (var input in inputs)
        {
            Debug.Log($"Key: '{input.Key}', Value: '{input.Value}'");
        }

        return inputs;
    }


}
