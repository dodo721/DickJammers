using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class OtherSwarmCounter : MonoBehaviour
{
    private SwarmController swarmCont;
    public GameObject floatingBeeCounterPrefab;
    private List<GameObject> floatingBeeCounter = new List<GameObject>();
    public BeeSwarm lastSwarm;
    

    private void Start()
    {
        swarmCont = Camera.main.GetComponent<SwarmController>();
    }

    void Update()
    {
        BeeSwarm currentSwarm = swarmCont.GetControlledBeeSwarm();
        if (GameObject.ReferenceEquals(lastSwarm, currentSwarm) == false)
        {
            lastSwarm = currentSwarm;
            foreach(GameObject obj in floatingBeeCounter)
            {
                GameObject.Destroy(obj);
            }

            List<BeeSwarm> otherSwarms = BeeSwarm.allTheBees;
            if (otherSwarms.Count == 1) return;
            otherSwarms.Remove(currentSwarm);

            foreach (BeeSwarm swarm in otherSwarms)
            {
                print(otherSwarms.Count);
                int beeCount = swarm.numBees;
                var obj = (GameObject)Instantiate(floatingBeeCounterPrefab, swarm.transform.position, Quaternion.Euler(45,45,0));
                GameObject x = obj.GetComponent<Wack>().getfloatObject();
                TextMeshPro tPro = x.GetComponent<TextMeshPro>();
                tPro.text = "BEANS";
                floatingBeeCounter.Add(obj);
            }
        }
    }
}
