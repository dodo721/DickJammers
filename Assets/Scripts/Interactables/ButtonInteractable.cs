using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonInteractable : Interactable
{

    public GameObject LockedDoor;
    public Material AfterInteractColour;

    public override void UseInteractable()
    {
        base.UseInteractable();
        GetComponent<Renderer>().material = AfterInteractColour;
        GameObject.Destroy(LockedDoor);
    }
}
