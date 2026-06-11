using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

/// <summary>
/// Manages all UI elements and HUD display.
/// Handles character creation, menus, and real-time information display.
/// </summary>
public class UIManager : MonoBehaviour
{
    [SerializeField] private Canvas mainCanvas;
    [SerializeField] private Canvas hudCanvas;
    [SerializeField] private Canvas menuCanvas;
    
    // HUD Elements
    [SerializeField] private TextMeshProUGUI playerNameDisplay;
    [SerializeField] private TextMeshProUGUI playerIDDisplay;
    [SerializeField] private TextMeshProUGUI playerRoleDisplay;
    [SerializeField] private TextMeshProUGUI locationDisplay;
    [SerializeField] private TextMeshProUGUI timeDisplay;
    [SerializeField] private Text interactionPromptText;
    [SerializeField] private Button inventoryButton;
    [SerializeField] private Button mapButton;
    [SerializeField] private Button characterButton;
    [SerializeField] private Button scheduleButton;
    
    // Character Creation UI
    [SerializeField] private GameObject characterCreationPanel;
    [SerializeField] private TMP_InputField firstNameInput;
    [SerializeField] private TMP_InputField lastNameInput;
    [SerializeField] private Button officerRoleButton;
    [SerializeField] private Button inmateRoleButton;
    [SerializeField] private Dropdown genderDropdown;
    [SerializeField] private Dropdown faceStyleDropdown;
    [SerializeField] private Dropdown personalityDropdown;
    [SerializeField] private Dropdown eyeColorDropdown;
    [SerializeField] private Button createCharacterButton;
    
    private PlayerData _currentPlayerData;
    private PlayerRole _selectedRole = PlayerRole.Officer;
    
    private static UIManager _instance;
    
    public static UIManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<UIManager>();
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
        InitializeUI();
    }
    
    private void InitializeUI()
    {
        // Setup character creation listeners
        officerRoleButton.onClick.AddListener(() => SelectRole(PlayerRole.Officer));
        inmateRoleButton.onClick.AddListener(() => SelectRole(PlayerRole.Inmate));
        createCharacterButton.onClick.AddListener(CreateCharacter);
        
        // Setup menu buttons
        inventoryButton.onClick.AddListener(OpenInventory);
        mapButton.onClick.AddListener(OpenMap);
        characterButton.onClick.AddListener(OpenCharacterMenu);
        scheduleButton.onClick.AddListener(OpenScheduleMenu);
        
        // Populate dropdowns
        PopulateDropdowns();
    }
    
    private void PopulateDropdowns()
    {
        // Gender
        genderDropdown.ClearOptions();
        genderDropdown.AddOptions(new List<string> { "Male", "Female" });
        
        // Face Style
        faceStyleDropdown.ClearOptions();
        faceStyleDropdown.AddOptions(new List<string> { "Professional", "Friendly", "Cute", "Stoic", "Aggressive", "Nervous" });
        
        // Personality
        personalityDropdown.ClearOptions();
        personalityDropdown.AddOptions(new List<string> { "Professional", "Friendly", "Quiet", "Confident", "Nervous", "Aggressive" });
        
        // Eye Color
        eyeColorDropdown.ClearOptions();
        eyeColorDropdown.AddOptions(new List<string> { "Brown", "Blue", "Green", "Hazel", "Gray" });
    }
    
    public void ShowCharacterCreation()
    {
        characterCreationPanel.SetActive(true);
        hudCanvas.gameObject.SetActive(false);
    }
    
    public void HideCharacterCreation()
    {
        characterCreationPanel.SetActive(false);
    }
    
    public void ShowGameHUD()
    {
        hudCanvas.gameObject.SetActive(true);
    }
    
    private void SelectRole(PlayerRole role)
    {
        _selectedRole = role;
        
        // Visual feedback
        officerRoleButton.interactable = role != PlayerRole.Officer;
        inmateRoleButton.interactable = role != PlayerRole.Inmate;
        
        Debug.Log($"[UIManager] Role selected: {role}");
    }
    
    private void CreateCharacter()
    {
        _currentPlayerData = new PlayerData
        {
            FirstName = firstNameInput.text,
            LastName = lastNameInput.text,
            Role = _selectedRole,
            Gender = (Gender)genderDropdown.value,
            FaceStyle = (FaceStyle)faceStyleDropdown.value,
            Personality = (Personality)personalityDropdown.value,
            EyeColor = (EyeColor)eyeColorDropdown.value,
            AccessLevel = _selectedRole == PlayerRole.Officer ? AccessLevel.Level3_Officer : AccessLevel.Level1_Visitor
        };
        
        // Pass to game manager
        GameManager.Instance.CreatePlayer(_currentPlayerData);
    }
    
    public void UpdatePlayerInfo(PlayerData playerData)
    {
        playerNameDisplay.text = playerData.FullName;
        playerIDDisplay.text = playerData.ID;
        playerRoleDisplay.text = playerData.Role.ToString();
    }
    
    public void UpdateTimeDisplay(float gameTime)
    {
        int hours = (int)gameTime;
        int minutes = (int)((gameTime - hours) * 60);
        timeDisplay.text = $"{hours:D2}:{minutes:D2}";
    }
    
    public void UpdateLocationDisplay(string location)
    {
        locationDisplay.text = location;
    }
    
    public void ShowInteractionPrompt(string message)
    {
        interactionPromptText.text = message;
        interactionPromptText.gameObject.SetActive(true);
    }
    
    public void HideInteractionPrompt()
    {
        interactionPromptText.gameObject.SetActive(false);
    }
    
    private void OpenInventory()
    {
        Debug.Log("[UIManager] Opening Inventory");
    }
    
    private void OpenMap()
    {
        Debug.Log("[UIManager] Opening Map");
    }
    
    private void OpenCharacterMenu()
    {
        Debug.Log("[UIManager] Opening Character Menu");
    }
    
    private void OpenScheduleMenu()
    {
        if (_currentPlayerData != null)
        {
            NPCRole role = _currentPlayerData.Role == PlayerRole.Officer ? NPCRole.Officer : NPCRole.Inmate;
            string schedule = ScheduleManager.Instance.GetScheduleDescription(
                GameManager.Instance.GetCurrentTime(),
                role
            );
            Debug.Log($"[UIManager] Current Schedule: {schedule}");
        }
    }
    
    private void Update()
    {
        // Update location based on player position
        if (GameManager.Instance.GetPlayer() != null)
        {
            UpdateLocationDisplay(GameManager.Instance.GetPlayer().CurrentLocation);
        }
    }
}
