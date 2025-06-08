using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [Header("Background Music")]
    public AudioClip backgroundMusic;
    
    [Header("Sound Effects")]
    public AudioClip shootSound; // Som de tiro padrão
    
    private AudioSource musicSource;
    private AudioSource sfxSource; // Para efeitos sonoros
    
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
        
        // Criar AudioSource para efeitos sonoros
        sfxSource = gameObject.AddComponent<AudioSource>();
        sfxSource.playOnAwake = false;
        sfxSource.volume = 0.05f; // Volume dos efeitos sonoros (5%)
        
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
    
    // Métodos para efeitos sonoros
    public void PlayShootSound()
    {
        if (shootSound != null && sfxSource != null)
        {
            sfxSource.PlayOneShot(shootSound);
        }
    }
    
    public void PlaySoundEffect(AudioClip clip)
    {
        if (clip != null && sfxSource != null)
        {
            sfxSource.PlayOneShot(clip);
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
    
    public void SetSFXVolume(float volume)
    {
        if (sfxSource != null)
        {
            sfxSource.volume = volume;
        }
    }
} 