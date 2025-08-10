using System;
using UnityEngine;

[CreateAssetMenu(fileName = "NewDailyTask", menuName = "DailyTask")]
public class DailyTask : MonoBehaviour
{
    [SerializeField] protected String taskName = "Unnamed_DailyTask";
    [SerializeField] protected String taskDescription = "Default_DailyTask_Description";
    [SerializeField] protected bool isCompleted = false;

    public String TaskName => taskName;
    public String TaskDescription => taskDescription;
    public bool IsCompleted => isCompleted;

    void OnEnable()
    {
        EventManager.Instance.OnDailyTaskCompleted += TryCompleteTask;
    }

    void OnDisable()
    {
        EventManager.Instance.OnDailyTaskCompleted -= TryCompleteTask;
    }

    public void BecomeDailyTask()
    {
        EventManager.Instance.RaiseSetDailyTask(this);
    }

    public void TryCompleteTask(String completedTaskName)
    {
        if (completedTaskName == this.TaskName)
        {
            isCompleted = true;
        }
    }

    public void ResetTask()
    {
        isCompleted = false;
    }
}
