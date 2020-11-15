using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class buttonPlatformInteractable : Interactable
{

    public MovingPlatform platform;
    public MovingPlatform platform2;
    public MovingPlatform platform3;

    protected override void OnTriggerEnter(Collider other)
    {
        base.OnTriggerEnter(other);
        if (other.tag == "Blue Crate")
        {
            if (platform != null) platform.setState(true);
            if (platform2 != null) platform2.setState(true);
            if (platform3 != null) platform3.setState(true);
        }
    }

    protected override void OnTriggerExit(Collider other)
    {
        base.OnTriggerExit(other);
        if (other.tag == "Blue Crate")
        {
            if (platform != null) platform.setState(false);
            if (platform2 != null) platform2.setState(false);
            if (platform3 != null) platform3.setState(false);
        }
    }
}
