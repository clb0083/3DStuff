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

    void Update()
    {
        // Rotate the camera based on user input
         if (Input.GetMouseButton(0))
        {
            float horizontalRotation = Input.GetAxis("Mouse X") * rotationSpeed;
            float verticalRotation = Input.GetAxis("Mouse Y") * rotationSpeed;

            transform.Rotate(Vector3.up, horizontalRotation);
            transform.Rotate(Vector3.left, verticalRotation);
        

            // Move the camera based on user input
            float horizontalInput = Input.GetAxis("Horizontal");
            float verticalInput = Input.GetAxis("Vertical");

            Vector3 moveDirection = new Vector3(horizontalInput, 0f, verticalInput).normalized;
            transform.Translate(moveDirection * moveSpeed * Time.deltaTime);

            // Move the camera vertically with Q and E
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

    public void TopDownView()
    {
        // Move the camera to the target position
        transform.position = new Vector3(0, 85, 0);

        // Rotate the camera to the target rotation
        transform.rotation = Quaternion.Euler(90,180,0);
    }

    public void StandardView()
    {
        // Move the camera to the target position
        transform.position = new Vector3(0, 48, 68);

        // Rotate the camera to the target rotation
        transform.rotation = Quaternion.Euler(36,180,0);
    }

    public void SideView()
    {
        transform.position = new Vector3(73,5,0);
        transform.rotation = Quaternion.Euler(5,-90,0);

    }
}
