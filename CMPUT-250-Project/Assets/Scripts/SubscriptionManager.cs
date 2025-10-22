using System;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

enum SubscriptionManagerState
{
    Unset,
    Set,
    WaitingToSet,
    WaitingToUnset,
}

public static class SubscriptionManager
{
    static SubscriptionManagerState state = SubscriptionManagerState.Unset;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void SetupCallbacks()
    {
        SceneManager.sceneLoaded += Set;
        SceneManager.sceneUnloaded += Unset;
    }

    static void Set(Scene _, LoadSceneMode __)
    {
        switch (state)
        {
            case SubscriptionManagerState.Unset:
                _setInner();
                state = SubscriptionManagerState.Set;
                break;
            case SubscriptionManagerState.Set:
                state = SubscriptionManagerState.WaitingToSet;
                break;
            case SubscriptionManagerState.WaitingToUnset:
                _setInner();
                _unsetInner();
                state = SubscriptionManagerState.Unset;
                break;
            case SubscriptionManagerState.WaitingToSet:
                break;
        }
    }

    static void _setInner()
    {
        Subscriber[] subscribers = GameObject.FindObjectsOfType<Subscriber>();
        foreach (Subscriber subscriber in subscribers)
        {
            subscriber.Subscribe();
        }
        foreach (Subscriber subscriber in subscribers)
        {
            subscriber.AfterSubscribe();
        }
    }

    static void Unset(Scene _)
    {
        switch (state)
        {
            case SubscriptionManagerState.Set:
                _unsetInner();
                state = SubscriptionManagerState.Unset;
                break;
            case SubscriptionManagerState.Unset:
                state = SubscriptionManagerState.WaitingToUnset;
                break;
            case SubscriptionManagerState.WaitingToSet:
                _unsetInner();
                _setInner();
                state = SubscriptionManagerState.Set;
                break;
            case SubscriptionManagerState.WaitingToUnset:
                break;
        }
    }

    static void _unsetInner()
    {
        IGameEvent[] event_channels = Resources
            .FindObjectsOfTypeAll<ScriptableObject>()
            .OfType<IGameEvent>()
            .ToArray();
        foreach (IGameEvent event_channel in event_channels)
        {
            event_channel.ClearAll();
        }
    }
}
