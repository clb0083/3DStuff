/*AU Team 1 (SP & SU 2024 used the help of Auburn graduate student Jake Botello
to learn Unity and C# in integratinf design ideas. The team applied background
knowledge of MATLAB and C++ coding languages to develop various tools and
functions used throughout this script. ChatGPT was also used as a troubleshooting
reference.*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This script is solely used to hide or show the external force UI
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
