using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class Enemy : MonoBehaviour
{
    // Start is called before the first frame update
    public float obliviousness = 20;
    public bool alert = false;
    public float countDown = 1f;
    private float ogCountdown;
    public float speed;
    public BeeSwarm targetedBees;
    public float noiseLevel;
    private NavMeshAgent agent;
    public AlertExclamationMark indicator;
    public List<BeeSwarm> inVision = new List<BeeSwarm>();


    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        ogCountdown = countDown;
    }

    // Update is called once per frame
    void Update()
    {
        targetedBees = NoticingSwarm();
        if(targetedBees != null)
        {
            if(countDown > 0)
                countDown -= Time.deltaTime;
            else
            {
                indicator.adjust = 1;
                agent.SetDestination(targetedBees.transform.position);
            }
            //ToDo damage, pathfinding
        }
        else
        {
            countDown = ogCountdown;
            //ToDo normal behavior
        }
    }

    void OnTriggerEnter (Collider other) {
        if (other.CompareTag("Player")) {
            if (!inVision.Contains(other.GetComponent<BeeSwarm>()))
                inVision.Add(other.GetComponent<BeeSwarm>());
        }
    }

    void OnTriggerExit (Collider other) {
        if (other.CompareTag("Player")) {
            inVision.Remove(other.GetComponent<BeeSwarm>());
        }
    }

    BeeSwarm NoticingSwarm()
    {
        noiseLevel = 0;
        float maxConspicuousness = 0f;
        BeeSwarm toReturn = null;
        foreach(BeeSwarm bees in BeeSwarm.allTheBees){

            noiseLevel += bees.Noise(this);
            float conspic = bees.Conspicuiosness(this);
            if (conspic > maxConspicuousness) maxConspicuousness = conspic;
            if(conspic > obliviousness || (alert && bees.Conspicuiosness(this) > obliviousness * 2/3))
            {
                alert = true;
                toReturn = bees;
            }
        }

        if (!alert) {
            if(noiseLevel > obliviousness) alert = true;
            else alert = false;
        }

        indicator.adjust = Mathf.Clamp(maxConspicuousness / obliviousness, 0f, 1f);

        return toReturn;
    }


    
}
