using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FaceMovement : MonoBehaviour
{

    public CharacterController controller;
    public float rotationSpeed;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (controller != null) {
            Vector3 lookat = Vector3.forward;
            if (SwarmController.i.GetControlledBeeSwarm() == controller.GetComponent<BeeSwarm>()) {
                lookat = SwarmController.i.direction * 10000;
            }
            Quaternion lookRot = Quaternion.LookRotation(lookat, Vector3.up);
            transform.rotation = Quaternion.Lerp(transform.rotation, lookRot, rotationSpeed * Time.deltaTime);
        }
    }
}
