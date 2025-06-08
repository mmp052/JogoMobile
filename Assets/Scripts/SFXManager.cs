using UnityEngine;

public class SFXManager : MonoBehaviour
{
    public AudioClip clickSound;
    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void PlayClick()
    {
        audioSource.PlayOneShot(clickSound);
    }
}
