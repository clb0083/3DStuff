using UnityEngine;
using TMPro;

public class AccelerationInput : MonoBehaviour
{
    public TMP_InputField inputField;
    [HideInInspector] public string accelerationExpression = "1"; // Default value

    public void SubmitExpression()
    {
        accelerationExpression = inputField.text;
        Debug.Log("User entered: " + accelerationExpression);
    }
}
