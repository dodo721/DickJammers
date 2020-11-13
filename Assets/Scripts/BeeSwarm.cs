using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeeSwarm : MonoBehaviour
{

    [Min(0)]
    public int numBees;

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

    void OnTriggerEnter (Collider other) {
        
    }

}
