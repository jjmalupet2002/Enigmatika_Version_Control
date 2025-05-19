using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.AI; //important

public class RandomMovement : MonoBehaviour
{
    public NavMeshAgent agent;
    public float range; //radius of sphere

    public Transform centrePoint; //centre of the area the agent wants to move around in
    public Animator animator; // Reference to the Animator

    public GameObject uiPanel; // Reference to the UI panel that controls the NPC movement

    private static readonly int Walk = Animator.StringToHash("Walking");
    private static readonly int Idle = Animator.StringToHash("Idle");

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        if (uiPanel != null && uiPanel.activeInHierarchy)
        {
            StopMovement(); // Stop the movement and play idle animation if the UI is active
        }
        else
        {
            StartMovement(); // Start the movement and play walk animation if the UI is inactive
        }

        if (agent.remainingDistance <= agent.stoppingDistance && !agent.isStopped) // Done with path and not stopped
        {
            Vector3 point;
            if (RandomPoint(centrePoint.position, range, out point)) // Pass in our centre point and radius of area
            {
                UnityEngine.Debug.DrawRay(point, Vector3.up, Color.blue, 1.0f); // So you can see with gizmos
                agent.SetDestination(point);
                animator.SetTrigger(Walk); // Set to Walk animation when moving
            }
        }
        else if (agent.remainingDistance > agent.stoppingDistance && !agent.isStopped) // If the agent is moving and not stopped
        {
            animator.SetTrigger(Walk); // Ensure Walk animation when moving
        }
        else if (!agent.isStopped) // Set to Idle animation when stopped
        {
            animator.SetTrigger(Idle);
        }
    }

    bool RandomPoint(Vector3 center, float range, out Vector3 result)
    {
        Vector3 randomPoint = center + UnityEngine.Random.insideUnitSphere * range; // Random point in a sphere 
        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomPoint, out hit, 1.0f, NavMesh.AllAreas)) // Documentation: https://docs.unity3d.com/ScriptReference/AI.NavMesh.SamplePosition.html
        {
            // The 1.0f is the max distance from the random point to a point on the navmesh, might want to increase if range is big
            // or add a for loop like in the documentation
            result = hit.position;
            return true;
        }

        result = Vector3.zero;
        return false;
    }

    public void StartMovement()
    {
        agent.isStopped = false;
        Vector3 point;
        if (RandomPoint(centrePoint.position, range, out point))
        {
            agent.SetDestination(point);
        }
        animator.SetTrigger(Walk);
    }

    public void StopMovement()
    {
        agent.isStopped = true;
        animator.SetTrigger(Idle);
    }
}
