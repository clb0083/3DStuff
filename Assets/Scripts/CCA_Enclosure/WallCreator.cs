using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WallCreator : MonoBehaviour
{
    public GameObject wallPrefab; //prefab for the wall
    public GameObject ceilingPrefab; //prefab for ceiling
    public Button wallButton; //reference to the create walls button
    public TMP_InputField wallHeightInput; //Refernece to the input field for wall height
    public Toggle ceilingToggle; //Reference to the toggle button for the ceiling
    public Material transparentWallMaterial;

    private BoardDetector boardDetector; //to detect the board
    private GameObject[] currentWalls; //stores the currently created walls
    private GameObject currentCeiling; //stores currently created ceiling

    private void Start()
    {
        // find the button and add a listener for the onClick event
        if (wallButton != null)
        {
            wallButton.onClick.AddListener(OnWallButtonPressed);
        }

        // Find the BoardDetector component in the scene
        boardDetector = GetComponent<BoardDetector>();

        if (boardDetector == null)
        {
            Debug.LogError("BoardDetector component is missing!");
        }
    }

    //called when the button is pressed
    public void OnWallButtonPressed()
    {
        DeleteExistingWallsAndCeiling();

        //Get the detected board from the BoardDetector script
        GameObject detectedBoard = boardDetector.detectedBoard;

        if (detectedBoard != null)
        {
            Renderer boardRenderer = detectedBoard.GetComponent<Renderer>();
            Bounds boardBounds = boardRenderer.bounds;

            //Debug.Log("Detected Board for Wall Placement: " + detectedBoard.name);
            //Debug.Log("detected Board Bounds for Wall PLacement: " + boardBounds);

            // The input from the text box to get the wall height
            float wallHeight;
            if (float.TryParse(wallHeightInput.text, out wallHeight))
            {
                // creates walls around the detected board
                CreateWalls(boardBounds, detectedBoard, wallHeight);

                //check if ceiling toggle is on
                if (ceilingToggle.isOn)
                {
                    CreateCeiling(boardBounds, detectedBoard, wallHeight);
                }
            }
            else
            {
                Debug.LogError("Invalid input for wall height. Please enter a valid number.");
            }
        }
        else
        {
            Debug.LogError(" No board detected.");
        }
    }


    private void CreateWalls(Bounds boardBounds, GameObject detectedBoard, float wallHeight)
    {
        Vector3 boardSize = boardBounds.size;
        Vector3 boardCenter = boardBounds.center;

        //ensures bottom of walls align with bottom of the board
        float wallBottomY = boardBounds.min.y;

        // Log the wall position and size
        //Debug.Log($"Creating Walls at Bottom Y: {wallBottomY}, Wall Height: {wallHeight}");

        // Wall dimensions
        float wallThickness = 0.1f;

        // positions and scales the walls
        Vector3 leftWallPos = new Vector3(boardCenter.x - (boardSize.x / 2) - (wallThickness / 2), wallBottomY + (wallHeight /2), boardCenter.z);
        Vector3 rightWallPos = new Vector3(boardCenter.x + (boardSize.x / 2) + (wallThickness / 2), wallBottomY + (wallHeight / 2), boardCenter.z);
        Vector3 frontWallPos = new Vector3(boardCenter.x, wallBottomY + (wallHeight / 2), boardCenter.z + (boardSize.z / 2) + (wallThickness / 2));
        Vector3 backWallPos = new Vector3(boardCenter.x, wallBottomY + (wallHeight / 2), boardCenter.z - (boardSize.z / 2) - (wallThickness / 2));


        // Creates an array to hold references for the new walls
        currentWalls = new GameObject[4];

        // Instantiate walls
        currentWalls[0] = CreateWall(leftWallPos, new Vector3(wallThickness, wallHeight, boardSize.z), detectedBoard);
        currentWalls[1] = CreateWall(rightWallPos, new Vector3(wallThickness, wallHeight, boardSize.z), detectedBoard);
        currentWalls[2] = CreateWall(frontWallPos, new Vector3(boardSize.x, wallHeight, wallThickness), detectedBoard);
        currentWalls[3] = CreateWall(backWallPos, new Vector3(boardSize.x, wallHeight, wallThickness), detectedBoard);
    }

    private void CreateCeiling(Bounds boardBounds, GameObject detectedBoard, float wallHeight)
    {
        Vector3 boardSize = boardBounds.size;
        Vector3 boardCenter = boardBounds.center;

        float ceilingThickness = 0.1f;

        //Positions ceiling at the top of the walls
        float ceilingY = boardBounds.min.y + wallHeight + (ceilingThickness / 2);

        //Debug.Log($"Creating Ceiling at Y: {ceilingY}, Board Center: {boardCenter}");

        Vector3 ceilingPos = new Vector3(boardCenter.x, ceilingY, boardCenter.z);

        // Create the ceiling
        currentCeiling = Instantiate(ceilingPrefab, ceilingPos, Quaternion.identity);
        currentCeiling.transform.localScale = new Vector3(boardSize.x, 0.1f, boardSize.z); //adjusts size and thickeness
        currentCeiling.transform.SetParent(detectedBoard.transform); //attaches ceiling to the board

        // Apply the transparent material to the ceiling
        Renderer ceilingRenderer = currentCeiling.GetComponent<Renderer>();
        if (ceilingRenderer != null)
        {
            ceilingRenderer.material = transparentWallMaterial;
        }
    }

    private GameObject CreateWall(Vector3 position, Vector3 scale, GameObject detectedBoard)
    {
        GameObject wall = Instantiate(wallPrefab, position, Quaternion.identity);
        wall.transform.localScale = scale;
        wall.transform.SetParent(detectedBoard.transform); //attaches wall to the board

        //Apply tansparent material to the wall
        Renderer wallrenderer = wall.GetComponent<Renderer>();
        if (wallrenderer != null)
        {
            wallrenderer.material = transparentWallMaterial;
        }
        return wall; //return reference to newly created wall
    }

    private void DeleteExistingWallsAndCeiling()
    {
        //Checks for existing walls
        if (currentWalls != null)
        {
            // Loop through the walls and destroy tgem
            for (int i=0; i < currentWalls.Length; i++)
            {
                if (currentWalls[i] != null)
                {
                    Destroy(currentWalls[i]);
                }
            }
        }

        //check and delete the ceiling if it exists
        if (currentCeiling != null)
        {
            Destroy(currentCeiling);
        }
    }
}
