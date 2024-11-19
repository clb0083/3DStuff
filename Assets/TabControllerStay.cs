using UnityEngine;
using UnityEngine.UI;

public class TabControllerStay : MonoBehaviour
{
    public RectTransform tab; // The RectTransform of the tab you want to move
    public Button moveButton; // The button that triggers the movement
    public Vector2 targetPosition; // The position to move the tab to

    private void Start()
    {
        // Ensure the button has a listener for the click event
        moveButton.onClick.AddListener(MoveTab);
    }

    private void MoveTab()
    {
        // Move the tab to the specified position
        tab.anchoredPosition = targetPosition;
    }
}

