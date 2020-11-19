using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Animator))]
public class EnemyAnimations : MonoBehaviour
{
    public NavMeshAgent agent;
    private Animator animator;
    public GameObject leftFootTarget;
    public GameObject rightFootTarget;
    
    void Start () {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        // Normalised velocity paramater - current speed / max speed
        animator.SetFloat("Velocity", agent.velocity.magnitude / agent.speed);
        
        
    }
}
