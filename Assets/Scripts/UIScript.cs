/*AU Team 1 (SP & SU 2024) used the help of Auburn graduate student Jake Botello
to learn Unity and C# in integrating design ideas. The team applied background
knowledge of MATLAB and C++ coding languages to develop various tools and
functions used throughout this script. ChatGPT was also used as a troubleshooting
reference.*/

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.IO;
using UnityEngine.UI;
using Unity.VisualScripting;
using System.Data.Common;

//This is the main script that controls many of the objects on the interface, namely the user inputs.
public class UIScript : MonoBehaviour
{
    private System.Random rand = new System.Random();
    public Dropdown distributionDropdown;
    public TMP_Dropdown whiskMat;
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
    public TMP_InputField vibrAmp;
    public TMP_InputField vibrFreq;
    public TMP_InputField vibrDur;
    public GameObject whisker;
    public TextMeshProUGUI totalBridges;
    public int bridgesDetected;
    public TextMeshProUGUI bridgesEachRun;
    public int bridgesPerRun;
    public TextMeshProUGUI errorMessage;
    public float simtimeElapsed;
    public bool startSim = false;
    public int simIntComplete = 1;
    public float simTimeThresh;
    public TMP_InputField totalRuns;
    public float moveSpeed = 5f;
    public DistributionType distributionType = DistributionType.Lognormal;
    public bool UIisOn = true;
    public GameObject UIui;
    public bool GridIsOn = true;
    public GameObject grid;
    public TextMeshProUGUI iterationCounter;
    public TextMeshProUGUI iterationCompleteMessage;
    public TMP_InputField material_input;
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
    public bool isShockActive = false;
    public VibrationManager vibrationManager;
    public ShockManager shockManager;
    public float shockPressTimer = 0f;
    public float shockPressInterval = 2f;
    public SimulationController simulationController; //reference to the simulation controller script
    private int whiskerCounter; //variable to track whisker numbers

    //Sets the lists for the dimensions/data to be stored in, as well as sets material properties from the dropdown.
    void Start()
    {
        whiskerCounter = 1; //initialize the counter to 1 when the script starts

        lengths = new List<float>();
        widths = new List<float>();
        volumes = new List<float>();
        masses = new List<float>();
        resistances = new List<float>();

        whiskMat.onValueChanged.AddListener(delegate {
            WhiskMatDropdownValueChanged(whiskMat);
        });

        UpdateMaterialPropertiesUI(currentMaterial);

        Time.fixedDeltaTime = 0.005f;
        Physics.defaultSolverIterations = 10;
        Physics.defaultSolverVelocityIterations = 10;
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

        if(startSim)
        {
            if (isShockActive && shockManager.shockButton.interactable)
            {
                shockPressTimer += Time.deltaTime;
                if (shockPressTimer >= shockPressInterval)
                {
                    shockManager.shockPressed();
                    shockPressTimer = 0f; 
                }
            }

            if(isVibrationActive && vibrationManager.vibrateButton.interactable)
            {
                vibrationManager.vibratePressed();
            }

            simtimeElapsed += Time.deltaTime;
            if (simIntComplete <= Convert.ToInt32(totalRuns.text)-1)
            {
                iterationCounter.text = "Iteration Counter: " + simIntComplete.ToString();
                if (simtimeElapsed > simTimeThresh)
                {
                    simIntComplete++;
                    ReloadWhiskersButton();
                    simtimeElapsed = 0f;
                }
            }

            if (simIntComplete == Convert.ToInt32(totalRuns.text))
            {
                
                iterationCounter.text = "Iteration Counter: " + simIntComplete.ToString();
                if (simtimeElapsed > simTimeThresh)
                {
                    iterationCompleteMessage.text = "Simulation Complete!";
                    startSim = false;
                }
            }
        }
    }

    //Resets the simulation counters back to original value
    public void resetSim()
    {
        simIntComplete = 1;
        simtimeElapsed = 0;
        bridgesDetected = 0;
        bridgesPerRun = 0;
        iterationCompleteMessage.text = "";
        iterationCounter.text = "";
        startSim = false;
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
            if (mu_log > 9 || mu_log < 0 )
                {
                    SetErrorMessage("Width Mu value outside acceptable range.");
                    return;
                }
                if (sigma_log > 4 || sigma_log < 0 )
                {
                    SetErrorMessage("Width Sigma value is outside acceptable range.");
                    return;
                }
                if (mu > 20 || mu < 0 )
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
            if (mu_log > 10000 || mu_log < 0 )
                {
                    SetErrorMessage("Width Mu value outside acceptable range.");
                    return;
                }
                if (sigma_log > 10000 || sigma_log < 0 )
                {
                    SetErrorMessage("Width Sigma value is outside acceptable range.");
                    return;
                }
                if (mu > 30000 || mu < 0 )
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

        if(numberIterations < 0 || !Mathf.Approximately(numberIterations, Mathf.Round(numberIterations)) )
        {
            SetErrorMessage("Iteration value must be a positive integer.");
            return;
        }

        lengths.Clear();
        widths.Clear();
        volumes.Clear();
        masses.Clear();
        resistances.Clear();

        for (int i = 0; i < numWhiskersToCreate; i++)
        {
            //Generate dimensions and spawn position
            float diameter = WidthDistributionGenerate() / 1000;
            float length = LengthDistributionGenerate() / 1000;
            float spawnPointX = UnityEngine.Random.Range(-float.Parse(xCoord.text)*10, float.Parse(xCoord.text)*10);
            float spawnPointY = UnityEngine.Random.Range(1, Convert.ToInt32(yCoord.text)*10);
            float spawnPointZ = UnityEngine.Random.Range(-float.Parse(zCoord.text)*10, float.Parse(zCoord.text)*10);
            Vector3 spawnPos = new Vector3(spawnPointX, spawnPointY, spawnPointZ);

            GameObject whiskerClone = Instantiate(whisker, spawnPos, Quaternion.Euler(UnityEngine.Random.Range(0, 360), UnityEngine.Random.Range(0, 360), UnityEngine.Random.Range(0, 360)));
            whiskerClone.tag = "whiskerClone";

            whiskerClone.name = $"whisker_{whiskerCounter}"; //unique name for each whisker
            whiskerCounter++; //increment counter for next whisker

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
            if(diameter*1000 < 10) 
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
           
            if(whiskerControl.confirmGravity)
            {
                WhiskerData data = new WhiskerData(length, diameter, volume, mass, resistance, simIntComplete); 
                SaveWhiskerData(data);
            }

            // Debug log for verification
            Debug.Log($"Whisker created with material: {currentMaterial}, Density: {currentProps.density}, Mass: {mass}, Resistance: {resistance}");
        }
    }

    //Updates the friction on the whiskers
    private void UpdatePhysicsMaterialFriction(PhysicMaterial material)
    {
        MaterialProperties currentProps = materialProperties[currentMaterial];
        material.staticFriction = currentProps.coefficientOfFriction;
        material.dynamicFriction = currentProps.coefficientOfFriction;
    }

    //Controls the Reload whiskers buttons. Clears out whiskers and generates new.
    public void ReloadWhiskersButton()
    {
        // Clear out all whiskers
        GameObject[] allWhiskers = GameObject.FindGameObjectsWithTag("whiskerClone");
        foreach (GameObject whisk in allWhiskers)
        {
            Destroy(whisk.gameObject);
        }
        bridgesPerRun = 0;

        // Reset the whisker counter before creating new whiskers
        whiskerCounter = 1; //resets counter to 1 for each iteration
        MakeWhiskerButton();
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
        string directoryPath = whiskerControl.directoryPath;
        string filePath = Path.Combine(directoryPath, whiskerControl.fileName + ".csv");

        string directoryPathCheck = whiskerControl.directoryPath;
        string fileNameCheck = whiskerControl.fileName;

    if (string.IsNullOrEmpty(directoryPath) || string.IsNullOrEmpty(whiskerControl.fileName))
    {
        SetErrorMessage("Failed to save data - Path or Filename cannot be empty");
        return;
    }

        try
        {
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }
            bool fileExists = File.Exists(filePath);

            using (StreamWriter writer = new StreamWriter(filePath, true))
            {
                if (!fileExists)
                {
                    // Write the column titles
                    writer.WriteLine("All Whiskers,,,,Bridged Whiskers");
                    writer.WriteLine("Length (um),Width (um),Resistance (ohm),Iteration,Length (um),Width (um),Resistance (ohm),Iteration");
                }

                writer.WriteLine($"{data.Length*1000},{data.Width*1000},{data.Resistance},{data.Iteration}");
                DataSaveManager.CurrentRowIndex++;
            }
            Debug.Log($"Whisker data saved successfully to {filePath}");
            
        }
        catch (Exception ex)
        {
            Debug.LogError($"Failed to save whisker data: {ex.Message}");
            
        }
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
        if(UIisOn)
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
        if(GridIsOn)
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
        if(!isShockActive)
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
        if(!isVibrationActive)
        {
            isVibrationActive = true;
        }
        else
        {
            isVibrationActive = false;
        }
    }

    //Class for whisker data to be stored.
    public class WhiskerData
        {
        public float Length { get; set; }
        public float Width { get; set; }
        public float Volume { get; set; }
        public float Mass { get; set; }
        public float Resistance { get; set; }
        public int Iteration { get; set; } // New property for iteration count

        public WhiskerData(float length, float width, float volume, float mass, float resistance, int iteration)
        {
            Length = length;
            Width = width;
            Volume = volume;
            Mass = mass;
            Resistance = resistance;
            Iteration = iteration; // Initialize iteration count
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
}

