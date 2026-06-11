using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Controls player character movement, interaction, and state.
/// </summary>
public class PlayerController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private CharacterDisplay characterDisplay;
    [SerializeField] private NavMeshAgent navMeshAgent;
    [SerializeField] private float interactionRange = 2.5f;
    
    private PlayerData _playerData;
    private MapManager _mapManager;
    private bool _isMoving;
    private Vector3 _moveDirection;
    private DoorController _nearestDoor;
    private GateController _nearestGate;
    
    public PlayerData PlayerData => _playerData;
    public string CurrentLocation { get; private set; }
    
    private void Start()
    {
        _mapManager = GameManager.Instance.GetComponent<MapManager>();
        navMeshAgent.speed = moveSpeed;
    }
    
    public void InitializePlayer(PlayerData playerData)
    {
        _playerData = playerData;
        characterDisplay.Initialize(playerData);
        CurrentLocation = playerData.Role == PlayerRole.Officer ? "StaffParking" : "HousingUnitA";
    }
    
    private void Update()
    {
        HandleInput();
        UpdateInteractionPrompts();
    }
    
    private void HandleInput()
    {
        // Get movement input (WASD or joystick)
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        
        Vector3 inputDirection = new Vector3(horizontal, 0, vertical).normalized;
        
        if (inputDirection.magnitude > 0.1f)
        {
            // Move character using NavMesh
            Vector3 targetPosition = transform.position + inputDirection * moveSpeed * Time.deltaTime;
            navMeshAgent.SetDestination(targetPosition);
            _isMoving = true;
            
            // Update character rotation to face movement direction
            if (inputDirection != Vector3.zero)
            {
                transform.rotation = Quaternion.LookRotation(inputDirection);
            }
        }
        else
        {
            _isMoving = false;
            navMeshAgent.velocity = Vector3.zero;
        }
        
        // Interaction input (E key or context button)
        if (Input.GetKeyDown(KeyCode.E))
        {
            TryInteract();
        }
    }
    
    private void UpdateInteractionPrompts()
    {
        // Check for nearby doors
        _nearestDoor = _mapManager.GetNearestDoor(transform.position, interactionRange);
        
        // Check for nearby gates
        _nearestGate = _mapManager.GetNearestGate(transform.position, interactionRange);
        
        // Update UI with interaction prompts
        UIManager uiManager = FindObjectOfType<UIManager>();
        if (uiManager != null)
        {
            if (_nearestDoor != null)
            {
                uiManager.ShowInteractionPrompt("Press E to interact with door");
            }
            else if (_nearestGate != null)
            {
                uiManager.ShowInteractionPrompt("Press E to interact with gate");
            }
            else
            {
                uiManager.HideInteractionPrompt();
            }
        }
    }
    
    private void TryInteract()
    {
        if (_nearestDoor != null)
        {
            _nearestDoor.RequestAccess(_playerData);
        }
        else if (_nearestGate != null)
        {
            _nearestGate.RequestAccess(_playerData);
        }
    }
    
    public void UpdateLocation(string newLocation)
    {
        CurrentLocation = newLocation;
    }
}
