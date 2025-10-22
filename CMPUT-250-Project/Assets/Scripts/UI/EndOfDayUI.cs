using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EndOfDayUI : Subscriber
{
    [SerializeField]
    private Text dayText;

    [SerializeField]
    private Text appealsText;

    [SerializeField]
    private Text quotaText;

    [SerializeField]
    private Text totalScoreText;

    [SerializeField]
    private Button nextButton;

    [Header("Event Listeners")]
    public DaySummaryGameEvent DisplayDaySummary;

    [Header("Events")]
    public UnitGameEvent RequestDaySummary;

    public override void Subscribe()
    {
        if (dayText)
            dayText.text = "Loading...";
        if (appealsText)
            appealsText.text = "Loading...";
        if (quotaText)
            quotaText.text = "Loading...";
        if (totalScoreText)
            totalScoreText.text = "Loading...";
        DisplayDaySummary?.Subscribe(OnDisplayDaySummary);
    }

    public override void AfterSubscribe()
    {
        RequestDaySummary?.Emit();
    }

    private void OnDisplayDaySummary((DaySummary, int, bool, int) summaryTuple)
    {
        DaySummary summary;
        int quota;
        bool passedQuota;
        int totalScore;
        (summary, quota, passedQuota, totalScore) = summaryTuple;

        if (dayText)
            dayText.text = $"Day {summary.DayIndex}";
        if (appealsText)
            appealsText.text =
                $"Appeals Correct: {summary.correctAppeals} / {summary.completedAppeals}";
        if (quotaText)
            quotaText.text = $"Score: {summary.TotalScore} / {quota}";
        if (totalScoreText)
            totalScoreText.text = $"Total Score: {totalScore}";
        if (nextButton)
            nextButton.onClick.AddListener(() => OnNextButton(summary.DayIndex, passedQuota));
    }

    private void OnNextButton(int completedDay, bool passedQuota)
    {
        if (passedQuota)
        {
            SceneManager.LoadScene(completedDay + 1, LoadSceneMode.Single);
        }
        else
        {
            SceneManager.LoadScene("Failscreen");
        }
    }
}
