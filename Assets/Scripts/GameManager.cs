using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] private GameObject player;
    [SerializeField] public bool InHighlightMode = false;

    // Non-Variable Arrays
    [SerializeField] private GameObject[] allObjects;
    [SerializeField] private Anomaly[] allAnomalies;
    [SerializeField] private DailyTask[] allDailyTasks;

    // Variable Arrays 
    [SerializeField] private List<HauntableObject> suspectObjects = new List<HauntableObject>();
    [SerializeField] private List<Anomaly> currentAnomalies = new List<Anomaly>();
    [SerializeField] private List<DailyTask> currentDailyTasks = new List<DailyTask>();

    // Day Variables
    private int currentDay = 0;
    private int failedDays = 0;
    private const int maxFailedDays = 3;

    // Colours
    [SerializeField] public Color Highlighted_Colour;
    [SerializeField] public Color Selected_Colour;

    void OnEnable()
    {
        EventManager.Instance.OnDailyTaskCompleted += CheckDailyTaskEvent;
    }
    
    void OnDisable()
    {
        EventManager.Instance.OnDailyTaskCompleted -= CheckDailyTaskEvent;
    }

    private void CheckDailyTaskEvent(String completedTaskName)
    {
        foreach (DailyTask dT in currentDailyTasks)
        {
            if (dT.TaskName.Equals(completedTaskName))
            {
                dT.CompleteTask();
                return;
            }
        }
    }

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

    void CheckArrayEmpty(Array array, string arrayName)
    {
        if (array.Length == 0)
        {
            throw new Exception(arrayName + " is empty.");
        }
    }

    void Start()
    {
        CheckArrayEmpty(allObjects, nameof(allObjects));
        CheckArrayEmpty(allAnomalies, nameof(allAnomalies));
    }

    public void ToggleHighlightMode()
    {
        Transform playerCameraT = player.transform.Find("Main Camera");
        Camera playerCamera = playerCameraT.GetComponent<Camera>();
        Transform darkFilterT = playerCamera.gameObject.transform.Find("DarkFilter");
        SpriteRenderer darkFilter = darkFilterT.GetComponent<SpriteRenderer>();
        Transform playerSpriteT = player.gameObject.transform.Find("Sprite");
        SpriteRenderer playerSprite = playerSpriteT.GetComponent<SpriteRenderer>();
        PlayerMovement playerMovement = player.GetComponent<PlayerMovement>();

        if (InHighlightMode)
        {
            // Unpause Player
            // Unhide Player
            darkFilter.gameObject.SetActive(false);
            playerSprite.gameObject.SetActive(true);
            playerMovement.SetCanMove(true);
        }
        else
        {
            // Pause Player
            // Hide Player
            darkFilter.gameObject.SetActive(true);
            playerSprite.gameObject.SetActive(false);
            playerMovement.SetCanMove(false);
        }

        InHighlightMode = !InHighlightMode;
        EventManager.Instance.RaiseHighlightModeChange();
    }

    public void ResetScene()
    {
        // Reset currentAnomalies[]
        // Reset currentTasks[]
        // Reset suspectObjects[]

        // Go through all HauntableObjects
        // if (ho.isTemporary) => delete it
        // set .hauntingAnomaly to null

        // set all other HauntableObject SpriteRenderers to visible

        // Reset the 
    }

    public void StartDay(int day)
    {
        GenerateAnomaliesForDay(day);
        Debug.Log($"Day {day} started.");
    }

    private void GenerateAnomaliesForDay(int day)
    {
        int anomalyCount = UnityEngine.Random.Range(0, 4);
        List<Anomaly> pool = allAnomalies.ToList();

        for (int i = 0; i < anomalyCount; i++)
        {
            int index = UnityEngine.Random.Range(0, pool.Count);
            Anomaly anomaly = pool[index];
            pool.RemoveAt(index);

            // TODO
            // - [ ] uhhh refactor / fix ALL of this Anomaly adding logic

            // FIXME
            // IMPORTANT: - [ ] use if (Anomaly.HauntRandom) to check!!!!!!!!!!!

            anomaly.ExecuteHaunt();

            currentAnomalies.Add(anomaly);
        }
    }

    private void GenerateDailyTasksForDay(int day)
    {
        // TODO
        // all this yeah
    }

    /// <summary>
    /// Try to add a HauntableObject to suspectObjects[].
    /// </summary>
    /// <param name="obj">Object to be added to suspectObjects[]</param>
    /// <returns>True if added successfully, false if it couldn't be.</returns>
    public bool AddSuspectObject(HauntableObject obj)
    {
        if (suspectObjects.Count() < 3)
        {
            suspectObjects.Add(obj);
            return true;
        }

        Debug.LogError($"{obj.name} couldn't be added to suspectObjects, it is full!");
        return false;
    }

    /// <summary>
    /// Try to remove a HauntableObject from suspectObjects[].
    /// </summary>
    /// <param name="obj">Object to be removed from suspectObjects[]</param>
    /// <returns>True if removed successfully, false if it wasn't found.</returns>
    public bool RemoveSuspectObject(HauntableObject obj)
    {
        if (suspectObjects.Contains(obj))
        {
            suspectObjects.Remove(obj);
            return true;
        }

        Debug.LogError($"{obj.name} couldn't be removed from suspectObjects, it's not there!");
        return false;
    }

    private void GameOver()
    {
        Debug.LogError("Too many failed Days, restarting game.");
        currentDay = 0;
        failedDays = 0;
        StartDay(currentDay);
    }
}
