using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WallCreator : MonoBehaviour
{
    public GameObject wallPrefab; //prefab for the wall
    public Button wallButton; //reference to the UI button
    public TMP_InputField wallHeightInput; //Refernece to the UI input field for wall height
    public Material transparentWallMaterial;

    private BoardDetector boardDetector; //to detect the board
    private GameObject[] currentWalls; //stores the currently created walls

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
        DeleteExistingWalls();
        //Get the detected board from the BoardDetector script
        GameObject detectedBoard = boardDetector.detectedBoard;

        if (detectedBoard != null)
        {
            Renderer boardRenderer = detectedBoard.GetComponent<Renderer>();
            Bounds bounds = boardRenderer.bounds;

            // The input from the text box to get the wall height
            float wallHeight;
                if (float.TryParse(wallHeightInput.text, out wallHeight))
            {
                // creates walls around the detected board
                CreateWalls(bounds, detectedBoard, wallHeight);
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


    private void CreateWalls(Bounds bounds, GameObject detectedBoard, float wallHeight)
    {
        Vector3 boardSize = bounds.size;
        Vector3 boardCenter = bounds.center;

        // positions and scales the walls
        Vector3 leftWallPos = new Vector3(boardCenter.x - boardSize.x / 2, boardCenter.y, boardCenter.z);
        Vector3 rightWallPos = new Vector3(boardCenter.x + boardSize.x / 2, boardCenter.y, boardCenter.z);
        Vector3 frontWallPos = new Vector3(boardCenter.x, boardCenter.y, boardCenter.z + boardSize.z / 2);
        Vector3 backWallPos = new Vector3(boardCenter.x, boardCenter.y, boardCenter.z - boardSize.z / 2);

        // Wall dimensions
        float wallThickness = 0.1f;

        // Creates an array to hold references for the new walls
        currentWalls = new GameObject[4];

        // Instantiate walls
        currentWalls[0] = CreateWall(leftWallPos, new Vector3(wallThickness, wallHeight, boardSize.z), detectedBoard);
        currentWalls[1] = CreateWall(rightWallPos, new Vector3(wallThickness, wallHeight, boardSize.z), detectedBoard);
        currentWalls[2] = CreateWall(frontWallPos, new Vector3(boardSize.x, wallHeight, wallThickness), detectedBoard);
        currentWalls[3] = CreateWall(backWallPos, new Vector3(boardSize.x, wallHeight, wallThickness), detectedBoard);
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

    private void DeleteExistingWalls()
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
    }
}
