using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
public class ToggleUI : MonoBehaviour
{
    public CanvasGroup canvasGroup;
    private bool isVisible = false;
    public void ToggleUIElement()
    {
        isVisible = !isVisible;
        canvasGroup.alpha = isVisible ? 1f : 0f;
        canvasGroup.interactable = isVisible;
        canvasGroup.blocksRaycasts = isVisible;
    }
}