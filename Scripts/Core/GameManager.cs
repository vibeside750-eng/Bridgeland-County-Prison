using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Central game state manager for the prison simulation.
/// Handles initialization, game flow, and global state.
/// </summary>
public class GameManager : MonoBehaviour
{
    [SerializeField] private PlayerController playerPrefab;
    [SerializeField] private MapManager mapManager;
    [SerializeField] private UIManager uiManager;
    [SerializeField] private AudioManager audioManager;
    
    private static GameManager _instance;
    private PlayerController _player;
    private bool _isGameActive;
    
    // Game state
    private float _currentTime = 6.0f; // 6:00 AM start
    private int _currentDay = 1;
    public float TimeScale = 1.0f; // 30 real minutes = 1 prison day
    
    public static GameManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<GameManager>();
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
        DontDestroyOnLoad(gameObject);
    }
    
    private void Start()
    {
        InitializeGame();
    }
    
    private void InitializeGame()
    {
        _isGameActive = true;
        
        // Initialize managers
        mapManager.InitializeMap();
        audioManager.PlayAmbientAudio();
        
        // Show character creation
        uiManager.ShowCharacterCreation();
    }
    
    public void CreatePlayer(PlayerData playerData)
    {
        // Instantiate player at spawn location
        Vector3 spawnLocation = playerData.Role == PlayerRole.Officer 
            ? mapManager.GetLocationSpawn("StaffParking")
            : mapManager.GetLocationSpawn("HousingUnitA");
        
        _player = Instantiate(playerPrefab, spawnLocation, Quaternion.identity);
        _player.InitializePlayer(playerData);
        
        // Update UI
        uiManager.UpdatePlayerInfo(playerData);
        uiManager.HideCharacterCreation();
        uiManager.ShowGameHUD();
    }
    
    private void Update()
    {
        if (!_isGameActive) return;
        
        // Update game time (30 real minutes = 1 prison day cycle)
        _currentTime += Time.deltaTime * (TimeScale / 1800f); // 1800 seconds = 30 minutes
        
        if (_currentTime >= 22.0f) // 10:00 PM
        {
            _currentTime = 6.0f;
            _currentDay++;
        }
        
        // Update UI time display
        uiManager.UpdateTimeDisplay(_currentTime);
    }
    
    public float GetCurrentTime() => _currentTime;
    public int GetCurrentDay() => _currentDay;
    public PlayerController GetPlayer() => _player;
    
    public void PauseGame()
    {
        _isGameActive = false;
        Time.timeScale = 0f;
    }
    
    public void ResumeGame()
    {
        _isGameActive = true;
        Time.timeScale = 1f;
    }
}
