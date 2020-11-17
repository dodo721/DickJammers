using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hive : MonoBehaviour
{

    public int numBees;

    public GameObject destroyedHive;

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter (Collider other) {
        Food food = other.GetComponent<Food>();
        if (food != null) {
            numBees += food.numBees;
            Destroy(food.gameObject);
        }
    }

    public void Die (BeeSwarm killer) {
        Debug.Log("Destroying!");
        if (destroyedHive != null) {
            Debug.Log("Adding broken hive!");
            Instantiate(destroyedHive, transform.position, destroyedHive.transform.rotation);
        }
        killer.numBees += numBees;
        Destroy(gameObject);
    }
}
