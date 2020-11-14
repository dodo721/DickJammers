using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Clothes : Interactable
{

    public float visibilityMod;
    public float noiseMod;
    public float speedMod;

    public float GetVisibilityModifier() {
        return visibilityMod;
    }
    public float GetRequiredBees() {
        return beesRequired;
    }
    public float GetNoiseModifier() {
        return noiseMod;
    }

    public float GetSpeedModifier () {
        return speedMod;
    }

    public override void UseInteractable()
    {
        
    }
}
