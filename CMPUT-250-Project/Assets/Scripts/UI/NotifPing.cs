using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class NotifPing : Subscriber
{
    public Image targetImage;
    public Sprite[] images;
    public float frameRate = 0.5f;
    private float timer = 0f;
    private bool active = false;
    private bool alreadyDMTab = true; // Need to adjust this based on starting state of day
    private bool dm = false;
    public float updateTime = 1f;
    private float baseTime = 0f;

    [Header("Event Listeners")]
    public DirectMessageGameEvent DMSent;
    public BoolGameEvent DMTabClick;

    public override void Subscribe()
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
        if (alreadyDMTab)
        {
            dm = false;
        }
        active = dm && !alreadyDMTab;
    }

    void Start() { }

    void Update()
    {
        targetImage = GetComponent<Image>();
        if (active)
        {
            timer += Time.deltaTime;
            baseTime += Time.deltaTime;
            if (timer >= frameRate && baseTime >= updateTime)
            {
                if (targetImage.sprite == images[0])
                {
                    targetImage.sprite = images[1];
                }
                else
                {
                    targetImage.sprite = images[0];
                }

                timer = 0f;
            }
        }
        else
        {
            baseTime = 0;
            targetImage.sprite = images[2];
        }
    }
}
