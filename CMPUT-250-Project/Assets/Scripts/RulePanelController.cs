using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RulePanelController : Subscriber
{
    [Header("UI")]
    public Text RulesText;

    private Validator validator;

    void OnEnable()
    {
        RefreshUI();
    }

    public void RefreshUI()
    {
        if (validator != null)
        {
            RulesText.text = validator.GetConditionText();
        }
        else
        {
            RulesText.text = "No conditions available.";
        }
    }

    void Update()
    {

    }

    public void SetValidator(Validator validator)
    {
        this.validator = validator;
        RefreshUI();
    }

}
