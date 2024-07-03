using System;
using UnityEngine;

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