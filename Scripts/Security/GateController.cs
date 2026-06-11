using UnityEngine;

/// <summary>
/// Controls gate behavior, access control, and animations.
/// Handles all three gates (A, B, C) with vehicle-scale sliding mechanics.
/// </summary>
public class GateController : MonoBehaviour
{
    [SerializeField] private string gateName = "Gate A";
    [SerializeField] private int requiredAccessLevel = 3; // Level 3 = Officer minimum
    [SerializeField] private float gateOpenDuration = 3f;
    [SerializeField] private float gateCloseDuration = 3f;
    [SerializeField] private Vector3 openPosition = Vector3.zero;
    [SerializeField] private Vector3 closedPosition = Vector3.zero;
    [SerializeField] private AudioClip accessBeepSound;
    [SerializeField] private AudioClip deniedBeepSound;
    [SerializeField] private AudioClip gateOpenCloseSound;
    
    private Vector3 _initialPosition;
    private bool _isOpen = false;
    private bool _isAnimating = false;
    private bool _isLocked = true;
    private float _animationTimer = 0f;
    private AudioSource _audioSource;
    private Vector3 _targetPosition;
    private float _autoCloseDuration = 5f; // Auto-close after 5 seconds
    private float _autoCloseTimer = 0f;
    
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
            HandleGateAnimation();
        }
        
        // Auto-close management
        if (_isOpen && !_isAnimating)
        {
            _autoCloseTimer += Time.deltaTime;
            if (_autoCloseTimer >= _autoCloseDuration)
            {
                CloseGate();
                _autoCloseTimer = 0f;
            }
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
        return playerData.CanAccessLevel(requiredAccessLevel) && !_isLocked;
    }
    
    private void GrantAccess()
    {
        // Play access granted sounds
        if (_audioSource != null)
        {
            _audioSource.PlayOneShot(accessBeepSound);
            _audioSource.PlayOneShot(gateOpenCloseSound, 0.6f);
        }
        
        Debug.Log($"[GateController] {gateName} - Access Granted");
        
        if (!_isOpen)
        {
            OpenGate();
        }
    }
    
    private void DenyAccess()
    {
        // Play access denied sound
        if (_audioSource != null)
        {
            _audioSource.PlayOneShot(deniedBeepSound);
        }
        
        Debug.Log($"[GateController] {gateName} - Access Denied");
    }
    
    private void OpenGate()
    {
        if (_isAnimating || _isOpen) return;
        
        _isOpen = true;
        _isAnimating = true;
        _animationTimer = 0f;
        _autoCloseTimer = 0f;
        _targetPosition = openPosition;
    }
    
    private void CloseGate()
    {
        if (_isAnimating || !_isOpen) return;
        
        _isOpen = false;
        _isAnimating = true;
        _animationTimer = 0f;
        _targetPosition = closedPosition;
        _isLocked = true;
    }
    
    private void HandleGateAnimation()
    {
        float duration = _isOpen ? gateOpenDuration : gateCloseDuration;
        _animationTimer += Time.deltaTime;
        
        if (_animationTimer >= duration)
        {
            _animationTimer = duration;
            _isAnimating = false;
        }
        
        // Smooth slide animation with easing
        float progress = _animationTimer / duration;
        progress = Mathf.SmoothStep(0, 1, progress);
        transform.position = Vector3.Lerp(
            _isOpen ? closedPosition : openPosition,
            _isOpen ? openPosition : closedPosition,
            progress
        );
    }
    
    public void LockGate()
    {
        _isLocked = true;
        if (_isOpen) CloseGate();
    }
    
    public void UnlockGate()
    {
        _isLocked = false;
    }
    
    public void SetRequiredAccessLevel(int level)
    {
        requiredAccessLevel = level;
    }
    
    public int GetRequiredAccessLevel() => requiredAccessLevel;
    public bool IsOpen => _isOpen;
    public bool IsLocked => _isLocked;
    public string GetGateName() => gateName;
}
