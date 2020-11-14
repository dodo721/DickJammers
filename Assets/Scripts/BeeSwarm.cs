﻿using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
[RequireComponent(typeof(CharacterController))]
public class BeeSwarm : MonoBehaviour
{

    private List<Rigidbody> pushing = new List<Rigidbody>();
    private SphereCollider sphereCollider;
    public float normalRadius;
    public float clothedRadius;

    public GameObject beesUnclothed;
    public GameObject beesClothed;
    bool wereBeesClothedLastFrame = false;

    public static List<BeeSwarm> allTheBees = new List<BeeSwarm>();
    public float pushStrength;

    public GameObject newBees;

    [Min(0)]
    public int numBees;

    public List<Clothes> clothes = new List<Clothes>();

    public Transform cameraTarget;
    private float lockHeight;
    public List<ParticleSystem> particles;

    // Start is called before the first frame update
    void Start()
    {
        sphereCollider = GetComponent<SphereCollider>();
        allTheBees.Add(this);
        SwarmController.i.SetControlledBeeSwarm(this);
        lockHeight = transform.position.y;
    }

    // Update is called once per frame
    void Update()
    {
        if (clothes.Count > 0 && !wereBeesClothedLastFrame) {
            beesClothed.SetActive(true);
            beesUnclothed.SetActive(false);
            wereBeesClothedLastFrame = true;
        } else if (clothes.Count == 0 && wereBeesClothedLastFrame) {
            beesClothed.SetActive(false);
            beesUnclothed.SetActive(true);
            wereBeesClothedLastFrame = false;
        }

        if (clothes.Count == 0) {
            foreach (ParticleSystem particle in particles) {
                ParticleSystem.EmissionModule emission = particle.emission;
                emission.rateOverTime = ((float)numBees / 1000f) * 100f;
                ParticleSystem.ShapeModule shape = particle.shape;
                shape.radius = ((float)numBees / 1000f) * 1.5f;
            }
        }
    }

    public bool Split()
    {
        if(numBees >= 400)
        {   
            Vector3 newPosition = transform.position + (3* SwarmController.i.getDirectionToMouse());

            bool validSpawn = true;

            foreach(BeeSwarm bees in allTheBees){
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

            if(validSpawn){
                BeeSwarm spawnedBees = Instantiate(newBees, newPosition, transform.rotation).GetComponent<BeeSwarm>();
                numBees /= 2;
                spawnedBees.numBees = this.numBees;
                spawnedBees.clothes = new List<Clothes>();
                return true;
            }
        }
        return false;
    }

    // Runs every PHYSICS frame
    void FixedUpdate () {
        foreach (Rigidbody toPush in pushing) {
            // Push each object in an outward direction from the swarm center,
            // TODO: maths????
            Vector3 force = (toPush.transform.position - transform.position);
            force *= pushStrength;
            toPush.AddForceAtPosition(force, transform.position, ForceMode.Acceleration);
        }
        transform.Translate(Vector3.down * (transform.position.y - lockHeight));
    }

    public float Visibility () {
        // TODO: Fill in
        float clothesFactor = 1;

        foreach (Clothes item in clothes){
            clothesFactor *= item.GetVisibilityModifier();
        }

        return clothesFactor * Mathf.Pow(numBees, .5f);
    }

    public float Noise () {
        // TODO: Fill in
        float clothesFactor = 1;

        foreach (Clothes item in clothes){
            clothesFactor *= item.GetNoiseModifier();
        }

        return clothesFactor * numBees;
    }

    public float Conspicuiosness (Enemy enemy) {
        // TODO: Fill in
        RaycastHit hit;
        int valLOS = 0;

        if(Physics.Raycast(enemy.transform.position, (transform.position - enemy.transform.position), out hit))
        {
            if(hit.transform == this.transform)
            {
                valLOS = 1;
            }
        }

        float distNoiseMod = 1 / Mathf.Pow(((this.transform.position - enemy.transform.position).magnitude), 2f);
        float noiseFactor = .2f;

        float visFactor = .2f;

        return (valLOS * visFactor * Visibility()) +(distNoiseMod * noiseFactor * Noise());
    }


    // Add/remove pushing objects when they enter/leave range
    void OnTriggerEnter (Collider other) {
        if (!other.CompareTag("Player") && other.GetComponent<Rigidbody>() != null && !other.isTrigger)
            pushing.Add(other.GetComponent<Rigidbody>());
        else 
        {
            BeeSwarm component = other.GetComponent<BeeSwarm>();
            if (component != null && other.gameObject != this.gameObject && this != SwarmController.i.GetControlledBeeSwarm())
            {
                component.numBees += numBees;
                allTheBees.Remove(this);
                Destroy(this.gameObject);
            }
        }
    }

    void OnTriggerExit (Collider other) {
        pushing.Remove(other.GetComponent<Rigidbody>());
    }

    void OnDrawGizmos ()
    {
        Handles.DrawWireDisc(transform.position, Vector3.up, numBees / 5);
        Handles.DrawWireDisc(transform.position, Vector3.up, numBees / 20);
        Handles.DrawWireDisc(transform.position, Vector3.up, numBees / 45);
        Handles.DrawWireDisc(transform.position, Vector3.up, numBees / 80);
        Handles.DrawWireDisc(transform.position, Vector3.up, numBees / 125);
        Handles.DrawWireDisc(transform.position, Vector3.up, numBees / 180);
        Handles.DrawWireDisc(transform.position, Vector3.up, numBees / 245);
        Handles.DrawWireDisc(transform.position, Vector3.up, numBees / 320);
        Handles.DrawWireDisc(transform.position, Vector3.up, numBees / 405);
        Handles.DrawWireDisc(transform.position, Vector3.up, numBees / 500);
    }


}
