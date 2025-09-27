using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Game Events/GameEvent")]
public class UnitGameEvent : GameEvent<System.ValueTuple>
{
    private event Action listeners;

    /// <summary>
    /// Emit the chosen event
    /// </summary>
    public void Emit()
    {
        listeners?.Invoke();
    }

    /// <summary>
    /// Subscribe to the event
    /// </summary>
    public void Subscribe(Action listener)
    {
        listeners += listener;
    }

    /// <summary>
    /// Unsubscribe from the event
    /// </summary>
    public void Unsubscribe(Action listener)
    {
        listeners -= listener;
    }
}
