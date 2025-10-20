using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingsManager : MonoBehaviour
{
    public static SettingsManager Instance { get; private set; }

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

    [Header("Sound Settings")]
    [SerializeField] public bool muteAudio = false;
    [SerializeField] public int masterVolume = 80;
    [SerializeField] public int musicVolume = 40;
    [SerializeField] public int sfxVolume = 60;
    [SerializeField] public int uiVolume = 50;

}
