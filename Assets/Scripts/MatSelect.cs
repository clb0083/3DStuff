/*AU Team 1 (SP & SU 2024) used the help of Auburn graduate student Jake Botello
to learn Unity and C# in integrating design ideas. The team applied background
knowledge of MATLAB and C++ coding languages to develop various tools and
functions used throughout this script. ChatGPT was also used as a troubleshooting
reference.*/

using System;
using UnityEngine;

//This allows the user to select between the three materials of Tin, Zinc, and Cadmium from the dropdown list.
public enum MaterialType
{
    Tin,
    Zinc,
    Cadmium
}

public class MatSelect : MonoBehaviour
{
    public MaterialType selectedMaterial = MaterialType.Tin;

    void Start()
    {
        selectedMaterial = MaterialType.Tin; // Default selection
    }
}