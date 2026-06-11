using UnityEngine;

/// <summary>
/// Manages ambient audio for the prison simulation.
/// Handles environmental sounds and audio cues.
/// </summary>
public class AudioManager : MonoBehaviour
{
    [SerializeField] private AudioSource exteriorAmbience;
    [SerializeField] private AudioSource interiorAmbience;
    [SerializeField] private AudioClip outsideAmbienceClip;
    [SerializeField] private AudioClip hvacClip;
    [SerializeField] private AudioClip lightHumClip;
    [SerializeField] private float interiorVolume = 0.3f;
    [SerializeField] private float exteriorVolume = 0.2f;
    
    private static AudioManager _instance;
    
    public static AudioManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<AudioManager>();
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
    
    public void PlayAmbientAudio()
    {
        if (exteriorAmbience != null && outsideAmbienceClip != null)
        {
            exteriorAmbience.clip = outsideAmbienceClip;
            exteriorAmbience.volume = exteriorVolume;
            exteriorAmbience.loop = true;
            exteriorAmbience.Play();
        }
        
        if (interiorAmbience != null && hvacClip != null)
        {
            interiorAmbience.clip = hvacClip;
            interiorAmbience.volume = interiorVolume;
            interiorAmbience.loop = true;
            interiorAmbience.Play();
        }
    }
    
    public void StopAmbientAudio()
    {
        if (exteriorAmbience != null)
            exteriorAmbience.Stop();
        if (interiorAmbience != null)
            interiorAmbience.Stop();
    }
    
    public void SetInteriorVolume(float volume)
    {
        if (interiorAmbience != null)
            interiorAmbience.volume = volume;
    }
    
    public void SetExteriorVolume(float volume)
    {
        if (exteriorAmbience != null)
            exteriorAmbience.volume = volume;
    }
}
