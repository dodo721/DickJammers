using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollower : MonoBehaviour
{

    public Transform target;
    public float speed;
    public float smooth;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (target != null)
            transform.position = Vector3.Lerp(transform.position, target.position, smooth * Time.deltaTime);
    }
}
