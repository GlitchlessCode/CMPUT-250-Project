using UnityEngine;

public abstract class Subscriber : MonoBehaviour
{
    protected virtual void Awake()
    {
        SubscriptionManager.Subscribe += Subscribe;
        SubscriptionManager.AfterSubscribe += AfterSubscribe;
    }

    protected virtual void Subscribe() { }

    protected virtual void AfterSubscribe() { }
}
