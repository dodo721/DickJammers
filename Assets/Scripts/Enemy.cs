using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    // Start is called before the first frame update
    public float obliviousness = 10;
    public BeeSwarm targetedBees;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        targetedBees = NoticingSwarm();
        if(targetedBees != null)
        {
            //ToDo behavior for angry at bees
        }
    }

    BeeSwarm NoticingSwarm()
    {
        foreach(BeeSwarm bees in BeeSwarm.allTheBees){
            if(bees.Conspicuiosness(this) > obliviousness)
            {
                return bees;
            }
        }
        return null;
    }
}
