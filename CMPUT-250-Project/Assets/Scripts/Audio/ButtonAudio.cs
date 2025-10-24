using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonAudio : MonoBehaviour
{
    public Audio Hover;
    public Audio ClickStart;
    public Audio ClickRelease;

    [Header("Events")]
    public AudioGameEvent AudioBus;

    void Awake()
    {
        gameObject.TryGetComponent(out EventTrigger trigger);
        if (trigger == null)
        {
            trigger = gameObject.AddComponent<EventTrigger>();
        }
        if (Hover.clip != null)
        {
            trigger.triggers.Add(createEntry(EventTriggerType.PointerEnter, OnHover));
        }
        if (ClickStart.clip != null)
        {
            trigger.triggers.Add(createEntry(EventTriggerType.PointerDown, OnClickStart));
        }
        if (ClickRelease.clip != null)
        {
            trigger.triggers.Add(createEntry(EventTriggerType.PointerUp, OnClickRelease));
        }
    }

    private void OnHover(PointerEventData _)
    {
        AudioBus?.Emit(Hover);
    }

    private void OnClickStart(PointerEventData _)
    {
        AudioBus?.Emit(ClickRelease);
    }

    private void OnClickRelease(PointerEventData _)
    {
        AudioBus?.Emit(ClickStart);
    }

    private EventTrigger.Entry createEntry(EventTriggerType kind, Action<PointerEventData> callback)
    {
        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.callback.AddListener((data) => callback((PointerEventData)data));
        entry.eventID = kind;
        return entry;
    }
}
