using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

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

    [SerializeField] private TextMeshProUGUI textComponent;
    public GameObject container;
    public Transform Panel;
    private List<GameObject> containers = new List<GameObject>();
    private List<RectTransform> transforms = new List<RectTransform>();

    [Header("Sounds")]
    public Audio HoverSound;
    public Audio PressSound;

    [Header("Delay Time")]
    public float delayTime = 1f;

    private bool canUpdate = true;

    [Header("Event Listeners")]
    public UserEntryGameEvent RefreshUserInfo;

    [Header("Events")]
    public BoolGameEvent ResolveAppeal;
    public UnitGameEvent RequestUser;
    public AudioGameEvent SoundBus;

    protected override void Subscribe()
    {
        RefreshUserInfo?.Subscribe(OnRefreshUserInfo);
    }

    void OnEnable()
    {
        RefreshUI(null);
        RequestUser?.Emit();
        canUpdate = true;
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

        // // Build chat log from messages[]
        // if (ChatLogText)
        // {
        //     var msgs = user.messages;
        //     ChatLogText.text =
        //         (msgs != null && msgs.Length > 0)
        //             ? string.Join("\n\n", msgs) // blank line between messages
        //             : "";

        //     // optional: scroll to bottom after layout updates
        //     if (ChatScroll)
        //     {
        //         Canvas.ForceUpdateCanvases();
        //         ChatScroll.verticalNormalizedPosition = 0f; // 0 = bottom, 1 = top
        //     }
        // }

        updateMessages(userOption);
        
    }

    void updateMessages (UserEntry? user)
    {
        foreach (string msg in user.Value.messages)
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
            if (canUpdate == true)
            {
                OnDecision(false);
                StartCoroutine(DelayAction(delayTime));
            }
        }
        if (Input.GetKey(KeyCode.D))
        {
            if (canUpdate == true)
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
