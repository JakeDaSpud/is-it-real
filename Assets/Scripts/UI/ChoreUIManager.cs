using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ChoreUIManager : MonoBehaviour
{
    public TextMeshProUGUI choreText; //The text in the UI it uses
    [SerializeField] private int choreID = 1;

    [SerializeField] private DailyTask dailyTask1;
    private bool dailyTask1Set = false;
    private TMP_Text dailyTask1Text;
    [SerializeField] private DailyTask dailyTask2;
    private bool dailyTask2Set = false;
    private TMP_Text dailyTask2Text;

    void Start()
    {
        dailyTask1Text = this.transform.Find("DailyTask1").GetComponent<TMP_Text>();
        dailyTask2Text = this.transform.Find("DailyTask2").GetComponent<TMP_Text>();
        //UpdateChoreText(choreID); // FOR TESTING PURPOSES - JOHN 
    }

    void OnEnable()
    {
        EventManager.Instance.OnSetDailyTask += SetDailyTaskText;
        EventManager.Instance.OnDailyTaskCompleted += CompleteDailyTask;
        EventManager.Instance.OnDayStart += ResetTasks;
    }

    void OnDisable()
    {
        EventManager.Instance.OnSetDailyTask -= SetDailyTaskText;
        EventManager.Instance.OnDailyTaskCompleted -= CompleteDailyTask;
        EventManager.Instance.OnDayStart -= ResetTasks;
    }

    private void SetDailyTaskText(DailyTask dT)
    {
        if (!dailyTask1Set)
        {
            dailyTask1 = dT;
            dailyTask1Text.text = dT.TaskDescription;
            dailyTask1Set = true;
        }

        else
        {
            dailyTask2 = dT;
            dailyTask2Text.text = dT.TaskDescription;
            dailyTask2Set = true;
        }
    }

    private void CompleteDailyTask(DailyTask dT)
    {
        if (dT == dailyTask1)
        {
            dailyTask1Text.text = $"<i><s>{dailyTask1Text.text}</s></i>";
        }
        
        else if (dT == dailyTask2)
        {
            dailyTask2Text.text = $"<i><s>{dailyTask2Text.text}</s></i>";
        }
    }
    
    private void ResetTasks(int dayNumber)
    {
        if (dailyTask1 == null || dailyTask2 == null) return;
        
        dailyTask1 = null;
        dailyTask1Text.text = "";
        dailyTask1Set = false;

        dailyTask2 = null;
        dailyTask2Text.text = "";
        dailyTask2Set = false;
    }

    public void UpdateChoreText(int choreID) //takes in choreID and updates text accordingly
    {
        switch (choreID)
        {
            case 1:
                choreText.text = "Make Coffee";
                break;

            case 2:
                choreText.text = "Feet The Cat";
                break;

            default:
                choreText.text = "WHAT THE **** DO YOU [little sponge]'s DO ALL DAY? NOTHING? NOTHING??? WELL I [salesman] WILL DO SOMETHING SO MAD, I WILL GO BEYOND THE TEXT LIMIT";
                break;
        }
    }
}
