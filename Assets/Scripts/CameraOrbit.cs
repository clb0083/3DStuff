using System.Collections.Generic;
using UnityEngine;
using TMPro; // Required for TextMeshPro

public class CameraOrbit : MonoBehaviour
{
    public float orbitSpeed = 1f;                  // Speed of orbiting
    public float distanceFromTarget = 5f;           // Distance from the target object
    public Vector3 defaultPosition;                 // Default position of the camera
    public Vector3 defaultRotation;                 // Default rotation of the camera
    public float speedChangeAmount = 15f;            // Amount to change the orbit speed
    public TextMeshProUGUI OrbitWhiskerName;        // Reference to the UI text element for displaying whisker name
    public TMP_Dropdown whiskerDropdown;            // Reference to the UI dropdown for whisker selection

    private List<Transform> bridgedWhiskers;        // List of game objects with the tag "bridgedWhisker"
    private List<string> previousWhiskerNames;      // List to store the last state of whisker names for comparison
    private int currentTargetIndex = -1;            // Current target index for orbiting
    private bool isOrbiting = false;                // Is the camera currently orbiting
    private bool isMouseControlled = false;         // Is the camera being controlled by the mouse
    private Vector3 lastMousePosition;              // Last position of the mouse during movement

    void Start()
    {
        // Store the default position and rotation of the camera
        defaultPosition = transform.position;
        defaultRotation = transform.rotation.eulerAngles;

        // Initialize the list of bridged whiskers
        UpdateBridgedWhiskers();

        // Populate the dropdown with the whisker names
        PopulateDropdown();

        // Ensure the text element is cleared at the start
        OrbitWhiskerName.text = "";
    }

    void Update()
    {
        // Continuously update the list of bridged whiskers to check for any changes
        UpdateBridgedWhiskers();

        // Check if the whisker list has changed and update the dropdown if necessary
        if (HasWhiskerListChanged())
        {
            PopulateDropdown();
        }

        // Check for mouse button to take over control of the camera
        HandleMouseControl();

        // Check for the up arrow key to start orbiting to the next whisker
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            if (bridgedWhiskers.Count > 0)
            {
                // Move to the next whisker in numerical order
                currentTargetIndex = (currentTargetIndex + 1) % bridgedWhiskers.Count;
                UpdateWhiskerText(bridgedWhiskers[currentTargetIndex]);
                isOrbiting = true; // Start orbiting the selected whisker
            }
        }

        // Check for the down arrow key to reset the camera
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            ResetCamera();
        }

        // Check for keys to change the distance from the target
        if (Input.GetKey(KeyCode.W)) // Increase distance
        {
            distanceFromTarget += Time.deltaTime * orbitSpeed; // Change speed if necessary
        }
        if (Input.GetKey(KeyCode.S)) // Decrease distance
        {
            distanceFromTarget = Mathf.Max(1f, distanceFromTarget - Time.deltaTime * orbitSpeed); // Prevent going below 1 unit
        }

        // Adjust orbit speed with [ and ]
        if (Input.GetKey(KeyCode.LeftBracket)) // Decrease orbit speed
        {
            orbitSpeed = Mathf.Max(1f, orbitSpeed - speedChangeAmount * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.RightBracket)) // Increase orbit speed
        {
            orbitSpeed += speedChangeAmount * Time.deltaTime;
        }

        // Handle orbiting the current target if mouse control is not active
        if (isOrbiting && !isMouseControlled && currentTargetIndex >= 0 && currentTargetIndex < bridgedWhiskers.Count)
        {
            OrbitAround(bridgedWhiskers[currentTargetIndex]);
        }
    }

    void HandleMouseControl()
    {
        if (Input.GetMouseButtonDown(0))
        {
            // Start mouse control when the left mouse button is pressed
            isMouseControlled = true;
            isOrbiting = false; // Disable automatic orbit when mouse control begins
            lastMousePosition = Input.mousePosition;
        }

        if (Input.GetMouseButton(0) && isMouseControlled)
        {
            // Calculate mouse movement difference
            Vector3 mouseDelta = Input.mousePosition - lastMousePosition;
            lastMousePosition = Input.mousePosition;

            // Rotate the camera based on mouse movement
            float rotationX = mouseDelta.x * orbitSpeed * Time.deltaTime;
            float rotationY = -mouseDelta.y * orbitSpeed * Time.deltaTime;

            // Apply the rotation around the target
            transform.RotateAround(bridgedWhiskers[currentTargetIndex].position, Vector3.up, rotationX);
            transform.RotateAround(bridgedWhiskers[currentTargetIndex].position, transform.right, rotationY);
        }

        if (Input.GetMouseButtonUp(0))
        {
            // Stop mouse control when the left mouse button is released
            isMouseControlled = false;
        }
    }
    void UpdateBridgedWhiskers()
    {
        bridgedWhiskers = new List<Transform>();
        GameObject[] whiskerObjects = GameObject.FindGameObjectsWithTag("bridgedWhisker");

        foreach (GameObject obj in whiskerObjects)
        {
            bridgedWhiskers.Add(obj.transform);
        }

        // Sort the whiskers by the numeric part in their names
        bridgedWhiskers.Sort((a, b) => ExtractNumber(a.name).CompareTo(ExtractNumber(b.name)));
    }

    int ExtractNumber(string name)
    {
        // Assumes the number in the whisker name is the last part after an underscore
        string[] parts = name.Split('_');
        if (parts.Length > 1 && int.TryParse(parts[1], out int number))
        {
            return number; // Return the extracted number
        }
        return int.MaxValue; // If no valid number found, return max value to sort last
    }

    bool HasWhiskerListChanged()
    {
        List<string> currentWhiskerNames = new List<string>();
        foreach (Transform whisker in bridgedWhiskers)
        {
            currentWhiskerNames.Add(whisker.name);
        }

        // Compare the new list with the previous list of whisker names
        if (previousWhiskerNames == null || currentWhiskerNames.Count != previousWhiskerNames.Count)
        {
            previousWhiskerNames = currentWhiskerNames;
            return true; // List has changed
        }

        for (int i = 0; i < currentWhiskerNames.Count; i++)
        {
            if (currentWhiskerNames[i] != previousWhiskerNames[i])
            {
                previousWhiskerNames = currentWhiskerNames;
                return true; // List has changed
            }
        }

        return false; // No changes detected
    }

    void PopulateDropdown()
    {
        if (bridgedWhiskers == null || bridgedWhiskers.Count == 0)
        {
            Debug.LogWarning("No bridged whiskers found to populate the dropdown.");
            return; // Early exit if there are no whiskers to add
        }

        // Clear the current options in the dropdown
        whiskerDropdown.ClearOptions();

        // Create a list of whisker names
        List<string> whiskerNames = new List<string>();
        foreach (Transform whisker in bridgedWhiskers)
        {
            whiskerNames.Add(whisker.name);
        }

        // Add the whisker names to the dropdown
        whiskerDropdown.AddOptions(whiskerNames);

        // Add a listener to handle when the user selects a new option
        whiskerDropdown.onValueChanged.AddListener(OnDropdownValueChanged);

        Debug.Log("Dropdown successfully populated with whisker names.");
    }

    void OnDropdownValueChanged(int selectedIndex)
    {
        if (selectedIndex >= 0 && selectedIndex < bridgedWhiskers.Count)
        {
            // Update the camera to focus on the selected whisker
            currentTargetIndex = selectedIndex;
            UpdateWhiskerText(bridgedWhiskers[currentTargetIndex]);
            isOrbiting = true; // Start orbiting the selected whisker
            Debug.Log($"Dropdown selection changed to: {bridgedWhiskers[selectedIndex].name}");
        }
    }

    void OrbitAround(Transform target)
    {
        // Calculate the new position of the camera based on the target
        Vector3 direction = (transform.position - target.position).normalized;
        Quaternion rotation = Quaternion.Euler(0, orbitSpeed * Time.deltaTime, 0);
        Vector3 newPosition = target.position + rotation * direction * distanceFromTarget;

        // Update the camera's position and rotation
        transform.position = newPosition;
        transform.LookAt(target.position);
    }

    void ResetCamera()
    {
        transform.position = defaultPosition;
        transform.rotation = Quaternion.Euler(defaultRotation);
        isOrbiting = false;
        currentTargetIndex = -1; // Reset the target index
        distanceFromTarget = 5f; // Reset distance to default if desired

        // Clear the text when resetting
        OrbitWhiskerName.text = "";
    }

    void UpdateWhiskerText(Transform target)
    {
        Debug.Log($"Updating text for target: {target.name}"); // Debug statement

        // Always display the static text
        string staticText = "Red = Bridged\nOrange = Momentary\n";
        
        // Update the text with the name of the current target whisker
        OrbitWhiskerName.text = staticText + target.gameObject.name;
    }
}
