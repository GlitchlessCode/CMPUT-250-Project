using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PowerButtonController : Subscriber
{
    public Animator PowerOffPanel;

    [Header("Audio")]
    public Audio ShutdownAudio;
    public Audio HoverAudio;
    public Audio ClickInvalidAudio;
    public Audio ClickValidAudio;

    [Header("Buttons")]
    public Button PowerButton;

    [Header("Button Animation")] //EOD Power button animator
    public Animator PowerButtonAnimator;

    [Header("Delay Time")]
    public float DelayTime = 0.5f;

    [Header("Events")]
    public AudioGameEvent AudioBus;

    [Header("Event Listeners")]
    public UnitGameEvent DayFinished;

    private bool isShuttingDown = false;
    private bool canShutdown = false;

    public bool canUpdate = true;

    public override void Subscribe()
    {
        DayFinished?.Subscribe(OnDayFinished);
    }

    public override void AfterSubscribe()
    {
        SetupButton(PowerButton);

        //Ensure we start PowerButtonAnimator in idle animation
        if (PowerButtonAnimator != null)
        {
            PowerButtonAnimator.SetBool("IsFlashing", false);
        }
    }

    private void OnDayFinished()
    {
        canShutdown = true;

        //Set PowerButtonAnimator to flashing sequence
        if (PowerButtonAnimator != null)
        {
            PowerButtonAnimator.SetBool("IsFlashing", true);
        }
    }

    public void OnHover(PointerEventData _)
    {
        if (HoverAudio.clip != null)
        {
            AudioBus?.Emit(HoverAudio);
            CursorManager.Instance.Clickable();
        }
    }

    public void OnPointerExit(PointerEventData _)
    {
        CursorManager.Instance.Default();
    }

    public void OnPowerPressed()
    {
        if (canShutdown && !isShuttingDown)
        {
            if (ClickValidAudio.clip != null)
            {
                AudioBus?.Emit(ClickValidAudio);
            }
            StartCoroutine(Shutdown());

            //Stop flashing and play pressed animation
            if (PowerButtonAnimator != null)
            {
                PowerButtonAnimator.SetBool("IsFlashing", false);
                PowerButtonAnimator.ResetTrigger("Pressed");
                PowerButtonAnimator.SetTrigger("Pressed");
            }
        }
        else
        {
            if (ClickInvalidAudio.clip != null)
            {
                AudioBus?.Emit(ClickInvalidAudio);
            }
        }
    }

    public void Update()
    {
        if (Input.GetKey(KeyCode.P))
        {
            if (canUpdate)
            {
                PowerButton.onClick.Invoke();
                StartCoroutine(DelayAction(DelayTime));
            }
        }
    }

    private IEnumerator Shutdown()
    {
        isShuttingDown = true;
        yield return new WaitForSeconds(0.15f);
        if (ShutdownAudio.clip != null)
        {
            AudioBus?.Emit(ShutdownAudio);
        }
        PowerOffPanel.gameObject.SetActive(true);
        PowerOffPanel.SetTrigger("PlayPowerOff");
    }

    private IEnumerator DelayAction(float time)
    {
        canUpdate = false;
        yield return new WaitForSeconds(time);
        canUpdate = true;
    }

    private void SetupButton(Button button)
    {
        button.TryGetComponent<EventTrigger>(out EventTrigger trigger);
        Animator animator = button.GetComponent<Animator>();
        if (trigger == null)
        {
            trigger = button.gameObject.AddComponent<EventTrigger>();
        }
        trigger.triggers.Add(createEntry(EventTriggerType.PointerEnter, OnHover));
        trigger.triggers.Add(createEntry(EventTriggerType.PointerExit, OnPointerExit));
    }

    private EventTrigger.Entry createEntry(EventTriggerType kind, Action<PointerEventData> callback)
    {
        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.callback.AddListener((data) => callback((PointerEventData)data));
        entry.eventID = kind;
        return entry;
    }
}
