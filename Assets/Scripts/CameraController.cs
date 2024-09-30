using System.Collections.Generic;
using UnityEngine;

public class WhiskerCameraController : MonoBehaviour
{
    public Transform defaultPosition;  // Default camera position
    public float orbitSpeed = 50f;     // Speed of camera orbit
    public float distanceFromTarget = 5f; // Distance between camera and whisker
    private bool isOrbiting = false;   // Flag to check if camera is orbiting
    private List<GameObject> bridgedWhiskers; // List of whiskers to cycle through
    private int currentWhiskerIndex = 0; // Index of the current whisker
    private Transform currentTarget;    // Current target whisker
    private Vector3 initialPosition;    // Store initial camera position
    private Quaternion initialRotation; // Store initial camera rotation
    private WhiskerControl whiskerControl; // Reference to the WhiskerControl script

    private void Start()
    {
        // Store the initial camera position and rotation
        initialPosition = transform.position;
        initialRotation = transform.rotation;

        // Find the WhiskerControl component in the scene
        whiskerControl = FindObjectOfType<WhiskerControl>();

        // Initialize bridgedWhiskers list from WhiskerControl
        if (whiskerControl != null)
        {
            bridgedWhiskers = whiskerControl.bridgedWhiskers.ConvertAll(w => GameObject.Find($"Whisker_{w.WhiskerNumber}"));
            if (bridgedWhiskers.Count > 0)
            {
                currentTarget = bridgedWhiskers[currentWhiskerIndex].transform;
            }
        }
        else
        {
            Debug.LogError("WhiskerControl script not found in the scene.");
        }
    }

    private void Update()
    {
        // Check for tab key to cycle through whiskers
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            CycleToNextWhisker();
        }

        // Check for "/" key to reset the camera
        if (Input.GetKeyDown(KeyCode.Slash))
        {
            ResetCamera();
        }

        // If orbiting, rotate the camera around the current target
        if (isOrbiting && currentTarget != null)
        {
            OrbitCurrentTarget();
        }
    }

    // Cycles to the next bridged whisker in the list
    private void CycleToNextWhisker()
    {
        if (bridgedWhiskers.Count == 0)
            return;

        currentWhiskerIndex = (currentWhiskerIndex + 1) % bridgedWhiskers.Count;
        currentTarget = bridgedWhiskers[currentWhiskerIndex].transform;
        isOrbiting = true; // Start orbiting the new target
    }

    // Orbits around the current target whisker
    private void OrbitCurrentTarget()
    {
        // Calculate the camera's new position around the target
        transform.position = currentTarget.position + (transform.position - currentTarget.position).normalized * distanceFromTarget;

        // Rotate the camera around the target
        transform.RotateAround(currentTarget.position, Vector3.up, orbitSpeed * Time.deltaTime);
        transform.LookAt(currentTarget); // Make sure the camera is looking at the target
    }

    // Resets the camera to the default position and stops orbiting
    private void ResetCamera()
    {
        isOrbiting = false;
        transform.position = initialPosition;
        transform.rotation = initialRotation;
    }
}
