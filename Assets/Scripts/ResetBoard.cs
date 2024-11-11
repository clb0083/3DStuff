using UnityEngine;
using UnityEngine.UI;

public class ToggleButton : MonoBehaviour
{
    public Toggle RotateSpinToggle; // Reference to the Toggle UI element
    public Button StartSimulation; // Reference to the Button UI element

    void Start()
    {
            RotateSpinToggle.isOn = !RotateSpinToggle.isOn; // Toggle the state
    }

    // Method to toggle the toggle state
    void ToggleState()
    {
        RotateSpinToggle.isOn = !RotateSpinToggle.isOn; // Toggle the state
    }
}
