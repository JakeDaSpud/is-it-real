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

    // State Events
    public event Action OnGameStart;
    public event Action OnGameOver;
    public event Action OnHighlightModeChange;

    // Player Events
    public event Action OnPlayerDeath;
    public event Action OnPlayerLeftClick;
    public event Action<HauntableObject> OnHauntableObjectHighlighted;
    public event Action<HauntableObject> OnHauntableObjectSelected;

    // Anomaly Events
    public event Action<Anomaly[]> OnAnomaliesRevealed;

    // DailyTask Events
    public event Action<String> OnDailyTaskCompleted;

    // Raising Functions
    public void RaiseDayStart(int dayNumber) { OnDayStart?.Invoke(dayNumber); }
    public void RaiseDaySucceed() { OnDaySucceed?.Invoke(); }
    public void RaiseDayFail() { OnDayFail?.Invoke(); }
    
    public void RaiseGameStart() { OnGameStart?.Invoke(); }
    public void RaiseGameOver() { OnGameOver?.Invoke(); }
    public void RaiseHighlightModeChange() { Debug.Log("Raised OnHighlightModeChange."); OnHighlightModeChange?.Invoke(); }

    public void RaisePlayerDeath() { OnPlayerDeath?.Invoke(); }
    public void RaisePlayerLeftClick() { Debug.Log("Raised OnPlayerLeftClick."); OnPlayerLeftClick?.Invoke(); }
    public void RaiseHauntableObjectHighlighted(HauntableObject obj) { Debug.Log($"Raised [{obj.name}] OnHauntableObjectHighlighted."); OnHauntableObjectHighlighted?.Invoke(obj); }
    public void RaiseHauntableObjectSelected(HauntableObject obj) { Debug.Log($"Raised [{obj.name}] OnHauntableObjectSelected."); OnHauntableObjectSelected?.Invoke(obj); }
    
    public void RaiseAnomaliesRevealed(Anomaly[] anomalies) { OnAnomaliesRevealed?.Invoke(anomalies); }

    public void RaiseDailyTaskCompleted(String dailyTaskName) { Debug.Log($"Raised [{dailyTaskName}] OnDailyTaskCompleted."); OnDailyTaskCompleted?.Invoke(dailyTaskName); }
}
