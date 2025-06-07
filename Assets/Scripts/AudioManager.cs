using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [Header("Background Music")]
    public AudioClip backgroundMusic;
    
    private AudioSource musicSource;
    
    // Singleton
    public static AudioManager Instance { get; private set; }
    
    void Awake()
    {
        // Singleton - só uma instância
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            SetupAudio();
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    void SetupAudio()
    {
        // Criar AudioSource para música
        musicSource = gameObject.AddComponent<AudioSource>();
        musicSource.clip = backgroundMusic;
        musicSource.loop = true;
        musicSource.volume = 0.5f; // Volume da música (50%)
        musicSource.playOnAwake = false;
        
        // Iniciar música
        if (backgroundMusic != null)
        {
            musicSource.Play();
            Debug.Log("AudioManager: Música de fundo iniciada");
        }
        else
        {
            Debug.LogWarning("AudioManager: Nenhuma música configurada!");
        }
    }
    
    // Métodos para controlar música (opcionais)
    public void PlayMusic()
    {
        if (musicSource != null && !musicSource.isPlaying)
        {
            musicSource.Play();
        }
    }
    
    public void StopMusic()
    {
        if (musicSource != null && musicSource.isPlaying)
        {
            musicSource.Stop();
        }
    }
    
    public void SetMusicVolume(float volume)
    {
        if (musicSource != null)
        {
            musicSource.volume = volume;
        }
    }
} 