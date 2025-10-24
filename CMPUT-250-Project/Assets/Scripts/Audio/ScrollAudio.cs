using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ScrollAudio : MonoBehaviour
{
    public Audio Scroll;

    [Header("Events")]
    public AudioGameEvent AudioBus;

    void Awake()
    {
        gameObject.TryGetComponent(out EventTrigger trigger);
        if (trigger == null)
        {
            trigger = gameObject.AddComponent<EventTrigger>();
        }
        if (Scroll.clip != null)
        {
            trigger.triggers.Add(createEntry(EventTriggerType.Scroll, OnScroll));
        }
    }

    private void OnScroll(PointerEventData _)
    {
        AudioBus?.Emit(Scroll);
    }

    private EventTrigger.Entry createEntry(EventTriggerType kind, Action<PointerEventData> callback)
    {
        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.callback.AddListener((data) => callback((PointerEventData)data));
        entry.eventID = kind;
        return entry;
    }
}
