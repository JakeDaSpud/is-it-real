using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] private TMP_Text tempDayText;
    [SerializeField] private TMP_Text tempTutorialText;

    [SerializeField] private GameObject player;
    [SerializeField] private int maxAnomaliesAndHighlightableObjects = 3;
    [SerializeField] private int maxDailyTasks = 2;
    [SerializeField] private Transform beginningSpawn;
    [SerializeField] private Transform bedroomSpawn;
    [SerializeField] public bool InHighlightMode = false;
    [SerializeField] public bool GamePaused = false;
    [SerializeField] public bool LoadingScreen = false;

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
    public enum Weather { NULL, CLEAR, RAIN };
    public Weather todaysWeather = Weather.NULL;

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
        EventManager.Instance.OnPause += ToggleGamePaused;
        EventManager.Instance.OnSetDailyTask += AddCurrentDailyTask;
        EventManager.Instance.OnStartLoadingScreen += InLoadingScreen;
        EventManager.Instance.OnFinishLoadingScreen += OutOfLoadingScreen;
        EventManager.Instance.OnAnimationFinished += HandleEssaySleepAnimation;
    }

    void OnDisable()
    {
        EventManager.Instance.OnPause -= ToggleGamePaused;
        EventManager.Instance.OnSetDailyTask -= AddCurrentDailyTask;
        EventManager.Instance.OnStartLoadingScreen -= InLoadingScreen;
        EventManager.Instance.OnFinishLoadingScreen -= OutOfLoadingScreen;
        EventManager.Instance.OnAnimationFinished -= HandleEssaySleepAnimation;
    }

    private void InLoadingScreen(int dayNumber)
    {
        LoadingScreen = true;
    }
    private void OutOfLoadingScreen() { LoadingScreen = false; }
    private void HandleEssaySleepAnimation(HauntableObject hauntableObject)
    {
        if (hauntableObject.objectName == "Desk" || hauntableObject.objectName == "Bed")
        {
            ToggleGamePaused();
            OutOfLoadingScreen();
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
        CheckArrayEmpty(allDailyTasks, nameof(allDailyTasks));

        StartDay(currentDay);
    }

    private void ToggleGamePaused()
    {
        if (GamePaused && InHighlightMode)
        {
            ToggleHighlightMode();
            return;
        }

        this.GamePaused = !this.GamePaused;
        
        Transform playerCameraT = player.transform.Find("Main Camera");
        Camera playerCamera = playerCameraT.GetComponent<Camera>();
        Transform darkFilterT = playerCamera.gameObject.transform.Find("DarkFilter");
        SpriteRenderer darkFilter = darkFilterT.GetComponent<SpriteRenderer>();
        Transform playerSpriteT = player.gameObject.transform.Find("Sprite and InteractBox").Find("Sprite");
        SpriteRenderer playerSprite = playerSpriteT.GetComponent<SpriteRenderer>();
        PlayerMovement playerMovement = player.GetComponent<PlayerMovement>();

        // Game is Paued
        // - Stop ENTITIES and PLAYER from MOVING
        // - Show DarkFilter
        Color zeroAlphaColor = darkFilter.color;
        if (GamePaused)
        {
            zeroAlphaColor.a = 0f;
            
            if (LoadingScreen)
            {
                darkFilter.color = zeroAlphaColor;
            }

            // Pause Player
            // Hide Player
            darkFilter.gameObject.SetActive(true);
            playerSprite.gameObject.SetActive(false);
            playerMovement.SetCanMove(false);

            //TODO
            // SHOW PAUSE MENU HERE
            if (!InHighlightMode)
            {

            }
        }

        // Game is Unpaused
        // - Let PLAYER and ENTITIES MOVE
        // - Hide DarkFilter
        else
        {
            zeroAlphaColor.a = 247 / 255f; // This is 247 out of 255 for the alpha value
            darkFilter.color = zeroAlphaColor;

            // Unpause Player
            // Unhide Player
            darkFilter.gameObject.SetActive(false);
            playerSprite.gameObject.SetActive(true);
            playerMovement.SetCanMove(true);
        }
    }

    public void ToggleHighlightMode()
    {
        // Player cannot go into Highlight Mode from regular Pause state
        if (this.GamePaused && !InHighlightMode)
        {
            return;
        }

        InHighlightMode = !InHighlightMode;
        ToggleGamePaused();

        EventManager.Instance.RaiseHighlightModeChange();
    }

    public void ResetScene(bool gameReset = false)
    {
        // Reset the Player Position
        if (currentDay == 0)
        {
            gameReset = true;
            player.GetComponent<PlayerMovement>().Respawn(beginningSpawn.position);
        }

        else
        {
            player.GetComponent<PlayerMovement>().Respawn(bedroomSpawn.position);
        }

        // Reset currentAnomalies[]
        // Reset currentTasks[]
        // Reset suspectObjects[]

        currentAnomalies.Clear();
        foreach (DailyTask dailyTask in currentDailyTasks)
        {
            dailyTask.ResetTask();
        }
        currentDailyTasks.Clear();
        currentDailyTasks.Clear();
        suspectObjects.Clear();

        // Go through all HauntableObjects
        for (int i = 0; i < allObjects.Length; i++)
        {
            allObjects[i].gameObject.SetActive(true);
            allObjects[i].ResetObject(gameReset);
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
        EventManager.Instance.RaiseDayStart(day);
        AudioManager.Instance.PlayMusic(AudioManager.Music.GAME, true);
        BeastiaryManager.Instance.UpdatePages(currentAnomalies);

        ResetScene();

        EventManager.Instance.RaiseStartLoadingScreen(day);

        GenerateWeather();
        GenerateAnomaliesForDay(day);
        GenerateDailyTasksForDay(day);
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

    private void GenerateWeather()
    {
        Array allWeathers = Enum.GetValues(typeof(Weather));

        // Randomly pick real weather
        todaysWeather = (Weather) allWeathers.GetValue( UnityEngine.Random.Range(1, allWeathers.Length) );

        // Set TV Image to real weather
        // Set Window Image to real weather
        EventManager.Instance.RaiseSetWeather(todaysWeather);
        
        Debug.Log($"Weather now set to [{todaysWeather}]");
    }

    private void GenerateAnomaliesForDay(int day)
    {
        int anomalyCount = UnityEngine.Random.Range(0, maxAnomaliesAndHighlightableObjects + 1);

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
            // Dog Cat Swap Anomaly
            if (anomaly == allAnomalies[6]) { DailyTaskManager.Instance.HandleDogAnomaly(); }
            DailyTaskManager.Instance.tasksMatter = false;
        }
    }

    private void GenerateDailyTasksForDay(int day)
    {
        if (day == 0)
        {
            // The 0th DailyTask is the Day0 Task
            allDailyTasks[0].BecomeDailyTask();
            //currentDailyTasks.Add(allDailyTasks[0]);
        }

        else
        {
            int dailyTaskCount = UnityEngine.Random.Range(1, maxDailyTasks + 1);

            Debug.Log($"dailyTaskCount = [{dailyTaskCount}]");

            List<DailyTask> pool = allDailyTasks.ToList();
            pool.RemoveAt(0); // Remove the Day0 DailyTask

            while (currentDailyTasks.Count() < dailyTaskCount)
            {
                int index = UnityEngine.Random.Range(0, pool.Count);

                if (pool.Count < 1)
                {
                    Debug.LogError("Ran out of DailyTasks to add to currentDailyTasks[].");
                    break;
                }

                DailyTask dT = pool[index];
                pool.RemoveAt(index);

                if (dT.GetHauntableObject().hauntingAnomaly != null)
                {
                    Debug.LogError($"Cannot add [{dT.TaskName}] to currentDailyTasks[], as [{dT.GetHauntableObject()}] is Haunted.");
                    continue;
                }

                else
                {
                    // Plant must be watered if there's no Anomalies today
                    if (dT.TaskName == "WaterPlant" && currentAnomalies.Count <= 0)
                    {
                        DailyTaskManager.Instance.plantShouldBeWatered = true;
                    }

                    dT.BecomeDailyTask();
                }
            }

        }
        Debug.Log($"currentDailyTasks: {currentDailyTasks}");
    }

    private void AddCurrentDailyTask(DailyTask dT)
    {
        currentDailyTasks.Add(dT);
    }

    private bool AllDailyTasksCompleted()
    {
        foreach (DailyTask dT in currentDailyTasks)
        {
            if (!dT.IsCompleted)
            {
                Debug.Log($"Today's DailyTask [{dT.TaskName}] is not finished.");
                return false;
            }
        }

        return true;
    }

    /// <summary>
    /// Try to add a HauntableObject to suspectObjects[].
    /// </summary>
    /// <param name="obj">Object to be added to suspectObjects[]</param>
    /// <returns>True if added successfully, false if it couldn't be.</returns>
    public bool AddSuspectObject(HauntableObject obj)
    {
        if (suspectObjects.Count() < maxAnomaliesAndHighlightableObjects)
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

    private bool HaveSameElements<Anomaly>(List<Anomaly> l1, List<Anomaly> l2)
    {
        if (l1 == null || l2 == null) return false;
        if (l1.Count != l2.Count) return false;

        l1.Sort((a1, a2) => a1.GetHashCode().CompareTo(a2.GetHashCode()));
        l2.Sort((a1, a2) => a1.GetHashCode().CompareTo(a2.GetHashCode()));

        for (int i = 0; i < l1.Count; i++)
        {
            if (l1[i].GetHashCode() != l2[i].GetHashCode())
            {
                Debug.LogError($"HaveSameElements: {l1[i]} is not equal to {l2[i]}");
                return false;
            }
        }

        return true;
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

            else
            {
                Debug.LogError($"{hauntableObject} was not haunted! Failing Day...");
                FailDay();
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

        if (currentDailyTasks[0].TaskName == "Day0") EventManager.Instance.RaiseDailyTaskCompleted(currentDailyTasks[0]);

        if (currentAnomalies.Count() > 0)
        {
            Debug.Log("WOE 1");
            FailDay();
        }

        else if (!AllDailyTasksCompleted() && DailyTaskManager.Instance.tasksMatter)
        {
            Debug.Log("WOE 2");
            FailDay();
        }

        else
        {
            Debug.Log("WOE 3");
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
