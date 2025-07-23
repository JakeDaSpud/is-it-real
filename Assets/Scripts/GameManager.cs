using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] private TMP_Text tempDayText;
    [SerializeField] private TMP_Text tempTutorialText;

    [SerializeField] private GameObject player;
    [SerializeField] private Transform playerSpawn;
    [SerializeField] public bool InHighlightMode = false;

    // Non-Variable Arrays
    [SerializeField] private HauntableObject[] allObjects;
    [SerializeField] private Anomaly[] allAnomalies;
    [SerializeField] private DailyTask[] allDailyTasks;

    // Variable Arrays 
    [SerializeField] private List<HauntableObject> suspectObjects = new List<HauntableObject>();
    [SerializeField] public List<Anomaly> currentAnomalies = new List<Anomaly>();
    [SerializeField] public List<HauntableObject> temporaryObjects = new List<HauntableObject>();
    [SerializeField] private List<DailyTask> currentDailyTasks = new List<DailyTask>();

    // Day Variables
    [SerializeField] private int currentDay = 0;
    [SerializeField] private int failedDays = 0;
    [SerializeField] private const int maxFailedDays = 3;
    [SerializeField] private String TEMP_DayState = "";

    // Colours
    [SerializeField] public Color Highlighted_Colour;
    [SerializeField] public Color Selected_Colour;
    [SerializeField] public Color InteractHighlighted_Colour;

    public void SetTEMPText(String str)
    {
        tempTutorialText.text = str;
    }

    public void SetTEMPDayText(String state)
    {
        tempDayText.text = "Day " + currentDay + '\n';

        if (currentDay == 7)
        {
            SetTEMPText("Congrats! You win! Press 'L' to restart.");
        }

        // If char is x, print ❌
        // If char is o, print ✅
        // If no more char, print -
        for (int i = 0; i < 7; i++)
        {
            if (i < state.Length)
            {
                char c = state[i];
                if (c == 'x')
                {
                    tempDayText.text += "X ";
                }

                else if (c == 'o')
                {
                    tempDayText.text += "O ";
                }

                else
                {
                    tempDayText.text += "- ";
                }
            }

            else
            {
                tempDayText.text += "- ";
            }
        }
    }

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

        StartDay(currentDay);
    }

    public void ToggleHighlightMode()
    {
        Transform playerCameraT = player.transform.Find("Main Camera");
        Camera playerCamera = playerCameraT.GetComponent<Camera>();
        Transform darkFilterT = playerCamera.gameObject.transform.Find("DarkFilter");
        SpriteRenderer darkFilter = darkFilterT.GetComponent<SpriteRenderer>();
        Transform playerSpriteT = player.gameObject.transform.Find("Sprite and InteractBox").Find("Sprite");
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
        // Reset the Player Position
        player.GetComponent<PlayerMovement>().Respawn(playerSpawn.position);

        // Reset currentAnomalies[]
        // Reset currentTasks[]
        // Reset suspectObjects[]

        currentAnomalies.Clear();
        currentDailyTasks.Clear();
        suspectObjects.Clear();

        // Go through all HauntableObjects
        for (int i = 0; i < allObjects.Length; i++)
        {
            allObjects[i].gameObject.SetActive(true);
            allObjects[i].ResetObject();
        }

        for (int i = temporaryObjects.Count - 1; i >= 0; i--)
        {
            temporaryObjects[i].ResetObject();
        }

        if (InHighlightMode)
        {
            ToggleHighlightMode();
        }
    }

    public void StartDay(int day)
    {
        ResetScene();
        GenerateAnomaliesForDay(day);
        Debug.Log($"Day {day} started.");
        SetTEMPDayText(TEMP_DayState);
        if (day == 0)
        {
            SetTEMPText("Learn the layout of your house, remember where everything is!\n- Work on your essay.");
        }
        else if (day < 7)
        {
            SetTEMPText("- Select every Anomaly and then sleep if you see one.\n- Work on your essay if you don't see any.");
        }
    }

    private void GenerateAnomaliesForDay(int day)
    {
        int anomalyCount = UnityEngine.Random.Range(0, 4);

        if (day == 0)
        {
            Debug.Log("Day 0 gets no Anomalies.");
            anomalyCount = 0;
            return;
        }

        Debug.Log($"anomalyCount = [{anomalyCount}]");
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

            anomaly.gameObject.SetActive(true);
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

    private bool HaveSameElements<T>(List<T> l1, List<T> l2)
    {
        if (l1 == null || l2 == null) return false;
        if (l1.Count != l2.Count) return false;

        return !l1.Except(l2).Any() && !l2.Except(l1).Any();
    }

    private List<Anomaly> GetAnomaliesFromSuspectObjects()
    {
        List<Anomaly> anomalies = new List<Anomaly>();

        foreach (HauntableObject hauntableObject in suspectObjects)
        {
            if (hauntableObject.hauntingAnomaly != null)
            {
                anomalies.Add(hauntableObject.hauntingAnomaly);
            }
        }

        return anomalies;
    }

    public void SleepInBed()
    {
        Debug.Log("SleepInBed()");
        List<Anomaly> foundAnomalies = GetAnomaliesFromSuspectObjects();

        Debug.Log($"Found anoms = [{String.Join(',', foundAnomalies)}]");
        Debug.Log($"Current anoms = [{String.Join(',', currentAnomalies)}]");

        if (currentAnomalies.Count() == 0)
        {
            Debug.Log("SIB 1");
            FailDay();
        }

        else if (HaveSameElements(foundAnomalies, currentAnomalies))
        {
            Debug.Log("SIB 2");
            SucceedDay();
        }

        else
        {
            Debug.Log("SIB 3");
            FailDay();
        }
    }

    public void WorkOnEssay()
    {
        Debug.Log("WorkOnEssay()");

        if (currentAnomalies.Count() > 0)
        {
            Debug.Log("WOE 1");
            FailDay();
        }

        else
        {
            Debug.Log("WOE 2");
            SucceedDay();
        }
    }

    private void SucceedDay()
    {
        TEMP_DayState += "o";
        Debug.Log("SUCCESS");
        currentDay++;
        StartDay(currentDay);
    }

    public void FailDay()
    {
        TEMP_DayState += "x";
        Debug.Log("FAILLLLLLL");
        currentDay++;
        failedDays++;

        if (failedDays >= maxFailedDays)
        {
            GameOver();
        }

        StartDay(currentDay);
    }

    private void GameOver()
    {
        Debug.LogError("Too many failed Days, restarting game.");
        currentDay = 0;
        failedDays = 0;
        TEMP_DayState = "";
        StartDay(currentDay);
    }

    public void FullReset()
    {
        GameOver();
    }
}
