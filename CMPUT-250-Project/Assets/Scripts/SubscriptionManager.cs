using System;
using UnityEngine;

public static class SubscriptionManager
{
    public static Action Subscribe;
    public static Action AfterSubscribe;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void Awake()
    {
        Subscribe?.Invoke();
        AfterSubscribe?.Invoke();
    }
}
