using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Win : Subscriber
{
    [Header("Scroll")]
    public EmailScroller email;
    public List<GameObject> emailObjects;
    public GameObject button;

    void Start()
    {
        foreach (GameObject thing in emailObjects)
        {
            email.updateObject(thing);
        }

        button.transform.SetAsLastSibling();
    }

    void Update()
    {
        email.scroll();
    }
}
