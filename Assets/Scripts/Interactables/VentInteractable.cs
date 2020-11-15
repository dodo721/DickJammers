using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VentInteractable : Interactable
{
    public VentInteractable brotherVent;
    public Transform exitPoint;
    private SwarmController sCont;
    public void Start()
    {
        sCont = Camera.main.GetComponent<SwarmController>();
    }

    public override void UseInteractable(BeeSwarm bees)
    {
        base.UseInteractable(bees);
        BeeSwarm swarm = sCont.controlling;
        print(exitPoint.transform.position);
        CharacterController x = swarm.gameObject.GetComponent<CharacterController>();
        swarm.gameObject.transform.position = exitPoint.position;
    }
}
