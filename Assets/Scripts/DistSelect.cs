/*AU Team 1 (SP & SU 2024) used the help of Auburn graduate student Jake Botello
to learn Unity and C# in integrating design ideas. The team applied background
knowledge of MATLAB and C++ coding languages to develop various tools and
functions used throughout this script. ChatGPT was also used as a troubleshooting
reference.*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

//used to allow the dropdown list to change between the lognormal/normal distributions, applys the selection in the UIScript for selection
public class DistributionSelector : MonoBehaviour
{
    public TMP_Dropdown dropdown; // Dropdown reference
    public UIScript uiScript; // Reference to your UIScript

    void Start()
    {
        dropdown.onValueChanged.AddListener(delegate {
            DropdownValueChanged(dropdown);
        });
    }

    void DropdownValueChanged(TMP_Dropdown change)
    {
        switch (change.value)
        {
            case 0:
                uiScript.distributionType = DistributionType.Lognormal;
                break;
            case 1:
                uiScript.distributionType = DistributionType.Normal;
                break;
        }
    }
}

public enum DistributionType
{
    Normal,
    Lognormal
}