using System.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(CameraFollower))]
[RequireComponent(typeof(LineRenderer))]
public class SwarmController : MonoBehaviour
{
    [Serializable]
    public struct ControllerAction {
        public UnityEvent hold;
        public UnityEvent down;
        public UnityEvent up;
    }
    private Dictionary<string, ControllerAction> actionRegister;
    
    // Serializable version
    [Serializable]
    private struct ControllerActionSerialized {
        public ControllerAction actions;
        public string input;
    }

    [SerializeField]
    private List<ControllerActionSerialized> actionRegisterSerialized;

    public struct ControllerActionContext {
        public GameObject hovered;
        public BeeSwarm actor;
        public CharacterController controller;
        public float inputValue;
    };

    public ControllerActionContext context;

    public string[] Inputs {
        get {
            string[] inputs = new string[actionRegisterSerialized.Count];
            for (int i = 0; i < actionRegisterSerialized.Count; i++) {
                inputs[i] = actionRegisterSerialized[i].input;
            }
            return inputs;
        }
    }

    public BeeSwarm controlling;
    public Transform cameraTransform;
    private CameraFollower cameraFollower;
    private LineRenderer lineRenderer;
    public float speed;
    public float dragStrength;
    private CharacterController controller;
    public static SwarmController i;
    private Rigidbody draggingObject;
    private GameObject hovered;
    private GameObject selected;
    public Vector3 direction;
    public Preview previewHivePrefab;
    public Preview previewBeesPrefab;
    public Preview previewCoatPrefab;
    private Preview previewHive = null;
    private Preview previewBees = null;
    private Preview previewCoat = null;

    // RAYCAST LAYERMASK CONSTANTS

    ///<summary>Masks the layers "Ignore Raycast" and "Invisible Walls"</summary>
    public static readonly int LAYER_MASK_DEFAULT_IGNORES = ~((1 << 2) | (1 << 10));

    ///<summary>Masks the layer "Invisible Walls"</summary>
    public static readonly int LAYER_MASK_IGNORE_WALLS_ONLY = ~(1 << 10);

    ///<summary>Masks the layer "Ignore Raycast"</summary>
    public static readonly int LAYER_MASK_IGNORE_OBJECTS_ONLY = ~(1 << 2);

    ///<summary>Masks the layers "Player", "Ignore Raycast" and "Invisible Walls"</summary>
    public static readonly int LAYER_MASK_IGNORE_PLAYER = ~((1 << 2) | (1 << 10) | (1 << 8));

    ///<summary>Masks the layer "Player"</summary>
    public static readonly int LAYER_MASK_IGNORE_PLAYER_ONLY = ~(1 << 8);

    ///<summary>Masks the layers "Player" and "Invisible Walls"</summary>
    public static readonly int LAYER_MASK_IGNORE_PLAYER_AND_WALLS_ONLY = ~((1 << 8) | (1 << 10));

    ///<summary>Masks the layers "Player" and "Ingnore Raycasts"</summary>
    public static readonly int LAYER_MASK_IGNORE_PLAYER_AND_OBJECTS_ONLY = ~((1 << 8) | (1 << 2));

    ///<summary>Masks no layers</summary>
    public static readonly int LAYER_MASK_ALL = ~0;

    // Setup singleton
    void Awake () {
        if (i == null) i = this;
        else Debug.LogError("There should only be 1 SwarmController in the scene!");
        // Convert action register to dictionary for performance
        actionRegister = new Dictionary<string, ControllerAction>();
        foreach (ControllerActionSerialized cas in actionRegisterSerialized) {
            actionRegister.Add(cas.input, cas.actions);
        }
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

        List<BeeSwarm> allTheBees = FindObjectsOfType<BeeSwarm>().ToList();
        
        if (controlling == null) {
            if (allTheBees.Count > 0) {
                int index = allTheBees.IndexOf(controlling);
                index = (allTheBees.Count + index - 1) % allTheBees.Count;
                SetControlledBeeSwarm(allTheBees[index]);
            } else {
                // TODO DEATH
                return;
            }
        }

        if (controlling != null && controller == null) {
            controller = controlling.GetComponent<CharacterController>();
        }

        // Build the context for an action
        context.hovered = GetHoveredGameObject();
        context.actor = controlling;
        context.controller = controller;

        // Perform controller actions
        foreach (string input in actionRegister.Keys) {
            // Add individual input values to context
            float inputVal = Input.GetAxisRaw(input);
            context.inputValue = inputVal;
            // Is button being held? Prepare the action
            if (Mathf.Abs(inputVal) > 0) {
                actionRegister[input].hold.Invoke();
            }
            // Was the button pushed down? Trigger immediate behaviour
            if (Input.GetButtonDown(input)) {
                actionRegister[input].down.Invoke();
            }
            // Is the button released? Finish the action
            if (Input.GetButtonUp(input)) {
                actionRegister[input].up.Invoke();
            }
        }

        if (Input.GetMouseButton(1)) {
            Vector3 newPos;
            bool valid = CheckMousePlacement(out newPos);
            if (controlling.numBees < 400) valid = false;
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
            int layerMask = LAYER_MASK_IGNORE_PLAYER;
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
            int index = allTheBees.IndexOf(controlling);
            index = (allTheBees.Count + index - 1) % allTheBees.Count;
            SetControlledBeeSwarm(allTheBees[index]);
        }

        if(Input.GetButtonDown("E")){
            int index = allTheBees.IndexOf(controlling);
            index = (index + 1) % allTheBees.Count;
            SetControlledBeeSwarm(allTheBees[index]);
        }

        if (Input.GetKey(KeyCode.F)) {
            Vector3 newPos;
            bool valid = CheckMousePlacement(out newPos);
            if (controlling.numBees < 400) valid = false;
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
            bool valid = CheckMousePlacement(out newPos);
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
            int layerMask = LAYER_MASK_IGNORE_PLAYER;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask))
            {
                if (controlling.inRange.Contains(hit.rigidbody)) {
                    Outline outline = hit.collider.GetComponent<Outline>();
                    if (outline == null) outline = hit.collider.gameObject.AddComponent<Outline>();
                    outline.OutlineWidth = 4;
                    outline.OutlineColor = Color.yellow;
                    outline.enabled = true;
                    if (hit.rigidbody != null && !hit.rigidbody.isKinematic)
                    {
                        draggingObject = hit.rigidbody;
                    }
                }
            }
        }
        if (Input.GetButtonUp("Fire1")) {
            if (draggingObject != null) {
                Outline outline = draggingObject.GetComponent<Outline>();
                if (outline == null) outline = draggingObject.gameObject.AddComponent<Outline>();
                outline.OutlineWidth = 4;
                outline.OutlineColor = Color.white;
            }
            draggingObject = null;
        }
        if (draggingObject != null) {
            if (!controlling.inRange.Contains(draggingObject)) {
                draggingObject = null;
            }
        }
    }

#if UNITY_EDITOR
    /// <summary>
    ///  Add a new input axis slot to the action register
    /// </summary>
    /// <param name="input">The input axis to add</param>
    /// <returns>True if the action was succesfully added, false if the input was already added</returns>
    public bool AddInputAxis (string input) {
        foreach (ControllerActionSerialized cas in actionRegisterSerialized) {
            if (cas.input == input) return false;
        }
        ControllerActionSerialized newCas = new ControllerActionSerialized();
        newCas.input = input;
        actionRegisterSerialized.Add(newCas);
        return true;
    }

    /// <summary>
    ///  Remove an input axis slot from the register
    /// </summary>
    /// <param name="input">The input axis to remove</param>
    public void RemoveInputAxis (string input) {
        ControllerActionSerialized toRemove = new ControllerActionSerialized();
        bool found = false;
        foreach (ControllerActionSerialized cas in actionRegisterSerialized) {
            if (cas.input == input) {
                toRemove = cas;
                found = true;
                break;
            }
        }
        if (found) {
            actionRegisterSerialized.Remove(toRemove);
        }
    }

    /// <summary>
    ///  Changes an input key and keeps associated actions
    /// </summary>
    /// <param name="from">The input axis to change</param>
    /// <param name="to">New name for the input axis</param>
    public bool ChangeInputAxis (string from, string to) {
        int index = -1;
        foreach (ControllerActionSerialized cas in actionRegisterSerialized) {
            if (cas.input == from) {
                index = actionRegisterSerialized.IndexOf(cas);
                break;
            }
        }
        if (index != -1) {
            ControllerActionSerialized cas = actionRegisterSerialized[index];
            cas.input = to;
            actionRegisterSerialized[index] = cas;
            return true;
        }
        return false;
    }
    
    /// <summary>
    ///  Get the related actions to an input axis
    /// </summary>
    /// <param name="input">The input axis to search with</param>
    /// <returns>The related actions</returns>
    public ControllerAction? GetActionsFromInput (string input) {
        foreach (ControllerActionSerialized cas in actionRegisterSerialized) {
            if (cas.input == input) {
                return cas.actions;
            }
        }
        return null;
    }
#endif

    // Runs every PHYSICS frame
    void FixedUpdate () {

        // Apply forces and draw effects for a dragging object
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

    /// <summary>
    ///  Get the placement position relative to the mouse and test if it is valid.
    /// </summary>
    /// <param name="pos">
    ///  Vector3 to store the calculated position in
    /// </param>
    /// <returns>
    ///  True if the placement is a valid position
    /// </returns>
    public bool CheckMousePlacement (out Vector3 pos) {
        Vector3 newPosition = controlling.transform.position + (controlling.spawnSwarmDistance * getDirectionToMouse());

        bool validSpawn = true;

        foreach(BeeSwarm bees in FindObjectsOfType<BeeSwarm>()){
            if((bees.transform.position - newPosition).magnitude < 2) validSpawn = false;
        }

        RaycastHit hit;
        int layerMask = LAYER_MASK_IGNORE_OBJECTS_ONLY;
        if(Physics.Raycast(newPosition, (controlling.transform.position - newPosition), out hit, Mathf.Infinity, layerMask))
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

    /// <summary>
    ///  Get the GameObject currently under the mouse.
    /// </summary>
    /// <remarks>
    ///  Uses default layer mask: avoid Player and Ignore Raycast layers
    /// </remarks>
    public GameObject GetHoveredGameObject () {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        // Avoid player and ignore raycast layers by default
        int layerMask = LAYER_MASK_IGNORE_PLAYER;
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask))
        {
            return hit.collider.gameObject;
        }
        else return null;
    }

    /// <summary>
    ///  Get the GameObject currently under the mouse.
    /// </summary>
    /// <param name="layerMask">
    ///  The layer mask to use on the raycast
    /// </param>
    public GameObject GetHoveredGameObject (int layerMask) {
        // Check to see if the mouse is over the interactable
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask))
        {
            return hit.collider.gameObject;
        }
        else return null;
    }

    public BeeSwarm GetControlledBeeSwarm () {
        return controlling;
    }
    public void SetControlledBeeSwarm (BeeSwarm bees) {
        controlling = bees;
        controller = bees.GetComponent<CharacterController>();
        if (cameraFollower == null) {
            cameraFollower = GetComponent<CameraFollower>();
        }
        cameraFollower.target = bees.cameraTarget;
    }
}
