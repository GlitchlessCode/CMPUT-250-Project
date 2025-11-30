using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MistakesController : Subscriber
{
    [Header("Validation")]
    public Animator RedRing;
    public Text BrokenRuleText;
    private Coroutine valRoutine;
    public float MistakeTime = 2f;

    [Header("Audio")]
    public Audio MistakeSound;

    [Header("Event")]
    public AudioGameEvent AudioBus;

    [Header("Event Listeners")]
    public StringGameEvent Mistake;
    public BoolGameEvent AfterAppeal;

    public override void Subscribe()
    {
        AfterAppeal?.Subscribe(OnAfterAppeal);
        Mistake?.Subscribe(OnMistake);
    }

    private void OnAfterAppeal(bool correct)
    {
        if (correct)
            RedRing.SetBool("Show", false);
    }

    private void OnMistake(string mistakeMsg)
    {
        BrokenRuleText.text = mistakeMsg;
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
