using UnityEngine;
using System.Collections.Generic;

public enum NPCRole
{
    Officer,
    Inmate
}

public enum NPCTask
{
    Idle,
    Patrolling,
    Moving,
    Waiting,
    Talking,
    Eating,
    Working
}

public enum NPCSchedule
{
    WakeUp,      // 6:00 AM
    Breakfast,   // 7:00 AM
    WorkAssignment, // 8:00 AM
    Lunch,       // 12:00 PM
    Programs,    // 1:00 PM
    Dinner,      // 5:00 PM
    Recreation,  // 6:00 PM
    ReturnToHousing, // 8:00 PM
    Count,       // 9:00 PM
    LightsOut,   // 10:00 PM
    Patrol,      // Officer patrol
    Break        // Officer break
}

/// <summary>
/// Stores complete NPC character data.
/// </summary>
public class NPCData
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public NPCRole Role { get; set; }
    public Gender Gender { get; set; }
    public FaceStyle FaceStyle { get; set; }
    public Personality Personality { get; set; }
    public EyeColor EyeColor { get; set; }
    public string ID { get; set; }
    public AccessLevel AccessLevel { get; set; }
    
    public string FullName => $"{FirstName} {LastName}";
    
    public NPCData()
    {
        ID = GenerateID();
        AccessLevel = AccessLevel.Level1_Visitor;
    }
    
    private string GenerateID()
    {
        string prefix = Role == NPCRole.Officer ? "OFF" : "INM";
        int number = Random.Range(100000, 999999);
        return $"{prefix}-{number}";
    }
}
