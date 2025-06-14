using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    public AudioClip shootClip;
    public AudioClip reloadClip;

    private AudioSource gunSound;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            gunSound = GetComponent<AudioSource>();
            if (gunSound == null)
                gunSound = gameObject.AddComponent<AudioSource>();
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

    public void PlayShoot()
    {
        PlaySound(shootClip);
    }

    public void PlayReload()
    {
        PlaySound(reloadClip);
    }

    private void PlaySound(AudioClip clip)
    {
        if (clip != null)
            gunSound.PlayOneShot(clip);
    }
}
