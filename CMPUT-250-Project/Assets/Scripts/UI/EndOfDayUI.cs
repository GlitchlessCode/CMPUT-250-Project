using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EndOfDayUI : MonoBehaviour
{
    [SerializeField] private Text dayScoreText;
    [SerializeField] private Text appealsText;
    [SerializeField] private Text totalScoreText;

    void Start()
    {
        var sm = FindObjectOfType<ScoreManager>();
        if (sm == null)
        {
            if (dayScoreText) dayScoreText.text = "Score: N/A";
            if (appealsText) appealsText.text = "Appeals: N/A";
            if (totalScoreText) totalScoreText.text = "Total Score: N/A";
            Debug.LogWarning("EndOfDayUI: ScoreManager not found. Is it marked DontDestroyOnLoad?");
            return;
        }

        int dayScore = sm.GetDayScore();
        int appeals = sm.GetAppealsProcessed();
        int totalScore = sm.GetTotalScoreSoFar();

        if (dayScoreText) dayScoreText.text = $"Day {sm.CurrentDayIndex} Score: {dayScore}";
        if (appealsText) appealsText.text = $"Appeals Processed: {appeals}";
        if (totalScoreText) totalScoreText.text = $"Total Score (So Far): {totalScore}";
    }
}

