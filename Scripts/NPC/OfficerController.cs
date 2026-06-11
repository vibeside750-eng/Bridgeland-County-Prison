using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Officer-specific NPC controller.
/// Handles patrols, door monitoring, and security duties.
/// </summary>
public class OfficerController : NPCController
{
    [SerializeField] private float patrolSpeed = 3.5f;
    [SerializeField] private float detectionRange = 15f;
    
    private List<string> patrolLocations = new List<string>();
    private int currentLocationIndex = 0;
    
    protected override void Start()
    {
        base.Start();
        SetupPatrolRoute();
    }
    
    private void SetupPatrolRoute()
    {
        // Define patrol locations based on assignment
        patrolLocations.AddRange(new string[]
        {
            "Gate A",
            "Vehicle Inspection Area",
            "Gate B",
            "Security Processing Corridor",
            "Gate C",
            "Administration Building",
            "Medical Building"
        });
    }
    
    protected override void OnScheduleChanged()
    {
        base.OnScheduleChanged();
        
        // Officers respond to schedule changes
        switch (currentSchedule)
        {
            case NPCSchedule.Patrol:
                currentTask = NPCTask.Patrolling;
                break;
            case NPCSchedule.Count:
                currentTask = NPCTask.Waiting; // Count time
                break;
            case NPCSchedule.Break:
                currentTask = NPCTask.Idle;
                break;
        }
    }
    
    protected override void HandlePatrol()
    {
        if (patrolLocations.Count == 0) return;
        
        string targetLocation = patrolLocations[currentLocationIndex];
        Vector3 targetPosition = mapManager.GetLocationSpawn(targetLocation);
        
        float distanceToTarget = Vector3.Distance(transform.position, targetPosition);
        
        if (distanceToTarget < 2f)
        {
            // Reached patrol location
            currentTask = NPCTask.Waiting;
            patrolWaitTimer = 3f; // Wait 3 seconds at each point
            currentLocationIndex = (currentLocationIndex + 1) % patrolLocations.Count;
        }
        else
        {
            navMeshAgent.SetDestination(targetPosition);
        }
    }
    
    private void DetectNearbyCharacters()
    {
        Collider[] nearbyColliders = Physics.OverlapSphere(transform.position, detectionRange);
        
        foreach (var collider in nearbyColliders)
        {
            if (collider.CompareTag("NPC") || collider.CompareTag("Player"))
            {
                // Officer sees character - could implement reactions
            }
        }
    }
}
