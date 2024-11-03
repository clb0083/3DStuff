using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WhiskerData : MonoBehaviour
{
    public int WhiskerNumber { get; set; }
    public float Length { get; set; }
    public float Width { get; set; } // For all whiskers
    public float Diameter { get; set; } // For bridged whiskers
    public float Volume { get; set; } // For all whiskers
    public float Mass { get; set; } // For all whiskers
    public float Resistance { get; set; }
    public int Iteration { get; set; } // For all whiskers
    public int SimulationIndex { get; set; } // For bridged whiskers
    public string Conductor1 { get; set; } // For bridged whiskers
    public string Conductor2 { get; set; } // For bridged whiskers

    // Constructor for all whiskers
    public WhiskerData(int whiskerNumber, float length, float width, float volume, float mass, float resistance, int iteration)
    {
        WhiskerNumber = whiskerNumber;
        Length = length;
        Width = width;
        Volume = volume;
        Mass = mass;
        Resistance = resistance;
        Iteration = iteration;
    }

    // Constructor for bridged whiskers
    public WhiskerData(int whiskerNumber, float length, float diameter, float resistance, int simulationIndex, string conductor1, string conductor2)
    {
        WhiskerNumber = whiskerNumber;
        Length = length;
        Diameter = diameter;
        Resistance = resistance;
        SimulationIndex = simulationIndex;
        Conductor1 = conductor1;
        Conductor2 = conductor2;
    }
}
