using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DailyTaskManager : MonoBehaviour
{
    public static DailyTaskManager Instance { get; private set; }

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
    
    void OnEnable()
    {
        EventManager.Instance.OnDailyTaskCompleted += HandleDailyTask;
        EventManager.Instance.OnDayStart += NewDayStart;
    }

    void OnDisable()
    {
        EventManager.Instance.OnDailyTaskCompleted -= HandleDailyTask;
        EventManager.Instance.OnDayStart -= NewDayStart;
    }

    [Header("General")]
    /// If Anomalies in day, then false
    [SerializeField] public bool tasksMatter = true;

    [Header("Cat Logic")]
    [SerializeField] private HauntableObject catTree;
    [SerializeField] private CatState catState = CatState.NULL;
    private enum CatState { NULL, LEFT, MIDDLE, RIGHT, TOP, DOG };

    [Header("Plant Logic")]
    [SerializeField] private HauntableObject plant;
    [SerializeField] private PlantState plantState = PlantState.NULL;
    private enum PlantState { NULL, NORMAL, GROWN, DEAD };

    [Header("Tea/Cookies Logic")]
    [SerializeField] private HauntableObject kitchenTable;
    [SerializeField] private TableState tableState = TableState.NULL;
    private enum TableState { NULL, EMPTY, TEA, COOKIES, TEA_AND_COOKIES };

    private void NewDayStart(int dayNumber)
    {
        tasksMatter = true;

        // Day 0 
        if (dayNumber == 0)
        {
            tasksMatter = false;
            plantState = PlantState.NORMAL;
        }


    }

    private void HandleDailyTask(DailyTask dailyTask)
    {
        // Check which type it is
    }
    
}
