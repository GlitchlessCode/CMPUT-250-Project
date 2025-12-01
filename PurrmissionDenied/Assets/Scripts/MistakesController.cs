using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MistakesController : Subscriber
{
    [Header("Validation")]
    public Animator RedRing;
    public Text BrokenRuleText;
    public Text NumMistakes;
    public Text RemainingAppealsText;
    private Coroutine valRoutine;
    public float MistakeTime = 4f;
    private int Mistakes = 0;
    private int RemainingAppeals = 0;

    [Header("Audio")]
    public Audio MistakeSound;

    [Header("Event")]
    public AudioGameEvent AudioBus;

    [Header("Event Listeners")]
    public StringGameEvent Mistake;
    public IntGameEvent DayStart;
    public BoolGameEvent AfterAppeal;
    public IntGameEvent GetUserAmount;

    public override void Subscribe()
    {
        AfterAppeal?.Subscribe(OnAfterAppeal);
        Mistake?.Subscribe(OnMistake);
        DayStart?.Subscribe(OnDayStart);
        GetUserAmount?.Subscribe(OnGetUserAmount);
    }

    private void OnGetUserAmount(int amount)
    {
        RemainingAppeals = amount;
        RemainingAppealsText.text = RemainingAppeals.ToString() + " Appeals Remaining";
    }

    private void OnAfterAppeal(bool correct)
    {
        RemainingAppeals -= 1;

        if (RemainingAppeals == 1)
        {
            RemainingAppealsText.text = RemainingAppeals.ToString() + " Appeal Remaining";
        } 
        else
        {
            RemainingAppealsText.text = RemainingAppeals.ToString() + " Appeals Remaining";
        } 
        
        if (correct)
            RedRing.SetBool("Show", false);
    }

    private void OnDayStart(int dayIndex)
    {
        Mistakes = 0;
    }

    private void OnMistake(string mistakeMsg)
    {
        Mistakes++;
        BrokenRuleText.text = mistakeMsg;
        NumMistakes.text = "Total Failed Appeals: " + Mistakes.ToString();
        RedRing.SetBool("Show", true);
        if (valRoutine != null)
        {
            StopCoroutine(valRoutine);
        }
        Canvas.ForceUpdateCanvases();
        BrokenRuleText.transform.parent.GetComponent<HorizontalLayoutGroup>().enabled = false;
        BrokenRuleText.transform.parent.GetComponent<HorizontalLayoutGroup>().enabled = true;

        if (MistakeSound.clip != null)
        {
            StartCoroutine(PlaySound());
        }

        valRoutine = StartCoroutine(RedRingOff());
    }

    private IEnumerator PlaySound()
    {
        yield return new WaitForSeconds(0.2f);
        AudioBus?.Emit(MistakeSound);
    }

    private IEnumerator RedRingOff()
    {
        yield return new WaitForSeconds(MistakeTime);
        RedRing.SetBool("Show", false);
    }
}
