using System.Collections;
using System.Collections.Generic;
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
            Destroy(this.gameObject);
        }
    }

    public bool Split()
    {
        if(numBees >= 400)
        {   
            Vector3 newPosition;

            bool validSpawn = SwarmController.i.CanBePlacedAt(out newPosition);

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
            Vector3 newPosition;

            bool validSpawn = SwarmController.i.CanBePlacedAt(out newPosition);

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
        if (HasClothes())
            clothes.Drop();
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
        foreach (BeeSwarm bees in FindObjectsOfType<BeeSwarm>()) {
            if (!bees.HasClothes()) {
                sum += bees.numBees;
            }
        }
        return sum;
    }


}
