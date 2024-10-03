using UnityEngine;
using UnityEngine.UI;
using TMPro; // Include this for TextMeshPro

public class DropdownHandler : MonoBehaviour
{
    public Dropdown dropdown; // Reference to your TMP Dropdown
    public InputField inputFieldX; // Reference to your TMP Input Field

    public InputField inputFieldY; // Reference to your TMP Input Field

    public InputField inputFieldZ; // Reference to your TMP Input Field
    
    private void Start()
    {
        // Initialize the Input Field as disabled
        inputFieldX.gameObject.SetActive(false);
        inputFieldY.gameObject.SetActive(false);
        inputFieldZ.gameObject.SetActive(false);

        // Add listener for when the dropdown value changes
        dropdown.onValueChanged.AddListener(OnDropdownValueChanged);
    }

    private void OnDropdownValueChanged(int index)
    {
        // Assuming the option you want to check is at index 1
        if (index == 3) // Change this to the index of your desired option
        {
            inputFieldX.gameObject.SetActive(true); // Show the input field
            inputFieldY.gameObject.SetActive(true);
            inputFieldZ.gameObject.SetActive(true);
        }
        else
        {
            inputFieldX.gameObject.SetActive(false); // Hide the input field
            inputFieldY.gameObject.SetActive(false);
            inputFieldZ.gameObject.SetActive(false);
        }
    }
}
