/*AU Team 1 (SP & SU 2024) used the help of Auburn graduate student Jake Botello
to learn Unity and C# in integrating design ideas. The team applied background
knowledge of MATLAB and C++ coding languages to develop various tools and
functions used throughout this script. ChatGPT was also used as a troubleshooting
reference.*/

using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

//This script tracks how many times each trigger has made contact with a whisker and formed a bridge
public class TriggerTracker : MonoBehaviour
{
    private int pingCount = 0;
    public UIScript uiScript;

    void Start()
    {
        uiScript = FindObjectOfType<UIScript>();
    }
    public void IncrementPingCount()
    {
        pingCount++;
        //Debug.Log($"Ping count for {gameObject.name}: {pingCount}"); //log to notify when whiskers hit nodes, can uncomment whenever
    }

    public int GetPingCount()
    {
        return pingCount;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("whiskerClone"))
        {
            StartCoroutine(CheckConnectionDelayed(other));
        }
    }

    private IEnumerator CheckConnectionDelayed(Collider other)
    {
        yield return new WaitForSeconds(0.1f);

        WhiskerControl whiskerControl = other.GetComponent<WhiskerControl>();
        if (whiskerControl != null && whiskerControl.haveLoggedConnection)
        {
            IncrementPingCount();
        }
    }
} 
