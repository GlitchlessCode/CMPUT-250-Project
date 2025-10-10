using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DMSPanelController : Subscriber
{

    [Header("Event Listeners")]
    public DirectMessageGameEvent DMSent;

    protected override void Subscribe()
    {
        DMSent?.Subscribe(OnDMSent);
    }

    public void OnDMSent(DirectMessage DM)
    {

    }

    void Update()
    {

    }


}
