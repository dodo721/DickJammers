using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[ExecuteAlways]
public class FootTarget : MonoBehaviour {

    public bool playInScene = false;
    public NavMeshAgent agent;

    [Header("Raycasting")]
    [Min(0)]
    public float castRange;
    public Vector3 castDirection = Vector3.down;
    private Vector3 currentHit;
    private bool currentlyHitting;

    [Header("IK Targets")]
    public GameObject target;
    public GameObject tracker;

    [Header("Movement")]
    [Min(0)]
    public float moveDistThreshold;
    public float stopDistThreshold;
    public float stepSpeed;
    public AnimationCurve stepCurve;
    public bool stepping;
    [Min(0)]
    public float velocityThreshold;
    public bool leadingFoot;
    private bool brokeVelThreshLastFrame = false;
    [Min(0)]
    public float rotationThreshold;

    void Update () {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, castDirection, out hit, castRange)) {
            currentlyHitting = true;
            currentHit = hit.point;
            if (leadingFoot && agent.velocity.magnitude > velocityThreshold && !brokeVelThreshLastFrame) {
                stepping = true;
                brokeVelThreshLastFrame = true;
            } else if (agent.velocity.magnitude <= velocityThreshold) {
                brokeVelThreshLastFrame = false;
            }
            if (!stepping) {
                if (Vector3.Distance(target.transform.position, currentHit) > moveDistThreshold) {
                    stepping = true;
                }
            }
            if (Application.isPlaying || playInScene) {
                if (stepping) {
                    Vector3 targetPos = currentHit + (moveDistThreshold * transform.forward);
                    Vector3 newPos = Vector3.Lerp(target.transform.position, targetPos, stepSpeed * Time.deltaTime);
                    target.transform.position = newPos;
                    if (Vector3.Distance(target.transform.position, targetPos) < stopDistThreshold) {
                        stepping = false;
                    }
                }
            }
        } else {
            currentlyHitting = false;
        }
    }

#if UNITY_EDITOR
    void OnDrawGizmos () {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + (castDirection * castRange));
        if (currentlyHitting) {
            Gizmos.color = new Color(1f, 0f, 0f, 0.5f);
            Gizmos.DrawSphere(currentHit, 0.2f);
        }
        Gizmos.color = Vector3.Distance(target.transform.position, currentHit) > moveDistThreshold ? Color.red : Color.green;
        Gizmos.DrawLine(target.transform.position, tracker.transform.position);
        UnityEditor.Handles.color = Color.yellow;
        UnityEditor.Handles.ArrowHandleCap(0, target.transform.position, Quaternion.LookRotation(transform.forward), moveDistThreshold, EventType.Repaint);
        UnityEditor.Handles.Label(tracker.transform.position + ((target.transform.position - tracker.transform.position) / 2), Vector3.Distance(tracker.transform.position, target.transform.position) + " / " + moveDistThreshold);
    }
#endif

}