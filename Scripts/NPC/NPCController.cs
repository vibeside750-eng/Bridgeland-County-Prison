using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Base NPC controller for managing AI behavior, schedules, and interactions.
/// </summary>
public class NPCController : MonoBehaviour
{
    [SerializeField] private CharacterDisplay characterDisplay;
    [SerializeField] private NavMeshAgent navMeshAgent;
    [SerializeField] private float walkSpeed = 3.5f;
    [SerializeField] private float patrolWaitDuration = 2f;
    
    protected NPCData npcData;
    protected NPCSchedule currentSchedule;
    protected NPCTask currentTask = NPCTask.Idle;
    protected Vector3 currentPatrolTarget;
    protected float patrolWaitTimer = 0f;
    protected bool isPatrolling = true;
    
    protected MapManager mapManager;
    protected ScheduleManager scheduleManager;
    
    protected List<Vector3> patrolRoute = new List<Vector3>();
    protected int patrolRouteIndex = 0;
    
    protected virtual void Start()
    {
        mapManager = FindObjectOfType<MapManager>();
        scheduleManager = FindObjectOfType<ScheduleManager>();
        navMeshAgent.speed = walkSpeed;
    }
    
    public virtual void InitializeNPC(NPCData data)
    {
        npcData = data;
        characterDisplay.Initialize(new PlayerData
        {
            FirstName = data.FirstName,
            LastName = data.LastName,
            Role = data.Role == NPCRole.Officer ? PlayerRole.Officer : PlayerRole.Inmate,
            FaceStyle = data.FaceStyle,
            Gender = data.Gender,
            EyeColor = data.EyeColor,
            ID = data.ID
        });
        
        gameObject.tag = "NPC";
    }
    
    protected virtual void Update()
    {
        UpdateSchedule();
        ExecuteCurrentTask();
    }
    
    protected virtual void UpdateSchedule()
    {
        float currentTime = GameManager.Instance.GetCurrentTime();
        
        // Check if schedule has changed
        NPCSchedule newSchedule = scheduleManager.GetScheduleForTime(currentTime, npcData.Role);
        if (newSchedule != currentSchedule)
        {
            currentSchedule = newSchedule;
            OnScheduleChanged();
        }
    }
    
    protected virtual void ExecuteCurrentTask()
    {
        switch (currentTask)
        {
            case NPCTask.Patrolling:
                HandlePatrol();
                break;
            case NPCTask.Moving:
                HandleMovement();
                break;
            case NPCTask.Waiting:
                HandleWaiting();
                break;
            case NPCTask.Idle:
                HandleIdle();
                break;
        }
    }
    
    protected virtual void OnScheduleChanged()
    {
        // Override in subclasses for specific behavior
        Debug.Log($"[NPC] {npcData.FullName} schedule changed to {currentSchedule}");
    }
    
    protected virtual void HandlePatrol()
    {
        if (patrolRoute.Count == 0) return;
        
        float distanceToTarget = Vector3.Distance(transform.position, currentPatrolTarget);
        
        if (distanceToTarget < 1f)
        {
            // Reached patrol point
            currentTask = NPCTask.Waiting;
            patrolWaitTimer = patrolWaitDuration;
            navMeshAgent.SetDestination(transform.position);
        }
        else
        {
            navMeshAgent.SetDestination(currentPatrolTarget);
        }
    }
    
    protected virtual void HandleMovement()
    {
        // Continue moving to destination
    }
    
    protected virtual void HandleWaiting()
    {
        patrolWaitTimer -= Time.deltaTime;
        if (patrolWaitTimer <= 0)
        {
            // Move to next patrol point
            patrolRouteIndex = (patrolRouteIndex + 1) % patrolRoute.Count;
            currentPatrolTarget = patrolRoute[patrolRouteIndex];
            currentTask = NPCTask.Patrolling;
        }
    }
    
    protected virtual void HandleIdle()
    {
        // Character stands still
        navMeshAgent.velocity = Vector3.zero;
    }
    
    public void SetPatrolRoute(List<Vector3> route)
    {
        patrolRoute = route;
        if (patrolRoute.Count > 0)
        {
            currentPatrolTarget = patrolRoute[0];
        }
    }
    
    public void StartTask(NPCTask task)
    {
        currentTask = task;
    }
    
    public NPCTask GetCurrentTask() => currentTask;
    public NPCData GetNPCData() => npcData;
}
