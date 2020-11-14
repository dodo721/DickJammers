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
    public bool beingDragged;
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
        if (Input.GetButtonDown("Fire1")) {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            int layerMask = 1 << 8; // Only bees layer
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask)) {
                if (hit.collider.CompareTag("Player") && hit.collider.gameObject == controlling.gameObject) {
                    beingDragged = true;
                }
            }
        } else if (Input.GetButtonUp("Fire1")) {
            beingDragged = false;
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

        if (beingDragged) {
            mousePos = new Vector2((Input.mousePosition.x / Screen.width) - 0.5f, (Input.mousePosition.y / Screen.height) - 0.5f);
            Vector3 translationWorldSpace = new Vector3(mousePos.x * speed * Time.deltaTime, 0, mousePos.y * speed * Time.deltaTime);
            Vector3 translationCameraSpace = cameraTransform.TransformDirection(translationWorldSpace);
            controller.Move(translationCameraSpace);
        }
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
