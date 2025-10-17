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
    public Button AcceptButton;
    public Button DenyButton;
    public GameObject appealPanel; 

    [Header("Chat")]
    public Text ChatLogText; // ← NEW
    public ScrollRect ChatScroll; // ← optional: auto-scroll

    [Header("Sounds")]
    public Audio HoverSound;
    public Audio PressSound;

    [Header("Delay Time")]
    public float delayTime = 1f;

    private bool canUpdate = false;

    [Header("Event Listeners")]
    public UserEntryGameEvent RefreshUserInfo;
    public GameObjectGameEvent AppealPanelActive;

    [Header("Events")]
    public BoolGameEvent ResolveAppeal;
    public UnitGameEvent RequestUser;
    public AudioGameEvent SoundBus;

    protected override void Subscribe()
    {
        RefreshUserInfo?.Subscribe(OnRefreshUserInfo);
        AppealPanelActive?.Subscribe(OnAppealPanelActive);
    }

    void OnEnable()
    {
        RefreshUI(null);
        RequestUser?.Emit();
        canUpdate = true;
    }

    void OnAppealPanelActive(GameObject appealPanel){
        if (appealPanel.activeSelf == true){
            AcceptButton.enabled = true;
            DenyButton.enabled = true;
        }
        else{
            AcceptButton.enabled = false;
            DenyButton.enabled = false;
        }
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

        if (PressSound.clip != null)
        {
            SoundBus?.Emit(PressSound);
        }
    }

    public void OnHover()
    {
        if (HoverSound.clip != null)
        {
            SoundBus?.Emit(HoverSound);
        }
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.A))
        {
            if (canUpdate == true && AcceptButton.enabled)
            {
                OnDecision(false);
                StartCoroutine(DelayAction(delayTime));
            }
        }
        if (Input.GetKey(KeyCode.D))
        {
            if (canUpdate == true && DenyButton.enabled)
            {
                OnDecision(false);
                StartCoroutine(DelayAction(delayTime));
            }
        }
    }

    IEnumerator DelayAction(float time)
    {
        canUpdate = false;
        Debug.Log("Updating...");
        yield return new WaitForSeconds(time);
        canUpdate = true;
    }
}
