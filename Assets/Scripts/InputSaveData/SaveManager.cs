using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    private string saveFilePath;

    private void Awake()
    {
        //set the file path to save the input data
        saveFilePath = Path.Combine(Application.dataPath + "/simulationInputData.json");

    }

    public void SaveSimulationData(SimulationData data)
    {
        string json = JsonUtility.ToJson(data);
        File.WriteAllText(saveFilePath, json);
        Debug.Log("Simulation data saved to " + saveFilePath);
    }

    public SimulationData LoadSimulationData()
    {
        if (File.Exists(saveFilePath))
        {
            string json = File.ReadAllText(saveFilePath);
            SimulationData data = JsonUtility.FromJson<SimulationData>(json);
            Debug.Log("Simulation data loaded from" + saveFilePath);
            return data;
        }
        else
        {
            Debug.LogWarning("No saved simulation data found at " + saveFilePath);
            return null;
        }
    }
}
