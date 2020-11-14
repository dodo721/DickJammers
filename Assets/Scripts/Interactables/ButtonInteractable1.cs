using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonInteractable1 : Interactable
{
    public Material unpressed;
    public Material pressed;
    public List<ButtonInteractable> brotherButtons;



    // Update is called once per frame
    protected virtual void Update()
    {
        base.Update();
        RaycastHit hit;
    }
}
