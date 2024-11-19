using System;
using System.IO;
using System.Linq;
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
                // Read all lines from the CSV file
                string[] lines = File.ReadAllLines(filePath);

                // Check for empty file or insufficient rows
                if (lines.Length < 2)
                {
                    outputText.text = "CSV file is empty or missing header row.";
                    return;
                }

                // Parse header row (second row) and calculate column widths based on header cells
                string[] header = lines[1].Split(',');
                int[] columnWidths = header.Select(h => h.Length).ToArray();

                // Get the total width in characters that the TextMeshProUGUI can display
                float totalTextWidth = outputText.rectTransform.rect.width;
                float characterWidth = outputText.fontSize * 0.5f;  // Estimate character width as half of the font size
                int totalColumnsWidth = Mathf.FloorToInt(totalTextWidth / characterWidth);

                // Distribute the total width across columns proportionally based on the header
                int headerWidthSum = columnWidths.Sum();
                for (int i = 0; i < columnWidths.Length; i++)
                {
                    columnWidths[i] = Mathf.FloorToInt((float)columnWidths[i] / headerWidthSum * totalColumnsWidth);
                }

                // Clear the existing text
                outputText.text = "";

                // Display the header row with center alignment and adjusted column widths
                for (int i = 0; i < header.Length; i++)
                {
                    outputText.text += CenterAlignText(header[i], columnWidths[i]) + "    ";
                }
                outputText.text += "\n" + new string('-', totalColumnsWidth + (header.Length - 1) * 4) + "\n";

                // Process each data row starting from the third line and format it based on the calculated column widths
                for (int row = 2; row < lines.Length; row++)
                {
                    string[] rowData = lines[row].Split(',');

                    for (int i = 0; i < columnWidths.Length; i++)
                    {
                        // Center-align each cell in the row based on header-defined width
                        string cellData = i < rowData.Length ? rowData[i] : "";  // Fallback to empty if data is missing
                        outputText.text += CenterAlignText(cellData, columnWidths[i]) + "    ";
                    }
                    outputText.text += "\n";  // New line for each row
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

    // Method to center-align text by adding spaces to both sides
    private string CenterAlignText(string text, int width)
    {
        int padding = width - text.Length;
        int padLeft = padding / 2 + text.Length;
        return text.PadLeft(padLeft).PadRight(width);
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
