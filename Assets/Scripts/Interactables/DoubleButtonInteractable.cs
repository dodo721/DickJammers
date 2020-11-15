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

    public GameObject uiTimer;
    public Transform uiTimerPos;
    public Transform uiTimerRot;
    private GameObject countdownObj;
   
    public override void UseInteractable(BeeSwarm bees)
    {
        base.UseInteractable(bees);
        if (bees.numBees < beesRequired) return;
        if (brotherButton.pressed)
        {
            GameObject.Destroy(lockedDoor);
            GameObject.Destroy(countdownObj);

            changeColour(2);
            brotherButton.changeColour(2);

            countdownButtons = false;
            brotherButton.countdownButtons = false;
            brotherButton.DestroyTimer();
            
            
        } else if (this.pressed == false)
        {
            this.pressed = true;
            changeColour(1);
            countdownObj = (GameObject)Instantiate(uiTimer, gameObject.transform.position, gameObject.transform.rotation);
            countdownObj.transform.localPosition = uiTimerPos.transform.position; 
            countdownObj.GetComponent<Countdown>().passParam(resetTime);
        }
    }


    public void DestroyTimer()
    {
        GameObject.Destroy(countdownObj);
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
