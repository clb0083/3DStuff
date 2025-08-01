using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WhiskerAcceleration : MonoBehaviour
{

    public bool applyForce = false;
    public float simTimeStart = 0f;
    public FunctionInputHandler FunctionSource;

    private Rigidbody rb;

    // Deactivate gravity on Whisker
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
    }

    // Gets force/acceleration value from Function Input Handler and
    // adds force/acceleration to Whisker
    void FixedUpdate()
    {
        if (applyForce && FunctionSource != null)
        {
            float time = Time.time - simTimeStart;
            float force = FunctionSource.GetValueAtTime(time);
            rb.AddForce(new Vector3(0, -force, 0), ForceMode.Acceleration);
            Debug.Log("t: " + time + ", force: " + force);
        
        }
    }
}
