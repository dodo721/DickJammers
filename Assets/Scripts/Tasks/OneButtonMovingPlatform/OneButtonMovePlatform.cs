using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OneButtonMovePlatform : Interactable
{
    public MovingPlatform platform;
    public MovingPlatform platform2;
    public MovingPlatform platform3;

    public Material InteractColourOne;
    public Material InteractColourTwo;

    private Material[] colours = new Material[2];
    private int i; 

    private void Start()
    {
        i = 0;
        colours[0] = InteractColourTwo;
        colours[1] = InteractColourOne;
    }


    public override void UseInteractable(BeeSwarm bees)

    {
        if (bees.numBees < beesRequired) return;
        Material col = colours[i % 2];
        i++;
        base.UseInteractable(bees);
        GetComponent<Renderer>().material = col;
        if (platform != null) platform.toggleState();
        if (platform2 != null) platform2.toggleState();
        if (platform3 != null) platform3.toggleState();
    }
}
