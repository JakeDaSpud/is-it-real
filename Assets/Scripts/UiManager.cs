using UnityEngine;
using UnityEngine.SceneManagement;

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
        //DontDestroyOnLoad(gameObject);
    }

    void OnEnable()
    {
        EventManager.Instance.OnToggleJournal += ToggleJournal;
        EventManager.Instance.OnPause += HandlePause;
        EventManager.Instance.OnAnythingHighlighted += EnableInteractHint;
        EventManager.Instance.OnNothingHighlighted += DisableInteractHint;
    }
    
    void OnDisable()
    {
        EventManager.Instance.OnToggleJournal -= ToggleJournal;
        EventManager.Instance.OnPause -= HandlePause;
        EventManager.Instance.OnAnythingHighlighted -= EnableInteractHint;
        EventManager.Instance.OnNothingHighlighted -= DisableInteractHint;
    }

    [Header("UI Elements")]
    [SerializeField] RectTransform journalMenu;
    [SerializeField] private bool journalIsActive = false;
    [SerializeField] public enum JournalStateRequest { TOGGLE, ACTIVATE, DEACTIVATE }
    [SerializeField] RectTransform journalButton;
    [SerializeField] RectTransform pauseMenuButton;
    private bool pauseMenuIsActive = false;
    [SerializeField] ChoreUIManager dailyTasksNotepad;
    [SerializeField] RectTransform interactHint;
    private bool interactHintIsActive = false;

    public void HandlePause()
    {
        if (GameManager.Instance.GamePaused && !GameManager.Instance.LoadingScreen)
        {
            DeactivateJournal();
            SetPauseMenuButtonActive(1);
        }

        else
        {
            SetPauseMenuButtonActive(0);
            DeactivateJournal(); // Make sure journal is off when unpausing
        }
    }
    
    private void SetPauseMenuButtonActive(int option = -1)
    {
        switch (option)
        {
            //Toggle
            case -1:
                if (pauseMenuIsActive)
                {
                    pauseMenuButton.gameObject.SetActive(false);
                    pauseMenuIsActive = false;
                }

                else
                {
                    pauseMenuButton.gameObject.SetActive(true);
                    pauseMenuIsActive = true;
                }
                break;

            // Enable, true
            case 1:
                pauseMenuButton.gameObject.SetActive(true);
                pauseMenuIsActive = true;
                break;

            // Disable, false
            case 0:
                pauseMenuButton.gameObject.SetActive(false);
                pauseMenuIsActive = false;
                break;
        }
    }

    public void ActivateJournal()
    {
        SetPauseMenuButtonActive(0);
        journalMenu.gameObject.SetActive(true);
        journalButton.gameObject.SetActive(false);
        dailyTasksNotepad.gameObject.SetActive(false);
        journalIsActive = true;
        SetPauseMenuButtonActive(0);
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
                SetPauseMenuButtonActive(0);
                ActivateJournal();
                break;

            case JournalStateRequest.DEACTIVATE:
                DeactivateJournal();
                break;

            case JournalStateRequest.TOGGLE:
                if (journalIsActive) DeactivateJournal();
                else
                {
                    SetPauseMenuButtonActive(0);
                    ActivateJournal();
                }
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

    public void EnterMainMenuScene()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
