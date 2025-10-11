using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class NotifPing : Subscriber
{
    public Image targetImage;
    public Sprite[] images;
    public float frameRate = 0.5f;
    private float timer = 0f;
    private bool active = false;
    private bool alreadyDMTab = false;
    private bool dm = false;

    [Header("Event Listeners")]
    public DirectMessageGameEvent DMSent;
    public BoolGameEvent DMTabClick;

    protected override void Subscribe()
    {
        DMSent?.Subscribe(OnDMSent);
        DMTabClick?.Subscribe(OnDMTabClick);
    }

    void OnDMSent(DirectMessage DM)
    {

        dm = !alreadyDMTab;
        active = dm && !alreadyDMTab;
    }

    void OnDMTabClick(bool value)
    {
        alreadyDMTab = value;
        if (alreadyDMTab){dm=false;}
        active = dm && !alreadyDMTab;
        Debug.Log("dm"+dm+"val"+alreadyDMTab);
        Debug.Log(""+active);
    }

    void Start()
    {

    } 

    void Update()
    {
        targetImage = GetComponent<Image>();
        if (active)
        {
            timer += Time.deltaTime;
            if (timer >= frameRate)
            {
                if (targetImage.sprite == images[0]){targetImage.sprite = images[1];}
                else {targetImage.sprite = images[0];}
                
                timer = 0f;
            }
        }
        else
        {
            targetImage.sprite = images[2];
        }
        
    }

}