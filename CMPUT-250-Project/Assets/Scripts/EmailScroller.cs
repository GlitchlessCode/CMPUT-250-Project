using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class EmailScroller : Subscriber
{
    private TextMeshProUGUI textComponent;
    public GameObject container;
    public RectTransform content;
    public Transform Panel;
    public float scrollSpeed = 5f;
    private List<GameObject> containers = new List<GameObject>();
    private List<RectTransform> transforms = new List<RectTransform>();


    public void updateMessages(string msg)
    {
        GameObject instantiatedObject = Instantiate(container, Panel);
        textComponent = instantiatedObject.GetComponentInChildren<TextMeshProUGUI>();
        textComponent.text = msg;

        containers.Add(instantiatedObject);

        RectTransform trans = instantiatedObject.GetComponent<RectTransform>();

        Canvas.ForceUpdateCanvases();
        LayoutRebuilder.ForceRebuildLayoutImmediate(Panel.GetComponent<RectTransform>());
        transforms.Add(trans);
    }

        public void updateObject(GameObject thing)
    {
        GameObject instantiatedObject = Instantiate(thing, Panel);

        RectTransform trans = instantiatedObject.GetComponent<RectTransform>();

        Canvas.ForceUpdateCanvases();
        LayoutRebuilder.ForceRebuildLayoutImmediate(Panel.GetComponent<RectTransform>());
        transforms.Add(trans);
    }

}