using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;


public class EndOfDayUI : Subscriber
{
    [SerializeField]
    private Text dayText;

    [SerializeField]
    private Text appealsText;

    [SerializeField]
    private Text appealsTextTitle;

    [SerializeField]
    private Text quotaText;

    [SerializeField]
    private Text quotaTextTitle;

    [SerializeField]
    private Text totalScoreText;

    [SerializeField]
    private Text totalScoreTextTitle;

    [SerializeField]
    private Button nextButton;

    [SerializeField]
    private Animator powerOnPanel;

    [Header("Audio")]
    public Audio TitleAudio;
    public Audio PlaceAudio;
    public Audio TallyAudio;
    public Audio ButtonPlaceAudio;

    [Header("Event Listeners")]
    public DaySummaryGameEvent DisplayDaySummary;

    [Header("Events")]
    public UnitGameEvent RequestDaySummary;
    public AudioGameEvent AudioBus;

    public override void Subscribe()
    {
        if (dayText)
            dayText.text = "";
        if (appealsText)
            appealsText.text = "";
        if (quotaText)
            quotaText.text = "";
        if (totalScoreText)
            totalScoreText.text = "";

        if (appealsTextTitle)
            appealsTextTitle.text = "";
        if (quotaTextTitle)
            quotaTextTitle.text = "";
        if (totalScoreTextTitle)
            totalScoreTextTitle.text = "";

        DisplayDaySummary?.Subscribe(OnDisplayDaySummary);
        powerOnPanel.gameObject.SetActive(true);
        if (nextButton)
            nextButton.gameObject.SetActive(false);
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

        if (nextButton)
            nextButton.onClick.AddListener(() => OnNextButton(summary.DayIndex, passedQuota));
        StartCoroutine(AnimatedDisplaySequence(summary, quota, totalScore));
    }

    private IEnumerator AnimatedDisplaySequence(DaySummary summary, int quota, int totalScore)
    {
        yield return new WaitForSeconds(0.4f);
        powerOnPanel.SetTrigger("PlayPowerOn");
        yield return new WaitForSeconds(1.0f);
        if (dayText)
            dayText.text = $"Day {summary.DayIndex}";
        if (TitleAudio.clip != null)
        {
            AudioBus?.Emit(TitleAudio);
        }
        yield return new WaitForSeconds(1.25f);
        if (appealsTextTitle)
            appealsTextTitle.text = "NO. OF APPEALS:";
        if (PlaceAudio.clip != null)
        {
            AudioBus?.Emit(PlaceAudio);
        }
        yield return new WaitForSeconds(0.3f);
        yield return TallyValue(appealsText, summary.correctAppeals, 1, summary.completedAppeals);
        yield return new WaitForSeconds(1.0f);
        if (quotaTextTitle)
            quotaTextTitle.text = "QUOTA:";
        if (PlaceAudio.clip != null)
        {
            AudioBus?.Emit(PlaceAudio);
        }
        yield return new WaitForSeconds(0.3f);
        yield return TallyValue(quotaText, summary.TotalScore, 17, quota);
        yield return new WaitForSeconds(1.0f);
        if (totalScoreTextTitle)
            totalScoreTextTitle.text = "SCORE:";
        if (PlaceAudio.clip != null)
        {
            AudioBus?.Emit(PlaceAudio);
        }
        yield return new WaitForSeconds(0.3f);
        yield return TallyValue(totalScoreText, totalScore, 37);
        yield return new WaitForSeconds(1.0f);
        if (nextButton)
            nextButton.gameObject.SetActive(true);
        if (ButtonPlaceAudio.clip != null)
        {
            AudioBus?.Emit(ButtonPlaceAudio);
        }
    }

    private IEnumerator TallyValue(Text target, int upTo, int stepSize, int outOf)
    {
        bool pitchOverride = TallyAudio.pitchOverride.active;
        TallyAudio.pitchOverride.active = true;
        target.text = $"/ {outOf}";
        yield return new WaitForSeconds(0.1f);

        int idx = 0;
        for (int i = 0; i < upTo; i = Math.Min(i + stepSize, upTo))
        {
            target.text = $"{i} / {outOf}";
            if (TallyAudio.clip != null)
            {
                TallyAudio.pitchOverride.value = (-1.0f / ((float)(idx++) * 0.03f + 1.0f)) + 2.0f;
                AudioBus?.Emit(TallyAudio);
            }
            yield return new WaitForSeconds(0.03f);
        }
        TallyAudio.pitchOverride.active = pitchOverride;

        target.text = $"{upTo} / {outOf}";
    }

    private IEnumerator TallyValue(Text target, int upTo, int stepSize)
    {
        bool pitchOverride = TallyAudio.pitchOverride.active;
        TallyAudio.pitchOverride.active = true;

        int idx = 0;
        for (int i = 0; i < upTo; i = Math.Min(i + stepSize, upTo))
        {
            target.text = $"{i}";
            if (TallyAudio.clip != null)
            {
                TallyAudio.pitchOverride.value = (-1.0f / ((float)(idx++) * 0.03f + 1.0f)) + 2.0f;
                AudioBus?.Emit(TallyAudio);
            }
            yield return new WaitForSeconds(0.03f);
        }
        TallyAudio.pitchOverride.active = pitchOverride;

        target.text = $"{upTo}";
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
    private void Update()
    {
        if (nextButton != null && nextButton.gameObject.activeSelf && nextButton.interactable)
        {
            if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
            {
                StartCoroutine(SimulateNextButtonPress());
            }
        }
    }
    private IEnumerator SimulateNextButtonPress()
    {
        var btn = nextButton;
        if (btn == null) yield break;

        // Ensure there’s an EventSystem (if your scene doesn’t already have one)
        if (EventSystem.current == null)
        {
            new GameObject("EventSystem", typeof(EventSystem), typeof(StandaloneInputModule));
        }

        // Create a fake pointer event to simulate a mouse click
        var ped = new PointerEventData(EventSystem.current)
        {
            button = PointerEventData.InputButton.Left,
            clickCount = 1
        };

        // Trigger the button's "pressed" visual state
        ExecuteEvents.Execute(btn.gameObject, ped, ExecuteEvents.pointerDownHandler);

        // Wait briefly so the pressed sprite is visible
        yield return new WaitForSeconds(0.1f);

        // Release and invoke the click
        ExecuteEvents.Execute(btn.gameObject, ped, ExecuteEvents.pointerUpHandler);
        btn.onClick.Invoke();
    }

}
