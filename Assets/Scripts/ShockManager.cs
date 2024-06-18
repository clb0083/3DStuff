using UnityEngine;
using TMPro;
using System;
using System.Collections;

public class SimpleShockManager3D : MonoBehaviour
{
    public TMP_InputField forceInputField; // Input field for shock force
    public Transform circuitBoard; // Reference to the circuit board transform
    private Rigidbody circuitBoardRigidbody; // Rigidbody of the circuit board
    public float duration = 0.1f; // Duration of the movement
    private Vector3 originalPosition; // Original position of the circuit board

    private void Start()
    {
        // Store the original position of the circuit board
        originalPosition = circuitBoard.position;
    }

    public void ApplyShock()
    {
        // Convert the input field text to an integer
        float force;
        if (float.TryParse(forceInputField.text, out force))
        {
            // Start the coroutine to move the board and return it back
            StartCoroutine(MoveBoard(force));
        }
        else
        {
            Debug.LogError("Invalid input for force. Please enter a valid integer.");
        }
    }

    private IEnumerator MoveBoard(float force)
    {
        Vector3 targetPosition = originalPosition + Vector3.right * force;
        float elapsedTime = 0;

        // Move to the target position
        while (elapsedTime < duration)
        {
            circuitBoard.position = Vector3.Lerp(originalPosition, targetPosition, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        circuitBoard.position = targetPosition;

        // Wait for a short period at the target position
        yield return new WaitForSeconds(0.01f);

        elapsedTime = 0;

        // Move back to the original position
        while (elapsedTime < duration)
        {
            circuitBoard.position = Vector3.Lerp(targetPosition, originalPosition, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        circuitBoard.position = originalPosition;
    }
}

