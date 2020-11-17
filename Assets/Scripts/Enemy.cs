using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class Enemy : MonoBehaviour
{
    // Start is called before the first frame update
    public float obliviousness;
    public bool alert = false;
    public float countDown;
    private float ogCountdown;
    public float speed;
    public float distractDist;
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
                if((targetedBees.transform.position - agent.transform.position).magnitude < 2.5f) targetedBees.Hurt();
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
        BeeSwarm mostConspicBee = null;
        foreach(BeeSwarm bees in FindObjectsOfType<BeeSwarm>()){

            float tempVis = 0;

            if(inVision.Contains(bees)) tempVis = bees.Visibility(this);

            float tempNoise = bees.Noise(this);
            
            noiseLevel += tempNoise;

            float tempConspic = tempVis + tempNoise;

            if (tempConspic > maxConspicuousness)
            {
                mostConspicBee = bees;
                maxConspicuousness = tempConspic;
            } 

        }

        if(noiseLevel > obliviousness) alert = true;
        else alert = false;

        if(alert)
        {
            indicator.adjust = Mathf.Clamp(maxConspicuousness / (obliviousness * .7f), 0f, 1f);
            if(maxConspicuousness > obliviousness * .7f) return mostConspicBee;
        } 
        else 
        {
            indicator.adjust = Mathf.Clamp(maxConspicuousness / obliviousness, 0f, 1f);
            if(maxConspicuousness > obliviousness) return mostConspicBee;
        }
        return null;
    }

    public void Distract (CollisionSounds obj) {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, (obj.transform.position - transform.position), out hit)) {
            if (hit.collider.GetComponent<CollisionSounds>() == obj) {
                if (Vector3.Distance(obj.transform.position, transform.position) <= distractDist) {
                    agent.SetDestination(obj.transform.position);
                }
            }
        }
    }

    void OnDrawGizmos () {
        UnityEngine.Gizmos.color = Color.red;
        UnityEngine.Gizmos.DrawLine(transform.position, transform.position + (Vector3.forward * distractDist));
    }
    
}
