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
    private List<GameObject> containers = new List<GameObject>();
    private List<RectTransform> transforms = new List<RectTransform>();
    private float height = 0;
    private float lastHeight =0;

    int o = 1;
    

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
            GameObject instantiatedObject = Instantiate(container, DMPanel);
            textComponent = instantiatedObject.GetComponentInChildren<TextMeshProUGUI>();
            textComponent.text = ""+o;

            containers.Add(instantiatedObject);
            
            transforms.Add(instantiatedObject.GetComponent<RectTransform>());

            foreach (GameObject con in containers)
            {
                height = 0;
                int currentIndex = containers.IndexOf(con); 
                Canvas.ForceUpdateCanvases();
                 
                 for (int i = 0; i <= currentIndex; i++)
                 {
                    height += transforms[currentIndex].rect.height;

                    Vector2 currentPosition = transforms[currentIndex].anchoredPosition;

                    currentPosition = new Vector2(currentPosition.x, currentPosition.y - height + 20);

                    Debug.Log(""+height);

                    transforms[i].anchoredPosition = currentPosition;

                    lastHeight = transforms[currentIndex].rect.height;

                 }

                if (transforms.Count == 1)
                {
                    Vector2 currentPosition = transforms[0].anchoredPosition;
                    currentPosition = new Vector2(currentPosition.x, currentPosition.y+height - 20);
                    transforms[0].anchoredPosition = currentPosition;
                    
                }

                for (int i = 0; i < transforms.Count-1; i++)
                {
                    Vector2 currentPosition = transforms[i].anchoredPosition;
                    currentPosition = new Vector2(currentPosition.x, currentPosition.y+height-lastHeight-20);
                    transforms[i].anchoredPosition = currentPosition;
                }

                
            }
            o++;

            

        }
    }


}
