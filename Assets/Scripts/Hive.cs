using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hive : MonoBehaviour
{

    public static List<Hive> allTheHives = new List<Hive>();

    public int numBees;

    // Start is called before the first frame update
    void Start()
    {
        allTheHives.Add(this);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
