using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class TriggerTracker : MonoBehaviour
{
    private int pingCount = 0;
    public UIScript uiScript;
    
    // Methods to increase the ping count
    void Start()
    {
        uiScript = FindObjectOfType<UIScript>();
    }
    public void IncrementPingCount()
    {
        pingCount++;
        Debug.Log($"Ping count for {gameObject.name}: {pingCount}");
    }

    public int GetPingCount()
    {
        return pingCount;
    }
    private void OnTriggerEnter(Collider other)
    {
        // Example: Check if the other object is what you expect (optional)
        if (other.CompareTag("whiskerClone"))
        {
            StartCoroutine(CheckConnectionDelayed(other));
        }
    }

    private IEnumerator CheckConnectionDelayed(Collider other)
    {
        yield return new WaitForSeconds(0.1f); // Adjust delay time as needed

        WhiskerControl whiskerControl = other.GetComponent<WhiskerControl>();
        if (whiskerControl != null && whiskerControl.haveLoggedConnection)
        {
            IncrementPingCount(); // Increment the ping count
        }
    }
    
} 
