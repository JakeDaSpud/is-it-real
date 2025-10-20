using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class UiManager : MonoBehaviour
{
    public static UiManager Instance { get; private set; }

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
        EventManager.Instance.OnToggleJournal += ToggleJournal;
    }
    
    void OnDisable()
    {
        EventManager.Instance.OnToggleJournal -= ToggleJournal;
    }

    [Header("UI Elements")]
    [SerializeField] RectTransform journalMenu;
    [SerializeField] RectTransform journalButton;
    [SerializeField] ChoreUIManager dailyTasksNotepad;
    private bool journalIsActive = false;

    private void ToggleJournal()
    {
        if (journalIsActive)
        {
            journalMenu.gameObject.SetActive(false);
            journalButton.gameObject.SetActive(true);
            dailyTasksNotepad.gameObject.SetActive(true);
        }

        else
        {
            journalMenu.gameObject.SetActive(true);
            journalButton.gameObject.SetActive(false);
            dailyTasksNotepad.gameObject.SetActive(false);
        }

        journalIsActive = !journalIsActive;
    }

}
