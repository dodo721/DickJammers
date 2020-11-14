using System.Collections;
using System.Collections.Generic;
using UnityEngine;
 

public class IconFadeDistance : MonoBehaviour
{
    public List<GameObject> bees = new List<GameObject>();
    private Transform ownPos;
    private List<BeeSwarm> allBees;
    public BeeSwarm MainSwarm;
    public float maxRange;
    public float maxSize;
    public float newScale;

    private void Update()
    {
        float minDist = 1000;

        foreach (BeeSwarm bee in BeeSwarm.allTheBees)
        {
            Vector3 swarmPos = bee.gameObject.transform.position;
            float dist = Vector3.Distance(swarmPos, gameObject.transform.position);
            if (dist < minDist) minDist = dist;
        }
        newScale = Mathf.Clamp(Mathf.Exp(-0.25f * minDist) * maxSize - 0.1f, 0, maxSize);
        gameObject.transform.localScale = new Vector3(newScale, newScale, 1);
    }
}
