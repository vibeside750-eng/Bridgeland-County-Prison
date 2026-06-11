using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Manages prison map layout, locations, and pathfinding.
/// Handles door/gate interactions and navigation.
/// </summary>
public class MapManager : MonoBehaviour
{
    [System.Serializable]
    public class LocationSpawn
    {
        public string locationName;
        public Vector3 spawnPosition;
        public float spawnRadius = 2f;
    }
    
    [SerializeField] private List<LocationSpawn> locationSpawns = new List<LocationSpawn>();
    [SerializeField] private List<DoorController> doors = new List<DoorController>();
    [SerializeField] private List<GateController> gates = new List<GateController>();
    
    private Dictionary<string, LocationSpawn> _locationMap = new Dictionary<string, LocationSpawn>();
    private NavMeshPath _pathfindingPath;
    
    public void InitializeMap()
    {
        // Build location dictionary
        foreach (var spawn in locationSpawns)
        {
            _locationMap[spawn.locationName] = spawn;
        }
        
        // Initialize pathfinding
        _pathfindingPath = new NavMeshPath();
        
        Debug.Log("[MapManager] Prison map initialized. Locations: " + _locationMap.Count);
    }
    
    public Vector3 GetLocationSpawn(string locationName)
    {
        if (_locationMap.TryGetValue(locationName, out var spawn))
        {
            // Add random offset within spawn radius
            Vector2 randomOffset = Random.insideUnitCircle * spawn.spawnRadius;
            return spawn.spawnPosition + new Vector3(randomOffset.x, 0, randomOffset.y);
        }
        
        Debug.LogWarning($"[MapManager] Location not found: {locationName}");
        return Vector3.zero;
    }
    
    public bool CanNavigateTo(Vector3 from, Vector3 to)
    {
        return NavMesh.CalculatePath(from, to, NavMesh.AllAreas, _pathfindingPath) && _pathfindingPath.status == NavMeshPathStatus.PathComplete;
    }
    
    public Vector3[] GetPath(Vector3 from, Vector3 to)
    {
        if (NavMesh.CalculatePath(from, to, NavMesh.AllAreas, _pathfindingPath))
        {
            return _pathfindingPath.corners;
        }
        return new Vector3[] { from };
    }
    
    public DoorController GetNearestDoor(Vector3 position, float maxDistance = 2f)
    {
        DoorController nearest = null;
        float nearestDistance = maxDistance;
        
        foreach (var door in doors)
        {
            float distance = Vector3.Distance(position, door.transform.position);
            if (distance < nearestDistance)
            {
                nearest = door;
                nearestDistance = distance;
            }
        }
        
        return nearest;
    }
    
    public GateController GetNearestGate(Vector3 position, float maxDistance = 3f)
    {
        GateController nearest = null;
        float nearestDistance = maxDistance;
        
        foreach (var gate in gates)
        {
            float distance = Vector3.Distance(position, gate.transform.position);
            if (distance < nearestDistance)
            {
                nearest = gate;
                nearestDistance = distance;
            }
        }
        
        return nearest;
    }
    
    public List<DoorController> GetAllDoors() => doors;
    public List<GateController> GetAllGates() => gates;
    
    // Prison layout structure (for reference)
    // Public Road
    // ↓ Visitor Entrance
    // ↓ Visitor Parking
    // ↓ Staff Parking
    // ↓ Staff Security Checkpoint (Gate A - Level 3)
    // ↓ Vehicle Inspection Area
    // ↓ Gate B - Level 3
    // ↓ Security Processing Corridor
    // ↓ Gate C - Level 3
    // ↓ Administration Building
    // ↓ Medical Building
    // ↓ Kitchen
    // ↓ Programs Building
    // ↓ Housing Unit A
    // ↓ Recreation Yard
    // ↓ Control Center
    // ↓ Server Room
}
