using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class ScreenshotHandler : MonoBehaviour
{
    public Button screenshotButton;   // Assign the in-game button in the Inspector
    public Image flashPanel;          // Assign the UI Image (flash panel) in the Inspector
    public Canvas uiCanvas;           // Reference to the Canvas that contains all UI elements
    private int screenshotCount = 1;  // Counter to track the number of screenshots
    private float flashDuration = 0.2f; // Duration of the flash

    private void Start()
    {
        // Ensure the button is set
        if (screenshotButton != null)
        {
            screenshotButton.onClick.AddListener(TakeScreenshot);
        }

        // Ensure the flash panel starts transparent
        if (flashPanel != null)
        {
            flashPanel.color = new Color(1, 1, 1, 0); // White color with 0 alpha (invisible)
            flashPanel.raycastTarget = false; // Ensure the panel does not block raycasts
        }
    }

    private void TakeScreenshot()
    {
        StartCoroutine(CaptureScreenshotWithoutUI());
    }

    private IEnumerator CaptureScreenshotWithoutUI()
    {
        // Hide the UI Canvas
        if (uiCanvas != null)
        {
            uiCanvas.enabled = false;
        }

        // Wait for the end of the frame to ensure the UI is hidden
        yield return new WaitForEndOfFrame();

        // Take the screenshot
        string desktopPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Desktop);
        string fileName = $"3D Whisker Simulation - {screenshotCount}.png"; // Format the filename with the index
        string fullPath = Path.Combine(desktopPath, fileName);

        ScreenCapture.CaptureScreenshot(fullPath);
        Debug.Log("Screenshot saved to: " + fullPath);

        screenshotCount++; // Increment the counter after each screenshot

        // Show the UI Canvas again
        if (uiCanvas != null)
        {
            uiCanvas.enabled = true;
        }

        // Trigger the flash effect
        StartCoroutine(ScreenFlash());
    }

    private IEnumerator ScreenFlash()
    {
        // Enable raycast blocking during the flash effect
        flashPanel.raycastTarget = true;

        // Flash in (increase alpha to 1)
        for (float t = 0; t < flashDuration; t += Time.deltaTime)
        {
            flashPanel.color = new Color(1, 1, 1, Mathf.Lerp(0, 1, t / flashDuration));
            yield return null;
        }

        // Hold flash for a brief moment
        yield return new WaitForSeconds(0.05f);

        // Flash out (decrease alpha to 0)
        for (float t = 0; t < flashDuration; t += Time.deltaTime)
        {
            flashPanel.color = new Color(1, 1, 1, Mathf.Lerp(1, 0, t / flashDuration));
            yield return null;
        }

        // Ensure the panel is fully transparent at the end
        flashPanel.color = new Color(1, 1, 1, 0);

        // Disable raycast blocking after the flash effect is done
        flashPanel.raycastTarget = false;
    }
}
