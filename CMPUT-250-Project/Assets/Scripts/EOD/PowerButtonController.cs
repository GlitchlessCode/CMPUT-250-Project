using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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

    private void OnDayFinished()
    {
        canShutdown = true;
    }

    public void OnHover()
    {
        if (HoverAudio.clip != null)
        {
            AudioBus?.Emit(HoverAudio);
        }
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
        Debug.Log("Updating...");
        yield return new WaitForSeconds(time);
        canUpdate = true;
    }
}
