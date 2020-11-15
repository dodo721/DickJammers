using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CameraFollower))]
[RequireComponent(typeof(LineRenderer))]
public class SwarmController : MonoBehaviour
{

    public BeeSwarm controlling;
    public Vector2 mousePos;
    public Transform cameraTransform;
    public CameraFollower cameraFollower;
    public LineRenderer lineRenderer;
    public float speed;
    public float dragStrength;
    private CharacterController controller;
    public static SwarmController i;
    public Rigidbody draggingObject;
    public Vector3 direction;
    public Preview previewHivePrefab;
    public Preview previewBeesPrefab;
    public Preview previewCoatPrefab;
    private Preview previewHive = null;
    private Preview previewBees = null;
    private Preview previewCoat = null;

    void Awake () {
        if (i == null) i = this;
        else Debug.LogError("There should only be 1 SwarmController in the scene!");
    }

    void Start () {
        cameraFollower = GetComponent<CameraFollower>();
        lineRenderer = GetComponent<LineRenderer>();
    }

    public Vector3 getDirectionToMouse() {
        Vector2 mousePos = new Vector2((Input.mousePosition.x / Screen.width) - 0.5f, (Input.mousePosition.y / Screen.height) - 0.5f);
        Vector3 translationWorldSpace = new Vector3(mousePos.x, 0, mousePos.y);
        Vector3 translationCameraSpace = SwarmController.i.cameraTransform.TransformDirection(translationWorldSpace);
        return translationCameraSpace.normalized;
    }

    // Update is called once per frame
    void Update()
    {
        if (controlling == null) {
            if (BeeSwarm.allTheBees.Count > 0) {
                int index = BeeSwarm.allTheBees.IndexOf(controlling);
                index = (BeeSwarm.allTheBees.Count + index - 1) % BeeSwarm.allTheBees.Count;
                SetControlledBeeSwarm(BeeSwarm.allTheBees[index]);
            } else {
                // TODO DEATH
                return;
            }
        }

        if (controlling != null && controller == null) {
            controller = controlling.GetComponent<CharacterController>();
        }

        if (Input.GetMouseButton(1)) {
            Vector3 newPos;
            bool valid = CanBePlacedAt(out newPos);
            if (previewBees == null) {
                previewBees = Instantiate(previewBeesPrefab, newPos, previewBeesPrefab.transform.rotation).GetComponent<Preview>();
            }
            previewBees.transform.position = newPos;
            previewBees.valid = valid;
        }
        if(Input.GetButtonUp("Fire2")) {
            // If over hive, destroy it
            bool onHive = false;
            RaycastHit hit;
            int layerMask = (1 << 8) | (1 << 2);
            layerMask = ~layerMask; // All except bees
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask))
            {
                if (hit.collider.CompareTag("Hive") && hit.rigidbody != null && controlling.inRange.Contains(hit.rigidbody))
                {
                    Hive hive = hit.collider.GetComponent<Hive>();
                    hive.Die(controlling);
                    onHive = true;
                }
            }
            if (!onHive && !controlling.HasClothes()) controlling.Split();
            if (previewBees != null) {
                Destroy(previewBees.gameObject);
            }
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

        if (Input.GetKey(KeyCode.F)) {
            Vector3 newPos;
            bool valid = CanBePlacedAt(out newPos);
            if (previewHive == null) {
                previewHive = Instantiate(previewHivePrefab, newPos, previewHivePrefab.transform.rotation).GetComponent<Preview>();
            }
            previewHive.transform.position = newPos;
            previewHive.valid = valid;
        }
        if(Input.GetButtonUp("F")){
            controlling.BuildHive();
            if (previewHive != null) {
                Destroy(previewHive.gameObject);
            }
        }

        if (Input.GetKey(KeyCode.Space) && controlling.HasClothes()) {
            Vector3 newPos;
            bool valid = CanBePlacedAt(out newPos);
            if (previewCoat == null) {
                previewCoat = Instantiate(previewCoatPrefab, newPos, previewCoatPrefab.transform.rotation).GetComponent<Preview>();
            }
            previewCoat.transform.position = newPos;
            previewCoat.valid = valid;
        }
        if(Input.GetButtonUp("Spacebar")){
            if (controlling.HasClothes())
                controlling.clothes.Drop();
            if (previewCoat != null) {
                Destroy(previewCoat.gameObject);
            }
        }

        direction = new Vector3();

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
        if (controlling.HasClothes())
            translationWorldSpace *= controlling.clothes.speedMod;
        Vector3 translationCameraSpace = cameraTransform.TransformDirection(translationWorldSpace);
        controller.Move(translationCameraSpace);
        controller.Move(Vector3.down * (controller.transform.position.y - controlling.lockHeight));

        // DRAGGING
        if (Input.GetButtonDown("Fire1")) {
            // Check if the mouse is over any in range objects
            RaycastHit hit;
            int layerMask = (1 << 8) | (1 << 2);
            layerMask = ~layerMask; // All except bees
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask))
            {
                if (hit.rigidbody != null && controlling.inRange.Contains(hit.rigidbody) && !hit.rigidbody.isKinematic)
                {
                    draggingObject = hit.rigidbody;
                }
            }
        }
        if (Input.GetButtonUp("Fire1")) {
            draggingObject = null;
        }
        if (draggingObject != null) {
            if (!controlling.inRange.Contains(draggingObject)) {
                draggingObject = null;
            }
        }
    }

    // Runs every PHYSICS frame
    void FixedUpdate () {
        if (draggingObject != null) {
            
            // Force
            Vector3 objectScreenPos = Camera.main.WorldToViewportPoint(draggingObject.transform.position) - new Vector3(0.5f, 0.5f, 0);
            Vector2 mousePos = new Vector2((Input.mousePosition.x / Screen.width) - 0.5f - objectScreenPos.x, (Input.mousePosition.y / Screen.height) - 0.5f - objectScreenPos.y);
            Vector3 directionWorldSpace = new Vector3(mousePos.x, 0, mousePos.y);
            Vector3 directionCameraSpace = SwarmController.i.cameraTransform.TransformDirection(directionWorldSpace);
            float forceMag = directionCameraSpace.magnitude * dragStrength;
            draggingObject.AddForce(directionCameraSpace * forceMag, ForceMode.Acceleration);

            // Line drawing
            lineRenderer.enabled = true;
            float colStrength = Mathf.Clamp(mousePos.magnitude * 20, 0f, 1f);
            Color lineColor = new Color(1f, 1f - colStrength, 1f - colStrength);
            lineRenderer.startColor = lineColor;
            lineRenderer.endColor = lineColor;
            lineRenderer.SetPositions(new Vector3[] {
                draggingObject.transform.position,
                draggingObject.transform.position + directionCameraSpace * 10
            });
        } else {
            lineRenderer.enabled = false;
        }
    }

    public bool CanBePlacedAt (out Vector3 pos) {
        Vector3 newPosition = controlling.transform.position + (controlling.spawnSwarmDistance * getDirectionToMouse());

        bool validSpawn = true;

        foreach(BeeSwarm bees in BeeSwarm.allTheBees){
            if((bees.transform.position - newPosition).magnitude < 2) validSpawn = false;
        }

        RaycastHit hit;
        if(Physics.Raycast(newPosition, (controlling.transform.position - newPosition), out hit))
        {
            if(!(hit.transform == controlling.transform))
            {
                validSpawn = false;
            }
        }

        if(Physics.Raycast(controlling.transform.position, (newPosition - controlling.transform.position), out hit, controlling.spawnSwarmDistance))
        {
            validSpawn = false;
        }

        pos = newPosition;
        return validSpawn;
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
