using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    // Start is called before the first frame update
    public float obliviousness = 20;
    public bool alert = false;
    public float countDown = 1f;
    public float speed;
    public BeeSwarm targetedBees;
    public float noiseLevel;



    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        targetedBees = NoticingSwarm();
        if(targetedBees != null)
        {
            countDown -= Time.deltaTime;

            if(countDown < 0)
            {
                Vector3 direction = (targetedBees.transform.position - this.transform.position).normalized;
                Vector3 translationWorldSpace = direction * speed * Time.deltaTime;
                this.transform.position = this.transform.position + translationWorldSpace;
            }
            //ToDo damage, pathfinding
        }
        else
        {
            countDown = 2f;
            //ToDo normal behavior
        }
    }

    BeeSwarm NoticingSwarm()
    {
        noiseLevel = 0;
        foreach(BeeSwarm bees in BeeSwarm.allTheBees){

            noiseLevel += bees.Noise(this);

            if(bees.Conspicuiosness(this) > obliviousness || (alert && bees.Conspicuiosness(this) > obliviousness * 2/3))
            {
                alert = true;
                return bees;
            }
        }

        if(noiseLevel > obliviousness) alert = true;
        else alert = false;

        return null;
    }


    
}
