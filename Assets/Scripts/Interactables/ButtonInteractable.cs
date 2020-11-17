using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonInteractable : Interactable
{

    public GameObject LockedDoor;
    public Material AfterInteractColour;
    public InteractableAlert alert;
    
    public override void UseInteractable(BeeSwarm bees)

    {
        base.UseInteractable(bees);
        if (bees.numBees < beesRequired) {
            alert.Alert(beesRequired + " bees required", 5);
        };
        GetComponent<Renderer>().material = AfterInteractColour;
        GameObject.Destroy(LockedDoor);
    }
}
