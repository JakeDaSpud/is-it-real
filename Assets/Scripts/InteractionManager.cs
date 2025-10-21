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

        switch (hauntableObject.objectName)
        {
            case "SinkObject":
                AudioManager.Instance.PlaySound(AudioManager.SFX.WASHING_DISHES);
                break;

            case "Sink":
                AudioManager.Instance.PlaySound(AudioManager.SFX.WATER_RUNNING);
                break;

            case "WindowObject":
                if (GameManager.Instance.todaysWeather == GameManager.Weather.RAIN) AudioManager.Instance.PlaySound(AudioManager.SFX.RAIN);
                break;

            case "TV":
                AudioManager.Instance.PlaySound(AudioManager.SFX.TV_RADIO);
                break;

            case "RadioObject":
                AudioManager.Instance.PlaySound(AudioManager.SFX.TV_RADIO);
                break;

            case "LaundryBasketObject":
                AudioManager.Instance.PlaySound(AudioManager.SFX.LAUNDRY);
                break;
        }
    }

    private void ImageShow(Sprite newSprite)
    {
        switch (newSprite.name)
        {
            case "weatherStormy_0":
                if (GameManager.Instance.todaysWeather == GameManager.Weather.RAIN) AudioManager.Instance.PlaySound(AudioManager.SFX.RAIN);
                break;

            case "weatherReportSunny_0":
                if (!GameManager.Instance.GamePaused) AudioManager.Instance.PlaySound(AudioManager.SFX.TV_RADIO);
                break;

            case "weatherReportRainy_0":
                if (!GameManager.Instance.GamePaused) AudioManager.Instance.PlaySound(AudioManager.SFX.TV_RADIO);
                break;
        }
    }

    private void Sleep()
    {
        AudioManager.Instance.PlaySound(AudioManager.SFX.LIGHT_SWITCH);
        GameManager.Instance.SleepInBed();
    }
    
    private void WriteEssay()
    {
        AudioManager.Instance.PlaySound(AudioManager.SFX.LIGHT_SWITCH);
        GameManager.Instance.WorkOnEssay();
    }
}
