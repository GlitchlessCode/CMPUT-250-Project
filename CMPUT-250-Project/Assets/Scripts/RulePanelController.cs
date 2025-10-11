using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RulePanelController : Subscriber
{
    [Header("UI")]
    public Text RulesText;

    void OnEnable()
    {
        RefreshUI();
    }

    void RefreshUI()
    {

        RulesText.text = "1. I HATE hate reading. Get the ones with messages longer than 50 char OUT.\n\n2. BUT BUT but. They need to try a Little... They need at least four messages!\n\n3. I won't allow ANYONE to have the word 'cat'!! 1!\n\n4. Don't bother with losers who don't have an appeal message...\n\n5. It gets boring if they repeat a letter more than 3 times. BAN\n\n6. Long names UUUGGLY,,, Keep in under 12 character else you're OUT\n\n7. REAL PPL ONLY! Your bio gotta be more than 4 words bae";

    }

    void Update()
    {

    }

}
