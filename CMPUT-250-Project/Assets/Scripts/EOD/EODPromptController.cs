using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EODPromptController : Subscriber   // same base class style as your other systems
{
    [SerializeField] private GameObject arrowPrompt; // your arrow or hint UI
    [SerializeField] private Button powerButton;     // your Power button

    [Header("Events")]
    public UnitGameEvent DayFinished; // assign this in Inspector

    protected override void Subscribe()
    {
        DayFinished?.Subscribe(OnDayFinished);
    }

    private void Start()
    {
        if (arrowPrompt) arrowPrompt.SetActive(false);
        if (powerButton) powerButton.interactable = false;
    }

    private void OnDayFinished()
    {
        if (arrowPrompt) arrowPrompt.SetActive(true);
        if (powerButton) powerButton.interactable = true;
    }
}
