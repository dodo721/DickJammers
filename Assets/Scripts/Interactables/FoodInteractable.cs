using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodInteractable : Interactable
{
    [Min(0)]
    public int numBees;
    public float followSmooth;
    public BeeSwarm heldBy = null;

    public override void UseInteractable(BeeSwarm bees)
    {
        heldBy = bees;
    }
}
