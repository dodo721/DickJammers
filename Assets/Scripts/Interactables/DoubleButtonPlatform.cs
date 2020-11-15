using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoubleButtonPlatform : Interactable
{
    public DoubleButtonPlatform brotherButton;
    public bool pressed;

    public Material InitialColour;
    public Material CountingDownColour;
    public Material FullyInteractedColour;
    private Material[] colList = new Material[3];

    public MovingPlatform platform1;
    public MovingPlatform platform2;
    public MovingPlatform platform3;


    public override void UseInteractable(BeeSwarm bees)
    {
        base.UseInteractable(bees);
        if (bees.numBees < beesRequired) return;
        if (!this.pressed && brotherButton.pressed)
        {
            
            changeColour(2);
            brotherButton.changeColour(2);

            if (platform1 != null) platform1.setState(true);
            if (platform2 != null) platform2.setState(true);
            if (platform3 != null) platform3.setState(true);

            this.pressed = true;

        }
        else if(this.pressed && brotherButton.pressed)
        {
            this.pressed = false;
            changeColour(0);
            brotherButton.changeColour(1);
            if (platform1 != null) platform1.setState(false);
            if (platform2 != null) platform2.setState(false);
            if (platform3 != null) platform3.setState(false);
        }
        else if(this.pressed && !brotherButton.pressed)
        {
            this.pressed = false;
            changeColour(0);
        }
        else if(!this.pressed && !brotherButton.pressed)
        {
            this.pressed = true;
            changeColour(1);
        }

    }

    public void Start()
    {
        colList[0] = InitialColour;
        colList[1] = CountingDownColour;
        colList[2] = FullyInteractedColour;
    }

    public void changeColour(int newCol)
    {
        GetComponent<Renderer>().material = colList[newCol];
    }
}
