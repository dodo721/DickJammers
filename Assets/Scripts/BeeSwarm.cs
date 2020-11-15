using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
[RequireComponent(typeof(CharacterController))]
public class BeeSwarm : MonoBehaviour
{

    public List<Rigidbody> inRange = new List<Rigidbody>();
    private SphereCollider sphereCollider;
    public float normalRadius;
    public float clothedRadius;

    public GameObject beesUnclothed;
    public GameObject beesClothed;
    bool wereBeesClothedLastFrame = false;

    public static List<BeeSwarm> allTheBees = new List<BeeSwarm>();

    public GameObject newBees;
    public GameObject newHive;

    [Min(0)]
    public int numBees;

    public float spawnSwarmDistance;

    public Clothes clothes;

    public Transform cameraTarget;
    public float lockHeight;
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
        if (HasClothes() && !wereBeesClothedLastFrame) {
            beesClothed.SetActive(true);
            beesUnclothed.SetActive(false);
            wereBeesClothedLastFrame = true;
        } else if (!HasClothes() && wereBeesClothedLastFrame) {
            beesClothed.SetActive(false);
            beesUnclothed.SetActive(true);
            wereBeesClothedLastFrame = false;
        }

        if (!HasClothes()) {
            foreach (ParticleSystem particle in particles) {
                ParticleSystem.EmissionModule emission = particle.emission;
                emission.rateOverTime = ((float)numBees / 1000f) * 100f;
                ParticleSystem.ShapeModule shape = particle.shape;
                shape.radius = ((float)numBees / 1000f) * 1.5f;
            }
        }

        if(numBees < 100)
        {
            allTheBees.Remove(this);
            Destroy(this.gameObject);
        }
    }

    public bool Split()
    {
        if(numBees >= 400)
        {   
            Vector3 newPosition = transform.position + (spawnSwarmDistance * SwarmController.i.getDirectionToMouse());

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

            if(Physics.Raycast(transform.position, (newPosition - transform.position), out hit, spawnSwarmDistance))
            {
                validSpawn = false;
            }

            if(validSpawn){
                BeeSwarm spawnedBees = Instantiate(newBees, newPosition, transform.rotation).GetComponent<BeeSwarm>();
                int newNumBees = 200;
                numBees -= newNumBees;
                spawnedBees.numBees = newNumBees; 
                spawnedBees.clothes = null;
                return true;
            }
        }
        return false;
    }

    public bool BuildHive()
    {
        if(numBees >= 400)
        {   
            Vector3 newPosition = transform.position + (spawnSwarmDistance * SwarmController.i.getDirectionToMouse());

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

            if(Physics.Raycast(transform.position, (newPosition - transform.position), out hit, spawnSwarmDistance))
            {
                validSpawn = false;
            }

            if(validSpawn){
                Hive spawnedHive = Instantiate(newHive, newPosition, newHive.transform.rotation).GetComponent<Hive>();
                int newNumBees = 200;
                numBees -= newNumBees;
                spawnedHive.numBees = newNumBees;
                return true;
            }
        }
        return false;
    }

    public float Visibility (Enemy enemy, float visFactor = 1f) {
        
        // TODO: Fill in
        float clothesFactor = 1;
        if (HasClothes())
            clothesFactor *= clothes.GetVisibilityModifier();

        RaycastHit hit;
        int valLOS = 0;

        if(Physics.Raycast(enemy.transform.position, (transform.position - enemy.transform.position), out hit))
        {
            if(hit.transform == this.transform)
            {
                valLOS = 1;
            }
        }

        return valLOS * visFactor * clothesFactor * Mathf.Pow(numBees, .5f);
    }

    public float Noise (Enemy enemy, float noiseFactor = .5f) {
        // TODO: Fill in
        float clothesFactor = 1;

        if (HasClothes())
            clothesFactor *= clothes.GetNoiseModifier();

        float distNoiseMod = 1 / Mathf.Pow(((this.transform.position - enemy.transform.position).magnitude), 2f);

        // Reduce by half when behind a wall
        float coverNoiseMod = 1f;
        RaycastHit hit;
        if (Physics.Raycast(enemy.transform.position, transform.position - enemy.transform.position, out hit)) {
            if (hit.transform != transform) {
                coverNoiseMod = 0.5f;
            }
        }

        return distNoiseMod * noiseFactor * clothesFactor * coverNoiseMod * numBees;
    }

    public float Conspicuiosness (Enemy enemy, float noiseFactor = .2f, float visFactor = .4f) {
        return Visibility(enemy, visFactor) + Noise(enemy, noiseFactor);
    }

    public void Hurt()
    {
        numBees--;
    }

    // Add/remove pushing objects when they enter/leave range
    void OnTriggerEnter (Collider other) {
        
        RaycastHit hit;
        if(Physics.Raycast(transform.position, other.transform.position - transform.position, out hit))
        {
            if (hit.collider == other) {
                if (!other.CompareTag("Player") && other.GetComponent<Rigidbody>() != null && (!other.isTrigger || other.CompareTag("Hive")))
                {
                    inRange.Add(other.GetComponent<Rigidbody>());
                }
                else if (other.GetComponent<BeeSwarm>() != null)
                {
                    // JOIN 2 SWARMS TOGETHER
                    BeeSwarm component = other.GetComponent<BeeSwarm>();
                    if (other.gameObject != this.gameObject && this != SwarmController.i.GetControlledBeeSwarm())
                    {
                        component.numBees += numBees;
                        if (component.HasClothes())
                            component.clothes.Drop();
                        allTheBees.Remove(this);
                        Destroy(this.gameObject);
                    }
                }
                
            }
        }
    }

    void OnTriggerExit (Collider other) {
        inRange.Remove(other.GetComponent<Rigidbody>());
    }

    public bool HasClothes () {
        return clothes != null;
    }

    public static int GetNumUnusedBees () {
        int sum = 0;
        foreach (BeeSwarm bees in allTheBees) {
            if (!bees.HasClothes()) {
                sum += bees.numBees;
            }
        }
        return sum;
    }

    void OnDrawGizmos ()
    {
        Gizmos.color = new Color(1f, 0f, 0f, 0.3f);
        Gizmos.DrawSphere(transform.position, GetComponent<SphereCollider>().radius);
        Gizmos.color = Color.magenta;
        Gizmos.DrawLine(transform.position, transform.position + (Vector3.forward * spawnSwarmDistance));
        Gizmos.color = new Color(0f, 0f, 1f, 0.3f);
        Gizmos.DrawSphere(transform.position + (Vector3.forward * spawnSwarmDistance), GetComponent<SphereCollider>().radius);

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
