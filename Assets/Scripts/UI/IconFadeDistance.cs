using System.Collections;
using System.Collections.Generic;
using UnityEngine;
 

public class IconFadeDistance : MonoBehaviour
{
    public List<GameObject> bees = new List<GameObject>();
    private List<BeeSwarm> allBees;
    public BeeSwarm MainSwarm;
    public float maxRange;
    public float maxSize;
    public float newScale;
    private Vector2 ownPos;

    private void Start()
    {
        Transform x = gameObject.transform;
        ownPos = new Vector2(x.position.x, x.position.z);
    }

    private void Update()
    {
        float minDist = 1000;

        foreach (BeeSwarm bee in FindObjectsOfType<BeeSwarm>())
        {
            Transform t = bee.gameObject.transform;
            Vector2 swarmPos = new Vector2 (t.transform.position.x, t.transform.position.z);
            float dist = Vector2.Distance(swarmPos, ownPos);
            if (dist < minDist) minDist = dist;
        }
        newScale = Mathf.Clamp(Mathf.Exp(-0.15f * minDist) * maxSize - 0.2f, 0, 1.12f);
        gameObject.transform.localScale = new Vector3(newScale, newScale, 1);
    }
}
