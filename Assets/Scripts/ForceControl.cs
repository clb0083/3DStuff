using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class ForceControl : MonoBehaviour
{
    public TMP_InputField shockMag;
    public Vector3 impulseDirection = Vector3.forward; // Direction of the impulse force

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
       
    public void shockForceGenerate()
    {
        // Apply impulse force when the script starts
        Rigidbody rb = GetComponent<Rigidbody>();
    
        rb.AddForce(impulseDirection * float.Parse(shockMag.text), ForceMode.Impulse);
    }   
       
    
}
