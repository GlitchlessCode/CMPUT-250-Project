using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SettingsButtonController : Subscriber
{
    [Header("Buttons")]
    public Button SettingsOpenButton;
    public Button SettingsCloseButton;

    [Header("Panel")]
    public GameObject SettingsPanel;

    [Header("Audio")]
    public Audio TabSwitch;

    [Header("Event Listeners")]
    public AudioGameEvent AudioBus;
    private bool canUpdate = true;

    void Start()
    {
        SettingsOpenButton.onClick.AddListener(OnSettingsOpen);
        SettingsCloseButton.onClick.AddListener(OnSettingsClosed);
        SetupButton(SettingsOpenButton);
        SetupButton(SettingsCloseButton);
    }

    void OnSettingsOpen()
    {
        SettingsPanel.SetActive(true);
        AudioBus?.Emit(TabSwitch);
    }

    void OnSettingsClosed()
    {
        SettingsPanel.SetActive(false);
        AudioBus?.Emit(TabSwitch);
        CursorManager.Instance.Default();
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.O))
        {
            if (canUpdate && !SettingsPanel.activeSelf)
            {
                OnSettingsOpen();
                StartCoroutine(DelayAction(0.5f));
            }
            else if (canUpdate && SettingsPanel.activeSelf)
            {
                OnSettingsClosed();
                StartCoroutine(DelayAction(0.5f));
            }
        }
    }

    IEnumerator DelayAction(float time)
    {
        canUpdate = false;
        yield return new WaitForSeconds(time);
        canUpdate = true;
    }

    public void OnHover(PointerEventData _)
    {
        CursorManager.Instance.Clickable();
    }

    public void OnPointerExit(PointerEventData _)
    {
        CursorManager.Instance.Default();
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
