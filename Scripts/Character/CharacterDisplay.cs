using UnityEngine;
using TMPro;

/// <summary>
/// Renders character face, name, and identity display.
/// Handles eye tracking, blinking, and expressions.
/// </summary>
public class CharacterDisplay : MonoBehaviour
{
    [SerializeField] private Canvas characterCanvas;
    [SerializeField] private TextMeshProUGUI nameDisplay;
    [SerializeField] private TextMeshProUGUI idDisplay;
    [SerializeField] private TextMeshProUGUI roleDisplay;
    [SerializeField] private RawImage faceDisplay;
    [SerializeField] private RawImage leftEyeDisplay;
    [SerializeField] private RawImage rightEyeDisplay;
    [SerializeField] private TextMeshProUGUI expressionDisplay;
    
    private PlayerData _playerData;
    private float _blinkTimer = 0f;
    private float _blinkInterval = 4f;
    private bool _isBlinking = false;
    private Vector3 _eyeLookTarget = Vector3.zero;
    
    // Eye animation
    private float _eyeTrackSmoothing = 0.1f;
    private Vector3 _leftEyePupilOffset = Vector3.zero;
    private Vector3 _rightEyePupilOffset = Vector3.zero;
    
    public void Initialize(PlayerData playerData)
    {
        _playerData = playerData;
        
        // Update display
        nameDisplay.text = playerData.FullName;
        idDisplay.text = playerData.ID;
        roleDisplay.text = playerData.Role.ToString();
        
        // Generate and display face
        GenerateFace();
    }
    
    private void GenerateFace()
    {
        // Generate minimalist character face based on style
        string faceExpression = GetExpressionForStyle(_playerData.FaceStyle);
        expressionDisplay.text = faceExpression;
        
        // Set eye color
        UpdateEyeColor();
    }
    
    private string GetExpressionForStyle(FaceStyle style)
    {
        return style switch
        {
            FaceStyle.Professional => ":|",
            FaceStyle.Friendly => ":D",
            FaceStyle.Cute => ":3",
            FaceStyle.Stoic => "._.",
            FaceStyle.Aggressive => ">:[",
            FaceStyle.Nervous => ":s",
            _ => ":)"
        };
    }
    
    private void UpdateEyeColor()
    {
        // Eye colors will be rendered with colored pupils
        Color eyeColor = _playerData.EyeColor switch
        {
            EyeColor.Brown => new Color(0.6f, 0.4f, 0.2f),
            EyeColor.Blue => new Color(0.2f, 0.5f, 1f),
            EyeColor.Green => new Color(0.2f, 0.8f, 0.4f),
            EyeColor.Hazel => new Color(0.7f, 0.6f, 0.3f),
            EyeColor.Gray => new Color(0.7f, 0.7f, 0.7f),
            _ => Color.brown
        };
    }
    
    private void Update()
    {
        HandleBlinking();
        UpdateEyeTracking();
    }
    
    private void HandleBlinking()
    {
        _blinkTimer += Time.deltaTime;
        
        if (_blinkTimer >= _blinkInterval)
        {
            _isBlinking = !_isBlinking;
            _blinkTimer = 0f;
            
            if (!_isBlinking)
            {
                _blinkInterval = Random.Range(3f, 6f);
            }
        }
        
        // Update expression based on blink state
        if (_isBlinking)
        {
            expressionDisplay.text = GetExpressionForStyle(_playerData.FaceStyle).Replace(')', '-').Replace('D', '-').Replace('3', '-');
        }
        else
        {
            expressionDisplay.text = GetExpressionForStyle(_playerData.FaceStyle);
        }
    }
    
    private void UpdateEyeTracking()
    {
        // Track nearby characters or objects
        Collider[] nearbyColliders = Physics.OverlapSphere(transform.position, 10f);
        
        foreach (var collider in nearbyColliders)
        {
            if (collider.CompareTag("NPC") || collider.CompareTag("Player"))
            {
                _eyeLookTarget = collider.transform.position;
                break;
            }
        }
        
        // Smoothly track target
        Vector3 directionToTarget = (_eyeLookTarget - transform.position).normalized;
        _leftEyePupilOffset = Vector3.Lerp(_leftEyePupilOffset, directionToTarget * 0.3f, _eyeTrackSmoothing);
        _rightEyePupilOffset = Vector3.Lerp(_rightEyePupilOffset, directionToTarget * 0.3f, _eyeTrackSmoothing);
    }
    
    public void ChangeExpression(FaceStyle newStyle)
    {
        _playerData.FaceStyle = newStyle;
        expressionDisplay.text = GetExpressionForStyle(newStyle);
    }
}
