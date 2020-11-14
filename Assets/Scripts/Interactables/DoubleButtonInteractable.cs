using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoubleButtonInteractable : Interactable
{
    public DoubleButtonInteractable brotherButton;
    public GameObject lockedDoor;
    public bool pressed;
    public bool countdownButtons;
    public float resetTime;
    public float timer;

    public Material InitialColour;
    public Material CountingDownColour;
    public Material FullyInteractedColour;
    private Material[] colList = new Material[3];

    public override void UseInteractable()
    {
        base.UseInteractable();
        if (brotherButton.pressed)
        {
            print(brotherButton.pressed);
            GameObject.Destroy(lockedDoor);
            changeColour(2);
            brotherButton.changeColour(2);

            countdownButtons = false;
            brotherButton.countdownButtons = false;
        } else {
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

    protected override void Update()
    {
        base.Update();
        if (countdownButtons && pressed)
        {
            timer += Time.deltaTime;
            if(timer >= resetTime)
            {
                timer = 0;
                pressed = false;
                changeColour(0);
                brotherButton.changeColour(0);
            }
        }

    }
}
