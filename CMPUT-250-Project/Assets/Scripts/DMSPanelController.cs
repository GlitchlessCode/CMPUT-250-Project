using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DMSPanelController : Subscriber
{
    [Header("Events")]
    public AudioGameEvent AudioBus;

    [Header("Event Listeners")]
    public DirectMessageGameEvent DMSent;
    public BoolGameEvent DMTabClick;

    [SerializeField]
    private TextMeshProUGUI textComponent;

    [SerializeField]
    private Image image;

    public GameObject container;
    public RectTransform content;
    public Transform DMPanel;
    private List<GameObject> containers = new List<GameObject>();
    private List<RectTransform> transforms = new List<RectTransform>();
    private List<float> heights = new List<float>();
    public Audio Scroll;
    private bool scrollable = true;
    public float scrollSpeed = 5000000f;
    private Vector3 initialPosition;

    void Start() { }

    public override void Subscribe()
    {
        DMSent?.Subscribe(OnDMSent);
        DMTabClick?.Subscribe(OnDMTabClick);
    }

    public void OnDMSent(DirectMessage DM)
    {
        AddDM(DM);
        // killDM();
    }

    public void OnDMTabClick(bool clicked)
    {
        if (clicked)
        {
            scrollable = true;
        }
        else
        {
            scrollable = false;
        }
    }

    void Update()
    {
        LayoutRebuilder.ForceRebuildLayoutImmediate(DMPanel.GetComponent<RectTransform>());
        scroll();
    }

    void AddDM(DirectMessage DM)
    {
        GameObject instantiatedObject = Instantiate(container, DMPanel);
        textComponent = instantiatedObject.GetComponentInChildren<TextMeshProUGUI>();
        textComponent.text = DM.message;

        containers.Add(instantiatedObject);

        RectTransform trans = instantiatedObject.GetComponent<RectTransform>();

        Canvas.ForceUpdateCanvases();
        LayoutRebuilder.ForceRebuildLayoutImmediate(DMPanel.GetComponent<RectTransform>());
        transforms.Add(trans);
    }

    void scroll()
    {
        if (Input.GetKey(KeyCode.UpArrow) && scrollable)
        {
            content.anchoredPosition -= new Vector2(0, scrollSpeed);
            AudioBus?.Emit(Scroll);
        }
        else if (Input.GetKey(KeyCode.DownArrow) && scrollable)
        {
            content.anchoredPosition += new Vector2(0, scrollSpeed);
            AudioBus?.Emit(Scroll);
        }
    }

    // void killDM()
    // {
    //     foreach (GameObject con in containers)
    //     {
    //         if(con.transform.position.y > 600)
    //         {
    //             Image image = con.GetComponentInChildren<Image>();
    //             textComponent = con.GetComponentInChildren<TextMeshProUGUI>();
    //             image.enabled = false;
    //             textComponent.enabled = false;
    //         }
    //     }
    // }
}
