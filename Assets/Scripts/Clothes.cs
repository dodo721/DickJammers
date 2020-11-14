using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(FaceMovement))]
[RequireComponent(typeof(Rigidbody))]
public class Clothes : Interactable
{

    public float visibilityMod;
    public float noiseMod;
    public float speedMod;
    private FaceMovement faceMovement;
    private Rigidbody rb;
    public float bobMagnitude;
    public float bobFrequency;

    void Start () {
        faceMovement = GetComponent<FaceMovement>();
        rb = GetComponent<Rigidbody>();
    }

    public override void ExtendUpdate () {
        transform.Translate(Vector3.down * Time.deltaTime * Mathf.Sin(Time.time * bobFrequency) * bobMagnitude);
    }

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

    public override void UseInteractable(BeeSwarm bees)
    {
        bees.clothes.Add(this);
        transform.rotation = bees.transform.rotation;
        faceMovement.controller = bees.GetComponent<CharacterController>();
        faceMovement.enabled = true;
        transform.SetParent(bees.transform);
        transform.localPosition = Vector3.zero;
        rb.isKinematic = true;
    }
}
