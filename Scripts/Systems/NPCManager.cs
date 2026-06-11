using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Manages NPC spawning and lifecycle.
/// Creates officers and inmates at appropriate locations.
/// </summary>
public class NPCManager : MonoBehaviour
{
    [SerializeField] private OfficerController officerPrefab;
    [SerializeField] private InmateController inmatePrefab;
    [SerializeField] private int initialOfficerCount = 5;
    [SerializeField] private int initialInmateCount = 15;
    
    private List<OfficerController> _officers = new List<OfficerController>();
    private List<InmateController> _inmates = new List<InmateController>();
    
    private static NPCManager _instance;
    
    public static NPCManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<NPCManager>();
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
    }
    
    private void Start()
    {
        SpawnNPCs();
    }
    
    private void SpawnNPCs()
    {
        MapManager mapManager = FindObjectOfType<MapManager>();
        
        // Spawn officers
        for (int i = 0; i < initialOfficerCount; i++)
        {
            OfficerController officer = Instantiate(officerPrefab);
            NPCData officerData = GenerateOfficerData();
            officer.InitializeNPC(officerData);
            
            // Position at staff parking
            Vector3 spawnPos = mapManager.GetLocationSpawn("StaffParking");
            officer.transform.position = spawnPos;
            
            _officers.Add(officer);
        }
        
        // Spawn inmates
        for (int i = 0; i < initialInmateCount; i++)
        {
            InmateController inmate = Instantiate(inmatePrefab);
            NPCData inmateData = GenerateInmateData();
            inmate.InitializeNPC(inmateData);
            
            // Position at housing unit A
            Vector3 spawnPos = mapManager.GetLocationSpawn("HousingUnitA");
            inmate.transform.position = spawnPos;
            
            _inmates.Add(inmate);
        }
        
        Debug.Log($"[NPCManager] Spawned {_officers.Count} officers and {_inmates.Count} inmates");
    }
    
    private NPCData GenerateOfficerData()
    {
        string[] firstNames = { "James", "Robert", "Michael", "David", "Richard", "Joseph", "Thomas", "Charles", "Patricia", "Jennifer", "Mary", "Linda", "Barbara", "Susan" };
        string[] lastNames = { "Smith", "Johnson", "Williams", "Brown", "Jones", "Garcia", "Miller", "Davis", "Rodriguez", "Martinez" };
        
        NPCData data = new NPCData
        {
            FirstName = firstNames[Random.Range(0, firstNames.Length)],
            LastName = lastNames[Random.Range(0, lastNames.Length)],
            Role = NPCRole.Officer,
            Gender = Random.value > 0.5f ? Gender.Male : Gender.Female,
            FaceStyle = (FaceStyle)Random.Range(0, 6),
            Personality = (Personality)Random.Range(0, 6),
            EyeColor = (EyeColor)Random.Range(0, 5),
            AccessLevel = (AccessLevel)Random.Range(3, 6) // Level 3-5 for officers
        };
        
        return data;
    }
    
    private NPCData GenerateInmateData()
    {
        string[] firstNames = { "John", "Marcus", "Anthony", "Christopher", "Raymond", "Luis", "Angel", "Jesus", "Darius", "Andre" };
        string[] lastNames = { "Smith", "Johnson", "Williams", "Brown", "Jones", "Garcia", "Miller", "Davis", "Rodriguez", "Martinez" };
        
        NPCData data = new NPCData
        {
            FirstName = firstNames[Random.Range(0, firstNames.Length)],
            LastName = lastNames[Random.Range(0, lastNames.Length)],
            Role = NPCRole.Inmate,
            Gender = Gender.Male, // Simplification: most inmates are male
            FaceStyle = (FaceStyle)Random.Range(0, 6),
            Personality = (Personality)Random.Range(0, 6),
            EyeColor = (EyeColor)Random.Range(0, 5),
            AccessLevel = AccessLevel.Level1_Visitor
        };
        
        return data;
    }
    
    public List<OfficerController> GetAllOfficers() => _officers;
    public List<InmateController> GetAllInmates() => _inmates;
    
    public OfficerController GetRandomOfficer() => _officers[Random.Range(0, _officers.Count)];
    public InmateController GetRandomInmate() => _inmates[Random.Range(0, _inmates.Count)];
}
