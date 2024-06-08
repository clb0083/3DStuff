using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using System.Globalization;
using System.IO;
using System.IO.Enumeration;
using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class UIScript : MonoBehaviour
{
    public TMP_InputField lengthMu;
    public TMP_InputField widthMu;
    public TMP_InputField lengthSigma;
    public TMP_InputField widthSigma;
    public TMP_InputField numWhiskers;
    public TMP_InputField xCoord;
    public TMP_InputField yCoord;
    public TMP_InputField zCoord;
    public TMP_InputField numIterations;
    public TextMeshProUGUI whiskerMat;
    public TextMeshProUGUI distributionSelect;
    public TextMeshProUGUI gravSelect;
    public TMP_InputField shockAmp;
    public TMP_InputField shockFreq;
    public TMP_InputField shockDur;
    public TMP_InputField vibrAmp;
    public TMP_InputField vibrFreq;
    public TMP_InputField vibrDur;
    public GameObject whisker;
    public TextMeshProUGUI totalBridges;
    public int bridgesDetected;
    public TextMeshProUGUI bridgesEachRun;
    public int bridgesPerRun;
    
    //for monte carlo stuff \/
    public float simtimeElapsed;
    public bool startSim;
    public int simIntComplete;
    public float simTimeThresh;
    public TMP_InputField totalRuns;
    
    public float moveSpeed = 5f;

    // Start is called before the first frame update
    void Start()
    {
       // Time.timeScale = 5f;
    }

    // Update is called once per frame
    void Update()
    {
        totalBridges.text = "Total Bridges: " + bridgesDetected.ToString();
        bridgesEachRun.text = "This Run: "+ bridgesPerRun.ToString();
        print(bridgesDetected);
        // Move the camera based on user input
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        Vector3 moveDirection = new Vector3(horizontalInput, 0f, verticalInput).normalized;
        transform.Translate(moveDirection * moveSpeed * Time.deltaTime);
//MONTECARLO
        simtimeElapsed += Time.deltaTime;

        if(startSim)
        {   
            
            if(simIntComplete < Convert.ToInt32(totalRuns.text))
            {
                              
                if(simtimeElapsed > simTimeThresh)
                {
                    ReloadWhiskersButton();
                                       
                    simIntComplete +=1;
                    simtimeElapsed = 0f;                    
                }
            }

            if(simIntComplete == Convert.ToInt32(totalRuns.text))
            {
                startSim = false; 
            }
        }
    }

    public float LengthDistributionGenerate()
    {
        float lenghtVal = RandomFromDistribution.RandomNormalDistribution(float.Parse(lengthMu.text),float.Parse(lengthSigma.text));
        return (lenghtVal);
    }

    public float WidthDistributionGenerate()
    {
        float widthVal = RandomFromDistribution.RandomNormalDistribution(float.Parse(widthMu.text),float.Parse(widthSigma.text));
        return (widthVal); 
    }

    public void MakeWhiskerButton()
    {
        for(int i = 0; i < Convert.ToInt32(numWhiskers.text); i++)
            {
                float diameter = WidthDistributionGenerate();
                float spawnPointX = UnityEngine.Random.Range(-float.Parse(xCoord.text),float.Parse(xCoord.text));
                float spawnPointY = UnityEngine.Random.Range(1,Convert.ToInt32(yCoord.text)); //can remove range to this if needed.
                float spawnPointZ = UnityEngine.Random.Range(-float.Parse(zCoord.text),float.Parse(zCoord.text));

                Vector3 spawnPos = new Vector3(spawnPointX,spawnPointY,spawnPointZ);

                GameObject whiskerClone = Instantiate(whisker,spawnPos,Quaternion.Euler(UnityEngine.Random.Range(0, 360),  UnityEngine.Random.Range(0, 360), UnityEngine.Random.Range(0, 360)));
                whiskerClone.tag = "whiskerClone";

                whiskerClone.transform.localScale = new Vector3(diameter,LengthDistributionGenerate(),diameter);
            }
    }
    public void ReloadWhiskersButton()
    {
        GameObject[] allWhiskers;
        allWhiskers = GameObject.FindGameObjectsWithTag("whiskerClone");
        foreach (GameObject whisk in allWhiskers)
        {
            Destroy(whisk.gameObject); // Remove the object from the scene
        }
        bridgesPerRun = 0;
        MakeWhiskerButton();
    }
}
