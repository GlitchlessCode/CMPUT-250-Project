using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class WordCounter : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private bool lengthSaved = false;
    private int savedLength = 0;
    private Coroutine wordCounter;

    [SerializeField]
    private TextMeshProUGUI textField;

    public void OnPointerEnter(PointerEventData _)
    {
        if (lengthSaved)
        {
            char plural = savedLength == 1 ? ' ' : 's';
            HoverTooltip.Instance.Type($"{savedLength} Word{plural}");
            HoverTooltip.Instance.SetIcon(HoverTooltip.Icon.Tick);
        }
        else
        {
            HoverTooltip.Instance.Type("Counting Words...");
            HoverTooltip.Instance.SetIcon(HoverTooltip.Icon.Throbber);
            wordCounter = StartCoroutine(CountWords());
        }
        HoverTooltip.Instance.Show();
    }

    public void OnPointerExit(PointerEventData _)
    {
        if (!lengthSaved)
        {
            HoverTooltip.Instance.SetText("Cancelled...");
            HoverTooltip.Instance.SetIcon(HoverTooltip.Icon.Cross);
            StopCoroutine(wordCounter);
        }
        HoverTooltip.Instance.Hide();
    }

    private IEnumerator CountWords()
    {
        yield return new WaitForSeconds(2.5f);
        lengthSaved = true;
        savedLength = (textField.text.Split(' ')).Length;
        char plural = savedLength == 1 ? ' ' : 's';
        HoverTooltip.Instance.Type($"{savedLength} Word{plural}");
        HoverTooltip.Instance.SetIcon(HoverTooltip.Icon.Tick);
    }
}
