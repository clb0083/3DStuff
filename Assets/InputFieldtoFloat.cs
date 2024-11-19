using UnityEngine;
using UnityEngine.UI;

public class InputFieldToFloat : MonoBehaviour
{
    public InputField inputField; // Drag your Input Field here in the inspector
    public float result; // This will hold the converted float

    public void ConvertInputToFloat()
    {
        // Try to parse the text from the Input Field
        if (float.TryParse(inputField.text, out result))
        {
            Debug.Log("Converted to float: " + result);
        }
        else
        {
            Debug.Log("Invalid input, please enter a valid float.");
        }
    }
}

