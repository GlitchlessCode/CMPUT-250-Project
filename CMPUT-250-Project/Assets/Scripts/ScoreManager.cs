using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class ScoreManager : Subscriber
{
    [Header("Event Listeners")]
    public BoolGameEvent ResolveAppeal;
    public UserEntryGameEvent UserLoaded;

    [Header("Score Calculation Details")]
    private int score = 0;
    private float perfectAppealTime = 15;
    private float worstTime = 120;

    private float timer = 0;
    private int userScore;

    protected override void Subscribe()
    {
        ResolveAppeal?.Subscribe(OnResolveAppeal);
        UserLoaded?.Subscribe(OnUserLoaded);
    }

    protected override void AfterSubscribe()
    {

    }

    private void Update()
    {
        if (timer < worstTime)
        {
            timer += Time.deltaTime;
            // Debug.Log(timer);
        }
    }

    private void OnResolveAppeal(bool accuracy)
    {
        score += ComputeScore(accuracy, timer);
        Debug.Log(score);
    }

    private void OnUserLoaded(UserEntry user)
    {
        Debug.Log("loaded");
        timer = 0;
    }

    private int ComputeScore(bool accuracy, float time)
    {
        userScore = (accuracy ? 1 : 0);
        return userScore;
    }

}