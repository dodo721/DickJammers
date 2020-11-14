using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FaceMovement : MonoBehaviour
{

    public CharacterController controller;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (controller) transform.LookAt(transform.position + controller.velocity);
    }
}
