using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwarmController : MonoBehaviour
{

    public Vector2 mousePos;
    public Transform cameraTransform;
    public float speed;
    public bool beingDragged;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Fire1")) {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit)) {
                if (hit.collider.CompareTag("Player")) {
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
            transform.Translate(translationCameraSpace);
        }
    }
}
