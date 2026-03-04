using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource ambientSource;
    [SerializeField] private AudioSource sfxSource;

    [SerializeField] private AudioClip backgroundMusicClip;
    [SerializeField] private AudioClip ambientClip;
    [SerializeField] private AudioClip diceThrowClip;
    [SerializeField] private AudioClip diceImpactClip;
    [SerializeField] private AudioClip footstepClip;
    [SerializeField] private AudioClip finalTileFlashClip;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        PlayMusic();
        PlayAmbient();
    }

    public void PlayMusic()
    {
        if (backgroundMusicClip != null && musicSource != null)
        {
            musicSource.clip = backgroundMusicClip;
            musicSource.loop = true;
            musicSource.Play();
        }
    }

    private void PlayAmbient()
    {
        if (ambientClip != null && ambientSource != null)
        {
            ambientSource.clip = ambientClip;
            ambientSource.loop = true;
            ambientSource.Play();
        }
    }

    public void PlayDiceThrow() => PlaySFX(diceThrowClip);
    public void PlayDiceImpact() => PlaySFX(diceImpactClip, Random.Range(0.9f, 1.1f)); //biraz aralýk yaptým ki hep ayný týný kulakta çýnlamasýn.
    public void PlayStepSound() => PlaySFX(footstepClip, Random.Range(1.0f, 1.05f));
    public void PlayFinalTileSound() => PlaySFX(finalTileFlashClip);

    private void PlaySFX(AudioClip clip, float pitch = 1.0f)
    {
        if (clip == null || sfxSource == null) return;
        sfxSource.pitch = pitch;
        sfxSource.PlayOneShot(clip);
    }
}