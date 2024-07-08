using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.IO;
using UnityEngine.UI;
using Unity.VisualScripting;

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
    public bool startSim;
    public int simIntComplete;
    public float simTimeThresh;
    public TMP_InputField totalRuns;
    public float moveSpeed = 5f;
    public DistributionType distributionType = DistributionType.Lognormal;

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
        // Yielding for one frame to ensure dropdown update is complete
        yield return null;

        // Close the dropdown
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

        simtimeElapsed += Time.deltaTime;

        if (startSim)
        {
            if (simIntComplete < Convert.ToInt32(totalRuns.text))
            {
                if (simtimeElapsed > simTimeThresh)
                {
                    ReloadWhiskersButton();
                    simIntComplete += 1;
                    simtimeElapsed = 0f;
                }
            }

            if (simIntComplete == Convert.ToInt32(totalRuns.text))
            {
                startSim = false;
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
    //error handling
    float mu_log = float.Parse(widthMu.text);
    float sigma_log = float.Parse(widthSigma.text);
    float mu = float.Parse(lengthMu.text);
    float sigma = float.Parse(lengthSigma.text);
    if (mu_log > 1000)
    {
        SetErrorMessage("Width Mu value outside acceptable range");
        return;
    }
    if (sigma_log > 50)
    {
        SetErrorMessage("Width Sigma value is outside acceptable range");
        return;
    }
    if (mu > 9999)
    {
        SetErrorMessage("Length Mu value outside acceptable range");
        return;
    }
    if (sigma > 500)
    {
        SetErrorMessage("Length Sigma value is outside acceptable range");
        return;
    }

    lengths.Clear();
    widths.Clear();
    volumes.Clear();
    masses.Clear();
    resistances.Clear();

    int numWhiskersToCreate = Convert.ToInt32(numWhiskers.text);
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
        float resistance = (currentProps.resistivity * length) / (Mathf.PI * Mathf.Pow(diameter / 2, 2));

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

        // Optionally, save whisker data if needed
        WhiskerData data = new WhiskerData(length, diameter, volume, mass, resistance);
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

    public void SaveWhiskerData(WhiskerData data)
    {
        string directoryPath = @"D:/Unity";
        string filePath = Path.Combine(directoryPath, "whisker_data.csv");

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
                    writer.WriteLine("Length,Width,Volume,Mass,Resistance");
                }

                writer.WriteLine($"{data.Length},{data.Width},{data.Volume},{data.Mass},{data.Resistance}");
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

    public class WhiskerData
    {
        public float Length { get; set; }
        public float Width { get; set; }
        public float Volume { get; set; }
        public float Mass { get; set; }
        public float Resistance { get; set; }

        public WhiskerData(float length, float width, float volume, float mass, float resistance)
        {
            Length = length;
            Width = width;
            Volume = volume;
            Mass = mass;
            Resistance = resistance;
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
/*original
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using System.IO;
using UnityEngine.UI;

public class UIScript : MonoBehaviour
{
    private System.Random rand = new System.Random();//NEW
    public TMP_Dropdown whiskMat;
    public Dropdown distributionDropdown;
    public TMP_InputField lengthMu;
    public TMP_InputField widthMu;
    public TMP_InputField lengthSigma;
    public TMP_InputField widthSigma;
    public TMP_InputField numWhiskers;
    public TMP_InputField xCoord;
    public TMP_InputField yCoord;
    public TMP_InputField zCoord;
    public TMP_InputField numIterations;
    public TextMeshProUGUI distributionSelect;
    public TextMeshProUGUI gravSelect;
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
    public TextMeshProUGUI bridgesCount;
    
    //for monte carlo stuff \/
    public float simtimeElapsed;
    public bool startSim;
    public int simIntComplete;
    public float simTimeThresh;
    public TMP_InputField totalRuns;
    public float moveSpeed = 5f;
    public DistributionType distributionType = DistributionType.Lognormal;
    private List<float> lengths;
    private List<float> widths;
    public MaterialType currentMaterial = MaterialType.Tin;

    private Dictionary<MaterialType, MaterialProperties> materialProperties = new Dictionary<MaterialType, MaterialProperties>()
    { //density (kg/cm^3), resistivity (ohm*cm), coefficient of friction (unitless)
        { MaterialType.Tin, new MaterialProperties(0.0073f, 1.09e-5f, 0.32f) },
        { MaterialType.Zinc, new MaterialProperties(0.00714f, 5.9e-6f, 0.6f) },
        { MaterialType.Cadmium, new MaterialProperties(0.00865f, 7.0e-6f, 0.5f) }
    };
   

    // Start is called before the first frame C
    void Start()
    {
        lengths = new List<float>();
        widths = new List<float>();

        whiskMat.onValueChanged.AddListener(delegate {
            WhiskMatDropdownValueChanged(whiskMat);
        });

        UpdateMaterialPropertiesUI(currentMaterial);
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
    public void WhiskMatDropdownValueChanged(TMP_Dropdown change)
    {
        currentMaterial = (MaterialType)change.value;
        UpdateMaterialPropertiesUI(currentMaterial);

        // Added to close dropdown after a short delay
        StartCoroutine(CloseDropdownAfterDelay());
    }

    // Coroutine to close dropdown after a short delay
    private IEnumerator CloseDropdownAfterDelay()
    {
        // Yielding for one frame to ensure dropdown update is complete
        yield return null;

        // Close the dropdown
        TMP_Dropdown dropdown = whiskMat.GetComponent<TMP_Dropdown>();
        dropdown.Hide();
    }
  
    // Update is called once per frame
    void Update()
    {
        totalBridges.text = "Total Bridges: " + bridgesDetected.ToString();
        bridgesEachRun.text = "This Run: "+ bridgesPerRun.ToString();
        print(bridgesDetected);
        // Move the camera based on user input
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        Vector3 moveDirection = new Vector3(horizontalInput, 0f, verticalInput).normalized;
        transform.Translate(moveDirection * moveSpeed * Time.deltaTime);
    //MONTECARLO
        simtimeElapsed += Time.deltaTime;

        if(startSim)
        {   
            
            if(simIntComplete < Convert.ToInt32(totalRuns.text))
            {
                              
                if(simtimeElapsed > simTimeThresh)
                {
                    ReloadWhiskersButton();
                                       
                    simIntComplete +=1;
                    simtimeElapsed = 0f;                    
                }
            }

            if(simIntComplete == Convert.ToInt32(totalRuns.text))
            {
                startSim = false; 
            }
        }
    }
 
//LogNormalStuff
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

    public float LengthDistributionGenerate()
    {
        float mu = float.Parse(lengthMu.text);///10;
        float sigma = float.Parse(lengthSigma.text);///10;
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

    public float WidthDistributionGenerate()
    {
        float mu_log = float.Parse(widthMu.text);///10;
        float sigma_log = float.Parse(widthSigma.text);///10;
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
 
    public void MakeWhiskerButton()
    {
        for(int i = 0; i < Convert.ToInt32(numWhiskers.text); i++)
            {
                float diameter = WidthDistributionGenerate()/1000; //NEW STUFF /1000;
                float length = LengthDistributionGenerate()/1000;
                float spawnPointX = UnityEngine.Random.Range(-float.Parse(xCoord.text),float.Parse(xCoord.text));
                float spawnPointY = UnityEngine.Random.Range(1,Convert.ToInt32(yCoord.text)); //can remove range to this if needed.
                float spawnPointZ = UnityEngine.Random.Range(-float.Parse(zCoord.text),float.Parse(zCoord.text));

                Vector3 spawnPos = new Vector3(spawnPointX,spawnPointY,spawnPointZ);

                GameObject whiskerClone = Instantiate(whisker,spawnPos,Quaternion.Euler(UnityEngine.Random.Range(0, 360),  UnityEngine.Random.Range(0, 360), UnityEngine.Random.Range(0, 360)));
                whiskerClone.tag = "whiskerClone";

                //whiskerClone.AddComponent<TriggerCounterManager>();
                Rigidbody whiskerRigidbody = whiskerClone.GetComponent<Rigidbody>();
                // Calculate mass based on material properties and dimensions
                MaterialProperties currentProps = materialProperties[currentMaterial];
                float volume = Mathf.PI * Mathf.Pow(diameter / 2, 2) * length;
                float mass = volume * currentProps.density;

                // Set the mass of the whisker Rigidbody
                whiskerRigidbody.mass = mass;

                        
                whiskerClone.transform.localScale = new Vector3(diameter,length/2,diameter);
                lengths.Add(length);//*10);
                widths.Add(diameter);//*10);
            }
        SaveListsToCSV("D:/Unity/LogNormal.csv");
        //ApplyMacro("D:/Unity/LogNormal.xlsx");
    }
    public void ReloadWhiskersButton()
    {
        GameObject[] allWhiskers;
        allWhiskers = GameObject.FindGameObjectsWithTag("whiskerClone");
        foreach (GameObject whisk in allWhiskers)
        {
            Destroy(whisk.gameObject); // Remove the object from the scene
        }
        bridgesPerRun = 0;
        MakeWhiskerButton();
    }
    public Text conductorName;

    public void UpdateConductorBridge(string conductorName, int pingCount)
    {
        bridgesCount.text = conductorName + ": " + pingCount; // prints UI instead of component name. Correct ping count though.
    }

    public void SaveListsToCSV(string filePath)
    {
        using (StreamWriter writer = new StreamWriter(filePath))
        {
            writer.WriteLine("Length,Width");
            for (int i = 0; i < lengths.Count; i++)
            {
                writer.WriteLine($"{lengths[i]},{widths[i]}");
            }
        }
    }
    public class WhiskerData
    {
        public float Length { get; set; }
        public float Width { get; set; }
        public float Volume { get; set; }
        public float Mass { get; set; }
        public float Resistance { get; set; }

        public WhiskerData(float length, float width, float volume, float mass, float resistance)
        {
            Length = length;
            Width = width;
            Volume = volume;
            Mass = mass;
            Resistance = resistance;
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
    
}*/
