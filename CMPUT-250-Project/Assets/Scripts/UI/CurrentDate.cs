using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static DayDefinition;

public class CurrentDate : Subscriber
{
    [Header("UI")]
    public Text DateText;

    [Header("Event Listeners")]
    public StringGameEvent DayDate;

    protected override void Subscribe()
    {
        DayDate?.Subscribe(OnDayDate);
    }

    public void OnDayDate(string text){
        if (text != "")
        {
            DateText.text = text;
        }
        else
        {
            DateText.text = "No date available.";
        }
    }
}
