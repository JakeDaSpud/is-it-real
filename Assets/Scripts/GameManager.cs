using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] private GameObject player;
    [SerializeField] public bool InHighlightMode = false;

    // Non-Variable Arrays
    [SerializeField] private GameObject[] allObjects;
    [SerializeField] private Anomaly[] allAnomalies;

    // Variable Arrays 
    [SerializeField] private HauntableObject[] suspectObjects = new HauntableObject[3];
    [SerializeField] private List<Anomaly> currentAnomalies = new List<Anomaly>();

    // Day Variables
    private int currentDay = 0;
    private int failedDays = 0;
    private const int maxFailedDays = 3;

    // Colours
    [SerializeField] public Color Highlighted_Colour;
    [SerializeField] public Color Selected_Colour;

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
    }

    public void StartDay(int day)
    {
        currentAnomalies[0] = null;
        currentAnomalies[1] = null;
        currentAnomalies[2] = null;
        suspectObjects[0] = null;
        suspectObjects[1] = null;
        suspectObjects[2] = null;

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
            // - [ ] highlight proper setup
            // - [ ] select proper setup
            // - [ ] detect hover on object
            // - [ ] add AddSuspect(HaunatbleObject HO)
            // - [ ] add RemoveSuspect(HaunatbleObject HO)

            // FIXME
            // IMPORTANT: - [ ] use if (Anomaly.HauntRandom) to check!!!!!!!!!!!

            anomaly.ExecuteHaunt();

            currentAnomalies.Add(anomaly);
        }
    }

    public bool AddSuspectObject(HauntableObject obj)
    {
        if (suspectObjects[0] == null)
        {
            suspectObjects[0] = obj;
            obj.BecomeSelected();
            return true;
        }

        if (suspectObjects[1] == null)
        {
            suspectObjects[1] = obj;
            obj.BecomeSelected();
            return true;
        }

        if (suspectObjects[2] == null)
        {
            suspectObjects[2] = obj;
            obj.BecomeSelected();
            return true;
        }

        Debug.LogError($"{obj.name} couldn't be added to suspectObjects, it is full!");
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
