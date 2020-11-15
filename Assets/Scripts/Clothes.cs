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
    public BeeSwarm wornBy;
    private Rigidbody rb;
    public float bobMagnitude;
    public float bobFrequency;
    public bool equipped = false;
    public List<Collider> toDisable = new List<Collider>();

    void Start () {
        faceMovement = GetComponent<FaceMovement>();
        rb = GetComponent<Rigidbody>();
    }

    protected override void Update () {
        base.Update();
        if (hasInteracted) {
            transform.Translate(Vector3.down * Time.deltaTime * Mathf.Sin(Time.time * bobFrequency) * bobMagnitude);
        }
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
        if (!equipped) {
            bees.clothes = this;
            transform.rotation = bees.transform.rotation;
            faceMovement.controller = bees.GetComponent<CharacterController>();
            faceMovement.enabled = true;
            transform.SetParent(bees.transform);
            transform.localPosition = Vector3.zero;
            rb.isKinematic = true;
            equipped = true;
            wornBy = bees;
            foreach (Collider collider in toDisable) {
                collider.enabled = false;
            }
        }
    }

    public bool Drop()
    {
        if(equipped)
        {   
            Vector3 newPosition = transform.position + (wornBy.spawnSwarmDistance * SwarmController.i.getDirectionToMouse());

            bool validSpawn = true;

            foreach(BeeSwarm bees in BeeSwarm.allTheBees){
                if((bees.transform.position - newPosition).magnitude < 2) validSpawn = false;
            }

            RaycastHit hit;
            if(Physics.Raycast(newPosition, (transform.position - newPosition), out hit))
            {
                if(!(hit.transform == this.transform))
                {
                    validSpawn = false;
                }
            }

            if(Physics.Raycast(transform.position, (newPosition - transform.position), out hit, wornBy.spawnSwarmDistance))
            {
                validSpawn = false;
            }

            if(validSpawn){
                wornBy.clothes = null;
                rb.isKinematic = false;
                faceMovement.controller = null;
                faceMovement.enabled = false;
                equipped = false;
                transform.SetParent(null);
                transform.position = newPosition;
                foreach (Collider collider in toDisable) {
                    collider.enabled = true;
                }
                return true;
            }
        }
        return false;
    }
}
