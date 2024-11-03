using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataManager : MonoBehaviour
{
    public static DataManager Instance { get; private set; }

    public List<WhiskerData> allWhiskersData = new List<WhiskerData>();
    public List<WhiskerData> bridgedWhiskersData = new List<WhiskerData>();
    public List<WhiskerData> criticalBridgedWhiskersData = new List<WhiskerData>(); // For critical pairs

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Optional: Keeps DataManager persistent across scenes
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
