using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RulePanelController : Subscriber
{
    [Header("UI")]
    public Text RulesText;

    [Header("Event Listeners")]
    public StringGameEvent RuleText;

    protected override void Subscribe()
    {
        RuleText?.Subscribe(OnRuleText);
        OnRuleText("");
    }

    public void OnRuleText(string text)
    {
        if (text != "")
        {
            RulesText.text = text;
        }
        else
        {
            RulesText.text = "No conditions available.";
        }
    }
}
