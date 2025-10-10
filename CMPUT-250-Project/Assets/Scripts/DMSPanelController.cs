using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DMSPanelController : Subscriber
{

    [Header("Event Listeners")]
    public DirectMessageGameEvent DMSent;


    RectTransform containerRectTrans;
    private RectTransform lastRectTrans = null;

    void Awake ()
    {
    }

    protected override void Subscribe()
    {
        DMSent?.Subscribe(OnDMSent);
    }

    public void OnDMSent(DirectMessage DM)
    {
        AddDM();
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.A))
        {

        }
    }

    void AddDM()
    {
        
    }


}
