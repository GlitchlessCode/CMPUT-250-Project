using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DMSPanelController : Subscriber
{

    [Header("Event Listeners")]
    public DirectMessageGameEvent DMSent;
    public BoolGameEvent DMTabClick;

    [SerializeField] private TextMeshProUGUI textComponent;
    [SerializeField] private Image image;

    public GameObject container;
    public Transform DMPanel;
    private List<GameObject> containers = new List<GameObject>();
    private List<RectTransform> transforms = new List<RectTransform>();
    private List<float> heights = new List<float>();
    

    void Start ()
    {
        
    }

    protected override void Subscribe()
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
        LayoutRebuilder.ForceRebuildLayoutImmediate(DMPanel.GetComponent<RectTransform>());
    }

    void Update()
    {

    }

    void AddDM(DirectMessage DM)
    {

        // if (container != null && DMPanel != null)
        // { 
            GameObject instantiatedObject = Instantiate(container, DMPanel);
            textComponent = instantiatedObject.GetComponentInChildren<TextMeshProUGUI>();
            textComponent.text = DM.message;
            

            containers.Add(instantiatedObject);

            
            
            RectTransform trans = instantiatedObject.GetComponent<RectTransform>();

            Canvas.ForceUpdateCanvases();
            LayoutRebuilder.ForceRebuildLayoutImmediate(DMPanel.GetComponent<RectTransform>());
            transforms.Add(trans);
            // heights.Add(trans.rect.height);

            // foreach (GameObject con in containers)
            // {
            //     int currentIndex = containers.IndexOf(con);

            //     if (currentIndex != 0){
            //         transforms[currentIndex].anchoredPosition = new Vector2( 
            //             transforms[currentIndex].anchoredPosition.x,
            //             transforms[currentIndex].anchoredPosition.y + transforms[currentIndex-1].sizeDelta.y
            //         );

            //         Debug.Log(""+transforms[currentIndex-1].sizeDelta.y);
            //     }
            // }
            

        // }
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
