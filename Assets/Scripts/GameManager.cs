using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] private PlayerMovement player;

    // Non-Variable Arrays
    [SerializeField] private GameObject[] allObjects;
    [SerializeField] private Anomaly[] allAnomalies;

    // Variable Arrays 
    private List<GameObject> suspectObjects = new List<GameObject>();
    private List<Anomaly> currentAnomalies = new List<Anomaly>();

    // Day Variables
    private int currentDay = 0;
    private int failedDays = 0;
    private const int maxFailedDays = 3;

    // Materials
    [SerializeField] Material Highlighted_Material;
    [SerializeField] Material Selected_Material;

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

    public void StartDay(int day)
    {
        currentAnomalies.Clear();
        suspectObjects.Clear();

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

    public void ToggleMarkObject(GameObject obj)
    {
        if (!suspectObjects.Contains(obj))
        {
            suspectObjects.Add(obj);
            Debug.Log($"Marked {obj.name} as suspect.");
        }

        else
        {
            suspectObjects.Remove(obj);
            Debug.Log($"Unmarked {obj.name} as suspect.");
        }
    }

    private void GameOver()
    {
        Debug.LogError("Too many failed Days, restarting game.");
        currentDay = 0;
        failedDays = 0;
        StartDay(currentDay);
    }
}
