using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    private Vector3 defaultPos;
    public Vector3 changeInPos;
    private Vector3 newPos;
    public bool state;
    public Vector3 x;

    private void Start()
    {
        defaultPos = dCopy(gameObject.transform.position);
        state = false;
        newPos = dCopy(gameObject.transform.position);
        newPos +=  changeInPos;
    }

    public void toggleState()
    {
        state = !state;
    }

    private void Update()
    {
        x = defaultPos;
        if (state == false)
        {
            this.transform.position = Vector3.Lerp(this.transform.position, defaultPos, Time.deltaTime);
        }
        else
        {
            this.transform.position = Vector3.Lerp(this.transform.position, newPos, Time.deltaTime);
        }
    }

    private Vector3 dCopy(Vector3 v)
    {
        return new Vector3(v.x, v.y, v.z);
    }
     
}
