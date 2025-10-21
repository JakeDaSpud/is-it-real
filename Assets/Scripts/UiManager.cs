using System.Collections;
using System.Collections.Generic;
using Microsoft.Unity.VisualStudio.Editor;
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
        EventManager.Instance.OnAnythingHighlighted += EnableInteractHint;
        EventManager.Instance.OnNothingHighlighted += DisableInteractHint;
    }
    
    void OnDisable()
    {
        EventManager.Instance.OnToggleJournal -= ToggleJournal;
        EventManager.Instance.OnAnythingHighlighted -= EnableInteractHint;
        EventManager.Instance.OnNothingHighlighted -= DisableInteractHint;
    }

    [Header("UI Elements")]
    [SerializeField] RectTransform journalMenu;
    private bool journalIsActive = false;
    [SerializeField] RectTransform journalButton;
    [SerializeField] ChoreUIManager dailyTasksNotepad;
    [SerializeField] RectTransform interactHint;
    private bool interactHintIsActive = false;

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

    private void EnableInteractHint()
    {
        interactHint.gameObject.SetActive(true);
        interactHintIsActive = true;
    }

    private void DisableInteractHint()
    {
        interactHint.gameObject.SetActive(false);
        interactHintIsActive = false;
    }
}
