using UnityEngine;

[CreateAssetMenu(menuName = "Game Events/DaySummaryGameEvent")]
public class DaySummaryGameEvent : GameEvent<(DaySummary, int, bool, int)> { }
