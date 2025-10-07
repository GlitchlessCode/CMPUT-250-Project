using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using System.Linq;
using UnityEngine;

public class ScoreManager : Subscriber
{
    [Header("Event Listeners")]
    public BoolGameEvent AfterAppeal;
    public UserEntryGameEvent UserLoaded;

    [Header("Score Calculation Details")]
    private float perfectAppealTime = 5;
    private float worstTime = 30;

    private float timer = 0;
    private bool timerStarted = false;

    // only stored seperately for now in case we want to do a fun 
    // little user by user score summing animation at end of day
    // will probably also need to reformat this for multiple days
    private List<int> currentDayScores = new List<int>();
    private List<int> currentDayTimes = new List<int>();
    private List<bool> currentDayAccuracies = new List<bool>();

    protected override void Subscribe()
    {
        AfterAppeal?.Subscribe(OnAfterAppeal);
        UserLoaded?.Subscribe(OnUserLoaded);
    }

    protected override void AfterSubscribe()
    {

    }

    private void Update()
    {
        if (timerStarted)
        {
            timer += Time.deltaTime;
        }
    }

    private void OnAfterAppeal(bool accuracy)
    {
        currentDayScores.Add(ComputeScore(accuracy, timer));
        currentDayAccuracies.Add(accuracy);
        currentDayTimes.Add((int)timer);
        timerStarted = false;
    }

    private void OnUserLoaded(UserEntry user)
    {
        timer = (timerStarted ? 1 : 0)*timer;
        timerStarted = true;
    }

    private int ComputeScore(bool accuracy, float time)
    {
        if (timer <= perfectAppealTime)
        {
            return 200*(accuracy ? 1 : 0);
        } 
        if (timer >= worstTime) 
        {
            return 100*(accuracy ? 1 : 0);
        }
        return (100+(int)(200/(1+Math.Exp(perfectAppealTime*(timer-perfectAppealTime)/(worstTime)))))*(accuracy ? 1 : 0);
    }

    public int GetDayScore(){
        return currentDayScores.Sum();
    }

}