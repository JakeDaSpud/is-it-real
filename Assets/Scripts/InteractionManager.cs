using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionManager : MonoBehaviour
{
    public static InteractionManager Instance { get; private set; }

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

    [SerializeField] private InteractionEvent[] allInteractionEvents;
    [SerializeField] private ImageShowInteractionEvent tvInteractionEvent;
    [SerializeField] private ImageShowInteractionEvent kitchenWindowInteractionEvent;

    void OnEnable()
    {
        EventManager.Instance.OnSpriteChangeInteraction += SpriteChange;
        EventManager.Instance.OnImageShowInteraction += ImageShow;
        EventManager.Instance.OnSetWeather += HandleWeather;
        EventManager.Instance.OnSleepInteraction += Sleep;
        EventManager.Instance.OnWriteEssayInteraction += WriteEssay;
    }

    void OnDisable()
    {
        EventManager.Instance.OnSpriteChangeInteraction -= SpriteChange;
        EventManager.Instance.OnImageShowInteraction -= ImageShow;
        EventManager.Instance.OnSetWeather -= HandleWeather;
        EventManager.Instance.OnSleepInteraction -= Sleep;
        EventManager.Instance.OnWriteEssayInteraction -= WriteEssay;
    }

    private void HandleWeather(GameManager.Weather newWeather)
    {
        // Clear
        if (newWeather == GameManager.Weather.CLEAR)
        {
            tvInteractionEvent.ChangeImage(0);
            kitchenWindowInteractionEvent.ChangeImage(0);
        }

        // Rain
        else if (newWeather == GameManager.Weather.RAIN)
        {
            tvInteractionEvent.ChangeImage(1);
            kitchenWindowInteractionEvent.ChangeImage(1);
        }

        // Null or other
        else
        {
            Debug.LogError($"InteractionManager::HandleWeather: Weather newWeather was unexpected value [{newWeather}].");
        }
    }

    private void SpriteChange(HauntableObject hauntableObject, Sprite newSprite, int newSpriteIndex)
    {
        if (newSpriteIndex != -1)
        {
            hauntableObject.ChangeSprite(newSpriteIndex);
        }

        else
        {
            hauntableObject.ChangeSprite(newSprite);
        }
    }

    private void ImageShow(Sprite newSprite)
    {
        
    }

    private void Sleep()
    {
        AudioManager.Instance.PlaySound(AudioManager.SFX.TV_RADIO);
        GameManager.Instance.SleepInBed();
    }
    
    private void WriteEssay()
    {
        AudioManager.Instance.PlaySound(AudioManager.SFX.TV_RADIO);
        GameManager.Instance.WorkOnEssay();
    }
}
