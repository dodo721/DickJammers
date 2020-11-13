using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Clothes : MonoBehaviour
{
    public abstract float GetVisibilityModifier();
    public abstract float GetRequiredBees();
    public abstract float GetNoiseModifier();
}
