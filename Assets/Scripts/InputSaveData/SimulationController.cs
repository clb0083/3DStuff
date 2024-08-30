using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;

public class SimulationController : MonoBehaviour
{
    public TMP_InputField lengthMuInput;
    public TMP_InputField widthMuInput;
    public TMP_InputField lengthSigmaInput;
    public TMP_InputField widthSigmaInput;
    public TMP_InputField numWhiskersInput;
    public TMP_InputField numIterationsInput;

    public SaveManager saveManager;
    private SimulationData currentSimulationData;

    private void Start()
    {
        // Try to find SaveManager if not set in the Inspector
        if (saveManager == null)
        {
            saveManager = FindObjectOfType<SaveManager>();
            if (saveManager == null)
            {
                Debug.LogError("SaveManager not found in the scene. Make sure there is a GameObject with the SaveManager script attached.");
                return; // Exit early to avoid null reference
            }
        }

        currentSimulationData = new SimulationData();

        // Check if saveManager is still null
        if (saveManager != null)
        {
            LoadSettings();
        }
        else
        {
            Debug.LogError("SaveManager is null. Cannot load settings.");
        }
    }

    public void SaveSettings()
    {
        if (currentSimulationData == null)
        {
            Debug.LogError("currentSimulationData is null.");
            return;
        }

        // Update current simulation data from UI inputs
        currentSimulationData.lengthMu = float.Parse(lengthMuInput.text);
        currentSimulationData.widthMu = float.Parse(widthMuInput.text);
        currentSimulationData.lengthSigma = float.Parse(lengthSigmaInput.text);
        currentSimulationData.widthSigma = float.Parse(widthSigmaInput.text);
        currentSimulationData.numWhiskers = int.Parse(numWhiskersInput.text);
        currentSimulationData.numIterations = int.Parse(numIterationsInput.text);

        if (saveManager != null)
        {
            saveManager.SaveSimulationData(currentSimulationData);
        }
        else
        {
            Debug.LogError("SaveManager is null. Cannot save settings.");
        }
    }

    public void LoadSettings()
    {
        if (saveManager != null)
        {
            SimulationData loadedData = saveManager.LoadSimulationData();
            if (loadedData != null)
            {
                currentSimulationData = loadedData;
                UpdateUIWithLoadedData(currentSimulationData);
            }
        }
        else
        {
            Debug.LogError("SaveManager is null. Cannot load settings.");
        }
    }

    private void UpdateUIWithLoadedData(SimulationData data)
    {
        lengthMuInput.text = data.lengthMu.ToString();
        widthMuInput.text = data.widthMu.ToString();
        lengthSigmaInput.text = data.lengthSigma.ToString();
        widthSigmaInput.text = data.widthSigma.ToString();
        numWhiskersInput.text = data.numWhiskers.ToString();
        numIterationsInput.text = data.numIterations.ToString();
    }
}
