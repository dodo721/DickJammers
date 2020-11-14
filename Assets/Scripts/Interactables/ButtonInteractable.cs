using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonInteractable : Interactable
{

    public GameObject LockedDoor;
    public Material AfterInteractColour;

    Canvas messageCanvas;

    private void Start()
    {

    }

    public override void UseInteractable()
    {
        base.UseInteractable();
        GetComponent<Renderer>().material = AfterInteractColour;
        GameObject.Destroy(LockedDoor);
    }

    protected virtual void Update()
    {
        base.Update();
        if (canInteract) { DisplayInteractUI(); } else{ RemoveInteractUI();}
    }

    public override void DisplayInteractUI()
    {
        base.DisplayInteractUI();


    }

    public override void RemoveInteractUI()
    {
        base.RemoveInteractUI();

    }
}
