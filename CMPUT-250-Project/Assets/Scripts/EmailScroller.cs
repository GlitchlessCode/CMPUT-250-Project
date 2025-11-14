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
    private bool audioUpdate = true;
    public GameObject container;
    public RectTransform content;
    public Transform Panel;
    public Audio Scroll;
    public float scrollSpeed = 5f;
    private List<GameObject> containers = new List<GameObject>();
    private List<RectTransform> transforms = new List<RectTransform>();

    [Header("Events")]
    public AudioGameEvent AudioBus;

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

    public void scroll()
    {
        if (Input.GetKey(KeyCode.UpArrow))
        {
            content.anchoredPosition -= new Vector2(0, 5);
            if (audioUpdate)
            {
                AudioBus?.Emit(Scroll);
                StartCoroutine(DelayAudio(0.01f));
            }
        }
        else if (Input.GetKey(KeyCode.DownArrow))
        {
            content.anchoredPosition += new Vector2(0, 5);
            if (audioUpdate)
            {
                AudioBus?.Emit(Scroll);
                StartCoroutine(DelayAudio(0.01f));
            }
        }
    }

    IEnumerator DelayAudio(float time)
    {
        audioUpdate = false;
        yield return new WaitForSeconds(time);
        audioUpdate = true;
    }
}
