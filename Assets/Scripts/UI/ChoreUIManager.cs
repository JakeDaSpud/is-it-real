using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ChoreUIManager : MonoBehaviour
{
    public TextMeshProUGUI choreText; //The text in the UI it uses
    [SerializeField] private int choreID = 1;

    void Start()
    {
        UpdateChoreText(choreID); // FOR TESTING PURPOSES - JOHN 
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
