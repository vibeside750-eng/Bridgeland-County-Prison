using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Inmate-specific NPC controller.
/// Follows daily schedule and participates in prison activities.
/// </summary>
public class InmateController : NPCController
{
    [SerializeField] private float inactiveIdleChance = 0.3f;
    [SerializeField] private float conversationChance = 0.2f;
    
    private float idleTimer = 0f;
    private List<string> activityLocations = new List<string>();
    
    protected override void Start()
    {
        base.Start();
        SetupActivityRoute();
    }
    
    private void SetupActivityRoute()
    {
        // Inmate activity locations
        activityLocations.AddRange(new string[]
        {
            "HousingUnitA",
            "Kitchen",
            "Programs Building",
            "Recreation Yard",
            "Medical Building"
        });
    }
    
    protected override void OnScheduleChanged()
    {
        base.OnScheduleChanged();
        
        // Inmates follow strict schedule
        switch (currentSchedule)
        {
            case NPCSchedule.WakeUp:
                NavigateToLocation("HousingUnitA");
                break;
            case NPCSchedule.Breakfast:
                NavigateToLocation("Kitchen");
                break;
            case NPCSchedule.WorkAssignment:
                NavigateToLocation("Programs Building");
                break;
            case NPCSchedule.Lunch:
                NavigateToLocation("Kitchen");
                break;
            case NPCSchedule.Programs:
                NavigateToLocation("Programs Building");
                break;
            case NPCSchedule.Dinner:
                NavigateToLocation("Kitchen");
                break;
            case NPCSchedule.Recreation:
                NavigateToLocation("Recreation Yard");
                break;
            case NPCSchedule.ReturnToHousing:
                NavigateToLocation("HousingUnitA");
                break;
            case NPCSchedule.Count:
                // Stay in housing during count
                break;
            case NPCSchedule.LightsOut:
                currentTask = NPCTask.Idle;
                break;
        }
    }
    
    private void NavigateToLocation(string locationName)
    {
        currentTask = NPCTask.Moving;
        Vector3 targetPosition = mapManager.GetLocationSpawn(locationName);
        navMeshAgent.SetDestination(targetPosition);
    }
    
    protected override void ExecuteCurrentTask()
    {
        base.ExecuteCurrentTask();
        
        // Inmates have idle behaviors
        if (currentTask == NPCTask.Idle)
        {
            idleTimer += Time.deltaTime;
            if (idleTimer > 5f && Random.value < conversationChance)
            {
                // Attempt conversation with nearby inmates
                idleTimer = 0f;
            }
        }
        else
        {
            idleTimer = 0f;
        }
    }
}
