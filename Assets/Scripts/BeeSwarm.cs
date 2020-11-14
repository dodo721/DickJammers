using System.Collections;
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

    public static List<BeeSwarm> allTheBees = new List<BeeSwarm>();
    public float pushStrength;

    [Min(0)]
    public int numBees;

    public List<Clothes> clothes = new List<Clothes>();

    public Transform cameraTarget;

    // Start is called before the first frame update
    void Start()
    {
        sphereCollider = GetComponent<SphereCollider>();
        allTheBees.Add(this);
    }

    // Update is called once per frame
    void Update()
    {
        
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
        if (!other.CompareTag("Player") && other.GetComponent<Rigidbody>() != null) pushing.Add(other.GetComponent<Rigidbody>());
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
