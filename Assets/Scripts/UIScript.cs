using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.IO;
using UnityEngine.UI;
using Unity.VisualScripting;
using System.Data.Common;

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
    public int simIntComplete = 0;
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

    void Start()
    {
        lengths = new List<float>();
        widths = new List<float>();
        volumes = new List<float>();
        masses = new List<float>();
        resistances = new List<float>();

        whiskMat.onValueChanged.AddListener(delegate {
            WhiskMatDropdownValueChanged(whiskMat);
        });

        UpdateMaterialPropertiesUI(currentMaterial);

        // Adjust physics settings
        Time.fixedDeltaTime = 0.005f; // Increase the frequency of physics updates
        Physics.defaultSolverIterations = 10; // Default is 6
        Physics.defaultSolverVelocityIterations = 10; // Default is 1
    }

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

    // Method to handle whiskMat dropdown value change
    void WhiskMatDropdownValueChanged(TMP_Dropdown change)
    {
        currentMaterial = (MaterialType)change.value;
        UpdateMaterialPropertiesUI(currentMaterial);

        // Added to close dropdown after a short delay
        StartCoroutine(CloseDropdownAfterDelay());
    }

    // Coroutine to close dropdown after a short delay
    private IEnumerator CloseDropdownAfterDelay()
    {
        yield return null;
        TMP_Dropdown dropdown = whiskMat.GetComponent<TMP_Dropdown>();
        dropdown.Hide();
    }

    void Update()
    {
        totalBridges.text = "Total Bridges: " + bridgesDetected.ToString();
        bridgesEachRun.text = "This Run: " + bridgesPerRun.ToString();

        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        Vector3 moveDirection = new Vector3(horizontalInput, 0f, verticalInput).normalized;
        transform.Translate(moveDirection * moveSpeed * Time.deltaTime);

        if (startSim)
        {
            simtimeElapsed += Time.deltaTime;
            if (simIntComplete < Convert.ToInt32(totalRuns.text))
            {
                iterationCounter.text = "Iteration Counter: " + simIntComplete.ToString();
                if (simtimeElapsed > simTimeThresh)
                {
                    ReloadWhiskersButton();
                    simtimeElapsed = 0f;
                }
            }

            if (simIntComplete == Convert.ToInt32(totalRuns.text))
            {
                iterationCounter.text = "Iteration Counter: " + simIntComplete.ToString();
                startSim = false;
                iterationCompleteMessage.text = "Simulation Complete!";
            }
        }
    }

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

    public float WidthDistributionGenerate() //xx_log = xx_width
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

    private float GenerateLogNormalValue(float mu_log, float sigma_log)
    {
        float normalVal = RandomFromDistribution.RandomNormalDistribution(mu_log, sigma_log);
        float logNormalVal = Mathf.Exp(normalVal);
        return logNormalVal;
    }

    private float GenerateNormalValue(float mu_norm, float sigma_norm)
    {
        return RandomFromDistribution.RandomNormalDistribution(mu_norm, sigma_norm);
    }

public void MakeWhiskerButton()
{
    //error handling | Limits are subject to change |
    float mu_log = float.Parse(widthMu.text);
    float sigma_log = float.Parse(widthSigma.text);
    float mu = float.Parse(lengthMu.text);
    float sigma = float.Parse(lengthSigma.text);
    float numWhiskersToCreate = float.Parse(numWhiskers.text);
    float numberIterations = float.Parse(totalRuns.text);

    string directoryPathCheck = whiskerControl.directoryPath;
    string fileNameCheck = whiskerControl.fileName;

    //float x = float.TryParse(xCoord.text);
    float x = Convert.ToSingle(xCoord.text);
    float y = Convert.ToSingle(yCoord.text);
    float z = Convert.ToSingle(zCoord.text);

    simIntComplete++;

    if (distributionType == DistributionType.Lognormal)
        {
           if (mu_log > 9)
            {
                SetErrorMessage("Width Mu value outside acceptable range.");
                return;
            }
            if (sigma_log > 4)
            {
                SetErrorMessage("Width Sigma value is outside acceptable range.");
                return;
            }
            if (mu > 20)
            {
                SetErrorMessage("Length Mu value outside acceptable range.");
                return;
            }
            if (sigma > 15)
            {
                SetErrorMessage("Length Sigma value is outside acceptable range.");
                return;
            } 
        }
    else
        {
           if (mu_log > 10000)
            {
                SetErrorMessage("Width Mu value outside acceptable range.");
                return;
            }
            if (sigma_log > 10000)
            {
                SetErrorMessage("Width Sigma value is outside acceptable range.");
                return;
            }
            if (mu > 30000)
            {
                SetErrorMessage("Length Mu value outside acceptable range.");
                return;
            }
            if (sigma > 10000)
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

    if(directoryPathCheck == null || fileNameCheck == null)
    {
        SetErrorMessage("Failed to save data - Path or Filename cannot be empty");
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

    //int numWhiskersToCreate = Convert.ToInt32(numWhiskers.text);
    for (int i = 0; i < numWhiskersToCreate; i++)
    {
        // Generate dimensions and spawn position
        float diameter = WidthDistributionGenerate() / 1000;
        float length = LengthDistributionGenerate() / 1000;
        float spawnPointX = UnityEngine.Random.Range(-float.Parse(xCoord.text)*10, float.Parse(xCoord.text)*10);
        float spawnPointY = UnityEngine.Random.Range(1, Convert.ToInt32(yCoord.text)*10);
        float spawnPointZ = UnityEngine.Random.Range(-float.Parse(zCoord.text)*10, float.Parse(zCoord.text)*10);
        Vector3 spawnPos = new Vector3(spawnPointX, spawnPointY, spawnPointZ);

        
        // Instantiate whisker clone
        GameObject whiskerClone = Instantiate(whisker, spawnPos, Quaternion.Euler(UnityEngine.Random.Range(0, 360), UnityEngine.Random.Range(0, 360), UnityEngine.Random.Range(0, 360)));
        whiskerClone.tag = "whiskerClone";

        // Ensure Rigidbody component exists and set mass
        Rigidbody whiskerRigidbody = whiskerClone.GetComponent<Rigidbody>();
        if (whiskerRigidbody == null)
        {
            whiskerRigidbody = whiskerClone.AddComponent<Rigidbody>();
        }

        // Calculate mass based on material properties and dimensions
        MaterialProperties currentProps = materialProperties[currentMaterial];
        float volume = Mathf.PI * Mathf.Pow(diameter / 2, 2) * length;
        float mass = volume * currentProps.density;
        float resistance = (currentProps.resistivity * length * 1000) / (Mathf.PI * Mathf.Pow(diameter * 1000 / 2, 2));

        // Set a minimum mass limit
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

        // Enable continuous collision detection
        whiskerRigidbody.collisionDetectionMode = CollisionDetectionMode.Continuous;

//UPSIZING WIDTHS
        Transform visual = whiskerClone.transform.Find("visual");
        Transform collider = whiskerClone.transform.Find("collider");
        if(diameter < 10) //can be changed to whatever 
        {
            visual.localScale = new Vector3(5, 1, 5); // scales relative to the parent object 
        } // so this is saying if the diameter is less than 50, it takes the diameter of the orignal whisker and *5.

        collider.localScale = new Vector3(1, 1, 1); // keeps it all the same as the original.

        // Scale whisker and update lists
        whiskerClone.transform.localScale = new Vector3(diameter, length / 2, diameter);
        lengths.Add(length);
        widths.Add(diameter);

        // Ensure Collider component exists and set physics material
        Collider whiskerCollider = whiskerClone.GetComponent<Collider>();
        if (whiskerCollider == null)
        {
            Debug.LogError("Whisker prefab must have a Collider component.");
            return;
        }

        // Get or create physics material and set its friction properties
        PhysicMaterial whiskerPhysicsMaterial = whiskerCollider.sharedMaterial;
        if (whiskerPhysicsMaterial == null)
        {
            whiskerPhysicsMaterial = new PhysicMaterial();
            whiskerCollider.sharedMaterial = whiskerPhysicsMaterial;
        }

        // Update friction properties based on selected material type
        whiskerPhysicsMaterial.staticFriction = currentProps.coefficientOfFriction;
        whiskerPhysicsMaterial.dynamicFriction = currentProps.coefficientOfFriction;

        WhiskerData data = new WhiskerData(length, diameter, volume, mass, resistance, simIntComplete); // Pass simIntComplete as iteration count
        SaveWhiskerData(data);

        // Debug log for verification
        Debug.Log($"Whisker created with material: {currentMaterial}, Density: {currentProps.density}, Mass: {mass}, Resistance: {resistance}");
    }
}


    public void ReloadWhiskersButton()
    {
        GameObject[] allWhiskers = GameObject.FindGameObjectsWithTag("whiskerClone");
        foreach (GameObject whisk in allWhiskers)
        {
            Destroy(whisk.gameObject);
        }
        bridgesPerRun = 0;
        MakeWhiskerButton();
    }

    private void SetErrorMessage(string message)
    {
        errorMessage.text = message;
        StartCoroutine(ClearErrorMessageAfterDelay(6f));
    }

    private IEnumerator ClearErrorMessageAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        errorMessage.text = "";
    }
    //saving whiskers data;
    
    public void SaveWhiskerData(WhiskerData data)
    {   
        string directoryPath = whiskerControl.directoryPath;
        string filePath = Path.Combine(directoryPath, whiskerControl.fileName + ".csv");

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

    public void UpdateConductorBridge(string conductorName, int pingCount)
    {
        bridgesCount.text = conductorName + ": " + pingCount; // prints UI instead of component name. Correct ping count though.
    }
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
}

