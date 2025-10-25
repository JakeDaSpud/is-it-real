using System;
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
        //DontDestroyOnLoad(gameObject);
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
    [SerializeField] private HauntableObject dogCouch;
    [SerializeField] private CatState catState = CatState.NULL;
    private enum CatState { NULL, LEFT, MIDDLE, RIGHT, TOP, DOG };

    [Header("Plant Logic")]
    [SerializeField] private HauntableObject plant;
    [SerializeField] private PlantState plantState = PlantState.NULL;
    private enum PlantState { NULL, NORMAL, GROWN, DEAD };
    [SerializeField] public bool plantShouldBeWatered = false;
    [SerializeField] private bool plantShouldHaveBeenWateredYesterday = false;
    [SerializeField] private bool plantWatered = false;
    [SerializeField] private bool plantWateredYesterday = false;

    [Header("Tea/Cookies Logic")]
    [SerializeField] private HauntableObject kitchenTable;
    [SerializeField] private TableState tableState = TableState.NULL;
    private enum TableState { NULL, EMPTY, TEA, COOKIES, TEA_AND_COOKIES };

    private void NewDayStart(int dayNumber)
    {
        tasksMatter = true;

        plantShouldHaveBeenWateredYesterday = plantShouldBeWatered;
        plantWateredYesterday = plantWatered;

        HandlePlant();

        plantWatered = false;
        plantShouldBeWatered = false;

        HandleKitchenTable(TableState.EMPTY); // Empty the Table

        HandleCatPosition();
        
        // Day 0 
        if (dayNumber == 0)
        {
            tasksMatter = false;
            HandleCatPosition(CatState.LEFT);
            plantState = PlantState.NORMAL;
            plantWatered = false;
            plantWateredYesterday = false;
            plantShouldBeWatered = false;
            plantShouldHaveBeenWateredYesterday = false;
        }
    }

    private void HandleDailyTask(DailyTask dT)
    {
        // Check which type it is
        switch (dT.TaskName)
        {
            case "BakeCookies":
                HandleKitchenTable(TableState.COOKIES);
                break;

            case "BrewTea":
                HandleKitchenTable(TableState.TEA);
                break;

            case "WaterPlant":
                plantWatered = true;
                break;

            default:
                Debug.Log($"HandleDailyTask() doesn't account for [{dT.TaskName}].");
                break;
        }
    }

    public void HandleDogAnomaly()
    {
        HandleCatPosition(CatState.DOG);
    }

    private void HandleCatPosition(CatState requestedState = CatState.NULL)
    {
        if (requestedState != CatState.DOG)
        {
            catTree.canBeInteracted = true;
            dogCouch.canBeInteracted = false;
        }

        if (requestedState == CatState.NULL)
        {
            Array catStates = Enum.GetValues(typeof(CatState));

            // Randomly pick position
            catState = (CatState)catStates.GetValue(UnityEngine.Random.Range(1, catStates.Length - 1)); // (1 cuts off CatState.NULL, Length-1 cuts off CatState.DOG)
            requestedState = catState;
        }

        // Cat Sprite Indices
        // Left == 0
        // Dog == 1
        // Top == 2
        // Right == 3
        // Middle == 4

        switch (requestedState)
        {
            case CatState.LEFT:
                catState = CatState.LEFT;
                catTree.ChangeSprite(0);
                break;

            case CatState.DOG:
                catState = CatState.DOG;
                catTree.ChangeSprite(1);
                catTree.canBeInteracted = false;
                dogCouch.canBeInteracted = true;
                break;

            case CatState.TOP:
                catState = CatState.TOP;
                catTree.ChangeSprite(2);
                break;

            case CatState.RIGHT:
                catState = CatState.RIGHT;
                catTree.ChangeSprite(3);
                break;

            case CatState.MIDDLE:
                catState = CatState.MIDDLE;
                catTree.ChangeSprite(4);
                break;
        }
    }

    private void HandlePlant()
    {
        // Plant stays in the same state
        if (!plantShouldHaveBeenWateredYesterday) return;

        // Sprite Indices
        // NORMAL == 0
        // GROWN == 1
        // DEAD == 2

        // Plant SHOULD be watered from here to grow
        switch (plantState)
        {
            case PlantState.NORMAL:
                if (plantWateredYesterday)
                {
                    plantState = PlantState.GROWN;
                    plant.ChangeSprite(1);
                }
                else
                {
                    plantState = PlantState.DEAD;
                    plant.ChangeSprite(2);
                }
                break;

            case PlantState.GROWN:
                if (!plantWateredYesterday)
                {
                    plantState = PlantState.NORMAL;
                    plant.ChangeSprite(0);
                }
                break;

            case PlantState.DEAD:
                if (plantWateredYesterday)
                {
                    plantState = PlantState.NORMAL;
                    plant.ChangeSprite(0);
                }
                break;
        }
    }

    private void HandleKitchenTable(TableState requestedState)
    {
        // Maxed out, ignore the request
        if (tableState == TableState.TEA_AND_COOKIES && requestedState != TableState.EMPTY) return;

        // Sprite Indices
        // 0 == EMPTY
        // 1 == TEA
        // 2 == COOKIES
        // 3 == TEA_AND_COOKIES

        switch (requestedState)
        {
            // Table SHOULD be empty
            case TableState.EMPTY:
                tableState = TableState.EMPTY;
                kitchenTable.ChangeSprite(0);
                break;

            // Add Tea to the Table
            case TableState.TEA:
                if (tableState == TableState.TEA) return;

                if (tableState == TableState.EMPTY)
                {
                    tableState = TableState.TEA;
                    kitchenTable.ChangeSprite(1);
                }

                else if (tableState == TableState.COOKIES)
                {
                    tableState = TableState.TEA_AND_COOKIES;
                    kitchenTable.ChangeSprite(3);
                }

                break;
                
            // Add Cookies to the Table
            case TableState.COOKIES:
                if (tableState == TableState.COOKIES) return;

                if (tableState == TableState.EMPTY)
                {
                    tableState = TableState.COOKIES;
                    kitchenTable.ChangeSprite(2);
                }

                else if (tableState == TableState.TEA)
                {
                    tableState = TableState.TEA_AND_COOKIES;
                    kitchenTable.ChangeSprite(3);
                }

                break;
        }
    }
    
}
