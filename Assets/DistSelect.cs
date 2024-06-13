using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

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
                uiScript.distributionType = DistributionType.Normal;
                break;
            case 1:
                uiScript.distributionType = DistributionType.Lognormal;
                break;
        }
    }
}

public enum DistributionType
{
    Normal,
    Lognormal
}