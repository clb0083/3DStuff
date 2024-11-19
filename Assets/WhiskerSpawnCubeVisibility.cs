using UnityEngine;
using UnityEngine.UI;

public class ToggleVisibility : MonoBehaviour
{
    public GameObject cube; // Reference to the cube
    public Toggle toggle; // Reference to the toggle

    void Start()
    {
        // Ensure the toggle's on value is synced with the cube's initial visibility
        toggle.isOn = cube.activeSelf;
        toggle.onValueChanged.AddListener(OnToggleChanged);
    }

    void OnToggleChanged(bool isOn)
    {
        cube.SetActive(isOn); // Set the cube's active state based on toggle value
    }
}

