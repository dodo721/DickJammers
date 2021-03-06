﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerTest : MonoBehaviour
{
    public bool hasInteracted = false;
    public bool canInteract = false;
    public bool canSelect = false;

    public virtual void UseInteractable()
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

    void OnTriggerEnter(Collider other)
    {

        if (other.gameObject.tag == "Player")
        {
            DisplayInteractUI();
            canInteract = true;
        }

    }

    private void OnTriggerExit(Collider other)
    {
        canInteract = false;
        canSelect = false;
    }

    private void Update()
    {
        // Check to see if we're in posistion to interact
        if (canInteract)
        {
            // Check to see if the mouse is over the interactable
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.rigidbody != null)
                {
                    canSelect = ReferenceEquals(hit.rigidbody.gameObject, gameObject);
                }
                else canSelect = false;
            }
            else canSelect = false;

            // If we can select and are wanting to interact with the interactable...
            if (canSelect && Input.GetKey(KeyCode.Mouse0))
            {
                UseInteractable();
            }
        }
    }
}
