using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BeeCounter : MonoBehaviour
{

    public int beeCounter = 0;
    public GameObject countText;
    private TextMeshPro tMesh;

    private void Start()
    {
        tMesh = countText.GetComponent<TextMeshPro>();
    }

    

    private void Update()
    {
        int locCounter = 0;
        foreach(BeeSwarm swarm in BeeSwarm.allTheBees)
        {
            locCounter += swarm.numBees;
        }
        beeCounter = locCounter;
        tMesh.text = "Total Bees\n" + beeCounter.ToString();
    }
}
