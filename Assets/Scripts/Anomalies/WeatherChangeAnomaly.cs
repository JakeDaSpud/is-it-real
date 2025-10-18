using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeatherChangeAnomaly : Anomaly
{
    [SerializeField] private HauntableObject windowHauntableObject;
    [SerializeField] private ImageShowInteractionEvent windowImageShowInteractionEvent;

    void OnEnable()
    {
        EventManager.Instance.OnImageShowInteraction += SwapWeatherImage;
        EventManager.Instance.OnDayStart += TurnOff;
    }
    
    private void TurnOff(int dayNumber)
    {
        EventManager.Instance.OnImageShowInteraction -= SwapWeatherImage;
        EventManager.Instance.OnDayStart -= TurnOff;

        this.gameObject.SetActive(false);
        return;
    }

    private void SwapWeatherImage(Sprite sprite)
    {
        if (GameManager.Instance.todaysWeather == GameManager.Weather.CLEAR)
        {
            windowImageShowInteractionEvent.ChangeImage(1);
        }

        else if (GameManager.Instance.todaysWeather == GameManager.Weather.RAIN)
        {
            windowImageShowInteractionEvent.ChangeImage(0);
        }

        //windowImageShowInteractionEvent.Interact(); // Force the interaction to happen again, so that the image will be changed
    }

    public override void ExecuteHaunt(HauntableObject hauntableObject)
    {
        windowHauntableObject.hauntingAnomaly = this;
    }
}
