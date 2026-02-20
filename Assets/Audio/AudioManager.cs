using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    public AudioSource bgmSource;
    public AudioSource sfxSource;

    public AudioClip bgmClip;
    public AudioClip collisionClip;
    public AudioClip respawnClip;
    public AudioClip launchClip;
    public AudioClip scoreClip;
    public AudioClip portalClip;
    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    void Start()
    {
        if (bgmSource != null && bgmClip != null)
        {
            bgmSource.clip = bgmClip;
            bgmSource.loop = true;
            bgmSource.Play();
        }
    }

    public void PlayCollision()
    {
        if (collisionClip != null)
            sfxSource.PlayOneShot(collisionClip);
    }

    public void PlayScore()
    {
        if(scoreClip != null)
            sfxSource.PlayOneShot(scoreClip);
    }

    public void PlayRespawn()
    {
        if (respawnClip != null)
            sfxSource.PlayOneShot(respawnClip);
    }

    public void PlayLaunch()
    {
        if (launchClip != null)
            sfxSource.PlayOneShot(launchClip);
    }
}
