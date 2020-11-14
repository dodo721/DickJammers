using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hive : MonoBehaviour
{

    public static List<Hive> allTheHives = new List<Hive>();

    public int numBees;

    public GameObject destroyedHive;

    // Start is called before the first frame update
    void Start()
    {
        allTheHives.Add(this);
    }

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

    public void Die () {
        if (destroyedHive != null) {
            Instantiate(destroyedHive, transform.position, destroyedHive.transform.rotation);
        }
        Destroy(gameObject);
    }
}
