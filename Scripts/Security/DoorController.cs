using UnityEngine;

/// <summary>
/// Controls door behavior, access control, and animations.
/// Handles keycard access verification and automatic locking/unlocking.
/// </summary>
public class DoorController : MonoBehaviour
{
    [SerializeField] private int requiredAccessLevel = 1;
    [SerializeField] private float doorOpenDuration = 2f;
    [SerializeField] private float doorCloseDuration = 1.5f;
    [SerializeField] private Vector3 openPosition = Vector3.zero;
    [SerializeField] private Vector3 closedPosition = Vector3.zero;
    [SerializeField] private AudioClip accessBeepSound;
    [SerializeField] private AudioClip deniedBeepSound;
    [SerializeField] private AudioClip doorOpenCloseSound;
    
    private Vector3 _initialPosition;
    private bool _isOpen = false;
    private bool _isAnimating = false;
    private float _animationTimer = 0f;
    private AudioSource _audioSource;
    private Vector3 _targetPosition;
    
    private void Start()
    {
        _initialPosition = transform.position;
        closedPosition = _initialPosition;
        _audioSource = GetComponent<AudioSource>();
        if (_audioSource == null)
        {
            _audioSource = gameObject.AddComponent<AudioSource>();
        }
    }
    
    private void Update()
    {
        if (_isAnimating)
        {
            HandleDoorAnimation();
        }
    }
    
    public void RequestAccess(PlayerData playerData)
    {
        if (CanAccess(playerData))
        {
            GrantAccess();
        }
        else
        {
            DenyAccess();
        }
    }
    
    private bool CanAccess(PlayerData playerData)
    {
        return playerData.CanAccessLevel(requiredAccessLevel);
    }
    
    private void GrantAccess()
    {
        // Play access granted sounds
        if (_audioSource != null)
        {
            _audioSource.PlayOneShot(accessBeepSound);
            _audioSource.PlayOneShot(doorOpenCloseSound, 0.5f);
        }
        
        if (!_isOpen)
        {
            OpenDoor();
        }
    }
    
    private void DenyAccess()
    {
        // Play access denied sound
        if (_audioSource != null)
        {
            _audioSource.PlayOneShot(deniedBeepSound);
        }
        
        Debug.Log("[DoorController] Access Denied");
    }
    
    private void OpenDoor()
    {
        if (_isAnimating) return;
        
        _isOpen = true;
        _isAnimating = true;
        _animationTimer = 0f;
        _targetPosition = openPosition;
    }
    
    private void CloseDoor()
    {
        if (_isAnimating) return;
        
        _isOpen = false;
        _isAnimating = true;
        _animationTimer = 0f;
        _targetPosition = closedPosition;
    }
    
    private void HandleDoorAnimation()
    {
        float duration = _isOpen ? doorOpenDuration : doorCloseDuration;
        _animationTimer += Time.deltaTime;
        
        if (_animationTimer >= duration)
        {
            _animationTimer = duration;
            _isAnimating = false;
            
            // Auto-close door after opening
            if (_isOpen && _animationTimer >= doorOpenDuration)
            {
                Invoke("CloseDoor", 1f);
            }
        }
        
        // Smooth slide animation
        float progress = _animationTimer / duration;
        progress = Mathf.SmoothStep(0, 1, progress); // Easing
        transform.position = Vector3.Lerp(
            _isOpen ? closedPosition : openPosition,
            _isOpen ? openPosition : closedPosition,
            progress
        );
    }
    
    public void SetRequiredAccessLevel(int level)
    {
        requiredAccessLevel = level;
    }
    
    public int GetRequiredAccessLevel() => requiredAccessLevel;
    public bool IsOpen => _isOpen;
}
