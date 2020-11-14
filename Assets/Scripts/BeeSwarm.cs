using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class BeeSwarm : MonoBehaviour
{

    private List<Rigidbody> pushing = new List<Rigidbody>();
    private SphereCollider sphereCollider;

    public float pushStrength;

    [Min(0)]
    public int numBees;

    public Clothes[] clothes = new Clothes[3];

    // Start is called before the first frame update
    void Start()
    {
        sphereCollider = GetComponent<SphereCollider>();
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
        return 1;
    }

    public float Noise () {
        // TODO: Fill in
        return 1;
    }

    public float Conspicuiosness (Enemy enemy) {
        // TODO: Fill in
        return 1;
    }


    // Add/remove pushing objects when they enter/leave range
    void OnTriggerEnter (Collider other) {
        pushing.Add(other.GetComponent<Rigidbody>());
    }

    void OnTriggerExit (Collider other) {
        pushing.Remove(other.GetComponent<Rigidbody>());
    }

}
