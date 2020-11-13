using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeeSwarm : MonoBehaviour
{

    [Min(0)]
    public int numBees;

    public List<Clothes> clothes = new List<Clothes>();

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
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
            if(hit.transform == this)
            {
                valLOS = 1;
            }
        }

        float distNoiseMod = (this.transform.position - enemy.transform.position).magnitude;
        return (valLOS * Visibility()) + Noise();
    }

    void OnTriggerEnter (Collider other) {

    }

}
