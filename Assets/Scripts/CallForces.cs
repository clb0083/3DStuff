using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CallForces : MonoBehaviour
{
    public GameObject VibrationUI;
    public GameObject ShockUI;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void toggleShockForces()
    {
        ShockUI.SetActive(!ShockUI.activeSelf);
    }

    public void hideShockForces()
    {
        ShockUI.SetActive(false);
    }

    public void toggleVibrForces()
    {
        VibrationUI.SetActive(!VibrationUI.activeSelf);
    }

    public void hideVibrForces()
    {
        VibrationUI.SetActive(false);
    }
}
