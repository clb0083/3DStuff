using UnityEngine;
using UnityEngine.UI;
using TMPro; // Include this for TextMeshPro

public class DropdownHandler : MonoBehaviour
{
    public Dropdown dropdown; // Reference to your TMP Dropdown
    public InputField inputField; // Reference to your TMP Input Field

    private void Start()
    {
        // Initialize the Input Field as disabled
        inputField.gameObject.SetActive(false);

        // Add listener for when the dropdown value changes
        dropdown.onValueChanged.AddListener(OnDropdownValueChanged);
    }

    private void OnDropdownValueChanged(int index)
    {
        // Assuming the option you want to check is at index 1
        if (index == 3) // Change this to the index of your desired option
        {
            inputField.gameObject.SetActive(true); // Show the input field
        }
        else
        {
            inputField.gameObject.SetActive(false); // Hide the input field
        }
    }
}
