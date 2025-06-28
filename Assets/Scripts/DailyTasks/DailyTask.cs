using System;
using UnityEngine;

[CreateAssetMenu(fileName = "NewDailyTask", menuName = "DailyTask")]
public class DailyTask : ScriptableObject
{
    [SerializeField] protected String taskName = "Unnamed_DailyTask";
    [SerializeField] protected String taskDescription = "Default_DailyTask_Description";
    [SerializeField] protected bool isCompleted = false;

    public String TaskName => taskName;
    public String TaskDescription => taskDescription;
    public bool IsCompleted => isCompleted;

    public void CompleteTask()
    {
        isCompleted = true;
    }

    public void ResetTask()
    {
        isCompleted = false;
    }
}
