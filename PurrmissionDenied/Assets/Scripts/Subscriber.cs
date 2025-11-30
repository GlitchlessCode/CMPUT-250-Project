using UnityEngine;

public abstract class Subscriber : MonoBehaviour
{
    public virtual void Subscribe() { }

    public virtual void AfterSubscribe() { }
}
