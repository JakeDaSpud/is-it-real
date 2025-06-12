using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] private PlayerMovement player;

    // Non-Variable Arrays
    [SerializeField] private GameObject[] allObjects;
    [SerializeField] private GameObject[] allAnomalies;
    //[SerializeField] private Anomaly[] allAnomalies;

    // To-Do
    // Uncomment these newer Variables when you create the Anomaly class

    // Variable Arrays 
    private List<GameObject> suspectObjects = new List<GameObject>();
    private List<GameObject> currentAnomalies = new List<GameObject>();
    //private List<Anomaly> currentAnomalies = new List<Anomaly>();

    // Day Variables
    private int currentDay = 0;
    private int failedDays = 0;
    private const int maxFailedDays = 3;

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

        for (int i = 0; i < anomalyCount; i++)
        {
            //Anomaly anomaly = allAnomalies[UnityEngine.Random.Range(0, allAnomalies.Length)]; 
            var anomaly = allAnomalies[UnityEngine.Random.Range(0, allAnomalies.Length)];

            //anomaly.ExecuteHaunt();

            currentAnomalies.Add(anomaly);
        }
    }

    public void MarkObject(GameObject obj)
    {
        if (!suspectObjects.Contains(obj))
        {
            suspectObjects.Add(obj);
            Debug.Log($"Marked {obj.name} as suspect.");
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
