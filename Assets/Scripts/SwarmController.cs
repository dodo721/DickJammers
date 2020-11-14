using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CameraFollower))]
public class SwarmController : MonoBehaviour
{

    public BeeSwarm controlling;
    public Vector2 mousePos;
    public Transform cameraTransform;
    public CameraFollower cameraFollower;
    public float speed;
    private CharacterController controller;
    public static SwarmController i;

    void Awake () {
        i = this;
    }

    void Start () {
        cameraFollower = GetComponent<CameraFollower>();
    }

    // Update is called once per frame
    void Update()
    {
        if (controlling != null && controller == null) {
            controller = controlling.GetComponent<CharacterController>();
        }

        if(Input.GetButtonDown("Fire2")) {
            controlling.Split();
        }

        if(Input.GetButtonDown("Q")){
            int index = BeeSwarm.allTheBees.IndexOf(controlling);
            index = (BeeSwarm.allTheBees.Count + index - 1) % BeeSwarm.allTheBees.Count;
            SetControlledBeeSwarm(BeeSwarm.allTheBees[index]);
        }

        if(Input.GetButtonDown("E")){
            int index = BeeSwarm.allTheBees.IndexOf(controlling);
            index = (index + 1) % BeeSwarm.allTheBees.Count;
            SetControlledBeeSwarm(BeeSwarm.allTheBees[index]);
        }

        Vector3 direction = new Vector3();

        if (Input.GetKey(KeyCode.W)) {
            direction += Vector3.forward;
        }
        if (Input.GetKey(KeyCode.S)) {
            direction += Vector3.back;
        }
        if (Input.GetKey(KeyCode.A)) {
            direction += Vector3.left;
        }
        if (Input.GetKey(KeyCode.D)) {
            direction += Vector3.right;
        }

        direction.Normalize();

        Vector3 translationWorldSpace = direction * speed * Time.deltaTime;
        Vector3 translationCameraSpace = cameraTransform.TransformDirection(translationWorldSpace);
        controller.Move(translationCameraSpace);
    }

    public BeeSwarm GetControlledBeeSwarm () {
        return controlling;
    }
    public void SetControlledBeeSwarm (BeeSwarm bees) {
        controlling = bees;
        controller = bees.GetComponent<CharacterController>();
        cameraFollower.target = bees.cameraTarget;
    }
}
