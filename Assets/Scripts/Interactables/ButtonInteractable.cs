using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonInteractable : Interactable
{

    public GameObject LockedDoor;
    public Material AfterInteractColour;

    public override void UseInteractable(BeeSwarm bees)

    {
        base.UseInteractable(bees);
        GetComponent<Renderer>().material = AfterInteractColour;
        GameObject.Destroy(LockedDoor);
    }
}
