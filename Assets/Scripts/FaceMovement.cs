using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FaceMovement : MonoBehaviour
{

    public CharacterController controller;
    public float rotationMagnitude;
    public float rotationSpeed;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (controller) {
            transform.LookAt(transform.position + controller.velocity);
            /*transform.localEulerAngles = Vector3.Lerp(
                transform.localEulerAngles,
                new Vector3(controller.velocity.magnitude * rotationMagnitude, 0, 0),
                rotationSpeed * Time.deltaTime);*/
        }
    }
}
