using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class FaceForward : MonoBehaviour
{
    public Transform reference;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (reference != null) transform.LookAt(transform.position + reference.forward);
    }
}
