using System;
using UnityEngine;

public class DailyTask : MonoBehaviour
{
    [SerializeField] protected String taskName = "Unnamed_DailyTask";
    [SerializeField] protected String taskDescription = "Default_DailyTask_Description";
    [SerializeField] protected bool isCompleted = false;
    [SerializeField] protected HauntableObject hauntableObject;

    public String TaskName => taskName;
    public String TaskDescription => taskDescription;
    public bool IsCompleted => isCompleted;

    void OnEnable()
    {
        EventManager.Instance.OnDailyTaskCompleted += TryCompleteTask;
        EventManager.Instance.OnDayStart += (dayNumber) => { ResetTask(); };
    }

    void OnDisable()
    {
        EventManager.Instance.OnDailyTaskCompleted -= TryCompleteTask;
        EventManager.Instance.OnDayStart -= (dayNumber) => { ResetTask(); };
    }

    public HauntableObject GetHauntableObject()
    {
        return this.hauntableObject;
    }

    public void SetHauntableObject(HauntableObject hauntableObject)
    {
        this.hauntableObject = hauntableObject;
    }

    public void BecomeDailyTask()
    {
        EventManager.Instance.RaiseSetDailyTask(this);
    }

    public void TryCompleteTask(DailyTask completedTaskName)
    {
        if (completedTaskName.TaskName == this.TaskName)
        {
            Debug.Log($"Completed [{TaskName}].");
            isCompleted = true;
        }
    }

    public void ResetTask()
    {
        isCompleted = false;
    }
}
