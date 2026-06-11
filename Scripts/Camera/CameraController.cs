using UnityEngine;

/// <summary>
/// Manages top-down camera follow and smooth tracking.
/// Maintains strict orthographic top-down perspective.
/// </summary>
public class CameraController : MonoBehaviour
{
    [SerializeField] private Transform playerTarget;
    [SerializeField] private float followSpeed = 5f;
    [SerializeField] private Vector3 cameraOffset = new Vector3(0, 15, 0);
    [SerializeField] private bool isOrthographic = true;
    [SerializeField] private float orthographicSize = 10f;
    [SerializeField] private float minX = -50f;
    [SerializeField] private float maxX = 50f;
    [SerializeField] private float minZ = -50f;
    [SerializeField] private float maxZ = 50f;
    
    private Camera _camera;
    private Vector3 _targetPosition;
    
    private void Start()
    {
        _camera = GetComponent<Camera>();
        _camera.orthographic = isOrthographic;
        _camera.orthographicSize = orthographicSize;
    }
    
    private void LateUpdate()
    {
        if (playerTarget == null) return;
        
        // Calculate target position with offset
        _targetPosition = playerTarget.position + cameraOffset;
        
        // Clamp to map boundaries
        _targetPosition.x = Mathf.Clamp(_targetPosition.x, minX, maxX);
        _targetPosition.z = Mathf.Clamp(_targetPosition.z, minZ, maxZ);
        
        // Smooth follow
        transform.position = Vector3.Lerp(transform.position, _targetPosition, followSpeed * Time.deltaTime);
        
        // Always look straight down (top-down)
        transform.rotation = Quaternion.Euler(90, 0, 0);
    }
    
    public void SetTarget(Transform target)
    {
        playerTarget = target;
    }
    
    public void SetFollowSpeed(float speed)
    {
        followSpeed = speed;
    }
    
    public void SetOrthographicSize(float size)
    {
        orthographicSize = size;
        if (_camera != null)
            _camera.orthographicSize = size;
    }
}
