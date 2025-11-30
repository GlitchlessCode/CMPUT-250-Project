using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Game Events/GameEvent")]
public class UnitGameEvent : GameEvent<System.ValueTuple>, IGameEvent
{
    private event Action unit_listeners;

    /// <summary>
    /// Emit the chosen event
    /// </summary>
    public void Emit()
    {
        unit_listeners?.Invoke();
    }

    /// <summary>
    /// Subscribe to the event
    /// </summary>
    public void Subscribe(Action listener)
    {
        unit_listeners += listener;
    }

    /// <summary>
    /// Unsubscribe from the event
    /// </summary>
    public void Unsubscribe(Action listener)
    {
        unit_listeners -= listener;
    }

    public new void ClearAll()
    {
        unit_listeners = null;
    }

    void OnEnable()
    {
        unit_listeners = null;
    }
}
