/*AU Team 1 (SP & SU 2024 used the help of Auburn graduate student Jake Botello
to learn Unity and C# in integratinf design ideas. The team applied background
knowledge of MATLAB and C++ coding languages to develop various tools and
functions used throughout this script. ChatGPT was also used as a troubleshooting
reference.*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    public float moveSpeed = 10f;
    public float rotationSpeed = 90000000f;
    public GameObject UIObject;
    public UIScript uiScript;

    // Start is called before the first frame update
    void Start()
    {
        
    }
    //freely moving camera based off user inputs.
    void Update()
    {
        // Rotates the camera based on user input
         if (Input.GetMouseButton(0))
        {
            float horizontalRotation = Input.GetAxis("Mouse X") * rotationSpeed;
            float verticalRotation = Input.GetAxis("Mouse Y") * rotationSpeed;

            transform.Rotate(Vector3.up, horizontalRotation);
            transform.Rotate(Vector3.left, verticalRotation);
        

            // Moves the camera based on user input
            float horizontalInput = Input.GetAxis("Horizontal");
            float verticalInput = Input.GetAxis("Vertical");

            Vector3 moveDirection = new Vector3(horizontalInput, 0f, verticalInput).normalized;
            transform.Translate(moveDirection * moveSpeed * Time.deltaTime);

            // Moves the camera vertically with Q and E
            if (Input.GetKey(KeyCode.Q))
            {
                transform.Translate(Vector3.down * moveSpeed * Time.deltaTime, Space.World);
            }
            if (Input.GetKey(KeyCode.E))
            {
                transform.Translate(Vector3.up * moveSpeed * Time.deltaTime, Space.World);
            }
        }
        
        if (Input.GetMouseButtonDown(1))
        {
            Quaternion currentRotation = transform.rotation;
            transform.rotation = Quaternion.Euler(currentRotation.eulerAngles.x, currentRotation.eulerAngles.y,0);
        }
    }

    //Preset camera angles for the user to click to if needed.
    public void TopDownView()
    {
        transform.position = new Vector3(0, 85, 0);
        transform.rotation = Quaternion.Euler(90,180,0);
    }

    public void StandardView()
    {
        transform.position = new Vector3(0, 48, 68);
        transform.rotation = Quaternion.Euler(36,180,0);
    }

    public void SideView()
    {
        transform.position = new Vector3(73,5,0);
        transform.rotation = Quaternion.Euler(5,-90,0);

    }
}
