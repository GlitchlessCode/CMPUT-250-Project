using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SettingsPersistance : Subscriber
{
    [Header("Event Transponders")]
    public List<FloatGameEvent> AudioVolumeAdjuster;

    private List<FloatGameEvent> subscribers;
    private List<float> savedVolumes;
    private bool setup = false;

    public override void Subscribe()
    {
        int persisterCount = FindObjectsOfType<SettingsPersistance>().Count();
        if (!setup)
        {
            if (persisterCount > 1)
            {
                Destroy(gameObject);
                return;
            }
            else
            {
                DontDestroyOnLoad(gameObject);
                savedVolumes = new List<float>();
                subscribers = new List<FloatGameEvent>();
                foreach (FloatGameEvent adjuster in AudioVolumeAdjuster)
                {
                    if (adjuster != null)
                    {
                        subscribers.Add(adjuster);
                        savedVolumes.Add(0.7f);
                    }
                }
                setup = true;
            }
        }

        for (int idx = 0; idx < subscribers.Count; idx++)
        {
            subscribers[idx].Subscribe(MakeSubscriberListener(idx));
        }
    }

    public override void AfterSubscribe()
    {
        if (setup)
        {
            for (int idx = 0; idx < subscribers.Count; idx++)
            {
                subscribers[idx].Emit(savedVolumes[idx]);
            }
        }
    }

    Action<float> MakeSubscriberListener(int index)
    {
        void SubscriberListener(float volume)
        {
            savedVolumes[index] = volume;
        }
        return SubscriberListener;
    }
}
