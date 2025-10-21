using System.Collections;
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

        SetupMixersSources(music_poolsize, ui_poolsize, sfx_poolsize);
    }

    [Header("Audio Files")]
    [SerializeField] private AudioClip doorSFX;
    [SerializeField] private AudioClip waterRunningSFX;
    [SerializeField] private AudioClip lightSwitchSFX;
    [SerializeField] private AudioClip washingDishesSFX;
    [SerializeField] private AudioClip floatingSkullLaughSFX;
    [SerializeField] private AudioClip tvRadioSFX;
    [SerializeField] private AudioClip laundrySFX;
    [SerializeField] private AudioClip rainSFX;
    public enum SFX { DOOR, WATER_RUNNING, LIGHT_SWITCH, WASHING_DISHES, FLOATING_SKULL_LAUGH, TV_RADIO, LAUNDRY, RAIN };

    [Header("Music Files")]
    [SerializeField] private AudioClip menuTheme;
    [SerializeField] private AudioClip gameTheme;
    [SerializeField] private AudioClip creditsTheme;
    public enum Music { MENU, GAME, CREDITS };

    [Header("Audio Pools & Sources")]
    [SerializeField] private AudioMixer mainAudioMixer;
    [SerializeField] private AudioMixerGroup masterAMG;

    [SerializeField] private int music_poolsize = 1;
    [SerializeField] private AudioMixerGroup musicAMG;
    private AudioSource[] musicSources;

    [SerializeField] private int ui_poolsize = 3;
    [SerializeField] private AudioMixerGroup uiAMG;
    private AudioSource[] uiSources;

    [SerializeField] private int sfx_poolsize = 3;
    [SerializeField] private AudioMixerGroup sfxAMG;
    private AudioSource[] sfxSources;

    private float VolumeToDecibels(int volume)
    {
        volume = Mathf.Clamp(volume, 0, 100);
        float volumef = volume / 100f;
        if (volumef <= 0f) volumef = -80;
        return Mathf.Log10(volumef) * 20f;
    }

    private void SetMixerGroupVolumes()
    {
        mainAudioMixer.SetFloat("MasterVolume", VolumeToDecibels(SettingsManager.Instance.masterVolume));
        mainAudioMixer.SetFloat("MusicVolume", VolumeToDecibels(SettingsManager.Instance.musicVolume));
        mainAudioMixer.SetFloat("SFXVolume", VolumeToDecibels(SettingsManager.Instance.sfxVolume));
        mainAudioMixer.SetFloat("UIVolume", VolumeToDecibels(SettingsManager.Instance.uiVolume));
    }

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

    void Start()
    {
        if (mainAudioMixer == null)
        {
            Debug.LogError("mainAudioMixer is null!");
            return;
        }

        float dummy;
        Debug.Log($"MasterVolume exists? {mainAudioMixer.GetFloat("MasterVolume", out dummy)}");
        Debug.Log($"MusicVolume exists? {mainAudioMixer.GetFloat("MusicVolume", out dummy)}");
        Debug.Log($"SFXVolume exists? {mainAudioMixer.GetFloat("SFXVolume", out dummy)}");
        Debug.Log($"UIVolume exists? {mainAudioMixer.GetFloat("UIVolume", out dummy)}");

        SetMixerGroupVolumes();
    }

    void OnEnable()
    {
        EventManager.Instance.OnSoundInteraction += HandleSound;
    }

    void OnDisable()
    {
        EventManager.Instance.OnSoundInteraction -= HandleSound;
    }

    private void HandleSound(AudioClip sound)
    {
        PlaySound(sound);
    }

    void HandleInteraction()
    {
        
    }

    public bool PlayMusic(Music music, bool forceChange = false)
    {
        foreach (AudioSource audioSource in musicSources)
        {
            // This AudioSource is being used
            if (audioSource.isPlaying && !forceChange) continue;

            switch (music)
            {
                case Music.MENU:
                    audioSource.clip = menuTheme;
                    break;

                case Music.GAME:
                    audioSource.clip = gameTheme;
                    break;

                case Music.CREDITS:
                    audioSource.clip = creditsTheme;
                    break;
            }

            audioSource.loop = true;
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

    public bool PlaySound(SFX sfx)
    {
        foreach (AudioSource audioSource in sfxSources)
        {
            // This AudioSource is being used
            if (audioSource.isPlaying)
            {
                continue;
            }

            else
            {
                audioSource.clip = null;
            }

            switch (sfx)
            {
                case SFX.WATER_RUNNING:
                    audioSource.clip = waterRunningSFX;
                    //audioSource.loop = true;
                    break;

                case SFX.DOOR:
                    audioSource.clip = doorSFX;
                    break;

                /*case SFX.TV_RADIO:
                    audioSource.clip = tvRadioSFX;
                    break;*/

                case SFX.LIGHT_SWITCH:
                    audioSource.clip = lightSwitchSFX;
                    break;

                case SFX.WASHING_DISHES:
                    audioSource.clip = washingDishesSFX;
                    break;

                case SFX.LAUNDRY:
                    audioSource.clip = laundrySFX;
                    break;

                case SFX.FLOATING_SKULL_LAUGH:
                    audioSource.clip = floatingSkullLaughSFX;
                    break;

                /*case SFX.RAIN:
                    audioSource.clip = rainSFX;
                    //audioSource.loop = true;
                    break;*/
            }

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
