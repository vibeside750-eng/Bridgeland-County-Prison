using UnityEngine;

public enum PlayerRole
{
    Officer,
    Inmate
}

public enum FaceStyle
{
    Professional,
    Friendly,
    Cute,
    Stoic,
    Aggressive,
    Nervous
}

public enum Personality
{
    Professional,
    Friendly,
    Quiet,
    Confident,
    Nervous,
    Aggressive
}

public enum Gender
{
    Male,
    Female
}

public enum EyeColor
{
    Brown,
    Blue,
    Green,
    Hazel,
    Gray
}

public enum AccessLevel
{
    Level1_Visitor = 1,
    Level2_Staff = 2,
    Level3_Officer = 3,
    Level4_Supervisor = 4,
    Level5_Executive = 5,
    Level6_Director = 6
}

/// <summary>
/// Stores complete player character data.
/// </summary>
public class PlayerData
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public PlayerRole Role { get; set; }
    public Gender Gender { get; set; }
    public FaceStyle FaceStyle { get; set; }
    public Personality Personality { get; set; }
    public EyeColor EyeColor { get; set; }
    public AccessLevel AccessLevel { get; set; }
    public string ID { get; set; }
    
    public string FullName => $"{FirstName} {LastName}";
    
    public PlayerData()
    {
        // Generate random ID
        ID = GenerateID();
        
        // Set default access level based on role
        AccessLevel = AccessLevel.Level1_Visitor;
    }
    
    private string GenerateID()
    {
        // Format: ROLE-XXXXXX (e.g., OFF-123456 or INM-654321)
        string prefix = Role == PlayerRole.Officer ? "OFF" : "INM";
        int number = Random.Range(100000, 999999);
        return $"{prefix}-{number}";
    }
    
    public bool CanAccessLevel(int requiredLevel)
    {
        return (int)AccessLevel >= requiredLevel;
    }
}
