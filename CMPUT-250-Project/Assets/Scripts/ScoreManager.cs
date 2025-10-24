using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public struct DaySummary
{
    public readonly int DayIndex;

    public List<int> Scores;
    public int TotalScore
    {
        get => Scores.Sum();
    }

    public List<float> Times;
    public float TotalTime
    {
        get => Times.Sum();
    }

    public int completedAppeals;
    public int correctAppeals;

    public DaySummary(int day)
    {
        DayIndex = day;
        Scores = new List<int>();
        Times = new List<float>();
        completedAppeals = 0;
        correctAppeals = 0;
    }
}

public class ScoreManager : Subscriber
{
    [Header("Score Calculation Details")]
    [SerializeField]
    private int SuccessScore = 100;

    [SerializeField]
    [Range(1.0f, 2.0f)]
    private float BonusScoreRatio = 1.25f;

    [SerializeField]
    [Range(0.0f, 1.0f)]
    // This is the ratio of Quota = SuccessScore * AppealCount * QuotaRatio
    //
    // We want to balance QuotaRatio and BonusScoreRatio so as to ensure that
    // Sum<i from 1 to AppealCount/2>(SuccessScore*BonusScoreRatio) < Quota
    // or in other words, that just spamming one option, getting ~50% success
    // with all the time bonuses is still less than the quota
    //
    // For these defaults, a bonus of 30% with a quota of 75% means that
    // No Bonuses: 75% correct to pass
    // Half Bonuses: 67% correct to pass
    // Full Bonuses: 60% correct to pass
    private float QuotaRatio = 0.75f;

    [SerializeField]
    private float perfectAppealTime = 5;

    [SerializeField]
    private float worstAppealTime = 30;

    private float startTime;

    private List<DaySummary> gameSummary = new List<DaySummary>();
    private DaySummary? currentDay;

    [Header("Event Listeners")]
    public IntGameEvent DayStart;
    public UnitGameEvent DayFinished;
    public UnitGameEvent CurrentDaySummaryRequest;
    public BoolGameEvent AfterAppeal;
    public UserEntryGameEvent UserLoaded;

    [Header("Events")]
    public DaySummaryGameEvent DisplayCurrentDaySummary;

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    public override void Subscribe()
    {
        AfterAppeal?.Subscribe(OnAfterAppeal);
        UserLoaded?.Subscribe(OnUserLoaded);
        DayStart?.Subscribe(OnDayStart);
        DayFinished?.Subscribe(OnDayFinished);
        CurrentDaySummaryRequest?.Subscribe(OnCurrentDaySummaryRequest);
    }

    private void OnAfterAppeal(bool accuracy)
    {
        float totalTime = Time.realtimeSinceStartup - startTime;
        if (currentDay != null)
        {
            DaySummary current = currentDay.Value;
            current.Scores.Add(ComputeScore(accuracy, totalTime));
            current.completedAppeals += 1;
            if (accuracy)
            {
                current.correctAppeals += 1;
            }
            current.Times.Add(totalTime);
            // Unsure if I need this? Does c-sharp copy when extracing from currentDay.Value?
            currentDay = current;
        }
    }

    private void OnUserLoaded(UserEntry user)
    {
        startTime = Time.realtimeSinceStartup;
    }

    private int ComputeScore(bool accuracy, float time)
    {
        if (!accuracy)
        {
            return 0;
        }

        if (time <= perfectAppealTime)
        {
            return (int)(SuccessScore * BonusScoreRatio);
        }
        if (time >= worstAppealTime)
        {
            return SuccessScore;
        }
        return (int)(
            SuccessScore
            * (
                BonusScoreRatio
                - (BonusScoreRatio - 1)
                    * Math.Sqrt((time - perfectAppealTime) / (worstAppealTime - perfectAppealTime))
            )
        );
    }

    private void OnDayFinished()
    {
        if (currentDay != null)
        {
            gameSummary.Add(currentDay.Value);
        }
    }

    private void OnDayStart(int dayIndex)
    {
        currentDay = new DaySummary(dayIndex);
    }

    private void OnCurrentDaySummaryRequest()
    {
        if (currentDay != null)
        {
            DaySummary current = currentDay.Value;
            int totalScore = 0;
            foreach (DaySummary summary in gameSummary)
            {
                totalScore += summary.TotalScore;
            }
            int quota = (int)(current.completedAppeals * SuccessScore * QuotaRatio);
            bool passedQuota = current.TotalScore >= quota;
            DisplayCurrentDaySummary?.Emit((current, quota, passedQuota, totalScore));
        }
    }
}
