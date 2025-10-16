using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class EventManager : MonoBehaviour
{
    public static EventManager Instance { get; private set; }

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

    // Day Events
    public event Action<int> OnDayStart;
    public event Action OnDaySucceed;
    public event Action OnDayFail;
    public event Action<GameManager.Weather> OnSetWeather;

    // State Events
    public event Action OnGameStart;
    public event Action OnGameOver;
    public event Action OnHighlightModeChange;
    public event Action OnPause;

    // Player Events
    public event Action OnPlayerDeath;
    public event Action OnPlayerLeftClick;
    public event Action<float> OnPlayerMove;
    public event Action<HauntableObject> OnHauntableObjectHighlighted;
    public event Action<HauntableObject> OnHauntableObjectSelected;

    // Interaction Events
    /// <summary>
    /// Changes the HauntableObject's Sprite to the passed-in Sprite, or changes to the currentSprite[int] if not -1.
    /// </summary>
    public event Action<HauntableObject, Sprite, int> OnSpriteChangeInteraction;
    /// <summary>
    /// Pauses the game, and shows the passed-in Sprite as an Image on Screen.
    /// </summary>
    public event Action<Sprite> OnImageShowInteraction;
    /// <summary>
    /// Plays an AudioClip.
    /// </summary>
    public event Action<AudioClip> OnSoundInteraction;
    /// <summary>
    /// Call next day protocol for Sleeping.
    /// </summary>
    public event Action OnSleepInteraction;
    /// <summary>
    /// Call next day protocol for Writing Essay.
    /// </summary>
    public event Action OnWriteEssayInteraction;

    // Anomaly Events
    public event Action<Anomaly[]> OnAnomaliesRevealed;

    // DailyTask Events
    public event Action<DailyTask> OnSetDailyTask;
    public event Action<DailyTask> OnDailyTaskCompleted;

    // Raising Functions
    public void RaiseDayStart(int dayNumber) { OnDayStart?.Invoke(dayNumber); }
    public void RaiseDaySucceed() { OnDaySucceed?.Invoke(); }
    public void RaiseDayFail() { OnDayFail?.Invoke(); }
    public void RaiseSetWeather(GameManager.Weather weather) { OnSetWeather?.Invoke(weather); }
    
    public void RaiseGameStart() { OnGameStart?.Invoke(); }
    public void RaiseGameOver() { OnGameOver?.Invoke(); }
    public void RaiseHighlightModeChange() { Debug.Log("Raised OnHighlightModeChange."); OnHighlightModeChange?.Invoke(); }
    public void RaisePause() { Debug.Log("Raised OnPause"); OnPause?.Invoke(); }

    public void RaisePlayerDeath() { OnPlayerDeath?.Invoke(); }
    public void RaisePlayerLeftClick() { Debug.Log("Raised OnPlayerLeftClick."); OnPlayerLeftClick?.Invoke(); }
    public void RaisePlayerMove(float playerY) { /*Debug.Log($"Raised OnPlayerMove([{playerY}]).");*/ OnPlayerMove?.Invoke(playerY); }
    public void RaiseHauntableObjectHighlighted(HauntableObject obj) { Debug.Log($"Raised [{obj.name}] OnHauntableObjectHighlighted."); OnHauntableObjectHighlighted?.Invoke(obj); }
    public void RaiseHauntableObjectSelected(HauntableObject obj) { Debug.Log($"Raised [{obj.name}] OnHauntableObjectSelected."); OnHauntableObjectSelected?.Invoke(obj); }
    
    public void RaiseSpriteChangeInteraction(HauntableObject hauntableObject, Sprite newSprite, int newSpriteIndex) { OnSpriteChangeInteraction?.Invoke(hauntableObject, newSprite, newSpriteIndex); }
    public void RaiseImageShowInteraction(Sprite sprite) { OnImageShowInteraction?.Invoke(sprite); }
    public void RaiseSoundInteraction(AudioClip audioClip) { OnSoundInteraction?.Invoke(audioClip); }
    public void RaiseSleepInteraction() { OnSleepInteraction?.Invoke(); }
    public void RaiseWriteEssayInteraction() { OnWriteEssayInteraction?.Invoke(); }

    public void RaiseAnomaliesRevealed(Anomaly[] anomalies) { OnAnomaliesRevealed?.Invoke(anomalies); }

    public void RaiseSetDailyTask(DailyTask dailyTask) { Debug.Log($"Raised [{dailyTask.TaskName}] SetDailyTask."); OnSetDailyTask?.Invoke(dailyTask); }
    public void RaiseDailyTaskCompleted(DailyTask dailyTask) { Debug.Log($"Raised [{dailyTask.TaskName}] OnDailyTaskCompleted."); OnDailyTaskCompleted?.Invoke(dailyTask); }
}
