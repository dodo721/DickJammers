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
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit)) {
                if (hit.collider.CompareTag("Player") && hit.collider.gameObject == controlling.gameObject) {
                    beingDragged = true;
                }
            }
        } else if (Input.GetButtonUp("Fire1")) {
            beingDragged = false;
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
