using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DMSPanelController : Subscriber
{

    [Header("Event Listeners")]
    public DirectMessageGameEvent DMSent;

    [SerializeField] private TextMeshProUGUI textComponent;

    public GameObject container;
    public Transform DMPanel;
    private GameObject currentContainer;
    private GameObject lastContainer;
    private RectTransform lastTransform;
    private float height = 0;
    

    void Start ()
    {
        
    }

    protected override void Subscribe()
    {
        DMSent?.Subscribe(OnDMSent);
    }

    public void OnDMSent(DirectMessage DM)
    {
        AddDM();
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.A))
        {
            AddDM();
        }
    }

    void AddDM()
    {
        if (container != null && DMPanel != null)
        { 

            currentContainer = Instantiate(container,DMPanel);
            LayoutElement currentLayout = currentContainer.GetComponent<LayoutElement>();

            textComponent = currentContainer.GetComponentInChildren<TextMeshProUGUI>();
            textComponent.text = "Hello world" + height;

            lastTransform = currentContainer.GetComponent<RectTransform>();
            
            Canvas.ForceUpdateCanvases(); 
            height += lastTransform.rect.height;

            Vector2 currentPosition = lastTransform.anchoredPosition;
            if (lastContainer != null){
                lastTransform.anchoredPosition = new Vector2(currentPosition.x, currentPosition.y + height+20);
            }

            lastContainer = currentContainer;
            
            Debug.Log(height);
        }
    }


}
