using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SwarmBeeCounter : MonoBehaviour
{

    public int beeCounter = 0;
    public GameObject countText;
    private TextMeshPro tMesh;
    SwarmController swarmCont;
    private void Start()
    {
        swarmCont = Camera.main.GetComponent<SwarmController>();
        tMesh = countText.GetComponent<TextMeshPro>();
    }



    private void Update()
    {
        BeeSwarm curSwarm = swarmCont.GetControlledBeeSwarm();
        tMesh.text = "Bees in swarm\n" + curSwarm.numBees.ToString();
    }
}
