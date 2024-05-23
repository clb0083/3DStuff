using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using UnityEngine.UI;

public class WhiskerControl : MonoBehaviour
{
    public Dropdown gravity;
    private ConstantForce cForce;
    private Vector3 forceDirection;
    public int selectedIndex = 0;

    // You can delete this variable--Jake added it to test the efficacy of the github pushes/pulls
    public string demoVar;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //gravityStuff
    public void ConfirmButtonPressed()
    {
        GetGravitySelection(gravity.value);
    }
    public void GetGravitySelection(int val)
    {
        ApplyGravity(val); //selectedIndex
    }

    public void ApplyGravity(int val)
    {
        GameObject[] objectsWithTag = GameObject.FindGameObjectsWithTag("whiskerClone");

        foreach (GameObject obj in objectsWithTag)
        {
            Rigidbody rb = obj.GetComponent<Rigidbody>();
        if (val == 0)
        {
            cForce = GetComponent<ConstantForce>();
            forceDirection = new Vector3(0,-10,0);
            cForce.force = forceDirection;
            
        }

        if (val == 1)
        {
            cForce = GetComponent<ConstantForce>();
            forceDirection = new Vector3(0,-2,0);
            cForce.force = forceDirection;
            
        }

        if (val == 2)
        {
            cForce = GetComponent<ConstantForce>();
            forceDirection = new Vector3(0,-4,0);
            cForce.force = forceDirection;
         
        }
            
        }
    }
    public void ResetGravity()
    {
        GameObject[] objectsWithTag = GameObject.FindGameObjectsWithTag("whiskerClone");

        foreach (GameObject obj in objectsWithTag)
        {
            cForce = GetComponent<ConstantForce>();
            forceDirection = new Vector3(0,0,0);
            cForce.force = forceDirection;
        }
    }

    //Collision Stuff
    public Material targetMaterial; // The material you want to check for
    private List<Material> contactedMaterials = new List<Material>(); // List to keep track of contacted materials
   
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<Renderer>() != null)
        {
            Material otherMaterial = other.gameObject.GetComponent<Renderer>().material;
            if (otherMaterial == targetMaterial && !contactedMaterials.Contains(otherMaterial))
            {
                contactedMaterials.Add(otherMaterial);
                Debug.Log("Object contacted with specific material: " + contactedMaterials.Count + " objects");
                
                if (contactedMaterials.Count >= 2)
                {
                    // Register the connection or perform other actions
                    Debug.Log("Connection registered because the object contacted with 2 or more objects with specific material.");
                }
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.GetComponent<Renderer>() != null)
        {
            Material otherMaterial = other.gameObject.GetComponent<Renderer>().material;
            if (otherMaterial == targetMaterial && contactedMaterials.Contains(otherMaterial))
            {
                contactedMaterials.Remove(otherMaterial);
                Debug.Log("Object left contact with specific material: " + contactedMaterials.Count + " objects");
            }
        }
    }
}

