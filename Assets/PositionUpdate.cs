using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PositionUpdater : MonoBehaviour
{
    public TMP_InputField WhiskerSpawnPointX;
    public TMP_InputField WhiskerSpawnPointY;
    public TMP_InputField WhiskerSpawnPointZ;
    public GameObject WhiskerSpawnPoint;

    void Start()
    {
        // Add listener to each input field to call UpdatePosition on value change
        WhiskerSpawnPointX.onValueChanged.AddListener(delegate { UpdatePosition(); });
        WhiskerSpawnPointY.onValueChanged.AddListener(delegate { UpdatePosition(); });
        WhiskerSpawnPointZ.onValueChanged.AddListener(delegate { UpdatePosition(); });
    }

    void UpdatePosition()
    {
        // Try to parse input field values to float
        if (float.TryParse(WhiskerSpawnPointX.text, out float x) &&
            float.TryParse(WhiskerSpawnPointY.text, out float y) &&
            float.TryParse(WhiskerSpawnPointZ.text, out float z))
        {
            // Set the position of the object
            WhiskerSpawnPoint.transform.position = new Vector3(x*10, y*10, z*10);
        }
    }
}

