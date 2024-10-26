using System;
using System.IO;
using UnityEngine;
using TMPro;

public class LoadBridgedWhiskerData : MonoBehaviour
{
    public TextMeshProUGUI outputText;  // Attach this to the TMP text object in your Scroll View
    public RectTransform contentRectTransform;  // Reference to the Content RectTransform
    private string filePath;

    void Start()
    {
        // Set the file path to the saved CSV file in the Assets folder
        filePath = Path.Combine(Application.dataPath, "bridgeOutput.csv");
    }

    // Method to load and display CSV data, now public so it can be called by a UI button
    public void LoadCSVData()
    {
        try
        {
            // Check if the file exists
            if (File.Exists(filePath))
            {
                // Read all the lines from the CSV file
                string[] lines = File.ReadAllLines(filePath);

                // Clear the existing text
                outputText.text = "";

                // Combine all lines into one single string with newlines
                foreach (string line in lines)
                {
                    outputText.text += line + Environment.NewLine;
                }

                // Adjust content size to make it scrollable
                AdjustContentHeight();
            }
            else
            {
                outputText.text = "No CSV file found.";
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"Failed to load CSV data: {ex.Message}");
        }
    }

    // Adjusts the content height based on the text size
    private void AdjustContentHeight()
    {
        // Reset the Content position to align with the top of the Viewport
        contentRectTransform.anchoredPosition = Vector2.zero;

        // Calculate the total preferred height of the text in outputText
        float textHeight = outputText.preferredHeight;

        // Set the Content height to match the height of the text
        contentRectTransform.sizeDelta = new Vector2(contentRectTransform.sizeDelta.x, textHeight);
    }

}
