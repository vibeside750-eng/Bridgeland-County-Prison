using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Manages prison daily schedule and time progression.
/// Coordinates NPC activities and transitions between schedule phases.
/// </summary>
public class ScheduleManager : MonoBehaviour
{
    [System.Serializable]
    public class SchedulePhase
    {
        public NPCSchedule schedule;
        public float startTime; // Military time (6.0 = 6:00 AM, 14.0 = 2:00 PM)
        public float endTime;
        public string description;
    }
    
    [SerializeField] private List<SchedulePhase> inmateSchedule = new List<SchedulePhase>();
    [SerializeField] private List<SchedulePhase> officerSchedule = new List<SchedulePhase>();
    
    private static ScheduleManager _instance;
    
    public static ScheduleManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<ScheduleManager>();
            }
            return _instance;
        }
    }
    
    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        
        _instance = this;
        InitializeSchedules();
    }
    
    private void InitializeSchedules()
    {
        // INMATE SCHEDULE
        inmateSchedule.Add(new SchedulePhase
        {
            schedule = NPCSchedule.WakeUp,
            startTime = 6.0f,
            endTime = 7.0f,
            description = "Wake Up"
        });
        
        inmateSchedule.Add(new SchedulePhase
        {
            schedule = NPCSchedule.Breakfast,
            startTime = 7.0f,
            endTime = 8.0f,
            description = "Breakfast"
        });
        
        inmateSchedule.Add(new SchedulePhase
        {
            schedule = NPCSchedule.WorkAssignment,
            startTime = 8.0f,
            endTime = 12.0f,
            description = "Work Assignment"
        });
        
        inmateSchedule.Add(new SchedulePhase
        {
            schedule = NPCSchedule.Lunch,
            startTime = 12.0f,
            endTime = 13.0f,
            description = "Lunch"
        });
        
        inmateSchedule.Add(new SchedulePhase
        {
            schedule = NPCSchedule.Programs,
            startTime = 13.0f,
            endTime = 17.0f,
            description = "Programs"
        });
        
        inmateSchedule.Add(new SchedulePhase
        {
            schedule = NPCSchedule.Dinner,
            startTime = 17.0f,
            endTime = 18.0f,
            description = "Dinner"
        });
        
        inmateSchedule.Add(new SchedulePhase
        {
            schedule = NPCSchedule.Recreation,
            startTime = 18.0f,
            endTime = 20.0f,
            description = "Recreation"
        });
        
        inmateSchedule.Add(new SchedulePhase
        {
            schedule = NPCSchedule.ReturnToHousing,
            startTime = 20.0f,
            endTime = 21.0f,
            description = "Return To Housing"
        });
        
        inmateSchedule.Add(new SchedulePhase
        {
            schedule = NPCSchedule.Count,
            startTime = 21.0f,
            endTime = 21.5f,
            description = "Count Time"
        });
        
        inmateSchedule.Add(new SchedulePhase
        {
            schedule = NPCSchedule.LightsOut,
            startTime = 21.5f,
            endTime = 6.0f,
            description = "Lights Out"
        });
        
        // OFFICER SCHEDULE
        officerSchedule.Add(new SchedulePhase
        {
            schedule = NPCSchedule.Patrol,
            startTime = 6.0f,
            endTime = 14.0f,
            description = "Morning Shift - Patrol"
        });
        
        officerSchedule.Add(new SchedulePhase
        {
            schedule = NPCSchedule.Break,
            startTime = 14.0f,
            endTime = 14.5f,
            description = "Lunch Break"
        });
        
        officerSchedule.Add(new SchedulePhase
        {
            schedule = NPCSchedule.Patrol,
            startTime = 14.5f,
            endTime = 22.0f,
            description = "Afternoon Shift - Patrol"
        });
    }
    
    public NPCSchedule GetScheduleForTime(float time, NPCRole role)
    {
        List<SchedulePhase> schedule = role == NPCRole.Officer ? officerSchedule : inmateSchedule;
        
        foreach (var phase in schedule)
        {
            // Handle wrap-around (e.g., 21:30 to 6:00)
            if (phase.endTime < phase.startTime)
            {
                if (time >= phase.startTime || time < phase.endTime)
                {
                    return phase.schedule;
                }
            }
            else if (time >= phase.startTime && time < phase.endTime)
            {
                return phase.schedule;
            }
        }
        
        // Default to idle
        return NPCSchedule.Patrol;
    }
    
    public string GetScheduleDescription(float time, NPCRole role)
    {
        List<SchedulePhase> schedule = role == NPCRole.Officer ? officerSchedule : inmateSchedule;
        
        foreach (var phase in schedule)
        {
            if (phase.endTime < phase.startTime)
            {
                if (time >= phase.startTime || time < phase.endTime)
                {
                    return phase.description;
                }
            }
            else if (time >= phase.startTime && time < phase.endTime)
            {
                return phase.description;
            }
        }
        
        return "Unknown";
    }
    
    public float GetTimeUntilNextPhase(float currentTime, NPCRole role)
    {
        List<SchedulePhase> schedule = role == NPCRole.Officer ? officerSchedule : inmateSchedule;
        
        foreach (var phase in schedule)
        {
            if (currentTime < phase.startTime)
            {
                return phase.startTime - currentTime;
            }
        }
        
        // Next phase is tomorrow morning
        return (24.0f - currentTime) + schedule[0].startTime;
    }
}
