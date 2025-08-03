using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    [SerializeField] private int music_poolsize = 1;
    [SerializeField] private AudioMixerGroup musicAMG;
    private AudioSource[] musicSources;

    [SerializeField] private int ui_poolsize = 3;
    [SerializeField] private AudioMixerGroup uiAMG;
    private AudioSource[] uiSources;

    [SerializeField] private int sfx_poolsize = 3;
    [SerializeField] private AudioMixerGroup sfxAMG;
    private AudioSource[] sfxSources;

    private void SetupMixersSources(int music_poolsize = 1, int ui_poolsize = 3, int sfx_poolsize = 3)
    {
        musicSources = new AudioSource[music_poolsize];
        for (int i = 0; i < music_poolsize; i++)
        {
            AudioSource audioSource = this.AddComponent<AudioSource>();
            musicSources[i] = audioSource;
            audioSource.outputAudioMixerGroup = musicAMG;
        }

        uiSources = new AudioSource[ui_poolsize];
        for (int i = 0; i < ui_poolsize; i++)
        {
            AudioSource audioSource = this.AddComponent<AudioSource>();
            uiSources[i] = audioSource;
            audioSource.outputAudioMixerGroup = uiAMG;
        }

        sfxSources = new AudioSource[sfx_poolsize];
        for (int i = 0; i < sfx_poolsize; i++)
        {
            AudioSource audioSource = this.AddComponent<AudioSource>();
            sfxSources[i] = audioSource;
            audioSource.outputAudioMixerGroup = sfxAMG;
        }
    }

    void OnEnable()
    {
        EventManager.Instance.OnSoundInteraction += (sound) => { PlaySound(sound); };
    }

    void Osable()
    {
        EventManager.Instance.OnSoundInteraction -= (sound) => { PlaySound(sound); };
    }

    void Start()
    {
        SetupMixersSources(music_poolsize, ui_poolsize, sfx_poolsize);
    }

    public bool PlayMusic(AudioClip musicClip, bool forceChange = false)
    {
        foreach (AudioSource audioSource in musicSources)
        {
            // This AudioSource is being used
            if (audioSource.isPlaying && !forceChange)
            {
                continue;
            }

            audioSource.clip = musicClip;
            audioSource.Play();
            return true;
        }

        // Couldn't find an available AudioClip from the pool
        return false;
    }

    public bool PlayUI(AudioClip uiClip)
    {
        foreach (AudioSource audioSource in uiSources)
        {
            // This AudioSource is being used
            if (audioSource.isPlaying)
            {
                continue;
            }

            audioSource.clip = uiClip;
            audioSource.Play();
            return true;
        }

        // Couldn't find an available AudioClip from the pool
        return false;
    }

    public bool PlaySound(AudioClip sfxClip)
    {
        foreach (AudioSource audioSource in sfxSources)
        {
            // This AudioSource is being used
            if (audioSource.isPlaying)
            {
                continue;
            }

            audioSource.clip = sfxClip;
            audioSource.Play();
            return true;
        }

        // Couldn't find an available AudioClip from the pool
        return false;
    }

}
