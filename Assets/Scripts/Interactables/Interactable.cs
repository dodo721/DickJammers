using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Interactable : MonoBehaviour
{
    public bool hasInteracted = false;
    public bool canInteract = false;
    public bool canSelect = false;

    public int beesRequired = 0;
    public int beesConsumed = 0;

    public InteractableAlert alert;

    public virtual void UseInteractable(BeeSwarm bees)
    {
        // Overwrite this for each interactable
    }

    public virtual void DisplayInteractUI()
    {
        // Do later
    }

    public virtual void RemoveInteractUI()
    {
        // Do later
    }

    protected virtual void OnTriggerEnter(Collider other)
    {

        if (other.gameObject.tag == "Player")
        {
            canInteract = true;
        }

    }

    protected virtual void OnTriggerExit(Collider other)
    {
        canInteract = false;
        canSelect = false;
    }

    protected virtual void Update()
    {
        // Check to see if we're in posistion to interact
        if (canInteract)
        {
            // Check to see if the mouse is over the interactable
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            int layerMask = SwarmController.LAYER_MASK_IGNORE_PLAYER;
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask))
            {
                if (hit.rigidbody != null)
                {
                    canSelect = ReferenceEquals(hit.rigidbody.gameObject, gameObject);
                    //Debug.Log("hit ourselves!");
                }
                else canSelect = false;
            }
            else canSelect = false;

            BeeSwarm interactor = SwarmController.i.GetControlledBeeSwarm();
            // If we can select and are wanting to interact with the interactable...
            if (canSelect && Input.GetButtonDown("Fire1"))
            {
                if (interactor.numBees >= beesRequired) {
                    UseInteractable(interactor);
                    hasInteracted = true;
                } else if (alert != null) {
                    alert.Alert(beesRequired + " bees required", 2);
                }
            }
        }
    }
}
