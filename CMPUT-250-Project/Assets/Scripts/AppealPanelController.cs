using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AppealPanelController : MonoBehaviour
{
    [Header("Data")]
    public UserManager userManager;

    [Header("UI")]
    public Text nameText;
    public Text dateText;
    public Text bioText;
    public Text appealText;

    [Header("Chat")]
    public Text chatLogText; // ← NEW
    public ScrollRect chatScroll; // ← optional: auto-scroll

    [Header("Buttons")]
    public Button approveButton;
    public Button ignoreButton;

    void Awake()
    {
        approveButton.onClick.AddListener(OnDecision);
        ignoreButton.onClick.AddListener(OnDecision);
    }

    void OnEnable()
    {
        RefreshUI();
    }

    void RefreshUI()
    {
        var uOpt = userManager.GetCurrentUser();
        if (uOpt == null)
        {
            nameText.text = "No appeals loaded";
            dateText.text = bioText.text = appealText.text = "";
            if (chatLogText)
                chatLogText.text = "";
            return;
        }

        var u = uOpt.Value;
        nameText.text = u.name;
        dateText.text = u.date;
        bioText.text = u.bio;
        appealText.text = u.appeal_message;

        // Build chat log from messages[]
        if (chatLogText)
        {
            var msgs = userManager.GetCurrentUserMessagesAll();
            chatLogText.text =
                (msgs != null && msgs.Length > 0)
                    ? string.Join("\n\n", msgs) // blank line between messages
                    : "";

            // optional: scroll to bottom after layout updates
            if (chatScroll)
            {
                Canvas.ForceUpdateCanvases();
                chatScroll.verticalNormalizedPosition = 0f; // 0 = bottom, 1 = top
            }
        }
    }

    void OnDecision()
    {
        if (userManager.MoveToNextUser())
            RefreshUI();
    }
}
