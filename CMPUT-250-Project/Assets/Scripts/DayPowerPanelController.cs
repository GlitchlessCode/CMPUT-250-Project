using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DayPowerPanelController : Subscriber
{
    public Animator PowerOnPanel;

    [Header("Event Listeners")]
    public UnitGameEvent AsyncComplete;

    // Start is called before the first frame update
    public override void Subscribe()
    {
        AsyncComplete?.Subscribe(OnAsyncComplete);
        PowerOnPanel?.gameObject.SetActive(true);
    }

    // Update is called once per frame
    private void OnAsyncComplete()
    {
        PowerOnPanel?.SetTrigger("PlayPowerOn");
    }
}
