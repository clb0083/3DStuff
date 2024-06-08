using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CallForces : MonoBehaviour
{
    public GameObject ForceUI;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void toggleForces()
    {
        ForceUI.SetActive(!ForceUI.activeSelf);
    }

    public void hideForces()
    {
        ForceUI.SetActive(false);
    }
}
