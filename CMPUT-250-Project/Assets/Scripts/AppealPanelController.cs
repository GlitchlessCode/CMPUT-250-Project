using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AppealPanelController : Subscriber
{
    [Header("UI")]
    public Text NameText;
    public Text DateText;
    public Text BioText;
    public Text AppealText;

    [Header("Chat")]
    public Text ChatLogText; // ← NEW
    public ScrollRect ChatScroll; // ← optional: auto-scroll

    [Header("Event Listeners")]
    public UserEntryGameEvent RefreshUserInfo;

    [Header("Events")]
    public BoolGameEvent ResolveAppeal;

    protected override void Subscribe()
    {
        RefreshUserInfo?.Subscribe(OnRefreshUserInfo);
    }

    void RefreshUI(UserEntry? userOption)
    {
        if (userOption == null)
        {
            NameText.text = "No appeals loaded";
            DateText.text = BioText.text = AppealText.text = "";
            if (ChatLogText)
                ChatLogText.text = "";
            return;
        }

        var user = userOption.Value;
        NameText.text = user.name;
        DateText.text = user.date;
        BioText.text = user.bio;
        AppealText.text = user.appeal_message;

        // Build chat log from messages[]
        if (ChatLogText)
        {
            var msgs = user.messages;
            ChatLogText.text =
                (msgs != null && msgs.Length > 0)
                    ? string.Join("\n\n", msgs) // blank line between messages
                    : "";

            // optional: scroll to bottom after layout updates
            if (ChatScroll)
            {
                Canvas.ForceUpdateCanvases();
                ChatScroll.verticalNormalizedPosition = 0f; // 0 = bottom, 1 = top
            }
        }
    }

    void OnRefreshUserInfo(UserEntry user)
    {
        RefreshUI(user);
    }

    public void OnDecision(bool decision)
    {
        ResolveAppeal?.Emit(decision);
    }
}
