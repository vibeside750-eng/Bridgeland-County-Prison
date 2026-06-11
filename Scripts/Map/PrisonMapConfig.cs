using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Configures and manages the complete prison map layout.
/// Defines all locations, buildings, and spatial relationships.
/// </summary>
public class PrisonMapConfig : MonoBehaviour
{
    [System.Serializable]
    public class Building
    {
        public string name;
        public Vector3 position;
        public Vector3 size;
        public AccessLevel requiredAccess = AccessLevel.Level1_Visitor;
    }
    
    [SerializeField] private List<Building> buildings = new List<Building>();
    
    public void InitializePrisonLayout()
    {
        // EXTERIOR AREAS
        buildings.Add(new Building
        {
            name = "Public Road",
            position = new Vector3(0, 0, -60),
            size = new Vector3(100, 0, 20),
            requiredAccess = AccessLevel.Level1_Visitor
        });
        
        buildings.Add(new Building
        {
            name = "Visitor Entrance",
            position = new Vector3(0, 0, -40),
            size = new Vector3(30, 0, 10),
            requiredAccess = AccessLevel.Level1_Visitor
        });
        
        buildings.Add(new Building
        {
            name = "Visitor Parking",
            position = new Vector3(-30, 0, -20),
            size = new Vector3(20, 0, 30),
            requiredAccess = AccessLevel.Level1_Visitor
        });
        
        buildings.Add(new Building
        {
            name = "StaffParking",
            position = new Vector3(30, 0, -20),
            size = new Vector3(20, 0, 30),
            requiredAccess = AccessLevel.Level2_Staff
        });
        
        // SECURITY CHECKPOINTS & GATES
        buildings.Add(new Building
        {
            name = "Staff Security Checkpoint",
            position = new Vector3(0, 0, 0),
            size = new Vector3(25, 0, 8),
            requiredAccess = AccessLevel.Level2_Staff
        });
        
        buildings.Add(new Building
        {
            name = "Gate A",
            position = new Vector3(0, 0, 8),
            size = new Vector3(20, 0, 3),
            requiredAccess = AccessLevel.Level3_Officer
        });
        
        buildings.Add(new Building
        {
            name = "Vehicle Inspection Area",
            position = new Vector3(0, 0, 15),
            size = new Vector3(30, 0, 8),
            requiredAccess = AccessLevel.Level3_Officer
        });
        
        buildings.Add(new Building
        {
            name = "Gate B",
            position = new Vector3(0, 0, 25),
            size = new Vector3(20, 0, 3),
            requiredAccess = AccessLevel.Level3_Officer
        });
        
        buildings.Add(new Building
        {
            name = "Security Processing Corridor",
            position = new Vector3(0, 0, 32),
            size = new Vector3(25, 0, 10),
            requiredAccess = AccessLevel.Level3_Officer
        });
        
        buildings.Add(new Building
        {
            name = "Gate C",
            position = new Vector3(0, 0, 42),
            size = new Vector3(20, 0, 3),
            requiredAccess = AccessLevel.Level3_Officer
        });
        
        // MAIN COMPLEX
        buildings.Add(new Building
        {
            name = "Administration Building",
            position = new Vector3(-20, 0, 50),
            size = new Vector3(15, 0, 15),
            requiredAccess = AccessLevel.Level2_Staff
        });
        
        buildings.Add(new Building
        {
            name = "Control Center",
            position = new Vector3(20, 0, 50),
            size = new Vector3(12, 0, 12),
            requiredAccess = AccessLevel.Level4_Supervisor
        });
        
        buildings.Add(new Building
        {
            name = "Server Room",
            position = new Vector3(20, 0, 65),
            size = new Vector3(10, 0, 10),
            requiredAccess = AccessLevel.Level5_Executive
        });
        
        buildings.Add(new Building
        {
            name = "Medical Building",
            position = new Vector3(-35, 0, 50),
            size = new Vector3(12, 0, 12),
            requiredAccess = AccessLevel.Level2_Staff
        });
        
        buildings.Add(new Building
        {
            name = "Kitchen",
            position = new Vector3(-50, 0, 35),
            size = new Vector3(12, 0, 12),
            requiredAccess = AccessLevel.Level2_Staff
        });
        
        buildings.Add(new Building
        {
            name = "Programs Building",
            position = new Vector3(-35, 0, 70),
            size = new Vector3(15, 0, 15),
            requiredAccess = AccessLevel.Level1_Visitor
        });
        
        // HOUSING UNITS
        buildings.Add(new Building
        {
            name = "HousingUnitA",
            position = new Vector3(-50, 0, 70),
            size = new Vector3(20, 0, 20),
            requiredAccess = AccessLevel.Level1_Visitor
        });
        
        buildings.Add(new Building
        {
            name = "Housing Unit B",
            position = new Vector3(-50, 0, 95),
            size = new Vector3(20, 0, 20),
            requiredAccess = AccessLevel.Level1_Visitor
        });
        
        // RECREATION & OUTDOOR
        buildings.Add(new Building
        {
            name = "Recreation Yard",
            position = new Vector3(35, 0, 70),
            size = new Vector3(30, 0, 30),
            requiredAccess = AccessLevel.Level1_Visitor
        });
        
        Debug.Log($"[PrisonMapConfig] Initialized {buildings.Count} buildings");
    }
    
    public Building GetBuilding(string name)
    {
        foreach (var building in buildings)
        {
            if (building.name == name)
                return building;
        }
        return null;
    }
    
    public List<Building> GetAllBuildings() => buildings;
}
