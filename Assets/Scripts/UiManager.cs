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
        EventManager.Instance.OnPause += DeactivateJournal;
        EventManager.Instance.OnAnythingHighlighted += EnableInteractHint;
        EventManager.Instance.OnNothingHighlighted += DisableInteractHint;
    }
    
    void OnDisable()
    {
        EventManager.Instance.OnToggleJournal -= ToggleJournal;
        EventManager.Instance.OnPause -= DeactivateJournal;
        EventManager.Instance.OnAnythingHighlighted -= EnableInteractHint;
        EventManager.Instance.OnNothingHighlighted -= DisableInteractHint;
    }

    [Header("UI Elements")]
    [SerializeField] RectTransform journalMenu;
    [SerializeField] private bool journalIsActive = false;
    [SerializeField] public enum JournalStateRequest { TOGGLE, ACTIVATE, DEACTIVATE }
    [SerializeField] RectTransform journalButton;
    [SerializeField] ChoreUIManager dailyTasksNotepad;
    [SerializeField] RectTransform interactHint;
    private bool interactHintIsActive = false;

    public void ActivateJournal()
    {
        journalMenu.gameObject.SetActive(true);
        journalButton.gameObject.SetActive(false);
        dailyTasksNotepad.gameObject.SetActive(false);
        journalIsActive = true;
    }

    public void DeactivateJournal()
    {
        // Ignore the game's request
        if (GameManager.Instance.GamePaused) return;

        journalMenu.gameObject.SetActive(false);
        journalButton.gameObject.SetActive(true);
        dailyTasksNotepad.gameObject.SetActive(true);
        journalIsActive = false;
    }

    public void CloseJournalButton() { EventManager.Instance.RaisePause(); }

    private void ToggleJournal(JournalStateRequest requestedState)
    {   
        switch (requestedState)
        {
            case JournalStateRequest.ACTIVATE:
                ActivateJournal();
                break;

            case JournalStateRequest.DEACTIVATE:
                DeactivateJournal();
                break;

            case JournalStateRequest.TOGGLE:
                if (journalIsActive) DeactivateJournal();
                else ActivateJournal();
                break;
        }
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
