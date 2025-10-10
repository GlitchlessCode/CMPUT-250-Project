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
    private float time = 0;
    private bool timerStarted = false;

    [Header("Events")]
    public UnitGameEvent DayFinished;

    // only stored seperately for now in case we want to do a fun 
    // little user by user score summing animation at end of day
    // will probably also need to reformat this for multiple days
    private List<int> currentDayScores = new List<int>();
    private List<int> currentDayTimes = new List<int>();
    private List<bool> currentDayAccuracies = new List<bool>();
    [SerializeField] private int currentDayIndex = 1;
    public int CurrentDayIndex => currentDayIndex;

    protected override void Subscribe()
    {
        AfterAppeal?.Subscribe(OnAfterAppeal);
        UserLoaded?.Subscribe(OnUserLoaded);
        DayFinished?.Subscribe(OnDayFinished);
    }

    protected override void AfterSubscribe()
    {
        // is there supposed to be something here? CMT
    }

    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(gameObject);
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
        time = timer;
        currentDayScores.Add(ComputeScore(accuracy, time));
        currentDayAccuracies.Add(accuracy);
        currentDayTimes.Add((int)time);
        timerStarted = false;
    }

    private void OnUserLoaded(UserEntry user)
    {
        timer = (timerStarted ? 1 : 0)*timer;
        timerStarted = true;
    }

    private int ComputeScore(bool accuracy, float time)
    {
        if (time <= perfectAppealTime)
        {
            return 200*(accuracy ? 1 : 0);
        } 
        if (time >= worstTime) 
        {
            return 100*(accuracy ? 1 : 0);
        }
        return (100+(int)(200/(1+Math.Exp(perfectAppealTime*(time-perfectAppealTime)/(worstTime)))))*(accuracy ? 1 : 0);
    }

    public int GetDayScore()
    {
        return currentDayScores.Sum();
    }
    
    
    [System.Serializable]
    public class DaySummary
    {
        public int dayIndex;
        public int appealsProcessed;
        public int dayScore;
        public float totalSeconds; // optional
    }

    [SerializeField] private List<DaySummary> runSummaries = new List<DaySummary>();


    public int GetAppealsProcessed() => currentDayTimes.Count;
    public int GetTotalScoreSoFar()
    {
        int sum = 0;
        foreach (var s in runSummaries) sum += s.dayScore;
        return sum;
    }
    public int GetTotalAppealsSoFar()
    {
        int sum = 0;
        foreach (var s in runSummaries) sum += s.appealsProcessed;
        return sum;
    }

    private void OnDayFinished()
    {
        var summary = new DaySummary
        {
            dayIndex = currentDayIndex,
            appealsProcessed = GetAppealsProcessed(),
            dayScore = GetDayScore(),
            totalSeconds = currentDayTimes.Sum()  // optional, since we store int seconds
        };

        runSummaries.Add(summary);
    }

    public void StartNewDay(int newDayIndex)
    {
        currentDayIndex = newDayIndex;

        // clear per-day only
        currentDayScores.Clear();
        currentDayTimes.Clear();
        currentDayAccuracies.Clear();

        timer = 0f;
        time = 0f;
        timerStarted = false;
    }


}