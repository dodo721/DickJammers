using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class BeeSwarm : MonoBehaviour
{

    private List<Rigidbody> pushing = new List<Rigidbody>();
    private SphereCollider sphereCollider;
    
    public static List<BeeSwarm> allTheBees = new List<BeeSwarm>();
    public float pushStrength;

    [Min(0)]
    public int numBees;

    public List<Clothes> clothes = new List<Clothes>();

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

        return clothesFactor * Mathf.Pow(numBees, 2/3);
    }

    public float Noise () {
        // TODO: Fill in
        float clothesFactor = 1;

        foreach (Clothes item in clothes){
            clothesFactor *= item.GetNoiseModifier();
        }

        return clothesFactor * Mathf.Pow(numBees, 2/3);
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

        float distNoiseMod = 1 / Mathf.Pow(((this.transform.position - enemy.transform.position).magnitude), 1/2);

        return (valLOS * Visibility()) +(distNoiseMod * Noise());
    }


    // Add/remove pushing objects when they enter/leave range
    void OnTriggerEnter (Collider other) {
        if (!other.CompareTag("Player")) pushing.Add(other.GetComponent<Rigidbody>());
    }

    void OnTriggerExit (Collider other) {
        pushing.Remove(other.GetComponent<Rigidbody>());
    }

}
